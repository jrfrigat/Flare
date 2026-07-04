using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareSlider</c>. Geometry tokens default to the keyword <c>initial</c>,
/// which makes <c>var(--flare-slider-X, &lt;fallback&gt;)</c> use the component's built-in MD3 Expressive
/// fallback (the per-size <c>--_trk-h</c>/<c>--_hnd-h</c>... vars set by the size classes). Because every
/// theme always emits these keys, switching themes deterministically overwrites the previous theme's
/// values (no reliance on stale-token clearing). A theme overrides only what it wants with plain
/// constants - e.g. Fluent sets a thin rail + white circular thumb, and an exotic theme could set
/// <see cref="HandleClipPath"/> to a triangle. Geometry is intentionally NOT size-dependent at the theme
/// level (theme tokens live on <c>:root</c> where the per-size vars are out of scope); size variation
/// comes from the component's size classes.
/// </summary>
public sealed record SliderTokens
{
    // ---- Geometry ("initial" = use the component's per-size MD3 fallback) ----

    /// <summary>Track thickness. <c>initial</c> = size-driven (MD3 16-96dp); Fluent = 4px rail.</summary>
    [CssVar(Slider.TrackHeight)] public required string TrackHeight { get; init; }

    /// <summary>Track corner radius. <c>initial</c> = size-driven; Fluent = full.</summary>
    [CssVar(Slider.TrackRadius)] public required string TrackRadius { get; init; }

    /// <summary>Corner radius of the track edges facing a notch (handle / interior anchor). <c>initial</c> = 2px; Fluent = 0.</summary>
    [CssVar(Slider.GapRadius)] public required string GapRadius { get; init; }

    /// <summary>Notch width on each side of the handle. <c>initial</c> = 6px; Fluent = 0.</summary>
    [CssVar(Slider.Gap)] public required string Gap { get; init; }

    /// <summary>Handle height. <c>initial</c> = size-driven bar (44-108dp); Fluent = circle diameter.</summary>
    [CssVar(Slider.HandleHeight)] public required string HandleHeight { get; init; }

    /// <summary>Handle width at rest. <c>initial</c> = 4px bar; Fluent = circle diameter.</summary>
    [CssVar(Slider.HandleWidth)] public required string HandleWidth { get; init; }

    /// <summary>Handle width while pressed/focused. <c>initial</c> = 2px (MD3 narrows); Fluent = circle diameter.</summary>
    [CssVar(Slider.HandlePressedWidth)] public required string HandlePressedWidth { get; init; }

    /// <summary>Handle corner radius. <c>initial</c> = full (circular). A theme could set 0 for a square/triangle handle.</summary>
    [CssVar(Slider.HandleRadius)] public required string HandleRadius { get; init; }

    /// <summary>Handle <c>clip-path</c> for arbitrary shapes (e.g. a triangle). <c>initial</c> = none.</summary>
    [CssVar(Slider.HandleClipPath)] public required string HandleClipPath { get; init; }

    /// <summary>Handle outline width (drawn in the accent color). <c>initial</c> = 0 (MD3 bar); Fluent = 2px brand ring.</summary>
    [CssVar(Slider.HandleBorderWidth)] public required string HandleBorderWidth { get; init; }

    /// <summary>
    /// Handle fill override. <c>initial</c> (MD3) = handle follows the accent (active) color; Fluent sets
    /// surface (white) so the thumb is a white circle with a brand outline.
    /// </summary>
    [CssVar(Slider.HandleFill)] public required string HandleFill { get; init; }

    // ---- Colors / state (defaults render MD3; per-instance Color still overrides the accent) ----

    /// <summary>Active (filled) track color. Per-instance <c>Color</c> overrides this via <c>--fc-main</c>.</summary>
    [CssVar(Slider.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Inactive (remaining) track color. MD3 = secondary-container; Fluent = outline-variant.</summary>
    [CssVar(Slider.InactiveColor)] public required string InactiveColor { get; init; }

    /// <summary>State-layer diameter around the handle. MD3 = 40dp.</summary>
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
