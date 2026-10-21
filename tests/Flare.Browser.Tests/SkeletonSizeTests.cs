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

        // The exact height is the theme's call (the token references its spacing scale), so the fact is
        // that the bare skeleton draws at all, fills the container, and shares the default with a
        // width-only one.
        var skeleton = page.Locator("#default");
        var defaultHeight = await WaitForHeightAsync(page, "#default");
        Assert.True(defaultHeight > 0, "A bare skeleton must draw something.");
        Assert.Equal("200px", await skeleton.EvaluateAsync<string>("el => getComputedStyle(el).width"));

        await Expect(page.Locator("#height-only")).ToHaveCSSAsync("height", "40px");
        Assert.Equal("200px", await page.Locator("#height-only").EvaluateAsync<string>("el => getComputedStyle(el).width"));

        await Expect(page.Locator("#width-only")).ToHaveCSSAsync("width", "80px");
        Assert.Equal(defaultHeight, await WaitForHeightAsync(page, "#width-only"));

        Assert.True(await WaitForHeightAsync(page, "#text") > 0, "The text placeholder must be visible.");
        await Expect(page.Locator("#circle")).ToHaveCSSAsync("width", "40px");
        await Expect(page.Locator("#circle")).ToHaveCSSAsync("height", "40px");

        // An override of the token on an ancestor reaches the placeholder: the height is the token's,
        // not a value baked into the stylesheet.
        await Expect(page.Locator("#themed")).ToHaveCSSAsync("height", "32px");
    }

    private static async Task<double> WaitForHeightAsync(IPage page, string selector)
    {
        await page.WaitForFunctionAsync(
            $"() => {{ const el = document.querySelector('{selector}'); return el !== null && parseFloat(getComputedStyle(el).height) > 0; }}");
        return await page.Locator(selector).EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).height)");
    }
}
