using Flare.Core.Tokens;

namespace Flare.Theme.FluentUI2;

/// <summary>
/// Public reference tokens for Fluent UI 2, for deriving custom themes/palettes by overriding
/// only what you need:
/// <code>
/// Design = Fluent2.DesignReference with { Typography = Fluent2.DesignReference.Typography with { ... } };
/// Light  = Fluent2.LightColors with { Primary = "#2B579A" };
/// </code>
/// </summary>
public static class Fluent2
{
    /// <summary>The Fluent design tokens (non-color); override via <c>with</c>.</summary>
    public static DesignTokens DesignReference => FluentUI2Tokens.Design;
    /// <summary>The Fluent baseline light color scheme; override via <c>with</c>.</summary>
    public static ColorScheme LightColors => FluentUI2Tokens.LightColors;
    /// <summary>The Fluent baseline dark color scheme; override via <c>with</c>.</summary>
    public static ColorScheme DarkColors => FluentUI2Tokens.DarkColors;
    /// <summary>Fluent's dark-mode Extended overrides (focus stroke, switch hover).</summary>
    public static IReadOnlyDictionary<string, string> DarkExtended => FluentUI2Tokens.DarkExtended;
}
