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
}
