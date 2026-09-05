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
        Assert.DoesNotContain(Css.Classes.Layout.ContentFill, plain.Find("main").ClassName, StringComparison.Ordinal);

        var filled = Render<FlareLayoutContent>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "page"))));
        Assert.Contains(Css.Classes.Layout.ContentFill, filled.Find("main").ClassName, StringComparison.Ordinal);
        Assert.NotEmpty(filled.FindAll($".{Css.Classes.Layout.ContentFrame}"));
    }

    [Fact]
    public void Tabs_FillHeight_MarksTheRoot()
    {
        var plain = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("One")));
        Assert.DoesNotContain(Css.Classes.Tabs.Fill, plain.Find($".{Css.Classes.Tabs.Root}").ClassName, StringComparison.Ordinal);

        var filled = Render<FlareTabs>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, OneTab("One")));
        Assert.Contains(Css.Classes.Tabs.Fill, filled.Find($".{Css.Classes.Tabs.Root}").ClassName, StringComparison.Ordinal);
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
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
    }

    [Fact]
    public void DataGrid_PageSize_BringsThePagerBack()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(30))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(10, cut.FindAll("tbody tr").Count);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
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

        var root = cut.Find($".{Css.Classes.DataGrid.Root}");
        Assert.Contains(Css.Classes.DataGrid.Bounded, root.ClassName, StringComparison.Ordinal);
        Assert.Contains("--_flare-datagrid-height:320px", root.GetAttribute("style"), StringComparison.Ordinal);
        Assert.DoesNotContain("--_flare-datagrid-height", cut.Find($".{Css.Classes.DataGrid.Wrapper}").GetAttribute("style") ?? "", StringComparison.Ordinal);
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

        var root = cut.Find($".{Css.Classes.DataGrid.Root}");
        Assert.Contains(Css.Classes.DataGrid.Sized, root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain(Css.Classes.DataGrid.Bounded, root.ClassName, StringComparison.Ordinal);
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

        var root = cut.Find($".{Css.Classes.DataGrid.Root}");
        Assert.DoesNotContain(Css.Classes.DataGrid.Bounded, root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain(Css.Classes.DataGrid.Sized, root.ClassName, StringComparison.Ordinal);
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

        var root = cut.Find($".{Css.Classes.DataGrid.Root}");
        Assert.Contains(Css.Classes.DataGrid.Fill, root.ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain(Css.Classes.DataGrid.Bounded, root.ClassName, StringComparison.Ordinal);
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

        var style = cut.Find($".{Css.Classes.DataGrid.Root}").GetAttribute("style");
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
        Assert.Contains(Css.Classes.DataGrid.WrapperStickyHead, sticky.Find($".{Css.Classes.DataGrid.Wrapper}").ClassName, StringComparison.Ordinal);

        var loose = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5))
            .Add(x => x.StickyHeader, false)
            .Add(x => x.Columns, OneColumn()));
        Assert.DoesNotContain("sticky-head", loose.Find($".{Css.Classes.DataGrid.Wrapper}").ClassName, StringComparison.Ordinal);
    }

    // Virtualization recycles the rows of whatever set it is given; it does not decide what that set is.
    // With a page size the set is the page, and the pager stays - the two answer different questions.
    //
    // The virtual path is asserted through the spacer rows Virtualize emits around its window, not
    // through how many rows it decided to render: that number is the framework's business and it differs
    // by target framework - net8's Virtualize renders the whole set in a headless host, where net9 and
    // net10 render a window. A count assertion passed here on two targets and failed on the third for a
    // reason that had nothing to do with the behaviour under test.
    [Fact]
    public void DataGrid_Virtual_ComposesWithPaging()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(500))
            .Add(x => x.PageSize, 10)
            .Add(x => x.Virtual, true)
            .Add(x => x.Columns, OneColumn()));

        Assert.True(cut.FindAll("tbody tr").Count > cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count,
            "The virtual path renders Virtualize's spacer rows around its window; an unvirtualized grid "
            + "renders nothing but data rows. Only spacers tell the two apart without depending on how "
            + "many rows Virtualize chose to build.");
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
        Assert.Equal(10, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
        // Recycling changes how many rows exist, so the columns must stop being measured from them.
        Assert.Contains(Css.Classes.DataGrid.TableFixed, cut.Find("table").ClassName, StringComparison.Ordinal);
    }

    // Recycling no longer changes anything but the number of rows in the DOM, so the grid can decide for
    // itself. It only decides YES with a height to scroll in - Virtualize derives its window from a
    // scroll container, and an unbounded one has no extent - and only for a set worth recycling. Both
    // guards matter: switching it on where it cannot work is the 0.26.2 bug, arrived at by accident.
    [Theory]
    [InlineData(600, "400px", null, true)]   // large set, a height: recycle
    [InlineData(600, null, null, false)]     // no height: nothing to scroll in, so no window to compute
    [InlineData(100, "400px", null, false)]  // small set: every row in the DOM is cheaper than the machinery
    [InlineData(600, "400px", false, false)] // an explicit no is a no
    [InlineData(600, "400px", true, true)]   // an explicit yes is a yes
    public void DataGrid_DecidesWhetherToRecycleRows(int rows, string? height, bool? @virtual, bool expected)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(rows))
            .Add(x => x.Height, height)
            .Add(x => x.Virtual, @virtual)
            .Add(x => x.Columns, OneColumn()));

        var recycling = cut.FindAll("tbody tr").Count > cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count;
        Assert.Equal(expected, recycling);
    }

    // The decision is made about the rows this render is responsible for, which is the PAGE when there
    // is one - fifty rows on screen is fifty rows, however many pages sit behind them.
    [Fact]
    public void DataGrid_DoesNotAutoRecycle_ASmallPageOfALargeSet()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(5000))
            .Add(x => x.PageSize, 50)
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(cut.FindAll("tbody tr").Count, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
        Assert.Equal(50, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
    }

    [Fact]
    public void DataGrid_WithoutAPageSize_RendersNoSpacerRows()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(500))
            .Add(x => x.Columns, OneColumn()));

        Assert.Equal(cut.FindAll("tbody tr").Count, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
        Assert.Equal(500, cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}").Count);
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
