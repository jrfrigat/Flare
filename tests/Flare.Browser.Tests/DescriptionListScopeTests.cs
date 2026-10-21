using Microsoft.Playwright;

namespace Flare.Browser.Tests;

/// <summary>
/// A striped or bordered description list paints only its own rows: a nested list inside a value cell
/// keeps its own configuration instead of inheriting the outer striping and dividers.
/// </summary>
public sealed class DescriptionListScopeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string Transparent = "rgba(0, 0, 0, 0)";

    [Fact]
    public async Task NestedList_KeepsItsOwnStripingAndBorders()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/description-list-scope");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('#outer-2 > dt') !== null"
            + $" && getComputedStyle(document.querySelector('#outer-2 > dt')).backgroundColor !== '{Transparent}'");

        var outerStripe = await CssAsync(page, "#outer-2 > dt", "backgroundColor");
        Assert.Equal(outerStripe, await CssAsync(page, "#outer-2 > dd", "backgroundColor"));
        Assert.Equal(Transparent, await CssAsync(page, "#outer-1 > dt", "backgroundColor"));

        // The plain nested list takes neither the stripe nor the divider of the outer list.
        foreach (var row in new[] { "#plain-1", "#plain-2", "#plain-3" })
        {
            Assert.Equal(Transparent, await CssAsync(page, $"{row} > dt", "backgroundColor"));
            Assert.Equal("0px", await CssAsync(page, $"{row} > dt", "borderBottomWidth"));
        }

        // A nested list with its own options keeps them, independently of the outer one.
        Assert.Equal(Transparent, await CssAsync(page, "#inner-1 > dt", "backgroundColor"));
        Assert.Equal(outerStripe, await CssAsync(page, "#inner-2 > dt", "backgroundColor"));
        Assert.Equal("1px", await CssAsync(page, "#inner-1 > dt", "borderBottomWidth"));
        Assert.Equal("0px", await CssAsync(page, "#inner-2 > dt", "borderBottomWidth"));

        // The outer list still borders every row but its last.
        foreach (var row in new[] { "#outer-1", "#outer-2", "#outer-3" })
            Assert.Equal("1px", await CssAsync(page, $"{row} > dt", "borderBottomWidth"));
        Assert.Equal("0px", await CssAsync(page, "#outer-4 > dt", "borderBottomWidth"));
    }

    private static Task<string> CssAsync(IPage page, string selector, string property)
        => page.Locator(selector).EvaluateAsync<string>(
            $"el => getComputedStyle(el).{property}");
}
