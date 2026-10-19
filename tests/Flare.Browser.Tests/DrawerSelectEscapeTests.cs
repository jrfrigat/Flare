using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class DrawerSelectEscapeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context, int width = 1280)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(width, 844);
        await page.GotoAsync($"{fixture.BaseUrl}/drawer-select");
        await page.Locator("#open").ClickAsync();
        await Expect(page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
        // The trap moves focus into the drawer after the Escape handler is registered.
        await Expect(page.GetByRole(AriaRole.Combobox)).ToBeFocusedAsync();
        return page;
    }

    [Theory]
    [InlineData(1280)]
    [InlineData(390)]
    public async Task EscapeClosesTheSelectFirst_ThenTheDrawer(int width)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width);
        var combo = page.GetByRole(AriaRole.Combobox);

        await combo.ClickAsync();
        await Expect(page.Locator("[role=listbox]")).ToBeVisibleAsync();
        await page.Keyboard.PressAsync("Escape");

        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("open");
        await Expect(page.Locator("[role=listbox]")).ToBeHiddenAsync();
        Assert.Equal("false", await combo.GetAttributeAsync("aria-expanded"));

        await page.Keyboard.PressAsync("Escape");
        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("closed");
    }

    [Fact]
    public async Task EscapeClosesTheDrawerOnItsOwn()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        await page.Keyboard.PressAsync("Escape");

        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("closed");
    }

    [Fact]
    public async Task EscapeClosesTheDrawerWhenTheSelectHasFocus()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        await page.GetByRole(AriaRole.Combobox).FocusAsync();
        await page.Keyboard.PressAsync("Escape");

        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("closed");
    }

    [Fact]
    public async Task EscapeThroughAnOpenTrigger_IsLeftToThePopup()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var combo = page.GetByRole(AriaRole.Combobox);

        await combo.EvaluateAsync("el => el.setAttribute('aria-expanded', 'true')");
        await page.Keyboard.PressAsync("Escape");
        await page.WaitForTimeoutAsync(300);
        Assert.Equal("open", await page.Locator("#drawer-state").TextContentAsync());

        await combo.EvaluateAsync("el => el.setAttribute('aria-expanded', 'false')");
        await page.Keyboard.PressAsync("Escape");
        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("closed");
    }

    [Fact]
    public async Task EscapeRightAfterOpeningTheSelect_StillLeavesTheDrawerOpen()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var combo = page.GetByRole(AriaRole.Combobox);

        await combo.ClickAsync();
        await page.Keyboard.PressAsync("Escape");

        await Expect(page.Locator("#drawer-state")).ToHaveTextAsync("open");
        await Expect(page.Locator("[role=listbox]")).ToBeHiddenAsync();
    }
}
