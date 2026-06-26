using Flare.Abstractions.Tokens;

namespace Flare.Theme.LiquidGlass;

/// <summary>
/// Public reference tokens for the Liquid Glass theme, for deriving custom themes/palettes by overriding
/// only what you need:
/// <code>
/// Light = LiquidGlass.LightColors with { Primary = "#AF52DE" };
/// Design = LiquidGlass.DesignReference with { Shape = LiquidGlass.DesignReference.Shape with { Large = "16px" } };
/// </code>
/// </summary>
public static class LiquidGlass
{
    /// <summary>The Liquid Glass design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => LiquidGlassTokens.Design;
    /// <summary>The Liquid Glass baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => LiquidGlassTokens.LightColors;
    /// <summary>The Liquid Glass baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => LiquidGlassTokens.DarkColors;
}
