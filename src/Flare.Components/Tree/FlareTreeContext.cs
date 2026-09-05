namespace Flare.Components;

/// <summary>
/// Group-wide tree state that <see cref="FlareTreeView"/> shares with every descendant
/// <see cref="FlareTreeItem"/> through a single cascading value: whether drag-and-drop is enabled, and
/// the registry an item puts itself in so a drop can name it. Per-item nesting depth stays a separate
/// <c>Level</c> cascade because it changes for each row, whereas everything here is constant for the
/// whole tree.
///
/// The drag/drop callbacks used to travel here too. They do not any more - the view owns the
/// <c>FlareDragContext</c> and raises them itself, so an item no longer knows what happens after a drop.
/// </summary>
internal sealed class FlareTreeContext
{
    /// <summary>Whether drag-and-drop reordering is enabled for the tree.</summary>
    public required bool Draggable { get; init; }
    /// <summary>Maps each item's drag id to the item it stands for.</summary>
    public required TreeDragRegistry DragRegistry { get; init; }
}
