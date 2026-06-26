namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for tabs.</summary>
public static class Tabs
{
    private const string FlareTabs = $"{Vars.Flare}-tabs";

    /// <summary>CSS custom-property name for the indicator thickness token.</summary>
    public const string IndicatorThickness = $"{FlareTabs}-indicator-thickness";
    /// <summary>CSS custom-property name for the active color token.</summary>
    public const string ActiveColor = $"{FlareTabs}-active-color";
    /// <summary>CSS custom-property name for the inactive color token.</summary>
    public const string InactiveColor = $"{FlareTabs}-inactive-color";
    /// <summary>CSS custom-property name for the divider color token.</summary>
    public const string DividerColor = $"{FlareTabs}-divider-color";

    // Variant surfaces (Tonal / Filled / Outlined), per FlareTabs.Variant.
    /// <summary>CSS custom-property name for the selected bg token.</summary>
    public const string SelectedBg = $"{FlareTabs}-selected-bg";
    /// <summary>CSS custom-property name for the selected fg token.</summary>
    public const string SelectedFg = $"{FlareTabs}-selected-fg";
    /// <summary>CSS custom-property name for the filled bg token.</summary>
    public const string FilledBg = $"{FlareTabs}-filled-bg";
    /// <summary>CSS custom-property name for the filled fg token.</summary>
    public const string FilledFg = $"{FlareTabs}-filled-fg";
    /// <summary>CSS custom-property name for the track bg token.</summary>
    public const string TrackBg = $"{FlareTabs}-track-bg";
    /// <summary>CSS custom-property name for the pill radius token.</summary>
    public const string PillRadius = $"{FlareTabs}-pill-radius";
}
