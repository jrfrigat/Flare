using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// FlareTabs.Collapsible: the bar never scrolls and the tabs that do not fit fold into the trailing
/// overflow menu, while the active tab stays in the visible window. With the switch off the bar is the
/// scrolling strip it always was.
/// </summary>
public sealed class TabsCollapsibleTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task CollapsibleFoldsTheOverflowIntoTheMenu()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 },
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        var more = page.Locator($"#collapsible .{Css.Classes.Tabs.More}");
        await Expect(more).ToBeVisibleAsync();
        await Expect(page.Locator($"#collapsible .{Css.Classes.Tabs.Scroll}")).ToHaveCountAsync(0);

        // The fold runs after the first measurement; some tabs must be folded and the active one visible.
        var folded = page.Locator($"#collapsible .{Css.Classes.Tabs.Item}[data-flare-tabs-hidden]");
        await Expect(folded.First).ToHaveCountAsync(1);

        var active = page.Locator("#collapsible [role=tab][aria-selected=true]");
        await Expect(active).ToBeVisibleAsync();
        Assert.Null(await active.GetAttributeAsync("data-flare-tabs-hidden"));

        // The menu lists exactly the folded tabs.
        await more.Locator("button").ClickAsync();
        var overflowTabs = page.Locator($"#collapsible .{Css.Classes.Tabs.OverflowTab}");
        await Expect(overflowTabs.First).ToBeVisibleAsync();
        Assert.Equal(await folded.CountAsync(), await overflowTabs.CountAsync());

        // Choosing a folded tab activates it and brings it back into the bar.
        var target = overflowTabs.Last;
        var targetLabel = await target.InnerTextAsync();
        await target.ClickAsync();
        await Expect(overflowTabs).ToHaveCountAsync(0);

        var nowActive = page.Locator($"#collapsible [role=tab][aria-selected=true]");
        await Expect(nowActive).ToHaveTextAsync(targetLabel);
        await Expect(nowActive).ToBeVisibleAsync();
        Assert.Null(await nowActive.GetAttributeAsync("data-flare-tabs-hidden"));
    }

    [Fact]
    public async Task CollapsibleMenuClosesOnAClickOutsideAContainedBox()
    {
        // The Gallery wraps every demo in FlareLazy (content-visibility: auto), which confines a fixed
        // backdrop to the box. The overflow menu must dismiss from a click outside it regardless.
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 },
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        var section = page.Locator("#contained-collapsible");
        var more = section.Locator($".{Css.Classes.Tabs.More}");
        await Expect(more).ToBeVisibleAsync();

        await more.Locator("button").ClickAsync();
        var menu = section.Locator($".{Css.Classes.Menu.Panel}");
        await Expect(menu).ToBeVisibleAsync();

        await page.Mouse.ClickAsync(5, 5);
        await Expect(menu).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task ScrollingTabsKeepTheArrowsAndGetNoOverflowControl()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 },
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        // Wait for the app to render before measuring; a count assertion on an element that should be
        // absent passes vacuously against an empty page.
        await Expect(page.Locator($"#scrolling .{Css.Classes.Tabs.Bar}")).ToBeVisibleAsync();
        await Expect(page.Locator($"#scrolling .{Css.Classes.Tabs.More}")).ToHaveCountAsync(0);
        Assert.True(await page.EvaluateAsync<bool>(
            """
            () => {
                const bar = document.querySelector('#scrolling .flare-tabs__bar');
                return bar.scrollWidth > bar.clientWidth + 1;
            }
            """));
        await Expect(page.Locator($"#scrolling .{Css.Classes.Tabs.ScrollNext}")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task CollapsibleFoldsVerticallyToo()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 },
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        await Expect(page.Locator($"#vertical .{Css.Classes.Tabs.More}")).ToBeVisibleAsync();
        await Expect(page.Locator($"#vertical .{Css.Classes.Tabs.Item}[data-flare-tabs-hidden]").First)
            .ToHaveCountAsync(1);
    }

    [Fact]
    public async Task TabsThatFitWithoutTheOverflowButtonStayVisible()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        var section = page.Locator("#collapsible");
        var more = section.Locator($".{Css.Classes.Tabs.More}");
        await Expect(more).ToBeVisibleAsync();
        await SetTabWidthsAsync(section, 27);

        await Expect(more).ToBeHiddenAsync();
        await Expect(section.Locator($".{Css.Classes.Tabs.Item}[data-flare-tabs-hidden]")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task OverflowWindowFitsTheWidthsAroundTheSelectedTab()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-collapsible");

        var section = page.Locator("#collapsible");
        await Expect(section.Locator($".{Css.Classes.Tabs.More}")).ToBeVisibleAsync();
        await SetTabWidthsAsync(section, 30, 160);
        await section.Locator("[role=tab][aria-selected=true]").PressAsync("End");

        var selected = section.Locator("[role=tab][aria-selected=true]");
        await Expect(selected).ToHaveTextAsync("Accessories");
        await Expect(selected).ToBeVisibleAsync();
        Assert.True(await section.Locator("[role=tablist]").EvaluateAsync<bool>(
            "bar => bar.scrollWidth <= bar.clientWidth + 1"));
    }

    private static Task SetTabWidthsAsync(ILocator section, int width, int? lastWidth = null) =>
        section.EvaluateAsync(
            """
            (section, { width, lastWidth }) => {
                const bar = section.querySelector('[role=tablist]');
                bar.style.gap = '0';
                const items = [...bar.children];
                items.forEach((item, index) => {
                    const basis = index === items.length - 1 ? (lastWidth ?? width) : width;
                    item.style.cssText = `flex: 0 0 ${basis}px; min-inline-size: 0; overflow: hidden`;
                });
                section.querySelector('.flare-tabs__more').style.inlineSize = '36px';
                section.style.inlineSize = '240px';
            }
            """, new { width, lastWidth });
}
