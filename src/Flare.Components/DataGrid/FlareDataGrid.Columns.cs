using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Column structure management: child registration (columns, bands, top-level nodes), projection of
// the declarative <FlareColumn> descriptors into the internal GridColumn model, leaf ordering and
// sort-header handling. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // Set when the column/band structure changes; the projection is rebuilt lazily (EnsureColumnsBuilt)
    // so a burst of N self-registering columns costs one rebuild instead of N (was O(N^2) on first render).
    private bool _columnsDirty = true;

    private void MarkColumnsDirty() => _columnsDirty = true;

    // Rebuilds the projected columns if a structural change marked them dirty. Called at the top of the
    // render (and the data pipeline) so every read sees an up-to-date column set.
    private void EnsureColumnsBuilt()
    {
        if (_columnsDirty) RebuildGridColumns();
    }

    internal void AddColumn(FlareColumn<TItem> col)
    {
        if (_columns.Contains(col)) return;
        _columns.Add(col);
        // Columns self-register from their OnInitialized while the grid's own render is still in flight
        // (the <Columns> fragment renders above the table). Mark dirty (rebuilt once on the next render)
        // and request that render so they appear instead of waiting for an unrelated StateHasChanged.
        MarkColumnsDirty();
        StateHasChanged();
    }

    internal void RemoveColumn(FlareColumn<TItem> col)
    {
        if (!_columns.Remove(col)) return;
        MarkColumnsDirty();
        StateHasChanged();
    }
    /// <summary>Returns all column titles. Used by DataGridColumnPicker when Columns parameter is empty.</summary>
    internal IEnumerable<string> GetColumnTitles() => _columns.Select(c => c.Title).Where(t => !string.IsNullOrEmpty(t));

    // Pickable (non-composite) columns as (Key, Title) pairs for the built-in column picker.
    private List<(string Key, string Title)> _pickerColumns =>
        _gridColumns.Where(c => !c.IsComposite).Select(c => (c.Key, c.Title)).ToList();

    /// <summary>Non-composite columns as (Key, Title) pairs, for a bound
    /// <see cref="DataGridColumnPicker{TItem}"/> to render its show/hide list.</summary>
    public IReadOnlyList<(string Key, string Title)> PickableColumns => _pickerColumns;

    /// <summary>Filterable, non-composite columns as (Key, Title) pairs, for a bound
    /// <see cref="DataGridFilterBuilder{TItem}"/> to offer as filter fields.</summary>
    public IReadOnlyList<(string Key, string Title)> FilterableColumns =>
        _gridColumns.Where(c => c.Filterable && !c.IsComposite).Select(c => (c.Key, c.Title)).ToList();

    /// <summary>Keys of the columns currently hidden (a key is absent here when the column is visible).</summary>
    public IReadOnlyCollection<string> HiddenColumnKeys => _hiddenColumns;

    /// <summary>Toggles a column's visibility by its stable key. Used by a bound
    /// <see cref="DataGridColumnPicker{TItem}"/>; honors controlled <c>HiddenColumns</c>.</summary>
    public Task ToggleColumnAsync(string key) => ToggleColumnVisibility(key);

    // Toggle a column's visibility by key. Honors controlled HiddenColumns via the change callback.
    private async Task ToggleColumnVisibility(string key)
    {
        if (!_hiddenColumns.Remove(key)) _hiddenColumns.Add(key);
        RebuildGridColumns();
        await HiddenColumnsChanged.InvokeAsync([.. _hiddenColumns]);
        await OnStateChanged.InvokeAsync(BuildState());
        await SaveStateAsync();
        StateHasChanged();
    }

    /// <summary>Registers a top-level band. Called by <see cref="FlareColumnBand"/> on initialization.</summary>
    public void AddBand(FlareColumnBand band)
    {
        if (_headerNodes.Contains(band)) return;
        _headerNodes.Add(band);
        NotifyStructureChanged();
    }

    /// <summary>Unregisters a top-level band on dispose.</summary>
    public void RemoveBand(FlareColumnBand band)
    {
        if (_headerNodes.Remove(band)) NotifyStructureChanged();
    }

    /// <summary>Registers a top-level (non-banded) column in the header structure.</summary>
    internal void AddTopLevelColumn(FlareColumnBase column)
    {
        if (_headerNodes.Contains(column)) return;
        _headerNodes.Add(column);
        NotifyStructureChanged();
    }

    /// <summary>Removes a top-level column from the header structure on dispose.</summary>
    internal void RemoveTopLevelColumn(FlareColumnBase column)
    {
        if (_headerNodes.Remove(column)) NotifyStructureChanged();
    }

    /// <summary>Re-projects columns and re-renders when the header structure (columns/bands) changes.</summary>
    public void NotifyStructureChanged()
    {
        MarkColumnsDirty();
        StateHasChanged();
    }

    // Cached header-node views, refreshed by RebuildGridColumns (structure changes only) so the header
    // render and FlattenLeafOrder don't re-sort/allocate on every property read.
    private List<FlareColumnBase> _orderedHeaderNodesCache = [];
    private List<FlareColumnBand> _topBandsCache = [];
    private bool _hasBandsCache;
    private int _bandRowCountCache;
    // Signature of the visible layout (key:width:frozen) - drives the frozen-offset JS sync so it
    // runs on structural/width changes, not on every render.
    private string _layoutSignature = "";

    // Project column sources into the single internal model and refresh derived caches.
    private void RebuildGridColumns()
    {
        _columnsDirty = false;
        // Refresh header-node caches first: FlattenLeafOrder and the banded header render read them.
        _orderedHeaderNodesCache = _headerNodes.OrderBy(n => n.DeclarationOrder).ToList();
        _topBandsCache = _orderedHeaderNodesCache.OfType<FlareColumnBand>().ToList();
        _hasBandsCache = _topBandsCache.Count > 0;
        _bandRowCountCache = _topBandsCache.Count > 0 ? _topBandsCache.Max(b => b.Depth) + 1 : 0;

        var list = new List<GridColumn<TItem>>(_columns.Count);
        foreach (var c in _columns)
        {
            var composite = c.CompositeRows.Count > 0 ? c.CompositeRows : null;
            list.Add(new GridColumn<TItem>
            {
                Title = c.Title,
                Id = c.Id,
                Value = c.Field,
                Type = c.Type,
                Format = c.Format,
                NullText = c.NullText,
                Align = c.Align,
                CellTemplate = c.Template,
                EditTemplate = c.EditTemplate,
                // A composite host is a layout column: not sortable/filterable/editable.
                Sortable = composite is null && c.Sortable,
                SortKey = c.SortKey,
                Filterable = composite is null && c.Filterable,
                FilterType = c.FilterType,
                FilterOptions = c.FilterOptions,
                Frozen = c.Frozen,
                FrozenRight = c.FrozenRight,
                Width = c.Width,
                Resizable = c.Resizable,
                Editable = composite is null && c.Editable,
                FilterDebounceMs = c.FilterDebounceMs,
                ClassFunc = c.ClassFunc,
                StyleFunc = c.StyleFunc,
                SortComparison = composite is null ? c.SortComparison : null,
                FilterFunc = composite is null ? c.FilterFunc : null,
                CompositeRows = composite,
                CompositeMode = c.CompositeMode,
            });
        }
        // Order columns to match the header leaf order (authoritative, and robust against
        // child-registration timing). For a flat grid this is just the declaration order.
        if (_headerNodes.Count > 0)
        {
            var leafOrder = FlattenLeafOrder();
            list = [.. list.OrderBy(c =>
            {
                var i = leafOrder.IndexOf(c.Key);
                return i < 0 ? int.MaxValue : i;
            })];
        }
        // Apply the user/host column order overlay (stable: unlisted columns keep relative order at the end).
        if (_columnOrder.Count > 0)
        {
            list = [.. list.OrderBy(c =>
            {
                var i = _columnOrder.IndexOf(c.Key);
                return i < 0 ? int.MaxValue : i;
            })];
        }

        _gridColumns = list;
        _columnStrategies = null; // depends only on the column set; rebuilt lazily
        InvalidateSelectorCache();
        _layoutSignature = string.Join("|", list.Where(c => !_hiddenColumns.Contains(c.Key))
            .Select(c => $"{c.Key}:{c.Width}:{c.Frozen}:{c.FrozenRight}"));
    }

    // Cached per-column strategy bundle. Depends only on _gridColumns, so it is computed once per
    // column-set change (cleared in RebuildGridColumns) instead of on every pipeline run.
    private DataGridColumnStrategies<TItem>? _columnStrategies;

    // Returns the per-column strategy overrides handed to the (column-agnostic) pipeline.
    private DataGridColumnStrategies<TItem> BuildColumnStrategies() => _columnStrategies ??= ComputeColumnStrategies();

    // Assembles compiled value selectors for every data column (and composite field) plus any custom
    // SortComparison / FilterFunc. Routing sort/filter through the columns' compiled Field accessors -
    // instead of the pipeline's property-name reflection - makes them work for any accessor (computed,
    // nested, non-POCO row types) and removes per-cell reflection from the hot path.
    private DataGridColumnStrategies<TItem> ComputeColumnStrategies()
    {
        var sortSelectors = CompositeSortSelectors();
        var filterSelectors = CompositeFilterSelectors();
        Dictionary<string, Comparison<TItem>>? comparers = null;
        Dictionary<string, Func<TItem, string, bool>>? filters = null;
        Dictionary<string, ColumnDataType>? types = null;
        foreach (var c in _gridColumns)
        {
            if (c.Value is not null)
            {
                sortSelectors.TryAdd(c.Key, c.Value);   // composite field keys ("Host/Field") never collide
                filterSelectors.TryAdd(c.Key, c.Value);
            }
            if (c.SortComparison is not null) (comparers ??= new(StringComparer.Ordinal))[c.Key] = c.SortComparison;
            if (c.FilterFunc is not null) (filters ??= new(StringComparer.Ordinal))[c.Key] = c.FilterFunc;
            // Resolve the type once (Auto samples the data) so the pipeline can compare numbers/dates by value.
            var type = ResolveColumnDataType(c.Key, c.Type);
            if (type is not (ColumnDataType.Text or ColumnDataType.Auto))
                (types ??= new(StringComparer.Ordinal))[c.Key] = type;
        }
        return new DataGridColumnStrategies<TItem>
        {
            SortSelectors = sortSelectors.Count > 0 ? sortSelectors : null,
            FilterSelectors = filterSelectors.Count > 0 ? filterSelectors : null,
            SortComparers = comparers,
            ColumnFilters = filters,
            ColumnTypes = types,
        };
    }

    // Flattens the header tree (_headerNodes) into its left-to-right leaf column order.
    private List<string> FlattenLeafOrder()
    {
        var order = new List<string>();
        void Walk(FlareColumnBase node)
        {
            if (node is FlareColumnBand band)
            {
                foreach (var child in band.Children) Walk(child);
            }
            else order.Add(node.ColumnKey);
        }
        foreach (var n in _orderedHeaderNodes) Walk(n);
        return order;
    }

    private async Task OnHeaderClick(GridColumn<TItem> col, MouseEventArgs e)
    {
        if (!col.Sortable) return;

        if (e.ShiftKey)
        {
            var existing = _sortStack.FindIndex(s => s.Column.Key == col.Key);
            if (existing >= 0)
            {
                var (c, dir) = _sortStack[existing];
                if (dir == SortDirection.Ascending)
                    _sortStack[existing] = (c, SortDirection.Descending);
                else
                    _sortStack.RemoveAt(existing);
            }
            else
            {
                _sortStack.Add((col, SortDirection.Ascending));
            }
        }
        else
        {
            var existing = _sortStack.FirstOrDefault(s => s.Column.Key == col.Key);
            _sortStack.Clear();
            if (existing.Column is not null)
            {
                if (existing.Direction == SortDirection.Ascending)
                    _sortStack.Add((col, SortDirection.Descending));
            }
            else
            {
                _sortStack.Add((col, SortDirection.Ascending));
            }
        }

        _sortDir = _sortStack.Count > 0 ? _sortStack[0].Direction : SortDirection.None;
        _sortedCache = null;
        _page = 0;

        await RaiseSortChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
    }
}
