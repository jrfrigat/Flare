namespace Flare.Components;

/// <summary>
/// Corner shape of a button, orthogonal to its variant. Every value resolves to per-theme shape
/// tokens, so a shape looks native in whichever theme is active.
/// </summary>
public enum ButtonShape
{
    /// <summary>
    /// The theme's own button shape, with no override.
    /// The default - keeps each theme's native button identity.
    /// </summary>
    Default,

    /// <summary>Rounded rectangle using the theme shape scale.</summary>
    Rounded,

    /// <summary>Fully rounded (pill) corners - a circle for an icon-only button.</summary>
    Circular,

    /// <summary>Sharp, non-rounded corners (zero radius).</summary>
    Square,
}
