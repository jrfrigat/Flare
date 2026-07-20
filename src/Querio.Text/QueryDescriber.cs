using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Querio.Text;

/// <summary>
/// The wording a description is built from. Every connective is swappable, so the same query can be
/// described in another language without touching how it is walked - and, because reading uses the
/// same words, a query can be written and read back in that language too.
/// </summary>
public sealed record QueryDescriptionLabels
{
    /// <summary>The set used when a caller supplies none.</summary>
    public static QueryDescriptionLabels Default { get; } = new();

    /// <summary>Introduces the entity the query draws from.</summary>
    public string From { get; init; } = "from";

    /// <summary>Introduces another entity brought into the query.</summary>
    public string JoinedWith { get; init; } = "joined with";

    /// <summary>Introduces the relation a join travels along.</summary>
    public string Through { get; init; } = "through";

    /// <summary>Introduces an explicit join match, when no relation names it.</summary>
    public string Matching { get; init; } = "matching";

    /// <summary>Introduces what the query returns.</summary>
    public string Showing { get; init; } = "showing";

    /// <summary>Introduces the name something returned is given.</summary>
    public string Called { get; init; } = "called";

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

    /// <summary>Says repeated rows are dropped.</summary>
    public string WithoutDuplicates { get; init; } = "without duplicates";

    /// <summary>Joins two conditions that must both hold.</summary>
    public string And { get; init; } = "and";

    /// <summary>Joins two conditions of which either may hold.</summary>
    public string Or { get; init; } = "or";

    /// <summary>Introduces what a function is applied to.</summary>
    public string Of { get; init; } = "of";

    /// <summary>Says only differing values are considered.</summary>
    public string Distinct { get; init; } = "distinct";

    /// <summary>Says a value counts rows rather than values.</summary>
    public string RowCount { get; init; } = "the number of rows";

    /// <summary>Stands for a value that is absent.</summary>
    public string Nothing { get; init; } = "nothing";

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

    /// <summary>How each way of keeping unmatched rows reads. An inner join adds nothing.</summary>
    public IReadOnlyDictionary<QueryJoinKind, string> JoinKinds { get; init; } =
        new Dictionary<QueryJoinKind, string>
        {
            [QueryJoinKind.Inner] = "",
            [QueryJoinKind.Left] = "keeping unmatched rows on the left",
            [QueryJoinKind.Right] = "keeping unmatched rows on the right",
            [QueryJoinKind.Full] = "keeping unmatched rows on both sides",
            [QueryJoinKind.Cross] = "paired with every row",
        };

    /// <summary>How each period reads.</summary>
    public IReadOnlyDictionary<QueryDateTruncation, string> Periods { get; init; } =
        new Dictionary<QueryDateTruncation, string>
        {
            [QueryDateTruncation.Minute] = "minute",
            [QueryDateTruncation.Hour] = "hour",
            [QueryDateTruncation.Day] = "day",
            [QueryDateTruncation.Week] = "week",
            [QueryDateTruncation.Month] = "month",
            [QueryDateTruncation.Quarter] = "quarter",
            [QueryDateTruncation.Year] = "year",
        };

    /// <summary>How each unit of time reads, singular and plural.</summary>
    public IReadOnlyDictionary<QueryTimeUnit, string> Units { get; init; } =
        new Dictionary<QueryTimeUnit, string>
        {
            [QueryTimeUnit.Minute] = "minute",
            [QueryTimeUnit.Hour] = "hour",
            [QueryTimeUnit.Day] = "day",
            [QueryTimeUnit.Week] = "week",
            [QueryTimeUnit.Month] = "month",
            [QueryTimeUnit.Quarter] = "quarter",
            [QueryTimeUnit.Year] = "year",
        };

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
/// Describes a query in words, and reads one back out of them.
/// <para>
/// Written in the labels a person chose rather than the names a store uses, so it reads the way the
/// query was built. Enough is written down that nothing is lost: aliases, joins and output names all
/// appear, and a value is quoted in the exact form the query stores it in. A description that could
/// not be read back would be a summary, not a translation.
/// </para>
/// </summary>
public sealed class QueryDescriber : QueryRenderer<string>
{
    private readonly QueryDescriptionLabels _labels;
    private bool _qualify;

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

    /// <summary>
    /// Reads a description back into the query it describes. The words come from the same labels, so
    /// a description written in one language reads back in that language.
    /// </summary>
    /// <param name="description">The sentence to read.</param>
    /// <param name="schema">The schema it was written against, which supplies the vocabulary.</param>
    /// <param name="labels">The wording it was written with. Defaults to English.</param>
    /// <exception cref="QueryParseException">The sentence does not read as a query.</exception>
    public static QuerySpec Parse(
        string description, QuerySchema schema, QueryDescriptionLabels? labels = null)
        => QueryTextReader.Read(description, schema, labels ?? QueryDescriptionLabels.Default);

    private string Run()
    {
        Prepare(Capabilities);
        // One table needs no qualifying, and saying "(r)" after every field would only be noise.
        // More than one and every field has to say which it came from. Reading applies the same rule.
        _qualify = Participants.Count > 1;

        var parts = new List<string> { $"{_labels.From} {Participant(0)}" };
        for (var i = 0; i < Spec.Joins.Count; i++) parts.Add(Joined(Spec.Joins[i], i));

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

        if (Spec.Distinct) parts.Add(_labels.WithoutDuplicates);
        if (Spec.Offset is > 0) parts.Add($"{_labels.Skipping} {Number(Spec.Offset.Value)}");
        if (Spec.Limit is not null) parts.Add($"{_labels.First} {Number(Spec.Limit.Value)}");

        // Substring rather than a range: netstandard2.0 has no System.Index.
        var sentence = string.Join(", ", parts);
        return char.ToUpperInvariant(sentence[0]) + sentence.Substring(1);
    }

