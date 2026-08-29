namespace Flare.Components;

/// <summary>
/// Per-subscription tuning for <c>IBrowserViewportService.ObserveElementAsync</c>.
/// </summary>
public sealed class ElementObserveOptions
{
    /// <summary>
    /// Quiet period, in milliseconds, that must pass after the last size change before the
    /// subscriber is told. This is a debounce, not a throttle: nothing is reported while the element
    /// is still changing size, and the one notification that arrives carries the settled geometry.
    /// That is what a layout consumer wants - it re-measures once per resize rather than once per
    /// frame of one. Default 200ms; set 0 to report on the next tick after each change.
    /// </summary>
    public int DebounceMs { get; set; } = 200;

    /// <summary>
    /// When true (default), a synthetic notification with the element's current geometry is delivered
    /// immediately on observe, so the subscriber starts in sync. The underlying browser
    /// <c>ResizeObserver</c> also emits an initial measurement; when this is false that first emission is
    /// suppressed and only genuine size changes are reported.
    /// </summary>
    public bool FireImmediately { get; set; } = true;
}
