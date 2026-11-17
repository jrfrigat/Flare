using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The bottom auto-hide flyout overlays the content above its rail. The shared floating rule sets
/// <c>top: 0</c>, so the bottom rule has to reset it to <c>auto</c>; otherwise the over-constrained
/// body starts at the top of the 32px region and extends past the layout, out of sight.
/// </summary>
public sealed class IdeBottomFlyoutTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task BottomFlyoutBody_SitsAboveTheRail_AndKeepsTheConfiguredHeight()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-bottom-flyout");

        var bottom = page.Locator($"#flyout .{Css.Classes.Ide.Layout.Bottom}");
        var body = bottom.Locator($":scope > .{Css.Classes.Ide.Layout.PanelBody}");
        var rail = bottom.Locator($":scope > .{Css.Classes.Ide.Layout.Rail}");
        await Expect(body).ToBeVisibleAsync();
        await Expect(rail).ToBeVisibleAsync();
        Assert.True(await bottom.EvaluateAsync<bool>(
                $"el => el.classList.contains('{Css.Classes.Ide.Layout.PanelFloating}')"),
            "the unpinned bottom region should be in the floating state");

        // [body top, body bottom, body height, rail top, rail height, configured bottom height]
        var m = await page.EvaluateAsync<double[]>(@"() => {
            const layout = document.getElementById('flyout');
            const bottom = layout.querySelector('.flare-ide-layout__bottom');
            const body = bottom.querySelector(':scope > .flare-ide-layout__panel-body');
            const rail = bottom.querySelector(':scope > .flare-ide-layout__rail');
            const b = body.getBoundingClientRect();
            const r = rail.getBoundingClientRect();
            const configured = parseFloat(getComputedStyle(layout).getPropertyValue('--flare-ide-bottom-height'))
                || parseFloat(layout.style.getPropertyValue('--flare-ide-bottom-height'));
            return [b.top, b.bottom, b.height, r.top, r.height, configured];
        }");

        Assert.True(m[5] > 0, "the layout should publish --flare-ide-bottom-height");
        Assert.Equal(m[5], m[2], tolerance: 1.0);
        Assert.True(m[1] <= m[3] + 0.5,
            $"the flyout body bottom ({m[1]}) should sit at or above the rail top ({m[3]})");
        Assert.True(m[0] < m[3] - 1,
            $"the flyout body top ({m[0]}) should rise above the rail top ({m[3]})");
    }
}
