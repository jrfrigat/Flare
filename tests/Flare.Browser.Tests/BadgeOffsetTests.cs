using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Every corner has to read the theme's offset tokens rather than carry its own literals: overriding
/// the tokens on the badge must move the indicator in all four origins, for counts and dots alike.
/// </summary>
public sealed class BadgeOffsetTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const double CountOffset = 17;
    private const double DotOffset = 9;

    // [root.left, root.top, root.right, root.bottom, indicator.left, indicator.top, indicator.right, indicator.bottom]
    private const string RectsScript = """
        el => {
            const root = el.getBoundingClientRect();
            const ind = el.querySelector(':scope > span[aria-hidden], :scope > span[aria-label]').getBoundingClientRect();
            return [root.left, root.top, root.right, root.bottom, ind.left, ind.top, ind.right, ind.bottom];
        }
        """;

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/badge-offset");
        await Expect(page.Locator("#count-top-right")).ToBeVisibleAsync();
        return page;
    }

    private static async Task<double[]> RectsAsync(IPage page, string id)
        => await page.Locator($"#{id}").EvaluateAsync<double[]>(RectsScript);

    private static void AssertNear(double expected, double actual, string what)
        => Assert.True(Math.Abs(expected - actual) <= 1.5, $"{what}: expected {expected:0.##}, got {actual:0.##}");

    [Theory]
    [InlineData("top-left", "TopLeft")]
    [InlineData("top-right", "TopRight")]
    [InlineData("bottom-left", "BottomLeft")]
    [InlineData("bottom-right", "BottomRight")]
    public async Task TheCountIndicatorUsesTheOffsetTokenInEveryCorner(string id, string corner)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var r = await RectsAsync(page, $"count-{id}");

        switch (corner)
        {
            case "TopLeft":
                AssertNear(CountOffset, r[0] - r[4], $"{corner} inline offset");
                AssertNear(CountOffset, r[1] - r[5], $"{corner} block offset");
                break;
            case "TopRight":
                AssertNear(CountOffset, r[6] - r[2], $"{corner} inline offset");
                AssertNear(CountOffset, r[1] - r[5], $"{corner} block offset");
                break;
            case "BottomLeft":
                AssertNear(CountOffset, r[0] - r[4], $"{corner} inline offset");
                AssertNear(CountOffset, r[7] - r[3], $"{corner} block offset");
                break;
            default:
                AssertNear(CountOffset, r[6] - r[2], $"{corner} inline offset");
                AssertNear(CountOffset, r[7] - r[3], $"{corner} block offset");
                break;
        }
    }

    // A dot's offset is inward: the dot sits that far inside the corner rather than outside the box.
    [Theory]
    [InlineData("top-left", "TopLeft")]
    [InlineData("top-right", "TopRight")]
    [InlineData("bottom-left", "BottomLeft")]
    [InlineData("bottom-right", "BottomRight")]
    public async Task TheDotIndicatorUsesTheDotOffsetTokenInEveryCorner(string id, string corner)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var r = await RectsAsync(page, $"dot-{id}");

        switch (corner)
        {
            case "TopLeft":
                AssertNear(DotOffset, r[4] - r[0], $"{corner} inline offset");
                AssertNear(DotOffset, r[5] - r[1], $"{corner} block offset");
                break;
            case "TopRight":
                AssertNear(DotOffset, r[2] - r[6], $"{corner} inline offset");
                AssertNear(DotOffset, r[5] - r[1], $"{corner} block offset");
                break;
            case "BottomLeft":
                AssertNear(DotOffset, r[4] - r[0], $"{corner} inline offset");
                AssertNear(DotOffset, r[3] - r[7], $"{corner} block offset");
                break;
            default:
                AssertNear(DotOffset, r[2] - r[6], $"{corner} inline offset");
                AssertNear(DotOffset, r[3] - r[7], $"{corner} block offset");
                break;
        }
    }
}
