using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridExportTests : FlareTestContext
{
    private record Row(string Name, decimal Amount, DateTime Date, bool Active);

    private static readonly Row[] _rows =
    [
        new("Alice", 1234.5m, new DateTime(2026, 6, 21, 9, 0, 0), true),
        new("Bob",   50m,     new DateTime(2026, 1, 2, 0, 0, 0),  false),
    ];

    private sealed class CapturingDownload : IFlareDownload
    {
        public string? Content;
        public string? FileName;
        public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false)
        { FileName = filename; Content = content; return default; }
        public ValueTask DownloadCsvAsync(string filename, string csv)
        { FileName = filename; Content = csv; return default; }
        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null)
        { FileName = filename; Content = System.Text.Encoding.UTF8.GetString(bytes); return default; }
    }

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Name");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Amount");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Amount)); inner.AddAttribute(s++, "Format", "N2"); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Date");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Date)); inner.AddAttribute(s++, "Type", ColumnDataType.Date); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Active");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Active)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        return Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, Cols()));
    }

    [Fact]
    public void GetExportData_AppliesTypeAwareText()
    {
        var data = RenderGrid().Instance.GetExportData("people");

        Assert.Equal("people", data.FileName);
        Assert.Equal(4, data.Columns.Count);
        Assert.Equal(2, data.Rows.Count);
        Assert.Equal("1,234.50", data.Columns.First(c => c.Title == "Amount").TextOf(_rows[0])); // N2
        Assert.Equal("06/21/2026", data.Columns.First(c => c.Title == "Date").TextOf(_rows[0])); // date only
        Assert.Equal("true", data.Columns.First(c => c.Title == "Active").TextOf(_rows[0]));      // bool -> text
    }

    [Fact]
    public async Task CsvExport_WritesFormattedValues()
    {
        var data = RenderGrid().Instance.GetExportData("people");
        var dl = new CapturingDownload();
        await new CsvGridExporter<Row>().ExportAsync(data, dl);

        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("1,234.50", dl.Content);   // formatted, not "1234.5"
        Assert.Contains("06/21/2026", dl.Content);  // date only, no time
        Assert.Contains("true", dl.Content);
    }

    [Fact]
    public void Export_InToolbarContent_ResolvesGridFromCascade()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var dl = new CapturingDownload();
        Services.AddScoped<IFlareDownload>(_ => dl);

        // No Grid is passed: the export resolves the enclosing grid via the toolbar cascade.
        RenderFragment toolbar = tb =>
        {
            tb.OpenComponent<DataGridExport<Row>>(0);
            tb.AddAttribute(1, "FileName", "people");
            tb.AddAttribute(2, "Exporters",
                (IReadOnlyList<IDataGridExporter<Row>>)[DataGridExporters.Csv<Row>()]);
            tb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.ToolbarContent, toolbar)
            .Add(x => x.Columns, Cols()));

        cut.Find($".{Css.Classes.DataGrid.Toolbar} button").Click();

        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("Alice", dl.Content);
        Assert.Contains("1,234.50", dl.Content); // grid's N2 format applied via the resolved grid
    }

    [Fact]
    public void Export_Split_RendersSplitButton_PrimaryExportsFirstExporter()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var dl = new CapturingDownload();
        Services.AddScoped<IFlareDownload>(_ => dl);

        RenderFragment toolbar = tb =>
        {
            tb.OpenComponent<DataGridExport<Row>>(0);
            tb.AddAttribute(1, "FileName", "people");
            tb.AddAttribute(2, "Split", true);
            tb.AddAttribute(3, "Exporters",
                (IReadOnlyList<IDataGridExporter<Row>>)[DataGridExporters.Csv<Row>(), DataGridExporters.Json<Row>()]);
            tb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.ToolbarContent, toolbar)
            .Add(x => x.Columns, Cols()));

        // Split layout: one split-button host with a primary action, not a button per exporter.
        Assert.Single(cut.FindAll($".{Css.Classes.SplitButton.Root}"));
        var primary = cut.Find($".{Css.Classes.SplitButton.Main}");
        Assert.Contains("CSV", primary.TextContent); // first exporter is the primary action

        primary.Click();

        Assert.Equal("people.csv", dl.FileName); // primary runs the first (CSV) exporter
        Assert.Contains("Alice", dl.Content);
    }
}
