using System.Collections.Immutable;

namespace Flare.Components.IDE;

/// <summary>A workspace layout node with a key unique within the workspace.</summary>
/// <param name="Key">The stable identity of the node.</param>
public abstract record DockNode(string Key);

/// <summary>A group of docked tabs with its own selection.</summary>
/// <param name="Key">The stable identity of the group.</param>
/// <param name="Items">Catalog keys in display order.</param>
/// <param name="ActiveKey">The selected item, or null for no selection.</param>
public sealed record DockGroup(string Key, ImmutableArray<string> Items, string? ActiveKey = null) : DockNode(Key);

/// <summary>The axis on which a split distributes space.</summary>
public enum DockAxis
{
    /// <summary>Place branches side by side.</summary>
    Horizontal,

    /// <summary>Place branches above and below one another.</summary>
    Vertical
}

/// <summary>Two branches separated by a resizable divider.</summary>
/// <param name="Key">The stable identity of the split.</param>
/// <param name="Axis">The axis of the split.</param>
/// <param name="First">The branch at the logical start.</param>
/// <param name="Second">The branch at the logical end.</param>
/// <param name="Fraction">The preferred fraction of available space assigned to the first branch.</param>
public sealed record DockSplit(string Key, DockAxis Axis, DockNode First, DockNode Second, double Fraction = 0.5) : DockNode(Key);

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
