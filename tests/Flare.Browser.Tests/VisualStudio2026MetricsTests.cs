using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The measured VS 2026 component metrics, read from the rendered shell: a 23px input well with a
/// #353535 fill and a #454545 stroke on 4px corners, a 24px command button carrying a 16px glyph, a
/// single 1px #9184EE focus line without a glow, and a 2px square scrollbar thumb.
/// </summary>
public sealed class VisualStudio2026MetricsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IBrowserContext> OpenDarkContextAsync()
    {
        var context = await fixture.Browser.NewContextAsync();
        await context.AddInitScriptAsync("""
            localStorage.setItem('flare-theme', 'visualstudio');
            localStorage.setItem('flare-palette', 'vs-blue');
            localStorage.setItem('flare-mode', 'dark');
            """);
        return context;
    }

    [Fact]
    public async Task FieldWell_IsTwentyThreePixelsOnFourPixelCorners()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-field .{Css.Classes.Input.Field}");

        await Expect(well).ToHaveCSSAsync("height", "23px");
        await Expect(well).ToHaveCSSAsync("border-radius", "4px");
        await Expect(well).ToHaveCSSAsync("background-color", "rgb(53, 53, 53)");
        await Expect(well).ToHaveCSSAsync("border-top-color", "rgb(69, 69, 69)");
    }

    [Fact]
    public async Task Focus_IsOneAccentLineWithoutAGlow()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-field .{Css.Classes.Input.Field}");

        await page.Locator("#metric-field input").FocusAsync();

        await Expect(well).ToHaveCSSAsync("outline-color", "rgb(145, 132, 238)");
        await Expect(well).ToHaveCSSAsync("outline-style", "solid");
        await Expect(well).ToHaveCSSAsync("outline-width", "1px");
        await Expect(well).ToHaveCSSAsync("box-shadow", "none");
    }

    [Fact]
    public async Task ComboWell_MatchesTheFieldHeight()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-combo .{Css.Classes.Input.Field}");

        await Expect(well).ToHaveCSSAsync("height", "23px");
        await Expect(well).ToHaveCSSAsync("border-radius", "4px");
        await Expect(well).ToHaveCSSAsync("background-color", "rgb(53, 53, 53)");
    }

    [Fact]
    public async Task CommandButton_IsTwentyFourPixelsWithASixteenPixelIcon()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var button = page.Locator("#metric-command");

        await Expect(button).ToHaveCSSAsync("height", "24px");
        await Expect(button).ToHaveCSSAsync("width", "24px");
        await Expect(button).ToHaveCSSAsync("border-radius", "4px");

        var icon = button.Locator($".{Css.Classes.Button.Icon}");
        await Expect(icon).ToHaveCSSAsync("width", "16px");
        await Expect(icon).ToHaveCSSAsync("height", "16px");
    }

    [Fact]
    public async Task Scrollbar_UsesTheTwoPixelWebKitPath()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var scroller = page.Locator("#metric-scroller");

        // Chromium paints ::-webkit-scrollbar only while the standard properties stay at auto; the
        // headless browser uses overlay scrollbars, so the layout width is not a usable measurement,
        // but the mechanism and every token that feeds it are.
        await Expect(scroller).ToHaveCSSAsync("scrollbar-width", "auto");
        await Expect(scroller).ToHaveCSSAsync("scrollbar-color", "auto");
        await page.WaitForFunctionAsync($"() => document.documentElement.classList.contains('{Css.Classes.Theme.ModeDark}')");

        var values = await page.EvaluateAsync<string[]>("""
            () => {
                const s = getComputedStyle(document.querySelector('#metric-scroller'));
                return [
                    s.getPropertyValue('--flare-scrollbar-size').trim(),
                    s.getPropertyValue('--flare-scrollbar-thumb').trim(),
                    s.getPropertyValue('--flare-scrollbar-track').trim(),
                    s.getPropertyValue('--flare-scrollbar-radius').trim(),
                ];
            }
            """);
        Assert.Equal("2px", values[0]);
        Assert.Equal("#9D9D9D", values[1]);
        Assert.Equal("#282828", values[2]);
        Assert.Equal("0px", values[3]);
    }
}
