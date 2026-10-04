using System.Diagnostics.CodeAnalysis;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IUiJsService" />
public sealed class UiJsService : FlareJsModule, IUiJsService
{
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
}
