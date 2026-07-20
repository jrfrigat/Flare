using System.Collections.Generic;

namespace Querio;

/// <summary>
/// How many operands an operator needs, which is what decides the shape of the editor a person is
/// given: nothing to fill in, one box, a pair of bounds, or a set.
/// </summary>
public enum QueryValueArity
{
    /// <summary>No operand at all, as in the null checks.</summary>
    None,

    /// <summary>A single operand.</summary>
    One,

    /// <summary>A lower and an upper bound.</summary>
    Two,

    /// <summary>A set of values.</summary>
    List,
}

/// <summary>
/// Something a query can be built on: an entity, or a table function standing where one would.
/// </summary>
/// <param name="Key">The entity or function key to name in the query.</param>
/// <param name="Label">Human-readable caption for a picker.</param>
/// <param name="Kind">Whether this is an entity or a table function.</param>
public sealed record QueryRootChoice(string Key, string Label, QuerySourceKind Kind)
{
    /// <summary>Arguments a table function has to be given. Empty for an entity.</summary>
    public IReadOnlyList<QueryFunctionParameter> Parameters { get; init; } = [];

    /// <summary>An alias not yet taken, offered so a caller need not invent one.</summary>
    public string SuggestedAlias { get; init; } = string.Empty;
}

/// <summary>One source already in the query, and the alias its fields are reached through.</summary>
/// <param name="Alias">The alias the query gave it.</param>
/// <param name="Label">Human-readable caption for the entity or function behind it.</param>
/// <param name="Entity">Key of the entity, or null when a table function stands here.</param>
/// <param name="Function">Key of the table function, or null when an entity stands here.</param>
public sealed record QueryParticipantChoice(string Alias, string Label, string? Entity, string? Function);

/// <summary>
/// One field the query can reach as it currently stands, with everything a caller needs to offer it:
/// what it is, whether it may be filtered or grouped, and which operators and aggregates apply.
/// </summary>
/// <param name="Alias">Alias of the participant the field belongs to.</param>
/// <param name="Field">Logical field name within that participant.</param>
/// <param name="Label">Human-readable caption for a picker.</param>
/// <param name="Type">The field's semantic kind.</param>
public sealed record QueryFieldChoice(string Alias, string Field, string Label, QueryFieldType Type)
{
    /// <summary>Caption of the participant, so a picker can group or qualify the field.</summary>
    public string ParticipantLabel { get; init; } = string.Empty;

    /// <summary>Whether the field can hold no value, so the null checks are worth offering.</summary>
    public bool Nullable { get; init; }

    /// <summary>Whether the field may appear in a condition.</summary>
    public bool Filterable { get; init; }

    /// <summary>Whether the field may appear in a grouping.</summary>
    public bool Groupable { get; init; }

    /// <summary>The operators offered here, already narrowed to what the target can do.</summary>
    public IReadOnlyList<QueryOperatorChoice> Operators { get; init; } = [];

    /// <summary>The aggregates offered here, already narrowed to what the target can do.</summary>
    public IReadOnlyList<QueryAggregate> Aggregates { get; init; } = [];

    /// <summary>Members of an enumerated field, for a value picker. Null for every other type.</summary>
    public IReadOnlyList<QueryEnumMember>? EnumMembers { get; init; }

    /// <summary>The reference a query carries for this field.</summary>
    public QueryFieldRef Reference => new(Alias, Field);

    /// <summary>Renders the choice as <c>alias.field</c>, for diagnostics and debugging.</summary>
    public override string ToString() => $"{Alias}.{Field}";
}

/// <summary>One operator offered for a field, with the shape of value it expects.</summary>
/// <param name="Operator">The operator itself.</param>
/// <param name="Arity">How many operands it needs.</param>
public sealed record QueryOperatorChoice(QueryOperator Operator, QueryValueArity Arity);

/// <summary>
/// One entity that can be brought into the query next, through a relation the schema declares.
/// </summary>
/// <param name="Relation">Key of the relation to traverse.</param>
/// <param name="Label">Human-readable caption for the relation.</param>
/// <param name="FromAlias">The participant already in the query that this attaches to.</param>
/// <param name="Entity">Key of the entity being brought in.</param>
/// <param name="EntityLabel">Human-readable caption for that entity.</param>
public sealed record QueryJoinChoice(
    string Relation, string Label, string FromAlias, string Entity, string EntityLabel)
{
    /// <summary>Row multiplicity across the relation, which tells a caller whether rows will multiply.</summary>
    public QueryCardinality Cardinality { get; init; }

    /// <summary>
    /// An alias not yet taken. Every choice in one list is offered the same suggestion, since only
    /// one of them is about to be applied; ask again once it has been.
    /// </summary>
    public string SuggestedAlias { get; init; } = string.Empty;

    /// <summary>The join kinds the target supports, so an outer join is never offered where it cannot run.</summary>
    public IReadOnlyList<QueryJoinKind> Kinds { get; init; } = [];
}

/// <summary>
/// Something the query can be ordered by, or that a grouping filter can test: either a field, or the
/// output name of something already selected.
/// </summary>
/// <param name="Label">Human-readable caption for a picker.</param>
public sealed record QuerySortChoice(string Label)
{
    /// <summary>The field, when the target is one. Mutually exclusive with <see cref="Select"/>.</summary>
    public QueryFieldRef? Field { get; init; }

    /// <summary>The output name, when the target is a selected item. Mutually exclusive with <see cref="Field"/>.</summary>
    public string? Select { get; init; }
}
