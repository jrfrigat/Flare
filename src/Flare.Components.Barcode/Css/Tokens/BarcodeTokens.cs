namespace Flare.Css.Tokens.Barcode;

/// <summary>
/// CSS custom-property names for the barcode surface. These are override points for a caller, not theme
/// tokens: a barcode has to scan, so its bars stay at maximum contrast on an explicitly white ground
/// whatever palette the page uses. The values live in <c>barcode.css</c>.
/// </summary>
public static class Surface
{
    /// <summary>Colour of the bars and the human-readable line.</summary>
    public const string Bar = "--flare-barcode-bar";

    /// <summary>Background behind the bars.</summary>
    public const string Ground = "--flare-barcode-ground";
}
