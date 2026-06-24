namespace Flare.Components;

/// <summary>Aggregate function type applied to a column's numeric values.</summary>
public enum AggregateType
{
    /// <summary>Sum.</summary>
    Sum,
    /// <summary>Count.</summary>
    Count,
    /// <summary>Average.</summary>
    Average,
    /// <summary>Min.</summary>
    Min,
    /// <summary>Max.</summary>
    Max,
}

/// <summary>
/// Horizontal text alignment of a column's header and cells. Values are logical (writing-direction
/// aware), so <see cref="End"/> is the right edge in LTR and the left edge in RTL.
/// </summary>
public enum ColumnAlign
{
    /// <summary>Leading edge (left in LTR). The default for text.</summary>
    Start,
    /// <summary>Centered. The default for booleans.</summary>
    Center,
    /// <summary>Trailing edge (right in LTR). The default for numbers.</summary>
    End,
}

/// <summary>
/// Semantic data type of a column. Drives type-aware cell rendering (boolean as a checkbox icon,
/// culture/format-aware dates and numbers) and the default inline filter editor. The default,
/// <see cref="Auto"/>, infers the type from the column's runtime cell values.
/// </summary>
public enum ColumnDataType
{
    /// <summary>Infer the type from the column's runtime value (the default).</summary>
    Auto,
    /// <summary>Plain text; rendered via <c>ToString</c>.</summary>
    Text,
    /// <summary>Numeric; rendered with the current culture and the column <c>Format</c>.</summary>
    Number,
    /// <summary>Boolean; rendered as a read-only checkbox icon.</summary>
    Boolean,
    /// <summary>Date only; rendered with the short-date pattern (or the column <c>Format</c>).</summary>
    Date,
    /// <summary>Date and time; rendered with the general pattern (or the column <c>Format</c>).</summary>
    DateTime,
    /// <summary>Time only; rendered with the short-time pattern (or the column <c>Format</c>).</summary>
    Time,
    /// <summary>Enumeration; rendered by name and filtered with a dropdown.</summary>
    Enum,
}

/// <summary>How a composite column (<c>FlareColumn</c> with a <c>Composite</c>) lays out its fields.</summary>
public enum CompositeMode
{
    /// <summary>Each record spans several real table rows; fields align across the whole table
    /// (DevExpress banded-view style). This is the default.</summary>
    Banded,
    /// <summary>All fields are stacked inside a single cell as a CSS grid (a card per record).</summary>
    Card,
}

/// <summary>A named, reusable advanced filter shown by <c>FlareDataGridFilterPresets</c>. Selecting
/// the preset applies its <see cref="Filter"/> tree to the grid.</summary>
/// <param name="Name">Display name shown in the preset list.</param>
/// <param name="Filter">The advanced filter group applied when the preset is chosen.</param>
public sealed record DataGridFilterPreset(string Name, DataGridFilterGroup Filter);

/// <summary>One pasted cell: the target row item, the column it lands in, and the pasted text.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
/// <param name="Item">The row the value is pasted into.</param>
/// <param name="ColumnKey">The key of the column the value is pasted into.</param>
/// <param name="Value">The pasted text (one tab-separated clipboard cell).</param>
public sealed record DataGridPasteCell<TItem>(TItem Item, string ColumnKey, string Value);

/// <summary>
/// Payload of a clipboard paste (Ctrl+V) into a cell-selected grid. The grid parses the clipboard's
/// tab-separated text and maps it onto the cells starting at the top-left of the current selection;
/// the handler applies each <see cref="DataGridPasteCell{TItem}"/> to its row item.
/// </summary>
/// <typeparam name="TItem">Row item type.</typeparam>
/// <param name="Cells">The cells to write, already mapped to row items and column keys.</param>
public sealed record DataGridPaste<TItem>(IReadOnlyList<DataGridPasteCell<TItem>> Cells);

/// <summary>Describes a row drag-and-drop reorder raised by <c>FlareDataGrid.OnRowReordered</c>.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
/// <param name="Item">The dragged row item.</param>
/// <param name="Target">The row item the dragged row was dropped onto.</param>
/// <param name="OldIndex">Index of the dragged row within the current page (0-based).</param>
/// <param name="NewIndex">Index of the drop target within the current page (0-based).</param>
public sealed record DataGridRowReorder<TItem>(TItem Item, TItem Target, int OldIndex, int NewIndex);

/// <summary>Defines an aggregate (footer) calculation for a data grid column.</summary>
public sealed class AggregateDefinition<TItem>
{
    /// <summary>Title of the column this aggregate applies to.</summary>
    public string ColumnTitle { get; init; } = "";
    /// <summary>Aggregate function to apply.</summary>
    public AggregateType Type { get; init; } = AggregateType.Count;
    /// <summary>Selector that extracts a numeric value from each row (required for Sum/Average/Min/Max).</summary>
    public Func<TItem, double>? ValueSelector { get; init; }
    /// <summary>Optional C# format string applied to the result (e.g. "N2", "C").</summary>
    public string? Format { get; init; }
}
