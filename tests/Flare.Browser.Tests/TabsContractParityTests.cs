using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The tab item is one contract: FlareTabs and every embedded strip render the same
/// <c>flare-tabs__tab</c> item, active modifier and label element, so a theme styles them without
/// knowing which surface they belong to.
/// </summary>
public sealed class TabsContractParityTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("/tabs-fallback")]
    [InlineData("/ide-toolpanel-group")]
    public async Task EveryTabRendersTheSharedItemContract(string url)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}{url}");

        var tabs = page.Locator("[role=tab]");
        await Expect(tabs.First).ToBeVisibleAsync();
        var count = await tabs.CountAsync();
        Assert.True(count > 0, $"{url} rendered no tabs");
        Assert.Equal(count, await page.Locator($"[role=tab].{Css.Classes.Tabs.TabButton}").CountAsync());
        Assert.Equal(count, await page.Locator($"[role=tab] .{Css.Classes.Tabs.Label}").CountAsync());

        var active = page.Locator("[role=tab][aria-selected=true]");
        await Expect(active).ToHaveCountAsync(1);
        await Expect(active).ToHaveClassAsync(new Regex($@"\b{Regex.Escape(Css.Classes.Tabs.TabActive)}\b"));
    }
}
