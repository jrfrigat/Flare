using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Querio.Sql;

/// <summary>
/// Turns a query into SQL text plus the parameters it refers to. It opens no connection and knows no
/// driver: a caller hands the two halves to whatever client it already uses.
/// <para>
/// The query is validated first and the target's capabilities are checked before a single character
/// is written, so a query that cannot be rendered faithfully fails before it can be run.
/// </para>
/// <para>
/// The walk over the query is the shared one. What is left here is the SQL of it: the clause
/// keywords, the operator symbols, and the parameters values travel in rather than being spliced
/// into the text. Anything that differs between engines is asked of the dialect.
/// </para>
/// </summary>
public sealed class SqlRenderer : QueryRenderer<string>
{
    private readonly SqlDialect _dialect;
    private readonly List<SqlQueryParameter> _parameters = [];
    private readonly HashSet<string> _selectAliases = new(StringComparer.OrdinalIgnoreCase);

    private SqlRenderer(QuerySpec spec, QuerySchema schema, SqlDialect dialect)
        : base(spec, schema) => _dialect = dialect;

    /// <inheritdoc/>
    protected override string TargetName => _dialect.Name;

    /// <summary>Renders a query for the given target.</summary>
    /// <param name="spec">The query to render.</param>
    /// <param name="schema">The schema it was built against, which supplies types and physical names.</param>
    /// <param name="dialect">The target engine.</param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">The target cannot express the query.</exception>
    public static SqlRenderResult Render(QuerySpec spec, QuerySchema schema, SqlDialect dialect)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        if (dialect is null) throw new ArgumentNullException(nameof(dialect));

