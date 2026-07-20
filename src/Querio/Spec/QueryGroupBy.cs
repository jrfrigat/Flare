namespace Querio;

/// <summary>
/// Collapses a timestamp to the start of the period containing it, which is how a time series gets
/// its buckets. Semantic rather than syntactic: each renderer maps these onto its own date function.
/// </summary>
public enum QueryDateTruncation
{
    /// <summary>Start of the minute.</summary>
    Minute,

    /// <summary>Start of the hour.</summary>
    Hour,

    /// <summary>Start of the day.</summary>
    Day,

    /// <summary>Start of the week.</summary>
    Week,

    /// <summary>Start of the month.</summary>
    Month,

    /// <summary>Start of the quarter.</summary>
    Quarter,

    /// <summary>Start of the year.</summary>
    Year,
}

/// <summary>
/// One grouping level. Setting <see cref="Truncate"/> on a timestamp is what turns "every event" into
/// "events per day" without needing a separate computed column.
/// </summary>
/// <param name="Field">The field to group by. Null when a <see cref="Call"/> supplies the key.</param>
public sealed record QueryGroupBy(QueryFieldRef? Field)
{
    /// <summary>A call to a value function to group by, instead of a field.</summary>
    public QueryFunctionCall? Call { get; init; }

    /// <summary>Truncates a timestamp to a period before grouping. Null groups by the exact value.</summary>
    public QueryDateTruncation? Truncate { get; init; }

    /// <summary>Output name for the grouping column. Falls back to a renderer-chosen name when null.</summary>
    public string? Alias { get; init; }
}

/// <summary>Which way an ordering runs.</summary>
public enum QuerySortDirection
{
    /// <summary>Smallest first.</summary>
    Ascending,

    /// <summary>Largest first.</summary>
    Descending,
}

/// <summary>
/// One ordering level. Set either <see cref="Field"/> to order by a field, or <see cref="Select"/> to
/// order by the output name of a selected item - which is how a result gets ordered by a computed
/// aggregate such as the row count.
/// </summary>
public sealed record QuerySort
{
    /// <summary>The field to order by. Mutually exclusive with <see cref="Select"/> and <see cref="Call"/>.</summary>
    public QueryFieldRef? Field { get; init; }

    /// <summary>A call to a value function to order by, instead of a field.</summary>
    public QueryFunctionCall? Call { get; init; }

    /// <summary>Output name of a selected item to order by. Mutually exclusive with <see cref="Field"/>.</summary>
    public string? Select { get; init; }

    /// <summary>Direction of the ordering. Defaults to ascending.</summary>
    public QuerySortDirection Direction { get; init; } = QuerySortDirection.Ascending;
}
