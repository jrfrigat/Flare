using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for the mobile bottom-navigation bar (<c>FlareBottomNav</c> / <c>FlareBottomNavItem</c>).
/// Defaults follow Material Design 3 (surface-container bar with a secondary-container active pill);
/// other themes may override individual fields (e.g. a flatter bar or an accent-bar active state).
/// </summary>
public sealed record BottomNavTokens
{
    /// <summary>Fixed height of the bar (excluding safe-area padding). MD3 = 80dp.</summary>
    [CssVar(BottomNavField.BarHeight)] public string BarHeight { get; init; } = "5rem";

    /// <summary>Bar background. MD3 = surface-container.</summary>
    [CssVar(BottomNavField.BarBg)] public string BarBg { get; init; } = Vars.Var(Color.SurfaceContainer);

    /// <summary>Top border color separating the bar from page content. MD3 = surface-variant.</summary>
    [CssVar(BottomNavField.BorderColor)] public string BorderColor { get; init; } = Vars.Var(Color.SurfaceVariant);

    /// <summary>
    /// Extra bottom padding reserved for the device safe area (iOS home indicator, gesture bar),
    /// stacked on top of <see cref="BarHeight"/> via <c>env(safe-area-inset-bottom)</c>.
    /// </summary>
    [CssVar(BottomNavField.SafeAreaPadding)] public string SafeAreaPadding { get; init; } = "env(safe-area-inset-bottom, 0px)";

    /// <summary>Icon + label color for an inactive item. MD3 = on-surface-variant.</summary>
    [CssVar(BottomNavField.InactiveColor)] public string InactiveColor { get; init; } = Vars.Var(Color.OnSurfaceVariant);

    /// <summary>Icon + label color for the active item. MD3 = on-secondary-container (sits atop the pill).</summary>
    [CssVar(BottomNavField.ActiveColor)] public string ActiveColor { get; init; } = Vars.Var(Color.OnSecondaryContainer);

    /// <summary>Icon glyph size.</summary>
    [CssVar(BottomNavField.IconSize)] public string IconSize { get; init; } = "1.5rem";

    /// <summary>Label font size. MD3 = label-medium.</summary>
    [CssVar(BottomNavField.LabelFontSize)] public string LabelFontSize { get; init; } = Vars.Var(Typography.Size("label-medium"));

    /// <summary>Inactive label font weight.</summary>
    [CssVar(BottomNavField.LabelFontWeight)] public string LabelFontWeight { get; init; } = "500";

    /// <summary>Active label font weight (slightly heavier to reinforce the selected state).</summary>
    [CssVar(BottomNavField.LabelFontWeightActive)] public string LabelFontWeightActive { get; init; } = "700";

    /// <summary>Background of the pill drawn behind the active item's icon. MD3 = secondary-container.</summary>
    [CssVar(BottomNavField.IndicatorBg)] public string IndicatorBg { get; init; } = Vars.Var(Color.SecondaryContainer);

    /// <summary>Corner radius of the active-item indicator pill. MD3 = full.</summary>
    [CssVar(BottomNavField.IndicatorRadius)] public string IndicatorRadius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Fixed height of the active-item indicator pill.</summary>
    [CssVar(BottomNavField.IndicatorSize)] public string IndicatorSize { get; init; } = "2rem";
}
