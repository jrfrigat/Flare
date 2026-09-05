using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridComputedColumnTests : FlareTestContext
{
    private record Person(string First, string Last, int Age);

    // Deliberately out of full-name order so a working sort visibly reorders them.
    private static readonly Person[] _people =
        [new("Carol", "Smith", 40), new("Alice", "Adams", 30), new("Bob", "Smith", 25)];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            // Title is not a property name and the value is computed: the old reflection-based
            // pipeline could not resolve it for sort/filter; the compiled Field selector can.
            inner.AddAttribute(11, "Title", "Full name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => $"{p.First} {p.Last}"));
            inner.AddAttribute(13, "Sortable", true);
            inner.AddAttribute(14, "Filterable", true);
            inner.AddAttribute(15, "FilterDebounceMs", 0); // apply instantly (no debounce timer in the test)
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Filter_ByComputedColumn_Works()
    {
        var cut = Render(Grid());
        cut.Find($".{Css.Classes.DataGrid.FilterRow} .{Css.Classes.Input.Control}").Input("Smith");

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal(2, rows.Count);
        Assert.All(rows, r => Assert.Contains("Smith", r.TextContent));
    }

    [Fact]
    public void Sort_ByComputedColumn_OrdersByAccessor()
    {
        var cut = Render(Grid());
        cut.Find($"th.{Css.Classes.DataGrid.ThSortable}").Click(); // ascending by full name

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal("Alice Adams", rows[0].QuerySelector("td")!.TextContent.Trim());
        Assert.Equal("Bob Smith", rows[1].QuerySelector("td")!.TextContent.Trim());
        Assert.Equal("Carol Smith", rows[2].QuerySelector("td")!.TextContent.Trim());
    }
}
