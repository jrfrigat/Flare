using Flare.Css.Classes;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A 360px app bar with a brand, the color-mode toggle and two icon buttons has to stay on one row at
/// 200% text: the square controls are capped at the bar's height instead of growing with the root font
/// size and pushing the trailing block over the title, and the title truncates to give them the room.
/// </summary>
public sealed class TextScaleAppBarTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static string AppBarSelector => $"header.{Css.Classes.Layout.AppBar}";

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    public async Task ControlsStayOnOneRow(int textScale)
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 360, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/text-scale-appbar");
        await Expect(page.Locator("#appbar-trailing")).ToBeVisibleAsync();

        await page.EvaluateAsync($"document.documentElement.style.fontSize = '{textScale}%'");
        // The theme's root classes and stylesheets land after the first paint; until then every
        // --flare-* token is empty and the bar is measured unstyled. Wait for a token-backed
        // computed value and the web fonts, not just for the width to settle.
        await page.WaitForFunctionAsync(
            "args => { const a = document.querySelector(args.sel); if (!a || document.fonts.status !== 'loaded') return false; "
            + "const cs = getComputedStyle(a); const root = getComputedStyle(document.documentElement); "
            + "return cs.paddingLeft !== '0px' && root.getPropertyValue(args.token).trim() !== ''; }",
            new { sel = AppBarSelector, token = Css.Tokens.Spacing.S8 });

        var bar = page.Locator(AppBarSelector);
        var barHeight = await bar.EvaluateAsync<double>("el => el.getBoundingClientRect().height");
        var barOverflow = await bar.EvaluateAsync<double>("el => el.scrollWidth - el.clientWidth");
        var shellOverflow = await page.Locator("#appbar-shell")
            .EvaluateAsync<double>("el => el.scrollWidth - el.clientWidth");
        Assert.True(barOverflow <= 1, $"the app bar overflows by {barOverflow:0.#}px at {textScale}% text");
        Assert.True(shellOverflow <= 1, $"the app bar overflows its shell by {shellOverflow:0.#}px at {textScale}% text");

        // Every control the trailing block holds is square, stays inside the bar, and is not squeezed.
        var controls = await page.Locator("#appbar-trailing button").EvaluateAllAsync<double[][]>(
            "els => els.map(e => { const r = e.getBoundingClientRect(); return [r.width, r.height]; })");
        Assert.Equal(3, controls.Length);
        foreach (var control in controls)
        {
            Assert.True(control[1] <= barHeight + 0.5,
                $"a control is {control[1]:0.#}px tall in a {barHeight:0.#}px bar at {textScale}% text");
            Assert.True(control[0] <= barHeight + 0.5,
                $"a control is {control[0]:0.#}px wide in a {barHeight:0.#}px bar at {textScale}% text");
            Assert.True(control[0] >= 24, $"a control shrank to {control[0]:0.#}px at {textScale}% text");
        }

        var brand = await page.Locator("#appbar-brand")
            .EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.x, r.right]; }");
        var trailing = await page.Locator("#appbar-trailing")
            .EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.x, r.right]; }");
        var shell = await page.Locator("#appbar-shell")
            .EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.x, r.right]; }");
        Assert.True(trailing[0] >= brand[1] - 0.5,
            $"the trailing block starts at {trailing[0]:0.#}, before the brand ends at {brand[1]:0.#}");
        Assert.True(trailing[1] <= shell[1] + 0.5,
            $"the trailing block ends at {trailing[1]:0.#}, past the shell edge at {shell[1]:0.#}");

        // The title is what yields: it truncates at 200%, and keeps its full text at 100%.
        var brandTruncated = await page.Locator("#appbar-brand")
            .EvaluateAsync<bool>("el => el.scrollWidth > el.clientWidth + 1");
        Assert.Equal(textScale > 100, brandTruncated);
    }
}
