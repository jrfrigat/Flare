using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The stretched native control is what makes a card keyboard- and screen-reader-correct: a link card
/// is announced as a link and follows it, a selectable card is announced as a checkbox and Space
/// toggles it. Both are reached by Tab.
/// </summary>
public sealed class CardActivationTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/card-activation");
        await Expect(page.Locator("#selectable-card")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task ASelectableCard_IsACheckboxThatSpaceToggles()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var checkbox = page.GetByRole(AriaRole.Checkbox, new() { Name = "Selectable card" });
        await Expect(checkbox).ToHaveAttributeAsync("aria-checked", "false");

        await checkbox.FocusAsync();
        await page.Keyboard.PressAsync("Space");
        await Expect(checkbox).ToHaveAttributeAsync("aria-checked", "true");
        await Expect(page.Locator("#status")).ToHaveTextAsync("selected");
        await Expect(page.Locator("#selectable-card"))
            .ToHaveClassAsync(new Regex(Regex.Escape(Css.Classes.Card.Selected)));

        await page.Keyboard.PressAsync("Space");
        await Expect(checkbox).ToHaveAttributeAsync("aria-checked", "false");
        await Expect(page.Locator("#status")).ToHaveTextAsync("not selected");
    }

    [Fact]
    public async Task ALinkCard_IsALinkAndFollowsIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var link = page.GetByRole(AriaRole.Link, new() { Name = "Linked card" });
        await Expect(link).ToBeVisibleAsync();
        await Expect(link).ToHaveAttributeAsync("href", "#target");

        await link.ClickAsync();
        await Expect(page).ToHaveURLAsync(new Regex("#target$"));
    }

    [Fact]
    public async Task BothCards_AreInTheTabOrder()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var link = page.GetByRole(AriaRole.Link, new() { Name = "Linked card" });
        var checkbox = page.GetByRole(AriaRole.Checkbox, new() { Name = "Selectable card" });

        await link.FocusAsync();
        await page.Keyboard.PressAsync("Tab");
        await Expect(checkbox).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Shift+Tab");
        await Expect(link).ToBeFocusedAsync();
    }
}
