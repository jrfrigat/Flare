using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridA11yTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data =
        Enumerable.Range(1, 12).Select(i => new Row($"N{i:00}", i)).ToArray();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
        inner.AddAttribute(13, "Sortable", true); inner.AddAttribute(14, "Filterable", true);
        inner.AddAttribute(15, "FilterDebounceMs", 0); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void LiveRegion_IsPresent()
    {
        var status = RenderGrid().Find("div[role=status]");
        Assert.Equal("polite", status.GetAttribute("aria-live"));
    }

    [Fact]
    public void Sort_AnnouncesColumnAndDirection()
    {
        var cut = RenderGrid();
        cut.FindAll($"th.{Css.Classes.DataGrid.ThSortable}").First(t => t.TextContent.Contains("Name")).Click();
        var status = cut.Find("div[role=status]").TextContent;
        Assert.Contains("Sorted by Name", status);
        Assert.Contains("ascending", status);
    }

    [Fact]
    public void Filter_AnnouncesResultCount()
    {
        var cut = RenderGrid();
        cut.Find($".{Css.Classes.DataGrid.FilterRow} input").Input("N1"); // N10, N11, N12
        Assert.Contains("3 results", cut.Find("div[role=status]").TextContent);
    }

    [Fact]
    public void DataCells_HaveGridcellRole()
        => Assert.NotEmpty(RenderGrid().FindAll("td[role=gridcell]"));
}
