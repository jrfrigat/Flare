using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IDialogDragJsService" />
public sealed class DialogDragJsService : FlareJsModule, IDialogDragJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public DialogDragJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-drag.js") { }

    /// <inheritdoc />
    public ValueTask RegisterDragAsync(ElementReference handle, ElementReference panel)
        => InvokeVoidAsync("registerDialogDrag", handle, panel);

    /// <inheritdoc />
    public ValueTask RemoveDragAsync(ElementReference handle)
        => InvokeVoidAsync("removeDialogDrag", handle);

    /// <inheritdoc />
    public ValueTask RegisterResizeAsync(ElementReference handle, ElementReference panel, string? minWidth, string? minHeight)
        => InvokeVoidAsync("registerDialogResize", handle, panel, minWidth, minHeight);

    /// <inheritdoc />
    public ValueTask RemoveResizeAsync(ElementReference handle)
        => InvokeVoidAsync("removeDialogResize", handle);
}
