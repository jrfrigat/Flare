using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The split button's corner radii are named by physical sides in the token records, but the pair has
/// to mirror as a whole under RTL: the main action's outer corners stay on the outside and the seam
/// corners stay inside once the flex row reverses.
/// </summary>
public sealed class SplitButtonRtlTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string[] Sizes = ["Xs", "Sm", "Md", "Lg", "Xl"];

    private static async Task<IPage> OpenAsync(IBrowserContext context, BrowserFixture fixture)
    {
        var page = await context.NewPageAsync();
        // The button shape animates; a computed radius read mid-transition is the frame, not the target.
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/split-button");
        await Expect(page.Locator("[data-testid=split-Md]")).ToBeVisibleAsync();
        return page;
    }

    private static Task<string[]> RadiiAsync(ILocator element) =>
        element.EvaluateAsync<string[]>(@"el => {
            const s = getComputedStyle(el);
            return [s.borderTopLeftRadius, s.borderTopRightRadius, s.borderBottomRightRadius, s.borderBottomLeftRadius];
        }");

    private static ILocator Part(IPage page, string size, string cssClass) =>
        page.Locator($"[data-testid=split-{size}] .{cssClass}");

    private static void AssertMirrored(string[] ltr, string[] rtl, string what)
    {
        Assert.True(ltr[0] != ltr[1], $"{what}: corners are not asymmetric in LTR: [{string.Join(", ", ltr)}]");
        Assert.True(
            ltr[0] == rtl[1] && ltr[1] == rtl[0] && ltr[2] == rtl[3] && ltr[3] == rtl[2],
            $"{what}: LTR [{string.Join(", ", ltr)}] RTL [{string.Join(", ", rtl)}]");
    }

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task TheCornersMirrorInRtl(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture);
        await page.Locator(themeId == "fluent2" ? "#fluent" : "#md3e").ClickAsync();
        var root = page.Locator($".{Css.Classes.Theme.Root}");
        await Expect(root).ToHaveClassAsync(new Regex(Regex.Escape($"flare-theme-{themeId}")));
        await page.Locator("#ltr").ClickAsync();
        await Expect(root).ToHaveAttributeAsync("dir", "ltr");
        // Park the pointer off the controls: the expressive hover morph changes the corners.
        await page.Mouse.MoveAsync(0, 0);

        var ltr = new Dictionary<string, (string[] Main, string[] Trigger)>();
        foreach (var size in Sizes)
        {
            ltr[size] = (
                await RadiiAsync(Part(page, size, Css.Classes.SplitButton.Main)),
                await RadiiAsync(Part(page, size, Css.Classes.SplitButton.Trigger)));
        }

        await page.Locator("#rtl").ClickAsync();
        await Expect(root).ToHaveAttributeAsync("dir", "rtl");

        foreach (var size in Sizes)
        {
            AssertMirrored(ltr[size].Main, await RadiiAsync(Part(page, size, Css.Classes.SplitButton.Main)),
                $"{themeId} main {size}");
            AssertMirrored(ltr[size].Trigger, await RadiiAsync(Part(page, size, Css.Classes.SplitButton.Trigger)),
                $"{themeId} trigger {size}");
        }
    }
}
