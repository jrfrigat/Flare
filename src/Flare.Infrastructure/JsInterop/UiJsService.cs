using System.Diagnostics.CodeAnalysis;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IUiJsService" />
public sealed class UiJsService : FlareJsModule, IUiJsService
{
    /// <inheritdoc />
    public ValueTask RegisterTabKeyboardAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference bar, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerTabKeyboard", bar, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveTabKeyboardAsync(ElementReference bar) => InvokeVoidAsync("removeTabKeyboard", bar);

    /// <inheritdoc />
    public ValueTask RevealFocusedTabAsync(ElementReference bar) => InvokeVoidAsync("revealFocusedTab", bar);

    /// <param name="js">The JS runtime (injected).</param>
    public UiJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-ui.js") { }

    /// <inheritdoc />
    public ValueTask RegisterTabScrollerAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference bar, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerTabScroller", bar, dotNetRef);

    /// <inheritdoc />
    public ValueTask ScrollTabsAsync(ElementReference bar, int direction) => InvokeVoidAsync("scrollTabs", bar, direction);

    /// <inheritdoc />
    public ValueTask RevealActiveLinkTabAsync(ElementReference bar) => InvokeVoidAsync("revealActiveLinkTab", bar);

    /// <inheritdoc />
    public ValueTask RemoveTabScrollerAsync(ElementReference bar) => InvokeVoidAsync("removeTabScroller", bar);

    /// <inheritdoc />
    public ValueTask RegisterButtonGroupCollapseAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference root, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerButtonGroupCollapse", root, dotNetRef);

    /// <inheritdoc />
    public ValueTask ApplyButtonGroupOverflowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference root, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("applyButtonGroupOverflow", root, dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveButtonGroupCollapseAsync(ElementReference root) => InvokeVoidAsync("removeButtonGroupCollapse", root);

    /// <inheritdoc />
    public ValueTask RegisterShortcutsAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerShortcutListener", dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveShortcutsAsync() => InvokeVoidAsync("removeShortcutListener");

    /// <inheritdoc />
    public ValueTask<bool> SupportsEyeDropperAsync() => InvokeAsync<bool>("supportsEyeDropper");

    /// <inheritdoc />
    public ValueTask<string?> OpenEyeDropperAsync() => InvokeAsync<string?>("openEyeDropper");

    /// <inheritdoc />
    public ValueTask SetUnloadPromptAsync(bool enabled) => InvokeVoidAsync("setUnloadPrompt", enabled);

    /// <inheritdoc />
    public ValueTask<double> GetAnimationDurationMsAsync(ElementReference element)
        => InvokeAsync<double>("getAnimationDurationMs", element);

    /// <inheritdoc />
    public ValueTask<bool> FocusElementAsync(string elementId)
        => InvokeAsync<bool>("focusElement", elementId);

    /// <inheritdoc />
    public ValueTask RegisterTreeKeyboardAsync(ElementReference root) => InvokeVoidAsync("registerTreeKeyboard", root);

    /// <inheritdoc />
    public ValueTask RemoveTreeKeyboardAsync(ElementReference root) => InvokeVoidAsync("removeTreeKeyboard", root);

    /// <inheritdoc />
    public ValueTask RegisterFabMenuKeyboardAsync(ElementReference list) => InvokeVoidAsync("registerFabMenuKeyboard", list);

    /// <inheritdoc />
    public ValueTask RemoveFabMenuKeyboardAsync(ElementReference list) => InvokeVoidAsync("removeFabMenuKeyboard", list);

    /// <inheritdoc />
    public ValueTask FocusTreeItemAsync(ElementReference root, int index) => InvokeVoidAsync("focusTreeItem", root, index);
}
