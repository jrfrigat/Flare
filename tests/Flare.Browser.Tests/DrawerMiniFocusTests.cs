using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The mini rail used to widen on hover only, so a keyboard user tabbed through a clipped panel. Focus
/// inside the drawer has to widen it the same way, keep it open while focus moves between its own
/// controls, and give the width back when focus leaves.
/// </summary>
public sealed class DrawerMiniFocusTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string Mini = $".{Css.Classes.Drawer.Mini}";

    private async Task<IPage> OpenAsync(IBrowserContext context, int width = 900)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.SetViewportSizeAsync(width, 800);
        await page.GotoAsync($"{fixture.BaseUrl}/drawer-mini");
        await Expect(page.Locator(Mini)).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        // The rail width comes from the theme's token bundle; before it lands the width is the
        // stylesheet fallback and every measurement below compares the wrong numbers.
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && parseFloat(getComputedStyle(el).width) > 0; }",
            Mini);
        return page;
    }

    private static Task<double> WidthAsync(IPage page) =>
        page.Locator(Mini).EvaluateAsync<double>("el => el.getBoundingClientRect().width");

    private static Task WaitForWidthAsync(IPage page, double rail, bool expanded) =>
        page.WaitForFunctionAsync(
            """
            ([sel, rail, expanded]) => {
                const el = document.querySelector(sel);
                if (!el) return false;
                const width = el.getBoundingClientRect().width;
                return expanded ? width > rail + 1 : width <= rail + 1;
            }
            """,
            new object[] { Mini, rail, expanded });

    [Fact]
    public async Task KeyboardFocusExpandsTheRailAndMovingInsideKeepsItOpen()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var rail = await WidthAsync(page);

        await page.Locator("#mini-link").FocusAsync();
        await WaitForWidthAsync(page, rail, expanded: true);
        var expanded = await WidthAsync(page);
        Assert.True(expanded > rail + 1, $"the rail did not widen on focus: {rail:0.#} -> {expanded:0.#}");

        // Focus moving between the drawer's own controls is not leaving it.
        await page.Locator("#mini-button").FocusAsync();
        await Task.Delay(100, Xunit.TestContext.Current.CancellationToken);
        Assert.True(Math.Abs(await WidthAsync(page) - expanded) <= 1,
            "the rail changed width while focus moved inside it");

        await page.Locator("#outside").FocusAsync();
        await WaitForWidthAsync(page, rail, expanded: false);

        var overflow = await page.EvaluateAsync<int>(
            "document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"the page gained {overflow}px of horizontal scroll");
    }

    [Fact]
    public async Task TheExpandedRailFitsA320Viewport()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width: 320);
        var rail = await WidthAsync(page);

        await page.Locator("#mini-link").FocusAsync();
        await WaitForWidthAsync(page, rail, expanded: true);

        var width = await WidthAsync(page);
        Assert.True(width <= 320.5, $"the expanded rail is {width:0.#}px wide in a 320px viewport");
        var overflow = await page.EvaluateAsync<int>(
            "document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"the page gained {overflow}px of horizontal scroll");
    }
}
