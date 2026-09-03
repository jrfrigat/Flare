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
    public FlareIcon? Icon => FlareIcons.Table;

    /// <summary>Exports the grid rows to a TSV file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join("\t", data.Columns.Select(c => Escape(c.Title))));
        foreach (var row in data.Rows)
            sb.AppendLine(string.Join("\t", data.Columns.Select(c => Escape(c.TextOf(row)))));
        var file = data.FileName + ".tsv";
        await download.DownloadAsync(file, sb.ToString(), "text/tab-separated-values");
    }

    // TSV has no quoting mechanism: a raw tab adds a column and a raw newline adds a row, so the file
    // silently changes shape rather than carrying a wrong value. Escape sequences keep the record count
    // exact and stay reversible, which quoting would not (a quote is an ordinary character here).
    // The leading-character guard is the same one CsvGridExporter applies, for the same reason: a
    // spreadsheet opens this file and treats a value starting with =, +, -, @, tab or CR as a formula.
    internal static string Escape(string value)
    {
        if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
            value = "'" + value;
        // Backslash first, or the sequences written below would be indistinguishable from literal input.
        return value
            .Replace("\\", "\\\\")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }
}
