using System.Collections.Generic;

namespace Flare.Components;

/// <summary>
/// Per-subscription tuning for <c>IBrowserViewportService.SubscribeAsync</c>. All properties have
/// sensible defaults, so <c>SubscribeAsync(callback)</c> works with no options at all.
/// </summary>
public sealed class ViewportSubscribeOptions
{
    /// <summary>
    /// Minimum gap, in milliseconds, between resize notifications (trailing throttle). Rapid resize
    /// events are coalesced so the callback runs at most once per window. Default 100ms; set 0 to
    /// disable throttling.
    /// </summary>
    public int ThrottleMs { get; set; } = 100;

    /// <summary>
    /// When true, the callback fires only when the active breakpoint tier changes, not on every
    /// throttled pixel change within the same tier. Default false (report every throttled size change);
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
