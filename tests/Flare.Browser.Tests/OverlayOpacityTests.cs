using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Opacity belongs to the decorative scrim: the content stays fully opaque and interactive while the
/// backdrop dims, including at Opacity=0.
/// </summary>
public sealed class OverlayOpacityTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task OverlayOpacity_DimsOnlyTheBackdrop()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/overlay-opacity");
        await page.WaitForFunctionAsync(
            "() => getComputedStyle(document.documentElement).getPropertyValue('--flare-color-scrim').trim() !== ''");

        var root = page.Locator(".flare-overlay");
        var scrim = page.Locator(".flare-overlay__scrim");
        var content = page.Locator("#content");
        await Expect(scrim).ToHaveCSSAsync("opacity", "0.2");

        Assert.Equal("1", await root.EvaluateAsync<string>("el => getComputedStyle(el).opacity"));
        Assert.Equal("1", await content.EvaluateAsync<string>("el => getComputedStyle(el).opacity"));
        Assert.NotEqual("rgba(0, 0, 0, 0)", await scrim.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor"));

        await page.Locator("#opacity-0").ClickAsync();
        await Expect(scrim).ToHaveCSSAsync("opacity", "0");
        Assert.Equal("1", await content.EvaluateAsync<string>("el => getComputedStyle(el).opacity"));

        await page.Locator("#toggle-dark").ClickAsync();
        await Expect(scrim).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");
    }
}
