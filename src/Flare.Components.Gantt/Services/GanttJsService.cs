using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Gantt.Services;

/// <summary>
/// Typed JS interop for <see cref="FlareGantt"/>: measures the chart, reports which part of each panel is on screen
/// and scrolls the axis. Wraps the <c>gantt.js</c> module so the component injects a service instead of calling
/// <see cref="IJSRuntime"/> directly.
/// </summary>
public interface IGanttJsService : IAsyncDisposable
{
    /// <summary>
    /// Starts watching one chart. Null when the browser cannot be reached (prerendering, a dropped circuit, a cached
    /// copy of the module older than this service).
    /// </summary>
    /// <param name="root">The chart's root element.</param>
    /// <param name="dotNetRef">Reference the browser reports the chart's width and visible part to.</param>
    /// <param name="virtualize">Whether the chart draws only what is near the visible part.</param>
    /// <param name="endless">Whether the axis grows when the view nears an edge.</param>
    ValueTask<IGanttView?> AttachAsync(ElementReference root, DotNetObjectReference<FlareGantt> dotNetRef, bool virtualize, bool endless);
}

/// <summary>One chart attached through <see cref="IGanttJsService.AttachAsync"/>. Every call is best effort: a browser
/// that cannot be reached leaves the chart as it is instead of throwing into the page.</summary>
public interface IGanttView : IAsyncDisposable
{
    /// <summary>Scrolls the time area so that <paramref name="x"/> pixels from its start sit at its left edge.</summary>
    ValueTask ScrollToAsync(double x);

    /// <summary>Moves the scroll by <paramref name="pixels"/>, keeping the same moment in view after the axis grew on the left.</summary>
    ValueTask ShiftAsync(double pixels);

    /// <summary>Where the pointer was last seen, in pixels from the start of the time area; null before any movement.</summary>
    ValueTask<double?> PointerXAsync();
}

/// <inheritdoc cref="IGanttJsService" />
public sealed class GanttJsService : FlareJsModule, IGanttJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public GanttJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components.Gantt/js/gantt.js") { }

    /// <inheritdoc />
    public async ValueTask<IGanttView?> AttachAsync(ElementReference root, DotNetObjectReference<FlareGantt> dotNetRef, bool virtualize, bool endless)
    {
        try
        {
            var handle = await InvokeAsync<IJSObjectReference?>("attach", root, dotNetRef, virtualize, endless);
            return handle is null ? null : new GanttView(handle);
        }
        catch (JSDisconnectedException) { return null; }
        catch (JSException) { return null; }
        catch (InvalidOperationException) { return null; } // JS interop is unavailable while prerendering
        catch (TaskCanceledException) { return null; }
    }

    private sealed class GanttView(IJSObjectReference handle) : IGanttView
    {
        public ValueTask ScrollToAsync(double x) => TryAsync(() => handle.InvokeVoidAsync("scrollTo", x));

        public ValueTask ShiftAsync(double pixels) => TryAsync(() => handle.InvokeVoidAsync("shift", pixels));

        public async ValueTask<double?> PointerXAsync()
        {
            try { return await handle.InvokeAsync<double?>("pointerX"); }
            catch (JSDisconnectedException) { return null; }
            catch (JSException) { return null; }
            catch (TaskCanceledException) { return null; }
        }

        public async ValueTask DisposeAsync()
        {
            await TryAsync(() => handle.InvokeVoidAsync("dispose"));
            try { await handle.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }

        private static async ValueTask TryAsync(Func<ValueTask> call)
        {
            try { await call(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            catch (TaskCanceledException) { }
        }
    }
}
