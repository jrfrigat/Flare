using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// The high-level, dependency-injected entry point for everything responsive: the current viewport
/// size and breakpoint, arbitrary media-query matching, throttled window-resize / breakpoint
/// subscriptions, and per-element resize observation.
/// <para>
/// Subscriptions return an <see cref="IAsyncDisposable"/> token - dispose it to unsubscribe. There is
/// no observer interface to implement, no <c>DotNetObjectReference</c> to create, and no subscription id
/// to track: the service owns a single JS listener shared across all subscribers and fans out
/// server-side. Inject it with <c>@inject IBrowserViewportService Viewport</c>.
/// </para>
/// <para>
/// Register it with <c>AddFlare()</c>. On a prerendered/disconnected circuit the getters return a
/// configured fallback and subscriptions attach lazily once JS is available.
/// </para>
/// </summary>
public interface IBrowserViewportService : IAsyncDisposable
{
    /// <summary>Returns the current viewport breakpoint tier (one-shot).</summary>
    ValueTask<Breakpoint> GetBreakpointAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the current viewport size in CSS pixels (one-shot).</summary>
    ValueTask<ViewportSize> GetViewportSizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates an arbitrary CSS media query against the browser right now, e.g.
    /// <c>"(orientation: portrait)"</c> or <c>"(prefers-reduced-motion: reduce)"</c>. Returns false
    /// when no browser is available (prerender).
    /// </summary>
    ValueTask<bool> MatchesAsync(string mediaQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to breakpoint-tier changes. The <paramref name="handler"/> runs only when the tier
    /// crosses a boundary. Dispose the returned token to unsubscribe.
    /// </summary>
    /// <param name="handler">Invoked with the new tier on every boundary crossing.</param>
    /// <param name="fireImmediately">When true (default) the handler also runs once immediately with the
    /// current tier, so the caller starts in sync.</param>
    /// <param name="cancellationToken">Cancels the subscribe round-trip.</param>
    ValueTask<IAsyncDisposable> SubscribeBreakpointAsync(
        Func<Breakpoint, Task> handler, bool fireImmediately = true, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SubscribeBreakpointAsync(Func{Breakpoint, Task}, bool, CancellationToken)"/>
    ValueTask<IAsyncDisposable> SubscribeBreakpointAsync(
        Action<Breakpoint> handler, bool fireImmediately = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to throttled viewport changes with full <see cref="ViewportChange"/> detail (pixel
    /// size + breakpoint). Dispose the returned token to unsubscribe.
    /// </summary>
    /// <param name="handler">Invoked on each throttled change (and once immediately when
    /// <see cref="ViewportSubscribeOptions.FireImmediately"/> is set).</param>
    /// <param name="options">Throttle rate, breakpoint-only filtering, immediate-fire and custom
    /// breakpoint bounds. Null uses the defaults.</param>
    /// <param name="cancellationToken">Cancels the subscribe round-trip.</param>
    ValueTask<IAsyncDisposable> SubscribeAsync(
        Func<ViewportChange, Task> handler, ViewportSubscribeOptions? options = null, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SubscribeAsync(Func{ViewportChange, Task}, ViewportSubscribeOptions, CancellationToken)"/>
    ValueTask<IAsyncDisposable> SubscribeAsync(
        Action<ViewportChange> handler, ViewportSubscribeOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Observes a single element's size via the browser <c>ResizeObserver</c> API and reports its
    /// geometry (relative to the viewport) on each throttled change. Dispose the returned token to stop
    /// observing.
    /// </summary>
    /// <param name="element">The element to observe.</param>
    /// <param name="handler">Invoked with the element's geometry on each throttled size change.</param>
    /// <param name="options">Throttle rate and immediate-fire behavior. Null uses the defaults.</param>
    /// <param name="cancellationToken">Cancels the observe round-trip.</param>
    ValueTask<IAsyncDisposable> ObserveElementAsync(
        ElementReference element, Func<ElementBoundingRect, Task> handler, ElementObserveOptions? options = null, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="ObserveElementAsync(ElementReference, Func{ElementBoundingRect, Task}, ElementObserveOptions, CancellationToken)"/>
    ValueTask<IAsyncDisposable> ObserveElementAsync(
        ElementReference element, Action<ElementBoundingRect> handler, ElementObserveOptions? options = null, CancellationToken cancellationToken = default);
}
