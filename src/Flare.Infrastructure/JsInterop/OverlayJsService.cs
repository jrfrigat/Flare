using System.Diagnostics.CodeAnalysis;
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
    public ValueTask PrepareAsync() => new(ModuleAsync());

    /// <inheritdoc />
    public ValueTask RegisterDialogEscAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(string id, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerDialogEscHandler", id, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveDialogEscAsync(string id) => InvokeVoidAsync("removeDialogEscHandler", id);

    /// <inheritdoc />
    public ValueTask TrapFocusAsync(string id, ElementReference container) => InvokeImmediateVoidAsync("trapFocus", id, container);

    /// <inheritdoc />
    public ValueTask ReleaseFocusTrapAsync(string id) => InvokeVoidAsync("releaseFocusTrap", id);

    /// <inheritdoc />
    public ValueTask FocusFirstAsync(ElementReference container) => InvokeVoidAsync("focusFirstInDialog", container);

    // Options go across as a JSON string written without reflection (AnchoredPanelOptionsJson), and null as null: the
    // script's defaults are the same as AnchoredPanelOptions'.

    /// <inheritdoc />
    public ValueTask PositionAnchoredPanelAsync(string id, ElementReference anchor, ElementReference panel,
        AnchoredPanelOptions? options = null)
        => InvokeImmediateVoidAsync("positionAnchoredPanel", id, anchor, panel, AnchoredPanelOptionsJson.Write(options));

    /// <inheritdoc />
    public ValueTask PositionAnchoredPanelByIdAsync(string id, string anchorElementId, ElementReference panel,
        AnchoredPanelOptions? options = null)
        => InvokeImmediateVoidAsync("positionAnchoredPanelById", id, anchorElementId, panel, AnchoredPanelOptionsJson.Write(options));

    // These exports finish synchronously. WASM can place and focus without an asynchronous JS round trip;
    // other runtimes keep the asynchronous path, including pending module imports.
    private async ValueTask InvokeImmediateVoidAsync(string identifier, params object?[] args)
    {
        var module = await ModuleAsync();
        if (module is IJSInProcessObjectReference immediate) immediate.InvokeVoid(identifier, args);
        else await module.InvokeVoidAsync(identifier, args);
    }

    /// <inheritdoc />
    public ValueTask RemoveAnchoredPanelAsync(string id) => InvokeVoidAsync("removeAnchoredPanel", id);

    /// <inheritdoc />
    public ValueTask RaiseToTopLayerAsync(ElementReference panel) => InvokeVoidAsync("raiseToTopLayer", panel);

    /// <inheritdoc />
    public ValueTask DropFromTopLayerAsync(ElementReference panel) => InvokeVoidAsync("dropFromTopLayer", panel);

    // One call per circuit rather than one per tooltip: the browser side is idempotent already, but a
    // page of two hundred tooltips would still make two hundred round trips to find that out.
    private bool _tooltipsRequested;

    /// <inheritdoc />
    public ValueTask InitFloatingTooltipsAsync()
    {
        if (_tooltipsRequested) return ValueTask.CompletedTask;
        _tooltipsRequested = true;
        return InvokeVoidAsync("initFloatingTooltips");
    }

    /// <inheritdoc />
    public ValueTask ScrollIntoViewAsync(string optionId, string block = "nearest")
        => InvokeVoidAsync("scrollOptionIntoView", optionId, block);

    /// <inheritdoc />
    public ValueTask RegisterDismissAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(string id, ElementReference element,
        DotNetObjectReference<T> dotNetRef, string method) where T : class
        => InvokeVoidAsync("registerDismiss", id, element, dotNetRef, method);

    /// <inheritdoc />
    public ValueTask RemoveDismissAsync(string id) => InvokeVoidAsync("removeDismiss", id);
}
