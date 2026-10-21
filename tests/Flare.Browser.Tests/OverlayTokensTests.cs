using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The overlay's layer and backdrop density come from the theme: the default matches the token, a
/// consumer can override the token, and an explicit ZIndex or Opacity still wins over both.
/// </summary>
public sealed class OverlayTokensTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task LayerAndOpacity_ComeFromTheThemeUnlessSet()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/overlay-tokens");

        await Expect(page.Locator("#default")).ToHaveCSSAsync("z-index", "1200");
        await Expect(ScrimOf(page, "#default")).ToHaveCSSAsync("opacity", "0.5");

        await Expect(page.Locator("#themed")).ToHaveCSSAsync("z-index", "1600");
        await Expect(ScrimOf(page, "#themed")).ToHaveCSSAsync("opacity", "0.2");

        await Expect(page.Locator("#explicit")).ToHaveCSSAsync("z-index", "9999");
        await Expect(ScrimOf(page, "#explicit")).ToHaveCSSAsync("opacity", "0.8");
    }

    private static ILocator ScrimOf(IPage page, string rootSelector)
        => page.Locator($"{rootSelector} > .{Css.Classes.Overlay.Scrim}");
}
