using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flare.Components;

/// <summary>
/// The dependency-injected entry point for everything scroll-related: reading where a container
/// stands, subscribing to throttled scroll changes on the page or on one container, driving the scroll
/// position, and taking a reference-counted body scroll lock.
/// <para>
/// Every method takes a <see cref="ScrollTarget"/>, which the page satisfies by default and which an
/// <c>ElementReference</c> or a CSS selector converts to implicitly.
/// </para>
/// <para>
/// Subscriptions return an <see cref="IAsyncDisposable"/> token - dispose it to unsubscribe. There is
/// no observer interface to implement, no <c>DotNetObjectReference</c> to create and no subscription id
/// to track: the service owns one throttled JS listener per subscription and derives direction,
/// progress and the edge flags server-side. Inject it with <c>@inject IScrollService Scroll</c>.
/// </para>
/// <para>
/// Register it with <c>AddFlare()</c>. On a prerendered or disconnected circuit the getters return a
/// zero position and subscriptions attach lazily once JS is available, so no call throws on the server.
/// </para>
/// <para>
/// A threshold crossing is cheaper to observe than a scroll stream: for "is this element on screen"
/// use <c>IntersectionObserver</c> - which is what <c>FlareInfiniteScroll</c> does - rather than
/// subscribing here and comparing offsets.
/// </para>
/// </summary>
public interface IScrollService : IAsyncDisposable
{
    /// <summary>Reads a container's current scroll position and extents (one-shot).</summary>
    /// <param name="target">The scroll container; omit for the page.</param>
    /// <param name="cancellationToken">Cancels the round-trip.</param>
    ValueTask<ScrollPosition> GetPositionAsync(ScrollTarget target = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to throttled scroll changes. Dispose the returned token to unsubscribe.
    /// </summary>
    /// <param name="handler">Invoked on each throttled change (and once immediately when
    /// <see cref="ScrollSubscribeOptions.FireImmediately"/> is set).</param>
    /// <param name="target">The scroll container to watch; omit for the page.</param>
    /// <param name="options">Throttle rate, immediate-fire and direction-only filtering. Null uses the defaults.</param>
    /// <param name="cancellationToken">Cancels the subscribe round-trip.</param>
    ValueTask<IAsyncDisposable> SubscribeAsync(
        Func<ScrollChange, Task> handler, ScrollTarget target = default,
        ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SubscribeAsync(Func{ScrollChange, Task}, ScrollTarget, ScrollSubscribeOptions, CancellationToken)"/>
    ValueTask<IAsyncDisposable> SubscribeAsync(
        Action<ScrollChange> handler, ScrollTarget target = default,
        ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Scrolls a container to an absolute vertical offset.</summary>
    /// <param name="top">Target offset from the top, in CSS pixels.</param>
    /// <param name="target">The scroll container; omit for the page.</param>
    /// <param name="behavior">Whether to animate.</param>
    ValueTask ScrollToAsync(double top, ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth);

    /// <summary>Scrolls a container back to the top.</summary>
    /// <param name="target">The scroll container; omit for the page.</param>
    /// <param name="behavior">Whether to animate.</param>
    ValueTask ScrollToTopAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth);

    /// <summary>Scrolls a container to the end of its content.</summary>
    /// <param name="target">The scroll container; omit for the page.</param>
    /// <param name="behavior">Whether to animate.</param>
    ValueTask ScrollToEndAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth);

    /// <summary>Brings an element into view by its DOM id.</summary>
    /// <param name="elementId">The <c>id</c> attribute of the element to reveal.</param>
    /// <param name="block">Where the element should come to rest.</param>
    /// <param name="behavior">Whether to animate.</param>
    ValueTask ScrollIntoViewAsync(string elementId, ScrollAlign block = ScrollAlign.Nearest, ScrollBehavior behavior = ScrollBehavior.Smooth);

    /// <summary>
    /// Freezes page scrolling until the returned token is disposed. Locks are reference-counted, so a
    /// dialog opened over a drawer does not hand the page back when only the inner one closes.
    /// </summary>
    ValueTask<IAsyncDisposable> LockAsync();
}
