using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IOverlayJsService" />
public sealed class OverlayJsService : FlareJsModule, IOverlayJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public OverlayJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-overlay.js") { }

    /// <inheritdoc />
    public ValueTask LockBodyScrollAsync() => InvokeVoidAsync("lockBodyScroll");

    /// <inheritdoc />
    public ValueTask UnlockBodyScrollAsync() => InvokeVoidAsync("unlockBodyScroll");

    /// <inheritdoc />
    public ValueTask RegisterDialogEscAsync<T>(string id, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerDialogEscHandler", id, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveDialogEscAsync(string id) => InvokeVoidAsync("removeDialogEscHandler", id);

    /// <inheritdoc />
    public ValueTask TrapFocusAsync(string id, ElementReference container) => InvokeVoidAsync("trapFocus", id, container);

    /// <inheritdoc />
    public ValueTask ReleaseFocusTrapAsync(string id) => InvokeVoidAsync("releaseFocusTrap", id);

    /// <inheritdoc />
    public ValueTask FocusFirstAsync(ElementReference container) => InvokeVoidAsync("focusFirstInDialog", container);

    /// <inheritdoc />
    public ValueTask RegisterOutsideClickAsync<T>(string id, ElementReference element,
        DotNetObjectReference<T> dotNetRef, string method) where T : class
        => InvokeVoidAsync("registerOutsideClick", id, element, dotNetRef, method);

    /// <inheritdoc />
    public ValueTask RemoveOutsideClickAsync(string id) => InvokeVoidAsync("removeOutsideClick", id);

    /// <inheritdoc />
    public ValueTask PositionAnchoredPanelAsync(string id, ElementReference anchor, ElementReference panel, object? options = null)
        => InvokeVoidAsync("positionAnchoredPanel", id, anchor, panel, options ?? new { });

    /// <inheritdoc />
    public ValueTask RemoveAnchoredPanelAsync(string id) => InvokeVoidAsync("removeAnchoredPanel", id);
}
