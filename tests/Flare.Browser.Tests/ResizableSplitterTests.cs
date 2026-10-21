using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A resizable handle reads changed limits, and a splitter with Value but no ValueVariable stays in
/// the value mode instead of silently resizing its neighbours.
/// </summary>
public sealed class ResizableSplitterTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Resizable_AppliesChangedLimits()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            "() => document.querySelector('#resizable-box [role=separator]')?.hasAttribute('aria-valuenow')");

        var handle = page.Locator("#resizable-box [role='separator']");
        await handle.FocusAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator($"#resizable-box .{Css.Classes.Resizable.Root}")).ToHaveCSSAsync("width", "300px");

        await page.Locator("#raise-max").ClickAsync();
        await Expect(page.Locator("#max-value")).ToHaveTextAsync("500px");
        await handle.FocusAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator($"#resizable-box .{Css.Classes.Resizable.Root}")).ToHaveCSSAsync("width", "500px");
    }

    [Fact]
    public async Task Splitter_ValueWithoutVariable_StaysInValueMode()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('#splitter-row [role=separator]')?.classList.contains('{Css.Classes.Splitter.Horizontal}')");

        var gutter = page.Locator("#splitter-row [role='separator']");
        await gutter.FocusAsync();
        await page.Keyboard.PressAsync("End");

        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "500");
        await Expect(page.Locator("#split-size")).ToHaveTextAsync("500px");
        Assert.Equal(300, await page.Locator("#pane-a").EvaluateAsync<double>("el => el.getBoundingClientRect().width"));
        Assert.Equal(300, await page.Locator("#pane-b").EvaluateAsync<double>("el => el.getBoundingClientRect().width"));
    }
}
