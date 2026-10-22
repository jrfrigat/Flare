using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The Gantt script captures the virtualization flags in its listeners, and the component used to attach it only
/// on the first render. Turning Virtualize on after mounting must reattach the script: without it no Horizontal
/// report arrives, and a virtualized chart would never draw a column. Turning it off again draws the whole axis.
/// </summary>
public sealed class GanttRuntimeOptionsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task TurningVirtualizeOnAfterMounting_StartsCuttingRowsAndColumns()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt-runtime");

        var rows = page.Locator($"#chart .{Css.Classes.Gantt.Row}");
        var ticks = page.Locator(
            $"#chart .{Css.Classes.Gantt.ScaleRow}:not(.{Css.Classes.Gantt.ScaleRowMajor}) .{Css.Classes.Gantt.Tick}");

        await Expect(rows.First).ToBeVisibleAsync();
        await Expect(ticks).ToHaveCountAsync(60);
        await Expect(rows).ToHaveCountAsync(200);

        // On, without a reload: the reattached script reports the view, so some columns stay and the far ones go.
        await page.Locator("#toggle-virtualize").ClickAsync();
        await page.WaitForFunctionAsync(
            $"() => {{ const n = document.querySelectorAll('#chart .{Css.Classes.Gantt.ScaleRow}:not(.{Css.Classes.Gantt.ScaleRowMajor}) .{Css.Classes.Gantt.Tick}').length; return n > 0 && n < 60; }}");
        Assert.True(await rows.CountAsync() < 120, "the rows should be cut to a window");

        // Off again: every column comes back.
        await page.Locator("#toggle-virtualize").ClickAsync();
        await Expect(ticks).ToHaveCountAsync(60);
        await Expect(rows).ToHaveCountAsync(200);
    }
}
