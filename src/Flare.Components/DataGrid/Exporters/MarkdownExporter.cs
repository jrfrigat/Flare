namespace Flare.Components;

/// <summary>Standard Markdown exporter.</summary>
public sealed class MarkdownExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "MD";
    /// <summary>Display label for the export action.</summary>
    public string Label => "Markdown";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public string? Icon => "data_object";

    /// <summary>Exports the grid rows to a Markdown table and triggers its download.</summary>
    public Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.Title)) + " |");
        sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(_ => "---")) + " |");

        foreach (var row in data.Rows)
            sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.TextOf(row))) + " |");

        var file = data.FileName + ".md";
        return download.DownloadAsync(file, sb.ToString(), "text/markdown").AsTask();
    }
}
