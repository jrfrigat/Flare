using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class CheckboxIndeterminateColorTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task IndeterminateUsesTheSameColorAsChecked_AndACustomColor()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/checkbox");
        await Expect(page.Locator("#role .flare-checkbox__indicator")).ToBeVisibleAsync();

        var role = await BackgroundAsync(page, "#role .flare-checkbox__indicator");
        var checkedRole = await BackgroundAsync(page, "#role-checked .flare-checkbox__indicator");
        var custom = await BackgroundAsync(page, "#custom .flare-checkbox__indicator");

        Assert.Equal(checkedRole, role);
        Assert.Equal("rgb(255, 0, 0)", custom);
    }

    private static Task<string> BackgroundAsync(IPage page, string selector)
        => page.EvaluateAsync<string>(
            "selector => getComputedStyle(document.querySelector(selector)).backgroundColor", selector);
}
