using System.Collections.Immutable;

namespace Flare.Components.IDE;

/// <summary>A workspace layout node with a key unique within the workspace.</summary>
public abstract record DockNode
{
    private protected DockNode(string key) => Key = key;

    /// <summary>The stable identity of the node.</summary>
    public string Key { get; }
}

/// <summary>A group of docked tabs with its own selection.</summary>
public sealed record DockGroup : DockNode
{
    /// <summary>Creates a group with its selection.</summary>
    /// <param name="key">The stable identity of the group.</param>
    /// <param name="items">Catalog keys in display order.</param>
    /// <param name="activeKey">The selected item, or null for no selection.</param>
    public DockGroup(string key, ImmutableArray<string> items, string? activeKey = null) : base(key)
    {
        Items = items;
        ActiveKey = activeKey;
    }

    /// <summary>Catalog keys in display order.</summary>
    public ImmutableArray<string> Items { get; init; }

    /// <summary>The selected item, or null for no selection.</summary>
    public string? ActiveKey { get; init; }
}

/// <summary>The axis on which a split distributes space.</summary>
public enum DockAxis
{
    /// <summary>Place branches side by side.</summary>
    Horizontal,

    /// <summary>Place branches above and below one another.</summary>
    Vertical
}

/// <summary>Two branches separated by a resizable divider.</summary>
public sealed record DockSplit : DockNode
{
    /// <summary>Creates a split with its branches and preferred fraction.</summary>
    /// <param name="key">The stable identity of the split.</param>
    /// <param name="axis">The axis of the split.</param>
    /// <param name="first">The branch at the logical start.</param>
    /// <param name="second">The branch at the logical end.</param>
    /// <param name="fraction">The preferred fraction of available space assigned to the first branch.</param>
    public DockSplit(string key, DockAxis axis, DockNode first, DockNode second, double fraction = 0.5) : base(key)
    {
        Axis = axis;
        First = first;
        Second = second;
        Fraction = fraction;
    }

    /// <summary>The axis of the split.</summary>
    public DockAxis Axis { get; init; }

    /// <summary>The branch at the logical start.</summary>
    public DockNode First { get; init; }

    /// <summary>The branch at the logical end.</summary>
    public DockNode Second { get; init; }

    /// <summary>The preferred fraction of available space assigned to the first branch.</summary>
    public double Fraction { get; init; }
}

/// <summary>A presentation entry whose key can appear in a dock group.</summary>
/// <param name="Key">A stable, non-empty key unique within the workspace catalog.</param>
/// <param name="Title">The visible title and accessible name of the tab.</param>
/// <param name="KeepAlive">Retain this item after the first activation until it leaves the workspace.</param>
/// <param name="CanClose">Whether the workspace offers a close action.</param>
public sealed record DockItem(string Key, string Title, bool KeepAlive = false, bool CanClose = true);

/// <summary>The kind of change proposed by the workspace.</summary>
public enum DockAction
{
    /// <summary>Select an item in its group.</summary>
    Activate,

    /// <summary>Remove the item from the layout.</summary>
    Close,

    /// <summary>Move the item within its group or into another group.</summary>
    Move,

    /// <summary>Create a new group beside a group and move an item into it.</summary>
    Split,

    /// <summary>Change the fraction of space assigned to the first branch.</summary>
    Resize
}

/// <summary>A workspace transition that the application can accept or reject.</summary>
/// <param name="Action">The operation that produced this proposal.</param>
/// <param name="ItemKey">The affected item, or null for a resize.</param>
/// <param name="TargetKey">The target group or split.</param>
/// <param name="Previous">The layout on which the proposal was based.</param>
/// <param name="Proposed">The proposed next layout.</param>
public sealed record DockLayoutChange(DockAction Action, string? ItemKey, string TargetKey, DockNode Previous, DockNode Proposed);
