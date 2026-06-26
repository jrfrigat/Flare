using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareTabs</c> / <c>FlareTab</c>.</summary>
public sealed record TabsTokens
{
    /// <summary>Active-indicator thickness. MD3 = 3dp.</summary>
    [CssVar(Tabs.IndicatorThickness)] public string IndicatorThickness { get; init; } = "3px";

    /// <summary>Active-indicator + active tab color. MD3 = primary.</summary>
    [CssVar(Tabs.ActiveColor)] public string ActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Inactive tab text color. MD3 = on-surface.</summary>
    [CssVar(Tabs.InactiveColor)] public string InactiveColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Bar divider color. MD3 = surface-variant.</summary>
    [CssVar(Tabs.DividerColor)] public string DividerColor { get; init; } = "var(--flare-color-surface-variant)";

    /// <summary>Selected-tab background for the <c>Tonal</c> variant. MD3 = secondary-container.</summary>
    [CssVar(Tabs.SelectedBg)] public string SelectedBg { get; init; } = "var(--flare-color-secondary-container)";

    /// <summary>Selected-tab text/icon color for the <c>Tonal</c> variant. MD3 = on-secondary-container.</summary>
    [CssVar(Tabs.SelectedFg)] public string SelectedFg { get; init; } = "var(--flare-color-on-secondary-container)";

    /// <summary>Selected-tab background for the <c>Filled</c> variant. MD3 = primary.</summary>
    [CssVar(Tabs.FilledBg)] public string FilledBg { get; init; } = "var(--flare-color-primary)";

    /// <summary>Selected-tab text/icon color for the <c>Filled</c> variant. MD3 = on-primary.</summary>
    [CssVar(Tabs.FilledFg)] public string FilledFg { get; init; } = "var(--flare-color-on-primary)";

    /// <summary>Track background behind the bar for <c>Tonal</c>/<c>Filled</c> (segmented look). Default none.</summary>
    [CssVar(Tabs.TrackBg)] public string TrackBg { get; init; } = "transparent";

    /// <summary>Corner radius of the selected pill for <c>Tonal</c>/<c>Filled</c>. MD3 = full.</summary>
    [CssVar(Tabs.PillRadius)] public string PillRadius { get; init; } = "var(--flare-shape-full)";
}
