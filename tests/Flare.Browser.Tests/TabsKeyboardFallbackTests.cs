using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The tab keyboard is registered from flare-ui.js. A stale PWA can serve that module as a failure;
/// the registration is retried once and the failure is marked on the tab list instead of disappearing
/// silently, while the rest of the component keeps working.
/// </summary>
public sealed class TabsKeyboardFallbackTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task ModuleFailureIsMarkedAndLeavesTheRestWorking()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.RouteAsync("**/flare-ui.js*", route => route.AbortAsync());

        await page.GotoAsync($"{fixture.BaseUrl}/tabs-fallback");
        var first = page.GetByRole(AriaRole.Tab, new() { Name = "One", Exact = true });
        await Expect(first).ToBeVisibleAsync();

        // The retry runs once and then the bar says the script never took over.
        await Expect(page.Locator("[role=tablist]"))
            .ToHaveAttributeAsync("data-flare-keyboard-fallback", "true");

        // The component still works; only the keyboard enhancement is missing and now visible.
        var second = page.GetByRole(AriaRole.Tab, new() { Name = "Two", Exact = true });
        await second.ClickAsync();
        await Expect(second).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(page.Locator("#active")).ToHaveTextAsync("1");
    }
}
