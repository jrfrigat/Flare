using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A refused date entry stays in the field with a visible, localized reason that the input points at;
/// the committed value does not change and correcting the entry clears the error.
/// </summary>
public sealed class DatePickerInputErrorTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task RefusedEntry_IsShownWithAReason_AndCorrectionClearsIt()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 800, Height = 600 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datepicker-input-error");
        await page.WaitForSelectorAsync("[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var input = page.Locator("input[type=text]");
        await input.FocusAsync();

        // An impossible date: the field keeps it and says why; the value stays.
        await input.FillAsync("02/31/2026");
        await page.Keyboard.PressAsync("Tab");
        await Expect(input).ToHaveAttributeAsync("aria-invalid", "true");
        var errorId = await input.GetAttributeAsync("aria-describedby");
        Assert.False(string.IsNullOrEmpty(errorId));
        await Expect(page.Locator("#" + errorId)).ToHaveTextAsync("Enter a valid date.");
        await Expect(page.Locator("#value")).ToHaveTextAsync("2026-10-18");
        await Expect(input).ToHaveValueAsync("02/31/2026");

        // A real date outside Min/Max gets its own reason and is refused too.
        await input.FocusAsync();
        await input.FillAsync("11/05/2026");
        await page.Keyboard.PressAsync("Tab");
        await Expect(input).ToHaveAttributeAsync("aria-invalid", "true");
        await Expect(page.Locator("#" + await input.GetAttributeAsync("aria-describedby")))
            .ToHaveTextAsync("That date is not available.");
        await Expect(page.Locator("#value")).ToHaveTextAsync("2026-10-18");

        // Correcting the entry commits it and takes the error away.
        await input.FocusAsync();
        await input.FillAsync("10/20/2026");
        await page.Keyboard.PressAsync("Tab");
        await Expect(page.Locator("#value")).ToHaveTextAsync("2026-10-20");
        await Expect(input).Not.ToHaveAttributeAsync("aria-invalid", "true");
    }
}
