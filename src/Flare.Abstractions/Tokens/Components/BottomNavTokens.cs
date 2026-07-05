using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for the mobile bottom-navigation bar (<c>FlareBottomNav</c> / <c>FlareBottomNavItem</c>).
/// A theme styles the bar, its active-indicator pill and the item colors entirely through these tokens
/// (e.g. a flatter bar or an accent-bar active state).
/// </summary>
public sealed record BottomNavTokens
{
    /// <summary>Fixed height of the bar (excluding safe-area padding).</summary>
    [CssVar(BottomNavField.BarHeight)] public required string BarHeight { get; init; }

    /// <summary>Bar background.</summary>
    [CssVar(BottomNavField.BarBg)] public required string BarBg { get; init; }

    /// <summary>Top border color separating the bar from page content.</summary>
    [CssVar(BottomNavField.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>
    /// Extra bottom padding reserved for the device safe area (iOS home indicator, gesture bar),
    /// stacked on top of <see cref="BarHeight"/> via <c>env(safe-area-inset-bottom)</c>.
    /// </summary>
    [CssVar(BottomNavField.SafeAreaPadding)] public required string SafeAreaPadding { get; init; }

    /// <summary>Icon and label color for an inactive item.</summary>
    [CssVar(BottomNavField.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>Icon and label color for the active item (drawn atop the active-indicator pill).</summary>
    [CssVar(BottomNavField.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Icon glyph size.</summary>
    [CssVar(BottomNavField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Label font size.</summary>
    [CssVar(BottomNavField.LabelFontSize)] public required string LabelFontSize { get; init; }

    /// <summary>Inactive label font weight.</summary>
    [CssVar(BottomNavField.LabelFontWeight)] public required string LabelFontWeight { get; init; }

    /// <summary>Active label font weight (slightly heavier to reinforce the selected state).</summary>
    [CssVar(BottomNavField.LabelFontWeightActive)] public required string LabelFontWeightActive { get; init; }

    /// <summary>Background of the pill drawn behind the active item's icon. Defaults to the shared nav
    /// active-indicator token so a theme that flattens the drawer indicator (by setting it to
    /// <c>none</c>) flattens the bottom bar too; override this token to restyle only the bottom bar.</summary>
    [CssVar(BottomNavField.IndicatorBg)] public required string IndicatorBg { get; init; }

    /// <summary>Corner radius of the active-item indicator pill. Defaults to the shared nav
    /// indicator-radius token; override to restyle only the bottom bar.</summary>
    [CssVar(BottomNavField.IndicatorRadius)] public required string IndicatorRadius { get; init; }

    /// <summary>Fixed height of the active-item indicator pill.</summary>
    [CssVar(BottomNavField.IndicatorSize)] public required string IndicatorSize { get; init; }
}
