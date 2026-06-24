namespace Flare.Components;

/// <summary>
/// Corner shape of a button. Unifies the Fluent UI 2 shape set (Rounded, Circular, Square)
/// with the Material Design 3 Expressive shape axis; every value resolves to per-theme shape
/// tokens, so a shape looks native in whichever theme is active.
/// </summary>
public enum ButtonShape
{
    /// <summary>
    /// The theme's own button shape, with no override (MD3 = pill, Fluent UI 2 = lightly rounded).
    /// The default - keeps each theme's native button identity.
    /// </summary>
    Default,

    /// <summary>Rounded rectangle using the theme shape scale (Fluent UI 2 "rounded", MD3 "square").</summary>
    Rounded,

    /// <summary>Fully rounded (pill) corners - a circle for an icon-only button.</summary>
    Circular,

    /// <summary>Sharp, non-rounded corners (zero radius), per Fluent UI 2 "square".</summary>
    Square,
}
