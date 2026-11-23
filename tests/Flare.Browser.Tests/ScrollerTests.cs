using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// FlareScroller is the one overflow owner: a row of buttons and a tab strip both get the same arrows,
/// HideArrows removes only the arrows (scrolling stays), and the dock uses the same strip without a
/// browser scrollbar.
/// </summary>
public sealed class ScrollerTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task ButtonsOverflowIntoArrows()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroller");

        var strip = page.Locator("#scroller-buttons");
        var content = strip.Locator("[data-flare-scroller-bar]");
        await Expect(strip.Locator($".{Css.Classes.Tabs.Scroll}")).ToHaveCountAsync(2);
        Assert.True(await ScrollsAsync(content), "the button row does not overflow");

        await strip.Locator($".{Css.Classes.Tabs.ScrollNext}").ClickAsync();
        await ExpectScrollAsync(content);
        Assert.Equal("none", await content.EvaluateAsync<string>(
            "el => getComputedStyle(el).scrollbarWidth"));
    }

    [Fact]
    public async Task HideArrowsKeepsTheScrolling()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroller");

        var strip = page.Locator("#scroller-hidden");
        var content = strip.Locator("[data-flare-scroller-bar]");
        await Expect(strip.Locator($".{Css.Classes.Tabs.Scroll}")).ToHaveCountAsync(0);
        Assert.True(await ScrollsAsync(content), "the hidden-arrow row does not overflow");

        // The wheel is the scroller module's own gesture; it must still move the row.
        await content.HoverAsync();
        await page.Mouse.WheelAsync(0, 240);
        await ExpectScrollAsync(content);
    }

    [Fact]
    public async Task TabsUseTheSameStripAndArrows()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroller");

        var strip = page.Locator("#scroller-tabs");
        var bar = strip.Locator("[role=tablist]");
        await Expect(strip.Locator($".{Css.Classes.Tabs.Scroll}")).ToHaveCountAsync(2);

        await strip.Locator($".{Css.Classes.Tabs.ScrollNext}").ClickAsync();
        await ExpectScrollAsync(bar);
        Assert.Equal("none", await bar.EvaluateAsync<string>(
            "el => getComputedStyle(el).scrollbarWidth"));
    }

    [Fact]
    public async Task SwitchingPlacementKeepsTheHorizontalArrowsWorking()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroller");

        var strip = page.Locator("#scroller-placement");
        var bar = strip.Locator("[role=tablist]");
        var arrows = strip.Locator($".{Css.Classes.Tabs.Scroll}");
        var toggle = page.Locator("#change-placement");
        await Expect(bar).ToHaveAttributeAsync("aria-orientation", "vertical");
        await Expect(arrows).ToHaveCountAsync(0);

        for (var turn = 0; turn < 2; turn++)
        {
            await toggle.ClickAsync();
            await Expect(bar).ToHaveAttributeAsync("aria-orientation", "horizontal");
            await Expect(arrows).ToHaveCountAsync(2);
            await strip.Locator($".{Css.Classes.Tabs.ScrollNext}").ClickAsync();
            await ExpectScrollAsync(bar);

            await toggle.ClickAsync();
            await Expect(bar).ToHaveAttributeAsync("aria-orientation", "vertical");
            await Expect(arrows).ToHaveCountAsync(0);
        }
    }

    [Fact]
    public async Task DockStripUsesArrowsInsteadOfAScrollbar()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-overflow");

        var strip = page.Locator("#dock-overflow");
        var bar = strip.Locator("[data-flare-dock-tabs]");
        await Expect(strip.Locator($".{Css.Classes.Tabs.Scroll}")).ToHaveCountAsync(2);
        Assert.Equal("none", await bar.EvaluateAsync<string>(
            "el => getComputedStyle(el).scrollbarWidth"));
        Assert.Equal(8, await strip.Locator(".flare-dock__grip").CountAsync());

        await strip.Locator($".{Css.Classes.Tabs.ScrollNext}").ClickAsync();
        await ExpectScrollAsync(bar);
    }

    private static Task<bool> ScrollsAsync(ILocator content) =>
        content.EvaluateAsync<bool>("el => el.scrollWidth > el.clientWidth + 1");

    private static async Task ExpectScrollAsync(ILocator content)
    {
        await content.Page.WaitForFunctionAsync(
            "el => el.scrollLeft > 0",
            await content.ElementHandleAsync());
    }
}
