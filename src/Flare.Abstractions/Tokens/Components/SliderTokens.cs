using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareSlider</c>. The theme supplies every value; the component CSS carries no
/// defaults of its own. Size-dependent geometry (track thickness / radius, handle height) therefore gets
/// ONE TOKEN PER SIZE: the theme emits all five on <c>:root</c> and the component's size class reads the
/// matching one, the same shape <c>FlareButton</c> uses for its per-size gaps and heights. A theme that
/// wants a single value for every size simply sets the same value five times; a theme that wants a real
/// ramp can express it - which a single <c>:root</c> token never could.
/// </summary>
public sealed record SliderTokens
{

    /// <summary>Track thickness at the xs size.</summary>
    [CssVar(Slider.TrackHeight.Xs)] public required string TrackHeightXs { get; init; }
    /// <summary>Track thickness at the sm size.</summary>
    [CssVar(Slider.TrackHeight.Sm)] public required string TrackHeightSm { get; init; }
    /// <summary>Track thickness at the md (default) size.</summary>
    [CssVar(Slider.TrackHeight.Md)] public required string TrackHeightMd { get; init; }
    /// <summary>Track thickness at the lg size.</summary>
    [CssVar(Slider.TrackHeight.Lg)] public required string TrackHeightLg { get; init; }
    /// <summary>Track thickness at the xl size.</summary>
    [CssVar(Slider.TrackHeight.Xl)] public required string TrackHeightXl { get; init; }

    /// <summary>Track corner radius at the xs size.</summary>
    [CssVar(Slider.TrackRadius.Xs)] public required string TrackRadiusXs { get; init; }
    /// <summary>Track corner radius at the sm size.</summary>
    [CssVar(Slider.TrackRadius.Sm)] public required string TrackRadiusSm { get; init; }
    /// <summary>Track corner radius at the md (default) size.</summary>
    [CssVar(Slider.TrackRadius.Md)] public required string TrackRadiusMd { get; init; }
    /// <summary>Track corner radius at the lg size.</summary>
    [CssVar(Slider.TrackRadius.Lg)] public required string TrackRadiusLg { get; init; }
    /// <summary>Track corner radius at the xl size.</summary>
    [CssVar(Slider.TrackRadius.Xl)] public required string TrackRadiusXl { get; init; }

    /// <summary>Handle height at the xs size.</summary>
    [CssVar(Slider.HandleHeight.Xs)] public required string HandleHeightXs { get; init; }
    /// <summary>Handle height at the sm size.</summary>
    [CssVar(Slider.HandleHeight.Sm)] public required string HandleHeightSm { get; init; }
    /// <summary>Handle height at the md (default) size.</summary>
    [CssVar(Slider.HandleHeight.Md)] public required string HandleHeightMd { get; init; }
    /// <summary>Handle height at the lg size.</summary>
    [CssVar(Slider.HandleHeight.Lg)] public required string HandleHeightLg { get; init; }
    /// <summary>Handle height at the xl size.</summary>
    [CssVar(Slider.HandleHeight.Xl)] public required string HandleHeightXl { get; init; }

