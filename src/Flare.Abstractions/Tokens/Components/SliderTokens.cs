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
    public string TrackHeight { get; init; } = "initial";

    /// <summary>Track corner radius. <c>initial</c> = size-driven; Fluent = full.</summary>
    public string TrackRadius { get; init; } = "initial";

    /// <summary>Corner radius of the track edges facing a notch (handle / interior anchor). <c>initial</c> = 2px; Fluent = 0.</summary>
    public string GapRadius { get; init; } = "initial";

    /// <summary>Notch width on each side of the handle. <c>initial</c> = 6px; Fluent = 0.</summary>
    public string Gap { get; init; } = "initial";

    /// <summary>Handle height. <c>initial</c> = size-driven bar (44-108dp); Fluent = circle diameter.</summary>
    public string HandleHeight { get; init; } = "initial";

    /// <summary>Handle width at rest. <c>initial</c> = 4px bar; Fluent = circle diameter.</summary>
    public string HandleWidth { get; init; } = "initial";

    /// <summary>Handle width while pressed/focused. <c>initial</c> = 2px (MD3 narrows); Fluent = circle diameter.</summary>
    public string HandlePressedWidth { get; init; } = "initial";

    /// <summary>Handle corner radius. <c>initial</c> = full (circular). A theme could set 0 for a square/triangle handle.</summary>
    public string HandleRadius { get; init; } = "initial";

    /// <summary>Handle <c>clip-path</c> for arbitrary shapes (e.g. a triangle). <c>initial</c> = none.</summary>
    public string HandleClipPath { get; init; } = "initial";

    /// <summary>Handle outline width (drawn in the accent color). <c>initial</c> = 0 (MD3 bar); Fluent = 2px brand ring.</summary>
    public string HandleBorderWidth { get; init; } = "initial";

    /// <summary>
    /// Handle fill override. <c>initial</c> (MD3) = handle follows the accent (active) color; Fluent sets
    /// surface (white) so the thumb is a white circle with a brand outline.
    /// </summary>
    public string HandleFill { get; init; } = "initial";

    // ---- Colors / state (defaults render MD3; per-instance Color still overrides the accent) ----

    /// <summary>Active (filled) track color. Per-instance <c>Color</c> overrides this via <c>--fc-main</c>.</summary>
    public string ActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Inactive (remaining) track color. MD3 = secondary-container; Fluent = outline-variant.</summary>
    public string InactiveColor { get; init; } = "var(--flare-color-secondary-container)";

    /// <summary>State-layer diameter around the handle. MD3 = 40dp.</summary>
    public string StateLayerSize { get; init; } = "40px";

    /// <summary>State-layer opacity on hover/focus.</summary>
    public string StateHoverOpacity { get; init; } = "0.08";

    /// <summary>State-layer opacity while pressed.</summary>
    public string StatePressedOpacity { get; init; } = "0.10";

    /// <summary>Stop-indicator color on the inactive track.</summary>
    public string StopColor { get; init; } = "var(--flare-color-on-secondary-container)";

    /// <summary>Stop-indicator color on the active (filled) track.</summary>
    public string StopColorSelected { get; init; } = "var(--flare-color-on-primary)";

    /// <summary>Stop-indicator diameter.</summary>
    public string StopSize { get; init; } = "4px";

    /// <summary>Value-indicator bubble background.</summary>
    public string ValueBg { get; init; } = "var(--flare-color-inverse-surface)";

    /// <summary>Value-indicator bubble text color.</summary>
    public string ValueColor { get; init; } = "var(--flare-color-inverse-on-surface)";
}
