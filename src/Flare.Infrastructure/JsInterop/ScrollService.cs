using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IScrollService" />
/// <remarks>
/// Owns a single <see cref="DotNetObjectReference{T}"/> to itself and one JS listener per SUBSCRIPTION,
/// which is what <see cref="IScrollService"/> documents and what the JS backend does. Sharing a listener
/// between the subscribers of one target would save a single passive registration per extra subscriber
/// and nothing more - the throttles differ per subscription, so the timers cannot be shared either.
/// Direction, delta and the reversal flag are derived here rather than in JS: each subscription carries
/// its own direction threshold, and keeping the derivation on this side lets the browser send one small
/// position per throttle window instead of a stream of computed events.
/// </remarks>
public sealed class ScrollService : FlareJsModule, IScrollService
{
    private readonly ConcurrentDictionary<string, ScrollSub> _subs = new();
    private readonly object _refLock = new();
    private DotNetObjectReference<ScrollService>? _selfRef;
    private bool _disposed;

    /// <param name="js">The JS runtime (injected).</param>
    public ScrollService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-scroll.js") { }

    private DotNetObjectReference<ScrollService> SelfRef()
    {
        lock (_refLock)
            return _selfRef ??= DotNetObjectReference.Create(this);
    }

    /// <inheritdoc />
    public async ValueTask<ScrollPosition> GetPositionAsync(
        ScrollTarget target = default, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = await InvokeAsync<ScrollPositionDto?>("getPosition", target.Element, target.Selector);
            return dto?.ToPosition() ?? default;
        }
        catch (Exception ex) when (IsInteropTeardown(ex)) { return default; }
    }

    /// <inheritdoc />
    public ValueTask<IAsyncDisposable> SubscribeAsync(
        Action<ScrollChange> handler, ScrollTarget target = default,
        ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return SubscribeAsync(change => { handler(change); return Task.CompletedTask; }, target, options, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> SubscribeAsync(
        Func<ScrollChange, Task> handler, ScrollTarget target = default,
        ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        options ??= new ScrollSubscribeOptions();
        var id = Guid.NewGuid().ToString("N");
        var sub = new ScrollSub(handler, options);
        _subs[id] = sub;

        ScrollPositionDto? initial = null;
        try { initial = await InvokeAsync<ScrollPositionDto?>("subscribe", id, SelfRef(), target.Element, target.Selector, options.ThrottleMs); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { /* prerender: attaches on a later subscribe */ }

        // Seed the baseline from the real starting offset, so the first genuine scroll reports a delta
        // against where the page actually was rather than against zero.
        var position = initial?.ToPosition() ?? default;
        sub.LastTop = position.Top;
        sub.LastLeft = position.Left;
        sub.HasLast = initial is not null;

        if (options.FireImmediately)
        {
            try { await handler(new ScrollChange(position, 0, ScrollDirection.None, DirectionChanged: false, IsImmediate: true)); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        }

        return new AsyncActionDisposable(async () =>
        {
            _subs.TryRemove(id, out _);
            try { await InvokeVoidAsync("unsubscribe", id); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        });
    }

    /// <inheritdoc />
    public async ValueTask ScrollToAsync(double top, ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth)
    {
        try { await InvokeVoidAsync("scrollTo", target.Element, target.Selector, top, behavior.ToString()); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
    }

    /// <inheritdoc />
    public ValueTask ScrollToTopAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth) =>
        ScrollToAsync(0, target, behavior);

    /// <inheritdoc />
    public async ValueTask ScrollToEndAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth)
    {
        try { await InvokeVoidAsync("scrollToEnd", target.Element, target.Selector, behavior.ToString()); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
    }

    /// <inheritdoc />
    public async ValueTask ScrollIntoViewAsync(
        string elementId, ScrollAlign block = ScrollAlign.Nearest, ScrollBehavior behavior = ScrollBehavior.Smooth)
    {
        if (string.IsNullOrWhiteSpace(elementId)) return;
        try { await InvokeVoidAsync("scrollIntoView", elementId, block.ToString(), behavior.ToString()); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
    }

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> LockAsync()
    {
        try { await InvokeVoidAsync("lock"); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }

        return new AsyncActionDisposable(async () =>
        {
            try { await InvokeVoidAsync("unlock"); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        });
    }

    /// <summary>JS callback: a watched target scrolled (throttled). Derives the change and fans it out.</summary>
    /// <param name="id">The subscription the notification belongs to.</param>
    /// <param name="position">The target's position at the end of the throttle window.</param>
    [JSInvokable]
    public async Task OnScrolled(string id, ScrollPositionDto position)
    {
        if (!_subs.TryGetValue(id, out var sub)) return;

        var pos = position.ToPosition();
        var delta = sub.HasLast ? pos.Top - sub.LastTop : 0;
        var horizontalDelta = sub.HasLast ? pos.Left - sub.LastLeft : 0;
        sub.LastTop = pos.Top;
        sub.LastLeft = pos.Left;
        sub.HasLast = true;

        var direction = delta > 0 ? ScrollDirection.Down : delta < 0 ? ScrollDirection.Up : ScrollDirection.None;
        var horizontalDirection = horizontalDelta > 0 ? ScrollDirection.Right
            : horizontalDelta < 0 ? ScrollDirection.Left : ScrollDirection.None;

        // A reversal only counts once the movement clears the threshold, and the running direction is
        // only updated then - otherwise a one-pixel jitter both reports a reversal and becomes the
        // baseline the next real move is compared against.
        var changed = Reversed(delta, direction, sub.LastDirection, sub.Options.DirectionThreshold, out var newVertical);
        sub.LastDirection = newVertical;
        var horizontalChanged = Reversed(
            horizontalDelta, horizontalDirection, sub.LastHorizontalDirection, sub.Options.DirectionThreshold,
            out var newHorizontal);
        sub.LastHorizontalDirection = newHorizontal;

        // Which axis is allowed to filter a notification out - both are always measured and reported.
        var filterSaw = sub.Options.Axis switch
        {
            ScrollAxis.Horizontal => horizontalChanged,
            ScrollAxis.Both => changed || horizontalChanged,
            _ => changed,
        };
        if (sub.Options.DirectionOnly && !filterSaw) return;

        var change = new ScrollChange(
            pos, delta, direction, changed, IsImmediate: false, horizontalDelta, horizontalDirection);
        try { await sub.Handler(change); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _subs.Clear();
        try { await InvokeVoidAsync("disposeAll"); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
        _selfRef?.Dispose();
        await base.DisposeAsync();
    }

    // True when this movement reverses the running direction on one axis. `running` is only advanced
    // past the threshold, so a jitter below it neither reports a reversal nor becomes the baseline the
    // next real move is compared against.
    private static bool Reversed(
        double delta, ScrollDirection direction, ScrollDirection running, double threshold,
        out ScrollDirection updated)
    {
        if (direction == ScrollDirection.None || Math.Abs(delta) <= threshold)
        {
            updated = running;
            return false;
        }

        updated = direction;
        return direction != running;
    }

    private static bool IsInteropTeardown(Exception ex) =>
        ex is JSDisconnectedException or JSException or InvalidOperationException or OperationCanceledException or ObjectDisposedException;

    private sealed class ScrollSub(Func<ScrollChange, Task> handler, ScrollSubscribeOptions options)
    {
        public Func<ScrollChange, Task> Handler { get; } = handler;
        public ScrollSubscribeOptions Options { get; } = options;
        public double LastTop { get; set; }
        public double LastLeft { get; set; }
        public bool HasLast { get; set; }
        public ScrollDirection LastDirection { get; set; }
        public ScrollDirection LastHorizontalDirection { get; set; }
    }

    private sealed class AsyncActionDisposable(Func<ValueTask> dispose) : IAsyncDisposable
    {
        private Func<ValueTask>? _dispose = dispose;

        public async ValueTask DisposeAsync()
        {
            var d = Interlocked.Exchange(ref _dispose, null);
            if (d is not null) await d();
        }
    }

    /// <summary>Wire shape of a scroll position from JS.</summary>
    /// <param name="Top">Distance scrolled from the top.</param>
    /// <param name="Left">Distance scrolled from the left.</param>
    /// <param name="ScrollHeight">Total scrollable height.</param>
    /// <param name="ClientHeight">Visible height.</param>
    /// <param name="ScrollWidth">Total scrollable width.</param>
    /// <param name="ClientWidth">Visible width.</param>
    public sealed record ScrollPositionDto(
        double Top, double Left, double ScrollHeight, double ClientHeight, double ScrollWidth, double ClientWidth)
    {
        internal ScrollPosition ToPosition() =>
            new(Top, Left, ScrollHeight, ClientHeight, ScrollWidth, ClientWidth);
    }
}
