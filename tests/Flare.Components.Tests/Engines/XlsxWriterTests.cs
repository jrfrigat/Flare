using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class XlsxWriterTests
{
    private static string ReadEntry(byte[] xlsx, string path)
    {
        using var ms = new MemoryStream(xlsx);
        using var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var entry = zip.GetEntry(path);
        Assert.NotNull(entry);
        using var r = new StreamReader(entry!.Open());
        return r.ReadToEnd();
    }

    [Fact]
    public void Write_ProducesValidZipWithRequiredParts()
    {
        var bytes = XlsxWriter.Write(["A", "B"], [new string?[] { "1", "x" }]);
        using var ms = new MemoryStream(bytes);
        using var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var names = zip.Entries.Select(e => e.FullName).ToHashSet();
        Assert.Contains("[Content_Types].xml", names);
        Assert.Contains("xl/workbook.xml", names);
        Assert.Contains("xl/worksheets/sheet1.xml", names);
        // xlsx magic bytes "PK"
        Assert.Equal(0x50, bytes[0]);
        Assert.Equal(0x4B, bytes[1]);
    }

    [Fact]
    public void Write_HeadersAndCellsAppearInSheet()
    {
        var bytes = XlsxWriter.Write(["Name", "Score"],
            [new string?[] { "Alice", "92" }, new string?[] { "Bob", "78" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("Name", sheet);
        Assert.Contains("Alice", sheet);
        // canonical number -> numeric <v> cell (no inlineStr)
        Assert.Contains("<v>92</v>", sheet);
    }

    [Fact]
    public void Write_NonCanonicalNumbersStayText()
    {
        // leading zero must not become a numeric cell
        var bytes = XlsxWriter.Write(["Code"], [new string?[] { "007" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("inlineStr", sheet);
        Assert.Contains("007", sheet);
        Assert.DoesNotContain("<v>007</v>", sheet);
    }

    [Fact]
    public void Write_EscapesXmlSpecialChars()
    {
        var bytes = XlsxWriter.Write(["H"], [new string?[] { "a<b>&\"c" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("a&lt;b&gt;&amp;&quot;c", sheet);
    }
}
