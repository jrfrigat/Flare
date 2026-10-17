using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The Gantt row-header column is resized with the shared FlareSplitter in its one-value mode: the column follows the
/// pointer through a CSS variable and .NET hears the width once, when the drag, the key or the step is over.
/// </summary>
public sealed class GanttSideColumnTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string Edge = $"#chart .{Css.Classes.Gantt.SideSplitter}";

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt-side");
        await Expect(page.Locator(Edge)).ToBeAttachedAsync();
        await page.WaitForTimeoutAsync(300);
        return page;
    }

    private static Task<double> CornerWidthAsync(IPage page) =>
        page.EvaluateAsync<double>($"document.querySelector('#chart .{Css.Classes.Gantt.Corner}').getBoundingClientRect().width");

    [Fact]
    public async Task DraggingTheEdge_WidensTheColumn_AndReportsOnce()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var box = (await page.Locator(Edge).BoundingBoxAsync())!;
        var x = box.X + box.Width / 2;
        var y = box.Y + box.Height / 2;

        await page.Mouse.MoveAsync(x, y);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(x + 30, y, new() { Steps = 4 });
        await page.Mouse.MoveAsync(x + 60, y, new() { Steps = 4 });
        Assert.Equal(260, await CornerWidthAsync(page), 1.0);
        await page.Mouse.UpAsync();

        await Expect(page.Locator("#result")).ToHaveTextAsync("260px");
    }

    [Fact]
    public async Task ArrowKeys_Home_AndADoubleClick_ChangeAndRestoreTheWidth()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator(Edge).FocusAsync();

        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("#result")).ToHaveTextAsync("216px");
        Assert.Equal(216, await CornerWidthAsync(page), 1.0);

        await page.Keyboard.PressAsync("Home");
        await Expect(page.Locator("#result")).ToHaveTextAsync("216px,200px");

        await page.Keyboard.PressAsync("ArrowRight");
        // The middle of the edge: its ends carry the -/+ steps.
        var box = (await page.Locator(Edge).BoundingBoxAsync())!;
        await page.Mouse.DblClickAsync(box.X + box.Width / 2, box.Y + box.Height / 2);
        await Expect(page.Locator("#result")).ToHaveTextAsync("216px,200px,216px,200px");
    }
}
