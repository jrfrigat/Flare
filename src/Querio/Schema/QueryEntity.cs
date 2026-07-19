namespace Querio;

/// <summary>
/// One queryable entity - a table, view, document or whatever the backing store calls it.
/// <see cref="Key"/> is the logical name a query refers to; <see cref="Source"/> optionally carries
/// the physical name, which is what lets one logical query render against "dbo.Products" in one
/// store and a differently-named object in another.
/// </summary>
/// <param name="Key">Logical entity name, unique within the schema. Matched case-insensitively.</param>
/// <param name="Label">Human-readable caption shown in an entity picker.</param>
/// <param name="Fields">The queryable fields of this entity.</param>
public sealed record QueryEntity(string Key, string Label, IReadOnlyList<QueryField> Fields)
{
    /// <summary>Physical object name when it differs from <see cref="Key"/>. Null means they match.</summary>
    public string? Source { get; init; }

    /// <summary>Field keys forming the primary key. Empty when the consumer does not declare one.</summary>
    public IReadOnlyList<string> PrimaryKey { get; init; } = [];

    /// <summary>The physical name to render: <see cref="Source"/> when set, otherwise <see cref="Key"/>.</summary>
    public string PhysicalName => string.IsNullOrEmpty(Source) ? Key : Source!;

    /// <summary>Finds a field by key, case-insensitively. Returns null when this entity has no such field.</summary>
    /// <param name="key">The logical field name to look for.</param>
    public QueryField? FindField(string key)
    {
        for (var i = 0; i < Fields.Count; i++)
        {
            if (string.Equals(Fields[i].Key, key, StringComparison.OrdinalIgnoreCase)) return Fields[i];
        }
        return null;
    }
}
