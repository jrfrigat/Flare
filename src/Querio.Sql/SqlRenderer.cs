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
/// </summary>
public sealed class SqlRenderer
{
    private readonly QuerySpec _spec;
    private readonly QuerySchema _schema;
    private readonly SqlDialect _dialect;
    private readonly List<SqlQueryParameter> _parameters = [];
    private readonly Dictionary<string, QueryEntity> _participants = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<KeyValuePair<string, string>> _ordered = [];
    private readonly Dictionary<string, string> _outputExpressions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, QueryFieldType> _outputTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _selectAliases = new(StringComparer.OrdinalIgnoreCase);

    private SqlRenderer(QuerySpec spec, QuerySchema schema, SqlDialect dialect)
    {
        _spec = spec;
        _schema = schema;
        _dialect = dialect;
    }

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
        // Rendering an incoherent query would produce SQL that is merely wrong in a new way.
        _spec.Validate(_schema).ThrowIfInvalid();
        CollectParticipants();
        RequireCapabilities();

        var sql = new StringBuilder();
        AppendSelect(sql);
        AppendFrom(sql);
        AppendJoins(sql);
        AppendClause(sql, " WHERE ", _spec.Where, having: false);
        AppendGroupBy(sql);
        AppendClause(sql, " HAVING ", _spec.Having, having: true);
        var hasOrderBy = AppendOrderBy(sql);
        _dialect.AppendPaging(sql, _spec.Limit, _spec.Offset, hasOrderBy);

