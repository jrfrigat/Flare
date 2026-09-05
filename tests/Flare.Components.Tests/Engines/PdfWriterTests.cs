using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class PdfWriterTests
{
    private static string Pdf(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows, string? title = null)
        => System.Text.Encoding.Latin1.GetString(PdfWriter.Write(headers, rows, title));

    [Fact]
    public void Write_ProducesValidPdf_WithHeaderAndCellText()
    {
        var text = Pdf(["Name", "Score"],
            [["Alice", "92"], ["Bob", "78"]], title: "people");

        Assert.StartsWith("%PDF-1.4", text);
        Assert.Contains("%%EOF", text);
        Assert.Contains("/BaseFont /Helvetica", text);
        Assert.Contains("(Name)", text);   // header in the content stream
        Assert.Contains("(Alice)", text);  // data cell
        Assert.Contains("(people)", text); // title
    }

    [Fact]
    public void Write_EscapesParensAndDropsNonLatin()
    {
        var bs = ((char)92).ToString();              // a single backslash
        var cyrillic = ((char)0x0429).ToString();    // a Cyrillic letter (outside Latin-1)
        var text = Pdf(["A"], [["x(y) " + cyrillic]]);
        Assert.Contains("x" + bs + "(y" + bs + ") ?", text); // parens escaped, non-Latin -> '?'
    }

    [Fact]
    public void Write_PaginatesManyRows()
    {
        var rows = Enumerable.Range(0, 200)
            .Select(i => (IReadOnlyList<string?>)new[] { $"Row {i}", "v" }).ToList();
        var text = Pdf(["A", "B"], rows);

        var pages = System.Text.RegularExpressions.Regex.Matches(text, "/MediaBox").Count;
        Assert.True(pages > 1, $"expected multiple pages, got {pages}");
    }
}
