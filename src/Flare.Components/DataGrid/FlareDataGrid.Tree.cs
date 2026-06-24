namespace Flare.Components;

// Tree-grid: expand/collapse state, depth calculation and hierarchical flattening of rows.
// Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- Tree-grid helpers ---------------------------------------------------

    private void ToggleExpand(TItem item)
    {
        if (_expandedItems.Contains(item))
            _expandedItems.Remove(item);
        else
            _expandedItems.Add(item);
    }

    private bool IsExpanded(TItem item) => _expandedItems.Contains(item);

    // Depth per visible item, captured once during FlattenTree so the render path is an O(1) lookup
    // instead of re-walking the whole tree for every row (which was O(rows * tree size) per render).
#pragma warning disable CS8714
    private readonly Dictionary<TItem, int> _treeDepths = new();
#pragma warning restore CS8714

    private string GetTreeIndentStyle(GridColumn<TItem> col, TItem item)
    {
        if (Tree is null || _gridColumns.Count == 0 || !ReferenceEquals(col, _gridColumns[0])) return "";
        if (!_treeDepths.TryGetValue(item, out var depth) || depth == 0) return "";
        var px = int.TryParse(Tree.IndentWidth.Replace("px", ""), out var w) ? w : 24;
        return $"padding-left:{depth * px}px";
    }

    /// <summary>Flattens hierarchical data into a list with depth information.</summary>
    private List<TreeNode<TItem>> FlattenTree(IEnumerable<TItem> items, int depth = 0)
    {
        if (depth == 0) _treeDepths.Clear();
        var result = new List<TreeNode<TItem>>();
        if (Tree is null)
        {
            foreach (var item in items)
                result.Add(new TreeNode<TItem> { Item = item, Depth = 0, HasChildren = false, IsExpanded = false });
            return result;
        }

        foreach (var item in items)
        {
            _treeDepths[item] = depth;
            var children = Tree.ChildrenSelector(item);
            var hasChildren = children?.Any() == true;
            var expanded = IsExpanded(item) || (Tree.InitiallyExpanded?.Invoke(item) == true);

            if (expanded && !_expandedItems.Contains(item) && Tree.InitiallyExpanded?.Invoke(item) == true)
                _expandedItems.Add(item);

            result.Add(new TreeNode<TItem>
            {
                Item = item,
                Depth = depth,
                HasChildren = hasChildren,
                IsExpanded = hasChildren && IsExpanded(item),
            });

            if (hasChildren && IsExpanded(item))
            {
                result.AddRange(FlattenTree(children!, depth + 1));
            }
        }
        return result;
    }
}
