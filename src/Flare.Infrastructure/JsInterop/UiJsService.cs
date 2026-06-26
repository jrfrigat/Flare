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
    public ValueTask RegisterScrollTopAsync<T>(string id, DotNetObjectReference<T> dotNetRef, double threshold, string? selector) where T : class
        => InvokeVoidAsync("registerScrollTopHandler", id, dotNetRef, threshold, selector);

    /// <inheritdoc />
    public ValueTask RemoveScrollTopAsync(string id) => InvokeVoidAsync("removeScrollTopHandler", id);

    /// <inheritdoc />
    public ValueTask ScrollToTopAsync(string? selector) => InvokeVoidAsync("scrollToTop", selector);

    /// <inheritdoc />
    public ValueTask<string> GetBreakpointAsync() => InvokeAsync<string>("getBreakpoint");

    /// <inheritdoc />
    public ValueTask<string> SubscribeBreakpointAsync<T>(string id, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeAsync<string>("subscribeBreakpoint", id, dotNetRef);

    /// <inheritdoc />
    public ValueTask UnsubscribeBreakpointAsync(string id) => InvokeVoidAsync("unsubscribeBreakpoint", id);

    /// <inheritdoc />
    public ValueTask RegisterTabScrollerAsync<T>(ElementReference bar, DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerTabScroller", bar, dotNetRef);

    /// <inheritdoc />
    public ValueTask ScrollTabsAsync(ElementReference bar, int direction) => InvokeVoidAsync("scrollTabs", bar, direction);

    /// <inheritdoc />
    public ValueTask RemoveTabScrollerAsync(ElementReference bar) => InvokeVoidAsync("removeTabScroller", bar);

    /// <inheritdoc />
    public ValueTask RegisterShortcutsAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class
        => InvokeVoidAsync("registerShortcutListener", dotNetRef);

    /// <inheritdoc />
    public ValueTask RemoveShortcutsAsync() => InvokeVoidAsync("removeShortcutListener");

    /// <inheritdoc />
    public ValueTask<bool> SupportsEyeDropperAsync() => InvokeAsync<bool>("supportsEyeDropper");

    /// <inheritdoc />
    public ValueTask<string?> OpenEyeDropperAsync() => InvokeAsync<string?>("openEyeDropper");
}
