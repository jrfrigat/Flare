using Flare.Css.Classes;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The column filter menu is a standard Flare popup: a click outside the widget - the page background
/// included - closes it without applying the filter, while clicks inside it, Escape and the trigger
/// itself keep their expected behavior.
/// </summary>
public sealed class DataGridFilterDismissTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task OutsideClick_ClosesTheMenu_AndKeepsTheFilterUnapplied()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1000, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-filter");
        await page.WaitForSelectorAsync($"[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var firstTrigger = page.Locator($"button.{Css.Classes.DataGrid.FilterTrigger}").First;
        var menu = page.Locator($".{Css.Classes.DataGrid.FilterMenu}");

        await firstTrigger.ClickAsync();
        await Expect(menu).ToBeVisibleAsync();

        // A click inside the menu is inside the widget and must not dismiss it.
        await menu.Locator($".{Css.Classes.DataGrid.FilterMenuLabel}").ClickAsync();
        await Expect(menu).ToBeVisibleAsync();

        await page.Locator("#page-background").ClickAsync();
        await Expect(menu).ToBeHiddenAsync();
        await Expect(page.Locator($"button.{Css.Classes.DataGrid.FilterTriggerActive}")).ToHaveCountAsync(0);
        await Expect(page.Locator($"tr.{Css.Classes.DataGrid.Row}")).ToHaveCountAsync(3);
    }

    [Fact]
    public async Task EscapeAndTrigger_ToggleTheMenu()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1000, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-filter");
        await page.WaitForSelectorAsync($"[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var firstTrigger = page.Locator($"button.{Css.Classes.DataGrid.FilterTrigger}").First;
        var menu = page.Locator($".{Css.Classes.DataGrid.FilterMenu}");

        await firstTrigger.ClickAsync();
        await Expect(menu).ToBeVisibleAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(menu).ToBeHiddenAsync();

        await firstTrigger.ClickAsync();
        await Expect(menu).ToBeVisibleAsync();
        await firstTrigger.ClickAsync();
        await Expect(menu).ToBeHiddenAsync();
    }

    [Fact]
    public async Task AnotherColumnsTrigger_MovesTheMenu()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1000, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-filter");
        await page.WaitForSelectorAsync($"[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var triggers = page.Locator($"button.{Css.Classes.DataGrid.FilterTrigger}");
        await triggers.Nth(0).ClickAsync();
        await Expect(page.Locator($".{Css.Classes.DataGrid.FilterMenu}")).ToBeVisibleAsync();

        await triggers.Nth(1).ClickAsync();
        await Expect(page.Locator($".{Css.Classes.DataGrid.FilterMenu}")).ToHaveCountAsync(1);
        await Expect(triggers.Nth(1)).ToHaveAttributeAsync("aria-expanded", "true");
        await Expect(triggers.Nth(0)).ToHaveAttributeAsync("aria-expanded", "false");
    }
}
