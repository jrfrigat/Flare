using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The binary responsive presentation: Table by default, Stacked on demand, and Auto measured from
/// the grid's own width against PresentationBreakpoint. The table stays the DOM in Stacked, so sorting,
/// filtering, selection and virtualization keep the same state and markup.
/// </summary>
public sealed class DataGridPresentationTests : FlareTestContext
{
    private sealed record Person(string Name, string City);

    private static readonly List<Person> People =
    [
        new("Ada", "London"),
        new("Grace", "New York"),
    ];

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Person>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
        b.CloseComponent();

        b.OpenComponent<FlareColumn<Person>>(3);
        b.AddAttribute(4, "Title", "City");
        b.AddAttribute(5, "Field", (Func<Person, object?>)(p => p.City));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Person>> RenderGrid(DataGridPresentation presentation)
        => Render<FlareDataGrid<Person>>(p => p
            .Add(x => x.Items, People)
            .Add(x => x.Presentation, presentation)
            .Add(x => x.Columns, Columns()));

    private void SetupContainerWidth(double width)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-viewport.js");
        module.Setup<BrowserViewportService.ElementRectDto?>("observeElement", _ => true)
            .SetResult(new BrowserViewportService.ElementRectDto(0, 0, width, 0, 1200, 800, 0, 0));
    }

    private static string RootClass(IRenderedComponent<FlareDataGrid<Person>> cut)
        => cut.Find($".{Css.Classes.DataGrid.Root}").ClassName ?? string.Empty;

    [Fact]
    public void Table_IsTheDefaultAndKeepsTheTable()
    {
        var cut = RenderGrid(DataGridPresentation.Table);

        Assert.DoesNotContain(Css.Classes.DataGrid.Stacked, RootClass(cut));
        Assert.NotNull(cut.Find("thead"));
        Assert.Empty(cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row} td[data-label]"));
    }

    [Fact]
    public void Stacked_AddsTheClassAndLabelsEveryDataCell()
    {
        var cut = RenderGrid(DataGridPresentation.Stacked);

        Assert.Contains(Css.Classes.DataGrid.Stacked, RootClass(cut));
        var cells = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row} td[data-label]");
        Assert.Equal(People.Count * 2, cells.Count);
        Assert.Contains(cells, c => c.GetAttribute("data-label") == "Name" && c.TextContent.Contains("Ada"));
        Assert.Contains(cells, c => c.GetAttribute("data-label") == "City" && c.TextContent.Contains("London"));
    }

    [Fact]
    public void Auto_BelowTheTier_SwitchesToStacked()
    {
        SetupContainerWidth(400);
        var cut = RenderGrid(DataGridPresentation.Auto);

        cut.WaitForAssertion(() => Assert.Contains(Css.Classes.DataGrid.Stacked, RootClass(cut)));
    }

    [Fact]
    public void Auto_AtOrAboveTheTier_StaysTable()
    {
        SetupContainerWidth(900);
        var cut = RenderGrid(DataGridPresentation.Auto);

        cut.WaitForAssertion(() => Assert.DoesNotContain(Css.Classes.DataGrid.Stacked, RootClass(cut)));
    }
}
