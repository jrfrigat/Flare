using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridQueryableWiringTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly List<Row> _data =
        Enumerable.Range(1, 12).Select(i => new Row($"Item{i:00}", i)).ToList();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Value");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Value));
        inner.AddAttribute(13, "Sortable", true); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "Name");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Queryable, _data.AsQueryable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void Queryable_PagesServerSide()
        => Assert.Equal(5, RenderGrid().FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);

    [Fact]
    public void Queryable_SortsViaTranslation()
    {
        var cut = RenderGrid();
        // Re-query before each click - a sort reloads via the queryable and re-renders the tree.
        string FirstValue() => cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll("td")[0].TextContent.Trim();

        cut.FindAll($"th.{Css.Classes.DataGrid.ThSortable}").First(t => t.TextContent.Contains("Value")).Click();
        Assert.Equal("1", FirstValue()); // ascending
        cut.FindAll($"th.{Css.Classes.DataGrid.ThSortable}").First(t => t.TextContent.Contains("Value")).Click();
        Assert.Equal("12", FirstValue()); // descending
    }
}
