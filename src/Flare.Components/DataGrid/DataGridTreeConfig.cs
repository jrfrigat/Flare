namespace Flare.Components;

/// <summary>
/// Configuration for tree-grid (hierarchical data) support.
/// </summary>
public sealed class DataGridTreeConfig<T>
{
    /// <summary>Selector that returns child items for a given row. Null means no children (leaf node).</summary>
    public required Func<T, IEnumerable<T>?> ChildrenSelector { get; init; }

    /// <summary>Selector that determines if a row should be initially expanded.</summary>
    public Func<T, bool>? InitiallyExpanded { get; init; }

    /// <summary>CSS width for each level of indentation.</summary>
    public string IndentWidth { get; init; } = "24px";

    /// <summary>Column title to render the expand/collapse toggle in. If null, uses the first column.</summary>
    public string? ToggleColumnTitle { get; init; }

    /// <summary>Icon for collapsed state (Material Symbols name).</summary>
    public string CollapsedIcon { get; init; } = "chevron_right";

    /// <summary>Icon for expanded state (Material Symbols name).</summary>
    public string ExpandedIcon { get; init; } = "expand_more";
}

/// <summary>
/// Represents a flattened tree node with depth information.
/// </summary>
public sealed class TreeNode<T>
{
    /// <summary>The original data item.</summary>
    public required T Item { get; init; }

    /// <summary>Nesting depth (0 = root).</summary>
    public required int Depth { get; init; }

    /// <summary>Whether this node has children.</summary>
    public required bool HasChildren { get; init; }

    /// <summary>Whether this node is currently expanded.</summary>
    public required bool IsExpanded { get; init; }

    /// <summary>Whether this node is a leaf (no children or collapsed).</summary>
    public bool IsLeaf => !HasChildren || !IsExpanded;
}
