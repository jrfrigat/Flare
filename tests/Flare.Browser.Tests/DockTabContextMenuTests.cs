using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class DockTabContextMenuTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task RightClickUsesTheInvokedTabAndKeepsTheMenuInsideTheViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 360, Height = 350 } });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-context-menu");
        var dock = page.Locator("#context-dock");
        var source = dock.GetByRole(AriaRole.Tablist, new() { Name = "source" });
        var inactive = source.GetByRole(AriaRole.Tab, new() { Name = "B", Exact = true });
        var menu = page.GetByRole(AriaRole.Menu, new() { Name = "Tab actions" });

        await Expect(inactive).ToHaveAttributeAsync("aria-selected", "false");
        await Expect(dock.Locator($".{Css.Classes.Tabs.HeaderEnd}")).ToHaveCountAsync(0);
        await inactive.ClickAsync(new() { Button = MouseButton.Right });
        await Expect(menu).ToBeVisibleAsync();
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Close B", Exact = true })).ToBeVisibleAsync();
        await Expect(inactive).ToHaveAttributeAsync("aria-selected", "false");
        await page.WaitForFunctionAsync("""
            () => {
                const menu = document.querySelector('[role="menu"]:popover-open');
                if (!menu) return false;
                const rect = menu.getBoundingClientRect();
                return rect.left >= 0 && rect.top >= 0 && rect.right <= innerWidth && rect.bottom <= innerHeight;
            }
            """);

        var longLabel = menu.GetByRole(AriaRole.Menuitem, new() { NameRegex = new("^Move before A particularly long") })
            .Locator($".{Css.Classes.Menu.ItemLabel}");
        await Expect(longLabel).ToBeVisibleAsync();
        Assert.True(await longLabel.EvaluateAsync<bool>("el => el.scrollWidth <= el.clientWidth"));

        await menu.GetByRole(AriaRole.Menuitem, new() { Name = "Move to target", Exact = true }).ClickAsync();
        await Expect(dock.GetByRole(AriaRole.Tablist, new() { Name = "target" })
            .GetByRole(AriaRole.Tab, new() { Name = "B", Exact = true })).ToBeVisibleAsync();
        await Expect(source.GetByRole(AriaRole.Tab, new() { Name = "A", Exact = true }))
            .ToHaveAttributeAsync("aria-selected", "true");
        await Expect(menu).ToBeHiddenAsync();
    }

    [Fact]
    public async Task KeyboardOpensTheTabMenuAndEscapeRestoresTabFocus()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-context-menu");
        var dock = page.Locator("#context-dock");
        var tab = dock.GetByRole(AriaRole.Tab, new() { Name = "B", Exact = true });
        var menu = page.GetByRole(AriaRole.Menu, new() { Name = "Tab actions" });

        await tab.PressAsync("Shift+F10");
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Close B", Exact = true })).ToBeVisibleAsync();
        await Expect(menu).ToBeFocusedAsync();
        await menu.PressAsync("Escape");
        await Expect(menu).ToBeHiddenAsync();
        await Expect(tab).ToBeFocusedAsync();
    }
}
