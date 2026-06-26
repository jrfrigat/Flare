using Flare.Abstractions.Tokens;

namespace Flare.Theme.VisualStudio;

/// <summary>
/// Public reference tokens for the Visual Studio 2026 theme, for deriving custom themes/palettes by
/// overriding only what you need:
/// <code>
/// Light = VisualStudio.LightColors with { Primary = "#68217A" };
/// Design = VisualStudio.DesignReference with { Shape = VisualStudio.DesignReference.Shape with { Small = "0px" } };
/// </code>
/// </summary>
public static class VisualStudio
{
    /// <summary>The Visual Studio design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => VisualStudioTokens.Design;
    /// <summary>The Visual Studio baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => VisualStudioTokens.LightColors;
    /// <summary>The Visual Studio baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => VisualStudioTokens.DarkColors;
}
