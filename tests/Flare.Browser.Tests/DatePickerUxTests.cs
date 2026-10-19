using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>Keyboard, naming and responsive calendar contracts exercised by a real browser.</summary>
public sealed partial class DatePickerUxTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task RunAsync(Func<IPage, Task> check, string query = "")
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync(fixture.BaseUrl + "/date-ux" + query);
        await Expect(page.GetByTestId("subject").Locator("input")).ToBeVisibleAsync(new() { Timeout = 15000 });
        await check(page);
    }

    private static async Task ApplyThemeAsync(IPage page, string theme, bool dark)
    {
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync(theme);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-theme", theme);
        var mode = dark ? "Dark" : "Light";
        await page.GetByLabel("Mode", new() { Exact = true }).SelectOptionAsync(mode);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-mode", mode);
        await page.EvaluateAsync("async () => { await Promise.all(document.getAnimations().map(a => a.finished.catch(() => {}))); }");
    }

    [Fact]
    public Task FieldName_UsesVisibleRichLabelBeforePlaceholderOrLabel() => RunAsync(async page =>
    {
        await Expect(page.GetByTestId("rich-placeholder").Locator("input")).ToHaveAccessibleNameAsync("Arrival date");
        await Expect(page.GetByTestId("rich-label").Locator("input")).ToHaveAccessibleNameAsync("Departure date");
        await Expect(page.GetByTestId("subject").Locator("input")).ToHaveAccessibleNameAsync("Date");
        await Expect(page.GetByTestId("name-fallback").Locator("input")).ToHaveAccessibleNameAsync("Booking date");
        await Expect(page.GetByTestId("name-override").Locator("input")).ToHaveAccessibleNameAsync("Custom date");
    });

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public Task CalendarViewPick_KeepsFocusOnTheNextGrid(bool preRender, bool inline) => RunAsync(async page =>
    {
        var subject = page.GetByTestId("subject");
        var panel = subject.Locator($".{Css.Classes.DatePicker.Panel}");
        if (!inline)
        {
            await subject.Locator("button[aria-haspopup]").ClickAsync();
            await Expect(panel).ToHaveAttributeAsync("data-flare-placed", "");
            await Expect(panel.Locator("[role=gridcell][tabindex='0']")).ToBeFocusedAsync();
        }
        var header = panel.Locator($".{Css.Classes.DatePicker.MonthLabel}");
        await header.PressAsync("Enter");
        await Expect(header).ToHaveTextAsync("2026");
        await header.PressAsync("Enter");
        var year = panel.Locator("button[aria-pressed=true]");
        await Expect(year).ToHaveTextAsync("2026");
        await year.PressAsync("Enter");
        var month = panel.Locator("button[aria-pressed=true]");
        await Expect(month).ToHaveTextAsync("Oct");
        await Expect(month).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(panel.Locator("button:focus")).ToHaveTextAsync("Nov");
        await page.Keyboard.PressAsync("Enter");
        await Expect(header).ToHaveTextAsync("November 2026");
        var cursor = panel.Locator("[role=gridcell][tabindex='0']");
        await Expect(cursor).ToBeFocusedAsync();
        await Expect(cursor).ToHaveAttributeAsync("aria-label", new Regex("November .*2026"));
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(cursor).ToBeFocusedAsync();
        if (!inline)
        {
            await page.Keyboard.PressAsync("Escape");
            await Expect(panel).ToBeHiddenAsync();
            await Expect(subject.Locator("input")).ToBeFocusedAsync();
        }
        else
        {
            await page.Keyboard.PressAsync("Enter");
            await Expect(page.GetByTestId("value")).ToHaveTextAsync(new Regex("^2026-11-"));
        }
    }, $"?preRender={preRender}&inline={inline}");

    // TASK-602: a date typed into the field takes the calendar's cursor with it. The picked 16th used to stay
    // tabbable and focused after the field committed the 20th, so ArrowRight plus Enter picked the 17th.
    [Theory]
    [InlineData("md2")]
    [InlineData("fluent2")]
    public Task TypedCommit_MovesTheCalendarCursorAndTheNextArrowKey(string theme) => RunAsync(async page =>
    {
        await ApplyThemeAsync(page, theme, dark: false);
        var subject = page.GetByTestId("subject");
        var panel = subject.Locator($".{Css.Classes.DatePicker.Panel}");
        var cursor = panel.Locator("[role=gridcell][tabindex='0']");

        await subject.Locator("button[aria-haspopup]").ClickAsync();
        await Expect(panel).ToHaveAttributeAsync("data-flare-placed", "");
        await page.Keyboard.PressAsync("ArrowRight");                 // from the 15th to the 16th
        await Expect(cursor).ToHaveAttributeAsync("aria-label", "Friday, October 16, 2026");
        await page.Keyboard.PressAsync("Enter");                      // picks the 16th and closes
        await Expect(page.GetByTestId("value")).ToHaveTextAsync("2026-10-16");
        await Expect(panel).ToBeHiddenAsync();                        // the focus trap is gone before typing

        var input = subject.Locator("input");
        await input.FocusAsync();                                     // focus, not click: a click opens the popup
        await input.PressAsync("Control+A");
        // One digit per server roundtrip: the mask rewrites the field and restores the caret after each key.
        await input.PressSequentiallyAsync("10202026", new() { Delay = 60 });
        await Expect(input).ToHaveValueAsync("10/20/2026");
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.GetByTestId("value")).ToHaveTextAsync("2026-10-20");

        await subject.Locator("button[aria-haspopup]").ClickAsync();
        await Expect(cursor).ToBeFocusedAsync();
        await Expect(cursor).ToHaveAttributeAsync("aria-label", "Tuesday, October 20, 2026");
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(cursor).ToHaveAttributeAsync("aria-label", "Wednesday, October 21, 2026");
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.GetByTestId("value")).ToHaveTextAsync("2026-10-21");
        await Expect(panel).ToBeHiddenAsync();
    });

    [Theory]
    [InlineData("?unavailable=true")]
    [InlineData("?inline=true&readOnly=true")]
    public Task CalendarViewWithoutAvailableDays_KeepsAKeyboardExit(string query) => RunAsync(async page =>
    {
        var subject = page.GetByTestId("subject");
        var panel = subject.Locator($".{Css.Classes.DatePicker.Panel}");
        if (!query.Contains("inline=true"))
        {
            await subject.Locator("button[aria-haspopup]").ClickAsync();
            await Expect(panel).ToHaveAttributeAsync("data-flare-placed", "");
        }
        var header = panel.Locator($".{Css.Classes.DatePicker.MonthLabel}");
        await header.PressAsync("Enter");
        var month = panel.Locator("button[aria-pressed=true]");
        await Expect(month).ToHaveTextAsync("Oct");
        await month.PressAsync("Enter");
        await Expect(header).ToHaveTextAsync("October 2026");
        await Expect(header).ToBeFocusedAsync();
        if (!query.Contains("inline=true"))
        {
            await page.Keyboard.PressAsync("Escape");
            await Expect(panel).ToBeHiddenAsync();
        }
    }, query);
}
