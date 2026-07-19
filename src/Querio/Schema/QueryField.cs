namespace Querio;

/// <summary>
/// One queryable field on a <see cref="QueryEntity"/>. <see cref="Key"/> is the logical name a query
/// refers to; <see cref="Column"/> optionally carries the physical name, so the same query renders
/// against differently-named storage. When <see cref="Operators"/> or <see cref="Aggregates"/> are
/// left null the defaults for <see cref="Type"/> apply - list them explicitly only to narrow what a
/// user may do, for instance to hide percentiles on a backend that cannot compute them.
/// </summary>
/// <param name="Key">Logical field name, unique within its entity. Matched case-insensitively.</param>
/// <param name="Label">Human-readable caption shown in a field picker.</param>
/// <param name="Type">Semantic kind, which drives the default operator and aggregate sets.</param>
public sealed record QueryField(string Key, string Label, QueryFieldType Type)
{
    /// <summary>Physical column name when it differs from <see cref="Key"/>. Null means they match.</summary>
    public string? Column { get; init; }

    /// <summary>Whether the field can hold no value, so null checks are worth offering.</summary>
    public bool Nullable { get; init; }

    /// <summary>Whether the field may appear in a GROUP BY. Defaults to true.</summary>
    public bool Groupable { get; init; } = true;

    /// <summary>Whether the field may appear in a condition. Defaults to true.</summary>
    public bool Filterable { get; init; } = true;

    /// <summary>Operators the consumer permits. Null falls back to the defaults for <see cref="Type"/>.</summary>
    public IReadOnlyList<QueryOperator>? Operators { get; init; }

    /// <summary>Aggregates the consumer permits. Null falls back to the defaults for <see cref="Type"/>.</summary>
    public IReadOnlyList<QueryAggregate>? Aggregates { get; init; }

    /// <summary>Members of an <see cref="QueryFieldType.Enum"/> field. Ignored for every other type.</summary>
    public IReadOnlyList<QueryEnumMember>? EnumMembers { get; init; }

    /// <summary>The operators actually allowed here: the explicit list, or the defaults for the type.</summary>
    public IReadOnlyList<QueryOperator> AllowedOperators => Operators ?? QueryDefaults.OperatorsFor(Type);

    /// <summary>The aggregates actually allowed here: the explicit list, or the defaults for the type.</summary>
    public IReadOnlyList<QueryAggregate> AllowedAggregates => Aggregates ?? QueryDefaults.AggregatesFor(Type);

    /// <summary>The physical name to render: <see cref="Column"/> when set, otherwise <see cref="Key"/>.</summary>
    public string PhysicalName => string.IsNullOrEmpty(Column) ? Key : Column!;
}
