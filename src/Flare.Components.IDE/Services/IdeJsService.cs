using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.IDE.Services;

/// <summary>
/// Typed JS-interop for <see cref="FlareIdeLayout"/> region resizing. Wraps the
/// <c>flare-ide.js</c> module so the component injects a service instead of calling
/// <see cref="IJSRuntime"/> directly.
/// </summary>
public interface IIdeJsService : IAsyncDisposable
{
    /// <summary>Begins a docked-region drag-resize; JS tracks the pointer and reports the new size.</summary>
    /// <param name="dotNetRef">Reference used to report the new region size back to the layout.</param>
    /// <param name="layout">The IDE layout root element (used to compute sizes).</param>
    /// <param name="position">The region being resized ("Left", "Right" or "Bottom").</param>
    /// <param name="minPx">Smallest size (px) the region may be dragged to.</param>
    ValueTask StartPanelResizeAsync(DotNetObjectReference<FlareIdeLayout> dotNetRef,
        ElementReference layout, string position, int minPx);
}

/// <inheritdoc cref="IIdeJsService" />
public sealed class IdeJsService : FlareJsModule, IIdeJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public IdeJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components.IDE/js/flare-ide.js") { }

    /// <inheritdoc />
    public ValueTask StartPanelResizeAsync(DotNetObjectReference<FlareIdeLayout> dotNetRef,
        ElementReference layout, string position, int minPx)
        => InvokeVoidAsync("startPanelResize", dotNetRef, layout, position, minPx);
}
