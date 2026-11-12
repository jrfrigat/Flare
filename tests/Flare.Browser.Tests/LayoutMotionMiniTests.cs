using Microsoft.Playwright;

namespace Flare.Browser.Tests;

/// <summary>
/// Surface motion changes only the track drawer. A Mini drawer that floats (a responsive layout below
/// the mobile breakpoint) is an overlay: it has to slide in as one full-width panel, keep its own
/// background and shadow, and not animate the width of its inner.
/// </summary>
public sealed class LayoutMotionMiniTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string Drawer = Css.Classes.Layout.Drawer;
    private const string DrawerInner = Css.Classes.Layout.DrawerInner;
    private const string DrawerFloating = Css.Classes.Layout.DrawerFloating;
    private const string DrawerOpen = Css.Classes.Layout.DrawerOpenMod;
    private const string Content = Css.Classes.Layout.Content;

    private async Task<IPage> OpenAsync(IBrowserContext context, int width)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(width, 800);
        await page.GotoAsync($"{fixture.BaseUrl}/layout-motion-mini");
        await page.WaitForSelectorAsync("#mini-panel");
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        // The rail width comes from the theme's token bundle; before it lands both widths fall back
        // to the stylesheet defaults and the comparison below would read the wrong numbers.
        await page.WaitForFunctionAsync(
            $"() => parseFloat(getComputedStyle(document.querySelector('.{DrawerInner}')).width) > 0");
        return page;
    }

    [Fact]
    public async Task FloatingMini_SlidesWithoutAnimatingTheInnerWidth()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width: 480);
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('.{Drawer}').classList.contains('{DrawerFloating}')");
        // The layout flips the drawer from a track Mini to a floating one once the viewport service
        // reports the breakpoint; the inner's width is still settling from that flip. Let it finish,
        // so the window below measures only the open transition.
        await page.WaitForTimeoutAsync(600);

        await page.ClickAsync("#mini-toggle");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('.{Drawer}').classList.contains('{DrawerOpen}')");

        // Sample the whole open transition: a floating panel must already be at its full width in
        // every frame. A rail that grows while the panel slides is the surface rule leaking in.
        var metrics = await page.EvaluateAsync<double[]>(
            $$"""
            async () => {
                const nav = document.querySelector('.{{Drawer}}');
                const inner = nav.querySelector('.{{DrawerInner}}');
                const full = nav.getBoundingClientRect().width;
                let min = full;
                const start = performance.now();
                await new Promise(resolve => {
                    const tick = () => {
                        min = Math.min(min, inner.getBoundingClientRect().width);
                        if (performance.now() - start < 350) requestAnimationFrame(tick); else resolve();
                    };
                    requestAnimationFrame(tick);
                });
                const cs = getComputedStyle(nav);
                return [full, min,
                        cs.backgroundColor === 'rgba(0, 0, 0, 0)' ? 0 : 1,
                        cs.boxShadow === 'none' ? 0 : 1];
            }
            """);
        Assert.True(metrics[0] > 200, $"the drawer is not full width: {metrics[0]:0.#}");
        Assert.True(metrics[1] > metrics[0] - 5,
            $"the inner animated its width during the slide: min {metrics[1]:0.#} of {metrics[0]:0.#}");
        Assert.True(metrics[2] > 0, "the floating drawer lost its background");
        Assert.True(metrics[3] > 0, "the floating drawer lost its shadow");
    }

    [Fact]
    public async Task TrackMini_StaysARailClosedAndTheFullPanelOpen()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width: 1200);
        Assert.False(await page.EvaluateAsync<bool>(
            $"() => document.querySelector('.{Drawer}').classList.contains('{DrawerFloating}')"));

        var closed = await page.EvaluateAsync<double[]>(
            $$"""
            () => {
                const nav = document.querySelector('.{{Drawer}}');
                const inner = nav.querySelector('.{{DrawerInner}}');
                return [nav.getBoundingClientRect().width, inner.getBoundingClientRect().width,
                        getComputedStyle(inner).position === 'absolute' ? 1 : 0];
            }
            """);
        Assert.True(closed[1] < 120, $"the closed rail is not a rail: {closed[1]:0.#}");
        Assert.True(closed[2] > 0, "the track drawer's surface is not the inner");

        await page.ClickAsync("#mini-toggle");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('.{DrawerInner}').getBoundingClientRect().width > 200");

        var opened = await page.EvaluateAsync<double[]>(
            $$"""
            () => {
                const nav = document.querySelector('.{{Drawer}}');
                const inner = nav.querySelector('.{{DrawerInner}}');
                return [inner.getBoundingClientRect().width, inner.getBoundingClientRect().right];
            }
            """);
        Assert.True(opened[0] > 200, $"the open track drawer is still a rail: {opened[0]:0.#}");
        // The content waits for the track step, then starts exactly at the panel edge.
        await page.WaitForFunctionAsync(
            $"() => Math.abs(document.querySelector('.{Content}').getBoundingClientRect().left - document.querySelector('.{DrawerInner}').getBoundingClientRect().right) <= 1");
    }
}
