using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ISplitterJsService" />
public sealed class SplitterJsService : FlareJsModule, ISplitterJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public SplitterJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-theme.js") { }

    /// <inheritdoc />
    public ValueTask RegisterAsync<T>(ElementReference gutter, string axis, string? minSize, string? maxSize,
        DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerSiblingSplitter", gutter, axis, minSize, maxSize, dotNetRef);

    /// <inheritdoc />
    public ValueTask NudgeAsync<T>(ElementReference gutter, string axis, int deltaPx, string keyAxis,
        string? minSize, string? maxSize, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("nudgeSiblingSplitter", gutter, axis, deltaPx, keyAxis, minSize, maxSize, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference gutter)
        => InvokeVoidAsync("removeSiblingSplitter", gutter);
}
