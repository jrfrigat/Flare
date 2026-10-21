using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The smallest skeleton draws: a Rect with no sizes takes the themed default height and the container's
/// width, an explicit size still wins, and the default is a token the consumer can override.
/// </summary>
public sealed class SkeletonSizeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task DefaultRect_IsVisibleAndSizesStayUnderTheCaller()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/skeleton-size");

        var skeleton = page.Locator("#default");
        await Expect(skeleton).ToHaveCSSAsync("height", "16px");
        Assert.Equal("200px", await skeleton.EvaluateAsync<string>("el => getComputedStyle(el).width"));

        await Expect(page.Locator("#height-only")).ToHaveCSSAsync("height", "40px");
        Assert.Equal("200px", await page.Locator("#height-only").EvaluateAsync<string>("el => getComputedStyle(el).width"));

        await Expect(page.Locator("#width-only")).ToHaveCSSAsync("width", "80px");
        await Expect(page.Locator("#width-only")).ToHaveCSSAsync("height", "16px");

        await Expect(page.Locator("#text")).ToHaveCSSAsync("height", "16px");
        await Expect(page.Locator("#circle")).ToHaveCSSAsync("width", "40px");
        await Expect(page.Locator("#circle")).ToHaveCSSAsync("height", "40px");

        await Expect(page.Locator("#themed")).ToHaveCSSAsync("height", "32px");
    }
}
