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
    // ---- Size-dependent geometry (one token per size; the size class picks which one to read) ----

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

    // ---- Size-independent geometry ----

    /// <summary>Corner radius of the track edges facing a notch (handle / interior anchor); 0 for a square cut.</summary>
    [CssVar(Slider.GapRadius)] public required string GapRadius { get; init; }

    /// <summary>Notch width on each side of the handle; 0 for a continuous track.</summary>
    [CssVar(Slider.Gap)] public required string Gap { get; init; }

    /// <summary>Handle width at rest (a bar handle is narrow; a circular handle sets its diameter).</summary>
    [CssVar(Slider.HandleWidth)] public required string HandleWidth { get; init; }

    /// <summary>Handle width while pressed/focused (a bar handle narrows).</summary>
    [CssVar(Slider.HandlePressedWidth)] public required string HandlePressedWidth { get; init; }

    /// <summary>Handle corner radius (full = circular/pill; 0 for a square or clipped handle).</summary>
    [CssVar(Slider.HandleRadius)] public required string HandleRadius { get; init; }

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

    // ---- Colors / state (per-instance Color still overrides the accent) ----

    /// <summary>Active (filled) track color. Per-instance <c>Color</c> overrides this via <c>--fc-main</c>.</summary>
    [CssVar(Slider.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Inactive (remaining) track color.</summary>
    [CssVar(Slider.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>State-layer diameter around the handle.</summary>
    [CssVar(Slider.StateLayerSize)] public required string StateLayerSize { get; init; }

    /// <summary>State-layer opacity on hover/focus.</summary>
    [CssVar(Slider.StateHoverOpacity)] public required string StateHoverOpacity { get; init; }

    /// <summary>State-layer opacity while pressed.</summary>
    [CssVar(Slider.StatePressedOpacity)] public required string StatePressedOpacity { get; init; }

    /// <summary>Stop-indicator color on the inactive track.</summary>
    [CssVar(Slider.StopColor)] public required string StopColor { get; init; }

    /// <summary>Stop-indicator color on the active (filled) track.</summary>
    [CssVar(Slider.StopColorSelected)] public required string StopColorSelected { get; init; }

    /// <summary>Stop-indicator diameter.</summary>
    [CssVar(Slider.StopSize)] public required string StopSize { get; init; }

    /// <summary>Value-indicator bubble background.</summary>
    [CssVar(Slider.ValueBg)] public required string ValueBg { get; init; }

    /// <summary>Value-indicator bubble text color.</summary>
    [CssVar(Slider.ValueColor)] public required string ValueColor { get; init; }
}
