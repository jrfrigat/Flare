using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A large MinColumnWidth is a preference, not a licence to overflow: in a 320px card a 30rem minimum
/// has to fall back to the container width instead of producing a 480px track.
/// </summary>
public sealed class GridNarrowTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task AWideMinimumFallsBackToTheNarrowContainer()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/grid-narrow");
        await Expect(page.Locator("#grid")).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");

        var sizes = await page.EvaluateAsync<double[]>(
            """
            () => {
                const grid = document.querySelector('#grid');
                const doc = document.documentElement;
                return [grid.scrollWidth, grid.clientWidth, doc.scrollWidth, doc.clientWidth];
            }
            """);

        Assert.True(sizes[0] <= sizes[1] + 1, $"the grid overflows: {sizes[0]:0.#} > {sizes[1]:0.#}");
        Assert.True(sizes[2] <= sizes[3] + 1, $"the page scrolls horizontally: {sizes[2]:0.#} > {sizes[3]:0.#}");
    }
}
