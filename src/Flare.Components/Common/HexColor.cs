namespace Flare.Components;

/// <summary>
/// Validation for a hex color written as <c>#rgb</c>, <c>#rrggbb</c> or <c>#rrggbbaa</c>. A style
/// attribute is built from the value, so this is the OWASP guard against CSS injection; it is a
/// character check rather than a regular expression so the whole regex engine stays out of the
/// startup graph of an application that only shows a colour swatch.
/// </summary>
internal static class HexColor
{
    /// <summary>True when the value is one of the three accepted hex forms.</summary>
    public static bool IsValid(string? value)
    {
        if (value is null || value.Length is not (4 or 7 or 9) || value[0] != '#') return false;
        foreach (var c in value.AsSpan(1))
            if (!char.IsAsciiHexDigit(c)) return false;
        return true;
    }
}
