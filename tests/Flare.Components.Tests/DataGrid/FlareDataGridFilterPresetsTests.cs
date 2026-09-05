using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridFilterPresetsTests : FlareTestContext
{
    private record Row(string Name, string City);
    private static readonly Row[] _data = [new("Alice", "Berlin"), new("Bob", "London"), new("Carol", "Berlin")];

    private static readonly DataGridFilterPreset[] _presets =
    [
        new("Berliners", new DataGridFilterGroup(false, [new DataGridFilter("City", FilterOperator.Equals, "Berlin")], [])),
        new("Londoners", new DataGridFilterGroup(false, [new DataGridFilter("City", FilterOperator.Equals, "London")], [])),
    ];

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "City");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.City)); inner.CloseComponent();
    };

    [Fact]
    public void PresetsComponent_ListsNoFilterPlusPresets()
    {
        var cut = Render<FlareDataGridFilterPresets<Row>>(p => p.Add(x => x.Presets, _presets));
        cut.Find($".{Css.Classes.Select.Control}").Click();
        var options = cut.FindAll($".{Css.Classes.Select.Option}").Select(o => o.TextContent.Trim()).ToList();
        Assert.Contains(options, o => o.Contains("(No filter)"));
        Assert.Contains(options, o => o.Contains("Berliners"));
        Assert.Contains(options, o => o.Contains("Londoners"));
    }

    [Fact]
    public async Task ApplyingPresetFilter_FiltersTheGrid()
    {
        // The preset component applies its DataGridFilterGroup via Grid.ApplyAdvancedFilter.
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, Cols()));

        await cut.InvokeAsync(() => cut.Instance.ApplyAdvancedFilter(_presets[0].Filter)); // Berliners
        var rows = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal(2, rows.Count);
        Assert.All(rows, r => Assert.Contains("Berlin", r.TextContent));
    }
}
