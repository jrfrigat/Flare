using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The group's name has to actually reach the accessibility tree, which only happens because the root
/// carries role="group". The buttons inside stay native toggles: Tab reaches each one and Space or
/// Enter fires the browser's own activation.
/// </summary>
public sealed class ToggleGroupAccessibilityTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/toggle-group");
        await Expect(page.Locator("#named")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task TheNamedGroupAndItsPressedButtonsReachTheAccessibilityTree()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var group = page.GetByRole(AriaRole.Group, new() { Name = "Text alignment", Exact = true });
        await Expect(group).ToBeVisibleAsync();

        var left = group.GetByRole(AriaRole.Button, new() { Name = "Left", Exact = true });
        var right = group.GetByRole(AriaRole.Button, new() { Name = "Right", Exact = true });
        await Expect(left).ToHaveAttributeAsync("aria-pressed", "false");
        await Expect(right).ToHaveAttributeAsync("aria-pressed", "false");
        await Expect(group.Locator("[role=radio]")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task TabReachesEachButtonAndSpaceAndEnterActivateThem()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var left = page.GetByRole(AriaRole.Button, new() { Name = "Left", Exact = true });
        var right = page.GetByRole(AriaRole.Button, new() { Name = "Right", Exact = true });

        await left.FocusAsync();
        await page.Keyboard.PressAsync("Space");
        await Expect(left).ToHaveAttributeAsync("aria-pressed", "true");

        await page.Keyboard.PressAsync("Tab");
        await Expect(right).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(right).ToHaveAttributeAsync("aria-pressed", "true");

        // MultiSelect is preserved: activating the second button left the first one pressed.
        await Expect(left).ToHaveAttributeAsync("aria-pressed", "true");
    }
}
