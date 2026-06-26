using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Group-wide tree state that <see cref="FlareTreeView"/> shares with every descendant
/// <see cref="FlareTreeItem"/> through a single cascading value: whether drag-and-drop is enabled,
/// the drag/drop event callbacks, and the shared drag coordinator. Replaces the previous stack of
/// individually-named cascades. Per-item nesting depth stays a separate <c>Level</c> cascade because
/// it changes for each row, whereas everything here is constant for the whole tree.
/// </summary>
internal sealed class FlareTreeContext
{
    /// <summary>Whether drag-and-drop reordering is enabled for the tree.</summary>
    public required bool Draggable { get; init; }
    /// <summary>Raised when an item is dropped on a valid target.</summary>
    public required EventCallback<TreeDropEventArgs> OnItemDrop { get; init; }
    /// <summary>Raised when an item drag starts.</summary>
    public required EventCallback<TreeDragEventArgs> OnItemDragStart { get; init; }
    /// <summary>Raised when an item drag ends.</summary>
    public required EventCallback OnItemDragEnd { get; init; }
    /// <summary>Shared coordinator tracking the currently dragged source item.</summary>
    public required TreeDragDropCoordinator DragCoordinator { get; init; }
}
