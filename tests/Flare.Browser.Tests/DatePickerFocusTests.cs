using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed partial class DatePickerUxTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public Task LiquidGlass_KeyboardFocusHasAVisibleOutline(bool dark, bool forcedColors) => RunAsync(async page =>
    {
        await page.EmulateMediaAsync(new()
        {
            ForcedColors = forcedColors ? ForcedColors.Active : ForcedColors.None,
            ReducedMotion = ReducedMotion.Reduce
        });
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync("liquid-glass");
        await page.GetByLabel("Mode", new() { Exact = true }).SelectOptionAsync(dark ? "Dark" : "Light");
        var subject = page.GetByTestId("subject");
        var panel = subject.Locator($".{Css.Classes.DatePicker.Panel}");
        await subject.Locator("button[aria-haspopup]").PressAsync("Enter");
        var day = panel.Locator("[role=gridcell][tabindex='0']");
        await VisibleOutlineAsync(day);
        await page.Keyboard.PressAsync("ArrowRight");
        await VisibleOutlineAsync(day);
        var header = panel.Locator($".{Css.Classes.DatePicker.MonthLabel}");
        await header.FocusAsync();
        await VisibleOutlineAsync(header);
        await header.PressAsync("Enter");
        var month = panel.Locator("button[aria-pressed=true]");
        await Expect(month).ToHaveTextAsync("Oct");
        await month.FocusAsync();
        await VisibleOutlineAsync(month);
        await header.PressAsync("Enter");
        var year = panel.Locator("button[aria-pressed=true]");
        await Expect(year).ToHaveTextAsync("2026");
        await year.FocusAsync();
        await VisibleOutlineAsync(year);
        var clear = panel.GetByRole(AriaRole.Button, new() { Name = "Clear", Exact = true });
        await clear.FocusAsync();
        await VisibleOutlineAsync(clear);
    });

    private static async Task VisibleOutlineAsync(ILocator element)
    {
        await Expect(element).ToBeFocusedAsync();
        Assert.True(await element.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await Expect(element).ToHaveCSSAsync("outline-style", "solid");
        Assert.True(await element.EvaluateAsync<bool>("el => parseFloat(getComputedStyle(el).outlineWidth) >= 2"));
    }
}
