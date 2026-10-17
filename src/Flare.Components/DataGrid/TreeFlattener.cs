namespace Flare.Components;

/// <summary>
/// Turns a hierarchy into the rows a tree-shaped view draws: every root, and under each expanded node its
/// children, in order, with their depth. Shared by the tree data grid and the Gantt panel, so a hierarchy is
/// walked one way everywhere.
/// </summary>
public static class TreeFlattener
{
    /// <summary>The visible rows of the hierarchy under <paramref name="roots"/>.</summary>
    /// <typeparam name="T">The caller's node type.</typeparam>
    /// <param name="roots">Top-level nodes, in order.</param>
    /// <param name="children">Child nodes of a node; null or empty for a leaf.</param>
    /// <param name="isExpanded">Whether a node with children shows them.</param>
    /// <param name="visit">
    /// Called for every node of the hierarchy, including the ones under a collapsed node, with the node's ancestors
    /// from the root down. Use it for what needs the whole tree, such as a summary of all descendants. When it is
    /// null, the children of a collapsed node are not walked at all.
    /// </param>
    public static List<TreeNode<T>> Flatten<T>(IEnumerable<T> roots, Func<T, IEnumerable<T>?> children,
        Func<T, bool> isExpanded, Action<T, IReadOnlyList<T>>? visit = null)
    {
        ArgumentNullException.ThrowIfNull(roots);
        ArgumentNullException.ThrowIfNull(children);
        ArgumentNullException.ThrowIfNull(isExpanded);

        var rows = new List<TreeNode<T>>();
        Walk(roots, 0, true, []);
        return rows;

        void Walk(IEnumerable<T> level, int depth, bool shown, List<T> ancestors)
        {
            foreach (var node in level)
            {
                visit?.Invoke(node, ancestors);
                var kids = children(node);
                var list = kids as IReadOnlyCollection<T> ?? kids?.ToList();
                var hasChildren = list is { Count: > 0 };
                var expanded = hasChildren && isExpanded(node);
                if (shown)
                    rows.Add(new TreeNode<T> { Item = node, Depth = depth, HasChildren = hasChildren, IsExpanded = expanded });
                if (hasChildren && (expanded && shown || visit is not null))
                {
                    ancestors.Add(node);
                    Walk(list!, depth + 1, shown && expanded, ancestors);
                    ancestors.RemoveAt(ancestors.Count - 1);
                }
            }
        }
    }
}
