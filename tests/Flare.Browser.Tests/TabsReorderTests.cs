using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// FlareTabs reordering: Alt+Arrow moves the focused tab through the JS keyboard module, and the
/// C# fallback keeps the same key working when the module never registers.
/// </summary>
public sealed class TabsReorderTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static async Task WaitForKeyboardAsync(IPage page)
    {
        await page.WaitForFunctionAsync(
            "() => !document.querySelector('[role=tablist]')?.hasAttribute('data-flare-keyboard-fallback')");
    }

    [Fact]
    public async Task AltArrowMovesTheActiveTab()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-fallback");
        await WaitForKeyboardAsync(page);

        var first = page.GetByRole(AriaRole.Tab, new() { Name = "One", Exact = true });
        await first.FocusAsync();
        await page.Keyboard.PressAsync("Alt+ArrowRight");

        await Expect(first).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(page.Locator("[role=tab]").First).ToHaveAttributeAsync("aria-label", "Two");
        Assert.Equal(new[] { "Two", "One", "Three" }, await page.Locator("[role=tab]").AllTextContentsAsync());
    }

    [Fact]
    public async Task AltArrowStillWorksWhenTheModuleFails()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.RouteAsync("**/flare-ui.js*", route => route.AbortAsync());
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-fallback");
        await Expect(page.Locator("[role=tablist]")).ToHaveAttributeAsync("data-flare-keyboard-fallback", "true");

        await page.GetByRole(AriaRole.Tab, new() { Name = "One", Exact = true }).FocusAsync();
        await page.Keyboard.PressAsync("Alt+ArrowRight");

        await Expect(page.Locator("[role=tab]").First).ToHaveAttributeAsync("aria-label", "Two");
        Assert.Equal(new[] { "Two", "One", "Three" }, await page.Locator("[role=tab]").AllTextContentsAsync());
    }
}