    /// <summary>Size of the icons flanking the track at the xs size.</summary>
    [CssVar(Slider.IconSize.Xs)] public required string IconSizeXs { get; init; }
    /// <summary>Size of the icons flanking the track at the sm size.</summary>
    [CssVar(Slider.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Size of the icons flanking the track at the md (default) size.</summary>
    [CssVar(Slider.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Size of the icons flanking the track at the lg size.</summary>
    [CssVar(Slider.IconSize.Lg)] public required string IconSizeLg { get; init; }
    /// <summary>Size of the icons flanking the track at the xl size.</summary>
    [CssVar(Slider.IconSize.Xl)] public required string IconSizeXl { get; init; }


    /// <summary>
    /// Default length of a VERTICAL slider (a horizontal one fills its container, but a vertical one has no
    /// height to fill). The length is the whole row - start and end icons and their gaps included - and the
    /// track takes what is left. This is the theme's default; a consumer overrides it per instance with an
    /// inline <c>--flare-slider-length</c>, which wins over the theme's value.
    /// </summary>
    [CssVar(Slider.Length)] public required string Length { get; init; }

    /// <summary>Corner radius of the track edges facing a notch (handle / interior anchor); 0 for a square cut.</summary>
    [CssVar(Slider.GapRadius)] public required string GapRadius { get; init; }

    /// <summary>Notch width on each side of the handle; 0 for a continuous track.</summary>
    [CssVar(Slider.Gap)] public required string Gap { get; init; }

    /// <summary>Handle width at rest (a bar handle is narrow; a circular handle sets its diameter).</summary>
    [CssVar(Slider.HandleWidth)] public required string HandleWidth { get; init; }

    /// <summary>Handle width while pressed/focused (a bar handle narrows).</summary>
    [CssVar(Slider.HandlePressedWidth)] public required string HandlePressedWidth { get; init; }

    /// <summary>Handle corner radius (full = circular/pill; 0 for a square or clipped handle).</summary>
    [CssVar(Slider.HandleRadius)] public string HandleRadius { get; init; } = "var(--flare-shape-full)";

    /// <summary>Handle <c>clip-path</c> for arbitrary shapes (e.g. a triangle); <c>none</c> for no clipping.</summary>
    [CssVar(Slider.HandleClipPath)] public required string HandleClipPath { get; init; }

    /// <summary>Handle outline width, drawn in the accent color; 0 for a plain bar handle.</summary>
    [CssVar(Slider.HandleBorderWidth)] public required string HandleBorderWidth { get; init; }

    /// <summary>
    /// Handle fill. To keep the handle following the per-instance <c>Color</c>, reference the local accent
    /// (<c>var(--fc-main, ...)</c>) rather than a fixed color; a theme that wants a surface-filled thumb
    /// with a brand outline sets a surface color here instead.
    /// </summary>
    [CssVar(Slider.HandleFill)] public required string HandleFill { get; init; }


    /// <summary>Active (filled) track color. Per-instance <c>Color</c> overrides this via <c>--fc-main</c>.</summary>
    [CssVar(Slider.ActiveColor)] public string ActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Inactive (remaining) track color.</summary>
    [CssVar(Slider.InactiveColor)] public string InactiveColor { get; init; } = "var(--flare-color-secondary-container)";

    /// <summary>State-layer diameter around the handle.</summary>
    [CssVar(Slider.StateLayerSize)] public required string StateLayerSize { get; init; }

    /// <summary>State-layer opacity on hover/focus.</summary>
    [CssVar(Slider.StateHoverOpacity)] public string StateHoverOpacity { get; init; } = "var(--flare-state-hover-opacity)";

    /// <summary>State-layer opacity while pressed.</summary>
    [CssVar(Slider.StatePressedOpacity)] public string StatePressedOpacity { get; init; } = "var(--flare-state-pressed-opacity)";

    /// <summary>
    /// Outline (shorthand) drawn round the track area while the handle has keyboard focus. <c>none</c>
    /// for a language that shows focus with the state layer alone.
    /// </summary>
    [CssVar(Slider.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Gap between the track area and the keyboard focus outline.</summary>
    [CssVar(Slider.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Stop-indicator color on the inactive track.</summary>
    [CssVar(Slider.StopColor)] public string StopColor { get; init; } = "var(--flare-color-on-secondary-container)";

    /// <summary>Stop-indicator color on the active (filled) track.</summary>
    [CssVar(Slider.StopColorSelected)] public string StopColorSelected { get; init; } = "var(--flare-color-on-primary)";

    /// <summary>Stop-indicator diameter.</summary>
    [CssVar(Slider.StopSize)] public required string StopSize { get; init; }

    /// <summary>Value-indicator bubble background.</summary>
    [CssVar(Slider.ValueBg)] public string ValueBg { get; init; } = "var(--flare-color-inverse-surface)";

    /// <summary>Value-indicator bubble text color.</summary>
    [CssVar(Slider.ValueColor)] public string ValueColor { get; init; } = "var(--flare-color-inverse-on-surface)";

    /// <summary>How far a disabled slider's label, value bubble and icons fade. The track and handle
    /// are repainted rather than faded, so this covers only the text around them.</summary>
    [CssVar(Slider.DisabledOpacity)] public string DisabledOpacity { get; init; } = "var(--flare-state-disabled-opacity)";

    /// <summary>Filled track and handle color while the slider is disabled. A design language that
    /// signals disabled with a flat palette names its grey here; one that fades names a translucent
    /// mix of its content color.</summary>
    [CssVar(Slider.DisabledActiveColor)] public string DisabledActiveColor { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-disabled-opacity) * 100%), transparent)";

    /// <summary>Remaining-track color while the slider is disabled. Separate from
    /// <see cref="DisabledActiveColor"/> because the filled and unfilled halves stay distinguishable
    /// in both models - one fades further than the other rather than matching it.</summary>
    [CssVar(Slider.DisabledInactiveColor)] public string DisabledInactiveColor { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)";
}
