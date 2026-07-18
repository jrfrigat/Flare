namespace Flare.Components;

/// <summary>Standard PDF exporter built on the dependency-free <see cref="PdfWriter"/>: a paginated
/// table with a repeated header row, using each column's display text.</summary>
public sealed class PdfGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "PDF";
    /// <summary>Display label for the export action.</summary>
    public string Label => "PDF";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public FlareIcon? Icon => FlareIcons.PictureAsPdf;

    /// <summary>Exports the grid rows to a PDF file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var headers = data.Columns.Select(c => c.Title).ToList();
        var rows = data.Rows
            .Select(row => (IReadOnlyList<string?>)data.Columns.Select(c => (string?)c.TextOf(row)).ToList())
            .ToList();
        var bytes = PdfWriter.Write(headers, rows, data.FileName);
        await download.DownloadBytesAsync(data.FileName + ".pdf", bytes, "application/pdf");
    }
}
