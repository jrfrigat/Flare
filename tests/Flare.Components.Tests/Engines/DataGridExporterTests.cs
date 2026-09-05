using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class DataGridExporterTests
{
    private sealed record Row(string Name, int Score);

    // Captures whatever an exporter sends to the download service.
    private sealed class CaptureDownload : IFlareDownload
    {
        public string? FileName; public string? Text; public byte[]? Bytes; public string? Mime;
        public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false)
        { FileName = filename; Text = content; Mime = mimeType; return ValueTask.CompletedTask; }
        public ValueTask DownloadCsvAsync(string filename, string csv)
        { FileName = filename; Text = csv; Mime = "text/csv"; return ValueTask.CompletedTask; }
        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null)
        { FileName = filename; Bytes = bytes; Mime = mimeType; return ValueTask.CompletedTask; }
    }

    private static DataGridExportData<Row> SampleData() => new()
    {
        Columns =
        [
            new("Name", r => r.Name),
            new("Score", r => r.Score),
        ],
        Rows = [new Row("Alice, A", 92), new Row("Bob", 78)],
        FileName = "people",
    };

    [Fact]
    public async Task Csv_WritesHeaderRowsAndQuotesCommas()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Csv<Row>().ExportAsync(SampleData(), dl);
        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("Name,Score", dl.Text);
        Assert.Contains("\"Alice, A\"", dl.Text); // comma forces quoting
        Assert.Contains("92", dl.Text);
    }

    [Fact]
    public async Task Tsv_And_Json_UseProperExtensions()
    {
        var tsv = new CaptureDownload();
        await DataGridExporters.Tsv<Row>().ExportAsync(SampleData(), tsv);
        Assert.Equal("people.tsv", tsv.FileName);
        Assert.Contains("Name\tScore", tsv.Text);

        var json = new CaptureDownload();
        await DataGridExporters.Json<Row>().ExportAsync(SampleData(), json);
        Assert.Equal("people.json", json.FileName);
        Assert.Contains("\"Name\"", json.Text);
    }

    // One row carrying every character that is structural in a text table format.
    private static DataGridExportData<Row> OneRow(string name) => new()
    {
        Columns = [new("Name", r => r.Name), new("Score", r => r.Score)],
        Rows = [new Row(name, 1)],
        FileName = "x",
    };

    private static string[] Lines(string text) =>
        text.TrimEnd('\r', '\n').Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

    // A tab used to add a column and a newline used to add a row: the file changed SHAPE, which is worse
    // than a wrong value because nothing downstream can detect it.
    [Fact]
    public async Task Tsv_EscapesTabsAndNewlines_SoTheRecordShapeSurvives()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Tsv<Row>().ExportAsync(OneRow("a\tb\nc"), dl);

        var lines = Lines(dl.Text!);
        Assert.Equal(2, lines.Length);                 // header + exactly one data row
        Assert.Equal(2, lines[1].Split('\t').Length);  // still two fields
        Assert.Contains("a\\tb\\nc", lines[1]);
    }

    // A backslash has to be escaped first, or "a\tb" typed by a user is indistinguishable from a real tab.
    [Fact]
    public async Task Tsv_EscapesTheBackslashItself()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Tsv<Row>().ExportAsync(OneRow("a\\tb"), dl);
        Assert.Contains("a\\\\tb", Lines(dl.Text!)[1]);
    }

    // TSV opens in the same spreadsheet CSV does, so it needs the same OWASP formula guard.
    [Theory]
    [InlineData("=SUM(A1)")]
    [InlineData("+1")]
    [InlineData("-1+2")]
    [InlineData("@import")]
    public async Task Tsv_PrefixesFormulaLeads_LikeCsv(string value)
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Tsv<Row>().ExportAsync(OneRow(value), dl);
        Assert.Contains("'" + value, dl.Text);
    }

    // GFM gives a table cell one escape (backslash-pipe) and defines a row as a single line.
    [Fact]
    public async Task Markdown_EscapesPipesAndFlattensNewlines_SoTheTableSurvives()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Markdown<Row>().ExportAsync(OneRow("a|b\nc"), dl);

        var lines = Lines(dl.Text!);
        Assert.Equal(3, lines.Length);                             // header, separator, one data row
        Assert.Contains("a\\|b c", lines[2]);
        // Three UNESCAPED pipes delimit two cells; the one inside the value is escaped and does not count.
        Assert.Equal(3, System.Text.RegularExpressions.Regex.Matches(lines[2], @"(?<!\\)\|").Count);
    }

    // Emphasis is content in a markdown export, not structure - over-escaping would ruin the output.
    [Fact]
    public async Task Markdown_LeavesNonStructuralMarkdownAlone()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Markdown<Row>().ExportAsync(OneRow("**total**"), dl);
        Assert.Contains("**total**", Lines(dl.Text!)[2]);
    }

    [Fact]
    public async Task Excel_DownloadsValidXlsxBytes()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Excel<Row>().ExportAsync(SampleData(), dl);
        Assert.Equal("people.xlsx", dl.FileName);
        Assert.NotNull(dl.Bytes);
        Assert.Equal(0x50, dl.Bytes![0]); // "PK" zip magic
        Assert.Equal(0x4B, dl.Bytes![1]);
    }

    [Fact]
    public async Task CustomExporter_IsInvokedWithGridData()
    {
        var custom = new MarkdownExporter();
        var dl = new CaptureDownload();
        await custom.ExportAsync(SampleData(), dl);
        Assert.Equal("people.md", dl.FileName);
        Assert.Contains("| Name | Score |", dl.Text);
    }

    // A minimal third-party-style exporter to prove the contract is open.
    private sealed class MarkdownExporter : IDataGridExporter<Row>
    {
        public string Id => "MD";
        public string Label => "Markdown";
        public FlareIcon? Icon => null;
        public Task ExportAsync(DataGridExportData<Row> data, IFlareDownload download)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.Title)) + " |");
            foreach (var r in data.Rows)
                sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.Value(r))) + " |");
            return download.DownloadAsync(System.IO.Path.ChangeExtension(data.FileName, ".md"), sb.ToString(), "text/markdown").AsTask();
        }
    }
}
