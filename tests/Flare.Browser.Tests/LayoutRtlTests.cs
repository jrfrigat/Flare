using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A floating layout drawer is pinned to a logical edge, but its slide direction is physical. Under
/// RTL the start drawer has to sit at the right edge and the end drawer at the left, both while open
/// and while sliding off.
/// </summary>
public sealed class LayoutRtlTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/layout-rtl");
        await Expect(page.Locator("#start-drawer")).ToBeVisibleAsync();
        await Expect(page.Locator("#end-drawer")).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        // The drawer width and its top offset come from the theme's token bundle.
        await page.WaitForFunctionAsync(
            "() => parseFloat(getComputedStyle(document.querySelector('#start-drawer')).width) > 0");
        return page;
    }

    [Fact]
    public async Task FloatingDrawersSwapEdgesWithTheWritingDirection()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var width = await page.EvaluateAsync<double>("document.documentElement.clientWidth");

        await page.EvaluateAsync("document.querySelector('#shell').dir = 'ltr'");
        await page.WaitForFunctionAsync(
            """
            w => {
                const start = document.querySelector('#start-drawer').getBoundingClientRect();
                const end = document.querySelector('#end-drawer').getBoundingClientRect();
                return Math.abs(start.left) <= 1 && Math.abs(end.right - w) <= 1;
            }
            """, width);

        await page.EvaluateAsync("document.querySelector('#shell').dir = 'rtl'");
        await page.WaitForFunctionAsync(
            """
            w => {
                const start = document.querySelector('#start-drawer').getBoundingClientRect();
                const end = document.querySelector('#end-drawer').getBoundingClientRect();
                return Math.abs(start.right - w) <= 1 && Math.abs(end.left) <= 1;
            }
            """, width);

        // Both panels keep their width; only the edge changed.
        var widths = await page.EvaluateAsync<double[]>(
            "() => ['#start-drawer', '#end-drawer'].map(s => document.querySelector(s).getBoundingClientRect().width)");
        Assert.True(widths[0] > 0 && Math.Abs(widths[0] - widths[1]) <= 1,
            $"the drawers changed width with the direction: {widths[0]:0.#} / {widths[1]:0.#}");
    }
}
