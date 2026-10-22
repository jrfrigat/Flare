using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

// A virtualized chart can live inside an ancestor that scrolls instead of scrolling itself. The rows
// must follow that ancestor: scrolling it is the only thing that moves the chart on screen.
public sealed class GanttExternalScrollTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task ScrollingAnAncestor_MovesTheVirtualizedWindow()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt-external-scroll");
        await Expect(page.Locator("#external")).ToBeAttachedAsync();
        await Expect(page.Locator("#chart")).ToBeVisibleAsync();

        // The chart starts below the ancestor's viewport. Scrolling the ancestor (not the chart) to it
        // must bring the first rows in.
        await page.EvaluateAsync("() => document.querySelector('#external').scrollTop = 600");
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}[data-row=\"r0\"]")).ToBeAttachedAsync();

        // A second scroll of the same ancestor moves the window far down; the first row leaves the DOM.
        await page.EvaluateAsync("() => document.querySelector('#external').scrollTop = 3000");
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}[data-row=\"r80\"]")).ToBeAttachedAsync();
        await Expect(page.Locator($"#chart .{Css.Classes.Gantt.Row}[data-row=\"r0\"]")).ToHaveCountAsync(0);

        var inner = await page.EvaluateAsync<double>(
            $"() => document.querySelector('#chart .{Css.Classes.Gantt.Scroll}').scrollTop");
        Assert.Equal(0, inner);
    }
}
