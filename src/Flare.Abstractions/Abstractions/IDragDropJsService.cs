using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for the drag-and-drop model: binds ONE pointer gesture to a
/// <c>FlareDragContext</c> root, and the browser hit-tests every draggable and drop zone beneath it.
/// Wraps <c>flare-dragdrop.js</c> so the component injects a service rather than importing the module.
/// </summary>
public interface IDragDropJsService : IAsyncDisposable
{
    /// <summary>
    /// Attaches the gesture to a context root. The registration is per CONTEXT, not per item, so a list
    /// of a thousand draggables costs one call.
    /// </summary>
    /// <param name="root">The context's root element.</param>
    /// <param name="dotNetRef">Reference the browser calls back on: once when a drag starts, to ask
    /// which targets accept the item, and once when it lands.</param>
    ValueTask RegisterContextAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
        where T : class;

    /// <summary>Detaches the gesture from a context root.</summary>
    ValueTask RemoveContextAsync(ElementReference root);

    /// <summary>
    /// The draggable ids under a context, per zone, in the order the DOM holds them. Registration order
    /// on the .NET side is not render order once a list has been reordered, so a keyboard reorder asks
    /// the browser what the order actually is.
    /// </summary>
    ValueTask<IReadOnlyList<DragZoneOrder>> ItemOrderAsync(ElementReference root, string group);

    /// <summary>
    /// Draws the insertion line at a position named as (zone, index), for the keyboard reorder - which
    /// has no pointer to find a position under, and still has to show one.
    /// </summary>
    ValueTask ShowDropHintAsync(ElementReference root, string targetId, int index, string sourceId);

    /// <summary>Removes the keyboard reorder's insertion line.</summary>
    ValueTask HideDropHintAsync(ElementReference root);
}

/// <summary>One drop zone's draggable ids, in DOM order.</summary>
/// <param name="Target">The zone's <c>Target</c>.</param>
/// <param name="Items">The ids of the draggables the zone owns directly.</param>
public sealed record DragZoneOrder(string Target, IReadOnlyList<string> Items);
