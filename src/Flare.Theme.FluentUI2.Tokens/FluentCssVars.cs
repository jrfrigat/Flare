namespace Flare.Theme.FluentUI2.Tokens;

/// <summary>
/// Fluent UI 2 theme-private CSS custom-property names. These are the theme's OWN tokens (not part of the
/// Flare core contract), so they live in the theme project - the core stays theme-agnostic. The scoped
/// Fluent CSS reads them and <see cref="FluentUI2Tokens"/> assigns their values per mode.
/// </summary>
public static class FluentCssVars
{
    /// <summary>Thin control stroke width.</summary>
    public const string StrokeWidthThin = "--flare-fluent-stroke-width-thin";
    /// <summary>Thick control stroke width.</summary>
    public const string StrokeWidthThick = "--flare-fluent-stroke-width-thick";
    /// <summary>Focus-stroke width.</summary>
    public const string FocusStrokeWidth = "--flare-fluent-focus-stroke-width";
    /// <summary>Inner focus-stroke color.</summary>
    public const string FocusStrokeColor = "--flare-fluent-focus-stroke-color";
    /// <summary>Outer focus-stroke color.</summary>
    public const string FocusStrokeOuter = "--flare-fluent-focus-stroke-outer";
    /// <summary>Flat disabled background (colorNeutralBackgroundDisabled).</summary>
    public const string DisabledBg = "--flare-fluent-disabled-bg";
    /// <summary>Flat disabled foreground (colorNeutralForegroundDisabled).</summary>
    public const string DisabledFg = "--flare-fluent-disabled-fg";
    /// <summary>Flat disabled stroke (colorNeutralStrokeDisabled).</summary>
    public const string DisabledBorder = "--flare-fluent-disabled-border";
}
