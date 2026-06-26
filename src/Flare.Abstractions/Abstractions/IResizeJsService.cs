using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareResizable</c>: attaches a pointer-drag handle that resizes a single
/// container along one edge. Wraps the shared <c>flare-drag.js</c> module so the component injects a
/// service instead of importing the module itself.
/// </summary>
public interface IResizeJsService : IAsyncDisposable
{
    /// <summary>Attaches a drag handler to <paramref name="handle"/> that resizes <paramref name="container"/>.</summary>
    /// <param name="container">The element being resized.</param>
    /// <param name="handle">The drag handle element.</param>
    /// <param name="edge">Which edge is dragged: "right", "left", "bottom" or "top".</param>
    /// <param name="minSize">Optional minimum size (e.g. "120px").</param>
    /// <param name="maxSize">Optional maximum size (e.g. "600px").</param>
    /// <param name="dotNetRef">Reference used to report the final size back (<c>OnResizedCallback</c>).</param>
    ValueTask RegisterAsync<T>(ElementReference container, ElementReference handle, string edge,
        string? minSize, string? maxSize, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Detaches the drag handler from <paramref name="handle"/>.</summary>
    ValueTask RemoveAsync(ElementReference handle);
}
