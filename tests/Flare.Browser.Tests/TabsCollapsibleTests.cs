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
}
