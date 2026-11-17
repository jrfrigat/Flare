using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The tool panel tab strip never scrolls: at every width the tabs share the row, their labels
/// ellipsize, and once a tab is too narrow for text the icon alone stays while the accessible name
/// survives the clipped label. Both placements are checked; the keyboard/navigation contract is
/// covered by the bUnit suite.
/// </summary>
public sealed class IdeToolPanelFittingTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string TopTabs = "#top-group .flare-toolpanel-group__tabs";
    private const string BottomTabs = "#bottom-group .flare-toolpanel-group__tabs";
    private const string Compact = "data-flare-tab-compact";
    private const string Label = ".flare-toolpanel-group__tab-label";

    private static Task SetLeftWidthAsync(IPage page, string width) =>
        page.EvaluateAsync(
            "width => document.querySelector('#fitting').style.setProperty('--flare-ide-left-width', width)",
            width);

    private static async Task<bool> EventuallyAsync(Func<Task<bool>> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (await condition()) return true;
            await Task.Delay(50);
        }

        return false;
    }

    private static Task<bool> HasCompactAsync(ILocator tab) =>
        tab.EvaluateAsync<bool>($"el => el.hasAttribute('{Compact}')");

    private static Task<bool> IsTruncatedAsync(ILocator tab) =>
        tab.EvaluateAsync<bool>(
            $"el => {{ const l = el.querySelector('{Label}'); return l.scrollWidth > l.clientWidth + 1; }}");

    private static async Task AssertFitsWithoutScrollingAsync(ILocator strip)
    {
        await Expect(strip).ToHaveCSSAsync("overflow-x", "hidden");
        Assert.True(await strip.EvaluateAsync<bool>("el => el.clientWidth > 0 && el.scrollWidth <= el.clientWidth + 1"));
        Assert.Equal(0, await strip.Locator(":scope > button:not([role=\"tab\"])").CountAsync());
    }

    [Fact]
    public async Task TopStrip_NarrowsFromFullTextToEllipsisToIconOnlyWithoutScrolling()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-toolpanel-fitting");
        await page.EvaluateAsync("() => document.fonts.ready.then(() => true)");
        await Expect(page.Locator(TopTabs)).ToBeVisibleAsync();

        var strip = page.Locator(TopTabs);
        var tabs = strip.Locator("[role=tab]");

        await SetLeftWidthAsync(page, "400px");
        await AssertFitsWithoutScrollingAsync(strip);
        Assert.False(await HasCompactAsync(tabs.Nth(0)));
        Assert.False(await IsTruncatedAsync(tabs.Nth(0)));

        await SetLeftWidthAsync(page, "220px");
        Assert.True(await EventuallyAsync(() => IsTruncatedAsync(tabs.Nth(0))),
            "The long tab label should ellipsize before the strip turns icon-only.");
        await AssertFitsWithoutScrollingAsync(strip);
        Assert.False(await HasCompactAsync(tabs.Nth(0)));
        Assert.False(await HasCompactAsync(tabs.Nth(1)));

        await SetLeftWidthAsync(page, "120px");
        Assert.True(await EventuallyAsync(() => HasCompactAsync(tabs.Nth(0))));
        Assert.True(await EventuallyAsync(() => HasCompactAsync(tabs.Nth(1))));
        await AssertFitsWithoutScrollingAsync(strip);

        await Expect(tabs.Nth(0)).ToHaveAccessibleNameAsync("Solution Explorer");
        await Expect(tabs.Nth(1)).ToHaveAccessibleNameAsync("File System");
        await Expect(tabs.Nth(0).Locator("svg")).ToBeVisibleAsync();
        await Expect(tabs.Nth(1).Locator("svg")).ToBeVisibleAsync();
        Assert.True(await tabs.Nth(0).EvaluateAsync<bool>(
            $"el => el.querySelector('{Label}').getBoundingClientRect().width <= 1"));
    }

    [Fact]
    public async Task BottomStrip_FitsAndFallsBackToIconsAtTheNarrowestWidth()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-toolpanel-fitting");
        await page.EvaluateAsync("() => document.fonts.ready.then(() => true)");
        await Expect(page.Locator(BottomTabs)).ToBeVisibleAsync();

        var strip = page.Locator(BottomTabs);
        var tabs = strip.Locator("[role=tab]");
        await AssertFitsWithoutScrollingAsync(strip);
        Assert.False(await HasCompactAsync(tabs.Nth(0)));

        await page.SetViewportSizeAsync(140, 800);

        var share = await strip.EvaluateAsync<double>(
            "el => el.clientWidth / el.querySelectorAll(':scope > [role=\"tab\"]').length");
        Assert.True(await EventuallyAsync(() => HasCompactAsync(tabs.Nth(0))),
            $"Expected icon-only tabs at a {share:0.#}px per-tab share; attributes: "
            + await tabs.Nth(0).EvaluateAsync<string>("el => el.outerHTML"));
        Assert.True(await EventuallyAsync(() => HasCompactAsync(tabs.Nth(1))));
        await AssertFitsWithoutScrollingAsync(strip);
        await Expect(tabs.Nth(0)).ToHaveAccessibleNameAsync("Solution Explorer");
        await Expect(tabs.Nth(1)).ToHaveAccessibleNameAsync("File System");
    }

    [Fact]
    public async Task RtlStrip_KeepsEveryTabInTheRowWithoutScrolling()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-toolpanel-fitting");
        await page.EvaluateAsync("() => document.fonts.ready.then(() => true)");
        await Expect(page.Locator(TopTabs)).ToBeVisibleAsync();
        await page.EvaluateAsync("() => document.querySelector('#fitting').setAttribute('dir', 'rtl')");

        var strip = page.Locator(TopTabs);
        var tabs = strip.Locator("[role=tab]");

        await SetLeftWidthAsync(page, "220px");
        await AssertFitsWithoutScrollingAsync(strip);
        Assert.False(await HasCompactAsync(tabs.Nth(0)));
        Assert.True(await EventuallyAsync(() => IsTruncatedAsync(tabs.Nth(0))));

        await SetLeftWidthAsync(page, "120px");
        Assert.True(await EventuallyAsync(() => HasCompactAsync(tabs.Nth(0))));
        await AssertFitsWithoutScrollingAsync(strip);
        await Expect(tabs.Nth(0)).ToHaveAccessibleNameAsync("Solution Explorer");
        await Expect(tabs.Nth(1)).ToHaveAccessibleNameAsync("File System");
    }
}
