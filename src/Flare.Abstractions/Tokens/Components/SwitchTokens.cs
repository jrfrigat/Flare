using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Switch. Track radius, transition, disabled/pressed-layer and the sm/lg size
/// variants are NOT tokens here - switch.css reuses the shared shape/motion/state scales and its own size
/// classes. The remaining tokens are the per-state geometry and colors the CSS reads (off/on/pressed track,
/// thumb and icon).
/// </summary>
public sealed record SwitchTokens
{
    /// <summary>Width of the track.</summary>
    [CssVar(SwitchField.TrackWidth)] public required string TrackWidth { get; init; }

    /// <summary>Height of the track.</summary>
    [CssVar(SwitchField.TrackHeight)] public required string TrackHeight { get; init; }

    /// <summary>Track background when off.</summary>
    [CssVar(SwitchField.TrackOffBg)] public required string TrackOffBg { get; init; }

    /// <summary>Track background when on.</summary>
    [CssVar(SwitchField.TrackOnBg)] public required string TrackOnBg { get; init; }

    /// <summary>Track border (shorthand) when off.</summary>
    [CssVar(SwitchField.TrackBorder)] public required string TrackBorder { get; init; }

    /// <summary>Track border color on hover.</summary>
    [CssVar(SwitchField.TrackHoverBorderColor)] public required string TrackHoverBorderColor { get; init; }

    /// <summary>Thumb diameter when off.</summary>
    [CssVar(SwitchField.ThumbOffSize)] public required string ThumbOffSize { get; init; }

    /// <summary>Thumb diameter when on.</summary>
    [CssVar(SwitchField.ThumbOnSize)] public required string ThumbOnSize { get; init; }

    /// <summary>Thumb diameter when pressed and off.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize)] public required string ThumbPressedOffSize { get; init; }

    /// <summary>Thumb diameter when pressed and on.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize)] public required string ThumbPressedOnSize { get; init; }

    /// <summary>Thumb inline position when off.</summary>
    [CssVar(SwitchField.ThumbOffLeft)] public required string ThumbOffLeft { get; init; }

    /// <summary>Thumb inline position when on.</summary>
    [CssVar(SwitchField.ThumbOnLeft)] public required string ThumbOnLeft { get; init; }

    /// <summary>Thumb color when off.</summary>
    [CssVar(SwitchField.ThumbOffColor)] public required string ThumbOffColor { get; init; }

    /// <summary>Thumb color when on.</summary>
    [CssVar(SwitchField.ThumbOnColor)] public required string ThumbOnColor { get; init; }

    /// <summary>Thumb color in the off hover/pressed state layer.</summary>
    [CssVar(SwitchField.ThumbStateOffColor)] public required string ThumbStateOffColor { get; init; }

    /// <summary>Thumb color in the on hover/pressed state layer.</summary>
    [CssVar(SwitchField.ThumbStateOnColor)] public required string ThumbStateOnColor { get; init; }

    /// <summary>Thumb icon size.</summary>
    [CssVar(SwitchField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Thumb icon color when off.</summary>
    [CssVar(SwitchField.IconOffColor)] public required string IconOffColor { get; init; }

    /// <summary>Thumb icon color when on.</summary>
    [CssVar(SwitchField.IconOnColor)] public required string IconOnColor { get; init; }

    /// <summary>Focus outline (shorthand).</summary>
    [CssVar(SwitchField.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Focus outline offset.</summary>
    [CssVar(SwitchField.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Focus shadow.</summary>
    [CssVar(SwitchField.FocusShadow)] public required string FocusShadow { get; init; }

    /// <summary>Track background on off-hover (default: unchanged from the off track).</summary>
    [CssVar(SwitchField.TrackHoverOffBg)] public required string TrackHoverOffBg { get; init; }

    /// <summary>Track background on on-hover (default: unchanged from the on track).</summary>
    [CssVar(SwitchField.TrackHoverOnBg)] public required string TrackHoverOnBg { get; init; }

    /// <summary>Off-state hover state-layer shadow (set to <c>none</c> to disable).</summary>
    [CssVar(SwitchField.HoverShadowOff)] public required string HoverShadowOff { get; init; }

    /// <summary>On-state hover state-layer shadow (set to <c>none</c> to disable).</summary>
    [CssVar(SwitchField.HoverShadowOn)] public required string HoverShadowOn { get; init; }

    /// <summary>Disabled track background.</summary>
    [CssVar(SwitchField.DisabledTrackBg)] public required string DisabledTrackBg { get; init; }

    /// <summary>Disabled track border color.</summary>
    [CssVar(SwitchField.DisabledTrackBorder)] public required string DisabledTrackBorder { get; init; }

    /// <summary>Disabled thumb (handle) background.</summary>
    [CssVar(SwitchField.DisabledHandleBg)] public required string DisabledHandleBg { get; init; }
}
