using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The VisualStudio shell must render its neutral chrome from its own palette: a Text-variant command
/// and a default Filled command are neutral, the tool window paints its own surface, and a disabled
/// field or button repaints muted instead of falling back to Fluent's light disabled palette.
/// </summary>
public sealed class VsChromeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task NeutralChromeAndDisabledStatesFollowTheShellPalette()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs-chrome");
        // The scope applies its theme and palette after the first paint, and the button colours
        // transition; read the computed values only once both have settled.
        await page.WaitForFunctionAsync(
            "() => document.querySelector('#vs-chrome')?.closest('.flare-theme-visualstudio') !== null" +
            " && document.querySelector('.flare-palette-vs-blue') !== null" +
            " && document.getAnimations().every(a => a.playState !== 'running')");

        var menuCommand = page.Locator($"#vs-chrome .flare-menubar .{Css.Classes.Button.Text}");
        await Expect(menuCommand).ToBeVisibleAsync();
        await ExpectColorAsync(menuCommand, "color", "#ffffff");

        // The tool window is a surface of its own, not the page behind it.
        var toolWindow = page.Locator("#vs-toolwindow .flare-toolpanel");
        await ExpectColorAsync(toolWindow, "backgroundColor", "#282828");

        // A default Filled command is the neutral VS command well; the accent is for Color="Primary".
        var neutral = page.Locator("#vs-toolwindow .flare-toolpanel__toolbar > button").Nth(0);
        await ExpectColorAsync(neutral, "backgroundColor", "#383838");
        await ExpectColorAsync(neutral, "color", "#ffffff");

        // Disabled Filled (Color Success): a muted shell container, not a white Fluent fill.
        var disabled = page.Locator("#vs-toolwindow .flare-toolpanel__toolbar > button").Nth(1);
        await Expect(disabled).ToBeDisabledAsync();
        await ExpectColorAsync(disabled, "backgroundColor", "#2c2c2c");
        await ExpectColorAsync(disabled, "color", "#858585");

        // Disabled Select: the VS disabled well and stroke, with the disabled foreground on its text.
        var field = page.Locator($"#vs-toolwindow .{Css.Classes.Select.Control}");
        await ExpectColorAsync(field, "backgroundColor", "#2c2c2c");
        await ExpectColorAsync(field, "borderBottomColor", "#3d3d3d");
        await Expect(page.Locator($"#vs-toolwindow .{Css.Classes.Input.Placeholder}"))
            .ToHaveCSSAsync("color", "rgb(133, 133, 133)");
    }

    // The scope applies its palette after the first paint and the colours transition towards it, so a
    // colour is read until it settles rather than once.
    private static async Task ExpectColorAsync(ILocator locator, string property, string expected)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        var actual = await CssColorAsync(locator, property);
        while (actual != expected && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50);
            actual = await CssColorAsync(locator, property);
        }

        Assert.Equal(expected, actual);
    }

    // color-mix() reaches the computed style as color(srgb ...) in Chromium. Painting the value into
    // a 1x1 canvas reads it back as plain channel bytes the assertions can compare.
    private static Task<string> CssColorAsync(ILocator locator, string property) =>
        locator.EvaluateAsync<string>(
            "(el, property) => { const css = getComputedStyle(el)[property];" +
            " const canvas = document.createElement('canvas'); canvas.width = 1; canvas.height = 1;" +
            " const ctx = canvas.getContext('2d'); ctx.fillStyle = css; ctx.fillRect(0, 0, 1, 1);" +
            " const data = ctx.getImageData(0, 0, 1, 1).data;" +
            " return '#' + [data[0], data[1], data[2]]" +
            "   .map(value => value.toString(16).padStart(2, '0')).join(''); }",
            property);
}
