using Microsoft.Playwright;

namespace Flare.Browser.Tests;

/// <summary>
/// Surface motion (B) keeps the panel in the reserved track when open and moves it off that track
/// while the track itself changes in one step. The content is pushed by the track, not by a padding
/// or margin guess.
/// </summary>
public sealed class LayoutMotionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string DrawerInner = Css.Classes.Layout.DrawerInner;
    private const string Content = Css.Classes.Layout.Content;

    [Fact]
    public async Task SurfaceMode_MovesThePanel_AndPushesTheContentByTheTrack()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/layout-motion");
        await page.WaitForSelectorAsync("#surface-panel");
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.querySelector('.{DrawerInner}')).position === 'absolute'");

        var open = await page.EvaluateAsync<double[]>(
            $$"""
            () => {
                const inner = document.querySelector('.{{DrawerInner}}').getBoundingClientRect();
                const content = document.querySelector('.{{Content}}').getBoundingClientRect();
                return [inner.left, inner.right, content.left];
            }
            """);
        Assert.True(open[0] <= 1, $"the open panel starts off its track: {open[0]:0.#}");
        Assert.True(open[1] > 200, $"the open panel has no width: {open[1]:0.#}");
        Assert.True(Math.Abs(open[1] - open[2]) <= 1,
            $"the content does not start at the panel edge: panel {open[1]:0.#}, content {open[2]:0.#}");

        await page.ClickAsync("#surface-toggle");
        await page.WaitForFunctionAsync(
            $$"""
            () => {
                const inner = document.querySelector('.{{DrawerInner}}').getBoundingClientRect();
                const content = document.querySelector('.{{Content}}').getBoundingClientRect();
                return Math.abs(content.left) <= 1 && inner.right <= 1;
            }
            """);

        var closed = await page.EvaluateAsync<double[]>(
            $$"""
            () => {
                const inner = document.querySelector('.{{DrawerInner}}').getBoundingClientRect();
                const content = document.querySelector('.{{Content}}').getBoundingClientRect();
                return [inner.right, content.left];
            }
            """);
        Assert.True(closed[1] <= 1, $"the content is still pushed after closing: {closed[1]:0.#}");
        Assert.True(closed[0] <= 1, $"the closed panel is still in view: {closed[0]:0.#}");
    }
}
