namespace Flare.Components;

// Column filtering: simple text filter row, header menu filters, typed (select/number/date)
// filters, the value-selector cache and distinct-value helpers. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- Menu filter mode helpers --------------------------------------------

    private readonly List<string> _menuFilterOperators = ["contains", "equals", "not equals", "starts with", "ends with", "greater than", "less than"];

    private void ToggleFilterMenu(string columnKey)
    {
        if (_filterMenuOpen == columnKey)
        {
            _filterMenuOpen = null;
            _filterMenuValue = "";
            _filterMenuOperator = "contains";
        }
        else
        {
            _filterMenuOpen = columnKey;
            _filterMenuSearch = "";
            var distinct = DistinctValues(columnKey).ToList();
            _filterMenuValues.Clear();
            // Pre-fill from the current filter: an In filter restores its checked set, anything else
            // (or no filter) starts with every value checked.
            if (_typedFilters.TryGetValue(columnKey, out var existing))
            {
                if (existing.Operator == FilterOperator.In && existing.Values is not null)
                    foreach (var v in existing.Values) _filterMenuValues.Add(v);
                else
                    foreach (var v in distinct) _filterMenuValues.Add(v);
                _filterMenuValue = existing.Value ?? "";
                _filterMenuOperator = existing.Operator switch
                {
                    FilterOperator.Equals => "equals",
                    FilterOperator.NotEquals => "not equals",
                    FilterOperator.StartsWith => "starts with",
                    FilterOperator.EndsWith => "ends with",
                    FilterOperator.GreaterThan => "greater than",
                    FilterOperator.LessThan => "less than",
                    _ => "contains"
                };
            }
            else
            {
                foreach (var v in distinct) _filterMenuValues.Add(v);
                _filterMenuValue = "";
                _filterMenuOperator = "contains";
            }
        }
        StateHasChanged();
    }

    // Distinct values matching the menu search box (the Excel-style value checklist contents).
    private IEnumerable<string> MenuFilterValues(string columnKey)
    {
        var all = DistinctValues(columnKey);
        return string.IsNullOrEmpty(_filterMenuSearch)
            ? all
            : all.Where(v => v.Contains(_filterMenuSearch, StringComparison.OrdinalIgnoreCase));
    }

    // True when every distinct value is checked (drives the "select all" checkbox).
    private bool MenuAllSelected(string columnKey) => _filterMenuValues.Count >= DistinctValues(columnKey).Count();

    private void ToggleMenuValue(string value)
    {
        if (!_filterMenuValues.Remove(value)) _filterMenuValues.Add(value);
        StateHasChanged();
    }

    private void ToggleSelectAllMenu(string columnKey)
    {
        var distinct = DistinctValues(columnKey).ToList();
        var allOn = _filterMenuValues.Count >= distinct.Count;
        _filterMenuValues.Clear();
        if (!allOn)
            foreach (var v in distinct) _filterMenuValues.Add(v);
        StateHasChanged();
    }

    private async Task ApplyFilterMenu(string columnKey)
    {
        var distinct = DistinctValues(columnKey).ToList();
        // A value checklist with anything unchecked is an "In" (set) filter and takes precedence;
        // otherwise fall back to the operator/value condition; an all-checked, empty condition clears.
        if (_filterMenuValues.Count < distinct.Count)
        {
            _typedFilters[columnKey] = new DataGridFilter(columnKey, FilterOperator.In, Values: [.. _filterMenuValues]);
        }
        else if (!string.IsNullOrEmpty(_filterMenuValue))
        {
            var op = _filterMenuOperator switch
            {
                "equals" => FilterOperator.Equals,
                "not equals" => FilterOperator.NotEquals,
                "starts with" => FilterOperator.StartsWith,
                "ends with" => FilterOperator.EndsWith,
                "greater than" => FilterOperator.GreaterThan,
                "less than" => FilterOperator.LessThan,
                _ => FilterOperator.Contains
            };
            _typedFilters[columnKey] = new DataGridFilter(columnKey, op, _filterMenuValue);
        }
        else
        {
            _typedFilters.Remove(columnKey);
        }

        _filterMenuOpen = null;
        _filterMenuValue = "";
        _filterMenuOperator = "contains";
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
        StateHasChanged();
    }

    private async Task ClearFilterMenu(string columnKey)
    {
        _typedFilters.Remove(columnKey);
        _filterMenuOpen = null;
        _filterMenuValue = "";
        _filterMenuOperator = "contains";
        _sortedCache = null;
        _itemsCount = null;
        _page = 0;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
        else
            await InvalidateAndRefreshAsync();
        StateHasChanged();
    }

    private Timer? _filterDebounceTimer;

    private void OnFilterInput(string columnTitle, string value)
    {
        if (string.IsNullOrEmpty(value))
            _filters.Remove(columnTitle);
        else
            _filters[columnTitle] = value;

        // Resolve debounce delay: column-level, then composite-field-level (default 300ms).
        var col = _gridColumns.FirstOrDefault(c => c.Key == columnTitle);
        var debounceMs = col?.FilterDebounceMs ?? CompositeFieldByKey(columnTitle)?.FilterDebounceMs ?? 300;

        if (debounceMs <= 0)
        {
            // Instant filter application
            _sortedCache = null;
            _itemsCount = null;
            _page = 0;
            _focusRow = -1;
            _ = RaiseFilterChangedAsync();
            _ = (_provider is not null ? LoadFromProviderAsync() : InvalidateAndRefreshAsync());
            return;
        }

        // Debounce
        _filterDebounceTimer?.Dispose();
        _filterDebounceTimer = new Timer(async _ =>
        {
            await InvokeAsync(async () =>
            {
                _sortedCache = null;
                _itemsCount = null;
                _page = 0;
                _focusRow = -1;
                await RaiseFilterChangedAsync();
                if (_provider is not null)
                    await LoadFromProviderAsync();
                else
                    await InvalidateAndRefreshAsync();
                StateHasChanged();
            });
        }, null, debounceMs, Timeout.Infinite);
    }

    // -- Typed column filters + advanced filter builder (Phase C) -------------

    // Per-column structured filters from non-text inline editors (select/number/date).
    private readonly Dictionary<string, DataGridFilter> _typedFilters = new();

    // Advanced filter tree (applied via DataGridFilterBuilder or ApplyAdvancedFilter).
    private DataGridFilterGroup? _advancedTree;

    private async Task OnTypedFilterChanged(string key, FilterOperator op, string? value)
    {
        if (string.IsNullOrEmpty(value))
            _typedFilters.Remove(key);
        else
            _typedFilters[key] = new DataGridFilter(key, op, value);
        _sortedCache = null;
        _page = 0;
        _focusRow = -1;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
    }

    // Context passed to the inline filter-editor template in the filter row.
    private readonly record struct FilterEditorCtx(string Key, ColumnFilterType Type, IEnumerable<string>? Options);

    // Resolves a column's effective data type: the declared type, or - when Auto - the type inferred
    // from the first non-null cell value sampled via the column selector.
    private ColumnDataType ResolveColumnDataType(string key, ColumnDataType declared)
    {
        if (declared != ColumnDataType.Auto) return declared;
        var sel = ResolveSelector(key);
        object? sample = null;
        if (sel is not null && Items is not null)
            foreach (var i in Items) { sample = sel(i); if (sample is not null) break; }
        return DataGridValueFormatter.Infer(sample?.GetType());
    }

    // Resolves the inline filter editor for a column: the explicit FilterType if set, otherwise one
    // derived from the column's (possibly auto-inferred) data type.
    private ColumnFilterType ResolveFilterType(string key, ColumnFilterType? explicitType, ColumnDataType dataType)
        => explicitType ?? DataGridValueFormatter.ToFilterType(ResolveColumnDataType(key, dataType));

    // Cache of column key -> alignment CSS class. Sampling the data type is O(rows); the cell/header
    // render calls AlignClass for every cell, so the result is memoized per column (a column's value
    // type is stable across data updates). Cleared with the selector cache on column-set changes.
    private readonly Dictionary<string, string> _alignClassCache = new(StringComparer.Ordinal);

    // Resolves a column's horizontal-alignment CSS class: explicit Align, else derived from the
    // (possibly inferred) data type - numbers trail, booleans center, everything else leads (no class).
    private string AlignClass(GridColumn<TItem> col)
    {
        if (_alignClassCache.TryGetValue(col.Key, out var cached)) return cached;
        var align = col.Align ?? DefaultAlign(ResolveColumnDataType(col.Key, col.Type));
        var cls = align switch
        {
            ColumnAlign.Center => Css.Classes.DataGrid.AlignCenter,
            ColumnAlign.End => Css.Classes.DataGrid.AlignEnd,
            _ => "",
        };
        _alignClassCache[col.Key] = cls;
        return cls;
    }

    private static ColumnAlign DefaultAlign(ColumnDataType type) => type switch
    {
        ColumnDataType.Number => ColumnAlign.End,
        ColumnDataType.Boolean => ColumnAlign.Center,
        _ => ColumnAlign.Start,
    };

    // Current value backing the typed (number/date) and text inline editors.
    private string? TypedFilterValue(string key) => _typedFilters.TryGetValue(key, out var f) ? f.Value : null;
    private string? TextFilterValue(string key) => _filters.TryGetValue(key, out var v) ? v : null;

    // Currently-selected values for a Select (multi) filter (FlareMultiSelect binds IReadOnlyList).
    private IReadOnlyList<string> SelectedFilterValues(string key)
        => _typedFilters.TryGetValue(key, out var f) && f.Values is not null ? [.. f.Values] : [];

    // A Select filter selects one or more values -> stored as an In filter.
    private async Task OnSelectFilterChanged(string key, IReadOnlyCollection<string> values)
    {
        if (values.Count == 0)
            _typedFilters.Remove(key);
        else
            _typedFilters[key] = new DataGridFilter(key, FilterOperator.In, Values: [.. values]);
        _sortedCache = null;
        _page = 0;
        _focusRow = -1;
        await RaiseFilterChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
    }

    // Distinct non-empty string values for a column, used to populate a Select filter.
    private IEnumerable<string> DistinctValues(string key)
    {
        var sel = ResolveSelector(key);
        if (sel is null || Items is null) return [];
        return Items.Select(i => sel(i)?.ToString() ?? "")
            .Where(s => s.Length > 0)
            .Distinct()
            .OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase);
    }

    // Resolve a column value selector by its filter key (sort key or title).
    // Cache of key -> value selector so per-row filter evaluation is O(1) instead of scanning
    // the column lists for every cell. Invalidated whenever the column set changes.
    private readonly Dictionary<string, Func<TItem, object?>?> _selectorCache = new(StringComparer.Ordinal);

    private void InvalidateSelectorCache()
    {
        _selectorCache.Clear();
        _alignClassCache.Clear();
    }

    private Func<TItem, object?>? ResolveSelector(string key)
    {
        if (_selectorCache.TryGetValue(key, out var cached)) return cached;
        var col = _gridColumns.FirstOrDefault(c => c.Key == key || c.Title == key);
        var selector = col?.Value;
        // Composite field keys ("Host/Field") are not in _gridColumns; resolve via the field accessor.
        if (selector is null && key.Contains('/') && CompositeFilterSelectors().TryGetValue(key, out var compSel))
            selector = compSel;
        _selectorCache[key] = selector;
        return selector;
    }
}
