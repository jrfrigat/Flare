using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// RTL fixes: Pagination's chevrons follow the reading direction while its flex order mirrors, and
/// its rows-per-page spacing uses a logical margin.
/// </summary>
public sealed class RtlComponentsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Pagination_ArrowsFollowTheReadingDirection()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/rtl-components");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.documentElement).getPropertyValue('{Css.Tokens.Spacing.S4}').trim() !== ''");

        var rtlPrev = page.Locator("#rtl-pagination button[aria-label='Previous page']");
        var rtlNext = page.Locator("#rtl-pagination button[aria-label='Next page']");
        await Expect(rtlPrev).ToBeVisibleAsync();
        Assert.StartsWith("matrix(-1", await rtlPrev.EvaluateAsync<string>("el => getComputedStyle(el).transform"));
        Assert.Equal("rtl", await page.Locator($"#rtl-pagination .{Css.Classes.Pagination.Root}")
            .EvaluateAsync<string>("el => getComputedStyle(el).direction"));

        var rtlRows = page.Locator($"#rtl-pagination .{Css.Classes.Pagination.Rows}");
        Assert.True(await rtlRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)") > 0);
        Assert.Equal(0, await rtlRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)"));

        var ltrPrev = page.Locator("#ltr-pagination button[aria-label='Previous page']");
        Assert.Equal("none", await ltrPrev.EvaluateAsync<string>("el => getComputedStyle(el).transform"));
        var ltrRows = page.Locator($"#ltr-pagination .{Css.Classes.Pagination.Rows}");
        Assert.True(await ltrRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)") > 0);
    }
}
