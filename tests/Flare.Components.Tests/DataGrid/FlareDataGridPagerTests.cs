using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridPagerTests : FlareTestContext
{
    // 20 items, 5 per page -> 4 pages.
    private static readonly string[] _items =
        Enumerable.Range(1, 20).Select(i => $"Item {i:00}").ToArray();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<string>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<string, object?>)(s => s)); inner.CloseComponent();
    };

    [Fact]
    public void BuiltInPager_RendersByDefault()
    {
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
        Assert.Equal(4, cut.Instance.PageCount); // 20 items / 5 per page
    }

    [Fact]
    public async Task GoToPage_ShowsThatPagesRows()
    {
        // Regression: client-side paging must slice the full row set, not the current page slice.
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

        Assert.Contains("Item 01", cut.Find("tbody").TextContent);

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(1)); // second page
        Assert.Contains("Item 06", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 01", cut.Find("tbody").TextContent);

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(3)); // last page (16..20)
        Assert.Contains("Item 20", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 15", cut.Find("tbody").TextContent);
    }

    [Fact]
    public void ShowPagerFalse_SuppressesBuiltInPager()
    {
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.Columns, Cols()));

        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
    }

    [Fact]
    public void FooterContent_RendersFooter_AndPagerResolvesGridAndPaginates()
    {
        RenderFragment footer = fb =>
        {
            // No Grid passed: the pager resolves the enclosing grid via the footer cascade.
            fb.OpenComponent<FlareDataGridPager<string>>(0);
            fb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.FooterContent, footer)
            .Add(x => x.Columns, Cols()));

        // The footer area renders, the built-in pager does not, and the pager lives in the footer.
        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.Footer}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Footer} .{Css.Classes.Pagination.Root}"));

        // Page 1 shows the first five items.
        Assert.Contains("Item 01", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 06", cut.Find("tbody").TextContent);

        // Click the page-2 button inside the footer pager -> grid advances a page.
        var pageTwo = cut.FindAll($".{Css.Classes.DataGrid.Footer} .{Css.Classes.Pagination.Root} button")
            .First(b => b.TextContent.Trim() == "2");
        pageTwo.Click();

        Assert.Contains("Item 06", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 01", cut.Find("tbody").TextContent);
    }

    [Fact]
    public void StandalonePager_WithoutGrid_RendersNothing()
    {
        var cut = Render<FlareDataGridPager<string>>();
        Assert.Empty(cut.FindAll($".{Css.Classes.Pagination.Root}"));
    }

    [Fact]
    public void Pager_OwnRowsPerPageOptions_OverrideGrid()
    {
        // The pager owns its presentation: its RowsPerPageOptions are used, not the grid's.
        RenderFragment footer = fb =>
        {
            fb.OpenComponent<FlareDataGridPager<string>>(0);
            fb.AddAttribute(1, "RowsPerPageOptions", (IReadOnlyList<int>)[5, 10, 25]);
            fb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.RowsPerPageOptions, new[] { 5, 50 }) // grid default - should be ignored by the pager
            .Add(x => x.FooterContent, footer)
            .Add(x => x.Columns, Cols()));

        var opts = cut.FindAll($".{Css.Classes.DataGrid.Footer} .{Css.Classes.Pagination.Root} option")
            .Select(o => o.TextContent.Trim()).ToList();
        Assert.Equal(["5", "10", "25"], opts);
    }
}

// ------------------------------------------------------------------------------
// DataGridPersistence - round-trips grid state through browser localStorage using
// the built-in localStorage.* interop (not a custom JS module export, which is the
// bug this guards against: the old code imported flare-theme.js and called exports
// that never existed, so persistence silently no-op'd / threw JSException).
// ------------------------------------------------------------------------------
