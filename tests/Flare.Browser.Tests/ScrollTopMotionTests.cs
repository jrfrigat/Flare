using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A smooth programmatic scroll follows the reader's motion preference: a reduced-motion reader gets
/// the jump, everyone else keeps the animation, and a preference changed while the page is open applies
/// to the next scroll. The container path of <c>FlareScrollTop</c> is what is exercised.
/// </summary>
public sealed class ScrollTopMotionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const double StartOffset = 600;

    [Fact]
    public async Task BackToTop_JumpsUnderReducedMotionAndAnimatesOtherwise()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ReducedMotion = ReducedMotion.NoPreference,
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/scroll-top-motion");
        await page.WaitForSelectorAsync("#scroller");

        var first = await ScrollToTopAndCollectAsync(page);
        Assert.True(first.Any(v => v > 0 && v < StartOffset),
            "The smooth mode should pass through intermediate offsets: " + string.Join(",", first));

        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        var reduced = await ScrollToTopAndCollectAsync(page);
        Assert.DoesNotContain(reduced, v => v > 0 && v < StartOffset);
        Assert.Equal(0d, reduced.LastOrDefault());

        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.NoPreference });
        var restored = await ScrollToTopAndCollectAsync(page);
        Assert.True(restored.Any(v => v > 0 && v < StartOffset),
            "Back in the normal mode the animation should return: " + string.Join(",", restored));
    }

    // Puts the container back at the start offset, arms a scroll sampler, clicks the button and returns
    // the offsets the browser reported on the way back to the top.
    private static async Task<double[]> ScrollToTopAndCollectAsync(IPage page)
    {
        await page.EvaluateAsync(
            "() => { const el = document.querySelector('#scroller'); el.scrollTop = 600; window.__scrollSamples = []; }");
        await Expect(page.Locator($".{Css.Classes.Scroll.TopVisible}")).ToHaveCountAsync(1);

        var collecting = page.EvaluateAsync(@"() => new Promise(resolve => {
            const el = document.querySelector('#scroller');
            const onScroll = () => {
                window.__scrollSamples.push(el.scrollTop);
                if (el.scrollTop === 0) { el.removeEventListener('scroll', onScroll); resolve(); }
            };
            el.addEventListener('scroll', onScroll);
            document.querySelector('#back').click();
        })");
        await collecting.WaitAsync(TimeSpan.FromSeconds(15));

        return await page.EvaluateAsync<double[]>("() => window.__scrollSamples");
    }
}
