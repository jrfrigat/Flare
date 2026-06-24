using System.Text;

namespace Flare.Components;

/// <summary>Standard CSV exporter (RFC-4180-ish, with CSV-injection guard).</summary>
public sealed class CsvGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "CSV";
    /// <summary>Display label for the export action.</summary>
    public string Label => "CSV";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public string? Icon => "download";

    /// <summary>Exports the grid rows to a CSV file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", data.Columns.Select(c => Escape(c.Title))));
        foreach (var row in data.Rows)
            sb.AppendLine(string.Join(",", data.Columns.Select(c => Escape(c.TextOf(row)))));
        var file = data.FileName + ".csv";
        await download.DownloadCsvAsync(file, sb.ToString());
    }

    // Prefix risky leads to neutralise CSV/formula injection, then quote as needed.
    internal static string Escape(string value)
    {
        if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
            value = "'" + value;
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}
