using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <see cref="FlareSplitter"/>: registers a sibling resize handle, applies
/// keyboard nudges, and detaches the handle. Wraps the shared <c>flare-theme.js</c> module so the
/// component injects a service instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface ISplitterJsService : IAsyncDisposable
{
    /// <summary>Attaches a drag handler to the splitter handle that resizes its two flex siblings.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    /// <param name="axis">"horizontal", "vertical", or "auto" (detect from the parent flex direction).</param>
    /// <param name="minSize">Optional minimum neighbour size (e.g. "120px").</param>
    /// <param name="maxSize">Optional maximum size for the previous neighbour (e.g. "600px").</param>
    /// <param name="dotNetRef">Reference used to report the new size back to the component.</param>
    ValueTask RegisterAsync(ElementReference gutter, string axis, string? minSize, string? maxSize,
        DotNetObjectReference<FlareSplitter> dotNetRef);

    /// <summary>Applies a keyboard nudge (px) along the resolved axis.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    /// <param name="axis">"horizontal", "vertical", or "auto".</param>
    /// <param name="deltaPx">Signed pixel delta to apply to the previous neighbour.</param>
    /// <param name="keyAxis">The arrow key's axis ("x" or "y"); nudges perpendicular to the resize axis are ignored.</param>
    /// <param name="minSize">Optional minimum neighbour size.</param>
    /// <param name="maxSize">Optional maximum size for the previous neighbour.</param>
    /// <param name="dotNetRef">Reference used to report the new size back to the component.</param>
    ValueTask NudgeAsync(ElementReference gutter, string axis, int deltaPx, string keyAxis,
        string? minSize, string? maxSize, DotNetObjectReference<FlareSplitter> dotNetRef);

    /// <summary>Detaches the drag handler from the splitter handle.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    ValueTask RemoveAsync(ElementReference gutter);
}

/// <inheritdoc cref="ISplitterJsService" />
public sealed class SplitterJsService : FlareJsModule, ISplitterJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public SplitterJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-theme.js") { }

    /// <inheritdoc />
    public ValueTask RegisterAsync(ElementReference gutter, string axis, string? minSize, string? maxSize,
        DotNetObjectReference<FlareSplitter> dotNetRef)
        => InvokeVoidAsync("registerSiblingSplitter", gutter, axis, minSize, maxSize, dotNetRef);

    /// <inheritdoc />
    public ValueTask NudgeAsync(ElementReference gutter, string axis, int deltaPx, string keyAxis,
        string? minSize, string? maxSize, DotNetObjectReference<FlareSplitter> dotNetRef)
        => InvokeVoidAsync("nudgeSiblingSplitter", gutter, axis, deltaPx, keyAxis, minSize, maxSize, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference gutter)
        => InvokeVoidAsync("removeSiblingSplitter", gutter);
}
