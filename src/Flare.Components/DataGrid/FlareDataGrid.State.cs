using Flare.Components.Resources;
using Microsoft.JSInterop;

namespace Flare.Components;

// State snapshots and change events, the public imperative API (SortByAsync/FilterByAsync/...),
// the sort/filter pipeline entry points (Sorted/SortedUnpaged), header sort helpers, the aggregate
// footer and selection. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- State snapshot + change notifications (Phase A) ----------------------

    private List<DataGridSort> BuildSorts()
        => _sortStack.Select(s => new DataGridSort(s.Column.Key, s.Direction)).ToList();

    private List<DataGridFilter> BuildFilters()
    {
        // Flat view for back-compat: text filter-row entries (Contains), typed inline filters
        // (select/number/date) and a flattened list of the advanced tree's leaf conditions. The full
        // nested AND/OR structure is carried separately as FilterTree on the request/state.
        var list = _filters.Select(kv => new DataGridFilter(kv.Key, FilterOperator.Contains, kv.Value)).ToList();
        list.AddRange(_typedFilters.Values);
        if (_advancedTree is not null)
            FlattenConditions(_advancedTree, list);
        return list;
    }

    private static void FlattenConditions(DataGridFilterGroup g, List<DataGridFilter> into)
    {
        into.AddRange(g.Conditions);
        foreach (var sub in g.Groups) FlattenConditions(sub, into);
    }

    // -- Conditional formatting ------------------------------------------------

    private static string CombineStyle(string? baseStyle, string? extraStyle)
    {
        if (string.IsNullOrEmpty(baseStyle) && string.IsNullOrEmpty(extraStyle)) return "";
        if (string.IsNullOrEmpty(baseStyle)) return extraStyle!;
        if (string.IsNullOrEmpty(extraStyle)) return baseStyle!;
        return baseStyle.TrimEnd() + " " + extraStyle.TrimStart();
    }

    private DataGridState BuildState()
        => new(_page, _effectivePageSize, BuildSorts(), BuildFilters(), [.. _groupKeys])
        {
            FilterTree = _advancedTree,
            ColumnOrder = _gridColumns.Select(c => c.Key).ToList(),
        };

    /// <summary>Builds the new immutable state snapshot from current internal state.</summary>
    private DataGridState<TItem> BuildCurrentState()
    {
        var visibleCols = _gridColumns.Where(c => !_hiddenColumns.Contains(c.Key)).ToList();
        return new DataGridState<TItem>
        {
            Columns = _gridColumns,
            VisibleColumns = visibleCols,
            Sorts = BuildSorts(),
            Filters = new Dictionary<string, string>(_filters),
            TypedFilters = new Dictionary<string, DataGridFilter>(_typedFilters),
            AdvancedFilter = _advancedTree,
            Page = _page,
            PageSize = _effectivePageSize,
            TotalCount = _itemsCount ?? 0,
            SelectedItems = new HashSet<TItem>(_selection),
            GroupKeys = [.. _groupKeys],
            ColumnOrder = _gridColumns.Select(c => c.Key).ToList(),
            HiddenColumns = new HashSet<string>(_hiddenColumns),
            BatchEditingItems = new HashSet<TItem>(_batchEditingItems),
            QuickFilter = QuickFilter,
        };
    }

    /// <summary>Applies a sort command to the grid.</summary>
    public async Task SortByAsync(GridColumn<TItem> column, bool shift = false)
    {
        if (shift)
        {
            var existing = _sortStack.FindIndex(s => s.Column.Key == column.Key);
            if (existing >= 0)
            {
                var (col, dir) = _sortStack[existing];
                if (dir == SortDirection.Ascending)
                    _sortStack[existing] = (col, SortDirection.Descending);
                else
                    _sortStack.RemoveAt(existing);
            }
            else
            {
                _sortStack.Add((column, SortDirection.Ascending));
            }
        }
        else
        {
            if (_sortStack.Count == 1 && _sortStack[0].Column.Key == column.Key)
            {
                if (_sortStack[0].Direction == SortDirection.Ascending)
                {
                    _sortStack.Clear();
                    _sortStack.Add((column, SortDirection.Descending));
                }
                else
                    _sortStack.Clear();
            }
            else
            {
                _sortStack.Clear();
                _sortStack.Add((column, SortDirection.Ascending));
            }
        }

        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseSortChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    /// <summary>Applies a filter command to the grid.</summary>
    public async Task FilterByAsync(string columnKey, string value)
    {
        if (string.IsNullOrEmpty(value))
            _filters.Remove(columnKey);
        else
            _filters[columnKey] = value;

        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    /// <summary>Applies a global quick filter: keeps rows where any visible column's text contains
    /// <paramref name="text"/> (case-insensitive). Pass null or empty to clear. Combined (AND) with the
    /// <c>QuickFilter</c> parameter. Client-side only - used by <c>FlareDataGridQuickFilter</c>.</summary>
    public async Task ApplyQuickFilter(string? text)
    {
        var normalized = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        if (_quickFilterText == normalized) return;
        _quickFilterText = normalized;
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    /// <summary>Clears all filters.</summary>
    public async Task ClearAllFiltersAsync()
    {
        _filters.Clear();
        _typedFilters.Clear();
        _advancedTree = null;
        _quickFilterText = null;
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    /// <summary>Toggles selection for an item.</summary>
    public void ToggleSelection(TItem item)
    {
        if (_selection.Contains(item))
            _selection.Remove(item);
        else
            _selection.Add(item);
    }

    /// <summary>Selects all visible items.</summary>
    public void SelectAll()
    {
        foreach (var item in _pageItems)
            _selection.Add(item);
    }

    /// <summary>Deselects all items.</summary>
    public void DeselectAll()
    {
        _selection.Clear();
    }

    /// <summary>Persists current grid state to localStorage if PersistStateKey is set.</summary>
    private async Task SaveStateAsync()
    {
        if (_persistence is null || !_persistenceLoaded) return;

        var state = new DataGridPersistedState
        {
            Sorts = _sortStack.Select(s => new PersistedSort
            {
                Key = s.Column.Key,
                Direction = s.Direction.ToString(),
            }).ToList(),
            Filters = new Dictionary<string, string>(_filters),
            ColumnOrder = _gridColumns.Select(c => c.Key).ToList(),
            HiddenColumns = [.. _hiddenColumns],
            Page = _page,
            PageSize = _effectivePageSize,
        };

        await _persistence.SaveAsync(state);
    }

    // Screen-reader live-region text. Updated on sort/filter/page changes; rendered into an
    // aria-live="polite" status region so assistive tech announces what changed.
    private string _ariaAnnouncement = "";

    private void Announce(string text)
    {
        _ariaAnnouncement = text;
        StateHasChanged();
    }

    // Filtered result count for the announcement: the provider total in server mode, else the
    // freshly filtered/sorted client list size.
    private int CurrentResultCount() => _provider is not null ? _serverTotalCount : SortedUnpaged().Count;

    private async Task RaiseSortChangedAsync()
    {
        Announce(_sortStack.Count == 0
            ? FlareStrings.DataGrid_AnnounceSortCleared
            : string.Format(_sortStack[0].Direction == SortDirection.Ascending
                ? FlareStrings.DataGrid_AnnounceSortAsc
                : FlareStrings.DataGrid_AnnounceSortDesc, _sortStack[0].Column.Title));
        await OnSortChanged.InvokeAsync(BuildSorts());
        await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
    }

    private async Task RaiseFilterChangedAsync()
    {
        Announce(string.Format(FlareStrings.DataGrid_AnnounceFiltered, CurrentResultCount()));
        await OnFilterChanged.InvokeAsync(BuildFilters());
        await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
    }

    /// <summary>Applies an advanced filter tree from DataGridFilterBuilder.</summary>
    public async Task ApplyAdvancedFilter(DataGridFilterGroup filterTree)
    {
        _advancedTree = filterTree;
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    /// <summary>Clears all advanced filters.</summary>
    public async Task ClearAdvancedFilter()
    {
        _advancedTree = null;
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
    }

    private async Task RaisePageChangedAsync()
    {
        Announce(string.Format(FlareStrings.DataGrid_AnnouncePage, _page + 1, Math.Max(1, _totalPages)));
        await OnPageChanged.InvokeAsync(_page);
        await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
    }

    /// <summary>Invalidates the sorted cache and refreshes Virtualize if present.</summary>
    private async Task InvalidateAndRefreshAsync()
    {
        _sortedCache = null;
        _itemsCount = null;
        try
        {
            if (_virtualizeProviderRef is not null)
                await _virtualizeProviderRef.RefreshDataAsync();
            else if (_virtualizeClientRef is not null)
                await _virtualizeClientRef.RefreshDataAsync();
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        // Non-virtualized grids render rows from _pageItems, which Virtualize.RefreshDataAsync does
        // not touch - request a render so the recomputed rows reach the DOM.
        StateHasChanged();
    }

    private List<TItem> Sorted()
    {
        if (_sortedCache is not null) return _sortedCache;
        EnsureColumnsBuilt();
        var src = Items ?? [];

        // Tree mode: flatten hierarchical data
        if (_treeMode && Tree is not null)
        {
            var treeNodes = FlattenTree(src);
            src = treeNodes.Select(n => n.Item).ToList();
        }

        // Build DataGridFilter list from text filters + typed filters
        var filters = new List<DataGridFilter>();
        foreach (var (colTitle, filterVal) in _filters)
        {
            if (!string.IsNullOrEmpty(filterVal))
                filters.Add(new DataGridFilter(colTitle, FilterOperator.Contains, filterVal));
        }
        foreach (var (colTitle, typedFilter) in _typedFilters)
        {
            filters.Add(typedFilter);
        }

        // Convert _sortStack to DataGridSort list
        var sorts = _sortStack.Select(s => new DataGridSort(s.Column.Key, s.Direction)).ToList();

        // Use pipeline for single-enumeration execution
        var srcList = src is IList<TItem> iList ? iList.ToList() : src.ToList();
        var result = DataGridPipeline<TItem>.Execute(
            srcList,
            sorts, filters, _advancedTree, _effectiveQuickFilter,
            null, null, _page, _effectivePageSize, BuildColumnStrategies());

        _sortedCache = result.Items.ToList();
        if (_itemsCount is null)
            _itemsCount = result.TotalCount;

        return _sortedCache;
    }

    // Full sorted/filtered list with NO paging. Client-side virtualization renders its own window,
    // so paging here would hide every record past the first page. Cache-free (the paged _sortedCache
    // serves the non-virtual paths). Used by the banded-composite virtual path.
    private List<TItem> SortedUnpaged()
    {
        var src = Items ?? [];
        if (_treeMode && Tree is not null)
            src = FlattenTree(src).Select(n => n.Item).ToList();

        var filters = new List<DataGridFilter>();
        foreach (var (colTitle, filterVal) in _filters)
            if (!string.IsNullOrEmpty(filterVal))
                filters.Add(new DataGridFilter(colTitle, FilterOperator.Contains, filterVal));
        foreach (var (_, typedFilter) in _typedFilters)
            filters.Add(typedFilter);

        var sorts = _sortStack.Select(s => new DataGridSort(s.Column.Key, s.Direction)).ToList();
        var srcList = src is IList<TItem> iList ? iList.ToList() : src.ToList();
        var result = DataGridPipeline<TItem>.Execute(
            srcList, sorts, filters, _advancedTree, _effectiveQuickFilter,
            null, null, 0, int.MaxValue, BuildColumnStrategies());
        return result.Items.ToList();
    }

    private int _effectivePageSize => _currentPageSize > 0 ? _currentPageSize : (PageSize > 0 ? PageSize : 10);

    /// <summary>Current page size in effect (the user-selected size, else <c>PageSize</c>, else 10).
    /// Used by a bound <see cref="FlareDataGridPager{TItem}"/>.</summary>
    public int EffectivePageSize => _effectivePageSize;

    /// <summary>Total number of pages for the current data and page size. Used by a bound
    /// <see cref="FlareDataGridPager{TItem}"/>.</summary>
    public int PageCount => _totalPages;

    /// <summary>Zero-based index of the current page. Used by a bound
    /// <see cref="FlareDataGridPager{TItem}"/>.</summary>
    public int CurrentPageIndex => _page;

    /// <summary>True when client-side paging applies (i.e. not a virtualized or infinite-scroll grid,
    /// which scroll instead of paging). A bound <see cref="FlareDataGridPager{TItem}"/> renders nothing
    /// when this is false.</summary>
    public bool PagingEnabled => !_scrollContainer;

    /// <summary>Navigates to a zero-based page index (clamped to the valid range). Used by a bound
    /// <see cref="FlareDataGridPager{TItem}"/>.</summary>
    /// <param name="pageIndex">Zero-based target page index.</param>
    public Task GoToPageAsync(int pageIndex) => SetPage(pageIndex);

    /// <summary>Changes the page size and resets to the first page. Used by a bound
    /// <see cref="FlareDataGridPager{TItem}"/>.</summary>
    /// <param name="size">New page size (ignored when not positive).</param>
    public Task SetPageSizeAsync(int size) => OnPageSizeChanged(size);

    // Page count must divide the FULL filtered row count, not the current page slice. Sorted()
    // returns only the active page, so use the unpaged total (server count, or SortedUnpaged) - the
    // same source CurrentResultCount() uses.
    private int _totalPages
    {
        get
        {
            if (_effectivePageSize <= 0) return 1;
            var total = _provider is not null ? _serverTotalCount : SortedUnpaged().Count;
            return (int)Math.Ceiling(total / (double)_effectivePageSize);
        }
    }

    private IEnumerable<TItem> _pageItems => _provider is not null
        ? (_sortedCache ?? [])
        : SortedUnpaged().Skip(_page * _effectivePageSize).Take(_effectivePageSize);

    // Frozen sticky class for a body cell: left-frozen, right-frozen, or none.
    private static string _tdFrozenClass(GridColumn<TItem> col) =>
        col.Frozen ? Css.Classes.DataGrid.TdFrozen
        : col.FrozenRight ? Css.Classes.DataGrid.TdFrozenRight
        : "";

    private string _thClass(GridColumn<TItem> col) =>
        Css.Classes.DataGrid.Th +
        (col.Sortable ? " " + Css.Classes.DataGrid.ThSortable : "") +
        (col.Frozen ? " " + Css.Classes.DataGrid.ThFrozen : "") +
        (col.FrozenRight ? " " + Css.Classes.DataGrid.ThFrozenRight : "") +
        (AlignClass(col) is { Length: > 0 } a ? " " + a : "");

    private string? _ariaSort(GridColumn<TItem> col)
    {
        if (!col.Sortable) return null;
        var state = _sortStack.FirstOrDefault(s => s.Column.Key == col.Key);
        if (state.Column is null) return "none";
        return state.Direction == SortDirection.Ascending ? "ascending" : "descending";
    }

    private string ComputeAggregate(AggregateDefinition<TItem> agg, IList<TItem> src)
    {
        if (agg.Type == AggregateType.Count)
        {
            var result = (double)src.Count;
            return agg.Format is not null ? result.ToString(agg.Format) : result.ToString("N0");
        }
        if (agg.ValueSelector is null) return string.Empty;
        var values = src.Select(agg.ValueSelector).ToList();
        double computed = agg.Type switch
        {
            AggregateType.Sum => values.Sum(),
            AggregateType.Average => values.Count > 0 ? values.Average() : 0,
            AggregateType.Min => values.Count > 0 ? values.Min() : 0,
            AggregateType.Max => values.Count > 0 ? values.Max() : 0,
            _ => 0,
        };
        return agg.Format is not null ? computed.ToString(agg.Format) : computed.ToString("N2");
    }

    // Visible (non-hidden) columns in display order, for export/aggregates.
    private List<GridColumn<TItem>> _visibleColumns => _gridColumns.Where(c => !_hiddenColumns.Contains(c.Key)).ToList();

    // Runs a pluggable exporter against the current visible columns + sorted rows.
    private async Task OnPageSizeChanged(int size)
    {
        if (size <= 0) return;
        _currentPageSize = size;
        if (_provider is null) _sortedCache = null;
        await SetPage(0);
    }

    private async Task HandleRowClickAsync(TItem item)
    {
        if (SelectionMode == SelectionMode.None)
        {
            await RowClick.InvokeAsync(item);
            return;
        }
        if (SelectionMode == SelectionMode.Single)
        {
            _selection = _selection.Contains(item) ? [] : [item];
        }
        else
        {
            if (!_selection.Add(item)) _selection.Remove(item);
        }
        await SelectedItemsChanged.InvokeAsync(_selection);
        await RowClick.InvokeAsync(item);
    }

    private async Task ToggleSelectAll()
    {
        var pageList = _pageItems.ToList();
        if (pageList.All(i => _selection.Contains(i)))
            foreach (var item in pageList) _selection.Remove(item);
        else
            foreach (var item in pageList) _selection.Add(item);
        await SelectedItemsChanged.InvokeAsync(_selection);
    }
}
