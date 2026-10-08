using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed partial class TimePickerScrollTests
{
    public static IEnumerable<object[]> DateThemes =>
        new[] { "md3-expressive", "md3", "md2", "fluent2", "aero", "liquid-glass", "visualstudio" }
            .SelectMany(theme => new object[][] { [theme, false], [theme, true] });

    [Theory]
    [MemberData(nameof(DateThemes))]
    public Task PreRenderedCalendar_RefreshesCellsAndKeepsKeyboardFocus(string theme, bool mobile) =>
        RunAsync(theme, mobile, async page =>
        {
            var panel = page.Locator($".{Css.Classes.DatePicker.Panel}[role=dialog]");
            var toggle = page.Locator($".{Css.Classes.DatePicker.Root} button[aria-haspopup=dialog]");
            var cells = panel.Locator("button[role=gridcell]");
            await Expect(panel).ToBeHiddenAsync();
            await Expect(cells).ToHaveCountAsync(42);
            var retained = (await panel.ElementHandleAsync())!;
            await page.GetByRole(AriaRole.Button, new() { Name = "Rerender", Exact = true }).ClickAsync();
            await Expect(page.GetByTestId("version")).ToHaveTextAsync("1");
            await toggle.ClickAsync();
            await Expect(panel).ToBeVisibleAsync();
            await Expect(panel).ToHaveAttributeAsync("data-flare-placed", "");
            await Expect(cells.Filter(new() { HasTextRegex = new Regex("^\\s*15\\s*$") })).ToBeFocusedAsync();
            await Expect(panel).ToBeInViewportAsync(new() { Ratio = 1 });
            await page.Keyboard.PressAsync("ArrowRight");
            await Expect(cells.Filter(new() { HasTextRegex = new Regex("^\\s*16\\s*$") })).ToBeFocusedAsync();
            await page.Keyboard.PressAsync("Enter");
            await Expect(page.GetByTestId("date-value")).ToHaveTextAsync("2026-10-16");
            await Expect(panel).ToBeHiddenAsync();
            await Expect(toggle).ToBeFocusedAsync();
            Assert.True(await retained.EvaluateAsync<bool>("el => el.isConnected"));

            await page.GetByRole(AriaRole.Button, new() { Name = "External value" }).ClickAsync();
            await Expect(panel.Locator("button[aria-selected=true]")).ToHaveTextAsync("20");
            await Expect(panel.Locator("button.after-day")).ToHaveCountAsync(42);
            await page.GetByRole(AriaRole.Button, new() { Name = "Disable current" }).ClickAsync();
            await Expect(cells.Filter(new() { HasTextRegex = new Regex("^\\s*20\\s*$") })).ToBeDisabledAsync();
            await toggle.ClickAsync();
            await page.Keyboard.PressAsync("Tab");
            await Expect(panel.Locator(":focus")).ToHaveCountAsync(1);
            await page.Keyboard.PressAsync("Escape");
            await Expect(panel).ToBeHiddenAsync();
            await Expect(page.Locator($".{Css.Classes.DatePicker.Root} input")).ToBeFocusedAsync();
            await toggle.ClickAsync();
            await cells.Filter(new() { HasTextRegex = new Regex("^\\s*21\\s*$") }).ClickAsync();
            await Expect(page.GetByTestId("date-value")).ToHaveTextAsync("2026-10-21");
            await Expect(panel).ToBeHiddenAsync();
            Assert.True(await retained.EvaluateAsync<bool>("el => el.isConnected"));
        }, "/date");
}
