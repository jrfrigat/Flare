using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class MenuBarNestedTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task EscapeClosesOneMenuAtATimeAndRestoresFocus()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/menu-bar-nested");

        var fileButton = page.GetByRole(AriaRole.Button, new() { Name = "File", Exact = true });
        var filePanel = page.GetByRole(AriaRole.Group, new() { Name = "File panel" });
        var exportButton = filePanel.GetByRole(AriaRole.Button, new() { Name = "Export" });
        var exportPanel = page.GetByRole(AriaRole.Menu, new() { Name = "Export menu" });

        await fileButton.ClickAsync();
        await Expect(filePanel).ToBeFocusedAsync();
        await exportButton.ClickAsync();
        await Expect(exportPanel).ToBeFocusedAsync();
        await Expect(filePanel).ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");
        await Expect(exportPanel).ToHaveCountAsync(0);
        await Expect(filePanel).ToBeVisibleAsync();
        await Expect(exportButton).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Escape");
        await Expect(filePanel).ToHaveCountAsync(0);
        await Expect(fileButton).ToBeFocusedAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DismissingAStandaloneMenuKeepsTheOutsideFieldFocused(bool useTab)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/menu-bar-nested");

        var panel = page.GetByRole(AriaRole.Group, new() { Name = "Standalone panel" });
        var inside = page.Locator("#menu-input");
        var outside = page.Locator("#outside-input");
        await page.GetByRole(AriaRole.Button, new() { Name = "Standalone menu" }).ClickAsync();
        await Expect(panel).ToBeFocusedAsync();

        if (useTab)
        {
            await inside.FocusAsync();
            await page.Keyboard.PressAsync("Tab");
        }
        else
        {
            await outside.ClickAsync();
        }

        await Expect(panel).ToHaveCountAsync(0);
        await Expect(outside).ToBeFocusedAsync();
        await page.Keyboard.TypeAsync("kept focus");
        await Expect(outside).ToHaveValueAsync("kept focus");
    }
}
