namespace Querio;

/// <summary>
/// The operators and aggregates that make sense for each <see cref="QueryFieldType"/>. A schema that
/// leaves <see cref="QueryField.Operators"/> or <see cref="QueryField.Aggregates"/> null falls back
/// to these, so the common case needs no configuration and an explicit list is only ever used to
/// narrow the choice.
/// </summary>
public static class QueryDefaults
{
    private static readonly QueryOperator[] TextOperators =
    [
        QueryOperator.Contains, QueryOperator.Equals, QueryOperator.NotEquals,
        QueryOperator.StartsWith, QueryOperator.EndsWith,
        QueryOperator.In, QueryOperator.NotIn,
        QueryOperator.IsNull, QueryOperator.IsNotNull,
    ];

    private static readonly QueryOperator[] OrderedOperators =
    [
        QueryOperator.Equals, QueryOperator.NotEquals,
        QueryOperator.GreaterThan, QueryOperator.GreaterThanOrEqual,
        QueryOperator.LessThan, QueryOperator.LessThanOrEqual,
        QueryOperator.Between, QueryOperator.NotBetween,
        QueryOperator.In, QueryOperator.NotIn,
        QueryOperator.IsNull, QueryOperator.IsNotNull,
    ];

    private static readonly QueryOperator[] DateOperators =
    [
        QueryOperator.Equals, QueryOperator.NotEquals,
        QueryOperator.GreaterThan, QueryOperator.GreaterThanOrEqual,
        QueryOperator.LessThan, QueryOperator.LessThanOrEqual,
        QueryOperator.Between, QueryOperator.NotBetween,
        QueryOperator.IsNull, QueryOperator.IsNotNull,
    ];

    private static readonly QueryOperator[] SetOperators =
    [
        QueryOperator.Equals, QueryOperator.NotEquals,
        QueryOperator.In, QueryOperator.NotIn,
        QueryOperator.IsNull, QueryOperator.IsNotNull,
    ];

    private static readonly QueryOperator[] BooleanOperators =
    [
        QueryOperator.Equals, QueryOperator.NotEquals,
        QueryOperator.IsNull, QueryOperator.IsNotNull,
    ];

    private static readonly QueryAggregate[] CountOnly = [QueryAggregate.Count];

    private static readonly QueryAggregate[] NumericAggregates =
    [
        QueryAggregate.Count, QueryAggregate.Sum, QueryAggregate.Avg,
        QueryAggregate.Min, QueryAggregate.Max, QueryAggregate.Percentile,
    ];

    private static readonly QueryAggregate[] DateAggregates =
        [QueryAggregate.Count, QueryAggregate.Min, QueryAggregate.Max];

    /// <summary>The operators offered for a field of the given type when the schema does not narrow them.</summary>
    /// <param name="type">The field's semantic kind.</param>
    public static IReadOnlyList<QueryOperator> OperatorsFor(QueryFieldType type) => type switch
    {
        QueryFieldType.Text => TextOperators,
        QueryFieldType.Number => OrderedOperators,
        QueryFieldType.DateTime => DateOperators,
        QueryFieldType.Boolean => BooleanOperators,
        QueryFieldType.Guid => SetOperators,
        QueryFieldType.Enum => SetOperators,
        _ => SetOperators,
    };

    /// <summary>The aggregates offered for a field of the given type when the schema does not narrow them.</summary>
    /// <param name="type">The field's semantic kind.</param>
    public static IReadOnlyList<QueryAggregate> AggregatesFor(QueryFieldType type) => type switch
    {
        QueryFieldType.Number => NumericAggregates,
        QueryFieldType.DateTime => DateAggregates,
        _ => CountOnly,
    };

    /// <summary>Whether the operator compares against no operand at all (the null checks).</summary>
    /// <param name="op">The operator to classify.</param>
    public static bool TakesNoValue(QueryOperator op)
        => op is QueryOperator.IsNull or QueryOperator.IsNotNull;

    /// <summary>Whether the operator needs a second operand (the range comparisons).</summary>
    /// <param name="op">The operator to classify.</param>
    public static bool TakesTwoValues(QueryOperator op)
        => op is QueryOperator.Between or QueryOperator.NotBetween;

    /// <summary>Whether the operator compares against a set rather than a single operand.</summary>
    /// <param name="op">The operator to classify.</param>
    public static bool TakesValueList(QueryOperator op)
        => op is QueryOperator.In or QueryOperator.NotIn;
}
