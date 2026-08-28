using Microsoft.AspNetCore.Components;

namespace Flare.Components;

// The external control surface: the DataGridContext handle, the internal read views it projects, and
// the commands that exist so an outside component never has to reach into grid internals to drive it.
public partial class FlareDataGrid<TItem>
{
    private readonly DataGridContext<TItem> _ownContext = new();
    private DataGridContext<TItem>? _attachedContext;

    /// <summary>
    /// An externally-owned handle for driving this grid. Declare a <see cref="DataGridContext{TItem}"/>
    /// in the page and pass it here to put pagers, column pickers, query editors or any custom control
    /// anywhere in the layout, bound to this grid. When omitted the grid uses one of its own, which
    /// children reach through the cascade.
    /// </summary>
    [Parameter] public DataGridContext<TItem>? Context { get; set; }

    /// <summary>The context actually in force: <see cref="Context"/> when one was supplied, otherwise
    /// the grid's own. Never null, and stable for as long as <see cref="Context"/> is.</summary>
    public DataGridContext<TItem> ActiveContext => Context ?? _ownContext;

    // Attach on first use and whenever the supplied context is swapped, so a control bound to the new
    // instance starts seeing this grid and one bound to the old instance stops.
    private void SyncContext()
    {
        var active = ActiveContext;
        if (ReferenceEquals(_attachedContext, active)) return;
        _attachedContext?.Detach(this);
        active.Attach(this);
        _attachedContext = active;
    }

    internal void NotifyContext(DataGridChange change) => _attachedContext?.Raise(change);

    // -- Read views -----------------------------------------------------------
    // Projections handed to DataGridContext. Those that map onto a live internal collection are handed
    // over directly rather than copied: a control reads them on every render, and the context's contract
    // is that only Snapshot() materializes.

    internal IReadOnlyList<GridColumn<TItem>> ColumnsView
    {
        get { EnsureColumnsBuilt(); return _gridColumns; }
    }

    internal IReadOnlyList<GridColumn<TItem>> VisibleColumnsView => _visibleColumns;

    internal IReadOnlyList<DataGridSort> SortsView => BuildSorts();

    internal IReadOnlyDictionary<string, string> TextFiltersView => _filters;

    internal IReadOnlyDictionary<string, DataGridFilter> TypedFiltersView => _typedFilters;

    internal DataGridFilterGroup? AdvancedFilterView => _advancedTree;

    internal string? QuickFilterTextView => _quickFilterText;

    internal IReadOnlySet<TItem> SelectionView => _selection;

    internal IReadOnlyList<string> GroupKeysView => _groupKeys;

    internal int FilteredRowCount => CurrentResultCount();

    // -- Commands -------------------------------------------------------------

    /// <summary>Replaces the sort stack in one step. Keys that match no column are dropped; a
    /// <see cref="SortDirection.None"/> entry removes that column from the stack.</summary>
    /// <param name="sorts">The sorts to apply, outermost first.</param>
    public async Task SetSortsAsync(IReadOnlyList<DataGridSort> sorts)
    {
        EnsureColumnsBuilt();
        _sortStack.Clear();
        foreach (var s in sorts)
        {
            if (s.Direction == SortDirection.None) continue;
            var col = _gridColumns.FirstOrDefault(c => c.Key == s.Key);
            if (col is not null) _sortStack.Add((col, s.Direction));
        }
        await AfterQueryChangedAsync(DataGridChange.Sort, RaiseSortChangedAsync);
    }

    /// <summary>Sets or clears a column's structured filter (the shape the column filter menus produce).</summary>
    /// <param name="columnKey">Key of the column to filter.</param>
    /// <param name="filter">The filter to apply; null clears the column's structured filter.</param>
    public async Task SetTypedFilterAsync(string columnKey, DataGridFilter? filter)
    {
        if (filter is null)
        {
            if (!_typedFilters.Remove(columnKey)) return;
        }
        else
        {
            _typedFilters[columnKey] = filter;
        }
        await AfterQueryChangedAsync(DataGridChange.Filter, RaiseFilterChangedAsync);
    }

