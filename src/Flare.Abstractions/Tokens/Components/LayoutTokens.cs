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

    /// <summary>Shadow under the app bar, for a language that lifts the bar off the page instead of drawing a border or a tone step.</summary>
    [CssVar(LayoutField.AppBarShadow)] public required string AppBarShadow { get; init; }

    /// <summary>The shell's own plane, behind every other one.</summary>
    [CssVar(LayoutField.ShellBg)] public required string ShellBg { get; init; }

    /// <summary>
    /// The drawer's plane. Core used to pick this role itself, which left the shell's three planes -
    /// page, chrome and canvas - decided in two different places: the app bar had a token and the
    /// drawer and the content did not, so a language wanting its chrome on the darkest plane and its
    /// canvas on the surface had to overwrite core rules by class name.
    /// </summary>
    [CssVar(LayoutField.DrawerBg)] public required string DrawerBg { get; init; }

    /// <summary>The collapsed rail's plane, named apart from the drawer so a language can set an
    /// icon rail against the panel it expands into. Point it at <see cref="DrawerBg"/> to keep them the
    /// same, which is what the in-box themes do.</summary>
    [CssVar(LayoutField.RailBg)] public required string RailBg { get; init; }

    /// <summary>The content plane. It had no background at all, so the canvas was whatever the shell
    /// happened to be painted.</summary>
    [CssVar(LayoutField.ContentBg)] public required string ContentBg { get; init; }

    /// <summary>
    /// Inline offset of the shadow the drawer casts on the content, for a language that separates the
    /// panel from the canvas by depth rather than by a line or a tone step. <c>0</c> - the in-box value -
    /// draws none.
    /// <para>
    /// Three parts rather than one <c>box-shadow</c> shorthand, unlike <see cref="AppBarShadow"/>,
    /// because this shadow has a side and the bar's does not: the bar always casts downward, while the
    /// drawer casts toward the content, which is the opposite direction for an end-anchored drawer and
    /// flips again under RTL. Core owns that sign so a theme never has to know which edge it is on.
    /// </para>
    /// </summary>
    [CssVar(LayoutField.DrawerShadowOffset)] public required string DrawerShadowOffset { get; init; }

    /// <summary>Blur of the drawer's edge shadow. See <see cref="DrawerShadowOffset"/>.</summary>
    [CssVar(LayoutField.DrawerShadowBlur)] public required string DrawerShadowBlur { get; init; }

    /// <summary>Colour of the drawer's edge shadow. See <see cref="DrawerShadowOffset"/>.</summary>
    [CssVar(LayoutField.DrawerShadowColor)] public required string DrawerShadowColor { get; init; }

    /// <summary>Side of the round button that toggles the primary drawer from the app bar. On a coarse
    /// pointer it still grows to the touch-target minimum.</summary>
    [CssVar(LayoutField.AppBarToggleSize)] public required string AppBarToggleSize { get; init; }

    /// <summary>Width of the three bars the drawer-toggle glyph is drawn from.</summary>
    [CssVar(LayoutField.AppBarToggleGlyphWidth)] public required string AppBarToggleGlyphWidth { get; init; }

    /// <summary>
    /// Height of the drawer-toggle glyph, from the top of its first bar to the bottom of its last. The
    /// middle bar is centred in it, and the open glyph crosses the outer bars at that centre.
    /// </summary>
    [CssVar(LayoutField.AppBarToggleGlyphHeight)] public required string AppBarToggleGlyphHeight { get; init; }

    /// <summary>Thickness of each drawer-toggle glyph bar. Its ends are rounded by half of it.</summary>
    [CssVar(LayoutField.AppBarToggleBarThickness)] public required string AppBarToggleBarThickness { get; init; }

    /// <summary>Corner radius of the drawer-toggle button - a circle for a language whose icon buttons are
    /// round, a small radius for one whose icon buttons are squares. Usually a reference to a shape step.</summary>
    [CssVar(LayoutField.AppBarToggleRadius)] public required string AppBarToggleRadius { get; init; }
}
