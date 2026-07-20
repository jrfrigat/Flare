using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.Http;

/// <summary>
/// Writes a query as a query string. Field and entity keys are written logically rather than by
/// their physical names, because this text is Querio's own and has to mean the same thing whichever
/// store the query is later pointed at.
/// </summary>
internal sealed class QueryHttpWriter : QueryRenderer<string>
{
    internal QueryHttpWriter(QuerySpec spec, QuerySchema schema) : base(spec, schema)
    {
    }

    /// <summary>Anything a query can express can be written down, so nothing is refused here.</summary>
    internal static IQueryCapabilities Capabilities { get; } = QueryCapabilities.All;

    /// <inheritdoc/>
    protected override string TargetName => "The query-string writer";

    internal string Run()
    {
        Prepare(Capabilities);

        var parts = new List<string> { "from=" + Source(Spec.From.Entity, Spec.From.Call, Spec.From.Alias) };
        for (var i = 0; i < Spec.Joins.Count; i++) parts.Add("join=" + Join(Spec.Joins[i]));

        if (Spec.Select.Count > 0) parts.Add("select=" + string.Join(",", Spec.Select.Select(Selected)));

        var where = Filter(Spec.Where);
        if (!string.IsNullOrEmpty(where)) parts.Add("where=" + Unwrap(where!));

        if (Spec.GroupBy.Count > 0) parts.Add("groupby=" + string.Join(",", Spec.GroupBy.Select(Grouped)));

        // The output names a grouping filter tests are written by the select items above, so the
        // expressions behind them are never needed here.
        foreach (var item in Spec.Select)
        {
            if (!string.IsNullOrEmpty(item.Alias)) OutputExpressions[item.Alias!] = item.Alias!;
        }
        var having = Filter(Spec.Having);
        if (!string.IsNullOrEmpty(having)) parts.Add("having=" + Unwrap(having!));

        if (Spec.OrderBy.Count > 0) parts.Add("orderby=" + string.Join(",", Spec.OrderBy.Select(Sorted)));
        if (Spec.Distinct) parts.Add("distinct=true");
        if (Spec.Limit is not null) parts.Add("top=" + Number(Spec.Limit.Value));
        if (Spec.Offset is not null) parts.Add("skip=" + Number(Spec.Offset.Value));

        return string.Join("&", parts);
    }

    // The outermost brackets say nothing, since a parameter is one expression by definition.
    private static string Unwrap(string filter)
        => filter.Length > 1 && filter[0] == '(' && filter[filter.Length - 1] == ')' && Balanced(filter)
            ? filter.Substring(1, filter.Length - 2)
            : filter;

