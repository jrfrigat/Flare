using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for layout - component-specific geometry read by layout.css.</summary>
public sealed record LayoutTokens
{
    /// <summary>App Bar Height.</summary>
    [CssVar(LayoutField.AppBarHeight)] public required string AppBarHeight { get; init; }

    /// <summary>Content Padding.</summary>
    [CssVar(LayoutField.ContentPadding)] public required string ContentPadding { get; init; }

    /// <summary>Content Padding Mobile.</summary>
    [CssVar(LayoutField.ContentPaddingMobile)] public required string ContentPaddingMobile { get; init; }

    /// <summary>Drawer Rail Width.</summary>
    [CssVar(LayoutField.DrawerRailWidth)] public required string DrawerRailWidth { get; init; }

    /// <summary>Drawer Width.</summary>
    [CssVar(LayoutField.DrawerWidth)] public required string DrawerWidth { get; init; }
}
