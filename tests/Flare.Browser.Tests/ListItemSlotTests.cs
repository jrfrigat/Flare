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

    [Fact]
    public async Task RowKey_StillActivatesTheRow()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/list-item");

        await page.GetByRole(AriaRole.Button, new() { Name = "Fine" }).PressAsync("Enter");

        await Expect(page.Locator("#row-clicks")).ToHaveTextAsync("1");
    }
}
