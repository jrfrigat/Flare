using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Progress indicators. Track/indicator/circular colors, indeterminate timing and
/// buffer color are NOT tokens here - progress.css reuses the shared color/motion scales directly. What
/// remains is the geometry the component actually reads. Circular size, stroke, gap and wave amplitude
/// are CSS lengths resolved by the browser; enlarging the diameter does not scale the other lengths.
/// Unitless circular gap and amplitude values are accepted as pixels for compatibility.
///
/// The size ramps carry one member per step of the <c>TrackSize</c> scale. <c>FlareMeter</c> reads the
/// linear ramp too, so a meter and a linear progress bar at the same size share one track geometry by
/// construction rather than by two themes agreeing.
/// </summary>
public sealed record ProgressTokens
{
    /// <summary>Height of the linear track/indicator at the xs size, which is also the default size.</summary>
    [CssVar(ProgressField.LinearHeight.Xs)] public required string LinearHeightXs { get; init; }
    /// <summary>Height of the linear track/indicator at the sm size.</summary>
    [CssVar(ProgressField.LinearHeight.Sm)] public required string LinearHeightSm { get; init; }
    /// <summary>Height of the linear track/indicator at the md size.</summary>
    [CssVar(ProgressField.LinearHeight.Md)] public required string LinearHeightMd { get; init; }
    /// <summary>Height of the linear track/indicator at the lg size.</summary>
    [CssVar(ProgressField.LinearHeight.Lg)] public required string LinearHeightLg { get; init; }
    /// <summary>Height of the linear track/indicator at the xl size.</summary>
    [CssVar(ProgressField.LinearHeight.Xl)] public required string LinearHeightXl { get; init; }

    /// <summary>Corner radius of the linear track. One value serves every size: a theme that wants a pill
    /// references a shape token that resolves against the height, rather than a per-size literal.</summary>
    [CssVar(ProgressField.TrackRadius)] public required string TrackRadius { get; init; }

    /// <summary>Gap between the active indicator and the remaining track.</summary>
    [CssVar(ProgressField.Gap)] public required string Gap { get; init; }

    /// <summary>Trailing stop-indicator dot size (0 = off).</summary>
    [CssVar(ProgressField.StopSize)] public required string StopSize { get; init; }

    /// <summary>Trailing stop-indicator inset from the end.</summary>
    [CssVar(ProgressField.StopInset)] public required string StopInset { get; init; }

    /// <summary>Color of the trailing stop indicator.</summary>
    [CssVar(ProgressField.StopColor)] public required string StopColor { get; init; }

    /// <summary>Opacity of the buffer track.</summary>
    [CssVar(ProgressField.BufferOpacity)] public required string BufferOpacity { get; init; }

    /// <summary>Outer diameter of the circular variant at the xs size, as a CSS length.</summary>
    [CssVar(ProgressField.CircularSize.Xs)] public required string CircularSizeXs { get; init; }
    /// <summary>Outer diameter of the circular variant at the sm size, as a CSS length.</summary>
    [CssVar(ProgressField.CircularSize.Sm)] public required string CircularSizeSm { get; init; }
    /// <summary>Outer diameter of the circular variant at the md (default) size, as a CSS length.</summary>
    [CssVar(ProgressField.CircularSize.Md)] public required string CircularSizeMd { get; init; }
    /// <summary>Outer diameter of the circular variant at the lg size, as a CSS length.</summary>
    [CssVar(ProgressField.CircularSize.Lg)] public required string CircularSizeLg { get; init; }
    /// <summary>Outer diameter of the circular variant at the xl size, as a CSS length.</summary>
    [CssVar(ProgressField.CircularSize.Xl)] public required string CircularSizeXl { get; init; }

    /// <summary>Stroke width of the circular indicator at the xs size.</summary>
    [CssVar(ProgressField.CircularWidth.Xs)] public required string CircularWidthXs { get; init; }
    /// <summary>Stroke width of the circular indicator at the sm size.</summary>
    [CssVar(ProgressField.CircularWidth.Sm)] public required string CircularWidthSm { get; init; }
    /// <summary>Stroke width of the circular indicator at the md size.</summary>
    [CssVar(ProgressField.CircularWidth.Md)] public required string CircularWidthMd { get; init; }
    /// <summary>Stroke width of the circular indicator at the lg size.</summary>
    [CssVar(ProgressField.CircularWidth.Lg)] public required string CircularWidthLg { get; init; }
    /// <summary>Stroke width of the circular indicator at the xl size.</summary>
    [CssVar(ProgressField.CircularWidth.Xl)] public required string CircularWidthXl { get; init; }

    /// <summary>Line cap of the circular indicator stroke (butt/round).</summary>
    [CssVar(ProgressField.CircularCap)] public required string CircularCap { get; init; }

    /// <summary>Distance along the smooth ring centerline excluded between indicator and track.
    /// A CSS length, independent of diameter; line caps extend into this distance.</summary>
    [CssVar(ProgressField.CircularGap)] public required string CircularGap { get; init; }

    /// <summary>Wavy-progress enable flag (1 = on); read by the component at runtime.</summary>
    [CssVar(ProgressField.WavyEnabled)] public required string WavyEnabled { get; init; }

    /// <summary>Wavy linear-track height.</summary>
    [CssVar(ProgressField.WavyHeight)] public required string WavyHeight { get; init; }

    /// <summary>Wave length of the wavy track.</summary>
    [CssVar(ProgressField.WaveLength)] public required string WaveLength { get; init; }

    /// <summary>Wave length of the indeterminate wavy linear track.</summary>
    [CssVar(ProgressField.IndeterminateWaveLength)] public required string IndeterminateWaveLength { get; init; }

    /// <summary>Wave amplitude of the wavy track.</summary>
    [CssVar(ProgressField.WaveAmplitude)] public required string WaveAmplitude { get; init; }

    /// <summary>Wave animation speed.</summary>
    [CssVar(ProgressField.WaveSpeed)] public required string WaveSpeed { get; init; }

    /// <summary>Duration of one linear indeterminate animation cycle.</summary>
    [CssVar(ProgressField.LinearIndeterminateDuration)] public required string LinearIndeterminateDuration { get; init; }

    /// <summary>Easing used by the moving ends of linear indeterminate indicators.</summary>
    [CssVar(ProgressField.LinearIndeterminateEasing)] public required string LinearIndeterminateEasing { get; init; }

    /// <summary>Duration of one wavy circular indeterminate rotation.</summary>
    [CssVar(ProgressField.CircularWavyIndeterminateRotationDuration)] public required string CircularWavyIndeterminateRotationDuration { get; init; }

    /// <summary>Duration of one wavy circular indeterminate grow and shrink cycle.</summary>
    [CssVar(ProgressField.CircularWavyIndeterminateProgressDuration)] public required string CircularWavyIndeterminateProgressDuration { get; init; }

    /// <summary>Circular wavy ring wave count.</summary>
    [CssVar(ProgressField.RingWaves)] public required string RingWaves { get; init; }

    /// <summary>Preferred circular wave length; the rendered ring uses a whole number of waves.</summary>
    [CssVar(ProgressField.RingWaveLength)] public required string RingWaveLength { get; init; }

    /// <summary>Radial amplitude of the circular wave, as a CSS length (or unitless pixels).
    /// Independent of diameter; clamped when necessary to keep the wave inside the viewport.</summary>
    [CssVar(ProgressField.RingWaveAmplitude)] public required string RingWaveAmplitude { get; init; }
}
