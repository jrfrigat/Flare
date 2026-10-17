using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A virtualized Gantt keeps only the rows near the view in the DOM, and its connectors still meet their bars: the
/// window is cut from the chart's own layout, so a row's position is exact rather than estimated.
/// </summary>
public sealed class GanttVirtualizeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task FiveHundredRows_KeepOnlyAWindow_AndConnectorsMeetTheirBars_AfterAScroll()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt");
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}").First).ToBeVisibleAsync();

        var first = await page.Locator($"#chart .{Css.Classes.Gantt.Row}").CountAsync();
        Assert.True(first < 120, $"the chart drew {first} of 500 rows");

        await page.EvaluateAsync($"() => document.querySelector('#chart .{Css.Classes.Gantt.Scroll}').scrollTop = 7000");
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}[data-row=\"r240\"]")).ToBeAttachedAsync();

        // [link end y in the connector layer, the target bar's middle in the same layer]
        var m = await page.EvaluateAsync<double[]>($@"() => {{
            const svg = document.querySelector('#chart .{Css.Classes.Gantt.Links}');
            const top = svg.getBoundingClientRect().top;
            const link = svg.querySelector('path[data-to=""w240""]');
            const ys = link.getAttribute('d').match(/V (-?[0-9.]+)/g).map(v => parseFloat(v.slice(2)));
            const bar = document.querySelector('#chart .{Css.Classes.Gantt.Bar}[data-key=""w240""]').getBoundingClientRect();
            return [ys[ys.length - 1], bar.top + bar.height / 2 - top];
        }}");

        Assert.Equal(m[1], m[0], tolerance: 1.0);
        Assert.True(await page.Locator($"#chart .{Css.Classes.Gantt.Row}").CountAsync() < 120);
    }
}
