using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A click inside the overlay content runs its action without dismissing the surface, while a click on
/// the backdrop raises the dedicated backdrop event; the legacy surface click still sees both.
/// </summary>
public sealed class OverlayBackdropTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task BackdropAndContentClicksReachDifferentHandlers()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/overlay-backdrop");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.documentElement).getPropertyValue('{Css.Tokens.Color.Scrim}').trim() !== ''");
        await Expect(page.Locator($"#surface .{Css.Classes.Overlay.Scrim}")).ToBeVisibleAsync();

        await page.Locator("#overlay-action").ClickAsync();
        await Expect(page.Locator("#actions")).ToHaveTextAsync("1");
        await Expect(page.Locator("#backdrops")).ToHaveTextAsync("0");
        await Expect(page.Locator("#surfaces")).ToHaveTextAsync("1");

        var box = await page.Locator("#surface").BoundingBoxAsync()
            ?? throw new InvalidOperationException("The overlay surface has no box.");
        await page.Mouse.ClickAsync(box.X + box.Width - 20, box.Y + box.Height - 20);

        await Expect(page.Locator("#backdrops")).ToHaveTextAsync("1");
        await Expect(page.Locator("#surfaces")).ToHaveTextAsync("2");
        await Expect(page.Locator("#actions")).ToHaveTextAsync("1");
    }
}
