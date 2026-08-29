using System.Collections.Generic;

namespace Flare.Components;

/// <summary>
/// Per-subscription tuning for <c>IBrowserViewportService.SubscribeAsync</c>. All properties have
/// sensible defaults, so <c>SubscribeAsync(callback)</c> works with no options at all.
/// </summary>
public sealed class ViewportSubscribeOptions
{
    /// <summary>
    /// Quiet period, in milliseconds, that must pass after the last resize event before the callback
    /// runs. This is a debounce, not a throttle: dragging a window edge reports nothing until the
    /// drag settles, and then reports the final size once. A breakpoint consumer wants exactly that;
    /// a consumer that needs sizes <em>during</em> the drag wants <see cref="IScrollService"/>-style
    /// throttling, which this service deliberately does not do. Default 100ms; set 0 to report on the
    /// next tick after each event.
    /// </summary>
    public int DebounceMs { get; set; } = 100;

    /// <summary>
    /// When true, the callback fires only when the active breakpoint tier changes, not on every
    /// debounced pixel change within the same tier. Default false (report every settled size change);
    /// the breakpoint-only convenience overload sets this to true.
    /// </summary>
    public bool NotifyOnBreakpointOnly { get; set; }

    /// <summary>
    /// When true (default), a synthetic notification with the current size/breakpoint is delivered
    /// immediately on subscribe (marked <see cref="ViewportChange.IsImmediate"/>), so the subscriber
    /// starts in sync without a manual "get current" call.
    /// </summary>
    public bool FireImmediately { get; set; } = true;

    /// <summary>
    /// Overrides the breakpoint lower-bound (min-width, px) map for this subscription. Null uses
    /// <see cref="FlareBreakpoints.Defaults"/>. Only the tiers present are considered.
    /// </summary>
    public IReadOnlyDictionary<Breakpoint, int>? Breakpoints { get; set; }
}
