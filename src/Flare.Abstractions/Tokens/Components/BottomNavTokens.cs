using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
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
    [CssVar(BottomNavField.BarBg)] public string BarBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);

    /// <summary>Top border color separating the bar from page content.</summary>
    [CssVar(BottomNavField.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>
    /// Extra bottom padding reserved for the device safe area (iOS home indicator, gesture bar),
    /// stacked on top of <see cref="BarHeight"/> via <c>env(safe-area-inset-bottom)</c>.
    /// </summary>
    [CssVar(BottomNavField.SafeAreaPadding)] public required string SafeAreaPadding { get; init; }

    /// <summary>Icon and label color for an inactive item.</summary>
    [CssVar(BottomNavField.InactiveColor)] public string InactiveColor { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);

    /// <summary>Icon and label color for the active item (drawn atop the active-indicator pill).</summary>
    [CssVar(BottomNavField.ActiveColor)] public string ActiveColor { get; init; } = Var(Css.Tokens.Color.OnSecondaryContainer);

    /// <summary>Icon glyph size.</summary>
    [CssVar(BottomNavField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Label font size.</summary>
    [CssVar(BottomNavField.LabelFontSize)] public string LabelFontSize { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Size);

    /// <summary>Inactive label font weight.</summary>
    [CssVar(BottomNavField.LabelFontWeight)] public string LabelFontWeight { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Weight);

    /// <summary>Active label font weight (slightly heavier to reinforce the selected state).</summary>
    [CssVar(BottomNavField.LabelFontWeightActive)] public string LabelFontWeightActive { get; init; } = Var(Css.Tokens.Typography.WeightBold);

    /// <summary>Background of the pill drawn behind the active item's icon. Defaults to the shared nav
    /// active-indicator token so a theme that flattens the drawer indicator (by setting it to
    /// <c>none</c>) flattens the bottom bar too; override this token to restyle only the bottom bar.</summary>
    [CssVar(BottomNavField.IndicatorBg)] public string IndicatorBg { get; init; } = Var(Css.Tokens.NavField.ActiveIndicator);

    /// <summary>Corner radius of the active-item indicator pill. Defaults to the shared nav
    /// indicator-radius token; override to restyle only the bottom bar.</summary>
    [CssVar(BottomNavField.IndicatorRadius)] public string IndicatorRadius { get; init; } = Var(Css.Tokens.NavField.IndicatorRadius);

    /// <summary>Fixed height of the active-item indicator pill.</summary>
    [CssVar(BottomNavField.IndicatorSize)] public required string IndicatorSize { get; init; }

    /// <summary>Minimum width of the active-item indicator pill; the icon and the pill's inline padding
    /// widen it past this. <c>0</c> leaves the width to the padding alone.</summary>
    [CssVar(BottomNavField.IndicatorWidth)] public required string IndicatorWidth { get; init; }

    /// <summary>Space between an item's indicator and its label.</summary>
    [CssVar(BottomNavField.ItemGap)] public string ItemGap { get; init; } = Var(Css.Tokens.Spacing.S2);

    /// <summary>How far a disabled item fades. A language that repaints disabled controls in a flat
    /// palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(BottomNavField.ItemDisabledOpacity)] public string ItemDisabledOpacity { get; init; } = Var(Css.Tokens.State.DisabledOpacity);
}
