using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareTabs</c> / <c>FlareTab</c>.</summary>
public sealed record TabsTokens
{
    /// <summary>Active weight.</summary>
    [CssVar(Tabs.ActiveWeight)] public required string ActiveWeight { get; init; }
    /// <summary>Close opacity.</summary>
    [CssVar(Tabs.CloseOpacity)] public required string CloseOpacity { get; init; }
    /// <summary>Label font.</summary>
    [CssVar(Tabs.LabelFont)] public required string LabelFont { get; init; }
    /// <summary>Label size.</summary>
    [CssVar(Tabs.LabelSize)] public required string LabelSize { get; init; }
    /// <summary>Label weight.</summary>
    [CssVar(Tabs.LabelWeight)] public required string LabelWeight { get; init; }
    /// <summary>Scroll shadow opacity.</summary>
    [CssVar(Tabs.ScrollShadowOpacity)] public required string ScrollShadowOpacity { get; init; }
    /// <summary>Active-indicator thickness.</summary>
    [CssVar(Tabs.IndicatorThickness)] public required string IndicatorThickness { get; init; }

    /// <summary>Active-indicator and active-tab color.</summary>
    [CssVar(Tabs.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Inactive tab text color.</summary>
    [CssVar(Tabs.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>Bar divider color.</summary>
    [CssVar(Tabs.DividerColor)] public required string DividerColor { get; init; }

    /// <summary>Selected-tab background for the <c>Tonal</c> variant.</summary>
    [CssVar(Tabs.SelectedBg)] public required string SelectedBg { get; init; }

    /// <summary>Selected-tab text/icon color for the <c>Tonal</c> variant.</summary>
    [CssVar(Tabs.SelectedFg)] public required string SelectedFg { get; init; }

    /// <summary>Selected-tab background for the <c>Filled</c> variant.</summary>
    [CssVar(Tabs.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Selected-tab text/icon color for the <c>Filled</c> variant.</summary>
    [CssVar(Tabs.FilledFg)] public required string FilledFg { get; init; }

    /// <summary>Track background behind the bar for <c>Tonal</c>/<c>Filled</c> (segmented look). Default none.</summary>
    [CssVar(Tabs.TrackBg)] public required string TrackBg { get; init; }

    /// <summary>Corner radius of the selected pill for <c>Tonal</c>/<c>Filled</c>.</summary>
    [CssVar(Tabs.PillRadius)] public required string PillRadius { get; init; }
}
