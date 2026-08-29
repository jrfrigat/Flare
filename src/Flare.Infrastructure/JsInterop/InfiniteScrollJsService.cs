using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IInfiniteScrollJsService" />
public sealed class InfiniteScrollJsService : FlareJsModule, IInfiniteScrollJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public InfiniteScrollJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") { }

    /// <inheritdoc />
    public ValueTask InitAsync<T>(ElementReference sentinel, DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class
        => InvokeVoidAsync("FlareInfiniteScroll.init", sentinel, dotNetRef, rootMargin);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference sentinel) => InvokeVoidAsync("FlareInfiniteScroll.dispose", sentinel);
}
