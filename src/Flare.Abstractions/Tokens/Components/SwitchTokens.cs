namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Switch component.
/// These control the geometry, sizing, and appearance of switches.
/// </summary>
public sealed record SwitchTokens
{
    /// <summary>Width of the track (default/md).</summary>
    public string TrackWidth { get; init; } = "52px";

    /// <summary>Height of the track (default/md).</summary>
    public string TrackHeight { get; init; } = "32px";

    /// <summary>Width of the track (sm).</summary>
    public string TrackWidthSm { get; init; } = "40px";

    /// <summary>Height of the track (sm).</summary>
    public string TrackHeightSm { get; init; } = "24px";

    /// <summary>Width of the track (lg).</summary>
    public string TrackWidthLg { get; init; } = "64px";

    /// <summary>Height of the track (lg).</summary>
    public string TrackHeightLg { get; init; } = "40px";

    /// <summary>Border radius of the track.</summary>
    public string TrackRadius { get; init; } = "var(--flare-shape-full)";

    /// <summary>Background color of the track (unchecked).</summary>
    public string TrackColor { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Border color of the track (unchecked).</summary>
    public string TrackBorderColor { get; init; } = "var(--flare-color-outline)";

    /// <summary>Border width of the track (unchecked).</summary>
    public string TrackBorderWidth { get; init; } = "2px";

    /// <summary>Background color of the track (checked).</summary>
    public string TrackColorSelected { get; init; } = "var(--flare-color-primary)";

    /// <summary>Border color of the track (checked).</summary>
    public string TrackBorderColorSelected { get; init; } = "var(--flare-color-primary)";

    /// <summary>Diameter of the thumb (default/md).</summary>
    public string ThumbSize { get; init; } = "24px";

    /// <summary>Diameter of the thumb (sm).</summary>
    public string ThumbSizeSm { get; init; } = "16px";

    /// <summary>Diameter of the thumb (lg).</summary>
    public string ThumbSizeLg { get; init; } = "32px";

    /// <summary>Background color of the thumb (unchecked).</summary>
    public string ThumbColor { get; init; } = "var(--flare-color-outline)";

    /// <summary>Background color of the thumb (checked).</summary>
    public string ThumbColorSelected { get; init; } = "var(--flare-color-on-primary)";

    /// <summary>Color of the thumb icon (unchecked).</summary>
    public string ThumbIconColor { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Color of the thumb icon (checked).</summary>
    public string ThumbIconColorSelected { get; init; } = "var(--flare-color-primary)";

    /// <summary>Shadow of the thumb.</summary>
    public string ThumbShadow { get; init; } = "0 1px 3px 1px rgba(0,0,0,0.15), 0 1px 2px rgba(0,0,0,0.3)";

    /// <summary>Width of the focus outline.</summary>
    public string FocusOutlineWidth { get; init; } = "2px";

    /// <summary>Color of the focus outline.</summary>
    public string FocusOutlineColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Offset of the focus outline.</summary>
    public string FocusOutlineOffset { get; init; } = "2px";

    /// <summary>Transition duration for state changes.</summary>
    public string TransitionDuration { get; init; } = "var(--flare-motion-duration-short2)";

    /// <summary>Transition easing for state changes.</summary>
    public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Background color of the pressed state layer.</summary>
    public string PressedLayerColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Opacity of the pressed state layer.</summary>
    public string PressedLayerOpacity { get; init; } = "var(--flare-state-pressed-opacity)";

    /// <summary>Disabled opacity of the switch.</summary>
    public string DisabledOpacity { get; init; } = "var(--flare-state-disabled-opacity)";
}
