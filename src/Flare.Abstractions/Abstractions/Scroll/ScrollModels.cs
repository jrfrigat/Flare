using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Where a scroll container stands right now, in CSS pixels, together with the extents needed to tell
/// how far through it that is.
/// </summary>
/// <param name="Top">Distance scrolled from the top.</param>
/// <param name="Left">Distance scrolled from the left.</param>
/// <param name="ScrollHeight">Total scrollable height of the content.</param>
/// <param name="ClientHeight">Visible height of the container.</param>
/// <param name="ScrollWidth">Total scrollable width of the content.</param>
/// <param name="ClientWidth">Visible width of the container.</param>
public readonly record struct ScrollPosition(
    double Top,
    double Left,
    double ScrollHeight,
    double ClientHeight,
    double ScrollWidth,
    double ClientWidth)
{
    /// <summary>How far through the vertical range the container is, from 0 at the top to 1 at the end.
    /// Content that does not overflow reads 0.</summary>
    public double Progress
    {
        get
        {
            var range = ScrollHeight - ClientHeight;
            return range > 0 ? Math.Clamp(Top / range, 0, 1) : 0;
        }
    }

    /// <summary>True while the container is at (or within a pixel of) the top.</summary>
    public bool AtStart => Top <= 1;

    /// <summary>True once the container is at (or within a pixel of) the bottom.</summary>
    public bool AtEnd => Top >= ScrollHeight - ClientHeight - 1;

    /// <summary>True when the content is taller than the container, so there is anywhere to scroll.</summary>
    public bool OverflowsVertically => ScrollHeight > ClientHeight + 1;

    /// <summary>How far through the horizontal range the container is, from 0 at the leading edge to 1
    /// at the trailing one. Content that does not overflow reads 0. Under a right-to-left writing mode
    /// the browser reports <see cref="Left"/> as negative or reversed depending on its scroll-direction
    /// behaviour, so read this as "distance travelled", not as "distance from the visual left".</summary>
    public double HorizontalProgress
    {
        get
        {
            var range = ScrollWidth - ClientWidth;
            return range > 0 ? Math.Clamp(Math.Abs(Left) / range, 0, 1) : 0;
        }
    }

    /// <summary>True while the container is at (or within a pixel of) its leading horizontal edge.</summary>
    public bool AtHorizontalStart => Math.Abs(Left) <= 1;

    /// <summary>True once the container is at (or within a pixel of) its trailing horizontal edge.</summary>
    public bool AtHorizontalEnd => Math.Abs(Left) >= ScrollWidth - ClientWidth - 1;

    /// <summary>True when the content is wider than the container - what an overflow affordance (a tab
    /// bar's arrows, a carousel's chevrons) turns itself on for.</summary>
    public bool OverflowsHorizontally => ScrollWidth > ClientWidth + 1;
}

/// <summary>
/// Which way a scroll container moved between two notifications. One enum for both axes: a vertical
/// reading only ever yields <see cref="None"/>, <see cref="Up"/> or <see cref="Down"/>, and a horizontal
/// one only <see cref="None"/>, <see cref="Left"/> or <see cref="Right"/>.
/// </summary>
public enum ScrollDirection
{
    /// <summary>No movement on this axis since the previous notification.</summary>
    None = 0,
    /// <summary>Moved toward the start - the reader is going back up.</summary>
    Up,
    /// <summary>Moved toward the end - the reader is going down.</summary>
    Down,
    /// <summary>Moved back toward the leading horizontal edge.</summary>
    Left,
    /// <summary>Moved on toward the trailing horizontal edge.</summary>
    Right,
}

/// <summary>How the browser should animate a programmatic scroll.</summary>
public enum ScrollBehavior
{
    /// <summary>Follow the CSS <c>scroll-behavior</c> in effect for the element.</summary>
    Auto = 0,
    /// <summary>Animate to the target.</summary>
    Smooth,
    /// <summary>Jump to the target with no animation.</summary>
    Instant,
}

/// <summary>Where a scrolled-to element should come to rest in the visible area.</summary>
public enum ScrollAlign
{
    /// <summary>Whichever edge is closer - scrolls the least.</summary>
    Nearest = 0,
    /// <summary>Against the start edge.</summary>
    Start,
    /// <summary>Centred.</summary>
    Center,
    /// <summary>Against the end edge.</summary>
    End,
}

