namespace Querio;

/// <summary>
/// How many rows stand on each side of a relation. A many-to-many association is not a kind of its
/// own: model its junction entity explicitly and declare the two relations that reach it, which is
/// what a query has to traverse anyway.
/// </summary>
public enum QueryCardinality
{
    /// <summary>Many rows of the source point at one row of the target (the ordinary foreign key).</summary>
    ManyToOne,

    /// <summary>One row of the source is pointed at by many rows of the target.</summary>
    OneToMany,

    /// <summary>At most one row on each side.</summary>
    OneToOne,
}

/// <summary>
/// One column pair inside a relation's join condition. A composite key simply lists several pairs,
/// all of which must match.
/// </summary>
/// <param name="FromField">Field key on the relation's source entity.</param>
/// <param name="ToField">Field key on the relation's target entity.</param>
public sealed record QueryFieldPair(string FromField, string ToField);

/// <summary>
/// A declared path between two entities - a foreign key, or any other join the consumer wants to
/// offer. Joining through a relation is what keeps a query portable: the builder names the relation
/// and the renderer decides how to express it, whether as an explicit JOIN or, where the dialect
/// allows it, as a dotted reference.
/// </summary>
/// <param name="Key">Relation name, unique within the schema. Matched case-insensitively.</param>
/// <param name="From">Key of the source entity.</param>
/// <param name="To">Key of the target entity.</param>
/// <param name="On">Field pairs that must match, one per column of the key.</param>
public sealed record QueryRelation(
    string Key,
    string From,
    string To,
    IReadOnlyList<QueryFieldPair> On)
{
    /// <summary>Row multiplicity across the relation. Defaults to <see cref="QueryCardinality.ManyToOne"/>.</summary>
    public QueryCardinality Cardinality { get; init; } = QueryCardinality.ManyToOne;

    /// <summary>Human-readable caption for a relation picker. Falls back to <see cref="Key"/> when null.</summary>
    public string? Label { get; init; }

    /// <summary>Builds a relation over a single-column key, the common case.</summary>
    /// <param name="key">Relation name, unique within the schema.</param>
    /// <param name="from">Key of the source entity.</param>
    /// <param name="fromField">Field key on the source entity.</param>
    /// <param name="to">Key of the target entity.</param>
    /// <param name="toField">Field key on the target entity.</param>
    public static QueryRelation Simple(string key, string from, string fromField, string to, string toField)
        => new(key, from, to, [new QueryFieldPair(fromField, toField)]);
}
