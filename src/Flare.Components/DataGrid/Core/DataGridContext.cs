namespace Flare.Components;

/// <summary>
/// The outside world's handle on a <see cref="FlareDataGrid{TItem}"/>: everything needed to read the
/// grid's state, change it, and be told when it changed - without deriving from the grid, modifying it,
/// or being rendered inside it.
/// </summary>
/// <remarks>
/// <para>
/// A grid always has a context. Declare one and pass it as <c>Context</c> to place controls anywhere on
/// the page (<c>&lt;FlareDataGrid Context="@_ctx" ...&gt;</c>); omit it and the grid creates its own,
/// which children reach through the cascade.
/// </para>
/// <para>
/// <see cref="Changed"/> carries a <see cref="DataGridChange"/> bit mask, not a state snapshot, so a
/// notification allocates nothing and a subscriber can ignore the kinds it does not display. Read what
/// you need from this object's properties afterwards; <see cref="Snapshot"/> is the one call that
/// materializes everything at once.
/// </para>
/// <para>
/// Derive a control from <see cref="FlareDataGridControl{TItem}"/> to get the resolution, subscription
/// and re-render plumbing for free.
/// </para>
/// </remarks>
/// <typeparam name="TItem">Row type of the grid this context drives.</typeparam>
public sealed class DataGridContext<TItem>
{
    private FlareDataGrid<TItem>? _grid;

    /// <summary>
    /// Raised after the grid's state changed, with a mask of what changed. Handlers run on the grid's
    /// dispatcher; a component handler should marshal its re-render through
    /// <c>InvokeAsync(StateHasChanged)</c>.
    /// </summary>
    public event Action<DataGridChange>? Changed;

    /// <summary>True once a grid is bound to this context. Commands are no-ops and reads return empty
    /// values until then, which is the state a control sees when it renders before its grid does.</summary>
    public bool IsAttached => _grid is not null;

    /// <summary>The bound grid, or null before one attaches. Prefer the members of this context; the
    /// grid itself is exposed for the rare case that needs something the context does not carry.</summary>
    public FlareDataGrid<TItem>? Grid => _grid;

    // -- Columns --------------------------------------------------------------

    /// <summary>Every registered column, in display order (user reordering included).</summary>
    public IReadOnlyList<DataGridColumn<TItem>> Columns => _grid?.ColumnsView ?? [];

    /// <summary>The columns currently shown, in display order.</summary>
    public IReadOnlyList<DataGridColumn<TItem>> VisibleColumns => _grid?.VisibleColumnsView ?? [];

    /// <summary>Keys of the columns hidden by the picker or by the grid's <c>HiddenColumns</c>.</summary>
    public IReadOnlyCollection<string> HiddenColumnKeys => _grid?.HiddenColumnKeys ?? [];

    /// <summary>Columns a picker should offer, as (Key, Title) pairs. Composite layout columns, which
    /// have no single identity to show or hide, are left out.</summary>
    public IReadOnlyList<(string Key, string Title)> PickableColumns => _grid?.PickableColumns ?? [];

    /// <summary>Columns a filter editor should offer, as (Key, Title) pairs.</summary>
    public IReadOnlyList<(string Key, string Title)> FilterableColumns => _grid?.FilterableColumns ?? [];

    /// <summary>Column keys in display order.</summary>
    public IReadOnlyList<string> ColumnOrder =>
        _grid is { } g ? [.. g.ColumnsView.Select(c => c.Key)] : [];

    // -- Sorting --------------------------------------------------------------

    /// <summary>Active sorts, outermost first. Allocates a small list per read.</summary>
    public IReadOnlyList<DataGridSort> Sorts => _grid?.SortsView ?? [];

    // -- Filtering ------------------------------------------------------------

    /// <summary>Plain text filters from the filter row, keyed by column key.</summary>
    public IReadOnlyDictionary<string, string> Filters => _grid?.TextFiltersView ?? EmptyFilters;

    /// <summary>Structured filters from the column filter menus, keyed by column key.</summary>
    public IReadOnlyDictionary<string, DataGridFilter> TypedFilters => _grid?.TypedFiltersView ?? EmptyTyped;

    /// <summary>The advanced nested filter tree, or null when none is applied.</summary>
    public DataGridFilterGroup? AdvancedFilter => _grid?.AdvancedFilterView;

    /// <summary>The global quick-search text, or null when no quick search is active.</summary>
    public string? QuickFilterText => _grid?.QuickFilterTextView;

    // -- Paging ---------------------------------------------------------------

    /// <summary>Zero-based index of the current page.</summary>
    public int Page => _grid?.CurrentPageIndex ?? 0;

