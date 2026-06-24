namespace Flare.Components;

/// <summary>
/// Immutable snapshot of the DataGrid state. Used for state management and persistence.
/// </summary>
public sealed record DataGridState<T>
{
    /// <summary>All registered columns.</summary>
    public required IReadOnlyList<GridColumn<T>> Columns { get; init; }

    /// <summary>Visible columns (after hiding some via picker).</summary>
    public required IReadOnlyList<GridColumn<T>> VisibleColumns { get; init; }

    /// <summary>Active sorts in apply order.</summary>
    public required IReadOnlyList<DataGridSort> Sorts { get; init; }

    /// <summary>Active text filters (column key -> value).</summary>
    public required IReadOnlyDictionary<string, string> Filters { get; init; }

    /// <summary>Active typed filters (column key -> structured filter).</summary>
    public required IReadOnlyDictionary<string, DataGridFilter> TypedFilters { get; init; }

    /// <summary>Advanced filter tree (null when unused).</summary>
    public DataGridFilterGroup? AdvancedFilter { get; init; }

    /// <summary>Current page index (0-based).</summary>
    public required int Page { get; init; }

    /// <summary>Rows per page.</summary>
    public required int PageSize { get; init; }

    /// <summary>Total number of rows (before paging).</summary>
    public required int TotalCount { get; init; }

    /// <summary>Currently selected items.</summary>
    public required IReadOnlySet<T> SelectedItems { get; init; }

    /// <summary>Group keys (outermost first).</summary>
    public required IReadOnlyList<string> GroupKeys { get; init; }

    /// <summary>Column display order (titles in order).</summary>
    public required IReadOnlyList<string> ColumnOrder { get; init; }

    /// <summary>Hidden column keys.</summary>
    public required IReadOnlySet<string> HiddenColumns { get; init; }

    /// <summary>Items currently being edited in batch mode.</summary>
    public required IReadOnlySet<T> BatchEditingItems { get; init; }

    /// <summary>Quick filter predicate (not serialized).</summary>
    public Func<T, bool>? QuickFilter { get; init; }

    /// <summary>Creates a default empty state.</summary>
    public static DataGridState<T> Empty() => new()
    {
        Columns = [],
        VisibleColumns = [],
        Sorts = [],
        Filters = new Dictionary<string, string>(),
        TypedFilters = new Dictionary<string, DataGridFilter>(),
        Page = 0,
        PageSize = 10,
        TotalCount = 0,
        SelectedItems = new HashSet<T>(),
        GroupKeys = [],
        ColumnOrder = [],
        HiddenColumns = new HashSet<string>(),
        BatchEditingItems = new HashSet<T>(),
    };

    /// <summary>Applies a sort change.</summary>
    public DataGridState<T> WithSort(IReadOnlyList<DataGridSort> sorts) =>
        this with { Sorts = sorts, Page = 0 };

    /// <summary>Applies a filter change.</summary>
    public DataGridState<T> WithFilters(IReadOnlyDictionary<string, string> filters) =>
        this with { Filters = filters, Page = 0 };

    /// <summary>Applies a page change.</summary>
    public DataGridState<T> WithPage(int page) =>
        this with { Page = page };

    /// <summary>Applies a page size change.</summary>
    public DataGridState<T> WithPageSize(int pageSize) =>
        this with { PageSize = pageSize, Page = 0 };

    /// <summary>Applies a selection change.</summary>
    public DataGridState<T> WithSelection(IReadOnlySet<T> selected) =>
        this with { SelectedItems = selected };

    /// <summary>Applies column visibility change.</summary>
    public DataGridState<T> WithHiddenColumns(IReadOnlySet<string> hidden) =>
        this with { HiddenColumns = hidden };

    /// <summary>Applies column order change.</summary>
    public DataGridState<T> WithColumnOrder(IReadOnlyList<string> order) =>
        this with { ColumnOrder = order };

    /// <summary>Builds a DataGridRequest for server-side providers.</summary>
    public DataGridRequest ToRequest() =>
        new(Page, PageSize,
            Sorts.Count > 0 ? Sorts[0].Key : null,
            Sorts.Count > 0 ? Sorts[0].Direction : SortDirection.None)
        {
            Sorts = Sorts,
            FilterModel = BuildFilterModel(),
            GroupKeys = GroupKeys,
            FilterTree = AdvancedFilter,
        };

    private IReadOnlyList<DataGridFilter> BuildFilterModel()
    {
        var list = Filters.Select(kv => new DataGridFilter(kv.Key, FilterOperator.Contains, kv.Value)).ToList();
        list.AddRange(TypedFilters.Values);
        if (AdvancedFilter is not null)
            FlattenConditions(AdvancedFilter, list);
        return list;
    }

    private static void FlattenConditions(DataGridFilterGroup g, List<DataGridFilter> into)
    {
        into.AddRange(g.Conditions);
        foreach (var sub in g.Groups) FlattenConditions(sub, into);
    }
}
