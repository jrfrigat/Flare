using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class FabMenuKeyboardTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static ILocator Trigger(IPage page) => page.Locator("#fab-trigger");
    private static ILocator Edit(IPage page) => page.Locator("#fab-edit button[role=menuitem]");
    private static ILocator Delete(IPage page) => page.Locator("#fab-delete button[role=menuitem]");

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/fab-menu");
        await Expect(Trigger(page)).ToBeVisibleAsync();
        await Trigger(page).FocusAsync();
        return page;
    }

    private static async Task OpenWithArrowAsync(IPage page)
    {
        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(Edit(page)).ToBeFocusedAsync();
    }

    [Fact]
    public async Task KeyboardActivation_OpensTheMenuAndFocusesTheFirstEnabledAction()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        await page.Keyboard.PressAsync("Enter");

        await Expect(Edit(page)).ToBeFocusedAsync();
        await Expect(Trigger(page)).ToHaveAttributeAsync("aria-expanded", "true");
        await Expect(page.Locator($".{Css.Classes.FabMenu.List}")).ToHaveAttributeAsync("aria-labelledby", "fab-trigger");
        await Expect(page.Locator("#fab-archived button[role=menuitem]")).ToBeDisabledAsync();
    }

    [Fact]
    public async Task ArrowKeys_MoveFocusOverEnabledActionsWithoutScrollingThePage()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await OpenWithArrowAsync(page);
        await page.EvaluateAsync("window.scrollTo(0, 0)");

        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(Delete(page)).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(Edit(page)).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("End");
        await Expect(Delete(page)).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Home");
        await Expect(Edit(page)).ToBeFocusedAsync();

        Assert.Equal(0, await page.EvaluateAsync<int>("window.scrollY"));
    }

    [Fact]
    public async Task Escape_ClosesTheMenuAndReturnsFocusToTheTrigger()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await OpenWithArrowAsync(page);

        await page.Keyboard.PressAsync("Escape");

        await Expect(Trigger(page)).ToBeFocusedAsync();
        await Expect(Trigger(page)).ToHaveAttributeAsync("aria-expanded", "false");
    }

    [Fact]
    public async Task Tab_ClosesTheMenuAndLeavesFocusOutsideOfIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await OpenWithArrowAsync(page);

        await page.Keyboard.PressAsync("Tab");

        // Wait for the close first: focus must stay where Tab put it even after the render lands.
        await Expect(Trigger(page)).ToHaveAttributeAsync("aria-expanded", "false");
        await Expect(page.Locator("#after-fab")).ToBeFocusedAsync();
    }

    [Fact]
    public async Task AnAction_ClosesTheMenuAndReturnsFocusToTheTrigger()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await OpenWithArrowAsync(page);

        await page.Keyboard.PressAsync("Enter");

        await Expect(Trigger(page)).ToBeFocusedAsync();
        await Expect(Trigger(page)).ToHaveAttributeAsync("aria-expanded", "false");
        await Expect(page.Locator("#actions")).ToHaveTextAsync("1");
    }
}
