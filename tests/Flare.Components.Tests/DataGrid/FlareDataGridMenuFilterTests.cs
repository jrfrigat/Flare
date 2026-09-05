using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridMenuFilterTests : FlareTestContext
{
    private record Fruit(string Name);

    private static readonly Fruit[] _items = [new("Apple"), new("Banana"), new("Cherry")];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Fruit>>(0);
        b.AddAttribute(1, "Items", _items.AsEnumerable());
        b.AddAttribute(2, "FilterMode", DataGridFilterMode.Menu);
        b.AddAttribute(3, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Fruit>>(10);
            inner.AddAttribute(11, "Title", "Name"); // key "Name" resolves the property for the pipeline
            inner.AddAttribute(12, "Field", (Func<Fruit, object?>)(f => f.Name));
            inner.AddAttribute(13, "Sortable", true);
            inner.AddAttribute(14, "Filterable", true);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void MenuMode_RendersTriggerButton_NotInlineFilterRow()
    {
        var cut = Render(Grid());
        Assert.Single(cut.FindAll($"button.{Css.Classes.DataGrid.FilterTrigger}"));
        // Menu mode must not also render the always-on inline filter row.
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.FilterRow}"));
    }

    [Fact]
    public void Trigger_TogglesPanel()
    {
        var cut = Render(Grid());
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.FilterMenu}"));
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.FilterMenu}"));
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.FilterMenu}"));
    }

    [Fact]
    public void ApplyFilter_FiltersRows_AndMarksTriggerActive()
    {
        var cut = Render(Grid());
        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click(); // open

        // Fields in order: [0] search box, [1] operator select, [2] condition value input.
        var valueField = cut.FindAll($".{Css.Classes.DataGrid.FilterMenuField}")[2];
        valueField.QuerySelector("input")!.Input("Ban"); // default operator = contains; all values stay checked

        // Apply is the filled button in the actions row.
        cut.FindAll($".{Css.Classes.DataGrid.FilterMenuActions} button")[^1].Click();

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Single(rows);
        Assert.Contains("Banana", rows[0].TextContent);
        Assert.Single(cut.FindAll($"button.{Css.Classes.DataGrid.FilterTriggerActive}"));
    }
}
