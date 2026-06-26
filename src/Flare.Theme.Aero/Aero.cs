using Flare.Abstractions.Tokens;

namespace Flare.Theme.Aero;

/// <summary>
/// Public reference tokens for the Aero theme, for deriving custom themes/palettes by overriding
/// only what you need:
/// <code>
/// Light = Aero.LightColors with { Primary = "#6B7A34" };
/// Design = Aero.DesignReference with { Shape = Aero.DesignReference.Shape with { Small = "0px" } };
/// </code>
/// </summary>
public static class Aero
{
    /// <summary>The Aero design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => AeroTokens.Design;
    /// <summary>The Aero baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => AeroTokens.LightColors;
    /// <summary>The Aero baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => AeroTokens.DarkColors;
}
