using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for an optionally draggable / resizable <c>FlareDialog</c>: attaches the shared
/// pointer-drag gesture so the panel can be moved by its header and resized from a bottom-right gripper.
/// Wraps the shared <c>flare-drag.js</c> module so the component injects a service instead of importing it.
/// </summary>
public interface IDialogDragJsService : IAsyncDisposable
{
    /// <summary>Makes <paramref name="panel"/> draggable by its <paramref name="handle"/> (typically the header).</summary>
    /// <param name="handle">The element that starts the drag (the dialog header).</param>
    /// <param name="panel">The dialog panel that moves.</param>
    ValueTask RegisterDragAsync(ElementReference handle, ElementReference panel);

    /// <summary>Detaches the drag gesture from <paramref name="handle"/>.</summary>
    ValueTask RemoveDragAsync(ElementReference handle);

    /// <summary>Makes <paramref name="panel"/> resizable from a bottom-right <paramref name="handle"/> gripper.</summary>
    /// <param name="handle">The resize gripper element.</param>
    /// <param name="panel">The dialog panel that is resized.</param>
    /// <param name="minWidth">Minimum width (e.g. "16rem" or "260px"), or null for none.</param>
    /// <param name="minHeight">Minimum height (e.g. "10rem" or "160px"), or null for none.</param>
    ValueTask RegisterResizeAsync(ElementReference handle, ElementReference panel, string? minWidth, string? minHeight);

    /// <summary>Detaches the resize gesture from <paramref name="handle"/>.</summary>
    ValueTask RemoveResizeAsync(ElementReference handle);
}
