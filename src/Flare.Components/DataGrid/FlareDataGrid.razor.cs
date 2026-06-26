using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace Flare.Components;

// Core code-behind for FlareDataGrid: parameters, fields, lifecycle, data loading (provider/
// infinite/virtual), the sort/filter/page pipeline entry points, public imperative API, the
// aggregate footer and selection helpers. Feature areas live in sibling partials
// (.Filtering, .Grouping, .Editing, .Bands, .Composite, .Reorder, .Tree). Only the two
// razor-template render helpers remain in FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- New state management API ---------------------------------------------

    /// <summary>Current grid state as an immutable snapshot. Updated on every state change.</summary>
    public DataGridState<TItem> CurrentState => BuildCurrentState();

    /// <summary>The grid's root CSS class.</summary>
    protected override string ComponentCssClass => Css.Classes.DataGrid.Root;

    // -- Helper methods to resolve effective values --

    private bool _effectiveVirtual => Virtual;
    private bool _effectiveInfiniteScroll => InfiniteScroll;
    private string _effectiveHeight => Height;
    // Virtualize rejects a non-positive ItemSize, so fall back to a sensible default row height
    // (matches the Virtualize default) when VirtualItemSize is unset or non-positive.
    private float _effectiveVirtualItemSize => VirtualItemSize > 0 ? VirtualItemSize.Value : DefaultVirtualItemSize;
    private const float DefaultVirtualItemSize = 48f;
    private int _effectiveOverscanCount => OverscanCount > 0 ? OverscanCount.Value : 20;

    private DataGridLoadingIndicator _effectiveLoadingIndicator => LoadingIndicator;
    private string? _effectiveLoadingText => LoadingText;

    private bool _effectiveInlineEdit => EditMode != DataGridEditMode.None;
    private bool _effectiveStriped => Striped;
    private bool _effectiveHoverable => Hoverable;
    private bool _effectiveDense => Dense;
    // Runtime quick-filter text set by FlareDataGridQuickFilter (a global search box).
    private string? _quickFilterText;

    // The effective quick filter: the QuickFilter parameter AND a global text-contains predicate over
    // the visible data columns (when a quick-filter search is active). Client-side only.
    private Func<TItem, bool>? _effectiveQuickFilter
    {
        get
        {
            var textPredicate = BuildQuickTextPredicate(_quickFilterText);
            if (QuickFilter is null) return textPredicate;
            if (textPredicate is null) return QuickFilter;
            return item => QuickFilter(item) && textPredicate(item);
        }
    }

    private Func<TItem, bool>? BuildQuickTextPredicate(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        var needle = text;
        var cols = _gridColumns.Where(c => c.Value is not null && !_hiddenColumns.Contains(c.Key)).ToList();
        return item => cols.Any(c => (c.Value!(item)?.ToString() ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase));
    }

    // Cache enumeration count to avoid double-enumeration on each render cycle.
    private int? _itemsCount;
    // Infinite scroll takes precedence over the known-total virtual window.
    private bool _infiniteMode => _effectiveInfiniteScroll && _provider is not null;
    private bool _shouldVirtualize => !_infiniteMode && _effectiveVirtual;
    // Both modes need a fixed-height scroll container with a sticky header.
    private bool _scrollContainer => _shouldVirtualize || _infiniteMode;

    // Dim the table only for the overlay indicators; ProgressLine/Skeleton keep the table at full opacity.
    private bool _dimTable => _loading &&
        (LoadingIndicator == DataGridLoadingIndicator.Spinner || LoadingIndicator == DataGridLoadingIndicator.Text);

    // Inline editing is on when explicitly enabled, EditMode is Inline/Cell, or when any column opts in via Editable.
    private bool _inlineEditEnabled => EditMode != DataGridEditMode.None || _gridColumns.Any(c => c.Editable);
    private bool _cellEditMode => EditMode == DataGridEditMode.Cell;

    private string? _striped => _effectiveStriped ? Css.Classes.DataGrid.Striped : null;
    private string? _hoverable => _effectiveHoverable ? Css.Classes.DataGrid.Hoverable : null;
    private string? _dense => _effectiveDense ? Css.Classes.DataGrid.Dense : null;
    private string? _bordered => Bordered ? Css.Classes.DataGrid.Bordered : null;

    // Stable render key for a row (RowKey selector, else the item reference).
    private object _rowKey(TItem item) => RowKey?.Invoke(item) ?? item!;

    // Tri-state select-all over the current page: true=all, false=none, null=some (indeterminate).
    private bool? _selectAllState
    {
        get
        {
            int total = 0, selected = 0;
            foreach (var i in _pageItems) { total++; if (_selection.Contains(i)) selected++; }
            if (total == 0 || selected == 0) return false;
            return selected == total ? true : null;
        }
    }

    private string _wrapperClass => _scrollContainer
        ? $"{Css.Classes.DataGrid.Wrapper} {Css.Classes.DataGrid.WrapperVirtual}"
        : Css.Classes.DataGrid.Wrapper;

    private string? _wrapperStyle => _scrollContainer
        ? $"--_virtual-height:{_effectiveHeight}"
        : null;

    private int _totalRows => _provider is not null ? _serverTotalCount : (Items?.Count() ?? 0);

    // Two public sources (declarative <FlareColumn>) projected into one
    // internal model so rendering/sorting/filtering have a single path.
    private readonly List<FlareColumn<TItem>> _columns = [];
    // Top-level header nodes (columns and bands). Ordered by DeclarationOrder for stable layout.
    private readonly List<FlareColumnBase> _headerNodes = [];
    // These read caches refreshed by RebuildGridColumns (structure changes only) - see Columns.cs.
    private List<FlareColumnBase> _orderedHeaderNodes => _orderedHeaderNodesCache;
    private List<FlareColumnBand> _topBands => _topBandsCache;
    private bool _hasBands => _hasBandsCache;
    // Number of band-title rows above the leaf column header row.
    private int _bandRowCount => _bandRowCountCache;
    private int _totalHeaderRows => _bandRowCount + 1;
    private List<GridColumn<TItem>> _gridColumns = [];

    // Column reorder state (display order overlay by Title; survives RebuildGridColumns).
    private readonly List<string> _columnOrder = [];
    private string? _dragColumn;
    private readonly Dictionary<string, string> _filters = new();
    private SortDirection _sortDir = SortDirection.None;
    private int _page;
    private List<TItem>? _sortedCache;
    private int _serverTotalCount;
    private bool _loading;
    private ElementReference _tableRef;
    // Virtualize refs for RefreshDataAsync() calls
    private Virtualize<TItem>? _virtualizeProviderRef;
    private Virtualize<TItem>? _virtualizeClientRef;
    // CancellationToken for async provider calls — cancelled on new request or dispose
    private CancellationTokenSource? _providerCts;
    // State persistence (localStorage)
    private DataGridPersistence<TItem>? _persistence;
    private bool _persistenceLoaded;
    // When any column is frozen the table must be allowed to exceed the wrapper width
    // (so it scrolls horizontally and the sticky columns have something to pin over).
    private bool _hasFrozen => _columns.Any(c => c.Frozen || c.FrozenRight);
    // Last layout signature pushed to JS (resize handles / frozen offsets), to skip redundant interop.
    private string? _lastSyncedLayout;

    // Infinite scroll state.
    private ElementReference _wrapperRef;
    private ElementReference _infiniteSentinel;
    private DotNetObjectReference<FlareDataGrid<TItem>>? _infiniteRef;
    private bool _infiniteObserverReady;
    private int _infinitePage;
    private bool _infiniteHasMore = true;
    private bool _infiniteLoading;
    private HashSet<TItem> _selection = [];
    private readonly List<(GridColumn<TItem> Column, SortDirection Direction)> _sortStack = [];
    private int _focusRow = -1;
    private int _focusCol = -1;
    private readonly HashSet<int> _expandedRows = new();

    // Pinned (always-visible, unsorted/unfiltered/unpaged) rows.
    private IReadOnlyList<TItem> _pinnedTop => PinnedTopRows ?? [];
    private IReadOnlyList<TItem> _pinnedBottom => PinnedBottomRows ?? [];

    // Menu filter mode state
    private string? _filterMenuOpen; // column key of the open filter menu
    private string _filterMenuOperator = "contains";
    private string _filterMenuValue = "";
    // Excel-style value checklist state: the set of currently checked distinct values + the search box.
    private readonly HashSet<string> _filterMenuValues = new(StringComparer.Ordinal);
    private string _filterMenuSearch = "";

    // Active group-by keys (server contract), populated from the effective grouping levels.
    private readonly List<string> _groupKeys = [];

    // Grouping state (Phase B). Collapsed group paths and the flattened render lines
    // (each line is either a GroupLine header or a boxed TItem data row).
    private readonly HashSet<string> _collapsedGroups = new(StringComparer.Ordinal);
    private List<object> _groupedLines = [];

    /// <summary>One group-header render line: a stable path, display text, nesting level, row count
    /// and the per-group aggregate (title, formatted-value) pairs.</summary>
    private sealed record GroupLine(
        string Path,
        string Display,
        int Level,
        int Count,
        IReadOnlyList<(string Title, string Value)> Aggregates,
        RenderFragment? HeaderTemplate = null);

    // Grouping levels registered by <DataGridGroup> children (outermost first).
    private readonly List<DataGridGroup<TItem>> _groupComponents = [];
    private IReadOnlyList<DataGridGroup<TItem>> _effectiveGroups => _groupComponents;

    // Tree-grid state
    private readonly HashSet<TItem> _expandedItems = new();
    private bool _treeMode => Tree is not null;

    // Column picker state
    private readonly HashSet<string> _hiddenColumns = [];

    // Inline edit state
    private TItem? _editingItem;
    private readonly Dictionary<string, string> _editValues = new();

    // Batch edit state. TItem is unconstrained, but rows are always non-null in practice, so these
    // collections key on TItem safely (CS8714 suppressed locally).
