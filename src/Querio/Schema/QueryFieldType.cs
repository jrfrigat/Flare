namespace Querio;

/// <summary>
/// The semantic kind of a field. It decides which operators and aggregates apply by default, and it
/// tells a renderer how to interpret the invariant-culture string a condition carries as its value.
/// Deliberately semantic rather than physical: a backend maps these onto its own column types.
/// </summary>
public enum QueryFieldType
{
    /// <summary>Textual value. Adds substring operators (contains, starts with, ends with).</summary>
    Text,

    /// <summary>Numeric value. Supports ordering comparisons and the arithmetic aggregates.</summary>
    Number,

    /// <summary>Boolean flag. Only equality and the null checks are meaningful.</summary>
    Boolean,

    /// <summary>A point in time. Supports ordering, ranges, relative offsets and date truncation.</summary>
    DateTime,

    /// <summary>An identifier. Equality and set membership only; never ordered or summed.</summary>
    Guid,

    /// <summary>A closed set of named values, enumerated by <see cref="QueryField.EnumMembers"/>.</summary>
    Enum,
}

/// <summary>
/// One member of an <see cref="QueryFieldType.Enum"/> field: the value as it is stored (and as it
/// travels in a condition) paired with the label a person sees when picking it.
/// </summary>
/// <param name="Value">The stored value, in invariant-culture string form.</param>
/// <param name="Label">The human-readable label for pickers and generated captions.</param>
public sealed record QueryEnumMember(string Value, string Label);
