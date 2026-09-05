using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridQuickFilterTests : FlareTestContext
{
    private record Row(string Name, string City);
    private static readonly Row[] _data = [new("Alice", "Berlin"), new("Bob", "London"), new("Carol", "Berlin")];

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "City");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.City)); inner.CloseComponent();
    };

    [Fact]
    public async Task ApplyQuickFilter_MatchesAnyColumn_AndClears()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, Cols()));

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter("berlin")); // City match, case-insensitive
        Assert.Equal(2, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter(null)); // clear
        Assert.Equal(3, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
    }

    [Fact]
    public void QuickFilterComponent_RendersSearchInput()
    {
        var cut = Render<FlareDataGridQuickFilter<Row>>(p => p.Add(x => x.DebounceMs, 0));
        Assert.NotEmpty(cut.FindAll("input"));
    }
}
