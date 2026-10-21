using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The scroll-to-top button sits inside the safe area at the reading-end corner, and an app's own
/// bottom bar pushes it up through a public token. Neither the reading direction nor another scroll
/// container changes that.
/// </summary>
public sealed class ScrollTopSafeAreaTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Button_ClearsTheSafeAreaAndTheBottomBar()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroll-top-safe-area");
        var button = page.Locator($".{Css.Classes.Scroll.Top}");
        await Expect(button).ToHaveCountAsync(1);
        await Expect(button).ToHaveCSSAsync("position", "fixed");
        // The theme's tokens have to be live before the fixed offsets mean anything.
        await page.WaitForFunctionAsync(
            $@"() => {{ const s = getComputedStyle(document.querySelector('.{Css.Classes.Scroll.Top}'));
                return s.getPropertyValue('{Css.Tokens.ScrollTopField.TopInset}').trim() !== ''
                    && s.getPropertyValue('{Css.Tokens.ScrollTopField.PanelOffset}').trim() !== ''; }}");

        // Show the button first: its hidden state slides it down by a quarter rem.
        await page.EvaluateAsync("() => window.scrollTo(0, 300)");
        await Expect(button).ToHaveCSSAsync("transform", "none");

        // Base: one inset from both edges (the theme's 1.5rem).
        var box = await RectAsync(page);
        Assert.Equal(900 - 24, box[2], 1);
        Assert.Equal(700 - 24, box[3], 1);

        // An app's own bottom bar adds through the public token.
        await page.Locator("#bar").ClickAsync();
        await Expect(button).ToHaveCSSAsync("margin-bottom", "56px");
        box = await RectAsync(page);
        Assert.Equal(700 - 80, box[3], 1);

        await page.Locator("#bar-clear").ClickAsync();
        await Expect(button).ToHaveCSSAsync("margin-bottom", "0px");

        // A safe-area inset is added on the physical edge the button sits on.
        var cdp = await context.NewCDPSessionAsync(page);
        await SetSafeAreaAsync(cdp, top: 0, bottom: 34, left: 0, right: 0);
        box = await RectAsync(page);
        Assert.Equal(700 - 58, box[3], 1);

        // The inline axis clears the inset on the side the button actually sits on.
        await SetSafeAreaAsync(cdp, top: 0, bottom: 0, left: 20, right: 30);
        box = await RectAsync(page);
        Assert.Equal(900 - 54, box[2], 1);
        Assert.Equal(700 - 24, box[3], 1);

        // RTL moves the button to the physical left, and the left inset is the one it clears.
        await page.Locator("#rtl").ClickAsync();
        await Expect(page.Locator("[dir='rtl']")).ToHaveCountAsync(1);
        box = await RectAsync(page);
        Assert.Equal(44, box[0], 1);
        Assert.True(box[2] < 450, $"The RTL button should sit on the left: right edge {box[2]}.");

        // Another scroll container on the page does not move it.
        await page.EvaluateAsync("() => document.querySelector('#scroller').scrollTop = 400");
        box = await RectAsync(page);
        Assert.Equal(44, box[0], 1);
    }

    private static async Task SetSafeAreaAsync(ICDPSession cdp, int top, int bottom, int left, int right)
        => await cdp.SendAsync("Emulation.setSafeAreaInsetsOverride", new Dictionary<string, object>
        {
            ["insets"] = new Dictionary<string, object>
            {
                ["top"] = top, ["bottom"] = bottom, ["left"] = left, ["right"] = right
            }
        });

    // x, y, right, bottom in viewport coordinates.
    private static Task<double[]> RectAsync(IPage page)
        => page.Locator($".{Css.Classes.Scroll.Top}").EvaluateAsync<double[]>(
            "el => { const r = el.getBoundingClientRect(); return [r.x, r.y, r.right, r.bottom]; }");
}
