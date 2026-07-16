namespace Flare.Components;

/// <summary>
/// The size step of a track-based indicator - <see cref="FlareSlider"/>, <see cref="FlareProgress"/> and
/// <see cref="FlareMeter"/> - on the shared Xs..Xl scale.
///
/// The scale is a set of LABELS, not measurements: each component maps a step onto its own per-size tokens,
/// and each theme decides what those are worth. A slider's <c>Md</c> is a chunky drag target while a progress
/// bar's <c>Md</c> is a few pixels of rule - the same step, deliberately different geometry. What the shared
/// scale does guarantee is that the three stay in step with each other within one theme, and that a caller
/// learns one vocabulary instead of three.
///
/// The DEFAULT step is per component, because the components' natural sizes sit at different points of their
/// own ranges: a slider defaults to <c>Xs</c> (its canonical thin-track form), while a progress bar and a
/// meter default to <c>Md</c> - a rule and a spinner want room to go finer as well as heavier, so their
/// resting size sits mid-scale rather than at the floor.
/// </summary>
public enum TrackSize
{
    /// <summary>Extra small - the finest step.</summary>
    Xs,
    /// <summary>Small.</summary>
    Sm,
    /// <summary>Medium.</summary>
    Md,
    /// <summary>Large.</summary>
    Lg,
    /// <summary>Extra large - the heaviest step.</summary>
    Xl,
}
