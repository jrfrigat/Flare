namespace Flare.Components;

// Row grouping (Phase B): builds nested collapsible group lines, per-group aggregates and the
// server group-by key contract. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- Grouping (Phase B) ---------------------------------------------------
    // Separator used to build a stable, collision-resistant group path across nesting levels.
    private const string GroupPathSeparator = "";

    // Flatten the current page items into render lines: nested group headers (respecting the
    // collapsed set) followed by the data rows that belong to each leaf group.
    private void BuildGroupedLines()
    {
        _groupedLines = [];
        var groups = _effectiveGroups;
        if (groups.Count == 0) return;
        AddGroupLevel(_pageItems.ToList(), 0, "");
    }

    private void AddGroupLevel(List<TItem> items, int level, string parentPath)
    {
        var groups = _effectiveGroups;
        var group = groups[level];
        foreach (var grp in items.GroupBy(group.Selector))
        {
            var keyText = grp.Key?.ToString() ?? string.Empty;
            var path = parentPath.Length == 0 ? keyText : parentPath + GroupPathSeparator + keyText;
            var members = grp.ToList();
            var display = $"{group.Key}: {keyText}";
            _groupedLines.Add(new GroupLine(path, display, level, members.Count, ComputeGroupAggs(members, group), group.HeaderTemplate));

            if (_collapsedGroups.Contains(path)) continue;

            if (level + 1 < groups.Count)
                AddGroupLevel(members, level + 1, path);
            else
                foreach (var item in members)
                    _groupedLines.Add(item!);
        }
    }

    // Per-level aggregates take precedence over the grid-level Aggregates when set.
    private IReadOnlyList<(string Title, string Value)> ComputeGroupAggs(List<TItem> members, DataGridGroup<TItem> group)
    {
        var aggs = group.Aggregates ?? Aggregates;
        if (aggs is null || aggs.Count == 0) return [];
        return aggs
            .Select(a => (a.ColumnTitle, ComputeAggregate(a, members)))
            .ToList();
    }

    private void ToggleGroup(string path)
    {
        if (!_collapsedGroups.Remove(path))
            _collapsedGroups.Add(path);
    }

    // Sync the server-provider group-key list with the registered grouping levels.
    private void RebuildGroupKeys()
    {
        _groupKeys.Clear();
        _groupKeys.AddRange(_effectiveGroups.Select(g => g.Key));
    }

    /// <summary>Registers a grouping level from a <see cref="DataGridGroup{TItem}"/> child.</summary>
    internal void RegisterGroup(DataGridGroup<TItem> group)
    {
        if (_groupComponents.Contains(group)) return;
        _groupComponents.Add(group);
        RebuildGroupKeys();
        _sortedCache = null;
        // Groups self-register from the <Grouping> fragment while the grid's render is still in flight,
        // so request a re-render to pick them up (mirrors AddColumn).
        StateHasChanged();
    }

    /// <summary>Unregisters a grouping level when its <see cref="DataGridGroup{TItem}"/> is disposed.</summary>
    internal void UnregisterGroup(DataGridGroup<TItem> group)
    {
        if (!_groupComponents.Remove(group)) return;
        RebuildGroupKeys();
        _sortedCache = null;
        StateHasChanged();
    }

}
