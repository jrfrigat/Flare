using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for layout - component-specific geometry read by layout.css.</summary>
public sealed record LayoutTokens
{
    /// <summary>Height of the layout's app bar.</summary>
    [CssVar(LayoutField.AppBarHeight)] public required string AppBarHeight { get; init; }

    /// <summary>Height of the app bar in its dense form.</summary>
    [CssVar(LayoutField.AppBarHeightDense)] public required string AppBarHeightDense { get; init; }

    /// <summary>Background of the app bar. Usually a reference to a surface colour role.</summary>
    [CssVar(LayoutField.AppBarBg)] public required string AppBarBg { get; init; }

    /// <summary>Content Padding.</summary>
    [CssVar(LayoutField.ContentPadding)] public required string ContentPadding { get; init; }

    /// <summary>Content Padding Mobile.</summary>
    [CssVar(LayoutField.ContentPaddingMobile)] public required string ContentPaddingMobile { get; init; }

    /// <summary>Drawer Rail Width.</summary>
    [CssVar(LayoutField.DrawerRailWidth)] public required string DrawerRailWidth { get; init; }

    /// <summary>Drawer Width.</summary>
    [CssVar(LayoutField.DrawerWidth)] public required string DrawerWidth { get; init; }

    /// <summary>Bottom edge of the shell app bar, as a <c>border</c> shorthand.</summary>
    [CssVar(LayoutField.AppBarBorder)] public required string AppBarBorder { get; init; }

    /// <summary>Edge the shell drawer draws against the content, as a <c>border</c> shorthand. It lands on
    /// the trailing edge, or on the leading edge of an end-anchored drawer.</summary>
    [CssVar(LayoutField.DrawerBorder)] public required string DrawerBorder { get; init; }
}
