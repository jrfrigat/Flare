using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridPinnedRowsTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data =
        Enumerable.Range(1, 20).Select(i => new Row($"Item {i}", i)).ToArray();
    private static readonly Row[] _top = [new("TOPROW", 0)];
    private static readonly Row[] _bottom = [new("BOTTOMROW", 999)];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Name");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Value");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Value)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.PinnedTopRows, _top)
            .Add(x => x.PinnedBottomRows, _bottom)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void PinnedTop_RendersInThead()
    {
        var cut = RenderGrid();
        var pinned = cut.FindAll($"thead tr.{Css.Classes.DataGrid.PinnedRow}");
        Assert.Single(pinned);
        Assert.Contains("TOPROW", pinned[0].TextContent);
    }

    [Fact]
    public void PinnedBottom_RendersInTfoot()
    {
        var cut = RenderGrid();
        var pinned = cut.FindAll($"tfoot tr.{Css.Classes.DataGrid.PinnedRow}");
        Assert.Single(pinned);
        Assert.Contains("BOTTOMROW", pinned[0].TextContent);
    }

    [Fact]
    public void PinnedRows_AreOutsidePaging()
    {
        var cut = RenderGrid();
        var bodyRows = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal(5, bodyRows.Count); // page size unaffected by the pinned rows
        Assert.DoesNotContain(bodyRows, r => r.TextContent.Contains("TOPROW") || r.TextContent.Contains("BOTTOMROW"));
    }
}
