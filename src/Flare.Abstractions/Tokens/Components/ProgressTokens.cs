using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Progress indicators. Track/indicator/circular colors, the sm/lg size and stroke
/// variants, indeterminate timing and buffer color are NOT tokens here - progress.css reuses the shared
/// color/motion scales (and the component's own size classes) directly. What remains is the geometry the
/// component actually reads (in CSS and, for the wavy variant, in C# via ReadToken).
/// </summary>
public sealed record ProgressTokens
{
    /// <summary>Height of the linear track/indicator.</summary>
    [CssVar(ProgressField.LinearHeight)] public required string LinearHeight { get; init; }

    /// <summary>Corner radius of the linear track.</summary>
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

    /// <summary>Diameter of the circular variant.</summary>
    [CssVar(ProgressField.CircularSize)] public required string CircularSize { get; init; }

    /// <summary>Stroke width of the circular indicator.</summary>
    [CssVar(ProgressField.CircularWidth)] public required string CircularWidth { get; init; }

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
