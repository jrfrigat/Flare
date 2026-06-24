using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace Flare.Components;

/// <summary>
/// Minimal OOXML (.xlsx) writer used by <c>FlareDataGrid</c> Excel export. Produces a single-sheet
/// workbook with a bold header row. Numeric-looking cells are written as numbers, everything else as
/// inline strings. Built on <see cref="System.IO.Compression"/> only - no third-party dependency.
/// </summary>
internal static class XlsxWriter
{
    /// <summary>Builds an .xlsx file (as bytes) from a header row and data rows.</summary>
    public static byte[] Write(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows, string sheetName = "Sheet1")
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddEntry(zip, "[Content_Types].xml", ContentTypes);
            AddEntry(zip, "_rels/.rels", RootRels);
            AddEntry(zip, "xl/workbook.xml", Workbook(sheetName));
            AddEntry(zip, "xl/_rels/workbook.xml.rels", WorkbookRels);
            AddEntry(zip, "xl/styles.xml", Styles);
            AddEntry(zip, "xl/worksheets/sheet1.xml", Sheet(headers, rows));
        }
        return ms.ToArray();
    }

    private static void AddEntry(ZipArchive zip, string path, string content)
    {
        var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
        using var w = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        w.Write(content);
    }

    private static string Sheet(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        var sb = new StringBuilder();
        sb.Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""");
        sb.Append("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>""");

        // Header row (style s="1" = bold).
        sb.Append("""<row r="1">""");
        for (var c = 0; c < headers.Count; c++)
            sb.Append(InlineStringCell(Ref(c, 1), headers[c], styleIndex: 1));
        sb.Append("</row>");

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            var rowNum = r + 2;
            sb.Append($"""<row r="{rowNum}">""");
            for (var c = 0; c < row.Count; c++)
                sb.Append(Cell(Ref(c, rowNum), row[c]));
            sb.Append("</row>");
        }

        sb.Append("</sheetData></worksheet>");
        return sb.ToString();
    }

    // Numeric cell only when the value is a canonical invariant number (round-trips exactly);
    // this keeps "007", "+1", "1.50", phone numbers etc. as text. Otherwise an inline string.
    private static string Cell(string reference, string? value)
    {
        if (string.IsNullOrEmpty(value)) return $"<c r=\"{reference}\"/>";
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var num)
            && num.ToString(CultureInfo.InvariantCulture) == value)
            return $"<c r=\"{reference}\"><v>{value}</v></c>";
        return InlineStringCell(reference, value, styleIndex: 0);
    }

    private static string InlineStringCell(string reference, string value, int styleIndex)
    {
        var s = styleIndex > 0 ? $" s=\"{styleIndex}\"" : "";
        return $"<c r=\"{reference}\"{s} t=\"inlineStr\"><is><t xml:space=\"preserve\">{Escape(value)}</t></is></c>";
    }

    // Column index (0-based) + row number -> A1-style reference.
    private static string Ref(int col, int row)
    {
        var name = "";
        var n = col + 1;
        while (n > 0)
        {
            var rem = (n - 1) % 26;
            name = (char)('A' + rem) + name;
            n = (n - 1) / 26;
        }
        return name + row.ToString(CultureInfo.InvariantCulture);
    }

    private static string Escape(string s) => s
        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        .Replace("\"", "&quot;").Replace("'", "&apos;");

    private const string ContentTypes =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>""";

    private const string RootRels =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>""";

    private const string WorkbookRels =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>""";

    // cellXfs index 0 = default, index 1 = bold (font 1).
    private const string Styles =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><name val="Calibri"/></font></fonts><fills count="1"><fill><patternFill patternType="none"/></fill></fills><borders count="1"><border/></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="2"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="0" fontId="1" fillId="0" borderId="0" xfId="0" applyFont="1"/></cellXfs></styleSheet>""";

    private static string Workbook(string sheetName) =>
        $"""<?xml version="1.0" encoding="UTF-8" standalone="yes"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="{Escape(sheetName)}" sheetId="1" r:id="rId1"/></sheets></workbook>""";
}
