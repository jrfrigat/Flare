using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ButtonLinkTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/button-link");
        await Expect(page.Locator("#pressed-link")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task AHrefButton_ReportsItsToggleStateAndStillNavigates()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var pressed = page.GetByRole(AriaRole.Button, new() { Name = "Pressed link", Exact = true });
        var unpressed = page.GetByRole(AriaRole.Button, new() { Name = "Unpressed link", Exact = true });

        await Expect(pressed).ToHaveAttributeAsync("aria-pressed", "true");
        await Expect(unpressed).ToHaveAttributeAsync("aria-pressed", "false");

        await pressed.ClickAsync();
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("#pressed$"));
    }

    [Fact]
    public async Task ADisabledHrefButton_KeepsTheStateAndDoesNotNavigate()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var disabled = page.GetByRole(AriaRole.Button, new() { Name = "Disabled link", Exact = true });

        await Expect(disabled).ToHaveAttributeAsync("aria-pressed", "true");
        await Expect(disabled).ToHaveAttributeAsync("aria-disabled", "true");
        await Expect(disabled).Not.ToHaveAttributeAsync("href", new System.Text.RegularExpressions.Regex("."));

        // Playwright will not click an element it reads as disabled; the point is that a forced click
        // still goes nowhere, because the href is gone.
        await disabled.ClickAsync(new() { Force = true });
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("button-link$"));
    }
}
