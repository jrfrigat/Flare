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
    /// <summary>Width of the track at the xs size.</summary>
    [CssVar(SwitchField.TrackWidth.Xs)] public required string TrackWidthXs { get; init; }
    /// <summary>Width of the track at the sm size.</summary>
    [CssVar(SwitchField.TrackWidth.Sm)] public required string TrackWidthSm { get; init; }
    /// <summary>Width of the track at the md (default) size.</summary>
    [CssVar(SwitchField.TrackWidth.Md)] public required string TrackWidthMd { get; init; }
    /// <summary>Width of the track at the lg size.</summary>
    [CssVar(SwitchField.TrackWidth.Lg)] public required string TrackWidthLg { get; init; }
    /// <summary>Width of the track at the xl size.</summary>
    [CssVar(SwitchField.TrackWidth.Xl)] public required string TrackWidthXl { get; init; }

    /// <summary>Height of the track at the xs size.</summary>
    [CssVar(SwitchField.TrackHeight.Xs)] public required string TrackHeightXs { get; init; }
    /// <summary>Height of the track at the sm size.</summary>
    [CssVar(SwitchField.TrackHeight.Sm)] public required string TrackHeightSm { get; init; }
    /// <summary>Height of the track at the md (default) size.</summary>
    [CssVar(SwitchField.TrackHeight.Md)] public required string TrackHeightMd { get; init; }
    /// <summary>Height of the track at the lg size.</summary>
    [CssVar(SwitchField.TrackHeight.Lg)] public required string TrackHeightLg { get; init; }
    /// <summary>Height of the track at the xl size.</summary>
    [CssVar(SwitchField.TrackHeight.Xl)] public required string TrackHeightXl { get; init; }

    /// <summary>Track background when off.</summary>
    [CssVar(SwitchField.TrackOffBg)] public required string TrackOffBg { get; init; }

    /// <summary>Track background when on.</summary>
    [CssVar(SwitchField.TrackOnBg)] public required string TrackOnBg { get; init; }

    /// <summary>Track border (shorthand) when off.</summary>
    [CssVar(SwitchField.TrackBorder)] public required string TrackBorder { get; init; }

    /// <summary>Track border color on hover.</summary>
    [CssVar(SwitchField.TrackHoverBorderColor)] public required string TrackHoverBorderColor { get; init; }

    /// <summary>Thumb diameter when off, at the xs size.</summary>
    [CssVar(SwitchField.ThumbOffSize.Xs)] public required string ThumbOffSizeXs { get; init; }
    /// <summary>Thumb diameter when off, at the sm size.</summary>
    [CssVar(SwitchField.ThumbOffSize.Sm)] public required string ThumbOffSizeSm { get; init; }
    /// <summary>Thumb diameter when off, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbOffSize.Md)] public required string ThumbOffSizeMd { get; init; }
    /// <summary>Thumb diameter when off, at the lg size.</summary>
    [CssVar(SwitchField.ThumbOffSize.Lg)] public required string ThumbOffSizeLg { get; init; }
    /// <summary>Thumb diameter when off, at the xl size.</summary>
    [CssVar(SwitchField.ThumbOffSize.Xl)] public required string ThumbOffSizeXl { get; init; }

    /// <summary>Thumb diameter when on, at the xs size.</summary>
    [CssVar(SwitchField.ThumbOnSize.Xs)] public required string ThumbOnSizeXs { get; init; }
    /// <summary>Thumb diameter when on, at the sm size.</summary>
    [CssVar(SwitchField.ThumbOnSize.Sm)] public required string ThumbOnSizeSm { get; init; }
    /// <summary>Thumb diameter when on, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbOnSize.Md)] public required string ThumbOnSizeMd { get; init; }
    /// <summary>Thumb diameter when on, at the lg size.</summary>
    [CssVar(SwitchField.ThumbOnSize.Lg)] public required string ThumbOnSizeLg { get; init; }
    /// <summary>Thumb diameter when on, at the xl size.</summary>
    [CssVar(SwitchField.ThumbOnSize.Xl)] public required string ThumbOnSizeXl { get; init; }

    /// <summary>Thumb diameter when pressed and off, at the xs size.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize.Xs)] public required string ThumbPressedOffSizeXs { get; init; }
    /// <summary>Thumb diameter when pressed and off, at the sm size.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize.Sm)] public required string ThumbPressedOffSizeSm { get; init; }
    /// <summary>Thumb diameter when pressed and off, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize.Md)] public required string ThumbPressedOffSizeMd { get; init; }
    /// <summary>Thumb diameter when pressed and off, at the lg size.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize.Lg)] public required string ThumbPressedOffSizeLg { get; init; }
    /// <summary>Thumb diameter when pressed and off, at the xl size.</summary>
    [CssVar(SwitchField.ThumbPressedOffSize.Xl)] public required string ThumbPressedOffSizeXl { get; init; }

    /// <summary>Thumb diameter when pressed and on, at the xs size.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize.Xs)] public required string ThumbPressedOnSizeXs { get; init; }
    /// <summary>Thumb diameter when pressed and on, at the sm size.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize.Sm)] public required string ThumbPressedOnSizeSm { get; init; }
    /// <summary>Thumb diameter when pressed and on, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize.Md)] public required string ThumbPressedOnSizeMd { get; init; }
    /// <summary>Thumb diameter when pressed and on, at the lg size.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize.Lg)] public required string ThumbPressedOnSizeLg { get; init; }
    /// <summary>Thumb diameter when pressed and on, at the xl size.</summary>
    [CssVar(SwitchField.ThumbPressedOnSize.Xl)] public required string ThumbPressedOnSizeXl { get; init; }

    /// <summary>Thumb inline position when off, at the xs size.</summary>
    [CssVar(SwitchField.ThumbOffLeft.Xs)] public required string ThumbOffLeftXs { get; init; }
    /// <summary>Thumb inline position when off, at the sm size.</summary>
    [CssVar(SwitchField.ThumbOffLeft.Sm)] public required string ThumbOffLeftSm { get; init; }
    /// <summary>Thumb inline position when off, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbOffLeft.Md)] public required string ThumbOffLeftMd { get; init; }
    /// <summary>Thumb inline position when off, at the lg size.</summary>
    [CssVar(SwitchField.ThumbOffLeft.Lg)] public required string ThumbOffLeftLg { get; init; }
    /// <summary>Thumb inline position when off, at the xl size.</summary>
    [CssVar(SwitchField.ThumbOffLeft.Xl)] public required string ThumbOffLeftXl { get; init; }

    /// <summary>Thumb inline position when on, at the xs size.</summary>
    [CssVar(SwitchField.ThumbOnLeft.Xs)] public required string ThumbOnLeftXs { get; init; }
    /// <summary>Thumb inline position when on, at the sm size.</summary>
    [CssVar(SwitchField.ThumbOnLeft.Sm)] public required string ThumbOnLeftSm { get; init; }
    /// <summary>Thumb inline position when on, at the md (default) size.</summary>
    [CssVar(SwitchField.ThumbOnLeft.Md)] public required string ThumbOnLeftMd { get; init; }
    /// <summary>Thumb inline position when on, at the lg size.</summary>
    [CssVar(SwitchField.ThumbOnLeft.Lg)] public required string ThumbOnLeftLg { get; init; }
    /// <summary>Thumb inline position when on, at the xl size.</summary>
    [CssVar(SwitchField.ThumbOnLeft.Xl)] public required string ThumbOnLeftXl { get; init; }

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
