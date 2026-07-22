using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.Language;

/// <summary>
/// Writes a query as text in the language.
/// <para>
/// Names are written logically and bracketed, so the text says what the query means rather than what
/// one store happens to call it, and a name colliding with a keyword still reads as a name.
/// </para>
/// <para>
/// Joins are written out, including the ones a foreign-key path produced. The dots cannot come back:
/// once a hop has become a join, nothing in the query records that it was ever written as sugar. So
/// this round trip is stable in what the query means, not in the characters it was typed as.
/// </para>
/// </summary>
internal sealed class QueryLanguageWriter : QueryRenderer<string>
{
    private readonly StringBuilder _text = new();

    internal QueryLanguageWriter(QuerySpec spec, QuerySchema schema) : base(spec, schema)
    {
    }

    /// <summary>Anything a query can express can be written down, so nothing is refused.</summary>
    internal static IQueryCapabilities Capabilities { get; } = QueryCapabilities.All;

    /// <inheritdoc/>
    protected override string TargetName => "The language writer";

    internal string Run()
    {
        Prepare(Capabilities);

        _text.Append("select ");
        if (Spec.Distinct) _text.Append("distinct ");
        _text.Append(Spec.Select.Count == 0 ? "*" : string.Join(", ", Spec.Select.Select(Selected)));

        _text.Append(Environment.NewLine).Append("from ").Append(Source(Spec.From.Entity, Spec.From.Call, Spec.From.Alias));
        foreach (var join in Spec.Joins) _text.Append(Environment.NewLine).Append(Joined(join));

        // Written before the filters so a condition naming a computed aggregate reads as that name.
        foreach (var item in Spec.Select)
        {
            if (!string.IsNullOrEmpty(item.Alias)) OutputExpressions[item.Alias!] = Name(item.Alias!);
        }

        var where = Filter(Spec.Where);
        if (!string.IsNullOrEmpty(where)) _text.Append(Environment.NewLine).Append("where ").Append(Unwrap(where!));

        if (Spec.GroupBy.Count > 0)
        {
            _text.Append(Environment.NewLine).Append("group by ")
                .Append(string.Join(", ", Spec.GroupBy.Select(Grouped)));
        }

        var having = Filter(Spec.Having);
        if (!string.IsNullOrEmpty(having)) _text.Append(Environment.NewLine).Append("having ").Append(Unwrap(having!));

        if (Spec.OrderBy.Count > 0)
        {
            _text.Append(Environment.NewLine).Append("order by ")
                .Append(string.Join(", ", Spec.OrderBy.Select(Sorted)));
        }

        if (Spec.Limit is not null) _text.Append(Environment.NewLine).Append("limit ").Append(Number(Spec.Limit.Value));
        if (Spec.Offset is not null) _text.Append(Environment.NewLine).Append("offset ").Append(Number(Spec.Offset.Value));

        return _text.ToString();
    }

    // The outermost brackets say nothing, since a clause holds one condition tree by definition.
    private static string Unwrap(string filter)
        => filter.Length > 1 && filter[0] == '(' && filter[filter.Length - 1] == ')' && Balanced(filter)
            ? filter.Substring(1, filter.Length - 2)
            : filter;

