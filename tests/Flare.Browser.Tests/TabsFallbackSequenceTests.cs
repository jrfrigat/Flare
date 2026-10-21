using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// With flare-ui.js blocked, the C# fallback has to keep receiving keys after the tab bar re-renders:
/// several sequential keys, not just the first one. The focus reveal that used to call into the failed
/// module threw an unhandled JSException and killed the circuit, which read as a lost second key.
/// </summary>
public sealed class TabsFallbackSequenceTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Fallback_HandlesSeveralSequentialKeys()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.RouteAsync("**/flare-ui.js*", route => route.AbortAsync());

        await page.GotoAsync($"{fixture.BaseUrl}/tabs-fallback");
        var first = page.GetByRole(AriaRole.Tab, new() { Name = "One", Exact = true });
        await Expect(first).ToBeVisibleAsync();
        var bar = page.Locator($"[role=tablist]");
        await Expect(bar).ToHaveAttributeAsync("data-flare-keyboard-fallback", "true");

        await first.ClickAsync();
        await Expect(page.Locator("#active")).ToHaveTextAsync("0");

        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator("#active")).ToHaveTextAsync("2");

        await page.Keyboard.PressAsync("Home");
        await Expect(page.Locator("#active")).ToHaveTextAsync("0");

        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("#active")).ToHaveTextAsync("1");
    }

    [Fact]
    public async Task WithTheModule_KeysMoveOneStep()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{fixture.BaseUrl}/tabs-fallback");
        var first = page.GetByRole(AriaRole.Tab, new() { Name = "One", Exact = true });
        await Expect(first).ToBeVisibleAsync();
        await Expect(page.Locator($"[role=tablist]")).Not.ToHaveAttributeAsync("data-flare-keyboard-fallback", "true");

        await first.ClickAsync();
        await Expect(page.Locator("#active")).ToHaveTextAsync("0");

        // One step per key: a double handler (module + C# fallback) would move two tabs.
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("#active")).ToHaveTextAsync("1");
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("#active")).ToHaveTextAsync("2");
    }
}
