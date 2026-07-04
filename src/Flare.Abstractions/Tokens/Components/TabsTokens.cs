using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareTabs</c> / <c>FlareTab</c>.</summary>
public sealed record TabsTokens
{
    /// <summary>Active-indicator thickness. MD3 = 3dp.</summary>
    [CssVar(Tabs.IndicatorThickness)] public required string IndicatorThickness { get; init; }

    /// <summary>Active-indicator + active tab color. MD3 = primary.</summary>
    [CssVar(Tabs.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Inactive tab text color. MD3 = on-surface.</summary>
    [CssVar(Tabs.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>Bar divider color. MD3 = surface-variant.</summary>
    [CssVar(Tabs.DividerColor)] public required string DividerColor { get; init; }

    /// <summary>Selected-tab background for the <c>Tonal</c> variant. MD3 = secondary-container.</summary>
    [CssVar(Tabs.SelectedBg)] public required string SelectedBg { get; init; }

    /// <summary>Selected-tab text/icon color for the <c>Tonal</c> variant. MD3 = on-secondary-container.</summary>
    [CssVar(Tabs.SelectedFg)] public required string SelectedFg { get; init; }

    /// <summary>Selected-tab background for the <c>Filled</c> variant. MD3 = primary.</summary>
    [CssVar(Tabs.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Selected-tab text/icon color for the <c>Filled</c> variant. MD3 = on-primary.</summary>
    [CssVar(Tabs.FilledFg)] public required string FilledFg { get; init; }

    /// <summary>Track background behind the bar for <c>Tonal</c>/<c>Filled</c> (segmented look). Default none.</summary>
    [CssVar(Tabs.TrackBg)] public required string TrackBg { get; init; }

    /// <summary>Corner radius of the selected pill for <c>Tonal</c>/<c>Filled</c>. MD3 = full.</summary>
    [CssVar(Tabs.PillRadius)] public required string PillRadius { get; init; }
}
