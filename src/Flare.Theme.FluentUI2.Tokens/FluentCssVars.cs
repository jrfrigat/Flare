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
    /// <summary>Subtle hover fill (colorSubtleBackgroundHover).</summary>
    public const string SubtleHover = "--flare-fluent-subtle-hover";
    /// <summary>Subtle pressed fill (colorSubtleBackgroundPressed).</summary>
    public const string SubtlePressed = "--flare-fluent-subtle-pressed";
    /// <summary>Neutral stroke on hover (colorNeutralStroke1Hover).</summary>
    public const string StrokeHover = "--flare-fluent-stroke-hover";
    /// <summary>Neutral stroke on press (colorNeutralStroke1Pressed).</summary>
    public const string StrokePressed = "--flare-fluent-stroke-pressed";

    /// <summary>Resting tab label and icon color (colorNeutralForeground2).</summary>
    public const string TabFg = "--flare-fluent-tab-fg";
    /// <summary>Hovered and pressed tab label and icon color (colorNeutralForeground2Hover/Pressed).</summary>
    public const string TabFgHover = "--flare-fluent-tab-fg-hover";
    /// <summary>Selected tab label color (colorNeutralForeground1).</summary>
    public const string TabSelectedFg = "--flare-fluent-tab-selected-fg";
    /// <summary>Fluent shadow4, used by the tab focus indicator.</summary>
    public const string Shadow4 = "--flare-fluent-shadow-4";
}