    /// <summary>Shows or hides one column by its stable key.</summary>
    /// <param name="columnKey">Key of the column.</param>
    /// <param name="visible">True to show it, false to hide it.</param>
    public Task SetColumnVisibleAsync(string columnKey, bool visible)
    {
        if (_hiddenColumns.Contains(columnKey) != visible) return Task.CompletedTask;
        return ToggleColumnVisibility(columnKey);
    }

    /// <summary>Replaces the column display order. Columns whose keys are not listed keep their relative
    /// order after the listed ones.</summary>
    /// <param name="columnKeys">Column keys in the wanted order.</param>
    public async Task SetColumnOrderAsync(IReadOnlyList<string> columnKeys)
    {
        _columnOrder.Clear();
        _columnOrder.AddRange(columnKeys);
        RebuildGridColumns();
        await RaiseColumnOrderChangedAsync();
    }

    /// <summary>Moves one column so that it sits immediately before another.</summary>
    /// <param name="columnKey">Key of the column to move.</param>
    /// <param name="beforeKey">Key of the column it should precede; null moves it to the end.</param>
    public async Task MoveColumnAsync(string columnKey, string? beforeKey)
    {
        if (columnKey == beforeKey) return;
        EnsureColumnsBuilt();
        var order = _gridColumns.Select(c => c.Key).ToList();
        if (!order.Remove(columnKey)) return;
        var idx = beforeKey is null ? -1 : order.IndexOf(beforeKey);
        order.Insert(idx < 0 ? order.Count : idx, columnKey);
        await SetColumnOrderAsync(order);
    }

    /// <summary>Selects or deselects one row, honoring the grid's selection mode.</summary>
    /// <param name="item">The row.</param>
    /// <param name="selected">True to select it, false to deselect it.</param>
    public async Task SetRowSelectedAsync(TItem item, bool selected)
    {
        if (SelectionMode == SelectionMode.None) return;
        if (selected && SelectionMode == SelectionMode.Single)
            _selection = [item];
        else if (selected)
        {
            if (!_selection.Add(item)) return;
        }
        else if (!_selection.Remove(item)) return;
        await RaiseSelectionChangedAsync();
    }

    /// <summary>Replaces the selection.</summary>
    /// <param name="items">The rows to select; an empty sequence clears the selection.</param>
    public async Task SetSelectionAsync(IEnumerable<TItem> items)
    {
        _selection = [.. items];
        await RaiseSelectionChangedAsync();
    }

    /// <summary>Selects every row on the current page.</summary>
    public async Task SelectAllAsync()
    {
        foreach (var item in _pageItems) _selection.Add(item);
        await RaiseSelectionChangedAsync();
    }

    /// <summary>Clears sorts, filters and paging, returning the grid to the query it started with.</summary>
    public async Task ResetQueryAsync()
    {
        _sortStack.Clear();
        _filters.Clear();
        _typedFilters.Clear();
        _advancedTree = null;
        _quickFilterText = null;
        await AfterQueryChangedAsync(DataGridChange.Sort | DataGridChange.Filter, async () =>
        {
            await RaiseSortChangedAsync();
            await RaiseFilterChangedAsync();
        });
    }

    /// <summary>Re-runs the query: reloads the page from the items provider, or re-filters and re-sorts
    /// the local <c>Items</c>.</summary>
    public async Task RefreshAsync()
    {
        _sortedCache = null;
        _itemsCount = null;
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
        NotifyContext(DataGridChange.Data);
    }

    // Shared tail of every query-shaping command: drop the caches, return to the first page, announce
    // the change and re-run the query the way this grid's data source requires.
    private async Task AfterQueryChangedAsync(DataGridChange change, Func<Task> raise)
    {
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await raise();
        NotifyContext(change | DataGridChange.Page);
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    private async Task RaiseSelectionChangedAsync()
    {
        await SelectedItemsChanged.InvokeAsync(_selection);
        NotifyContext(DataGridChange.Selection);
        StateHasChanged();
    }

    private async Task RaiseColumnOrderChangedAsync()
    {
        var order = _gridColumns.Select(c => c.Key).ToList();
        if (OnColumnOrderChanged.HasDelegate) await OnColumnOrderChanged.InvokeAsync(order);
        if (OnStateChanged.HasDelegate) await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
        NotifyContext(DataGridChange.Columns);
        StateHasChanged();
    }
}
