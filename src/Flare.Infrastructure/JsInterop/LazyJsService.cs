using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ILazyJsService" />
public sealed class LazyJsService : FlareJsModule, ILazyJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public LazyJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") { }

    /// <inheritdoc />
    public ValueTask InitAsync<T>(ElementReference element, DotNetObjectReference<T> dotNetRef,
        string rootMargin, bool keepRendered, string? rootSelector) where T : class
        => InvokeVoidAsync("FlareLazy.init", element, dotNetRef, rootMargin, keepRendered, rootSelector);

    /// <inheritdoc />
    public ValueTask RemoveAsync(ElementReference element) => InvokeVoidAsync("FlareLazy.dispose", element);
}
