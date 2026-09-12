using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>
/// Public reference tokens for the Material Design 3 BASELINE, for deriving custom themes and
/// palettes by overriding only what you need. Named after the lineage, not after this assembly:
/// the Expressive theme derives from the same reference and overrides what Expressive changes.
/// <code>
/// Design = Md3.DesignReference with { Shape = Md3.DesignReference.Shape with { Medium = "10px" } };
/// Light  = Md3.LightColors with { Primary = "#0B57D0" };
/// </code>
/// </summary>
public static class Md3
{
    /// <summary>The MD3 design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => MaterialDesign3Tokens.Design;
    /// <summary>The MD3 baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => MaterialDesign3Tokens.LightColors;
    /// <summary>The MD3 baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => MaterialDesign3Tokens.DarkColors;
}
