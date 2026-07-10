using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IBrowserViewportService" />
/// <remarks>
/// Owns a single <see cref="DotNetObjectReference{T}"/> to itself plus one shared JS listener, and fans
/// out to every subscription server-side. Subscribers get a plain disposable token and never touch
/// interop plumbing. Breakpoint resolution happens here (not in JS) so each subscription can carry its
/// own bounds while the browser still runs just one resize listener.
/// </remarks>
public sealed class BrowserViewportService : FlareJsModule, IBrowserViewportService
{
    private readonly ConcurrentDictionary<Guid, ViewportSub> _viewportSubs = new();
    private readonly ConcurrentDictionary<string, ElementSub> _elementSubs = new();
    private readonly object _refLock = new();
    private DotNetObjectReference<BrowserViewportService>? _selfRef;
    private bool _disposed;

    /// <param name="js">The JS runtime (injected).</param>
    public BrowserViewportService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-viewport.js") { }

    private DotNetObjectReference<BrowserViewportService> SelfRef()
    {
        lock (_refLock)
            return _selfRef ??= DotNetObjectReference.Create(this);
    }

    /// <inheritdoc />
    public async ValueTask<ViewportSize> GetViewportSizeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = await InvokeAsync<WindowSizeDto?>("getViewportSize");
            return dto is null ? default : new ViewportSize((int)dto.Width, (int)dto.Height);
        }
        catch (Exception ex) when (IsInteropTeardown(ex)) { return default; }
    }

    /// <inheritdoc />
    public async ValueTask<Breakpoint> GetBreakpointAsync(CancellationToken cancellationToken = default)
    {
        var size = await GetViewportSizeAsync(cancellationToken);
        // No reliable width (prerender/teardown): default to Md rather than reporting the narrowest
        // tier off a 0 reading, mirroring the CSS-side assumption.
        return size.Width > 0 ? FlareBreakpoints.FromWidth(size.Width) : Breakpoint.Md;
    }

    /// <inheritdoc />
    public async ValueTask<bool> MatchesAsync(string mediaQuery, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mediaQuery)) return false;
        try { return await InvokeAsync<bool>("matchMedia", mediaQuery); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { return false; }
    }

    /// <inheritdoc />
    public ValueTask<IAsyncDisposable> SubscribeBreakpointAsync(
        Func<Breakpoint, Task> handler, bool fireImmediately = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var options = new ViewportSubscribeOptions { NotifyOnBreakpointOnly = true, FireImmediately = fireImmediately };
        return SubscribeAsync(change => handler(change.Breakpoint), options, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<IAsyncDisposable> SubscribeBreakpointAsync(
        Action<Breakpoint> handler, bool fireImmediately = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return SubscribeBreakpointAsync(bp => { handler(bp); return Task.CompletedTask; }, fireImmediately, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<IAsyncDisposable> SubscribeAsync(
        Action<ViewportChange> handler, ViewportSubscribeOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return SubscribeAsync(change => { handler(change); return Task.CompletedTask; }, options, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> SubscribeAsync(
        Func<ViewportChange, Task> handler, ViewportSubscribeOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        options ??= new ViewportSubscribeOptions();
        var id = Guid.NewGuid();
        var sub = new ViewportSub(handler, options);
        _viewportSubs[id] = sub;

        // Seed the current breakpoint so the first real resize can report BreakpointChanged correctly,
        // and deliver the immediate notification when requested.
        var size = await GetViewportSizeAsync(cancellationToken);
        var bp = size.Width > 0 ? FlareBreakpoints.FromWidth(size.Width, options.Breakpoints) : Breakpoint.Md;
        sub.LastBreakpoint = bp;
        sub.HasLast = true;

        try { await InvokeVoidAsync("ensureViewportListener", SelfRef(), options.ThrottleMs); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { /* prerender: attaches on a later subscribe */ }

        if (options.FireImmediately)
        {
            try { await sub.Handler(new ViewportChange(size, bp, BreakpointChanged: true, IsImmediate: true)); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        }

        return new AsyncActionDisposable(async () =>
        {
            _viewportSubs.TryRemove(id, out _);
            if (_viewportSubs.IsEmpty)
            {
                try { await InvokeVoidAsync("stopViewportListener"); }
                catch (Exception ex) when (IsInteropTeardown(ex)) { }
            }
        });
    }

    /// <inheritdoc />
    public ValueTask<IAsyncDisposable> ObserveElementAsync(
        ElementReference element, Action<ElementBoundingRect> handler, ElementObserveOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return ObserveElementAsync(element, rect => { handler(rect); return Task.CompletedTask; }, options, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> ObserveElementAsync(
        ElementReference element, Func<ElementBoundingRect, Task> handler, ElementObserveOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        options ??= new ElementObserveOptions();
        var id = Guid.NewGuid().ToString("N");
        _elementSubs[id] = new ElementSub(handler);

        ElementRectDto? initial = null;
        try { initial = await InvokeAsync<ElementRectDto?>("observeElement", id, SelfRef(), element, options.ThrottleMs); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }

        if (options.FireImmediately && initial is not null)
        {
            try { await handler(initial.ToRect()); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        }

        return new AsyncActionDisposable(async () =>
        {
            _elementSubs.TryRemove(id, out _);
            try { await InvokeVoidAsync("unobserveElement", id); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        });
    }

    /// <summary>JS callback: the shared window-resize listener fired (throttled). Fans out to subscribers.</summary>
    [JSInvokable]
    public async Task OnViewportResized(double width, double height)
    {
        if (_viewportSubs.IsEmpty) return;
        var size = new ViewportSize((int)width, (int)height);
        foreach (var sub in _viewportSubs.Values)
        {
            var bp = FlareBreakpoints.FromWidth(width, sub.Options.Breakpoints);
            var changed = !sub.HasLast || bp != sub.LastBreakpoint;
            sub.LastBreakpoint = bp;
            sub.HasLast = true;
            if (sub.Options.NotifyOnBreakpointOnly && !changed) continue;
            try { await sub.Handler(new ViewportChange(size, bp, changed, IsImmediate: false)); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        }
    }

    /// <summary>JS callback: an observed element's size changed (throttled).</summary>
    [JSInvokable]
    public async Task OnElementResized(string id, ElementRectDto rect)
    {
        if (_elementSubs.TryGetValue(id, out var sub))
        {
            try { await sub.Handler(rect.ToRect()); }
            catch (Exception ex) when (IsInteropTeardown(ex)) { }
        }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _viewportSubs.Clear();
        _elementSubs.Clear();
        try { await InvokeVoidAsync("disposeAll"); }
        catch (Exception ex) when (IsInteropTeardown(ex)) { }
        _selfRef?.Dispose();
        await base.DisposeAsync();
    }

    private static bool IsInteropTeardown(Exception ex) =>
        ex is JSDisconnectedException or JSException or InvalidOperationException or OperationCanceledException or ObjectDisposedException;

    private sealed class ViewportSub(Func<ViewportChange, Task> handler, ViewportSubscribeOptions options)
    {
        public Func<ViewportChange, Task> Handler { get; } = handler;
        public ViewportSubscribeOptions Options { get; } = options;
        public Breakpoint LastBreakpoint { get; set; }
        public bool HasLast { get; set; }
    }

    private sealed class ElementSub(Func<ElementBoundingRect, Task> handler)
    {
        public Func<ElementBoundingRect, Task> Handler { get; } = handler;
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

    /// <summary>Wire shape of <c>getViewportSize()</c> / the shared resize callback.</summary>
    public sealed record WindowSizeDto(double Width, double Height);

    /// <summary>Wire shape of an observed element's geometry from JS.</summary>
    public sealed record ElementRectDto(
        double Top, double Left, double Width, double Height,
        double WindowWidth, double WindowHeight, double ScrollX, double ScrollY)
    {
        internal ElementBoundingRect ToRect() =>
            new(Top, Left, Width, Height, WindowWidth, WindowHeight, ScrollX, ScrollY);
    }
}
