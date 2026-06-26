using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ILazyJsService" />
public sealed class LazyJsService(IJSRuntime js) : ILazyJsService
{
    /// <inheritdoc />
    public ValueTask InitAsync<T>(ElementReference element, DotNetObjectReference<T> dotNetRef,
        string rootMargin, bool keepRendered, string? rootSelector) where T : class
        => js.InvokeVoidAsync("FlareLazy.init", element, dotNetRef, rootMargin, keepRendered, rootSelector);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference element) => js.InvokeVoidAsync("FlareLazy.dispose", element);
}
