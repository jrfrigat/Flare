using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// AutoGrow hands the height to the content, so the manual resize handle has to disappear even when
/// Resize says otherwise; switching AutoGrow off gives the caller's Resize back.
/// </summary>
public sealed class TextAreaResizeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/textarea-autogrow");
        await Expect(page.Locator("#grow textarea")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task AutoGrowHidesTheResizeHandleAndTurningItOffRestoresIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        await Expect(page.Locator("#grow textarea")).ToHaveCSSAsync("resize", "none");
        await Expect(page.Locator("#manual textarea")).ToHaveCSSAsync("resize", "both");

        await page.Locator("#toggle").ClickAsync();
        await Expect(page.Locator("#grow textarea")).ToHaveCSSAsync("resize", "both");

        await page.Locator("#toggle").ClickAsync();
        await Expect(page.Locator("#grow textarea")).ToHaveCSSAsync("resize", "none");
    }
}
