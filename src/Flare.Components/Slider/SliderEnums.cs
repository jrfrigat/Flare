namespace Flare.Components;

/// <summary>
/// Size of <see cref="FlareSlider"/> (track thickness 16/24/40/56/96dp). Larger sizes also grow the
/// handle and icons. Uses the shared Xs..Xl scale; Xs is the default.
/// </summary>
public enum SliderSize
{
    /// <summary>Extra small - 16dp track, 44dp handle (the default).</summary>
    Xs,
    /// <summary>Small - 24dp track, 44dp handle.</summary>
    Sm,
    /// <summary>Medium - 40dp track, 52dp handle; supports start/end icons.</summary>
    Md,
    /// <summary>Large - 56dp track, 68dp handle; supports start/end icons.</summary>
    Lg,
    /// <summary>Extra large - 96dp track, 108dp handle; supports start/end icons.</summary>
    Xl,
}
