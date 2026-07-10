namespace Flare.Components;

/// <summary>
/// Per-subscription tuning for <c>IBrowserViewportService.ObserveElementAsync</c>.
/// </summary>
public sealed class ElementObserveOptions
{
    /// <summary>
    /// Minimum gap, in milliseconds, between size-change notifications for the observed element
    /// (trailing throttle). Default 200ms; set 0 to disable throttling.
    /// </summary>
    public int ThrottleMs { get; set; } = 200;

    /// <summary>
    /// When true (default), a synthetic notification with the element's current geometry is delivered
    /// immediately on observe, so the subscriber starts in sync. The underlying browser
    /// <c>ResizeObserver</c> also emits an initial measurement; when this is false that first emission is
    /// suppressed and only genuine size changes are reported.
    /// </summary>
    public bool FireImmediately { get; set; } = true;
}
