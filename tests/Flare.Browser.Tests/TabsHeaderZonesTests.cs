using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A tab bar between header zones shows its scroll arrows only while the tabs overflow. The arrows must not
/// change the room the row gives the bar: when they did, showing them pushed the end zone onto its own line,
/// the bar then fitted, the arrows went away and the zone came back, over and over.
/// </summary>
public sealed class TabsHeaderZonesTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    // [samples taken, distinct layouts seen, overflowing, arrows shown, end zone overlaps the bar]
    private const string Measure = @"async width => {
        document.getElementById('host').style.inlineSize = width + 'px';
        const frame = () => new Promise(r => requestAnimationFrame(() => r()));
        // One resize may take a few round trips (overflow report, arrows render); a loop never ends.
        for (let i = 0; i < 6; i++) await frame();
        await new Promise(r => setTimeout(r, 300));
        const tabs = document.querySelector('.flare-tabs');
        const bar = tabs.querySelector('.flare-tabs__bar');
        const end = tabs.querySelector('.flare-tabs__header-end');
        const seen = new Set();
        for (let i = 0; i < 20; i++) {
            await new Promise(r => setTimeout(r, 25));
            seen.add(Math.round(end.getBoundingClientRect().top) + ':' + tabs.querySelectorAll('.flare-tabs__scroll').length);
        }
        const b = bar.getBoundingClientRect(), e = end.getBoundingClientRect();
        const overlaps = e.left < b.right - 0.5 && e.right > b.left + 0.5 && e.top < b.bottom - 0.5 && e.bottom > b.top + 0.5;
        return [20, seen.size, bar.scrollWidth > bar.clientWidth + 1 ? 1 : 0,
            tabs.querySelectorAll('.flare-tabs__scroll').length ? 1 : 0, overlaps ? 1 : 0];
    }";

    [Fact]
    public async Task EveryWidth_SettlesOnOneLayout_WithArrowsExactlyWhenTheTabsOverflow()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 900, Height = 600 } });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-header-zones");
        await Expect(page.Locator("[role=tab]").First).ToBeVisibleAsync();

        var failures = new List<string>();
        for (var width = 260; width <= 760; width += 10)
        {
            var m = await page.EvaluateAsync<int[]>(Measure, width);
            if (m[1] != 1) failures.Add($"{width}px: layout kept changing ({m[1]} states)");
            if (m[2] != m[3]) failures.Add($"{width}px: overflowing={m[2]} but arrows={m[3]}");
            if (m[4] == 1) failures.Add($"{width}px: the end zone covers the tabs");
        }

        Assert.True(failures.Count == 0, string.Join("; ", failures));
    }
}
