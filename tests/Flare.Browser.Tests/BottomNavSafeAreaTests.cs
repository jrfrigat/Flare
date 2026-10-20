using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// BarHeight is the height of the controls, not of the bar including the safe area. The bar has to
/// reserve BarHeight + safe-area outside and keep BarHeight for its content, and the spacer has to
/// reserve exactly the space the pinned bar occupies.
/// </summary>
public sealed class BottomNavSafeAreaTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task TheSafeAreaIsAddedAroundTheBarAndReservedByTheSpacer()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/bottomnav-safe-area");
        await Expect(page.Locator("#nav")).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        await page.WaitForFunctionAsync(
            "() => parseFloat(getComputedStyle(document.querySelector('#nav')).height) > 0");

        var heights = await page.EvaluateAsync<double[]>(
            """
            () => {
                const nav = document.querySelector('#nav');
                const spacer = document.querySelector('.flare-bottom-nav__spacer');
                const item = document.querySelector('#item-home');
                const navRect = nav.getBoundingClientRect();
                return [
                    navRect.height,
                    spacer ? spacer.getBoundingClientRect().height : -1,
                    item.getBoundingClientRect().bottom - navRect.top
                ];
            }
            """);

        // 80px of controls plus the 34px inset the host overrode the token with.
        Assert.Equal(114, heights[0], 1);
        Assert.Equal(114, heights[1], 1);
        // The item ends above the inset: the controls still have their full 80px.
        Assert.True(heights[2] <= 80.5, $"the item ends at {heights[2]:0.#}px, inside the safe area");
    }
}
