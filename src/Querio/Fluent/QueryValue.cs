using System.Collections.Generic;
using System.Globalization;

namespace Querio;

/// <summary>
/// Turns .NET values into the invariant-culture strings the query model carries.
/// <para>
/// Every value in a query travels as text in a fixed, culture-independent form, and the renderer
/// parses it back using the field's declared <see cref="QueryFieldType"/>. That indirection is what
/// lets one saved query run against stores with different type systems, and it means a query built
/// in one locale means exactly the same thing when opened in another. Use this helper on both sides
/// so the two agree on the format.
/// </para>
/// </summary>
public static class QueryValue
{
    /// <summary>
    /// Formats a value the way the query model stores it: ISO 8601 for timestamps, lowercase
    /// <c>true</c> and <c>false</c> for booleans, invariant digits for numbers, the name for an enum
    /// member. Null stays null.
    /// </summary>
    /// <param name="value">The value to format.</param>
    public static string? ToInvariant(object? value) => value switch
    {
        null => null,
        string text => text,
        bool flag => flag ? "true" : "false",
        DateTime moment => moment.ToString("o", CultureInfo.InvariantCulture),
        DateTimeOffset moment => moment.ToString("o", CultureInfo.InvariantCulture),
        Guid id => id.ToString("D", CultureInfo.InvariantCulture),
        // Enum is checked before IFormattable so a member is stored by name rather than by number.
        Enum member => member.ToString(),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// Formats a set of values for the set operators, dropping any that format to null.
    /// </summary>
    /// <typeparam name="T">Element type of the source sequence.</typeparam>
    /// <param name="values">The values to format.</param>
    public static IReadOnlyList<string> ToInvariantList<T>(IEnumerable<T> values)
    {
        if (values is null) return [];
        var result = new List<string>();
        foreach (var value in values)
        {
            var formatted = ToInvariant(value);
            if (formatted is not null) result.Add(formatted);
        }
        return result;
    }
}
