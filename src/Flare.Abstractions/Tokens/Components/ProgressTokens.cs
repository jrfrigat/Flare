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
    [CssVar(ProgressField.TrackColor)] public required string TrackColor { get; init; }

    /// <summary>Color of the linear indicator (determinate).</summary>
    [CssVar(ProgressField.IndicatorColor)] public required string IndicatorColor { get; init; }

    /// <summary>Color of the circular indicator.</summary>
    [CssVar(ProgressField.CircularColor)] public required string CircularColor { get; init; }

    /// <summary>Color of the circular track.</summary>
    [CssVar(ProgressField.CircularTrackColor)] public required string CircularTrackColor { get; init; }

    /// <summary>Height of the linear variant (default).</summary>
    [CssVar(ProgressField.LinearHeight)] public required string LinearHeight { get; init; }

    /// <summary>Height of the linear variant (small).</summary>
    [CssVar(ProgressField.LinearHeightSm)] public required string LinearHeightSm { get; init; }

    /// <summary>Height of the linear variant (large).</summary>
    [CssVar(ProgressField.LinearHeightLg)] public required string LinearHeightLg { get; init; }

    /// <summary>Border radius of the linear variant.</summary>
    [CssVar(ProgressField.LinearRadius)] public required string LinearRadius { get; init; }

    /// <summary>Size of the circular variant (default).</summary>
    [CssVar(ProgressField.CircularSize)] public required string CircularSize { get; init; }

    /// <summary>Size of the circular variant (small).</summary>
    [CssVar(ProgressField.CircularSizeSm)] public required string CircularSizeSm { get; init; }

    /// <summary>Size of the circular variant (large).</summary>
    [CssVar(ProgressField.CircularSizeLg)] public required string CircularSizeLg { get; init; }

    /// <summary>Stroke width of the circular variant (default).</summary>
    [CssVar(ProgressField.CircularStrokeWidth)] public required string CircularStrokeWidth { get; init; }

    /// <summary>Stroke width of the circular variant (small).</summary>
    [CssVar(ProgressField.CircularStrokeWidthSm)] public required string CircularStrokeWidthSm { get; init; }

    /// <summary>Stroke width of the circular variant (large).</summary>
    [CssVar(ProgressField.CircularStrokeWidthLg)] public required string CircularStrokeWidthLg { get; init; }

    /// <summary>Duration of the indeterminate animation.</summary>
    [CssVar(ProgressField.IndeterminateDuration)] public required string IndeterminateDuration { get; init; }

    /// <summary>Easing of the indeterminate animation.</summary>
    [CssVar(ProgressField.IndeterminateEasing)] public required string IndeterminateEasing { get; init; }

    /// <summary>Color of the buffer track.</summary>
    [CssVar(ProgressField.BufferColor)] public required string BufferColor { get; init; }

    /// <summary>Duration of the wavy animation.</summary>
    [CssVar(ProgressField.WavyDuration)] public required string WavyDuration { get; init; }

    // ---- Linear/circular geometry the component reads at runtime. Defaults match the component's
    // built-in fallbacks so a theme that doesn't set them renders unchanged. ----

    /// <summary>Linear track corner radius. Default = full (rounded track).</summary>
    [CssVar(ProgressField.TrackRadius)] public required string TrackRadius { get; init; }

    /// <summary>Gap between the active indicator and the remaining track. Default 0.</summary>
    [CssVar(ProgressField.Gap)] public required string Gap { get; init; }

    /// <summary>Trailing stop-indicator dot size. Default 0px (off).</summary>
    [CssVar(ProgressField.StopSize)] public required string StopSize { get; init; }

    /// <summary>Trailing stop-indicator inset from the end. Default 0px.</summary>
    [CssVar(ProgressField.StopInset)] public required string StopInset { get; init; }

    /// <summary>Circular indicator stroke width. Default 4px.</summary>
    [CssVar(ProgressField.CircularWidth)] public required string CircularWidth { get; init; }

    /// <summary>Circular indicator stroke line cap (butt/round). Default butt.</summary>
    [CssVar(ProgressField.CircularCap)] public required string CircularCap { get; init; }

    /// <summary>Gap between the circular indicator and its track. Default 0.</summary>
    [CssVar(ProgressField.CircularGap)] public required string CircularGap { get; init; }

    /// <summary>Wavy-progress enable flag (1 = on). Default 0 (off).</summary>
    [CssVar(ProgressField.WavyEnabled)] public required string WavyEnabled { get; init; }

    /// <summary>Wavy linear-track height. Default 10px.</summary>
    [CssVar(ProgressField.WavyHeight)] public required string WavyHeight { get; init; }

    /// <summary>Wave length of the wavy track. Default 40px.</summary>
    [CssVar(ProgressField.WaveLength)] public required string WaveLength { get; init; }

    /// <summary>Wave amplitude of the wavy track. Default 3px.</summary>
    [CssVar(ProgressField.WaveAmplitude)] public required string WaveAmplitude { get; init; }

    /// <summary>Wave animation speed. Default 1s.</summary>
    [CssVar(ProgressField.WaveSpeed)] public required string WaveSpeed { get; init; }

    /// <summary>Circular wavy ring wave count. Default 8.</summary>
    [CssVar(ProgressField.RingWaves)] public required string RingWaves { get; init; }

    /// <summary>Circular wavy ring wave amplitude. Default 1.6.</summary>
    [CssVar(ProgressField.RingWaveAmplitude)] public required string RingWaveAmplitude { get; init; }
}
