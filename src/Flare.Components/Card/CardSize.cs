namespace Flare.Components;

/// <summary>
/// Overall scale of a <see cref="FlareCard"/>. Controls the corner radius and the inner gap between
/// stacked anatomy slots, resolving to the active theme's shape scale so the size looks native in
/// every theme.
/// </summary>
public enum CardSize
{
    /// <summary>Small - tighter radius and gap (theme shape-small).</summary>
    Sm,

    /// <summary>Medium - the default radius and gap (theme shape-medium).</summary>
    Md,

    /// <summary>Large - rounder radius and roomier gap (theme shape-large).</summary>
    Lg,
}
