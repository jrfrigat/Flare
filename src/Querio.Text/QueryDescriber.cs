using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.Text;

/// <summary>
/// The wording a description is built from. Every connective is swappable, so the same query can be
/// described in another language without touching how it is walked.
/// </summary>
public sealed record QueryDescriptionLabels
{
    /// <summary>The set used when a caller supplies none.</summary>
    public static QueryDescriptionLabels Default { get; } = new();

    /// <summary>Introduces the entity the query draws from.</summary>
    public string From { get; init; } = "from";

    /// <summary>Introduces what the query returns.</summary>
    public string Showing { get; init; } = "showing";

    /// <summary>Introduces the conditions.</summary>
    public string Where { get; init; } = "where";

    /// <summary>Introduces the grouping.</summary>
    public string GroupedBy { get; init; } = "grouped by";

    /// <summary>Introduces the conditions applied to groups.</summary>
    public string Having { get; init; } = "keeping groups where";

    /// <summary>Introduces the ordering.</summary>
    public string OrderedBy { get; init; } = "ordered by";

    /// <summary>Introduces a row cap.</summary>
    public string First { get; init; } = "first";

    /// <summary>Introduces a number of skipped rows.</summary>
    public string Skipping { get; init; } = "skipping";

    /// <summary>Joins two conditions that must both hold.</summary>
    public string And { get; init; } = "and";

    /// <summary>Joins two conditions of which either may hold.</summary>
    public string Or { get; init; } = "or";

    /// <summary>Says a value counts rows rather than values.</summary>
    public string RowCount { get; init; } = "the number of rows";

    /// <summary>Says an ordering runs from smallest to largest.</summary>
    public string Ascending { get; init; } = "ascending";

    /// <summary>Says an ordering runs from largest to smallest.</summary>
    public string Descending { get; init; } = "descending";

    /// <summary>Says a window reaches into the past, with the amount and unit filled in.</summary>
    public string LastWindow { get; init; } = "the last {0} {1}";

    /// <summary>Says a window reaches into the future, with the amount and unit filled in.</summary>
    public string NextWindow { get; init; } = "the next {0} {1}";

    /// <summary>Says a value is grouped into periods, with the period filled in.</summary>
    public string PerPeriod { get; init; } = "by {0}";

    /// <summary>How each operator reads. Anything missing falls back to the operator's name.</summary>
    public IReadOnlyDictionary<QueryOperator, string> Operators { get; init; } =
        new Dictionary<QueryOperator, string>
        {
            [QueryOperator.Contains] = "contains",
            [QueryOperator.Equals] = "is",
            [QueryOperator.NotEquals] = "is not",
            [QueryOperator.StartsWith] = "starts with",
            [QueryOperator.EndsWith] = "ends with",
            [QueryOperator.GreaterThan] = "is more than",
            [QueryOperator.GreaterThanOrEqual] = "is at least",
            [QueryOperator.LessThan] = "is less than",
            [QueryOperator.LessThanOrEqual] = "is at most",
            [QueryOperator.Between] = "is between",
            [QueryOperator.NotBetween] = "is outside",
            [QueryOperator.In] = "is one of",
            [QueryOperator.NotIn] = "is none of",
            [QueryOperator.IsNull] = "is empty",
            [QueryOperator.IsNotNull] = "is not empty",
        };

    /// <summary>How each aggregate reads, with the value filled in.</summary>
    public IReadOnlyDictionary<QueryAggregate, string> Aggregates { get; init; } =
        new Dictionary<QueryAggregate, string>
        {
            [QueryAggregate.Count] = "the number of {0}",
            [QueryAggregate.Sum] = "the total {0}",
            [QueryAggregate.Avg] = "the average {0}",
            [QueryAggregate.Min] = "the smallest {0}",
            [QueryAggregate.Max] = "the largest {0}",
            [QueryAggregate.Percentile] = "the {1} percentile of {0}",
        };
}

/// <summary>
/// Describes a query in words. It is a renderer like any other - it walks the same query and the
/// same schema - which is the clearest demonstration that a target need not produce a query at all.
/// <para>
/// Labels rather than physical names are used throughout, so the description reads the way a person
/// picked the fields rather than the way the store stores them.
/// </para>
/// </summary>
public sealed class QueryDescriber : QueryRenderer<string>
{
    private readonly QueryDescriptionLabels _labels;

    private QueryDescriber(QuerySpec spec, QuerySchema schema, QueryDescriptionLabels labels)
        : base(spec, schema) => _labels = labels;

    /// <summary>Anything a query can express can be described, so nothing is refused here.</summary>
    public static IQueryCapabilities Capabilities { get; } = QueryCapabilities.All;

    /// <inheritdoc/>
    protected override string TargetName => "The description renderer";

    /// <summary>Describes a query as a sentence.</summary>
    /// <param name="spec">The query to describe.</param>
    /// <param name="schema">The schema it was built against, which supplies the labels.</param>
    /// <param name="labels">The wording to use. Defaults to English.</param>
    public static string Describe(QuerySpec spec, QuerySchema schema, QueryDescriptionLabels? labels = null)
        => new QueryDescriber(spec, schema, labels ?? QueryDescriptionLabels.Default).Run();

