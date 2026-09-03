namespace Flare.Components;

/// <summary>Standard Markdown exporter.</summary>
public sealed class MarkdownExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "MD";
    /// <summary>Display label for the export action.</summary>
    public string Label => "Markdown";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public FlareIcon? Icon => FlareIcons.DataObject;

    /// <summary>Exports the grid rows to a Markdown table and triggers its download.</summary>
    public Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => Escape(c.Title))) + " |");
        sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(_ => "---")) + " |");

        foreach (var row in data.Rows)
            sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => Escape(c.TextOf(row)))) + " |");

        var file = data.FileName + ".md";
        return download.DownloadAsync(file, sb.ToString(), "text/markdown").AsTask();
    }

    // Only what breaks the TABLE is escaped, not every markdown construct: a cell that reads **total**
    // is meant to render bold in a markdown export. The GFM tables extension gives a cell exactly one
    // escape - a backslash before a pipe - and defines a row as a single line, so a raw newline ends the
    // row early and shifts every remaining value into the wrong column. A newline has no in-cell
    // representation there; a space is the lossy-but-inert substitute (<br> would inject markup into
    // data that a renderer may treat as HTML).
    internal static string Escape(string value) => value
        .Replace("|", "\\|")
        .Replace("\r\n", " ")
        .Replace('\r', ' ')
        .Replace('\n', ' ');
}
