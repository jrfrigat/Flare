using System.Text;

namespace Flare.Components;

/// <summary>Standard TSV exporter.</summary>
public sealed class TsvGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "TSV";
    /// <summary>Display label for the export action.</summary>
    public string Label => "TSV";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public string? Icon => "table";

    /// <summary>Exports the grid rows to a TSV file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join("\t", data.Columns.Select(c => c.Title)));
        foreach (var row in data.Rows)
            sb.AppendLine(string.Join("\t", data.Columns.Select(c => c.TextOf(row))));
        var file = data.FileName + ".tsv";
        await download.DownloadAsync(file, sb.ToString(), "text/tab-separated-values");
    }
}
