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
    [CssVar(Tabs.LabelFont)] public string LabelFont { get; init; } = "var(--flare-typescale-label-large-font)";
    /// <summary>Label size.</summary>
    [CssVar(Tabs.LabelSize)] public string LabelSize { get; init; } = "var(--flare-typescale-label-large-size)";

    /// <summary>Letter-spacing of the tab label; a language that sets tab labels in its button type carries that tracking here.</summary>
    [CssVar(Tabs.LabelSpacing)] public string LabelSpacing { get; init; } = "var(--flare-typescale-label-large-spacing)";
    /// <summary>Label weight.</summary>
    [CssVar(Tabs.LabelWeight)] public string LabelWeight { get; init; } = "var(--flare-typescale-label-large-weight)";
    /// <summary>Scroll shadow opacity.</summary>
    [CssVar(Tabs.ScrollShadowOpacity)] public required string ScrollShadowOpacity { get; init; }
    /// <summary>Active-indicator thickness.</summary>
    [CssVar(Tabs.IndicatorThickness)] public required string IndicatorThickness { get; init; }

    /// <summary>Active-indicator and active-tab color.</summary>
    [CssVar(Tabs.ActiveColor)] public string ActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Active-indicator thickness for the <c>Secondary</c> variant. Separate from
    /// <see cref="IndicatorThickness"/> because the two tab levels are meant to be told apart at a
    /// glance, and thickness is half of how a language does that.</summary>
    [CssVar(Tabs.SecondaryIndicatorThickness)] public required string SecondaryIndicatorThickness { get; init; }

    /// <summary>Active label and icon color for the <c>Secondary</c> variant - the other half. A
    /// secondary tab sits beneath a primary one, so its active state is usually the content colour
    /// rather than the accent, leaving the indicator alone to carry the accent.</summary>
    [CssVar(Tabs.SecondaryActiveColor)] public string SecondaryActiveColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Inactive tab text color.</summary>
    [CssVar(Tabs.InactiveColor)] public string InactiveColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Bar divider color.</summary>
    [CssVar(Tabs.DividerColor)] public string DividerColor { get; init; } = "var(--flare-color-surface-variant)";

    /// <summary>Selected-tab background for the <c>Tonal</c> variant.</summary>
    [CssVar(Tabs.SelectedBg)] public string SelectedBg { get; init; } = "var(--flare-color-secondary-container)";

    /// <summary>Selected-tab text/icon color for the <c>Tonal</c> variant.</summary>
    [CssVar(Tabs.SelectedFg)] public string SelectedFg { get; init; } = "var(--flare-color-on-secondary-container)";

    /// <summary>Selected-tab background for the <c>Filled</c> variant.</summary>
    [CssVar(Tabs.FilledBg)] public string FilledBg { get; init; } = "var(--flare-color-primary)";

    /// <summary>Selected-tab text/icon color for the <c>Filled</c> variant.</summary>
    [CssVar(Tabs.FilledFg)] public string FilledFg { get; init; } = "var(--flare-color-on-primary)";

    /// <summary>Track background behind the bar for <c>Tonal</c>/<c>Filled</c> (segmented look). Default none.</summary>
    [CssVar(Tabs.TrackBg)] public required string TrackBg { get; init; }

    /// <summary>Corner radius of the selected pill for <c>Tonal</c>/<c>Filled</c>.</summary>
    [CssVar(Tabs.PillRadius)] public string PillRadius { get; init; } = "var(--flare-shape-full)";

    /// <summary>How far a disabled tab fades. A language that repaints disabled controls in a flat
    /// palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(Tabs.TabDisabledOpacity)] public string TabDisabledOpacity { get; init; } = "var(--flare-state-disabled-opacity)";

    /// <summary>How far a disabled scroll button fades - the arrow shown when the strip overflows and
    /// has reached one end. Separate from the tab, because a language may mute a spent affordance more
    /// heavily than an unavailable destination.</summary>
    [CssVar(Tabs.ScrollDisabledOpacity)] public string ScrollDisabledOpacity { get; init; } = "var(--flare-state-disabled-opacity)";

    /// <summary>How tall a tab is. A language that sets the strip's height rather than letting the
    /// label and its padding decide it names the number here.</summary>
    [CssVar(Tabs.TabHeight)] public required string TabHeight { get; init; }

    /// <summary>The narrowest a tab gets, so a one-word destination does not shrink to its label and
    /// leave the strip looking ragged.</summary>
    [CssVar(Tabs.TabMinWidth)] public required string TabMinWidth { get; init; }

    /// <summary>Padding either side of a tab's content.</summary>
    [CssVar(Tabs.TabPaddingInline)] public string TabPaddingInline { get; init; } = "var(--flare-spacing-8)";

    /// <summary>Glyph size of a tab's leading icon. The icon is the caller's own fragment, so the size
    /// reaches it through the shared icon property rather than a class on the icon.</summary>
    [CssVar(Tabs.IconSize)] public required string IconSize { get; init; }
}
