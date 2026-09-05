using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IDragDropJsService" />
public sealed class DragDropJsService : FlareJsModule, IDragDropJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public DragDropJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-dragdrop.js") { }

    /// <inheritdoc />
    public ValueTask RegisterContextAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
        where T : class
        => InvokeVoidAsync("registerDragContext", root, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveContextAsync(ElementReference root)
        => InvokeVoidAsync("removeDragContext", root);

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DragZoneOrder>> ItemOrderAsync(ElementReference root, string group)
        => await InvokeAsync<DragZoneOrder[]>("dragItemOrder", root, group);
}
