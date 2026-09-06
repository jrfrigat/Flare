using AngleSharp.Dom;
using Flare.Components;
using Flare.Integration.Tests.Screens;

namespace Flare.Integration.Tests;

/// <summary>
/// A grid inside a card inside a tab, driven the way a user drives it. Each component here has its own
/// tests already; what those cannot ask is whether the composition still works - whether the grid's
/// pager is reachable through two containers, whether its state survives a tab switch, and whether the
/// height chain the screen declares arrives unbroken at the grid.
/// </summary>
public class OrdersScreenTests : AppScreenContext
{
    private IRenderedComponent<OrdersScreen> Screen(bool lazy = false) =>
        Render<OrdersScreen>(p => p.Add(x => x.Lazy, lazy));

    private static IElement OpenPanel(IRenderedComponent<OrdersScreen> screen) =>
        screen.FindAll($".{Css.Classes.Tabs.Panel}:not(.{Css.Classes.Tabs.PanelHidden})").Single();

    [Fact]
    public void TheGridIsInsideTheCardInsideTheOpenTab()
    {
        var screen = Screen();

        // Not three separate lookups: one selector that only matches if the nesting is what the screen
        // declares, run against the panel that is actually showing.
        var grid = OpenPanel(screen)
            .QuerySelectorAll($".{Css.Classes.Card.Root} .{Css.Classes.DataGrid.Root}");

        Assert.Single(grid);
        Assert.Contains("ORD-001", screen.Find("tbody").TextContent);
    }

