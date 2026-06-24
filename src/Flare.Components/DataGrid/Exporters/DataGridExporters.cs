namespace Flare.Components;

/// <summary>Factory + registry for the standard <see cref="IDataGridExporter{TItem}"/> implementations.</summary>
public static class DataGridExporters
{
    /// <summary>Comma-separated values exporter (with a UTF-8 BOM for spreadsheet compatibility).</summary>
    public static IDataGridExporter<TItem> Csv<TItem>() => new CsvGridExporter<TItem>();
    /// <summary>Tab-separated values exporter.</summary>
    public static IDataGridExporter<TItem> Tsv<TItem>() => new TsvGridExporter<TItem>();
    /// <summary>JSON (array of objects keyed by column title) exporter.</summary>
    public static IDataGridExporter<TItem> Json<TItem>() => new JsonGridExporter<TItem>();
    /// <summary>Excel (.xlsx, OOXML) exporter.</summary>
    public static IDataGridExporter<TItem> Excel<TItem>() => new ExcelGridExporter<TItem>();
    /// <summary>Markdown exporter.</summary>
    public static IDataGridExporter<TItem> Markdown<TItem>() => new MarkdownExporter<TItem>();
    /// <summary>PDF exporter (dependency-free; standard Helvetica fonts, Latin-1 text).</summary>
    public static IDataGridExporter<TItem> Pdf<TItem>() => new PdfGridExporter<TItem>();

    /// <summary>Maps a legacy format string ("CSV"/"JSON"/"TSV"/"EXCEL"/"XLSX") to a standard exporter.</summary>
    public static IDataGridExporter<TItem>? FromId<TItem>(string id) => id?.ToUpperInvariant() switch
    {
        "CSV" => Csv<TItem>(),
        "TSV" => Tsv<TItem>(),
        "JSON" => Json<TItem>(),
        "EXCEL" or "XLSX" or "XLS" => Excel<TItem>(),
        "MD" or "MARKDOWN" => Markdown<TItem>(),
        "PDF" => Pdf<TItem>(),
        _ => null,
    };
}
