using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IDataGridJsService" />
public sealed class DataGridJsService : FlareJsModule, IDataGridJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public DataGridJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") { }

    /// <inheritdoc />
    public ValueTask InitResizeHandlesAsync(ElementReference table) =>
        InvokeVoidAsync("FlareDataGrid.initAllResizeHandles", table);

    /// <inheritdoc />
    public ValueTask UpdateFrozenOffsetsAsync(ElementReference table) =>
        InvokeVoidAsync("FlareDataGrid.updateFrozenOffsets", table);

    /// <inheritdoc />
    public ValueTask InitInfiniteAsync<T>(ElementReference sentinel, ElementReference root,
        DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class
        => InvokeVoidAsync("FlareDataGrid.initInfinite", sentinel, root, dotNetRef, rootMargin);

    /// <inheritdoc />
    public ValueTask DisposeInfiniteAsync(ElementReference sentinel) =>
        InvokeVoidAsync("FlareDataGrid.disposeInfinite", sentinel);
}
