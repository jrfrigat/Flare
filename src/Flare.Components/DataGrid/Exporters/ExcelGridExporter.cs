namespace Flare.Components;

/// <summary>Standard Excel (.xlsx) exporter built on the dependency-free <see cref="XlsxWriter"/>.</summary>
public sealed class ExcelGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "EXCEL";
    /// <summary>Display label for the export action.</summary>
    public string Label => "Excel";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public string? Icon => "grid_on";

    /// <summary>Exports the grid rows to an Excel-compatible file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var headers = data.Columns.Select(c => c.Title).ToList();
        var rows = data.Rows
            .Select(row => (IReadOnlyList<string?>)data.Columns.Select(c => (string?)c.TextOf(row)).ToList())
            .ToList();
        var bytes = XlsxWriter.Write(headers, rows);
        var file = data.FileName + ".xlsx";
        await download.DownloadBytesAsync(file, bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}
