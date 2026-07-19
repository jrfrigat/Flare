namespace Querio;

/// <summary>What kind of thing a query draws its rows from.</summary>
public enum QuerySourceKind
{
    /// <summary>An entity declared in the schema.</summary>
    Entity,
}

/// <summary>
/// The query's root participant. <see cref="Kind"/> exists so that a future source - a subquery, a
/// derived table - can be added without reshaping the contract that saved queries are stored in.
/// </summary>
/// <param name="Entity">Key of the entity to draw from.</param>
/// <param name="Alias">Alias every field reference to this participant uses.</param>
public sealed record QuerySource(string Entity, string Alias)
{
    /// <summary>What this source draws from. Currently always an entity.</summary>
    public QuerySourceKind Kind { get; init; } = QuerySourceKind.Entity;
}

/// <summary>How unmatched rows on either side of a join are treated.</summary>
public enum QueryJoinKind
{
    /// <summary>Keeps only rows matched on both sides.</summary>
    Inner,

    /// <summary>Keeps every row already in the query, matched or not.</summary>
    Left,

    /// <summary>Keeps every row of the joined entity, matched or not.</summary>
    Right,

    /// <summary>Keeps unmatched rows from both sides.</summary>
    Full,

    /// <summary>Pairs every row with every other row, with no matching condition at all.</summary>
    Cross,
}

/// <summary>One field-to-field match inside an explicit join condition.</summary>
/// <param name="Left">Field on a participant already in the query.</param>
/// <param name="Right">Field on the entity being joined.</param>
public sealed record QueryJoinCondition(QueryFieldRef Left, QueryFieldRef Right);

/// <summary>
/// Brings another entity into the query under its own alias.
/// <para>
/// Prefer <see cref="Relation"/>: naming a schema relation states the intent, survives a schema
/// changing its key columns, and gives a renderer the freedom to express the traversal in whatever
/// way its dialect prefers. <see cref="On"/> is the escape hatch for a join the schema has not
/// declared; when both are set the relation wins.
/// </para>
/// </summary>
/// <param name="Entity">Key of the entity being joined in.</param>
/// <param name="Alias">Alias every field reference to this participant uses.</param>
public sealed record QueryJoin(string Entity, string Alias)
{
    /// <summary>How unmatched rows are treated. Defaults to <see cref="QueryJoinKind.Inner"/>.</summary>
    public QueryJoinKind Kind { get; init; } = QueryJoinKind.Inner;

    /// <summary>Key of the schema relation to traverse. The preferred way to express a join.</summary>
    public string? Relation { get; init; }

    /// <summary>
    /// Alias of the participant this join attaches to. Optional: with one candidate a renderer finds
    /// it, but once the same entity appears twice - a chain of managers, say - only the caller knows
    /// which occurrence was meant, and a renderer must not guess.
    /// </summary>
    public string? From { get; init; }

    /// <summary>Explicit match conditions, for a join the schema does not declare.</summary>
    public IReadOnlyList<QueryJoinCondition>? On { get; init; }
}