/// <summary>
/// Payload delivered to a scroll subscriber. Everything a handler normally branches on is computed
/// once here rather than recomputed by every subscriber from raw offsets.
/// </summary>
/// <param name="Position">Where the container stands, with its extents.</param>
/// <param name="Delta">Vertical pixels moved since the previous notification; negative going up.</param>
/// <param name="Direction">Which way <paramref name="Delta"/> points.</param>
/// <param name="DirectionChanged">True when this notification reversed the direction of travel - the
/// signal a hide-on-scroll app bar actually reacts to.</param>
/// <param name="IsImmediate">True for the synthetic notification delivered right after subscribing
/// (see <see cref="ScrollSubscribeOptions.FireImmediately"/>), not a real scroll event.</param>
/// <param name="HorizontalDelta">Horizontal pixels moved since the previous notification; negative
/// travelling back toward the leading edge.</param>
/// <param name="HorizontalDirection">Which way <paramref name="HorizontalDelta"/> points -
/// <see cref="ScrollDirection.Left"/>, <see cref="ScrollDirection.Right"/> or
/// <see cref="ScrollDirection.None"/>.</param>
public readonly record struct ScrollChange(
    ScrollPosition Position,
    double Delta,
    ScrollDirection Direction,
    bool DirectionChanged,
    bool IsImmediate,
    double HorizontalDelta = 0,
    ScrollDirection HorizontalDirection = ScrollDirection.None)
{
    /// <summary>Distance scrolled from the top, in CSS pixels.</summary>
    public double Top => Position.Top;

    /// <summary>Distance scrolled from the leading horizontal edge, in CSS pixels.</summary>
    public double Left => Position.Left;

    /// <summary>How far through the vertical range the container is, from 0 to 1.</summary>
    public double Progress => Position.Progress;

    /// <summary>How far through the horizontal range the container is, from 0 to 1.</summary>
    public double HorizontalProgress => Position.HorizontalProgress;

    /// <summary>True while the container is at the top.</summary>
    public bool AtStart => Position.AtStart;

    /// <summary>True once the container is at the bottom.</summary>
    public bool AtEnd => Position.AtEnd;

    /// <summary>True while the container is at its leading horizontal edge.</summary>
    public bool AtHorizontalStart => Position.AtHorizontalStart;

    /// <summary>True once the container is at its trailing horizontal edge.</summary>
    public bool AtHorizontalEnd => Position.AtHorizontalEnd;

    /// <summary>True when the content is wider than the container.</summary>
    public bool OverflowsHorizontally => Position.OverflowsHorizontally;

    /// <summary>True when the content is taller than the container.</summary>
    public bool OverflowsVertically => Position.OverflowsVertically;
}

/// <summary>
/// Per-subscription tuning for <see cref="IScrollService"/>. Every property has a working default, so
/// <c>SubscribeAsync(handler)</c> needs no options at all.
/// </summary>
public sealed class ScrollSubscribeOptions
{
    /// <summary>
    /// Minimum gap, in milliseconds, between notifications (trailing throttle). The coalescing happens
    /// in the browser, before the interop crossing, because the crossing is the expensive part.
    /// Default 100ms; 0 disables throttling.
    /// </summary>
    public int ThrottleMs { get; set; } = 100;

    /// <summary>
    /// When true (default), a synthetic notification carrying the current position is delivered on
    /// subscribe (marked <see cref="ScrollChange.IsImmediate"/>), so the subscriber starts in sync.
    /// </summary>
    public bool FireImmediately { get; set; } = true;

    /// <summary>
    /// When true, the handler runs only when the direction of travel reverses, not on every throttled
    /// movement. Default false. A hide-on-scroll app bar wants this: it has nothing to do with the
    /// pixels in between.
    /// </summary>
    public bool DirectionOnly { get; set; }

    /// <summary>
    /// Pixels that must accumulate before a movement counts as a change of direction. Default 0.
    /// Raising it stops a trackpad's jitter from flapping a <see cref="DirectionOnly"/> subscriber.
    /// </summary>
    public double DirectionThreshold { get; set; }

    /// <summary>
    /// Which axis <see cref="DirectionOnly"/> and <see cref="DirectionThreshold"/> watch. Default
    /// <see cref="ScrollAxis.Vertical"/>. Both axes are always measured and reported on every
    /// notification; this only decides which one is allowed to filter one out.
    /// </summary>
    public ScrollAxis Axis { get; set; } = ScrollAxis.Vertical;
}

/// <summary>Which axis a direction filter reacts to.</summary>
public enum ScrollAxis
{
    /// <summary>Only vertical reversals count.</summary>
    Vertical = 0,
    /// <summary>Only horizontal reversals count - a carousel, a horizontal timeline, a Gantt chart.</summary>
    Horizontal,
    /// <summary>A reversal on either axis counts.</summary>
    Both,
}

/// <summary>
/// Which container a scroll call applies to: the page by default, an element the caller holds a
/// reference to, or one addressed by CSS selector.
/// <para>
/// Both an <see cref="ElementReference"/> and a selector string convert implicitly, so the target
/// reads as an ordinary argument: <c>ScrollToTopAsync(_panel)</c> or
/// <c>ScrollToTopAsync(".app-content")</c>, and no argument at all means the page. A selector is what
/// an app shell usually needs - the scrolling panel belongs to the layout, not to the component doing
/// the scrolling, so there is no reference to pass.
/// </para>
/// </summary>
public readonly record struct ScrollTarget
{
    private ScrollTarget(ElementReference? element, string? selector)
    {
        Element = element;
        Selector = selector;
    }

    /// <summary>The element to scroll, when the caller holds a reference to it.</summary>
    public ElementReference? Element { get; }

    /// <summary>The CSS selector of the container to scroll, resolved in the browser at call time.</summary>
    public string? Selector { get; }

    /// <summary>The page itself - the default target.</summary>
    public static ScrollTarget Page => default;

    /// <summary>Targets an element the caller has a reference to.</summary>
    /// <param name="element">The scroll container.</param>
    public static ScrollTarget From(ElementReference element) => new(element, null);

    /// <summary>Targets the first element matching a CSS selector. An unmatched selector is a no-op.</summary>
    /// <param name="selector">A CSS selector, e.g. <c>".app-content"</c>.</param>
    public static ScrollTarget From(string? selector) =>
        string.IsNullOrWhiteSpace(selector) ? Page : new(null, selector);

    /// <summary>True when this target is the page rather than a specific container.</summary>
    public bool IsPage => Element is null && Selector is null;

    /// <summary>Converts an element reference to a target.</summary>
    /// <param name="element">The scroll container.</param>
    public static implicit operator ScrollTarget(ElementReference element) => From(element);

    /// <summary>Converts a CSS selector to a target.</summary>
    /// <param name="selector">A CSS selector.</param>
    public static implicit operator ScrollTarget(string? selector) => From(selector);
}
