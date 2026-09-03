namespace Flare.Components;

/// <summary>Standard CSV exporter. Writes through <see cref="FlareCsv"/>, so the dialect is a
/// <see cref="FlareCsvOptions"/> and the escaping is the same one the standalone builder applies.</summary>
/// <typeparam name="TItem">Row item type.</typeparam>
public sealed class CsvGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "CSV";
    /// <summary>Display label for the export action.</summary>
    public string Label => "CSV";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public FlareIcon? Icon => FlareIcons.Download;

    /// <summary>
    /// Dialect to write. The default is RFC 4180 plus a byte order mark: a comma-separated file that a
    /// spreadsheet still opens as UTF-8. Pass <see cref="FlareCsvOptions.Spreadsheet"/> for a file the
    /// local spreadsheet opens without an import wizard.
    /// </summary>
    public FlareCsvOptions Options { get; init; } = FlareCsvOptions.Rfc4180 with { ByteOrderMark = true };

    /// <summary>Exports the grid rows to a CSV file and triggers its download.</summary>
    /// <param name="data">Columns and rows handed over by the grid.</param>
    /// <param name="download">The download port.</param>
    /// <returns>A task that completes once the download has been handed to the browser.</returns>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        ArgumentNullException.ThrowIfNull(data);
        await FlareCsv.DownloadAsync(download, data.FileName + ".csv", data.Rows, data.Columns, Options);
    }
}
