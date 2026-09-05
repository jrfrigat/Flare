using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridExcelFilterTests : FlareTestContext
{
    private record Fruit(int Id, string Name);
    private static readonly Fruit[] _items =
        [new(1, "Apple"), new(2, "Banana"), new(3, "Cherry"), new(4, "Banana")];

    private IRenderedComponent<FlareDataGrid<Fruit>> RenderGrid() =>
        Render<FlareDataGrid<Fruit>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.FilterMode, DataGridFilterMode.Menu)
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Fruit>>(10);
                inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Fruit, object?>)(f => f.Name));
                inner.AddAttribute(13, "Filterable", true);
                inner.CloseComponent();
            })));

    [Fact]
    public void Menu_ShowsDistinctValueChecklist()
    {
        var cut = RenderGrid();
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();

        var checks = cut.FindAll($".{Css.Classes.DataGrid.FilterMenuList} .{Css.Classes.Checkbox.Root}");
        // 3 distinct values (Apple/Banana/Cherry) + the "(Select all)" row.
        Assert.Equal(4, checks.Count);
        Assert.Contains(checks, c => c.TextContent.Contains("Apple"));
        Assert.Contains(checks, c => c.TextContent.Contains("Banana"));
        Assert.Contains(checks, c => c.TextContent.Contains("Cherry"));
    }

    [Fact]
    public void Uncheck_Value_AppliesInFilter_ExcludingIt()
    {
        var cut = RenderGrid();
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();

        var banana = cut.FindAll($".{Css.Classes.DataGrid.FilterMenuList} .{Css.Classes.Checkbox.Root}")
            .First(c => c.TextContent.Contains("Banana"));
        banana.QuerySelector("input")!.Change(false); // uncheck Banana

        cut.FindAll($".{Css.Classes.DataGrid.FilterMenuActions} button")[^1].Click(); // Apply

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal(2, rows.Count); // Apple, Cherry (both Bananas excluded)
        Assert.DoesNotContain(rows, r => r.TextContent.Contains("Banana"));
        Assert.Single(cut.FindAll($"button.{Css.Classes.DataGrid.FilterTriggerActive}"));
    }

    [Fact]
    public void Search_FiltersTheChecklist()
    {
        var cut = RenderGrid();
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        cut.Find($".{Css.Classes.DataGrid.FilterMenu} .{Css.Classes.Input.Control}").Input("ban");

        var values = cut.FindAll($".{Css.Classes.DataGrid.FilterMenuList} .{Css.Classes.Checkbox.Root}")
            .Where(c => !c.TextContent.Contains("Select all")).ToList();
        Assert.Single(values);
        Assert.Contains("Banana", values[0].TextContent);
    }
}
