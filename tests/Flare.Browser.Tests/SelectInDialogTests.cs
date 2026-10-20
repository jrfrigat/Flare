using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A mouse choice in a select inside a dialog must apply the value and keep the dialog open with the
/// focus inside it. A multi-select keeps its list open while the values change. This regressed once
/// for dialogs and drawers and is pinned here against the browser, not bUnit.
/// </summary>
public sealed class SelectInDialogTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(900, 700);
        await page.GotoAsync($"{fixture.BaseUrl}/select-in-dialog");
        await page.Locator("#open-dialog").ClickAsync();
        await Expect(page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
        return page;
    }

    private static async Task<bool> FocusIsInsideDialogAsync(IPage page) =>
        await page.EvaluateAsync<bool>(
            "() => { const dialog = document.querySelector('[role=dialog]'); return !!dialog && dialog.contains(document.activeElement); }");

    [Fact]
    public async Task MouseChoice_AppliesTheValueAndKeepsFocusInTheDialog()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var combo = page.GetByRole(AriaRole.Combobox, new() { Name = "Status" });
        await combo.ClickAsync();
        var list = page.Locator("[role=listbox]");
        await Expect(list).ToBeVisibleAsync();

        await list.GetByRole(AriaRole.Option, new() { Name = "Closed" }).ClickAsync();

        await Expect(page.Locator("#value")).ToHaveTextAsync("Closed");
        await Expect(list).ToBeHiddenAsync();
        await Expect(page.Locator("#dialog-state")).ToHaveTextAsync("open");
        Assert.True(await FocusIsInsideDialogAsync(page));
    }

    [Fact]
    public async Task MultiSelectMouseChoices_ChangeTheValuesAndKeepTheListOpen()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var combo = page.GetByRole(AriaRole.Combobox, new() { Name = "Tags" });
        await combo.ClickAsync();
        var list = page.Locator("[role=listbox]");
        await Expect(list).ToBeVisibleAsync();

        await list.GetByRole(AriaRole.Option, new() { Name = "Red" }).ClickAsync();
        await Expect(page.Locator("#count")).ToHaveTextAsync("1");
        await Expect(list).ToBeVisibleAsync();

        await list.GetByRole(AriaRole.Option, new() { Name = "Green" }).ClickAsync();
        await Expect(page.Locator("#count")).ToHaveTextAsync("2");
        await Expect(list).ToBeVisibleAsync();
        await Expect(page.Locator("#dialog-state")).ToHaveTextAsync("open");
        Assert.True(await FocusIsInsideDialogAsync(page));
    }
}
