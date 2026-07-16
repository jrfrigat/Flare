namespace Flare.Components;

/// <summary>
/// Size of a <see cref="FlareFloatingActionButton"/>. The steps are labels, not measurements: each theme maps
/// them onto its own padding and radius tokens, so what a step is worth depends on the theme in use.
/// </summary>
public enum FabSize
{
    /// <summary>Small - the compact FAB, for a secondary or in-content action.</summary>
    Sm,

    /// <summary>Baseline - the FAB's resting size, and the default.</summary>
    Md,

    /// <summary>Large - the hero FAB, for a screen's single dominant action.</summary>
    Lg,
}