    private static bool Balanced(string text)
    {
        var depth = 0;
        var quoted = false;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\'') quoted = !quoted;
            else if (quoted) continue;
            else if (text[i] == '(') depth++;
            else if (text[i] == ')' && --depth == 0 && i != text.Length - 1) return false;
        }
        return true;
    }

    private string Source(string? entity, QueryFunctionCall? call, string alias)
        => (call is not null ? RenderCall(call) : entity!) + ":" + alias;

    private string Join(QueryJoin join)
    {
        var match = join.On is { Count: > 0 }
            ? "on(" + string.Join(",", join.On.Select(pair => $"{pair.Left}={pair.Right}")) + ")"
            : join.Relation ?? string.Empty;

        var fields = new List<string>
        {
            join.Call is not null ? RenderCall(join.Call) : join.Entity!,
            join.Alias,
            match,
            join.Kind.ToString().ToLowerInvariant(),
            join.From ?? string.Empty,
        };

        // A trailing field that says nothing is left out; the reader fills in the same defaults.
        while (fields.Count > 2 && fields[fields.Count - 1].Length == 0) fields.RemoveAt(fields.Count - 1);
        if (fields.Count == 4 && fields[3] == "inner") fields.RemoveAt(3);
        return string.Join(":", fields);
    }

    private string Selected(QuerySelect item)
    {
        var body = item.Aggregate is null
            ? Period(Value(item.Field, item.Call), item.Truncate)
            : Aggregated(item);
        return string.IsNullOrEmpty(item.Alias) ? body : body + " as " + item.Alias;
    }

    private string Aggregated(QuerySelect item)
    {
        var name = item.Aggregate!.Value.ToString().ToLowerInvariant();
        // A row count names nothing, which is exactly what tells the two counts apart.
        var inner = item.Field is null && item.Call is null ? string.Empty : Value(item.Field, item.Call);
        if (item.Distinct) inner = "distinct " + inner;
        if (item.Aggregate == QueryAggregate.Percentile && item.Percentile is not null)
        {
            inner += ", " + item.Percentile.Value.ToString("R", CultureInfo.InvariantCulture);
        }
        return $"{name}({inner})";
    }

    private string Grouped(QueryGroupBy group)
    {
        var body = Period(Value(group.Field, group.Call), group.Truncate);
        return string.IsNullOrEmpty(group.Alias) ? body : body + " as " + group.Alias;
    }

    private string Sorted(QuerySort sort)
    {
        var body = string.IsNullOrEmpty(sort.Select) ? Value(sort.Field, sort.Call) : sort.Select!;
        return sort.Direction == QuerySortDirection.Descending ? body + " desc" : body;
    }

    private static string Period(string value, QueryDateTruncation? truncate)
        => truncate is null ? value : value + ":" + truncate.Value.ToString().ToLowerInvariant();

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private string RenderCall(QueryFunctionCall call)
    {
        var function = Schema.FindFunction(call.Function)!;
        var arguments = new List<string>(call.Arguments.Count);
        for (var i = 0; i < call.Arguments.Count; i++)
        {
            var type = i < function.Parameters.Count ? function.Parameters[i].Type : QueryFieldType.Text;
            arguments.Add(Operand(call.Arguments[i], type));
        }
        return $"{call.Function}({string.Join(",", arguments)})";
    }

    // ---- What each node means as text -------------------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field) => $"{alias}.{field.Key}";

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type)
    {
        // Back to the exact form the query stores, so reading it returns the same value rather than
        // one that merely looks the same.
        var stored = QueryValue.ToInvariant(value);
        return stored is null ? "null" : "'" + stored.Replace("'", "''") + "'";
    }

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
    {
        var unit = offset.Unit switch
        {
            QueryTimeUnit.Minute => "min",
            QueryTimeUnit.Hour => "h",
            QueryTimeUnit.Day => "d",
            QueryTimeUnit.Week => "w",
            QueryTimeUnit.Month => "mon",
            QueryTimeUnit.Quarter => "q",
            _ => "y",
        };
        var sign = offset.Amount < 0 ? "-" : "+";
        return sign + Math.Abs(offset.Amount).ToString(CultureInfo.InvariantCulture) + unit;
    }

    /// <inheritdoc/>
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => $"{function.Key}({string.Join(",", arguments)})";

    /// <inheritdoc/>
    protected override string Comparison(
        string left, QueryOperator op, QueryFieldType type, string? right, string? upper) => op switch
    {
        QueryOperator.IsNull => left + " is null",
        QueryOperator.IsNotNull => left + " is not null",
        QueryOperator.Between => $"{left} between {right} and {upper}",
        QueryOperator.NotBetween => $"{left} not between {right} and {upper}",
        _ => $"{left} {Word(op)} {right}",
    };

    /// <inheritdoc/>
    protected override string Membership(string left, QueryOperator op, IReadOnlyList<string> values)
        => $"{left} {(op == QueryOperator.NotIn ? "not in" : "in")} ({string.Join(",", values)})";

    /// <inheritdoc/>
    protected override string Combine(bool or, IReadOnlyList<string> parts)
        => "(" + string.Join(or ? " or " : " and ", parts) + ")";

    internal static string Word(QueryOperator op) => op switch
    {
        QueryOperator.Equals => "eq",
        QueryOperator.NotEquals => "ne",
        QueryOperator.GreaterThan => "gt",
        QueryOperator.GreaterThanOrEqual => "ge",
        QueryOperator.LessThan => "lt",
        QueryOperator.LessThanOrEqual => "le",
        QueryOperator.Contains => "contains",
        QueryOperator.StartsWith => "startswith",
        QueryOperator.EndsWith => "endswith",
        _ => op.ToString().ToLowerInvariant(),
    };
}
