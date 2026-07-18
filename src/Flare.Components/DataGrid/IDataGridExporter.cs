namespace Flare.Components;

/// <summary>A single exportable column: its header, the raw value accessor and an optional formatted
/// text accessor.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
/// <param name="Title">Column header text.</param>
/// <param name="Value">Returns the raw cell value for a row (used by structured exporters like JSON).</param>
public sealed record DataGridExportColumn<TItem>(string Title, Func<TItem, object?> Value)
{
    /// <summary>Optional type/format-aware display text for a row (matches what the grid renders).
    /// Text exporters (CSV, TSV, Markdown, Excel) should prefer this over <see cref="Value"/>.</summary>
    public Func<TItem, string>? Text { get; init; }

    /// <summary>The display text for a row: <see cref="Text"/> when set, else the raw value's string.</summary>
    public string TextOf(TItem row) => Text is not null ? Text(row) : Value(row)?.ToString() ?? "";
}

/// <summary>The data handed to an exporter: the visible columns and the rows to export.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
public sealed class DataGridExportData<TItem>
{
    /// <summary>Visible columns in display order.</summary>
    public required IReadOnlyList<DataGridExportColumn<TItem>> Columns { get; init; }
    /// <summary>Rows to export (already sorted/filtered by the grid).</summary>
    public required IReadOnlyList<TItem> Rows { get; init; }
    /// <summary>Base file name configured on the grid (e.g. "export.csv" or "people"). The exporter
    /// is expected to apply its own extension.</summary>
    public required string FileName { get; init; }
}

/// <summary>
/// Pluggable DataGrid exporter. The grid renders a toolbar button per exporter and calls
/// <see cref="ExportAsync"/> with the current data. Implement this to add custom formats; the
/// library ships standard implementations (see <see cref="DataGridExporters"/>) that are not
/// hard-wired into the grid.
/// </summary>
/// <typeparam name="TItem">Row item type.</typeparam>
public interface IDataGridExporter<TItem>
{
    /// <summary>Stable identifier (also matches legacy <c>ExportFormats</c> strings like "CSV").</summary>
    string Id { get; }
    /// <summary>Toolbar button label (e.g. "CSV", "Excel").</summary>
    string Label { get; }
    /// <summary>Optional Material Symbols icon name shown on the button.</summary>
    FlareIcon? Icon { get; }
    /// <summary>Produces the file and triggers the download via <paramref name="download"/>.</summary>
    Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download);
}
