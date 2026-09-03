using System.Text;

namespace Flare.Components;

/// <summary>
/// Builds CSV text from any data, with the escaping and formula guard the grid exporter uses. The
/// export pipeline behind <see cref="FlareDataGrid{TItem}"/> is grid-shaped on purpose - it exports
/// what the grid shows - which leaves two ordinary needs unanswered: the full set behind a filtered
/// view, and data that has no grid at all. Both are this class.
/// </summary>
/// <example>
/// <code>
/// var columns = new[]
/// {
///     FlareExportColumn&lt;Order&gt;.Of("Number", o =&gt; o.Number),
///     FlareExportColumn&lt;Order&gt;.Of("Shipped", o =&gt; o.Shipped.ToString("d")),
/// };
/// await FlareCsv.DownloadAsync(Download, "orders.csv", allOrders, columns, FlareCsvOptions.Spreadsheet);
/// </code>
/// </example>
public static class FlareCsv
{
    /// <summary>Builds CSV text from typed rows and column descriptors.</summary>
    /// <typeparam name="TRow">Row item type.</typeparam>
    /// <param name="rows">The rows to write, in output order.</param>
    /// <param name="columns">Columns to write, in output order; their titles form the header row.</param>
    /// <param name="options">Dialect to write; defaults to <see cref="FlareCsvOptions.Rfc4180"/>.</param>
    /// <returns>The CSV text, header row first. Never carries a byte order mark - see
    /// <see cref="FlareCsvOptions.ByteOrderMark"/>.</returns>
    public static string Build<TRow>(
        IEnumerable<TRow> rows,
        IReadOnlyList<FlareExportColumn<TRow>> columns,
        FlareCsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(columns);
        var opt = options ?? FlareCsvOptions.Rfc4180;
        return Build(
            columns.Select(c => c.Title).ToList(),
            rows.Select(r => columns.Select(c => c.TextOf(r, opt.FormatProvider))),
            opt);
    }

    /// <summary>Builds CSV text from a header row and rows of already-formatted cells.</summary>
    /// <param name="headers">Header cells; pass an empty sequence for a headerless file.</param>
    /// <param name="rows">Rows of cells, each in the same order as <paramref name="headers"/>.</param>
    /// <param name="options">Dialect to write; defaults to <see cref="FlareCsvOptions.Rfc4180"/>.</param>
    /// <returns>The CSV text. Never carries a byte order mark - see
    /// <see cref="FlareCsvOptions.ByteOrderMark"/>.</returns>
    public static string Build(
        IEnumerable<string> headers,
        IEnumerable<IEnumerable<string>> rows,
        FlareCsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(rows);
        var opt = options ?? FlareCsvOptions.Rfc4180;
        var separator = opt.Delimiter.ToString();
        var sb = new StringBuilder();

        var header = headers.ToList();
        if (header.Count > 0)
        {
            sb.Append(string.Join(separator, header.Select(h => Escape(h, opt))));
            sb.Append(opt.NewLine);
        }
        foreach (var row in rows)
        {
            sb.Append(string.Join(separator, row.Select(cell => Escape(cell, opt))));
            sb.Append(opt.NewLine);
        }
        return sb.ToString();
    }

    /// <summary>Builds the file and hands it to the browser, applying
    /// <see cref="FlareCsvOptions.ByteOrderMark"/> as the file is written.</summary>
    /// <typeparam name="TRow">Row item type.</typeparam>
    /// <param name="download">The download port (inject <see cref="IFlareDownload"/>).</param>
    /// <param name="fileName">File name offered to the browser, extension included.</param>
    /// <param name="rows">The rows to write, in output order.</param>
    /// <param name="columns">Columns to write, in output order.</param>
    /// <param name="options">Dialect to write; defaults to <see cref="FlareCsvOptions.Rfc4180"/>.</param>
    /// <returns>A task that completes once the download has been handed to the browser.</returns>
    public static ValueTask DownloadAsync<TRow>(
        IFlareDownload download,
        string fileName,
        IEnumerable<TRow> rows,
        IReadOnlyList<FlareExportColumn<TRow>> columns,
        FlareCsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(download);
        var opt = options ?? FlareCsvOptions.Rfc4180;
        return download.DownloadAsync(fileName, Build(rows, columns, opt), "text/csv", opt.ByteOrderMark);
    }

    /// <summary>
    /// Escapes one cell: neutralizes a formula lead when
    /// <see cref="FlareCsvOptions.GuardFormulas"/> is on, then quotes and doubles inner quotes when the
    /// value contains the delimiter, a quote or a line break.
    /// </summary>
    /// <param name="value">The raw cell text.</param>
    /// <param name="options">The dialect being written; decides the delimiter and the guard.</param>
    /// <returns>The cell as it appears in the file.</returns>
    public static string Escape(string value, FlareCsvOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        value ??= "";
        if (options.GuardFormulas && value.Length > 0 && "=+-@\t\r".Contains(value[0]))
            value = "'" + value;
        return value.Contains(options.Delimiter) || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}
