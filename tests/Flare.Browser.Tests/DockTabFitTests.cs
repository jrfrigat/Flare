using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The dock strip is compact chrome: its tabs fill the header token under every theme, and the core
/// tab bar never becomes a vertical scroll container. FluentUI2 uses an auto tab height and MD3
/// Expressive is the densest case, so both are checked beside the default theme.
/// </summary>
public sealed class DockTabFitTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("dock-md3")]
    [InlineData("dock-fui")]
    [InlineData("dock-md3e")]
    public async Task TabStripHasNoVerticalOverflow(string id)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-fit");

        var bar = page.Locator($"#{id} [role=tablist]");
        await Expect(bar).ToBeVisibleAsync();

        var overflow = await bar.EvaluateAsync<bool>("el => el.scrollHeight > el.clientHeight + 1");
        Assert.False(overflow, $"{id} tab strip overflows vertically");

        var header = page.Locator($"#{id} .flare-dock__header").First;
        var tab = page.Locator($"#{id} [role=tab]").First;
        var headerBox = await header.BoundingBoxAsync();
        var tabBox = await tab.BoundingBoxAsync();
        Assert.NotNull(headerBox);
        Assert.NotNull(tabBox);
        Assert.True(tabBox!.Height <= headerBox!.Height + 1,
            $"{id} tab is {tabBox.Height}px in a {headerBox.Height}px header");
    }
}