    private static bool Balanced(string text)
    {
        var depth = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '(') depth++;
            else if (text[i] == ')' && --depth == 0 && i != text.Length - 1) return false;
        }
        return true;
    }

    private string Source(string? entity, QueryFunctionCall? call, string alias)
        => (call is not null ? RenderCall(call) : Name(entity!)) + " as " + Name(alias);

    private string Joined(QueryJoin join)
    {
        var kind = join.Kind == QueryJoinKind.Inner ? string.Empty : join.Kind.ToString().ToLowerInvariant() + " ";
        var text = kind + "join " + Source(join.Entity, join.Call, join.Alias);

        if (join.On is { Count: > 0 })
        {
            var matches = join.On.Select(pair => $"{Reference(pair.Left)} = {Reference(pair.Right)}");
            return text + " on " + string.Join(" and ", matches);
        }
        if (!string.IsNullOrEmpty(join.Relation)) return text + " through " + Name(join.Relation!);
        return text;
    }

    private string Reference(QueryFieldRef reference)
    {
        var field = FindField(reference.Alias, reference.Field);
        return Name(reference.Alias) + "." + Name(field?.Key ?? reference.Field);
    }

    private string Selected(QuerySelect item)
    {
        var body = item.Aggregate is null
            ? Period(Value(item.Field, item.Call), item.Truncate)
            : Aggregated(item);
        return string.IsNullOrEmpty(item.Alias) ? body : body + " as " + Name(item.Alias!);
    }

    private string Aggregated(QuerySelect item)
    {
        var name = item.Aggregate!.Value.ToString().ToLowerInvariant();

        // A row count names nothing, which is exactly what tells the two counts apart.
        if (item.Aggregate == QueryAggregate.Count && item.Field is null && item.Call is null) return "count(*)";

        var inner = Value(item.Field, item.Call);
        if (item.Distinct) inner = "distinct " + inner;
        if (item.Aggregate == QueryAggregate.Percentile && item.Percentile is not null)
        {
            inner += ", " + item.Percentile.Value.ToString("R", CultureInfo.InvariantCulture);
        }
        return $"{name}({inner})";
    }

    private string Grouped(QueryGroupBy group) => Period(Value(group.Field, group.Call), group.Truncate);

    private string Sorted(QuerySort sort)
    {
        var body = string.IsNullOrEmpty(sort.Select) ? Value(sort.Field, sort.Call) : Name(sort.Select!);
        return sort.Direction == QuerySortDirection.Descending ? body + " desc" : body + " asc";
    }

    private static string Period(string value, QueryDateTruncation? truncate)
        => truncate is null ? value : $"trunc({value}, {truncate.Value.ToString().ToLowerInvariant()})";

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>Brackets a name, doubling any closing bracket inside it as the reader expects.</summary>
    private static string Name(string name) => "[" + name.Replace("]", "]]") + "]";

    private string RenderCall(QueryFunctionCall call)
    {
        var function = Schema.FindFunction(call.Function);
        var arguments = new List<string>(call.Arguments.Count);
        for (var i = 0; i < call.Arguments.Count; i++)
        {
            var type = function is not null && i < function.Parameters.Count
                ? function.Parameters[i].Type
                : QueryFieldType.Text;
            arguments.Add(Operand(call.Arguments[i], type));
        }
        return $"{call.Function}({string.Join(", ", arguments)})";
    }

    // ---- What each node means as text -------------------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field) => Name(alias) + "." + Name(field.Key);

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type)
    {
        if (value is null) return "null";
        if (value is bool flag) return flag ? "true" : "false";

        // Back to the exact form the query stores, so reading it returns the same value rather than
        // one that merely looks the same.
        var stored = QueryValue.ToInvariant(value)!;
        if (type == QueryFieldType.Number) return stored;
        return "'" + stored.Replace("'", "''") + "'";
    }

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
    {
        var unit = offset.Unit.ToString().ToLowerInvariant();
        var sign = offset.Amount < 0 ? " - " : " + ";
        return "now" + sign + Math.Abs(offset.Amount).ToString(CultureInfo.InvariantCulture) + " " + unit;
    }

    /// <inheritdoc/>
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => $"{function.Key}({string.Join(", ", arguments)})";

    /// <inheritdoc/>
    protected override string Comparison(
        string left, QueryOperator op, QueryFieldType type, string? right, string? upper) => op switch
    {
        QueryOperator.IsNull => left + " is null",
        QueryOperator.IsNotNull => left + " is not null",
        QueryOperator.Between => $"{left} between {right} and {upper}",
        QueryOperator.NotBetween => $"{left} not between {right} and {upper}",
        QueryOperator.Contains => $"{left} contains {right}",
        QueryOperator.StartsWith => $"{left} startswith {right}",
        QueryOperator.EndsWith => $"{left} endswith {right}",
        QueryOperator.NotEquals => $"{left} <> {right}",
        QueryOperator.GreaterThan => $"{left} > {right}",
        QueryOperator.GreaterThanOrEqual => $"{left} >= {right}",
        QueryOperator.LessThan => $"{left} < {right}",
        QueryOperator.LessThanOrEqual => $"{left} <= {right}",
        _ => $"{left} = {right}",
    };

    /// <inheritdoc/>
    protected override string Membership(string left, QueryOperator op, IReadOnlyList<string> values)
        => $"{left} {(op == QueryOperator.NotIn ? "not in" : "in")} ({string.Join(", ", values)})";

    /// <inheritdoc/>
    protected override string Combine(bool or, IReadOnlyList<string> parts)
        => "(" + string.Join(or ? " or " : " and ", parts) + ")";
}