    private string Participant(int index)
    {
        var participant = Participants[index];
        var source = index == 0 ? Spec.From.Call : Spec.Joins[index - 1].Call;
        var label = source is not null
            ? RenderCall(source)
            : Schema.FindEntity(participant.EntityKey!)?.Label ?? participant.Label;
        return $"{label} ({participant.Alias})";
    }

    private string Joined(QueryJoin join, int index)
    {
        var text = $"{_labels.JoinedWith} {Participant(index + 1)}";

        if (join.On is { Count: > 0 })
        {
            var pairs = join.On.Select(pair => $"{Reference(pair.Left)} {_labels.Operators[QueryOperator.Equals]} {Reference(pair.Right)}");
            text += $" {_labels.Matching} {Join(pairs.ToList(), _labels.And)}";
        }
        else if (!string.IsNullOrEmpty(join.Relation))
        {
            var relation = Schema.FindRelation(join.Relation!);
            text += $" {_labels.Through} {relation?.Label ?? join.Relation}";
        }

        // Which side an ambiguous join hangs off is part of the query, so it has to be written down.
        if (!string.IsNullOrEmpty(join.From)) text += $" ({join.From})";

        var kind = _labels.JoinKinds.TryGetValue(join.Kind, out var word) ? word : string.Empty;
        return kind.Length == 0 ? text : $"{text} {kind}";
    }

    private string Reference(QueryFieldRef reference)
    {
        var field = FindField(reference.Alias, reference.Field);
        var label = field?.Label ?? reference.Field;
        return _qualify ? $"{label} ({reference.Alias})" : label;
    }

    private string DescribeSelect()
    {
        var items = new List<string>(Spec.Select.Count);
        foreach (var item in Spec.Select)
        {
            var text = DescribeSelected(item);
            if (!string.IsNullOrEmpty(item.Alias))
            {
                text += $" {_labels.Called} {item.Alias}";
                // Recorded so a grouping filter naming the aggregate reads as that name too.
                OutputExpressions[item.Alias!] = item.Alias!;
                OutputTypes[item.Alias!] = item.Aggregate is null
                    ? ValueType(item.Field, item.Call)
                    : QueryFieldType.Number;
            }
            items.Add(text);
        }
        return Join(items, _labels.And);
    }

    private string DescribeSelected(QuerySelect item)
    {
        if (item.Aggregate is null) return Period(Value(item.Field, item.Call), item.Truncate);

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
        var inner = Value(item.Field, item.Call);
        if (item.Distinct) inner = $"{_labels.Distinct} {inner}";
        return string.Format(CultureInfo.InvariantCulture, pattern, inner, rank);
    }

    private string DescribeGroup(QueryGroupBy group)
    {
        var text = Period(Value(group.Field, group.Call), group.Truncate);
        return string.IsNullOrEmpty(group.Alias) ? text : $"{text} {_labels.Called} {group.Alias}";
    }

    private string DescribeSort(QuerySort sort)
    {
        // An ordering by something already shown names it, rather than repeating what it was.
        var subject = string.IsNullOrEmpty(sort.Select) ? Value(sort.Field, sort.Call) : sort.Select!;
        var direction = sort.Direction == QuerySortDirection.Descending ? _labels.Descending : _labels.Ascending;
        return $"{subject} {direction}";
    }

    private string Period(string value, QueryDateTruncation? truncation)
        => truncation is null
            ? value
            : value + " " + string.Format(
                CultureInfo.InvariantCulture, _labels.PerPeriod, _labels.Periods[truncation.Value]);

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private string Join(IReadOnlyList<string> parts, string connective)
    {
        if (parts.Count == 0) return string.Empty;
        if (parts.Count == 1) return parts[0];
        var head = string.Join(", ", parts.Take(parts.Count - 1));
        return $"{head} {connective} {parts[parts.Count - 1]}";
    }

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
        return Call(function ?? new QueryFunction(call.Function, call.Function, QueryFunctionKind.Value), arguments);
    }

    // ---- What each node means as words -----------------------------------------------------------

    /// <inheritdoc/>
    protected override string Field(string alias, QueryField field)
        => _qualify ? $"{field.Label} ({alias})" : field.Label;

    /// <inheritdoc/>
    protected override string Literal(object? value, QueryFieldType type)
    {
        // Quoted, and in the form the query stores rather than a prettier one: a description that
        // rounded a moment or dropped the quotes could not be read back as the same query.
        var stored = QueryValue.ToInvariant(value);
        return stored is null ? _labels.Nothing : "\"" + stored.Replace("\"", "\"\"") + "\"";
    }

    /// <inheritdoc/>
    protected override string Relative(QueryRelativeValue offset)
    {
        var amount = Math.Abs(offset.Amount);
        var unit = _labels.Units.TryGetValue(offset.Unit, out var word) ? word : offset.Unit.ToString();
        if (amount != 1) unit += "s";
        var pattern = offset.Amount < 0 ? _labels.LastWindow : _labels.NextWindow;
        return string.Format(CultureInfo.InvariantCulture, pattern, Number(amount), unit);
    }

    /// <inheritdoc/>
    protected override string Call(QueryFunction function, IReadOnlyList<string> arguments)
        => arguments.Count == 0
            ? function.Label
            : $"{function.Label} {_labels.Of} {Join(arguments, _labels.And)}";

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
