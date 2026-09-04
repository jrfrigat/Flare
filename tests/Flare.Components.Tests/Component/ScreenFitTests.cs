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

    // Paging is a page size, not a mode. Nothing is set here, so every row is on one page and there is
    // no pager to show - which is what a business grid handed a list is expected to do.
    [Fact]
    public void DataGrid_WithoutAPageSize_RendersEveryRowAndNoPager()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(30, cut.FindAll("tbody tr").Count);
        Assert.Empty(cut.FindAll(".flare-datagrid__pagination"));
    }

    [Fact]
    public void DataGrid_PageSize_BringsThePagerBack()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(10, cut.FindAll("tbody tr").Count);
        Assert.NotEmpty(cut.FindAll(".flare-datagrid__pagination"));
    }

    // The defect this whole model change started from: the height was gated on a mode flag, so a paged
    // grid ignored the parameter it declares a default for. Both shapes must carry the same cap, and it
    // must sit on the ROOT - the toolbar and the pager are inside the budget the page asked for.
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void DataGrid_Height_CapsTheComponentPagedOrNot(int pageSize)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, pageSize)
            .Add(x => x.Height, "320px")
            .Add(x => x.Columns, OneColumn()));

        var root = cut.Find(".flare-datagrid");
        Assert.Contains("flare-datagrid--bounded", root.ClassName, StringComparison.Ordinal);
        Assert.Contains("--_flare-datagrid-height:320px", root.GetAttribute("style"), StringComparison.Ordinal);
        Assert.DoesNotContain("--_flare-datagrid-height", cut.Find(".flare-datagrid__wrapper").GetAttribute("style") ?? "", StringComparison.Ordinal);
    }

    // A percentage cap against a content-sized parent computes to `none`, so a percentage has to size
    // the component instead of capping it. It used to be emitted as max-height and silently do nothing.
    [Fact]
    public void DataGrid_Height_InPercent_SizesRatherThanCaps()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Height, "50%")
            .Add(x => x.Columns, OneColumn()));

        var root = cut.Find(".flare-datagrid");
        Assert.Contains("flare-datagrid--sized", root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-datagrid--bounded", root.ClassName, StringComparison.Ordinal);
        Assert.Contains("--_flare-datagrid-height:50%", root.GetAttribute("style"), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("auto")]
    [InlineData("none")]
    public void DataGrid_Height_Unset_LeavesTheGridToGrow(string? height)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Height, height)
            .Add(x => x.Columns, OneColumn()));

        var root = cut.Find(".flare-datagrid");
        Assert.DoesNotContain("flare-datagrid--bounded", root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-datagrid--sized", root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("--_flare-datagrid-height", root.GetAttribute("style") ?? "", StringComparison.Ordinal);
    }

    [Fact]
    public void DataGrid_FillHeight_ReplacesTheHeightWithTheLayoutsAnswer()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.FillHeight, true)
            .Add(x => x.Height, "320px")
            .Add(x => x.Columns, OneColumn()));

        var root = cut.Find(".flare-datagrid");
        Assert.Contains("flare-datagrid--fill", root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-datagrid--bounded", root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("--_flare-datagrid-height", root.GetAttribute("style") ?? "", StringComparison.Ordinal);
    }

    // The user-supplied Style has to survive the custom property the grid writes next to it.
    [Fact]
    public void DataGrid_Height_KeepsTheCallersOwnStyle()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Height, "320px")
            .Add(x => x.Style, "margin-top:8px")
            .Add(x => x.Columns, OneColumn()));

        var style = cut.Find(".flare-datagrid").GetAttribute("style");
        Assert.Contains("--_flare-datagrid-height:320px", style, StringComparison.Ordinal);
        Assert.Contains("margin-top:8px", style, StringComparison.Ordinal);
    }

    // Sticky is a question of its own - "the grid scrolls" and "the header stays" are different things -
    // so it is a parameter rather than a side effect of the mode, and it is on unless asked otherwise.
    [Fact]
    public void DataGrid_StickyHeader_IsOnByDefaultAndCanBeTurnedOff()
    {
        var sticky = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.Columns, OneColumn()));
        Assert.Contains("flare-datagrid__wrapper--sticky-head", sticky.Find(".flare-datagrid__wrapper").ClassName, StringComparison.Ordinal);

        var loose = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.StickyHeader, false)
            .Add(x => x.Columns, OneColumn()));
        Assert.DoesNotContain("sticky-head", loose.Find(".flare-datagrid__wrapper").ClassName, StringComparison.Ordinal);
    }

    // Virtual still replaces paging - it renders its own window over the whole set - so a page size is
    // ignored and no pager appears. Orthogonal virtualization is the next piece of work, not this one.
    //
    // Asserted through the spacer rows Virtualize emits around its window, not through how many rows it
    // decided to render: that number is the framework's business and it differs by target framework -
    // net8's Virtualize renders the whole set in a headless host, where net9 and net10 render a window.
    // A count assertion passed here on two targets and failed on the third for a reason that had nothing
    // to do with the behaviour under test.
    [Fact]
    public void DataGrid_Virtual_StillReplacesPaging()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(500))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Virtual, true)
            .Add(x => x.Columns, OneColumn()));

        Assert.True(cut.FindAll("tbody tr").Count > cut.FindAll("tbody tr.flare-datagrid__row").Count,
            "The virtual path renders Virtualize's spacer rows around its window; an unpaged grid renders "
            + "nothing but data rows. Only spacers tell the two apart without depending on how many rows "
            + "Virtualize chose to build.");
        Assert.Empty(cut.FindAll(".flare-datagrid__pagination"));
    }

    [Fact]
    public void DataGrid_WithoutAPageSize_RendersNoSpacerRows()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(500))
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(cut.FindAll("tbody tr").Count, cut.FindAll("tbody tr.flare-datagrid__row").Count);
        Assert.Equal(500, cut.FindAll("tbody tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void DataGrid_WithoutAPageSize_NumbersEveryRowFromTheTop()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.Columns, OneColumn()));

        var rows = cut.FindAll("tbody tr");
        Assert.Equal("2", rows[0].GetAttribute("aria-rowindex"));
        Assert.Equal("31", rows[^1].GetAttribute("aria-rowindex"));
    }
}
