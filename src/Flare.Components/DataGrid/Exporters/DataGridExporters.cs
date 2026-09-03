namespace Flare.Components;

/// <summary>Factory + registry for the standard <see cref="IDataGridExporter{TItem}"/> implementations.</summary>
public static class DataGridExporters
{
    /// <summary>Comma-separated values exporter (with a UTF-8 BOM for spreadsheet compatibility).</summary>
    /// <typeparam name="TItem">Row item type.</typeparam>
    /// <returns>The exporter, writing RFC 4180 with a byte order mark.</returns>
    public static IDataGridExporter<TItem> Csv<TItem>() => new CsvGridExporter<TItem>();
    /// <summary>Comma-separated values exporter writing a chosen dialect - a semicolon separator for a
    /// locale whose spreadsheet expects one, a formula guard turned off for a machine-read file.</summary>
    /// <typeparam name="TItem">Row item type.</typeparam>
    /// <param name="options">The dialect to write.</param>
    /// <returns>The exporter, writing <paramref name="options"/>.</returns>
    public static IDataGridExporter<TItem> Csv<TItem>(FlareCsvOptions options) =>
        new CsvGridExporter<TItem> { Options = options };
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
