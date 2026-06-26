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
    [CssVar(ProgressField.TrackColor)] public string TrackColor { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Color of the linear indicator (determinate).</summary>
    [CssVar(ProgressField.IndicatorColor)] public string IndicatorColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Color of the circular indicator.</summary>
    [CssVar(ProgressField.CircularColor)] public string CircularColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Color of the circular track.</summary>
    [CssVar(ProgressField.CircularTrackColor)] public string CircularTrackColor { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Height of the linear variant (default).</summary>
    [CssVar(ProgressField.LinearHeight)] public string LinearHeight { get; init; } = "4px";

    /// <summary>Height of the linear variant (small).</summary>
    [CssVar(ProgressField.LinearHeightSm)] public string LinearHeightSm { get; init; } = "2px";

    /// <summary>Height of the linear variant (large).</summary>
    [CssVar(ProgressField.LinearHeightLg)] public string LinearHeightLg { get; init; } = "8px";

    /// <summary>Border radius of the linear variant.</summary>
    [CssVar(ProgressField.LinearRadius)] public string LinearRadius { get; init; } = "var(--flare-shape-full)";

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
    [CssVar(ProgressField.IndeterminateEasing)] public string IndeterminateEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Color of the buffer track.</summary>
    [CssVar(ProgressField.BufferColor)] public string BufferColor { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 32%, var(--flare-color-surface-container-highest))";

    /// <summary>Duration of the wavy animation.</summary>
    [CssVar(ProgressField.WavyDuration)] public string WavyDuration { get; init; } = "1.5s";
}
