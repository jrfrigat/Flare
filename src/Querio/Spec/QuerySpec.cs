namespace Querio;

/// <summary>
/// A complete query, described but not executed. This is the whole contract: a builder - visual or
/// fluent - produces one of these, and a renderer turns it into whatever its store speaks. Nothing
/// here is dialect-specific, and no part of it holds query text, so the same object can drive a SQL
/// database, a document store or an HTTP API.
/// <para>
/// It is designed to be serialized and kept. Consumers persist saved reports as JSON, so treat the
/// shape as a data contract: adding optional members is safe, repurposing existing ones is not.
/// </para>
/// <para>
/// Beware that the collection members compare by reference under the synthesized record equality, so
/// two structurally identical specs built separately are not <c>==</c> to each other. Compare
/// serialized form when structural comparison is what is meant.
/// </para>
/// </summary>
/// <param name="From">The root participant every other one is joined to.</param>
public sealed record QuerySpec(QuerySource From)
{
    /// <summary>Entities brought into the query, each under its own alias.</summary>
    public IReadOnlyList<QueryJoin> Joins { get; init; } = [];

    /// <summary>What the query returns. Empty means the renderer decides, typically every field.</summary>
    public IReadOnlyList<QuerySelect> Select { get; init; } = [];

    /// <summary>Conditions applied to rows before grouping. Null applies none.</summary>
    public QueryFilterGroup? Where { get; init; }

    /// <summary>Grouping levels, outermost first. Empty returns ungrouped rows.</summary>
    public IReadOnlyList<QueryGroupBy> GroupBy { get; init; } = [];

    /// <summary>Conditions applied to groups after aggregation. Null applies none.</summary>
    public QueryFilterGroup? Having { get; init; }

    /// <summary>Ordering levels, most significant first.</summary>
    public IReadOnlyList<QuerySort> OrderBy { get; init; } = [];

    /// <summary>Drops duplicate rows from the result.</summary>
    public bool Distinct { get; init; }

    /// <summary>Caps how many rows come back. Null returns them all.</summary>
    public int? Limit { get; init; }

    /// <summary>Skips this many rows before returning any. Null starts at the first row.</summary>
    public int? Offset { get; init; }
}
