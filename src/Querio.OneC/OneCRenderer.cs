using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.OneC;

/// <summary>
/// Turns a query into 1C:Enterprise query text plus the parameters it refers to.
/// <para>
/// Two things differ from a SQL target and shape this renderer. 1C has no way to quote an
/// identifier, so names are validated instead of escaped; and its query language has no function for
/// the current moment, so a relative time window is resolved into a parameter value as the query is
/// rendered. That keeps the window relative where it matters - the spec still says "the last 30
/// days", and every render resolves it afresh.
/// </para>
/// <para>
/// Keywords are emitted in Russian, which is what 1C developers read and what its documentation
/// uses. They are output data, not source identifiers.
/// </para>
/// <para>
/// The walk over the query is the shared one, so what is left here is only how 1C spells each part
/// of it - which is exactly the thing that makes the query model semantic rather than SQL-shaped.
/// </para>
/// </summary>
public sealed class OneCRenderer : QueryRenderer<string>
{
    private readonly DateTime _now;
    private readonly List<OneCQueryParameter> _parameters = [];

    private OneCRenderer(QuerySpec spec, QuerySchema schema, DateTime now)
        : base(spec, schema) => _now = now;

    /// <summary>
    /// What 1C can do. It has no percentile, no row offset and no cross join, so a query asking for
    /// one of those is refused rather than approximated.
    /// </summary>
    public static IQueryCapabilities Capabilities { get; } = QueryCapabilities.All.Without(
        QueryFeature.Percentile,
        QueryFeature.Offset,
        QueryFeature.CrossJoin,
        // A value function maps onto whatever 1C spells the same job, given a physical name. Its
        // table-valued equivalent - a virtual table - has a wholly different syntax, so it is not
        // something this renderer can honestly produce.
        QueryFeature.TableFunctions);

    /// <inheritdoc/>
    protected override string TargetName => "The 1C query language";

