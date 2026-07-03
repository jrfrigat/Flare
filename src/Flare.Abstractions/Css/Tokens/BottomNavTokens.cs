namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for the mobile bottom-navigation bar.</summary>
public static class BottomNavField
{
    private const string Prefix = $"{Vars.Flare}-bottom-nav";

    /// <summary>CSS custom-property name for the bar height token.</summary>
    public const string BarHeight = $"{Prefix}-bar-height";
    /// <summary>CSS custom-property name for the bar background token.</summary>
    public const string BarBg = $"{Prefix}-bar-bg";
    /// <summary>CSS custom-property name for the bar top-border color token.</summary>
    public const string BorderColor = $"{Prefix}-border-color";
    /// <summary>CSS custom-property name for the safe-area-aware bottom padding token.</summary>
    public const string SafeAreaPadding = $"{Prefix}-safe-area-padding";
    /// <summary>CSS custom-property name for the inactive item color token.</summary>
    public const string InactiveColor = $"{Prefix}-inactive-color";
    /// <summary>CSS custom-property name for the active item color token.</summary>
    public const string ActiveColor = $"{Prefix}-active-color";
    /// <summary>CSS custom-property name for the icon size token.</summary>
    public const string IconSize = $"{Prefix}-icon-size";
    /// <summary>CSS custom-property name for the label font size token.</summary>
    public const string LabelFontSize = $"{Prefix}-label-font-size";
    /// <summary>CSS custom-property name for the label font weight token.</summary>
    public const string LabelFontWeight = $"{Prefix}-label-font-weight";
    /// <summary>CSS custom-property name for the active label font weight token.</summary>
    public const string LabelFontWeightActive = $"{Prefix}-label-font-weight-active";
    /// <summary>CSS custom-property name for the active-item indicator pill background token.</summary>
    public const string IndicatorBg = $"{Prefix}-indicator-bg";
    /// <summary>CSS custom-property name for the active-item indicator pill corner-radius token.</summary>
    public const string IndicatorRadius = $"{Prefix}-indicator-radius";
}