    private string Run()
    {
        Prepare(Capabilities);

        var parts = new List<string>();
        var root = Participants[0];
        parts.Add($"{_labels.From} {EntityLabel(root)}");

        var selected = DescribeSelect();
        if (selected.Length > 0) parts.Add($"{_labels.Showing} {selected}");

        var where = Filter(Spec.Where);
        if (!string.IsNullOrEmpty(where)) parts.Add($"{_labels.Where} {where}");

        if (Spec.GroupBy.Count > 0)
        {
            var keys = Spec.GroupBy.Select(DescribeGroup).ToList();
            parts.Add($"{_labels.GroupedBy} {Join(keys, _labels.And)}");
        }

        var having = Filter(Spec.Having);
        if (!string.IsNullOrEmpty(having)) parts.Add($"{_labels.Having} {having}");

        if (Spec.OrderBy.Count > 0)
        {
            var orders = Spec.OrderBy.Select(DescribeSort).ToList();
            parts.Add($"{_labels.OrderedBy} {Join(orders, _labels.And)}");
        }

        if (Spec.Offset is > 0) parts.Add($"{_labels.Skipping} {Number(Spec.Offset.Value)}");
        if (Spec.Limit is not null) parts.Add($"{_labels.First} {Number(Spec.Limit.Value)}");

        // Substring rather than a range: netstandard2.0 has no System.Index.
        var sentence = string.Join(", ", parts);
        return char.ToUpperInvariant(sentence[0]) + sentence.Substring(1);
    }

    private string EntityLabel(QueryParticipantShape participant)
    {
        if (participant.EntityKey is null) return Schema.FindFunction(participant.Label)?.Label ?? participant.Label;
        return Schema.FindEntity(participant.EntityKey)?.Label ?? participant.Label;
    }

    private string DescribeSelect()
    {
        var items = new List<string>(Spec.Select.Count);
        foreach (var item in Spec.Select)
        {
            var text = DescribeSelected(item);
            if (!string.IsNullOrEmpty(item.Alias))
            {
                // Recorded so a grouping filter that names the aggregate reads as words too.
                OutputExpressions[item.Alias!] = text;
                OutputTypes[item.Alias!] = item.Aggregate is QueryAggregate.Count or QueryAggregate.Percentile
                    ? QueryFieldType.Number
                    : item.Field is null && item.Call is null
                        ? QueryFieldType.Number
                        : ValueType(item.Field, item.Call);
            }
            items.Add(text);
        }
        return Join(items, _labels.And);
    }

    private string DescribeSelected(QuerySelect item)
    {
        if (item.Aggregate is null)
        {
            var plain = Value(item.Field, item.Call);
            return item.Truncate is null
                ? plain
                : plain + " " + string.Format(CultureInfo.InvariantCulture, _labels.PerPeriod, Period(item.Truncate.Value));
        }

        if (item.Aggregate == QueryAggregate.Count && item.Field is null && item.Call is null)
        {
            return _labels.RowCount;
        }

        var pattern = _labels.Aggregates.TryGetValue(item.Aggregate.Value, out var found)
            ? found
            : item.Aggregate.Value.ToString() + " {0}";
        var rank = item.Percentile is null
            ? string.Empty
            : (item.Percentile.Value * 100).ToString("0.##", CultureInfo.InvariantCulture);
        return string.Format(CultureInfo.InvariantCulture, pattern, Value(item.Field, item.Call), rank);
    }

    private string DescribeGroup(QueryGroupBy group)
    {
        var key = Value(group.Field, group.Call);
        return group.Truncate is null
            ? key
            : key + " " + string.Format(CultureInfo.InvariantCulture, _labels.PerPeriod, Period(group.Truncate.Value));
    }

    private string DescribeSort(QuerySort sort)
    {
        var subject = sort.Field is not null || sort.Call is not null
            ? Value(sort.Field, sort.Call)
            : OutputExpressions.TryGetValue(sort.Select!, out var known) ? known : sort.Select!;
        var direction = sort.Direction == QuerySortDirection.Descending ? _labels.Descending : _labels.Ascending;
        return $"{subject} {direction}";
    }

    private static string Period(QueryDateTruncation truncation) => truncation.ToString().ToLowerInvariant();

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private string Join(IReadOnlyList<string> parts, string connective)
    {
        if (parts.Count == 0) return string.Empty;
        if (parts.Count == 1) return parts[0];
        var head = string.Join(", ", parts.Take(parts.Count - 1));
        return $"{head} {connective} {parts[parts.Count - 1]}";
    }

    // ---- What each node means as words -----------------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field) => field.Label;

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type) => value switch
    {
        null => "nothing",
        bool flag => flag ? "true" : "false",
        DateTime moment => moment.ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty,
    };

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
    {
        var amount = Math.Abs(offset.Amount);
        var unit = offset.Unit.ToString().ToLowerInvariant() + (amount == 1 ? string.Empty : "s");
        var pattern = offset.Amount < 0 ? _labels.LastWindow : _labels.NextWindow;
        return string.Format(CultureInfo.InvariantCulture, pattern, Number(amount), unit);
    }

    /// <inheritdoc/>
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => arguments.Count == 0
            ? function.Label
            : $"{function.Label} of {Join(arguments, _labels.And)}";

    /// <inheritdoc/>
    protected override string Comparison(
        string left, QueryOperator op, QueryFieldType type, string? right, string? upper)
    {
        var verb = _labels.Operators.TryGetValue(op, out var found) ? found : op.ToString();
        if (right is null) return $"{left} {verb}";
        return upper is null
            ? $"{left} {verb} {right}"
            : $"{left} {verb} {right} {_labels.And} {upper}";
    }

    /// <inheritdoc/>
    protected override string Membership(string left, QueryOperator op, IReadOnlyList<string> values)
    {
        var verb = _labels.Operators.TryGetValue(op, out var found) ? found : op.ToString();
        return $"{left} {verb} {Join(values, _labels.Or)}";
    }

    /// <inheritdoc/>
    protected override string Combine(bool or, IReadOnlyList<string> parts)
        => "(" + string.Join($" {(or ? _labels.Or : _labels.And)} ", parts) + ")";
}
