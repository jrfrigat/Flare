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
    private readonly Dictionary<string, Node> _items = new(StringComparer.Ordinal);

    // The way back from a payload to the id it was registered under. A drag hands the caller a payload
    // and nothing else, and ancestry is a question about ids. Reference identity, because two nodes
    // holding the same data object are the same node as far as any of this can tell.
    private readonly Dictionary<object, string> _ids = new(ReferenceEqualityComparer.Instance);

    private readonly record struct Node(object Item, string? ParentId);

    public void Register(string id, object item, string? parentId)
    {
        if (_items.TryGetValue(id, out var prior) && !ReferenceEquals(prior.Item, item))
            _ids.Remove(prior.Item);

        _items[id] = new Node(item, parentId);
        _ids[item] = id;
    }

    public void Unregister(string id)
    {
        if (!_items.Remove(id, out var node)) return;
        if (_ids.TryGetValue(node.Item, out var owner) && string.Equals(owner, id, StringComparison.Ordinal))
            _ids.Remove(node.Item);
    }

    public bool TryGet(string id, out object item)
    {
        var found = _items.TryGetValue(id, out var node);
        item = found ? node.Item : null!;
        return found;
    }

    public bool TryGetId(object item, out string id) => _ids.TryGetValue(item, out id!);

    /// <summary>
    /// Every drop zone inside the given node, including its own branch - which is to say the node and
    /// each descendant that has children of its own, since a zone is a branch and a leaf has none.
    /// Leaves are left out because the list is marshalled to the browser and a subtree is mostly leaves.
    ///
    /// The child map is built here rather than kept: it is asked for once at the start of a drag, and
    /// keeping it would mean an invariant to maintain across every expand, collapse and re-registration
    /// of a live tree.
    /// </summary>
    public List<string> Branches(string id)
    {
        var children = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var (childId, node) in _items)
        {
            if (node.ParentId is not { } parent) continue;
            if (!children.TryGetValue(parent, out var siblings)) children[parent] = siblings = [];
            siblings.Add(childId);
        }

        var branches = new List<string> { id };
        var walk = new List<string> { id };
        var seen = new HashSet<string>(StringComparer.Ordinal) { id };
        for (var i = 0; i < walk.Count; i++)
        {
            if (!children.TryGetValue(walk[i], out var next)) continue;
            foreach (var child in next)
            {
                if (!seen.Add(child)) continue;
                walk.Add(child);
                if (children.ContainsKey(child)) branches.Add(child);
            }
        }
        return branches;
    }
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