        return new SqlRenderer(spec, schema, dialect).Run();
    }

    private SqlRenderResult Run()
    {
        Prepare(_dialect);

        var sql = new StringBuilder();
        AppendSelect(sql);
        AppendFrom(sql);
        AppendJoins(sql);
        AppendClause(sql, " WHERE ", Spec.Where);
        AppendGroupBy(sql);
        AppendClause(sql, " HAVING ", Spec.Having);
        var hasOrderBy = AppendOrderBy(sql);
        _dialect.AppendPaging(sql, Spec.Limit, Spec.Offset, hasOrderBy);

        return new SqlRenderResult(sql.ToString(), _parameters);
    }

    private void AppendSelect(StringBuilder sql)
    {
        sql.Append("SELECT ");
        if (Spec.Distinct) sql.Append("DISTINCT ");
        sql.Append(_dialect.RenderTop(Spec.Limit, Spec.Offset));

        if (Spec.Select.Count == 0)
        {
            sql.Append('*');
            return;
        }

        var items = new List<string>(Spec.Select.Count);
        foreach (var item in Spec.Select)
        {
            var expression = SelectExpression(item);
            if (string.IsNullOrEmpty(item.Alias))
            {
                items.Add(expression);
                continue;
            }
            OutputExpressions[item.Alias!] = expression;
            OutputTypes[item.Alias!] = OutputType(item);
            _selectAliases.Add(item.Alias!);
            items.Add(expression + " AS " + _dialect.Quote(item.Alias!));
        }
        sql.Append(string.Join(", ", items));
    }

    private string SelectExpression(QuerySelect item)
    {
        if (item.Aggregate is null)
        {
            var plain = Value(item.Field, item.Call);
            return item.Truncate is null ? plain : _dialect.TruncateDate(plain, item.Truncate.Value);
        }

        if (item.Aggregate == QueryAggregate.Count && item.Field is null && item.Call is null) return "COUNT(*)";

        var inner = Value(item.Field, item.Call);
        if (item.Aggregate == QueryAggregate.Percentile)
        {
            return _dialect.Percentile(inner, item.Percentile ?? 0d, Spec.GroupBy.Count > 0);
        }

        var function = item.Aggregate.Value switch
        {
            QueryAggregate.Count => "COUNT",
            QueryAggregate.Sum => "SUM",
            QueryAggregate.Avg => "AVG",
            QueryAggregate.Min => "MIN",
            QueryAggregate.Max => "MAX",
            _ => "COUNT",
        };
        return $"{function}({(item.Distinct ? "DISTINCT " : string.Empty)}{inner})";
    }

    // What a selected item yields, so a HAVING condition against it reads its value correctly.
    private QueryFieldType OutputType(QuerySelect item)
    {
        if (item.Aggregate is QueryAggregate.Count or QueryAggregate.Percentile) return QueryFieldType.Number;
        if (item.Field is null && item.Call is null) return QueryFieldType.Number;
        return ValueType(item.Field, item.Call);
    }

    private void AppendFrom(StringBuilder sql)
    {
        sql.Append(" FROM ")
           .Append(SourceExpression(Spec.From.Entity, Spec.From.Call))
           .Append(" AS ")
           .Append(_dialect.Quote(Spec.From.Alias));
    }

    private void AppendJoins(StringBuilder sql)
    {
        for (var i = 0; i < Spec.Joins.Count; i++)
        {
            var join = Spec.Joins[i];
            var keyword = join.Kind switch
            {
                QueryJoinKind.Left => "LEFT JOIN",
                QueryJoinKind.Right => "RIGHT JOIN",
                QueryJoinKind.Full => "FULL JOIN",
                QueryJoinKind.Cross => "CROSS JOIN",
                _ => "INNER JOIN",
            };

            sql.Append(' ').Append(keyword).Append(' ')
               .Append(SourceExpression(join.Entity, join.Call))
               .Append(" AS ")
               .Append(_dialect.Quote(join.Alias));

            if (join.Kind == QueryJoinKind.Cross) continue;
            sql.Append(" ON ").Append(string.Join(" AND ", JoinMatches(join, i).Select(match =>
                $"{Member(match.LeftAlias, match.LeftField)} = {Member(match.RightAlias, match.RightField)}")));
        }
    }

    private string SourceExpression(string? entityKey, QueryFunctionCall? call)
        => call is not null
            ? Value(null, call)
            : _dialect.QuoteQualified(Schema.FindEntity(entityKey!)!.PhysicalName);

    private void AppendClause(StringBuilder sql, string keyword, QueryFilterGroup? group)
    {
        if (group is null || group.IsEmpty) return;

        // The shared walk is entered through the base so the outermost group comes back unbracketed:
        // the clause keyword already delimits it. Everything below it goes through the override.
        sql.Append(keyword).Append(base.Filter(group));
    }

    private void AppendGroupBy(StringBuilder sql)
    {
        if (Spec.GroupBy.Count == 0) return;

        var items = new List<string>(Spec.GroupBy.Count);
        foreach (var group in Spec.GroupBy)
        {
            var expression = Value(group.Field, group.Call);
            if (group.Truncate is not null) expression = _dialect.TruncateDate(expression, group.Truncate.Value);
            // A grouping key may be ordered by through its alias even when nothing selects it.
            if (!string.IsNullOrEmpty(group.Alias) && !OutputExpressions.ContainsKey(group.Alias!))
            {
                OutputExpressions[group.Alias!] = expression;
            }
            items.Add(expression);
        }
        sql.Append(" GROUP BY ").Append(string.Join(", ", items));
    }

    private bool AppendOrderBy(StringBuilder sql)
    {
        if (Spec.OrderBy.Count == 0) return false;

        var items = new List<string>(Spec.OrderBy.Count);
        foreach (var sort in Spec.OrderBy)
        {
            string expression;
            if (sort.Field is not null || sort.Call is not null)
            {
                expression = Value(sort.Field, sort.Call);
            }
            else if (_selectAliases.Contains(sort.Select!))
            {
                // Every target engine accepts an output alias here, and it reads better than the
                // aggregate spelled out a second time.
                expression = _dialect.Quote(sort.Select!);
            }
            else
            {
                expression = OutputExpressions[sort.Select!];
            }
            items.Add(expression + (sort.Direction == QuerySortDirection.Descending ? " DESC" : " ASC"));
        }
        sql.Append(" ORDER BY ").Append(string.Join(", ", items));
        return true;
    }

    private string Member(string alias, string fieldKey) => Field(alias, FindField(alias, fieldKey)!);

    private string AddParameter(object? value)
    {
        var ordinal = _parameters.Count;
        _parameters.Add(new SqlQueryParameter(_dialect.ParameterName(ordinal), value));
        return _dialect.ParameterPlaceholder(ordinal);
    }

    // ---- What each node means as SQL ---------------------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field)
        => _dialect.Quote(alias) + "." + _dialect.Quote(field.PhysicalName);

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type) => AddParameter(value);

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
        => _dialect.RelativeMoment(offset.Amount, offset.Unit);

    /// <inheritdoc/>
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => _dialect.CallFunction(_dialect.QuoteQualified(function.PhysicalName), arguments);

    /// <inheritdoc/>
    protected override string Comparison(
        string left, QueryOperator op, QueryFieldType type, string? right, string? upper) => op switch
    {
        QueryOperator.IsNull => left + " IS NULL",
        QueryOperator.IsNotNull => left + " IS NOT NULL",
        QueryOperator.Between => $"{left} BETWEEN {right} AND {upper}",
        QueryOperator.NotBetween => $"{left} NOT BETWEEN {right} AND {upper}",
        _ => $"{left} {Symbol(op)} {right}",
    };

    /// <inheritdoc/>
    protected override string Membership(string left, QueryOperator op, IReadOnlyList<string> values)
        => $"{left} {(op == QueryOperator.NotIn ? "NOT IN" : "IN")} ({string.Join(", ", values)})";

    /// <inheritdoc/>
    protected override string Combine(bool or, IReadOnlyList<string> parts)
        => string.Join(or ? " OR " : " AND ", parts);

    /// <inheritdoc/>
    protected override string? Filter(QueryFilterGroup? group)
    {
        // A nested group is bracketed so its connective binds tighter than the one around it. The
        // outermost group is not, which is why AppendClause enters the walk through the base.
        var rendered = base.Filter(group);
        return rendered is null ? null : "(" + rendered + ")";
    }

    /// <inheritdoc/>
    protected override string Condition(QueryCondition condition)
    {
        // A pattern match is the one thing that has to see the operand before it is rendered: the
        // wildcards and the escape character belong to the pattern, not to the text a user typed,
        // and by the time the shared walk hands over an operand that text has become a parameter.
        var shape = PatternShape(condition.Operator);
        return shape is null
            ? base.Condition(condition)
            : Like(Subject(condition), condition.Value!, shape);
    }

    private static string? PatternShape(QueryOperator op) => op switch
    {
        QueryOperator.Contains => "%{0}%",
        QueryOperator.StartsWith => "{0}%",
        QueryOperator.EndsWith => "%{0}",
        _ => null,
    };

    // The left side of a condition, which is all a pattern match needs of the shared resolution.
    private string Subject(QueryCondition condition)
        => string.IsNullOrEmpty(condition.Select)
            ? Value(condition.Field, condition.Call)
            : OutputExpressions[condition.Select!];

    private string Like(string left, QueryOperand operand, string shape)
    {
        // Comparing against another expression cannot carry an escaped pattern, so it is emitted as-is.
        if (operand.Kind != QueryOperandKind.Literal)
        {
            return $"{left} {_dialect.LikeOperator} {Operand(operand, QueryFieldType.Text)}";
        }

        var pattern = string.Format(shape, _dialect.EscapeLikePattern(operand.Value ?? string.Empty));
        return $"{left} {_dialect.LikeOperator} {AddParameter(pattern)} ESCAPE '{_dialect.LikeEscape}'";
    }

    private static string Symbol(QueryOperator op) => op switch
    {
        QueryOperator.Equals => "=",
        QueryOperator.NotEquals => "<>",
        QueryOperator.GreaterThan => ">",
        QueryOperator.GreaterThanOrEqual => ">=",
        QueryOperator.LessThan => "<",
        QueryOperator.LessThanOrEqual => "<=",
        _ => "=",
    };
}
