using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IResizeJsService" />
public sealed class ResizeJsService : FlareJsModule, IResizeJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public ResizeJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-drag.js") { }

    /// <inheritdoc />
    public ValueTask RegisterAsync<T>(ElementReference container, ElementReference handle, string edge,
        string? minSize, string? maxSize, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerResizeHandle", container, handle, edge, minSize, maxSize, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference handle) => InvokeVoidAsync("removeResizeHandle", handle);
}
