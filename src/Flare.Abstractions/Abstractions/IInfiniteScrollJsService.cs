using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareInfiniteScroll</c>, backed by an IntersectionObserver in the global
/// <c>flare-components.js</c> bundle. Inject instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface IInfiniteScrollJsService
{
    /// <summary>Observes <paramref name="sentinel"/> and invokes <c>TriggerLoad</c> when it scrolls into view.</summary>
    /// <param name="sentinel">The bottom sentinel element.</param>
    /// <param name="dotNetRef">Reference whose <c>TriggerLoad</c> is invoked.</param>
    /// <param name="rootMargin">IntersectionObserver root margin (e.g. "0px").</param>
    ValueTask InitAsync<T>(ElementReference sentinel, DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class;

    /// <summary>Stops observing <paramref name="sentinel"/>.</summary>
    ValueTask RemoveAsync(ElementReference sentinel);
}
