using System.Globalization;
using System.Text;

namespace Flare.Components;

/// <summary>
/// Minimal, dependency-free PDF writer used by <c>FlareDataGrid</c> PDF export. Renders a paginated
/// table using the standard Helvetica / Helvetica-Bold fonts (no font embedding), with a header row
/// repeated on every page and light row separators. Text is encoded as Latin-1 (WinAnsi); characters
/// outside that range (e.g. Cyrillic) are replaced with '?' since the standard fonts cannot render
/// them. Built on the BCL only.
/// </summary>
internal static class PdfWriter
{
    private const double PageW = 612, PageH = 792, Margin = 36; // US Letter, 0.5in margins
    private const double FontSize = 9, RowH = 15;

    /// <summary>Builds a PDF (as bytes) from a header row and data rows, with an optional title.</summary>
    public static byte[] Write(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows, string? title = null)
    {
        var cols = Math.Max(1, headers.Count);
        var colW = (PageW - 2 * Margin) / cols;

        var pages = new List<string>();
        var sb = new StringBuilder();
        double y = 0;
        var pageIndex = 0;

        void BeginPage()
        {
            sb = new StringBuilder();
            y = PageH - Margin;
            if (pageIndex == 0 && !string.IsNullOrEmpty(title))
            {
                Text(sb, Margin, y - 12, title!, "F2", 14);
                y -= 26;
            }
            Row(sb, headers, y, colW, "F2");
            y -= RowH;
        }

        BeginPage();
        foreach (var row in rows)
        {
            if (y - RowH < Margin)
            {
                pages.Add(sb.ToString());
                pageIndex++;
                BeginPage();
            }
            Row(sb, row, y, colW, "F1");
            y -= RowH;
        }
        pages.Add(sb.ToString());

        return Assemble(pages);
    }

    // One table row: each cell's (truncated) text, then a light separator line below it.
    private static void Row(StringBuilder sb, IReadOnlyList<string?> cells, double y, double colW, string font)
    {
        for (var i = 0; i < cells.Count; i++)
            Text(sb, Margin + i * colW + 2, y, Truncate(cells[i] ?? "", colW - 4), font, FontSize);
        Line(sb, Margin, y - 3, PageW - Margin, y - 3);
    }

    private static void Text(StringBuilder sb, double x, double y, string text, string font, double size) =>
        sb.Append("BT /").Append(font).Append(' ').Append(N(size)).Append(" Tf ")
          .Append(N(x)).Append(' ').Append(N(y)).Append(" Td (").Append(Escape(text)).Append(") Tj ET\n");

    private static void Line(StringBuilder sb, double x1, double y1, double x2, double y2) =>
        sb.Append("0.5 w 0.8 G ").Append(N(x1)).Append(' ').Append(N(y1)).Append(" m ")
          .Append(N(x2)).Append(' ').Append(N(y2)).Append(" l S\n");

    // Rough Helvetica width estimate (avg ~0.5em) to clip a cell to its column width.
    private static string Truncate(string s, double width)
    {
        s = s.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
        var max = Math.Max(1, (int)(width / (FontSize * 0.5)));
        return s.Length <= max ? s : s[..Math.Max(1, max - 2)] + "..";
    }

    // Escape PDF string delimiters and drop characters the standard fonts cannot render.
    private static string Escape(string s)
    {
        var sb = new StringBuilder(s.Length + 4);
        foreach (var ch in s)
        {
            var c = ch <= 0xFF ? ch : '?';
            if (c is '\\' or '(' or ')') sb.Append('\\');
            sb.Append(c);
        }
        return sb.ToString();
    }

    private static string N(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);

    private static byte[] Assemble(List<string> pages)
    {
        var objs = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            $"<< /Type /Pages /Kids [{string.Join(" ", Enumerable.Range(0, pages.Count).Select(p => $"{5 + 2 * p} 0 R"))}] /Count {pages.Count} >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>",
        };
        for (var p = 0; p < pages.Count; p++)
        {
            objs.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageW:0} {PageH:0}] " +
                     $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {6 + 2 * p} 0 R >>");
            var content = pages[p];
            objs.Add($"<< /Length {Encoding.Latin1.GetByteCount(content)} >>\nstream\n{content}endstream");
        }

        var ms = new MemoryStream();
        void W(string s) { var b = Encoding.Latin1.GetBytes(s); ms.Write(b, 0, b.Length); }

        W("%PDF-1.4\n%\u00E2\u00E3\u00CF\u00D3\n"); // high bytes mark the file as binary
        var offsets = new long[objs.Count + 1];
        for (var i = 0; i < objs.Count; i++)
        {
            offsets[i + 1] = ms.Length;
            W($"{i + 1} 0 obj\n{objs[i]}\nendobj\n");
        }

        var xref = ms.Length;
        W($"xref\n0 {objs.Count + 1}\n0000000000 65535 f \n");
        for (var i = 1; i <= objs.Count; i++)
            W(offsets[i].ToString("0000000000", CultureInfo.InvariantCulture) + " 00000 n \n");
        W($"trailer\n<< /Size {objs.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");

        return ms.ToArray();
    }
}
