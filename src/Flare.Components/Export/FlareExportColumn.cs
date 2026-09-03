namespace Flare.Components;

/// <summary>
/// One exportable column: its header, the raw value accessor and an optional formatted-text accessor.
/// The same descriptor serves both export paths - the one a <see cref="FlareDataGrid{TItem}"/> builds
/// from its visible columns, and the one a caller writes by hand for data that has no grid behind it -
/// so a formatter written for one works in the other.
/// </summary>
/// <typeparam name="TRow">Row item type.</typeparam>
/// <param name="Title">Column header text.</param>
/// <param name="Value">Returns the raw cell value for a row (used by structured exporters like JSON).</param>
public sealed record FlareExportColumn<TRow>(string Title, Func<TRow, object?> Value)
{
    /// <summary>Optional type/format-aware display text for a row (matches what the grid renders).
    /// Text exporters (CSV, TSV, Markdown, Excel) should prefer this over <see cref="Value"/>.</summary>
    public Func<TRow, string>? Text { get; init; }

    /// <summary>The display text for a row: <see cref="Text"/> when set, else the raw value's string.</summary>
    /// <param name="row">The row to read.</param>
    /// <returns>The cell text.</returns>
    public string TextOf(TRow row) => Text is not null ? Text(row) : Value(row)?.ToString() ?? "";

    /// <summary>
    /// The display text for a row, formatting the raw value with <paramref name="provider"/>. An
    /// explicit <see cref="Text"/> wins: it already decided how the value reads, and re-formatting it
    /// would only be able to change the parts it deliberately fixed.
    /// </summary>
    /// <param name="row">The row to read.</param>
    /// <param name="provider">Culture used to format a raw value; ignored when <see cref="Text"/> is set.</param>
    /// <returns>The cell text.</returns>
    public string TextOf(TRow row, IFormatProvider provider)
    {
        if (Text is not null) return Text(row);
        var value = Value(row);
        return value switch
        {
            null => "",
            IFormattable f => f.ToString(null, provider),
            _ => value.ToString() ?? "",
        };
    }

    /// <summary>Builds a column whose cells are read and formatted by one selector.</summary>
    /// <param name="title">Column header text.</param>
    /// <param name="text">Returns the cell text for a row.</param>
    /// <returns>A column that exports exactly what the selector returns.</returns>
    public static FlareExportColumn<TRow> Of(string title, Func<TRow, string> text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new FlareExportColumn<TRow>(title, r => text(r)) { Text = text };
    }
}
