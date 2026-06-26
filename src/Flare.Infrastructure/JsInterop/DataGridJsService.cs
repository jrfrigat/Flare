using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IDataGridJsService" />
public sealed class DataGridJsService(IJSRuntime js) : IDataGridJsService
{
    /// <inheritdoc />
    public ValueTask InitResizeHandlesAsync(ElementReference table) =>
        js.InvokeVoidAsync("FlareDataGrid.initAllResizeHandles", table);

    /// <inheritdoc />
    public ValueTask UpdateFrozenOffsetsAsync(ElementReference table) =>
        js.InvokeVoidAsync("FlareDataGrid.updateFrozenOffsets", table);

    /// <inheritdoc />
    public ValueTask InitInfiniteAsync<T>(ElementReference sentinel, ElementReference root,
        DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class
        => js.InvokeVoidAsync("FlareDataGrid.initInfinite", sentinel, root, dotNetRef, rootMargin);

    /// <inheritdoc />
    public ValueTask DisposeInfiniteAsync(ElementReference sentinel) =>
        js.InvokeVoidAsync("FlareDataGrid.disposeInfinite", sentinel);
}
