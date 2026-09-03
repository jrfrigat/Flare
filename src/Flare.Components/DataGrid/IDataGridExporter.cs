namespace Flare.Components;

/// <summary>The data handed to an exporter: the visible columns and the rows to export.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
public sealed class DataGridExportData<TItem>
{
    /// <summary>Visible columns in display order.</summary>
    public required IReadOnlyList<FlareExportColumn<TItem>> Columns { get; init; }
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
