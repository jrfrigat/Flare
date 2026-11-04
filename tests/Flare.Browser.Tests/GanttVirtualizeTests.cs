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
        await BrowserFonts.WaitAsync(page.Locator("#chart"));

        var first = await page.Locator($"#chart .{Css.Classes.Gantt.Row}").CountAsync();
        Assert.True(first < 120, $"the chart drew {first} of 500 rows");

        await page.EvaluateAsync($"() => document.querySelector('#chart .{Css.Classes.Gantt.Scroll}').scrollTop = 7000");
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}[data-row=\"r240\"]")).ToBeAttachedAsync();

        // [link end y in the connector layer, the target bar's middle in the same layer]
        // Rows and bars can land in separate virtualized renders. Capture both in the same browser
        // callback once they exist, without waiting for the coordinates to satisfy the assertion.
        var sample = await page.WaitForFunctionAsync($@"() => {{
            const svg = document.querySelector('#chart .{Css.Classes.Gantt.Links}');
            const barElement = document.querySelector('#chart .{Css.Classes.Gantt.Bar}[data-key=""w240""]');
            const link = svg?.querySelector('path[data-to=""w240""]');
            const verticals = link?.getAttribute('d')?.match(/V (-?[0-9.]+)/g);
            if (!svg || !barElement || !verticals?.length) return null;
            const top = svg.getBoundingClientRect().top;
            const ys = verticals.map(v => parseFloat(v.slice(2)));
            const bar = barElement.getBoundingClientRect();
            return [ys[ys.length - 1], bar.top + bar.height / 2 - top];
        }}");
        var m = (await sample.JsonValueAsync<double[]>())!;

        Assert.Equal(m[1], m[0], tolerance: 1.0);
        Assert.True(await page.Locator($"#chart .{Css.Classes.Gantt.Row}").CountAsync() < 120);
    }
}