    [Fact]
    public void TheHeightChainReachesTheGridUnbroken()
    {
        // Every link between the shell and the scrolling table declares it spends the height it was
        // given. One link left at auto collapses the chain back to content height - silently, and a
        // collapsed box with overflow clips its rows away rather than showing fewer of them. This is
        // the assertion no per-component test can make: each of them owns one link.
        var screen = Screen();

        Assert.Contains(Css.Classes.Layout.ContentFill,
            screen.Find($".{Css.Classes.Layout.Content}").ClassName, StringComparison.Ordinal);

        foreach (var link in new[]
                 {
                     $".{Css.Classes.Tabs.Root}",
                     $".{Css.Classes.Card.Root}",
                     $".{Css.Classes.DataGrid.Root}",
                 })
            Assert.Contains(Css.Classes.Fill.Root, screen.Find(link).ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void PagingTheNestedGridShowsTheNextRows()
    {
        var screen = Screen();
        Assert.Contains("ORD-001", screen.Find("tbody").TextContent);
        Assert.DoesNotContain("ORD-005", screen.Find("tbody").TextContent);

        // The pager the grid draws for itself, found through the panel and the card rather than by
        // reaching for the component instance: this is the button a user would click.
        var pageTwo = OpenPanel(screen)
            .QuerySelectorAll($".{Css.Classes.Pagination.Btn}")
            .First(b => b.TextContent.Trim() == "2");
        pageTwo.Click();

        Assert.Contains("ORD-005", screen.Find("tbody").TextContent);
        Assert.DoesNotContain("ORD-001", screen.Find("tbody").TextContent);
    }

    [Fact]
    public void SortingTheNestedGridReordersItsRows()
    {
        var screen = Screen();
        Assert.StartsWith("ORD-001", FirstRow(screen), StringComparison.Ordinal);

        // Re-found between the clicks: sorting re-renders the header, and the element from before the
        // first click carries an event handler id the new tree no longer has.
        TotalHeader(screen).Click();  // ascending - already the natural order
        TotalHeader(screen).Click();  // descending - the highest total first

        Assert.StartsWith("ORD-009", FirstRow(screen), StringComparison.Ordinal);
    }

    [Fact]
    public void TheGridKeepsItsPageAcrossATabSwitch()
    {
        // Default (non-lazy) tabs keep every panel built, which is what makes transient state survive:
        // the grid is hidden, not unmounted, so the page the user was on is still the page.
        var screen = Screen();
        OpenPanel(screen).QuerySelectorAll($".{Css.Classes.Pagination.Btn}")
            .First(b => b.TextContent.Trim() == "2").Click();
        Assert.Contains("ORD-005", screen.Find("tbody").TextContent);

        SwitchTo(screen, "New order");
        SwitchTo(screen, "All orders");

        Assert.Contains("ORD-005", screen.Find("tbody").TextContent);
    }

    [Fact]
    public void LazyTabsUnmountTheGridAndBuildItAgainAtPageOne()
    {
        // The other half of the same choice, and the reason it is a parameter: Lazy trades that state
        // for never building the heavy panel at all. The grid must be GONE from the document while the
        // form tab shows - a hidden-but-built grid would still cost what lazy is meant to save.
        var screen = Screen(lazy: true);
        OpenPanel(screen).QuerySelectorAll($".{Css.Classes.Pagination.Btn}")
            .First(b => b.TextContent.Trim() == "2").Click();
        Assert.Contains("ORD-005", screen.Find("tbody").TextContent);

        SwitchTo(screen, "New order");
        Assert.Empty(screen.FindAll($".{Css.Classes.DataGrid.Root}"));

        SwitchTo(screen, "All orders");
        Assert.Contains("ORD-001", screen.Find("tbody").TextContent);
        Assert.DoesNotContain("ORD-005", screen.Find("tbody").TextContent);
    }

    [Fact]
    public void FilteringTheNestedGridNarrowsItsRows()
    {
        // The grid draws its own filter row, and it is reachable through the card and the panel like
        // every other control of its own - narrowing the rows here is one interaction crossing three
        // containers, which is the whole shape this suite is for.
        var screen = Screen();
        Assert.Contains("Borage", screen.Find("tbody").TextContent);

        FilterInput(screen).Change("Aster");

        // Waited for rather than asserted straight away: the grid debounces filter input by 300ms, so a
        // test that reads the rows immediately reads the unfiltered ones and would only pass by accident.
        screen.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Borage", screen.Find("tbody").TextContent);
            Assert.Equal(3, screen.FindAll("tbody tr").Count);
        }, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void TheAppBarsHamburgerClosesTheDrawerBesideIt()
    {
        // Neither component knows the other: the app bar asks the layout, the layout tells the drawer.
        // Rendered alone, both are correct and nothing happens.
        var screen = Screen();
        Assert.Contains(Css.Classes.Layout.DrawerOpenMod,
            screen.Find($".{Css.Classes.Layout.Drawer}").ClassName, StringComparison.Ordinal);

        screen.Find($".{Css.Classes.Layout.AppBarToggle}").Click();

        Assert.DoesNotContain(Css.Classes.Layout.DrawerOpenMod,
            screen.Find($".{Css.Classes.Layout.Drawer}").ClassName, StringComparison.Ordinal);
        Assert.Equal("false", screen.Find($".{Css.Classes.Layout.AppBarToggle}").GetAttribute("aria-expanded"));
    }

    private static IElement FilterInput(IRenderedComponent<OrdersScreen> screen) =>
        OpenPanel(screen).QuerySelectorAll($".{Css.Classes.DataGrid.FilterRow} input").First();

    private static IElement TotalHeader(IRenderedComponent<OrdersScreen> screen) =>
        OpenPanel(screen)
            .QuerySelectorAll($"th.{Css.Classes.DataGrid.ThSortable}")
            .First(th => th.TextContent.Contains("Total", StringComparison.Ordinal));

    private static string FirstRow(IRenderedComponent<OrdersScreen> screen) =>
        screen.Find("tbody tr").TextContent;

    private static void SwitchTo(IRenderedComponent<OrdersScreen> screen, string label) =>
        screen.FindAll($".{Css.Classes.Tabs.TabButton}")
            .First(b => b.TextContent.Contains(label, StringComparison.Ordinal))
            .Click();
}
