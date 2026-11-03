using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ComponentTokensTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task ThemeServiceOverridesReachIndependentComponents()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/component-tokens");
        var bar = page.Locator($"#chart .{Css.Classes.Gantt.Bar}");
        await Expect(bar).ToBeVisibleAsync();
        await page.Locator("#base").ClickAsync();
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(12, 34, 56)");
        await Expect(page.Locator("#board .flare-kanban__card")).ToHaveCSSAsync("background-color", "rgb(56, 34, 12)");
        await page.Locator("#customize").ClickAsync();
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(18, 52, 86)");
        await Expect(page.Locator("#board .flare-kanban__card")).ToHaveCSSAsync("background-color", "rgb(86, 52, 18)");
    }
}
