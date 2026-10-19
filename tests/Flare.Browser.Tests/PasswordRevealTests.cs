using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A disabled field must not let its value be revealed: the reveal control is natively disabled, skipped
/// by Tab, and even a forced click leaves the input masked. Read-only keeps the reveal working - the
/// value is already visible in meaning, only editing is blocked.
/// </summary>
public sealed class PasswordRevealTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/password-reveal");
        await Expect(page.Locator("#normal input")).ToBeVisibleAsync();
        return page;
    }

    private static ILocator Reveal(IPage page, string id)
        => page.Locator($"#{id} button[aria-pressed]");

    [Fact]
    public async Task ADisabledField_DoesNotRevealItsValue()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var reveal = Reveal(page, "disabled");

        await Expect(reveal).ToBeDisabledAsync();
        await Expect(page.Locator("#disabled input")).ToHaveAttributeAsync("type", "password");

        // A forced click is the way around a native disabled button; it must change nothing.
        await reveal.ClickAsync(new() { Force = true });
        await Expect(page.Locator("#disabled input")).ToHaveAttributeAsync("type", "password");
        await Expect(reveal).ToHaveAttributeAsync("aria-pressed", "false");

        // Tab from the input skips the disabled reveal entirely.
        await page.Locator("#disabled input").FocusAsync();
        await page.Keyboard.PressAsync("Tab");
        await Expect(reveal).Not.ToBeFocusedAsync();
    }

    [Fact]
    public async Task ANormalField_RevealsAndAnnouncesIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var reveal = Reveal(page, "normal");

        await Expect(page.Locator("#normal input")).ToHaveAttributeAsync("type", "password");
        await reveal.ClickAsync();
        await Expect(page.Locator("#normal input")).ToHaveAttributeAsync("type", "text");
        await Expect(reveal).ToHaveAttributeAsync("aria-pressed", "true");

        await reveal.ClickAsync();
        await Expect(page.Locator("#normal input")).ToHaveAttributeAsync("type", "password");
        await Expect(reveal).ToHaveAttributeAsync("aria-pressed", "false");
    }

    [Fact]
    public async Task AReadOnlyField_StillReveals()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var reveal = Reveal(page, "readonly");

        await Expect(reveal).ToBeEnabledAsync();
        await reveal.ClickAsync();
        await Expect(page.Locator("#readonly input")).ToHaveAttributeAsync("type", "text");
        await Expect(reveal).ToHaveAttributeAsync("aria-pressed", "true");
    }
}
