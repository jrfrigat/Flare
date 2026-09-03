using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The chain a screen-fit page runs down: the layout content frame has a height, a tab set can spend
/// it, and a grid can fill what is left and scroll inside it. Each link is a parameter, because an
/// application that has to write the same four CSS rules - and know which flex item needs
/// <c>min-height: 0</c> and which panel class means "hidden" - is not using a component library for
/// layout, it is patching one.
/// </summary>
public sealed class ScreenFitTests : FlareTestContext
{
    private sealed record Row(string Name);

    private static RenderFragment OneColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.CloseComponent();
    };

    private static IEnumerable<Row> Rows(int count) =>
        Enumerable.Range(1, count).Select(i => new Row("row " + i));

    private static RenderFragment OneTab(string label) => b =>
    {
        b.OpenComponent<FlareTab>(0);
        b.AddAttribute(1, "Label", label);
        b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddContent(0, "panel")));
        b.CloseComponent();
    };

    // The first link: without it the frame around the page is `auto` tall and every percentage below
    // resolves against nothing.
    [Fact]
    public void LayoutContent_FillHeight_MarksTheContentRegion()
    {
        var plain = Render<FlareLayoutContent>(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "page"))));
        Assert.DoesNotContain("flare-layout-content--fill", plain.Find("main").ClassName, StringComparison.Ordinal);

        var filled = Render<FlareLayoutContent>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "page"))));
        Assert.Contains("flare-layout-content--fill", filled.Find("main").ClassName, StringComparison.Ordinal);
        Assert.NotEmpty(filled.FindAll(".flare-layout__content-frame"));
    }

    [Fact]
    public void Tabs_FillHeight_MarksTheRoot()
    {
        var plain = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("One")));
        Assert.DoesNotContain("flare-tabs--fill", plain.Find(".flare-tabs").ClassName, StringComparison.Ordinal);

        var filled = Render<FlareTabs>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, OneTab("One")));
        Assert.Contains("flare-tabs--fill", filled.Find(".flare-tabs").ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void DataGrid_Scroll_RendersEveryRowAndHidesThePager()
    {
        var paged = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, OneColumn()));
        Assert.Equal(10, paged.FindAll("tbody tr").Count);
        Assert.NotEmpty(paged.FindAll(".flare-datagrid__pagination"));

        var scrolling = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Scroll, true)
            .Add(x => x.Columns, OneColumn()));
        Assert.Equal(30, scrolling.FindAll("tbody tr").Count);
        Assert.Empty(scrolling.FindAll(".flare-datagrid__pagination"));
    }

    [Fact]
    public void DataGrid_Scroll_TakesTheScrollBoxAndTheHeightCap()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Scroll, true)
            .Add(x => x.Height, "320px")
            .Add(x => x.Columns, OneColumn()));

        var wrapper = cut.Find(".flare-datagrid__wrapper");
        Assert.Contains("flare-datagrid__wrapper--scroll", wrapper.ClassName, StringComparison.Ordinal);
        Assert.Contains("--_flare-datagrid-height:320px", wrapper.GetAttribute("style"), StringComparison.Ordinal);
    }

    // Virtual and infinite scroll already replace paging by recycling rows; scroll mode's promise is the
    // opposite - every row in the DOM - so it must not quietly turn one of them off.
    [Fact]
    public void DataGrid_Scroll_YieldsToVirtual()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(500))
            .Add(x => x.Scroll, true)
            .Add(x => x.Virtual, true)
            .Add(x => x.Columns, OneColumn()));

        Assert.True(cut.FindAll("tbody tr").Count < 500);
    }

    [Fact]
    public void DataGrid_FillHeight_ReplacesTheHeightCapWithTheLayoutsAnswer()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Scroll, true)
            .Add(x => x.FillHeight, true)
            .Add(x => x.Height, "320px")
            .Add(x => x.Columns, OneColumn()));

        Assert.Contains("flare-datagrid--fill", cut.Find(".flare-datagrid").ClassName, StringComparison.Ordinal);
        var wrapper = cut.Find(".flare-datagrid__wrapper");
        Assert.Contains("flare-datagrid__wrapper--scroll", wrapper.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("--_flare-datagrid-height", wrapper.GetAttribute("style") ?? "", StringComparison.Ordinal);
    }

    // A paged grid given a height by the layout still needs the sticky header and the scrollbar: its
    // page can be taller than the space it was handed.
    [Fact]
    public void DataGrid_FillHeight_AloneStillMakesAScrollBox()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.FillHeight, true)
            .Add(x => x.Columns, OneColumn()));

        Assert.Contains("flare-datagrid__wrapper--scroll", cut.Find(".flare-datagrid__wrapper").ClassName, StringComparison.Ordinal);
        Assert.NotEmpty(cut.FindAll(".flare-datagrid__pagination"));
    }

    [Fact]
    public void DataGrid_Scroll_NumbersEveryRowFromTheTop()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Scroll, true)
            .Add(x => x.Columns, OneColumn()));

        var rows = cut.FindAll("tbody tr");
        Assert.Equal("2", rows[0].GetAttribute("aria-rowindex"));
        Assert.Equal("31", rows[^1].GetAttribute("aria-rowindex"));
    }
}
