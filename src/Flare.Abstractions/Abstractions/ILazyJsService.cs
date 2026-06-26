using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareLazy</c> deferred rendering, backed by an IntersectionObserver in the
/// global <c>flare-components.js</c> bundle. Inject instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface ILazyJsService
{
    /// <summary>Observes <paramref name="element"/> and invokes <c>OnVisibilityChanged</c> as it enters/leaves view.</summary>
    /// <param name="element">The placeholder element to watch.</param>
    /// <param name="dotNetRef">Reference whose <c>OnVisibilityChanged(bool)</c> is invoked.</param>
    /// <param name="rootMargin">IntersectionObserver root margin (e.g. "200px").</param>
    /// <param name="keepRendered">When true, stop observing after the first reveal (render once and stay).</param>
    /// <param name="rootSelector">Optional scroll-ancestor selector to observe within; null watches the viewport.</param>
    ValueTask InitAsync<T>(ElementReference element, DotNetObjectReference<T> dotNetRef,
        string rootMargin, bool keepRendered, string? rootSelector) where T : class;

    /// <summary>Stops observing <paramref name="element"/>.</summary>
    ValueTask RemoveAsync(ElementReference element);
}