        return new SqlRenderResult(sql.ToString(), _parameters);
    }

    private void CollectParticipants()
    {
        Register(_spec.From.Entity, _spec.From.Alias);
        foreach (var join in _spec.Joins) Register(join.Entity, join.Alias);
    }

    private void Register(string entityKey, string alias)
    {
        // Validation already proved the entity exists, so a miss here is impossible by construction.
        _participants[alias] = _schema.FindEntity(entityKey)!;
        _ordered.Add(new KeyValuePair<string, string>(alias, entityKey));
    }

    private void RequireCapabilities()
    {
        foreach (var join in _spec.Joins)
        {
            switch (join.Kind)
            {
                case QueryJoinKind.Left: Require(QueryFeature.LeftJoin); break;
                case QueryJoinKind.Right: Require(QueryFeature.RightJoin); break;
                case QueryJoinKind.Full: Require(QueryFeature.FullJoin); break;
                case QueryJoinKind.Cross: Require(QueryFeature.CrossJoin); break;
            }
        }

        foreach (var item in _spec.Select)
        {
            if (item.Aggregate is not null) Require(QueryFeature.Aggregates);
            if (item.Distinct) Require(QueryFeature.DistinctAggregate);
            if (item.Aggregate == QueryAggregate.Percentile) Require(QueryFeature.Percentile);
            if (item.Truncate is not null) Require(QueryFeature.DateTruncation);
        }

        if (_spec.GroupBy.Count > 0) Require(QueryFeature.Grouping);
        if (_spec.GroupBy.Any(group => group.Truncate is not null)) Require(QueryFeature.DateTruncation);
        if (_spec.Having is not null) Require(QueryFeature.Having);
        if (_spec.Distinct) Require(QueryFeature.Distinct);
        if (_spec.Limit is not null) Require(QueryFeature.Limit);
        if (_spec.Offset is not null) Require(QueryFeature.Offset);

        foreach (var condition in AllConditions(_spec.Where).Concat(AllConditions(_spec.Having)))
        {
            if (condition.Operator is QueryOperator.In or QueryOperator.NotIn) Require(QueryFeature.SetOperators);
            if (condition.Operator is QueryOperator.Between or QueryOperator.NotBetween) Require(QueryFeature.RangeOperators);
            if (condition.Operator is QueryOperator.Contains or QueryOperator.StartsWith or QueryOperator.EndsWith)
            {
                Require(QueryFeature.TextSearch);
            }
            foreach (var operand in new[] { condition.Value, condition.Value2 })
            {
                if (operand is null) continue;
                if (operand.Kind == QueryOperandKind.Field) Require(QueryFeature.FieldComparison);
                if (operand.Kind == QueryOperandKind.Relative) Require(QueryFeature.RelativeTime);
            }
        }
    }

    private void Require(QueryFeature feature)
    {
        if (_dialect.Supports(feature)) return;
        throw new QueryRenderException($"{_dialect.Name} does not support {feature}.", feature);
    }

    private static IEnumerable<QueryCondition> AllConditions(QueryFilterGroup? group)
    {
        if (group is null) yield break;
        foreach (var condition in group.Conditions) yield return condition;
        foreach (var nested in group.Groups)
        {
            foreach (var condition in AllConditions(nested)) yield return condition;
        }
    }

    private void AppendSelect(StringBuilder sql)
    {
        sql.Append("SELECT ");
        if (_spec.Distinct) sql.Append("DISTINCT ");
        sql.Append(_dialect.RenderTop(_spec.Limit, _spec.Offset));

        if (_spec.Select.Count == 0)
        {
            sql.Append('*');
            return;
        }

        var items = new List<string>(_spec.Select.Count);
        foreach (var item in _spec.Select)
        {
            var expression = SelectExpression(item);
            if (string.IsNullOrEmpty(item.Alias))
            {
                items.Add(expression);
                continue;
            }
            _outputExpressions[item.Alias!] = expression;
            _outputTypes[item.Alias!] = OutputType(item);
            _selectAliases.Add(item.Alias!);
            items.Add(expression + " AS " + _dialect.Quote(item.Alias!));
        }
        sql.Append(string.Join(", ", items));
    }

    private string SelectExpression(QuerySelect item)
    {
        if (item.Aggregate is null)
        {
            var plain = FieldExpression(item.Field!);
            return item.Truncate is null ? plain : _dialect.TruncateDate(plain, item.Truncate.Value);
        }

        if (item.Aggregate == QueryAggregate.Count && item.Field is null) return "COUNT(*)";

        var inner = FieldExpression(item.Field!);
        if (item.Aggregate == QueryAggregate.Percentile)
        {
            return _dialect.Percentile(inner, item.Percentile ?? 0d, _spec.GroupBy.Count > 0);
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
        if (item.Aggregate == QueryAggregate.Count) return QueryFieldType.Number;
        if (item.Aggregate == QueryAggregate.Percentile) return QueryFieldType.Number;
        if (item.Field is null) return QueryFieldType.Number;
        return FieldType(item.Field);
    }

    private void AppendFrom(StringBuilder sql)
    {
        var entity = _participants[_spec.From.Alias];
        sql.Append(" FROM ")
           .Append(_dialect.QuoteQualified(entity.PhysicalName))
           .Append(" AS ")
           .Append(_dialect.Quote(_spec.From.Alias));
    }

    private void AppendJoins(StringBuilder sql)
    {
        for (var i = 0; i < _spec.Joins.Count; i++)
        {
            var join = _spec.Joins[i];
            var entity = _participants[join.Alias];
            var keyword = join.Kind switch
            {
                QueryJoinKind.Left => "LEFT JOIN",
                QueryJoinKind.Right => "RIGHT JOIN",
                QueryJoinKind.Full => "FULL JOIN",
                QueryJoinKind.Cross => "CROSS JOIN",
                _ => "INNER JOIN",
            };

            sql.Append(' ').Append(keyword).Append(' ')
               .Append(_dialect.QuoteQualified(entity.PhysicalName))
               .Append(" AS ")
               .Append(_dialect.Quote(join.Alias));

            if (join.Kind == QueryJoinKind.Cross) continue;
            sql.Append(" ON ").Append(JoinCondition(join, i));
        }
    }

    private string JoinCondition(QueryJoin join, int index)
    {
        if (join.On is { Count: > 0 })
        {
            return string.Join(" AND ", join.On.Select(pair =>
                $"{FieldExpression(pair.Left)} = {FieldExpression(pair.Right)}"));
        }

        var relation = _schema.FindRelation(join.Relation!)!;

        // A self-relation matches on both ends. Reading the new participant as the target - "the
        // manager of the user we already have" - is the direction a person means when they add it.
        var newIsTarget = string.Equals(relation.To, join.Entity, StringComparison.OrdinalIgnoreCase);
        var otherEntity = newIsTarget ? relation.From : relation.To;
        var otherAlias = join.From ?? ResolveOtherAlias(otherEntity, index, join.Alias);

        return string.Join(" AND ", relation.On.Select(pair =>
        {
            var source = newIsTarget
                ? Member(otherAlias, pair.FromField)
                : Member(join.Alias, pair.FromField);
            var target = newIsTarget
                ? Member(join.Alias, pair.ToField)
                : Member(otherAlias, pair.ToField);
            return $"{source} = {target}";
        }));
    }

    // Finds which earlier participant the join attaches to. Never guesses: with the same entity
    // present twice, only the caller knows which occurrence was meant.
    private string ResolveOtherAlias(string entityKey, int joinIndex, string joinAlias)
    {
        var candidates = _ordered
            .Take(joinIndex + 1) // the root plus every join declared before this one
            .Where(participant => !string.Equals(participant.Key, joinAlias, StringComparison.OrdinalIgnoreCase)
                && string.Equals(participant.Value, entityKey, StringComparison.OrdinalIgnoreCase))
            .Select(participant => participant.Key)
            .ToList();

        if (candidates.Count == 1) return candidates[0];

        if (candidates.Count == 0)
        {
            throw new QueryRenderException(
                $"The join onto '{joinAlias}' traverses a relation reaching '{entityKey}', which no earlier participant uses.");
        }

        throw new QueryRenderException(
            $"'{entityKey}' appears more than once ({string.Join(", ", candidates)}), so the join onto " +
            $"'{joinAlias}' is ambiguous. Set QueryJoin.From to the alias it attaches to.");
    }

    private void AppendClause(StringBuilder sql, string keyword, QueryFilterGroup? group, bool having)
    {
        if (group is null || group.IsEmpty) return;
        sql.Append(keyword).Append(RenderGroup(group, having));
    }

    private string RenderGroup(QueryFilterGroup group, bool having)
    {
        var parts = new List<string>(group.Conditions.Count + group.Groups.Count);
        foreach (var condition in group.Conditions) parts.Add(RenderCondition(condition, having));
        foreach (var nested in group.Groups)
        {
            if (nested.IsEmpty) continue;
            parts.Add("(" + RenderGroup(nested, having) + ")");
        }
        return string.Join(group.Or ? " OR " : " AND ", parts);
    }

    private string RenderCondition(QueryCondition condition, bool having)
    {
        string left;
        QueryFieldType type;
        if (condition.Field is not null)
        {
            left = FieldExpression(condition.Field);
            type = FieldType(condition.Field);
        }
        else
        {
            // A HAVING condition names a computed aggregate. Some engines refuse an output alias
            // here, so the expression behind it is emitted instead.
            left = _outputExpressions[condition.Select!];
            type = _outputTypes.TryGetValue(condition.Select!, out var known) ? known : QueryFieldType.Number;
        }

        switch (condition.Operator)
        {
            case QueryOperator.IsNull:
                return left + " IS NULL";
            case QueryOperator.IsNotNull:
                return left + " IS NOT NULL";
            case QueryOperator.Contains:
                return Like(left, condition.Value!, "%{0}%");
            case QueryOperator.StartsWith:
                return Like(left, condition.Value!, "{0}%");
            case QueryOperator.EndsWith:
                return Like(left, condition.Value!, "%{0}");
            case QueryOperator.In:
                return $"{left} IN ({ValueList(condition.Value!, type)})";
            case QueryOperator.NotIn:
                return $"{left} NOT IN ({ValueList(condition.Value!, type)})";
            case QueryOperator.Between:
                return $"{left} BETWEEN {Operand(condition.Value!, type)} AND {Operand(condition.Value2!, type)}";
            case QueryOperator.NotBetween:
                return $"{left} NOT BETWEEN {Operand(condition.Value!, type)} AND {Operand(condition.Value2!, type)}";
            default:
                return $"{left} {Symbol(condition.Operator)} {Operand(condition.Value!, type)}";
        }
    }

    private string Like(string left, QueryOperand operand, string shape)
    {
        // Comparing against another column cannot carry an escaped pattern, so it is emitted as-is.
        if (operand.Kind != QueryOperandKind.Literal)
        {
            return $"{left} {_dialect.LikeOperator} {Operand(operand, QueryFieldType.Text)}";
        }

        var pattern = string.Format(shape, _dialect.EscapeLikePattern(operand.Value ?? string.Empty));
        return $"{left} {_dialect.LikeOperator} {AddParameter(pattern)} ESCAPE '{_dialect.LikeEscape}'";
    }

    private string ValueList(QueryOperand operand, QueryFieldType type)
        => string.Join(", ", (operand.Values ?? []).Select(value => AddParameter(QueryValue.Parse(value, type))));

    private string Operand(QueryOperand operand, QueryFieldType type) => operand.Kind switch
    {
        QueryOperandKind.Field => FieldExpression(operand.Field!),
        QueryOperandKind.Relative => _dialect.RelativeMoment(operand.Relative!.Amount, operand.Relative.Unit),
        QueryOperandKind.List => ValueList(operand, type),
        _ => AddParameter(QueryValue.Parse(operand.Value, type)),
    };

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

    private void AppendGroupBy(StringBuilder sql)
    {
        if (_spec.GroupBy.Count == 0) return;

        var items = new List<string>(_spec.GroupBy.Count);
        foreach (var group in _spec.GroupBy)
        {
            var expression = FieldExpression(group.Field);
            if (group.Truncate is not null) expression = _dialect.TruncateDate(expression, group.Truncate.Value);
            // A grouping key may be ordered by through its alias even when nothing selects it.
            if (!string.IsNullOrEmpty(group.Alias) && !_outputExpressions.ContainsKey(group.Alias!))
            {
                _outputExpressions[group.Alias!] = expression;
            }
            items.Add(expression);
        }
        sql.Append(" GROUP BY ").Append(string.Join(", ", items));
    }

    private bool AppendOrderBy(StringBuilder sql)
    {
        if (_spec.OrderBy.Count == 0) return false;

        var items = new List<string>(_spec.OrderBy.Count);
        foreach (var sort in _spec.OrderBy)
        {
            string expression;
            if (sort.Field is not null)
            {
                expression = FieldExpression(sort.Field);
            }
            else if (_selectAliases.Contains(sort.Select!))
            {
                // Every target engine accepts an output alias here, and it reads better than the
                // aggregate spelled out a second time.
                expression = _dialect.Quote(sort.Select!);
            }
            else
            {
                expression = _outputExpressions[sort.Select!];
            }
            items.Add(expression + (sort.Direction == QuerySortDirection.Descending ? " DESC" : " ASC"));
        }
        sql.Append(" ORDER BY ").Append(string.Join(", ", items));
        return true;
    }

    private string FieldExpression(QueryFieldRef reference) => Member(reference.Alias, reference.Field);

    private string Member(string alias, string fieldKey)
    {
        var field = _participants[alias].FindField(fieldKey)!;
        return _dialect.Quote(alias) + "." + _dialect.Quote(field.PhysicalName);
    }

    private QueryFieldType FieldType(QueryFieldRef reference)
        => _participants[reference.Alias].FindField(reference.Field)!.Type;

    private string AddParameter(object? value)
    {
        var ordinal = _parameters.Count;
        _parameters.Add(new SqlQueryParameter(_dialect.ParameterName(ordinal), value));
        return _dialect.ParameterPlaceholder(ordinal);
    }
}
