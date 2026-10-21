using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A mini-rail link without Tooltip must keep its label in the accessibility tree: the icon is
/// decorative, so hiding the text with display:none used to leave the link unnamed.
/// </summary>
public sealed class NavRailAccessibleNameTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task RailLinkWithoutTooltip_KeepsItsAccessibleName()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/nav-rail");

        var link = page.GetByRole(AriaRole.Link, new() { Name = "Dashboard", Exact = true });
        await Expect(link).ToBeVisibleAsync();
        await Expect(link).ToHaveAccessibleNameAsync("Dashboard");
        var text = link.Locator(".flare-nav-link__text");
        Assert.Equal("absolute", await text.EvaluateAsync<string>("el => getComputedStyle(el).position"));
        Assert.Equal("1px", await text.EvaluateAsync<string>("el => getComputedStyle(el).width"));

        var labeled = page.GetByRole(AriaRole.Link, new() { Name = "Reports", Exact = true });
        await Expect(labeled).ToBeVisibleAsync();
        await Expect(labeled).ToContainTextAsync("Reports");
        var labeledText = labeled.Locator(".flare-nav-link__text");
        Assert.NotEqual("absolute", await labeledText.EvaluateAsync<string>("el => getComputedStyle(el).position"));
        Assert.True(await labeledText.EvaluateAsync<double>("el => el.getBoundingClientRect().width") > 1);
    }
}
