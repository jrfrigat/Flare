using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A dock tab is the shared FlareTabs item: the theme's tab language must reach it with no dock-only
/// overrides. Under VisualStudio that is the VS 2026 document-tab look - compact padding, rounded top
/// corners, the flowing active fill and, in dark mode, the 1px #9184EE outline.
/// </summary>
public sealed class DockTabLookTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task DockTabMatchesTheThemeTabLanguage()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-look");

        var dockTab = page.Locator($"#dock-vs [role=tablist] .{Css.Classes.Tabs.TabActive}");
        var referenceTab = page.Locator("#tabs-vs [role=tab][aria-selected=true]");
        await Expect(dockTab).ToBeVisibleAsync();
        await Expect(referenceTab).ToBeVisibleAsync();

        await ExpectSameCssAsync(dockTab, referenceTab, "padding-left");
        await ExpectSameCssAsync(dockTab, referenceTab, "padding-top");
        await ExpectSameCssAsync(dockTab, referenceTab, "border-top-left-radius");
        await ExpectSameCssAsync(dockTab, referenceTab, "background-color");
        await ExpectSameCssAsync(dockTab, referenceTab, "box-shadow");

        var dockBar = page.Locator("#dock-vs [data-flare-dock-tabs]");
        var referenceBar = page.Locator($"#tabs-vs .{Css.Classes.Tabs.Bar}");
        await ExpectSameCssAsync(dockBar, referenceBar, "gap");

        // The measured dark document-tab selection: rounded top corners and the purple outline.
        Assert.Equal("6px", await dockTab.EvaluateAsync<string>(
            "el => getComputedStyle(el).borderTopLeftRadius"));
        Assert.Contains("rgb(145, 132, 238)", await dockTab.EvaluateAsync<string>(
            "el => getComputedStyle(el).boxShadow"));
    }

    private static async Task ExpectSameCssAsync(ILocator a, ILocator b, string property)
    {
        // Theme tokens land after the first paint and the colours transition, so compare until both
        // settle instead of sampling once mid-transition.
        var deadline = DateTime.UtcNow.AddSeconds(5);
        var left = string.Empty;
        var right = string.Empty;
        while (DateTime.UtcNow < deadline)
        {
            left = await a.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
            right = await b.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
            if (left == right)
            {
                return;
            }

            await Task.Delay(50);
        }

        Assert.Equal(right, left);
    }
}