#pragma warning disable CS8714
    private readonly HashSet<TItem> _batchEditingItems = new();
    private readonly Dictionary<TItem, Dictionary<string, string>> _batchEditValues = new();
#pragma warning restore CS8714

    // Rows-per-page
    private int _currentPageSize;
    private bool _pageSizeInitialized;

    /// <summary>Recomputes derived column/row state when parameters change.</summary>
    protected override void OnParametersSet()
    {
        // Client (in-memory) mode recomputes on every parameter change; provider modes manage the
        // cache via their load methods (and infinite scroll accumulates across pages).
        if (_provider is null) _sortedCache = null;
        _selection = SelectedItems ?? _selection;
        // Controlled column order: sync the internal overlay when the parameter differs.
        if (ColumnOrder is not null && !ColumnOrder.SequenceEqual(_columnOrder))
        {
            _columnOrder.Clear();
            _columnOrder.AddRange(ColumnOrder);
        }
        // Controlled column visibility: mirror the parameter into the internal hidden set.
        if (HiddenColumns is not null && !_hiddenColumns.SetEquals(HiddenColumns))
        {
            _hiddenColumns.Clear();
            foreach (var key in HiddenColumns) _hiddenColumns.Add(key);
        }
        // A column's own parameters (Sortable/Width/...) can change without the grid knowing, so mark
        // the projection dirty on every parameter set; EnsureColumnsBuilt rebuilds it once before render.
        MarkColumnsDirty();
        RebuildGroupKeys();
        if (!_pageSizeInitialized)
        {
            _currentPageSize = PageSize;
            _pageSizeInitialized = true;
        }
        // Update item count used by virtualization threshold check.
        // Use ICollection.Count when available to avoid full enumeration.
        if (Items is ICollection<TItem> coll)
            _itemsCount = coll.Count;
        else if (Items is null)
            _itemsCount = 0;
        else
            _itemsCount = null; // unknown until Sorted() materialises the list
    }

    // Track whether the provider reference changed so we don't reload on every parent render.
    private Func<DataGridRequest, Task<DataGridResult<TItem>>>? _lastProvider;

    // The effective server-side provider: an explicit ItemsProvider, or one synthesized from
    // Queryable that runs DataGridQuery.Execute (the expression-tree translation). Cached so its
    // reference is stable for the provider-change check.
    private Func<DataGridRequest, Task<DataGridResult<TItem>>>? _queryProvider;
    private Func<DataGridRequest, Task<DataGridResult<TItem>>>? _provider
        => ItemsProvider ?? (Queryable is not null ? (_queryProvider ??= ExecuteQueryableAsync) : null);

    private Task<DataGridResult<TItem>> ExecuteQueryableAsync(DataGridRequest req)
        => Task.FromResult(DataGridQuery.Execute(Queryable!, req));

    // Virtual + provider mode loads windows on scroll through the Virtualize component instead.
    private bool _virtualProvider => _shouldVirtualize && _provider is not null;

    private IQueryable<TItem>? _lastQueryable;

    /// <summary>Reloads server-side data when data-bound parameters change.</summary>
    protected override async Task OnParametersSetAsync()
    {
        // Reload when the effective provider, or the Queryable source behind it, changes - and not in
        // virtual+provider mode (there the Virtualize scroller requests windows on demand).
        var queryableChanged = !ReferenceEquals(Queryable, _lastQueryable);
        _lastQueryable = Queryable;
        // Compare by delegate value (target + method), not reference: an `ItemsProvider="@x.Method"`
        // method group yields a fresh delegate instance on every parent render, so ReferenceEquals
        // would treat the provider as "changed" each render and reload (cancelling the in-flight load)
        // on every unrelated re-render.
        if (_provider is not null && !_virtualProvider
            && (!Equals(_provider, _lastProvider) || queryableChanged))
        {
            _lastProvider = _provider;
            _sortedCache = null;
            await LoadFromProviderAsync();
        }
    }

    // Virtualize ItemsProvider: maps a windowed (StartIndex/Count) request onto the user's
    // DataGridRequest/DataGridResult contract so rows load as the user scrolls.
    private async ValueTask<ItemsProviderResult<TItem>> ProvideVirtualItems(ItemsProviderRequest request)
    {
        if (_provider is null) return new ItemsProviderResult<TItem>([], 0);
        var sortKey = _sortStack.Count > 0 ? _sortStack[0].Column.Key : null;
        IReadOnlyDictionary<string, string>? activeFilters = _filters.Count > 0
            ? new Dictionary<string, string>(_filters)
            : null;
        var req = new DataGridRequest(0, request.Count, sortKey, _sortDir,
            StartIndex: request.StartIndex, Count: request.Count, Filters: activeFilters)
        {
            Sorts = BuildSorts(),
            FilterModel = BuildFilters(),
            GroupKeys = [.. _groupKeys],
            FilterTree = _advancedTree,
        };
        var result = await _provider!(req);
        _serverTotalCount = result.TotalCount;
        return new ItemsProviderResult<TItem>(result.Items, result.TotalCount);
    }

    private void ToggleRowDetail(int rowIndex)
    {
        if (!_expandedRows.Remove(rowIndex))
            _expandedRows.Add(rowIndex);
    }

    private async Task LoadFromProviderAsync()
    {
        if (_provider is null) return;
        // Infinite scroll: a (re)load resets the accumulation and fetches the first page.
        if (_infiniteMode)
        {
            ResetInfinite();
            await LoadNextPageAsync();
            return;
        }

        // Cancel any in-flight provider call
        _providerCts?.Cancel();
        _providerCts?.Dispose();
        _providerCts = new CancellationTokenSource();
        var ct = _providerCts.Token;

        _loading = true;
        StateHasChanged();
        try
        {
            var sortKey = _sortStack.Count > 0 ? _sortStack[0].Column.Key : null;
            IReadOnlyDictionary<string, string>? activeFilters = _filters.Count > 0
                ? new Dictionary<string, string>(_filters)
                : null;
            var req = new DataGridRequest(_page, _effectivePageSize, sortKey, _sortDir, Filters: activeFilters)
            {
                Sorts = BuildSorts(),
                FilterModel = BuildFilters(),
                GroupKeys = [.. _groupKeys],
                FilterTree = _advancedTree,
            };
            var result = await _provider!(req);
            if (ct.IsCancellationRequested) return;
            _sortedCache = result.Items.ToList();
            _serverTotalCount = result.TotalCount;
        }
        catch (OperationCanceledException) { }
        finally
        {
            // Only the winning (non-superseded) load clears the loading flag and re-renders. A
            // superseded load leaves the state to the newer one. Re-rendering here is essential:
            // LoadFromProviderAsync is also invoked fire-and-forget and from timers, where the
            // framework does not auto-render on completion -- without this the data would load but
            // the grid would keep showing the skeleton until the next unrelated event.
            if (!ct.IsCancellationRequested)
            {
                _loading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    // -- Infinite scroll ------------------------------------------------------

    private void ResetInfinite()
    {
        _sortedCache = [];
        _infinitePage = 0;
        _infiniteHasMore = true;
        _page = 0;
    }

    // Fetch the next page and append it to the accumulated rows. Stops when a short page is
    // returned or the reported total has been reached.
    private async Task LoadNextPageAsync()
    {
        if (_provider is null || _infiniteLoading || !_infiniteHasMore) return;

        // Cancel any previous infinite load
        _providerCts?.Cancel();
        _providerCts?.Dispose();
        _providerCts = new CancellationTokenSource();
        var ct = _providerCts.Token;

        _infiniteLoading = true;
        StateHasChanged();
        try
        {
            var sortKey = _sortStack.Count > 0 ? _sortStack[0].Column.Key : null;
            IReadOnlyDictionary<string, string>? activeFilters = _filters.Count > 0
                ? new Dictionary<string, string>(_filters)
                : null;
            var req = new DataGridRequest(_infinitePage, _effectivePageSize, sortKey, _sortDir, Filters: activeFilters)
            {
                Sorts = BuildSorts(),
                FilterModel = BuildFilters(),
                GroupKeys = [.. _groupKeys],
                FilterTree = _advancedTree,
            };
            var result = await _provider!(req);
            if (ct.IsCancellationRequested) return;
            var items = result.Items.ToList();
            (_sortedCache ??= []).AddRange(items);
            _serverTotalCount = result.TotalCount;
            _infinitePage++;
            if (items.Count < _effectivePageSize || (result.TotalCount > 0 && _sortedCache.Count >= result.TotalCount))
                _infiniteHasMore = false;
        }
        catch (OperationCanceledException) { }
        finally
        {
            _infiniteLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>Invoked from JS (IntersectionObserver) when the bottom sentinel becomes visible.</summary>
    [JSInvokable]
    public Task TriggerLoad() => LoadNextPageAsync();

    /// <summary>Performs the initial data load and persisted-state restore.</summary>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (!string.IsNullOrEmpty(PersistStateKey))
        {
            _persistence = new DataGridPersistence<TItem>(Storage, PersistStateKey);
            var saved = await _persistence.LoadAsync();
            if (saved is not null)
            {
                // Restore sorts
                if (saved.Sorts is { Count: > 0 })
                {
                    foreach (var s in saved.Sorts)
                    {
                        var col = _gridColumns.FirstOrDefault(c => c.Key == s.Key);
                        if (col is not null && col.Sortable)
                        {
                            var dir = Enum.TryParse<SortDirection>(s.Direction, true, out var d) ? d : SortDirection.Ascending;
                            _sortStack.Add((col, dir));
                        }
                    }
                }
                // Restore filters
                if (saved.Filters is { Count: > 0 })
                {
                    foreach (var (k, v) in saved.Filters)
                        _filters[k] = v;
                }
                // Restore column order
                if (saved.ColumnOrder is { Count: > 0 })
                {
                    _columnOrder.Clear();
                    _columnOrder.AddRange(saved.ColumnOrder);
                }
                // Restore hidden columns
                if (saved.HiddenColumns is { Count: > 0 })
                {
                    foreach (var k in saved.HiddenColumns)
                        _hiddenColumns.Add(k);
                }
                // Restore page size. Mark it initialized so OnParametersSet's default-page-size
                // step does not clobber the restored value (OnParametersSet runs after this).
                if (saved.PageSize > 0)
                {
                    _currentPageSize = saved.PageSize;
                    _pageSizeInitialized = true;
                }
                // Restore page
                if (saved.Page > 0)
                    _page = saved.Page;
            }

            // Mark ready AFTER the initial load completes, regardless of whether saved state existed:
            // a brand-new grid with empty storage must still begin persisting the user's first change.
            // (The restore above mutates fields directly and never triggers a save, so enabling it here
            // does not re-persist the just-loaded state.)
            _persistenceLoaded = true;
        }
    }

    /// <summary>Wires up JS interop (sticky headers, observers) after the first render.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Resize handles and frozen-column offsets only change with the column layout (set, order,
        // visibility, width), so sync them when the layout signature changes - not on every render.
        if (_layoutSignature != _lastSyncedLayout)
        {
            _lastSyncedLayout = _layoutSignature;
            if (_columns.Any(c => c.Resizable))
            {
                try { await JS.InvokeVoidAsync("FlareDataGrid.initAllResizeHandles", _tableRef); }
                catch (InvalidOperationException) { }
                catch (JSDisconnectedException) { }
            }
            if (_columns.Any(c => c.Frozen || c.FrozenRight))
            {
                try { await JS.InvokeVoidAsync("FlareDataGrid.updateFrozenOffsets", _tableRef); }
                catch (InvalidOperationException) { }
                catch (JSDisconnectedException) { }
            }
        }

        // (Re)attach the IntersectionObserver to the sentinel while in infinite mode.
        if (_infiniteMode && !_infiniteObserverReady)
        {
            _infiniteRef ??= DotNetObjectReference.Create(this);
            try
            {
                await JS.InvokeVoidAsync("FlareDataGrid.initInfinite", _infiniteSentinel, _wrapperRef, _infiniteRef, "160px");
                _infiniteObserverReady = true;
            }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
        }
        else if (!_infiniteMode && _infiniteObserverReady)
        {
            await DisposeInfiniteObserverAsync();
        }
    }

    private async Task DisposeInfiniteObserverAsync()
    {
        try { await JS.InvokeVoidAsync("FlareDataGrid.disposeInfinite", _infiniteSentinel); }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        _infiniteObserverReady = false;
    }

    /// <summary>Tears down JS interop and observers.</summary>
    public override async ValueTask DisposeAsync()
    {
        _providerCts?.Cancel();
        _providerCts?.Dispose();
        _filterDebounceTimer?.Dispose();
        await DisposeInfiniteObserverAsync();
        _infiniteRef?.Dispose();
        await base.DisposeAsync();
    }

}
