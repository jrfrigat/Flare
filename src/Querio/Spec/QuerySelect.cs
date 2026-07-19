namespace Querio;

/// <summary>
/// One item the query returns: a plain field, or an aggregate computed over the group.
/// <para>
/// <see cref="Field"/> is null only for a row count, which counts rows rather than values.
/// <see cref="Percentile"/> must be set when <see cref="Aggregate"/> is
/// <see cref="QueryAggregate.Percentile"/>, and is ignored otherwise.
/// </para>
/// </summary>
public sealed record QuerySelect
{
    /// <summary>The field to return or aggregate. Null means count rows rather than values.</summary>
    public QueryFieldRef? Field { get; init; }

    /// <summary>The aggregate to compute. Null returns the field itself.</summary>
    public QueryAggregate? Aggregate { get; init; }

    /// <summary>Considers only distinct values, as in a distinct count.</summary>
    public bool Distinct { get; init; }

    /// <summary>The rank for a percentile aggregate, as a fraction: 0.95 means the 95th percentile.</summary>
    public double? Percentile { get; init; }

    /// <summary>Output name for the returned column. Falls back to a renderer-chosen name when null.</summary>
    public string? Alias { get; init; }
}
