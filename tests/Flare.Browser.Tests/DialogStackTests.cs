using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Service dialogs as an app uses them: a message box over the page, an editor opened through
/// <c>IDialogService.ShowAsync</c> whose field takes autofocus, and a confirmation opened from inside it.
/// Focus has to stay in the top dialog, Escape has to close only that dialog, and closing has to hand focus
/// back to whatever opened it.
/// </summary>
public sealed class DialogStackTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("#alert", "Notice", true)]
    [InlineData("#confirm", "Leave?", false)]
    [InlineData("#prompt", "Rename", false)]
    public async Task MessageBox_KeepsFocusInside_ThenEscapeReturnsItToOpener(string opener, string title, bool alert)
    {
        var page = await OpenAsync();
        await page.Locator(opener).PressAsync("Enter");
        var box = page.GetByRole(alert ? AriaRole.Alertdialog : AriaRole.Dialog, new() { Name = title, Exact = true });
        await Expect(box).ToBeVisibleAsync();
        await ExpectFocusWithinAsync(box);

        for (var i = 0; i < 4; i++)
        {
            await page.Keyboard.PressAsync("Tab");
            await ExpectFocusWithinAsync(box);
        }
        await page.Keyboard.PressAsync("Shift+Tab");
        await ExpectFocusWithinAsync(box);
        await page.Locator("#background").FocusAsync();
        await ExpectFocusWithinAsync(box);

        await page.Keyboard.PressAsync("Escape");
        await Expect(box).ToBeHiddenAsync();
        await Expect(page.Locator(opener)).ToBeFocusedAsync();
    }

    [Fact]
    public async Task Prompt_StartsInItsInput()
    {
        var page = await OpenAsync();
        await page.Locator("#prompt").PressAsync("Enter");
        var box = page.GetByRole(AriaRole.Dialog, new() { Name = "Rename", Exact = true });
        await Expect(box.GetByRole(AriaRole.Textbox)).ToBeFocusedAsync();
    }

    [Theory]
    [InlineData("escape")]
    [InlineData("cancel")]
    [InlineData("save")]
    public async Task EditorWithAutofocus_ReturnsFocusToOpener(string closeBy)
    {
        var page = await OpenAsync();
        await page.Locator("#new").PressAsync("Enter");
        var editor = page.GetByRole(AriaRole.Dialog, new() { Name = "Fine", Exact = true });
        var field = editor.GetByLabel("Violation");
        await Expect(field).ToBeFocusedAsync();

        // Escape has to work from the first moment the autofocused field has it, before the dialog's JS
        // registration has finished its round trip; no scroll-lock wait is allowed to hide that window.
        if (closeBy == "escape") await page.Keyboard.PressAsync("Escape");
        else await page.Locator($"#{closeBy}").PressAsync("Enter");

        await Expect(editor).ToBeHiddenAsync();
        await Expect(page.Locator("#new")).ToBeFocusedAsync();
    }

    [Fact]
    public async Task NestedConfirmation_EscapeClosesOnlyTheTopDialog()
    {
        var page = await OpenAsync();
        await page.Locator("#new").PressAsync("Enter");
        var editor = page.GetByRole(AriaRole.Dialog, new() { Name = "Fine", Exact = true });
        var field = editor.GetByLabel("Violation");
        await Expect(field).ToBeFocusedAsync();
        await field.FillAsync("Late again");

        await page.Locator("#delete").PressAsync("Enter");
        var confirm = page.GetByRole(AriaRole.Dialog, new() { Name = "Delete?", Exact = true });
        await Expect(confirm).ToBeVisibleAsync();
        await ExpectFocusWithinAsync(confirm);

        await page.Keyboard.PressAsync("Escape");
        await Expect(confirm).ToBeHiddenAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(field).ToHaveValueAsync("Late again");
        await Expect(page.Locator("#delete")).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Escape");
        await Expect(editor).ToBeHiddenAsync();
        await Expect(page.Locator("#new")).ToBeFocusedAsync();
    }

    [Fact]
    public async Task OpenSelectInsideDialog_EscapeClosesOnlyTheSelect()
    {
        var page = await OpenAsync();
        await page.Locator("#new").PressAsync("Enter");
        var editor = page.GetByRole(AriaRole.Dialog, new() { Name = "Fine", Exact = true });
        var select = editor.GetByRole(AriaRole.Combobox, new() { Name = "Kind" });
        await select.ClickAsync();
        await Expect(page.GetByRole(AriaRole.Listbox)).ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");
        await Expect(page.GetByRole(AriaRole.Listbox)).ToBeHiddenAsync();
        await Expect(editor).ToBeVisibleAsync();
    }

    [Fact]
    public async Task RemovedOpener_FocusFallsBackToItsContainer()
    {
        var page = await OpenAsync();
        await page.Locator("#remove-a").PressAsync("Enter");
        var confirm = page.GetByRole(AriaRole.Dialog, new() { Name = "Remove?", Exact = true });
        await Expect(confirm).ToBeVisibleAsync();
        await ExpectFocusWithinAsync(confirm);
        await confirm.GetByRole(AriaRole.Button, new() { Name = "Yes", Exact = true }).PressAsync("Enter");

        await Expect(confirm).ToBeHiddenAsync();
        await Expect(page.Locator("#remove-a")).ToHaveCountAsync(0);
        await Expect(page.Locator("#rows")).ToBeFocusedAsync();
    }

    // :focus inside the dialog, retried like every other expectation: focus moves after the interop call.
    private static Task ExpectFocusWithinAsync(ILocator dialog) =>
        Expect(dialog.Locator(":focus")).ToHaveCountAsync(1);

    private async Task<IPage> OpenAsync()
    {
        var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dialog-stack");
        await Expect(page.Locator("#new")).ToBeVisibleAsync();
        // The overlay module remembers focus from the moment it is evaluated, which an app starts at start-up; a
        // test reaches the first button sooner than a person could. Importing the same URL waits for that module.
        await page.EvaluateAsync("() => import('./_content/Flare.Components/js/flare-overlay.js')");
        return page;
    }
}
