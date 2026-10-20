using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A clickable list row with its own action in a slot. Enter and Space on that action reach the row's
/// keyboard handler by bubbling unless the slot stops them, which bUnit cannot show: it bubbles keydown past
/// stopPropagation.
/// </summary>
public sealed class ListItemSlotTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("Enter")]
    [InlineData("Space")]
    public async Task SlotButtonKey_ActivatesOnlyTheButton(string key)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/list-item");
        await Expect(page.Locator("#slot-action")).ToBeVisibleAsync();

        await page.Locator("#slot-action").PressAsync(key);

        await Expect(page.Locator("#button-clicks")).ToHaveTextAsync("1");
        await Expect(page.Locator("#row-clicks")).ToHaveTextAsync("0");
    }

    // Space on a non-button row would scroll the page while also activating it; the native action
    // button makes both keys a plain button activation.
    [Theory]
    [InlineData("Enter")]
    [InlineData("Space")]
    public async Task RowKey_ActivatesOnceWithoutScrolling(string key)
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 420, Height = 300 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/list-item");

        var row = page.GetByRole(AriaRole.Button, new() { Name = "Fine" });
        await row.FocusAsync();
        await page.Keyboard.PressAsync(key);

        await Expect(page.Locator("#row-clicks")).ToHaveTextAsync("1");
        Assert.Equal(0, await page.EvaluateAsync<int>("() => window.scrollY"));
    }

    [Fact]
    public async Task UnrelatedKey_DoesNotActivateTheRow()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/list-item");

        var row = page.GetByRole(AriaRole.Button, new() { Name = "Fine" });
        await row.FocusAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await page.Keyboard.PressAsync("Enter");

        // One activation means ArrowDown added none of its own.
        await Expect(page.Locator("#row-clicks")).ToHaveTextAsync("1");
    }

    // The slot sits above the stretched action, so a real click lands on the slot's own control.
    [Fact]
    public async Task SlotButtonClick_ActivatesOnlyTheButton()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/list-item");

        await page.Locator("#slot-action").ClickAsync();

        await Expect(page.Locator("#button-clicks")).ToHaveTextAsync("1");
        await Expect(page.Locator("#row-clicks")).ToHaveTextAsync("0");
    }
}
