using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>
/// Public reference tokens for Material Design 3 Expressive, for deriving custom themes/palettes
/// by overriding only what you need:
/// <code>
/// Design = Md3.DesignReference with { Shape = Md3.DesignReference.Shape with { Medium = "10px" } };
/// Light  = Md3.LightColors with { Primary = "#0B57D0" };
/// </code>
/// </summary>
public static class Md3
{
    /// <summary>The MD3 design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => MaterialDesignTokens.Design;
    /// <summary>The MD3 baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => MaterialDesignTokens.LightColors;
    /// <summary>The MD3 baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => MaterialDesignTokens.DarkColors;
}