    /// <summary>Rows per page currently in effect.</summary>
    public int PageSize => _grid?.EffectivePageSize ?? 0;

    /// <summary>Number of pages for the current filter and page size.</summary>
    public int PageCount => _grid?.PageCount ?? 0;

    /// <summary>Rows matching the current filters, across all pages.</summary>
    public int FilteredCount => _grid?.FilteredRowCount ?? 0;

    /// <summary>False for a virtualized or infinite-scroll grid, which scrolls instead of paging.</summary>
    public bool PagingEnabled => _grid?.PagingEnabled ?? false;

    /// <summary>The page-size choices the grid offers.</summary>
    public IReadOnlyList<int> RowsPerPageOptions => _grid?.RowsPerPageOptions ?? [];

    // -- Selection and grouping ----------------------------------------------

    /// <summary>The selected rows.</summary>
    public IReadOnlySet<TItem> SelectedItems => _grid?.SelectionView ?? EmptySelection;

    /// <summary>Group-by keys, outermost first.</summary>
    public IReadOnlyList<string> GroupKeys => _grid?.GroupKeysView ?? [];

    // -- Commands -------------------------------------------------------------

    /// <summary>Sorts by a column key. Ascending, then descending, then unsorted on repeat.</summary>
    /// <param name="columnKey">Key of the column to sort (<see cref="DataGridColumn{TItem}.Key"/>).</param>
    /// <param name="additive">True to add to the sort stack instead of replacing it (multi-sort).</param>
    public Task SortByAsync(string columnKey, bool additive = false)
    {
        if (_grid is null) return Task.CompletedTask;
        var col = _grid.ColumnsView.FirstOrDefault(c => c.Key == columnKey);
        return col is null ? Task.CompletedTask : _grid.SortByAsync(col, additive);
    }

    /// <summary>Replaces the whole sort stack. Unknown keys are ignored.</summary>
    /// <param name="sorts">The sorts to apply, outermost first.</param>
    public Task SetSortsAsync(IReadOnlyList<DataGridSort> sorts) =>
        _grid?.SetSortsAsync(sorts) ?? Task.CompletedTask;

    /// <summary>Removes every sort.</summary>
    public Task ClearSortsAsync() => _grid?.SetSortsAsync([]) ?? Task.CompletedTask;

    /// <summary>Sets or clears a column's text filter.</summary>
    /// <param name="columnKey">Key of the column to filter.</param>
    /// <param name="value">The filter text; null or empty clears it.</param>
    public Task FilterAsync(string columnKey, string? value) =>
        _grid?.FilterByAsync(columnKey, value ?? "") ?? Task.CompletedTask;

    /// <summary>Sets or clears a column's structured filter.</summary>
    /// <param name="columnKey">Key of the column to filter.</param>
    /// <param name="filter">The filter to apply; null clears the column's structured filter.</param>
    public Task SetTypedFilterAsync(string columnKey, DataGridFilter? filter) =>
        _grid?.SetTypedFilterAsync(columnKey, filter) ?? Task.CompletedTask;

    /// <summary>Sets the global quick-search text. Null or blank clears it.</summary>
    /// <param name="text">Text matched against every visible column, case-insensitively.</param>
    public Task SetQuickFilterAsync(string? text) =>
        _grid?.ApplyQuickFilter(text) ?? Task.CompletedTask;

    /// <summary>Applies or clears the advanced filter tree.</summary>
    /// <param name="filter">The tree to apply; null clears it.</param>
    public Task ApplyAdvancedFilterAsync(DataGridFilterGroup? filter) =>
        _grid is null ? Task.CompletedTask
        : filter is null ? _grid.ClearAdvancedFilter()
        : _grid.ApplyAdvancedFilter(filter);

    /// <summary>Clears every filter: text, structured, advanced and quick search.</summary>
    public Task ClearFiltersAsync() => _grid?.ClearAllFiltersAsync() ?? Task.CompletedTask;

    /// <summary>Navigates to a zero-based page index, clamped to the valid range.</summary>
    /// <param name="page">Target page index.</param>
    public Task GoToPageAsync(int page) => _grid?.GoToPageAsync(page) ?? Task.CompletedTask;

    /// <summary>Moves one page forward.</summary>
    public Task NextPageAsync() => GoToPageAsync(Page + 1);

    /// <summary>Moves one page back.</summary>
    public Task PreviousPageAsync() => GoToPageAsync(Page - 1);

    /// <summary>Changes the page size and returns to the first page.</summary>
    /// <param name="size">New rows-per-page value.</param>
    public Task SetPageSizeAsync(int size) => _grid?.SetPageSizeAsync(size) ?? Task.CompletedTask;

