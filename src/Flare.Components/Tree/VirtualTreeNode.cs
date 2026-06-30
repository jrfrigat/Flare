namespace Flare.Components;

internal sealed class VirtualTreeNode<TItem>
{
    public TItem Item { get; init; } = default!;
    public int Level { get; init; }
    /// <summary>Whether this node is currently in an expanded state.</summary>
    public bool IsExpanded { get; set; }
    /// <summary>Whether this node is currently selected.</summary>
    public bool IsSelected { get; set; }
    public bool HasChildren { get; set; } = true;
    public bool ChildrenLoaded { get; set; }
    public bool IsLoading { get; set; }
    /// <summary>The loaded child items, cached so re-expanding after a collapse re-inserts them without
    /// re-querying the provider.</summary>
    public List<TItem>? Children { get; set; }
}
