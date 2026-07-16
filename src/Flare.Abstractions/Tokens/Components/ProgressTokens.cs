using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Progress indicators. Track/indicator/circular colors, indeterminate timing and
/// buffer color are NOT tokens here - progress.css reuses the shared color/motion scales directly. What
/// remains is the geometry the component actually reads (in CSS and, for the wavy variant, in C# via
/// ReadToken).
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

    /// <summary>Diameter of the circular variant at the xs size, which is also the default size.</summary>
    [CssVar(ProgressField.CircularSize.Xs)] public required string CircularSizeXs { get; init; }
    /// <summary>Diameter of the circular variant at the sm size.</summary>
    [CssVar(ProgressField.CircularSize.Sm)] public required string CircularSizeSm { get; init; }
    /// <summary>Diameter of the circular variant at the md size.</summary>
    [CssVar(ProgressField.CircularSize.Md)] public required string CircularSizeMd { get; init; }
    /// <summary>Diameter of the circular variant at the lg size.</summary>
    [CssVar(ProgressField.CircularSize.Lg)] public required string CircularSizeLg { get; init; }
    /// <summary>Diameter of the circular variant at the xl size.</summary>
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

    /// <summary>Gap between the circular indicator and its track.</summary>
    [CssVar(ProgressField.CircularGap)] public required string CircularGap { get; init; }

    /// <summary>Wavy-progress enable flag (1 = on); read by the component at runtime.</summary>
    [CssVar(ProgressField.WavyEnabled)] public required string WavyEnabled { get; init; }

    /// <summary>Wavy linear-track height.</summary>
    [CssVar(ProgressField.WavyHeight)] public required string WavyHeight { get; init; }

    /// <summary>Wave length of the wavy track.</summary>
    [CssVar(ProgressField.WaveLength)] public required string WaveLength { get; init; }

    /// <summary>Wave amplitude of the wavy track.</summary>
    [CssVar(ProgressField.WaveAmplitude)] public required string WaveAmplitude { get; init; }

    /// <summary>Wave animation speed.</summary>
    [CssVar(ProgressField.WaveSpeed)] public required string WaveSpeed { get; init; }

    /// <summary>Circular wavy ring wave count.</summary>
    [CssVar(ProgressField.RingWaves)] public required string RingWaves { get; init; }

    /// <summary>Circular wavy ring wave amplitude.</summary>
    [CssVar(ProgressField.RingWaveAmplitude)] public required string RingWaveAmplitude { get; init; }
}
