using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class MultiSelectSelectAllTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task CtrlA_TogglesAllOptions_AndKeepsFocusOnTheCombobox()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/multiselect");
        var combo = page.GetByRole(AriaRole.Combobox);
        await Expect(combo).ToHaveAttributeAsync("aria-keyshortcuts", "Control+A Meta+A");

        await combo.ClickAsync();
        await combo.PressAsync("Control+a");
        await Expect(page.Locator("#selected")).ToHaveTextAsync("Berlin,Boston,Paris");
        await Expect(combo).ToBeFocusedAsync();

        await combo.PressAsync("Control+a");
        await Expect(page.Locator("#selected")).ToHaveTextAsync("");
        await Expect(combo).ToBeFocusedAsync();
    }
}
