namespace Flare.Components;

// Drag-and-drop reordering of columns (by header) and rows. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- Column reorder (header drag-and-drop) -------------------------------
    // Columns are identified by their stable Key (Id/SortKey/Title), so reorder survives
    // duplicated or localized titles.
    private void OnColumnDragStart(string key)
    {
        _dragColumn = key;
    }

    private void OnColumnDragEnter(string key) { /* visual hover handled via CSS :hover */ }

    private void OnColumnDragEnd() => _dragColumn = null;

    private async Task OnColumnDrop(string targetKey)
    {
        if (_dragColumn is null || _dragColumn == targetKey)
        {
            _dragColumn = null;
            return;
        }

        // Build the order from the current display order, then move the dragged column
        // to just before the drop target.
        var order = _gridColumns.Select(c => c.Key).ToList();
        order.Remove(_dragColumn);
        var idx = order.IndexOf(targetKey);
        if (idx < 0) idx = order.Count;
        order.Insert(idx, _dragColumn);

        _columnOrder.Clear();
        _columnOrder.AddRange(order);
        _dragColumn = null;
        RebuildGridColumns();

        await OnColumnOrderChanged.InvokeAsync(order);
        await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
        StateHasChanged();
    }

    // -- Row reorder (drag-and-drop) -----------------------------------------
    private TItem? _dragRow;

    private void OnRowDragStart(TItem item) => _dragRow = item;
    private void OnRowDragEnd() => _dragRow = default;

    private async Task OnRowDrop(TItem target)
    {
        var dragged = _dragRow;
        _dragRow = default;
        if (dragged is null || EqualityComparer<TItem>.Default.Equals(dragged, target)) return;

        var rows = _pageItems.ToList();
        var oldIndex = rows.FindIndex(r => EqualityComparer<TItem>.Default.Equals(r, dragged));
        var newIndex = rows.FindIndex(r => EqualityComparer<TItem>.Default.Equals(r, target));
        if (oldIndex < 0 || newIndex < 0) return;

        await OnRowReordered.InvokeAsync(new DataGridRowReorder<TItem>(dragged, target, oldIndex, newIndex));
        StateHasChanged();
    }
}