    /// <summary>Shows or hides one column.</summary>
    /// <param name="columnKey">Key of the column.</param>
    /// <param name="visible">True to show it, false to hide it.</param>
    public Task SetColumnVisibleAsync(string columnKey, bool visible) =>
        _grid?.SetColumnVisibleAsync(columnKey, visible) ?? Task.CompletedTask;

    /// <summary>Flips one column's visibility.</summary>
    /// <param name="columnKey">Key of the column.</param>
    public Task ToggleColumnAsync(string columnKey) =>
        _grid?.ToggleColumnAsync(columnKey) ?? Task.CompletedTask;

    /// <summary>Replaces the column display order. Keys left out keep their relative order after the
    /// listed ones.</summary>
    /// <param name="columnKeys">Column keys in the wanted order.</param>
    public Task SetColumnOrderAsync(IReadOnlyList<string> columnKeys) =>
        _grid?.SetColumnOrderAsync(columnKeys) ?? Task.CompletedTask;

    /// <summary>Moves one column to sit just before another.</summary>
    /// <param name="columnKey">Key of the column to move.</param>
    /// <param name="beforeKey">Key of the column it should precede; null moves it to the end.</param>
    public Task MoveColumnAsync(string columnKey, string? beforeKey) =>
        _grid?.MoveColumnAsync(columnKey, beforeKey) ?? Task.CompletedTask;

    /// <summary>Selects or deselects one row.</summary>
    /// <param name="item">The row.</param>
    /// <param name="selected">True to select it, false to deselect it.</param>
    public Task SetSelectedAsync(TItem item, bool selected) =>
        _grid?.SetRowSelectedAsync(item, selected) ?? Task.CompletedTask;

    /// <summary>Replaces the selection.</summary>
    /// <param name="items">The rows to select.</param>
    public Task SetSelectionAsync(IEnumerable<TItem> items) =>
        _grid?.SetSelectionAsync(items) ?? Task.CompletedTask;

    /// <summary>Selects every row on the current page.</summary>
    public Task SelectAllAsync() => _grid?.SelectAllAsync() ?? Task.CompletedTask;

    /// <summary>Clears the selection.</summary>
    public Task ClearSelectionAsync() => _grid?.SetSelectionAsync([]) ?? Task.CompletedTask;

    /// <summary>Clears sorts, filters and paging, returning the grid to its initial query.</summary>
    public Task ResetAsync() => _grid?.ResetQueryAsync() ?? Task.CompletedTask;

    /// <summary>Re-runs the query: reloads from the items provider, or re-filters the local items.</summary>
    public Task RefreshAsync() => _grid?.RefreshAsync() ?? Task.CompletedTask;

    // -- Materialization ------------------------------------------------------

    /// <summary>Copies the whole state into an immutable snapshot. Unlike the individual properties this
    /// allocates every collection, so take it when a consistent whole is needed - persistence, undo, a
    /// diff - rather than on each notification.</summary>
    /// <returns>The current state, or an empty state when no grid is attached.</returns>
    public DataGridState<TItem> Snapshot() => _grid?.BuildCurrentState() ?? DataGridState<TItem>.Empty();

    /// <summary>Projects the current state into the request shape a server-side provider receives, so an
    /// external query editor can build the same query the grid would.</summary>
    /// <returns>The request describing the current page, sorts and filters.</returns>
    public DataGridRequest ToRequest() => Snapshot().ToRequest();

    /// <summary>Builds the export payload for the current visible columns and filtered, sorted rows.</summary>
    /// <param name="fileName">File name (without extension) carried into the exported file.</param>
    /// <returns>The payload an <see cref="IDataGridExporter{TItem}"/> writes, or null when no grid is
    /// attached.</returns>
    public DataGridExportData<TItem>? GetExportData(string fileName) => _grid?.GetExportData(fileName);

    // -- Grid plumbing --------------------------------------------------------

    internal void Attach(FlareDataGrid<TItem> grid)
    {
        if (_grid is not null && !ReferenceEquals(_grid, grid))
            throw new InvalidOperationException(
                "A DataGridContext drives one grid. Give each FlareDataGrid its own Context instance.");
        _grid = grid;
        Raise(DataGridChange.All);
    }

    internal void Detach(FlareDataGrid<TItem> grid)
    {
        if (!ReferenceEquals(_grid, grid)) return;
        _grid = null;
        Raise(DataGridChange.All);
    }

    internal void Raise(DataGridChange change) => Changed?.Invoke(change);

    private static readonly Dictionary<string, string> EmptyFilters = [];
    private static readonly Dictionary<string, DataGridFilter> EmptyTyped = [];
    private static readonly HashSet<TItem> EmptySelection = [];
}
