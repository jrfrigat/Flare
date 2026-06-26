using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IInfiniteScrollJsService" />
public sealed class InfiniteScrollJsService(IJSRuntime js) : IInfiniteScrollJsService
{
    /// <inheritdoc />
    public ValueTask InitAsync<T>(ElementReference sentinel, DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class
        => js.InvokeVoidAsync("FlareInfiniteScroll.init", sentinel, dotNetRef, rootMargin);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference sentinel) => js.InvokeVoidAsync("FlareInfiniteScroll.dispose", sentinel);
}
