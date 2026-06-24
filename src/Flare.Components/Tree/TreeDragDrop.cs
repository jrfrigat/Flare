namespace Flare.Components;

/// <summary>Event arguments for tree item drop events.</summary>
public sealed class TreeDropEventArgs
{
    /// <summary>The item that was dragged.</summary>
    public required object SourceItem { get; init; }
    /// <summary>The item that the source was dropped on.</summary>
    public required object TargetItem { get; init; }
    /// <summary>The drop position relative to the target (Before, After, Inside).</summary>
    public TreeDropPosition Position { get; init; }
}

/// <summary>Event arguments for tree item drag events.</summary>
public sealed class TreeDragEventArgs
{
    /// <summary>The item being dragged.</summary>
    public required object Item { get; init; }
}

/// <summary>
/// Per-tree coordinator shared from <see cref="FlareTreeView"/> down to each
/// <see cref="FlareTreeItem"/> via a cascading value. It remembers the item picked up at
/// drag start so the drop target can report the real source (rather than itself).
/// </summary>
internal sealed class TreeDragDropCoordinator
{
    /// <summary>The item currently being dragged; set on drag start, cleared on drag end.</summary>
    public object? DraggedItem { get; set; }
}

/// <summary>Position where an item is dropped relative to the target.</summary>
public enum TreeDropPosition
{
    /// <summary>Dropped before the target item.</summary>
    Before,
    /// <summary>Dropped after the target item.</summary>
    After,
    /// <summary>Dropped inside the target item (as a child).</summary>
    Inside,
}
