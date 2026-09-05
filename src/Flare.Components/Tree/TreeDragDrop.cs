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
/// Per-tree map from a drag id to the item it stands for, shared from <see cref="FlareTreeView"/> down
/// to each <see cref="FlareTreeItem"/> via a cascading value.
///
/// A tree node is an <c>li</c> and cannot be wrapped in a <c>FlareDraggable</c> without producing
/// markup a browser rearranges, so each item puts the drag model's attributes on its own element and
/// registers here; the view answers <c>FlareDragContext.ResolveItem</c> from this. It replaces a
/// coordinator that held one field - the item picked up at drag start - which existed only because
/// HTML5 drag-and-drop tells the drop target nothing about where the drag began.
/// </summary>
internal sealed class TreeDragRegistry
{
    private readonly Dictionary<string, object> _items = new(StringComparer.Ordinal);

    public void Register(string id, object item) => _items[id] = item;

    public void Unregister(string id) => _items.Remove(id);

    public bool TryGet(string id, out object item) => _items.TryGetValue(id, out item!);
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
