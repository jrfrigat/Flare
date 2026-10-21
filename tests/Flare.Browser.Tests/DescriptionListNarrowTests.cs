using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A description list fits the container it is given: a fixed label width stays a preference, a long
/// term wraps, the nested list keeps a width, and neither theme nor RTL brings the overflow back.
/// </summary>
public sealed class DescriptionListNarrowTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task NarrowContainer_KeepsTheListInsideItsParent()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/description-list-narrow");
        await Expect(page.Locator("#dl")).ToHaveCSSAsync("display", "grid");

        await AssertFitsAsync(page, "320px");
        await AssertFitsAsync(page, "160px");

        await page.Locator("#md3e").ClickAsync();
        await Expect(page.Locator("[class*='flare-theme-md3-expressive']")).ToHaveCountAsync(1);
        await AssertFitsAsync(page, "160px");

        await page.Locator("#fluent").ClickAsync();
        await Expect(page.Locator("[class*='flare-theme-fluent2']")).ToHaveCountAsync(1);
        await AssertFitsAsync(page, "160px");

        await page.Locator("#rtl").ClickAsync();
        await Expect(page.Locator("[dir='rtl']")).ToHaveCountAsync(1);
        await AssertFitsAsync(page, "160px");

        // The long term wrapped rather than staying one clipped line.
        var label = page.Locator("#row-long > dt");
        var lines = await label.EvaluateAsync<int>(
            "el => { const r = document.createRange(); r.selectNodeContents(el); return r.getClientRects().length; }");
        Assert.True(lines > 1, $"The long label should wrap; it occupies {lines} line box(es).");
    }

    private static async Task AssertFitsAsync(IPage page, string width)
    {
        await page.EvaluateAsync($"() => document.querySelector('#narrow').style.width = '{width}'");

        var dlClient = await page.Locator("#dl").EvaluateAsync<double>("el => el.clientWidth");
        var dlScroll = await page.Locator("#dl").EvaluateAsync<double>("el => el.scrollWidth");
        var detail = await page.EvaluateAsync<string>($@"() => ['#row-long > dt', '#row-long > dd', '#row-nested > dd', '#nested', '#nested > .{Css.Classes.DescriptionList.Row}']
            .map(s => {{ const el = document.querySelector(s); return el ? s + '=' + el.clientWidth + '/' + el.scrollWidth : s + '=none'; }}).join(', ')");
        Assert.True(dlScroll <= dlClient + 1, $"The list overflows at {width}: {dlScroll} > {dlClient}. {detail}");

        var parentScroll = await page.Locator("#narrow").EvaluateAsync<double>("el => el.scrollWidth");
        var parentClient = await page.Locator("#narrow").EvaluateAsync<double>("el => el.clientWidth");
        Assert.True(parentScroll <= parentClient + 1, $"The parent overflows at {width}: {parentScroll} > {parentClient}.");

        Assert.True(await page.Locator("#nested").EvaluateAsync<double>("el => el.clientWidth") > 0,
            "The nested list must keep a width.");
        Assert.True(await page.Locator("#row-nested > dd").EvaluateAsync<double>("el => el.clientWidth") > 0,
            "The value column must keep a width.");
    }
}
