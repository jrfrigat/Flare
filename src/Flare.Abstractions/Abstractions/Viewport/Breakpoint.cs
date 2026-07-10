namespace Flare.Components;

/// <summary>
/// A responsive viewport tier on the shared <c>Xs..Xxl</c> scale. The values are the discrete tiers
/// only; range comparisons (at-least / at-most / between) are expressed with the strongly typed helpers
/// on <see cref="BreakpointExtensions"/> rather than being baked into the enum.
/// <para>Default lower bounds (see <see cref="FlareBreakpoints"/>): Xs 0, Sm 600, Md 960, Lg 1280,
/// Xl 1920, Xxl 2560 (px). The bounds are configurable per subscription and via <c>FlareOptions</c>.</para>
/// </summary>
public enum Breakpoint
{
    /// <summary>Handset / smallest tier (viewport width below the Sm bound; default &lt; 600px).</summary>
    Xs,
    /// <summary>Small tier (default 600px and up, below Md).</summary>
    Sm,
    /// <summary>Medium tier (default 960px and up, below Lg).</summary>
    Md,
    /// <summary>Large tier (default 1280px and up, below Xl).</summary>
    Lg,
    /// <summary>Extra-large tier (default 1920px and up, below Xxl).</summary>
    Xl,
    /// <summary>Extra-extra-large tier (default 2560px and up).</summary>
    Xxl,
}
