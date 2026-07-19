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
    /// Reads a stored value back into a .NET value, using the field's declared type to decide how.
    /// The exact inverse of <see cref="ToInvariant"/>: a renderer calls this to build the parameters
    /// it hands to its driver, which is why a value never has to be spliced into query text.
    /// </summary>
    /// <param name="value">The stored, invariant-culture value. Null stays null.</param>
    /// <param name="type">The field's declared type, which decides how the text is read.</param>
    /// <exception cref="FormatException">The value does not read as the declared type.</exception>
    public static object? Parse(string? value, QueryFieldType type)
    {
        if (TryParse(value, type, out var result)) return result;
        throw new FormatException($"'{value}' does not read as {type}.");
    }

    /// <summary>Reads a stored value back into a .NET value, reporting failure rather than throwing.</summary>
    /// <param name="value">The stored, invariant-culture value. Null stays null and succeeds.</param>
    /// <param name="type">The field's declared type, which decides how the text is read.</param>
    /// <param name="result">The parsed value on success, otherwise null.</param>
    public static bool TryParse(string? value, QueryFieldType type, out object? result)
    {
        result = null;
        if (value is null) return true;

        switch (type)
        {
            // An enum member travels by name, and the CLR type it belongs to is unknown here, so the
            // name is handed on as-is for the driver or the store to match.
            case QueryFieldType.Text:
            case QueryFieldType.Enum:
                result = value;
                return true;

            case QueryFieldType.Boolean:
                if (!bool.TryParse(value, out var flag)) return false;
                result = flag;
                return true;

            case QueryFieldType.Number:
                // Narrowest first, so whole numbers stay whole and exact values stay exact.
                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var whole))
                {
                    result = whole;
                    return true;
                }
                if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var exact))
                {
                    result = exact;
                    return true;
                }
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var approximate))
                {
                    result = approximate;
                    return true;
                }
                return false;

            case QueryFieldType.DateTime:
                if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var moment))
                {
                    return false;
                }
                result = moment;
                return true;

            case QueryFieldType.Guid:
                if (!Guid.TryParse(value, out var id)) return false;
                result = id;
                return true;

            default:
                result = value;
                return true;
        }
    }

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
