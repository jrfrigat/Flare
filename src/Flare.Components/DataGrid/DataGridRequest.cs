namespace Flare.Components;

/// <summary>Operator applied by a structured <see cref="DataGridFilter"/>.</summary>
public enum FilterOperator
{
    /// <summary>Cell text contains the value (default, case-insensitive).</summary>
    Contains,
    /// <summary>Equals.</summary>
    Equals,
    /// <summary>Not equals.</summary>
    NotEquals,
    /// <summary>Starts with.</summary>
    StartsWith,
    /// <summary>Ends with.</summary>
    EndsWith,
    /// <summary>Greater than.</summary>
    GreaterThan,
    /// <summary>Greater than or equal.</summary>
    GreaterThanOrEqual,
    /// <summary>Less than.</summary>
    LessThan,
    /// <summary>Less than or equal.</summary>
    LessThanOrEqual,
    /// <summary>Inclusive range between <see cref="DataGridFilter.Value"/> and <see cref="DataGridFilter.Value2"/>.</summary>
    Between,
    /// <summary>Outside the inclusive range between <see cref="DataGridFilter.Value"/> and <see cref="DataGridFilter.Value2"/>.</summary>
    NotBetween,
    /// <summary>Cell value is one of <see cref="DataGridFilter.Values"/>.</summary>
    In,
    /// <summary>Cell value is none of <see cref="DataGridFilter.Values"/>.</summary>
    NotIn,
    /// <summary>Cell value is null or empty.</summary>
    IsNull,
    /// <summary>Cell value is not null or empty.</summary>
    IsNotNull,
}

/// <summary>Inline filter editor rendered in a column's filter-row cell.</summary>
public enum ColumnFilterType
{
    /// <summary>Free-text box matched with <see cref="FilterOperator.Contains"/> (default).</summary>
    Text,
    /// <summary>Dropdown of values (explicit or distinct), matched with <see cref="FilterOperator.Equals"/>.</summary>
    Select,
    /// <summary>Numeric input, matched with <see cref="FilterOperator.Equals"/> (numeric comparison).</summary>
    Number,
    /// <summary>Date input, matched with <see cref="FilterOperator.Equals"/> (date comparison).</summary>
    Date,
}

/// <summary>One column sort instruction. Multiple are applied in list order (multi-sort).</summary>
public sealed record DataGridSort(string Key, SortDirection Direction);

/// <summary>
/// One structured column filter. <see cref="Key"/> is the column sort key / title, <see cref="Operator"/>
/// the comparison, and the value(s) the operand(s). Forward-compatible with the advanced filter builder.
/// </summary>
public sealed record DataGridFilter(
    string Key,
    FilterOperator Operator = FilterOperator.Contains,
    string? Value = null,
    string? Value2 = null,
    IReadOnlyList<string>? Values = null);

/// <summary>
/// A node in the advanced filter tree: a logical group combining child conditions and sub-groups with
/// AND (<paramref name="Or"/> false) or OR (<paramref name="Or"/> true). Mirrors a dbForge-style builder
/// and lets a server provider rebuild a nested WHERE clause.
/// </summary>
public sealed record DataGridFilterGroup(
    bool Or,
    IReadOnlyList<DataGridFilter> Conditions,
    IReadOnlyList<DataGridFilterGroup> Groups)
{
    /// <summary>True when this group has no conditions and no sub-groups.</summary>
    public bool IsEmpty => Conditions.Count == 0 && Groups.Count == 0;
}

/// <summary>
/// The full data state of a <c>FlareDataGrid</c> at a point in time: page, sorts, filters and group
/// keys. Emitted via the grid's change events so the host can build a server query or persist layout.
/// </summary>
public sealed record DataGridState(
    int Page,
    int PageSize,
    IReadOnlyList<DataGridSort> Sorts,
    IReadOnlyList<DataGridFilter> Filters,
    IReadOnlyList<string> GroupKeys)
{
    /// <summary>The advanced nested filter tree (null when the advanced builder is unused).</summary>
    public DataGridFilterGroup? FilterTree { get; init; }

    /// <summary>Column titles in their current display order (reflects user reordering). Empty when unchanged.</summary>
    public IReadOnlyList<string> ColumnOrder { get; init; } = [];
}

/// <summary>
/// Request passed to a server-side items provider. The positional members are kept for
/// backward compatibility; the init-only members carry the richer multi-sort / structured-filter /
/// grouping model (mirrors <see cref="DataGridState"/>).
/// </summary>
public sealed record DataGridRequest(
    int Page,
    int PageSize,
    string? SortKey,
    SortDirection SortDirection,
    int? StartIndex = null,
    int? Count = null,
    IReadOnlyDictionary<string, string>? Filters = null)
{
    /// <summary>All active sorts, in apply order (the legacy <see cref="SortKey"/> is the first one).</summary>
    public IReadOnlyList<DataGridSort> Sorts { get; init; } = [];

    /// <summary>Structured filters (the legacy <see cref="Filters"/> dictionary is a flattened view).</summary>
    public IReadOnlyList<DataGridFilter> FilterModel { get; init; } = [];

    /// <summary>Active group-by column keys, outermost first.</summary>
    public IReadOnlyList<string> GroupKeys { get; init; } = [];

    /// <summary>The advanced nested filter tree (null when the advanced builder is unused).</summary>
    public DataGridFilterGroup? FilterTree { get; init; }

    /// <summary>The global quick-filter search text (set by <c>FlareDataGridQuickFilter</c>), or null when
    /// no quick search is active. A server provider should match it across its searchable fields.</summary>
    public string? QuickFilter { get; init; }
}

/// <summary>Result returned by a server-side items provider: the page of items plus the total count.</summary>
public sealed record DataGridResult<TItem>(
    IEnumerable<TItem> Items,
    int TotalCount);

/// <summary>Selection mode for the DataGrid.</summary>
public enum DataGridSelectionMode
{
    /// <summary>No selection.</summary>
    None,
    /// <summary>Single row selection.</summary>
    Single,
    /// <summary>Multiple row selection with checkboxes.</summary>
    Multiple,
}

/// <summary>Edit mode for the DataGrid.</summary>
public enum DataGridEditMode
{
    /// <summary>No editing.</summary>
    None,
    /// <summary>Inline row editing (one row at a time).</summary>
    Inline,
    /// <summary>Batch editing (multiple rows, save all at once).</summary>
    Batch,
    /// <summary>Cell editing (click to edit individual cells).</summary>
    Cell,
}
