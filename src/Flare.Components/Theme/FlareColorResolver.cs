using Flare.Components.Security;
using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Helpers for the <b>custom</b> color path of <see cref="FlareColor"/> (the role path uses a shared
/// CSS class instead). Derives the paired values an arbitrary color needs: contrast for the filled
/// text, and a tonal container via <c>color-mix</c>. Inputs are assumed already sanitized
/// (see <see cref="FlareColor"/>).
/// </summary>
internal static class FlareColorResolver
{
    /// <summary>Filled-text color on a custom accent: explicit override, else black/white by luminance.</summary>
    public static string OnColor(string customColor, string? explicitOn = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitOn))
        {
            var s = CssValidator.SanitizeColor(explicitOn);
            if (s is not null) return s;
        }
        return Contrast(customColor);
    }

    /// <summary>Tonal container background derived from a custom accent.</summary>
    public static string Container(string customColor) => $"color-mix(in srgb, {customColor} 16%, transparent)";

    /// <summary>Foreground on the tonal container for a custom accent (the color itself).</summary>
    public static string OnContainer(string customColor) => customColor;

    // For a hex color, choose black/white text by relative luminance; otherwise default to white.
    private static string Contrast(string color)
    {
        if (color.StartsWith('#'))
        {
            var hex = color[1..];
            if (hex.Length == 3)
                hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
            if (hex.Length >= 6
                && int.TryParse(hex.AsSpan(0, 2), NumberStyles.HexNumber, null, out var r)
                && int.TryParse(hex.AsSpan(2, 2), NumberStyles.HexNumber, null, out var g)
                && int.TryParse(hex.AsSpan(4, 2), NumberStyles.HexNumber, null, out var b))
            {
                var luminance = (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0;
                return luminance > 0.55 ? "#000000" : "#ffffff";
            }
        }
        return "#ffffff";
    }
}
