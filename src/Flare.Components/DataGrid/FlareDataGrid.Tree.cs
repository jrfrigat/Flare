namespace Flare.Components;

// Tree-grid: expand/collapse state, depth calculation and hierarchical flattening of rows.
public partial class FlareDataGrid<TItem>
{
    // Expansion is kept by key - Tree.KeySelector, or the item itself - so a controlled set and a
    // recreated item can name the same row.
    private readonly HashSet<object?> _expandedKeys = new();
    private readonly HashSet<object?> _seededKeys = new();
    private readonly Dictionary<object, TItem> _itemsByKey = new();

    private object? KeyOf(TItem item) => Tree?.KeySelector?.Invoke(item) ?? item;

    // Mirrors a controlled ExpandedItems into the key set. An equal set is left alone, so a parent
    // that passes the same collection on every render does not overwrite a click it has not seen yet.
    private void SyncExpandedItems()
    {
        if (ExpandedItems is null) return;
        var keys = new HashSet<object?>();
        foreach (var item in ExpandedItems)
        {
            var key = KeyOf(item);
            keys.Add(key);
            if (key is not null) _itemsByKey[key] = item;
        }

        if (keys.SetEquals(_expandedKeys)) return;
        _expandedKeys.Clear();
        foreach (var key in keys) _expandedKeys.Add(key);
        // Drop instances no longer named by the controlled set; the map is only a key-to-row view.
        foreach (var stale in _itemsByKey.Keys.Where(k => !_expandedKeys.Contains(k)).ToList())
            _itemsByKey.Remove(stale);
    }

    private async Task ToggleExpandAsync(TItem item)
    {
        if (Tree is null) return;

        var key = KeyOf(item);
        var expanded = !_expandedKeys.Contains(key);
        if (expanded)
            _expandedKeys.Add(key);
        else
            _expandedKeys.Remove(key);
        if (key is not null) _itemsByKey[key] = item;

        // FlattenTree depends on expansion, and both projections memoize its result.
        InvalidateData();
        StateHasChanged();

        await ExpandedItemsChanged.InvokeAsync(CurrentExpandedItems());
        await OnExpandedChanged.InvokeAsync(new DataGridTreeExpandChanged<TItem>(item, expanded));
    }

    private HashSet<TItem> CurrentExpandedItems()
    {
        var items = new HashSet<TItem>();
        foreach (var key in _expandedKeys)
            if (key is not null && _itemsByKey.TryGetValue(key, out var item))
                items.Add(item);
        return items;
    }

    private bool IsExpanded(TItem item) => _expandedKeys.Contains(KeyOf(item));

    // A hierarchical sort orders each level before the flatten, so a child never leaves its parent.
    private bool _hierarchicalTreeSort => _treeMode && Tree is { SortMode: DataGridTreeSortMode.Hierarchical };

    private Comparison<TItem>? BuildTreeComparison()
    {
        EnsureColumnsBuilt();
        var sorts = _sortStack.Select(s => new DataGridSort(s.Column.Key, s.Direction)).ToList();
        return DataGridPipeline<TItem>.BuildComparison(sorts, BuildColumnStrategies());
    }

    // Depth per visible item, captured once during FlattenTree so the render path is an O(1) lookup
    // instead of re-walking the whole tree for every row (which was O(rows * tree size) per render).
#pragma warning disable CS8714
    private readonly Dictionary<TItem, int> _treeDepths = new();
#pragma warning restore CS8714

    private string GetTreeIndentStyle(DataGridColumn<TItem> col, TItem item)
    {
        if (Tree is null || _gridColumns.Count == 0 || !ReferenceEquals(col, _gridColumns[0])) return "";
        if (!_treeDepths.TryGetValue(item, out var depth) || depth == 0) return "";
        var px = int.TryParse(Tree.IndentWidth.Replace("px", ""), out var w) ? w : 24;
        return $"padding-left:{depth * px}px";
    }

    private int TreeDepthOf(TItem item) => _treeDepths.TryGetValue(item, out var depth) ? depth : 0;

    private bool HasTreeChildren(TItem item) => Tree?.ChildrenSelector(item)?.Any() == true;

    // The parent of a visible row: the nearest earlier row one level up. Null at the root level.
    private TItem? TreeParentOf(IList<TItem> rows, int index)
    {
        var depth = TreeDepthOf(rows[index]);
        if (depth == 0) return default;
        for (var i = index - 1; i >= 0; i--)
            if (TreeDepthOf(rows[i]) == depth - 1)
                return rows[i];
        return default;
    }

    // Whether the visible row at index sits under the given ancestor. The walk is bounded by the row
    // count so a malformed depth map can never loop.
    private bool TreeIsDescendantOf(IList<TItem> rows, TItem ancestor, int index)
    {
        var current = index;
        for (var guard = 0; guard < rows.Count; guard++)
        {
            if (TreeParentOf(rows, current) is not { } parent) return false;
            if (EqualityComparer<TItem>.Default.Equals(parent, ancestor)) return true;
            current = IndexOf(rows, parent);
            if (current < 0) return false;
        }
        return false;
    }

    // Position and count of a row among its visible siblings, for the keyboard announcement.
    private (int Position, int Count) TreeSiblingPosition(IList<TItem> rows, int index)
    {
        var depth = TreeDepthOf(rows[index]);
        var start = index;
        while (start > 0 && TreeDepthOf(rows[start - 1]) >= depth) start--;
        var end = index;
        while (end + 1 < rows.Count && TreeDepthOf(rows[end + 1]) >= depth) end++;

        var position = 0;
        var count = 0;
        for (var i = start; i <= end; i++)
        {
            if (TreeDepthOf(rows[i]) != depth) continue;
            count++;
            if (i == index) position = count;
        }
        return (position, count);
    }

    /// <summary>Flattens hierarchical data into a list with depth information.</summary>
    private List<TreeNode<TItem>> FlattenTree(IEnumerable<TItem> items)
    {
        _treeDepths.Clear();
        if (Tree is null)
            return items.Select(item => new TreeNode<TItem> { Item = item, Depth = 0, HasChildren = false, IsExpanded = false }).ToList();

        var comparison = _hierarchicalTreeSort ? BuildTreeComparison() : null;
        var comparer = comparison is null ? null : Comparer<TItem>.Create(comparison);
        var roots = comparer is null ? items : items.OrderBy(x => x, comparer);
        Func<TItem, IEnumerable<TItem>?> children = Tree.ChildrenSelector;
        if (comparer is not null)
        {
            var selector = Tree.ChildrenSelector;
            children = item =>
            {
                var level = selector(item);
                return level is null ? null : level.OrderBy(x => x, comparer);
            };
        }

        var rows = TreeFlattener.Flatten(roots, children, item =>
        {
            var key = KeyOf(item);
            // InitiallyExpanded seeds a key once, so collapsing a seeded row sticks. A controlled
            // ExpandedItems set is the only source of truth and is never seeded.
            if (ExpandedItems is null && _seededKeys.Add(key) && Tree.InitiallyExpanded?.Invoke(item) == true)
                _expandedKeys.Add(key);
            return _expandedKeys.Contains(key);
        });
        foreach (var row in rows)
        {
            _treeDepths[row.Item] = row.Depth;
            var key = KeyOf(row.Item);
            // Only an expanded row's instance is ever needed again; keeping every visible row would
            // hold the whole data set in memory for a page-controlled grid.
            if (key is not null && _expandedKeys.Contains(key)) _itemsByKey[key] = row.Item;
        }
        return rows;
    }
}
