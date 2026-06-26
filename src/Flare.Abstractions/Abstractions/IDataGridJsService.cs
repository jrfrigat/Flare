using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareDataGrid</c>'s DOM-level helpers in the global <c>flare-components.js</c>
/// bundle: column-resize handles, sticky (frozen) column offset recalculation and infinite-scroll
/// observation. Inject instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface IDataGridJsService
{
    /// <summary>Attaches column-resize drag handlers to every header cell in <paramref name="table"/>.</summary>
    ValueTask InitResizeHandlesAsync(ElementReference table);

    /// <summary>Recomputes the cumulative left/right offsets for frozen (sticky) columns in <paramref name="table"/>.</summary>
    ValueTask UpdateFrozenOffsetsAsync(ElementReference table);

    /// <summary>Observes <paramref name="sentinel"/> within <paramref name="root"/> and invokes <c>TriggerLoad</c> on view.</summary>
    /// <param name="sentinel">The bottom sentinel element.</param>
    /// <param name="root">The scroll container (grid wrapper) the sentinel scrolls within.</param>
    /// <param name="dotNetRef">Reference whose <c>TriggerLoad</c> is invoked.</param>
    /// <param name="rootMargin">IntersectionObserver root margin (e.g. "160px").</param>
    ValueTask InitInfiniteAsync<T>(ElementReference sentinel, ElementReference root,
        DotNetObjectReference<T> dotNetRef, string rootMargin) where T : class;

    /// <summary>Stops observing the infinite-scroll <paramref name="sentinel"/>.</summary>
    ValueTask DisposeInfiniteAsync(ElementReference sentinel);
}
