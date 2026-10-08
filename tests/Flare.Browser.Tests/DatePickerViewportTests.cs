using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed partial class DatePickerUxTests
{
    public static IEnumerable<object[]> DateThemes => TimePickerScrollTests.DateThemes;

    [Theory]
    [MemberData(nameof(DateThemes))]
    public Task ShortViewport_KeepsCalendarAndClearReachable(string theme, bool dark) => RunAsync(async page =>
    {
        await page.SetViewportSizeAsync(320, 256);
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync(theme);
        await page.GetByLabel("Mode", new() { Exact = true }).SelectOptionAsync(dark ? "Dark" : "Light");
        var subject = page.GetByTestId("subject");
        var panel = subject.Locator($".{Css.Classes.DatePicker.Panel}");
        await subject.Locator("button[aria-haspopup]").ClickAsync();
        await Expect(panel).ToHaveAttributeAsync("data-flare-placed", "");
        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
        await page.WaitForFunctionAsync("() => window.scrollY > 100");
        await FitsViewportAsync(page);
        await page.SetViewportSizeAsync(320, 200);
        await FitsViewportAsync(page);
        var clear = panel.GetByRole(AriaRole.Button, new() { Name = "Clear", Exact = true });
        await clear.FocusAsync();
        await Expect(clear).ToBeInViewportAsync(new() { Ratio = 1 });
        await clear.PressAsync("Enter");
        await Expect(page.GetByTestId("value")).ToHaveTextAsync("");
        await Expect(panel).ToBeHiddenAsync();
    });

    private static Task FitsViewportAsync(IPage page) => page.WaitForFunctionAsync("""
        () => {
            const panel = document.querySelector('[data-testid=subject] [data-flare-placed]');
            const r = panel.getBoundingClientRect(), v = visualViewport;
            return r.top >= v.offsetTop + 3 && r.bottom <= v.offsetTop + v.height - 3
                && r.left >= v.offsetLeft + 3 && r.right <= v.offsetLeft + v.width - 3;
        }
        """, null, new() { Timeout = 5000 });
}
