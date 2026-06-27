using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Progress component.
/// These control the geometry, sizing, and appearance of progress indicators.
/// </summary>
public sealed record ProgressTokens
{
    /// <summary>Color of the linear track.</summary>
    [CssVar(ProgressField.TrackColor)] public string TrackColor { get; init; } = Vars.Var(Color.SurfaceContainerHighest);

    /// <summary>Color of the linear indicator (determinate).</summary>
    [CssVar(ProgressField.IndicatorColor)] public string IndicatorColor { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Color of the circular indicator.</summary>
    [CssVar(ProgressField.CircularColor)] public string CircularColor { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Color of the circular track.</summary>
    [CssVar(ProgressField.CircularTrackColor)] public string CircularTrackColor { get; init; } = Vars.Var(Color.SurfaceContainerHighest);

    /// <summary>Height of the linear variant (default).</summary>
    [CssVar(ProgressField.LinearHeight)] public string LinearHeight { get; init; } = "4px";

    /// <summary>Height of the linear variant (small).</summary>
    [CssVar(ProgressField.LinearHeightSm)] public string LinearHeightSm { get; init; } = "2px";

    /// <summary>Height of the linear variant (large).</summary>
    [CssVar(ProgressField.LinearHeightLg)] public string LinearHeightLg { get; init; } = "8px";

    /// <summary>Border radius of the linear variant.</summary>
    [CssVar(ProgressField.LinearRadius)] public string LinearRadius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Size of the circular variant (default).</summary>
    [CssVar(ProgressField.CircularSize)] public string CircularSize { get; init; } = "40px";

    /// <summary>Size of the circular variant (small).</summary>
    [CssVar(ProgressField.CircularSizeSm)] public string CircularSizeSm { get; init; } = "24px";

    /// <summary>Size of the circular variant (large).</summary>
    [CssVar(ProgressField.CircularSizeLg)] public string CircularSizeLg { get; init; } = "56px";

    /// <summary>Stroke width of the circular variant (default).</summary>
    [CssVar(ProgressField.CircularStrokeWidth)] public string CircularStrokeWidth { get; init; } = "4px";

    /// <summary>Stroke width of the circular variant (small).</summary>
    [CssVar(ProgressField.CircularStrokeWidthSm)] public string CircularStrokeWidthSm { get; init; } = "3px";

    /// <summary>Stroke width of the circular variant (large).</summary>
    [CssVar(ProgressField.CircularStrokeWidthLg)] public string CircularStrokeWidthLg { get; init; } = "5px";

    /// <summary>Duration of the indeterminate animation.</summary>
    [CssVar(ProgressField.IndeterminateDuration)] public string IndeterminateDuration { get; init; } = "2s";

    /// <summary>Easing of the indeterminate animation.</summary>
    [CssVar(ProgressField.IndeterminateEasing)] public string IndeterminateEasing { get; init; } = Vars.Var(Motion.EasingStandard);

    /// <summary>Color of the buffer track.</summary>
    [CssVar(ProgressField.BufferColor)] public string BufferColor { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 32%, var(--flare-color-surface-container-highest))";

    /// <summary>Duration of the wavy animation.</summary>
    [CssVar(ProgressField.WavyDuration)] public string WavyDuration { get; init; } = "1.5s";

    // ---- MD3 Expressive linear/circular geometry the component reads at runtime. Defaults match
    // the component's built-in fallbacks so a theme that doesn't set them renders unchanged. ----

    /// <summary>Linear track corner radius. Default = full (rounded track).</summary>
    [CssVar(ProgressField.TrackRadius)] public string TrackRadius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Gap between the active indicator and the remaining track. Default 0.</summary>
    [CssVar(ProgressField.Gap)] public string Gap { get; init; } = "0";

    /// <summary>Trailing stop-indicator dot size. Default 0px (off).</summary>
    [CssVar(ProgressField.StopSize)] public string StopSize { get; init; } = "0px";

    /// <summary>Trailing stop-indicator inset from the end. Default 0px.</summary>
    [CssVar(ProgressField.StopInset)] public string StopInset { get; init; } = "0px";

    /// <summary>Circular indicator stroke width. Default 4px.</summary>
    [CssVar(ProgressField.CircularWidth)] public string CircularWidth { get; init; } = "4px";

    /// <summary>Circular indicator stroke line cap (butt/round). Default butt.</summary>
    [CssVar(ProgressField.CircularCap)] public string CircularCap { get; init; } = "butt";

    /// <summary>Gap between the circular indicator and its track. Default 0.</summary>
    [CssVar(ProgressField.CircularGap)] public string CircularGap { get; init; } = "0";

    /// <summary>Wavy-progress enable flag (1 = on). Default 0 (off).</summary>
    [CssVar(ProgressField.WavyEnabled)] public string WavyEnabled { get; init; } = "0";

    /// <summary>Wavy linear-track height. Default 10px.</summary>
    [CssVar(ProgressField.WavyHeight)] public string WavyHeight { get; init; } = "10px";

    /// <summary>Wave length of the wavy track. Default 40px.</summary>
    [CssVar(ProgressField.WaveLength)] public string WaveLength { get; init; } = "40px";

    /// <summary>Wave amplitude of the wavy track. Default 3px.</summary>
    [CssVar(ProgressField.WaveAmplitude)] public string WaveAmplitude { get; init; } = "3px";

    /// <summary>Wave animation speed. Default 1s.</summary>
    [CssVar(ProgressField.WaveSpeed)] public string WaveSpeed { get; init; } = "1s";

    /// <summary>Circular wavy ring wave count. Default 8.</summary>
    [CssVar(ProgressField.RingWaves)] public string RingWaves { get; init; } = "8";

    /// <summary>Circular wavy ring wave amplitude. Default 1.6.</summary>
    [CssVar(ProgressField.RingWaveAmplitude)] public string RingWaveAmplitude { get; init; } = "1.6";
}
