using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Unified internal column model for <c>FlareDataGrid</c>. Both the declarative
/// <c>&lt;FlareColumn&gt;</c> child content and the fluent <c>ColumnsBuilder</c> API are projected
/// into this single shape so the grid has one rendering / sorting / filtering path.
/// </summary>
public sealed class GridColumn<TItem>
{
    /// <summary>Header text shown in the column heading.</summary>
    public string Title { get; init; } = "";
    /// <summary>Explicit stable identity override (see <c>FlareColumn.Id</c>); null falls back to SortKey/Title.</summary>
    public string? Id { get; init; }
    /// <summary>Value accessor used for cell text, sorting and filtering.</summary>
    public Func<TItem, object?>? Value { get; init; }
    /// <summary>Semantic data type used for type-aware cell rendering and the default filter editor.
    /// <see cref="ColumnDataType.Auto"/> (default) infers it from the runtime cell value.</summary>
    public ColumnDataType Type { get; init; } = ColumnDataType.Auto;
    /// <summary>Optional .NET format string applied to the cell value (e.g. "N2", "C", "yyyy-MM-dd").</summary>
    public string? Format { get; init; }
    /// <summary>Text rendered when the cell value is null (default: empty string).</summary>
    public string? NullText { get; init; }
    /// <summary>Horizontal alignment of the header and cells. Null = derive from <see cref="Type"/>
    /// (numbers right, booleans centered, otherwise leading).</summary>
    public ColumnAlign? Align { get; init; }
    /// <summary>Custom cell renderer; overrides <see cref="Value"/> text when set.</summary>
    public RenderFragment<TItem>? CellTemplate { get; init; }
    /// <summary>Custom editor renderer used while the row is being inline-edited.</summary>
    public RenderFragment<TItem>? EditTemplate { get; init; }
    /// <summary>Enables click-to-sort on this column.</summary>
    public bool Sortable { get; init; }
    /// <summary>Sort key sent to a server-side provider (falls back to <see cref="Title"/>).</summary>
    public string? SortKey { get; init; }
    /// <summary>Shows an inline filter editor for this column in the filter row.</summary>
    public bool Filterable { get; init; }
    /// <summary>Inline filter editor type (text/select/number/date). Null = derive from <see cref="Type"/>.</summary>
    public ColumnFilterType? FilterType { get; init; }
    /// <summary>Explicit option values for a select filter (null = distinct cell values).</summary>
    public IEnumerable<string>? FilterOptions { get; init; }
    /// <summary>Sticks the column to the left edge while scrolling horizontally.</summary>
    public bool Frozen { get; init; }
    /// <summary>Sticks the column to the right edge while scrolling horizontally.</summary>
    public bool FrozenRight { get; init; }
    /// <summary>Fixed CSS width.</summary>
    public string? Width { get; init; }
    /// <summary>Allows the user to drag-resize this column.</summary>
    public bool Resizable { get; init; }
    /// <summary>Marks the column's cells as inline-editable.</summary>
    public bool Editable { get; init; }
    /// <summary>Debounce delay for this column's filter input (null = use grid default).</summary>
    public int? FilterDebounceMs { get; init; }
    /// <summary>Returns extra CSS class(es) for this column's cell, given the row item.</summary>
    public Func<TItem, string>? ClassFunc { get; init; }
    /// <summary>Returns an inline style string for this column's cell, given the row item.</summary>
    public Func<TItem, string>? StyleFunc { get; init; }
    /// <summary>Custom row comparison used when this column is sorted (overrides value-based sorting).</summary>
    public Comparison<TItem>? SortComparison { get; init; }
    /// <summary>Custom filter predicate for this column: (row, filter text) =&gt; keep (overrides the built-in match).</summary>
    public Func<TItem, string, bool>? FilterFunc { get; init; }
    /// <summary>Composite layout rows for a layout-host column (null for a normal data column).</summary>
    public IReadOnlyList<FlareColumnRow>? CompositeRows { get; init; }

    /// <summary>How a composite column lays out (banded multi-row vs single-cell card).</summary>
    public CompositeMode CompositeMode { get; init; } = CompositeMode.Banded;

    /// <summary>True when this column is a composite layout host (stacked fields).</summary>
    public bool IsComposite => CompositeRows is { Count: > 0 };

    /// <summary>True when this composite host renders as a single-cell card.</summary>
    public bool IsCard => IsComposite && CompositeMode == CompositeMode.Card;

    /// <summary>Single stable column identity used everywhere: sort stack, filter/visibility/order state,
    /// persistence and the server sort/filter contract. Precedence: explicit Id, then SortKey, then Title.</summary>
    public string Key => Id ?? SortKey ?? Title;
}