    /// <summary>Renders a query into 1C query text.</summary>
    /// <param name="spec">The query to render.</param>
    /// <param name="schema">The schema it was built against, which supplies types and physical names.</param>
    /// <param name="now">
    /// The moment a relative time window is measured from. Defaults to the current UTC time; pass one
    /// explicitly to render deterministically.
    /// </param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">1C cannot express the query.</exception>
    public static OneCRenderResult Render(QuerySpec spec, QuerySchema schema, DateTime? now = null)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));

        return new OneCRenderer(spec, schema, now ?? DateTime.UtcNow).Run();
    }

    private OneCRenderResult Run()
    {
        Prepare();
        // Every alias is checked once the participants are known and before anything is written,
        // since an alias this renderer cannot write has to stop the render rather than be escaped.
        foreach (var participant in Participants) EnsureIdentifier(participant.Alias, "alias");
        RequireCapabilities(Capabilities);

        var query = new StringBuilder();
        AppendSelect(query);
        AppendFrom(query);
        AppendJoins(query);
        AppendClause(query, " ГДЕ ", Spec.Where);
        AppendGroupBy(query);
        AppendClause(query, " ИМЕЮЩИЕ ", Spec.Having);
        AppendOrderBy(query);

        return new OneCRenderResult(query.ToString(), _parameters);
    }

    private void AppendSelect(StringBuilder query)
    {
        query.Append("ВЫБРАТЬ ");
        if (Spec.Distinct) query.Append("РАЗЛИЧНЫЕ ");
        if (Spec.Limit is not null)
        {
            query.Append("ПЕРВЫЕ ")
                 .Append(Spec.Limit.Value.ToString(CultureInfo.InvariantCulture))
                 .Append(' ');
        }

        if (Spec.Select.Count == 0)
        {
            query.Append('*');
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
            EnsureIdentifier(item.Alias!, "output alias");
            OutputExpressions[item.Alias!] = expression;
            OutputTypes[item.Alias!] = OutputType(item);
            items.Add(expression + " КАК " + item.Alias);
        }
        query.Append(string.Join(", ", items));
    }

    private string SelectExpression(QuerySelect item)
    {
        if (item.Aggregate is null)
        {
            var plain = Value(item.Field, item.Call);
            return item.Truncate is null ? plain : Truncate(plain, item.Truncate.Value);
        }

        if (item.Aggregate == QueryAggregate.Count && item.Field is null && item.Call is null) return "КОЛИЧЕСТВО(*)";

        var inner = Value(item.Field, item.Call);
        var function = item.Aggregate.Value switch
        {
            QueryAggregate.Count => "КОЛИЧЕСТВО",
            QueryAggregate.Sum => "СУММА",
            QueryAggregate.Avg => "СРЕДНЕЕ",
            QueryAggregate.Min => "МИНИМУМ",
            QueryAggregate.Max => "МАКСИМУМ",
            _ => "КОЛИЧЕСТВО",
        };
        return $"{function}({(item.Distinct ? "РАЗЛИЧНЫЕ " : string.Empty)}{inner})";
    }

    private QueryFieldType OutputType(QuerySelect item)
    {
        if (item.Aggregate == QueryAggregate.Count) return QueryFieldType.Number;
        if (item.Field is null && item.Call is null) return QueryFieldType.Number;
        return ValueType(item.Field, item.Call);
    }

    private static string Truncate(string expression, QueryDateTruncation truncation)
    {
        var period = truncation switch
        {
            QueryDateTruncation.Minute => "МИНУТА",
            QueryDateTruncation.Hour => "ЧАС",
            QueryDateTruncation.Day => "ДЕНЬ",
            QueryDateTruncation.Week => "НЕДЕЛЯ",
            QueryDateTruncation.Month => "МЕСЯЦ",
            QueryDateTruncation.Quarter => "КВАРТАЛ",
            QueryDateTruncation.Year => "ГОД",
            _ => "ДЕНЬ",
        };
        return $"НАЧАЛОПЕРИОДА({expression}, {period})";
    }

    private void AppendFrom(StringBuilder query)
    {
        query.Append(" ИЗ ")
             .Append(SourceExpression(Spec.From.Entity, Spec.From.Call))
             .Append(" КАК ")
             .Append(Spec.From.Alias);
    }

    private string SourceExpression(string? entityKey, QueryFunctionCall? call)
        => call is not null ? Value(null, call) : QualifiedName(Schema.FindEntity(entityKey!)!.PhysicalName);

    private void AppendJoins(StringBuilder query)
    {
        for (var i = 0; i < Spec.Joins.Count; i++)
        {
            var join = Spec.Joins[i];
            var keyword = join.Kind switch
            {
                QueryJoinKind.Left => "ЛЕВОЕ СОЕДИНЕНИЕ",
                QueryJoinKind.Right => "ПРАВОЕ СОЕДИНЕНИЕ",
                QueryJoinKind.Full => "ПОЛНОЕ СОЕДИНЕНИЕ",
                _ => "ВНУТРЕННЕЕ СОЕДИНЕНИЕ",
            };

            query.Append(' ').Append(keyword).Append(' ')
                 .Append(SourceExpression(join.Entity, join.Call))
                 .Append(" КАК ")
                 .Append(join.Alias)
                 .Append(" ПО ")
                 .Append(string.Join(" И ", JoinMatches(join, i).Select(match =>
                     $"{Member(match.LeftAlias, match.LeftField)} = {Member(match.RightAlias, match.RightField)}")));
        }
    }

    private void AppendClause(StringBuilder query, string keyword, QueryFilterGroup? group)
    {
        if (group is null || group.IsEmpty) return;

        // The shared walk is entered through the base so the outermost group comes back unbracketed:
        // the clause keyword already delimits it. Everything below it goes through the override.
        query.Append(keyword).Append(base.Filter(group));
    }

    private void AppendGroupBy(StringBuilder query)
    {
        if (Spec.GroupBy.Count == 0) return;

        var items = new List<string>(Spec.GroupBy.Count);
        foreach (var group in Spec.GroupBy)
        {
            var expression = Value(group.Field, group.Call);
            if (group.Truncate is not null) expression = Truncate(expression, group.Truncate.Value);
            if (!string.IsNullOrEmpty(group.Alias) && !OutputExpressions.ContainsKey(group.Alias!))
            {
                OutputExpressions[group.Alias!] = expression;
            }
            items.Add(expression);
        }
        query.Append(" СГРУППИРОВАТЬ ПО ").Append(string.Join(", ", items));
    }

    private void AppendOrderBy(StringBuilder query)
    {
        if (Spec.OrderBy.Count == 0) return;

        var items = new List<string>(Spec.OrderBy.Count);
        foreach (var sort in Spec.OrderBy)
        {
            var expression = sort.Field is not null || sort.Call is not null
                ? Value(sort.Field, sort.Call)
                : sort.Select!;
            items.Add(expression + (sort.Direction == QuerySortDirection.Descending ? " УБЫВ" : " ВОЗР"));
        }
        query.Append(" УПОРЯДОЧИТЬ ПО ").Append(string.Join(", ", items));
    }

    private string Member(string alias, string fieldKey) => Field(alias, FindField(alias, fieldKey)!);

    private string QualifiedName(string name)
    {
        foreach (var part in name.Split('.')) EnsureIdentifier(part, "object name");
        return name;
    }

    // 1C offers no way to quote a name, so an unusable name has to be rejected rather than escaped.
    // This is also what keeps a user-chosen alias from being able to alter the query.
    private static void EnsureIdentifier(string identifier, string what)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new QueryRenderException($"An empty {what} cannot be written into a 1C query.");
        }
        for (var i = 0; i < identifier.Length; i++)
        {
            var character = identifier[i];
            var allowed = char.IsLetter(character) || character == '_' || (i > 0 && char.IsDigit(character));
            if (!allowed)
            {
                throw new QueryRenderException(
                    $"The {what} '{identifier}' is not a valid 1C identifier. 1C cannot quote a name, so " +
                    "only letters, digits and underscores are usable.");
            }
        }
    }

    private static DateTime Shift(DateTime now, QueryRelativeValue relative) => relative.Unit switch
    {
        QueryTimeUnit.Minute => now.AddMinutes(relative.Amount),
        QueryTimeUnit.Hour => now.AddHours(relative.Amount),
        QueryTimeUnit.Day => now.AddDays(relative.Amount),
        QueryTimeUnit.Week => now.AddDays(relative.Amount * 7),
        QueryTimeUnit.Month => now.AddMonths(relative.Amount),
        QueryTimeUnit.Quarter => now.AddMonths(relative.Amount * 3),
        QueryTimeUnit.Year => now.AddYears(relative.Amount),
        _ => now.AddDays(relative.Amount),
    };

    private string AddParameter(object? value)
    {
        var name = "p" + _parameters.Count.ToString(CultureInfo.InvariantCulture);
        _parameters.Add(new OneCQueryParameter(name, value));
        return "&" + name;
    }

    // ---- What each node means in the 1C language ---------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field)
    {
        EnsureIdentifier(field.PhysicalName, "field name");
        return alias + "." + field.PhysicalName;
    }

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type) => AddParameter(value);

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
        // 1C queries have no current-moment function, so the window is resolved here and travels as a
        // value. The spec keeps the offset, so the next render resolves it again.
        => AddParameter(Shift(_now, offset));

    /// <inheritdoc/>
    // A value function maps onto whatever the target spells the same job, which the schema supplies
    // as the physical name - so a call renders here exactly as it would in SQL, just unquoted.
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => QualifiedName(function.PhysicalName) + "(" + string.Join(", ", arguments) + ")";

    /// <inheritdoc/>
    protected override string Comparison(
        string left, QueryOperator op, QueryFieldType type, string? right, string? upper) => op switch
    {
        QueryOperator.IsNull => left + " ЕСТЬ NULL",
        QueryOperator.IsNotNull => left + " ЕСТЬ НЕ NULL",
        QueryOperator.Between => $"{left} МЕЖДУ {right} И {upper}",
        QueryOperator.NotBetween => $"НЕ ({left} МЕЖДУ {right} И {upper})",
        _ => $"{left} {Symbol(op)} {right}",
    };

    /// <inheritdoc/>
    protected override string Membership(string left, QueryOperator op, IReadOnlyList<string> values)
        => $"{left} {(op == QueryOperator.NotIn ? "НЕ В" : "В")} ({string.Join(", ", values)})";

    /// <inheritdoc/>
    protected override string Combine(bool or, IReadOnlyList<string> parts)
        => string.Join(or ? " ИЛИ " : " И ", parts);

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
        if (operand.Kind != QueryOperandKind.Literal)
        {
            return $"{left} ПОДОБНО {Operand(operand, QueryFieldType.Text)}";
        }
        var pattern = string.Format(shape, EscapePattern(operand.Value ?? string.Empty));
        return $"{left} ПОДОБНО {AddParameter(pattern)} СПЕЦСИМВОЛ \"\\\"";
    }

    private static string EscapePattern(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character is '\\' or '%' or '_' or '[') builder.Append('\\');
            builder.Append(character);
        }
        return builder.ToString();
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
