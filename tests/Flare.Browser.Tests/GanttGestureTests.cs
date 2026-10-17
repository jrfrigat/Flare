using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Bars are moved, resized, re-rowed and linked with a real pointer through the shared Flare drag gesture: the
/// browser draws the preview and .NET hears one call when the gesture ends. Days are 40px wide, rows 30px tall.
/// </summary>
public sealed class GanttGestureTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static string Bar(string key) => $"#chart .{Css.Classes.Gantt.Bar}[data-key=\"{key}\"]";

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt");
        await Expect(page.Locator(Bar("w1"))).ToBeVisibleAsync();
        // The module attaches after the first render; until then a press is the browser's own.
        await ListenerWait.ForAsync(page, "#chart", "pointerdown");
        return page;
    }

    // The slot's left and width once the page has applied the change, and whether a preview transform is left over.
    private static Task<double[]> SlotAsync(IPage page, string key) => page.EvaluateAsync<double[]>($@"() => {{
        const slot = document.querySelector('{Bar(key)}').closest('.{Css.Classes.Gantt.BarSlot}');
        return [parseFloat(slot.style.left), parseFloat(slot.style.width), slot.style.transform ? 1 : 0];
    }}");

    private static async Task DragAsync(IPage page, float x, float y, float dx, float dy)
    {
        await page.Mouse.MoveAsync(x, y);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(x + dx / 4, y + dy / 4, new() { Steps = 3 });
        await page.Mouse.MoveAsync(x + dx, y + dy, new() { Steps = 6 });
        await page.Mouse.UpAsync();
    }

    [Fact]
    public async Task DraggingABar_MovesItByWholeDays()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var box = (await page.Locator(Bar("w1")).BoundingBoxAsync())!;

        var before = await SlotAsync(page, "w1");

        await DragAsync(page, box.X + box.Width / 2, box.Y + box.Height / 2, 85, 0);

        await Expect(page.Locator("#result")).ToHaveTextAsync("w1@10-15..10-18");
        // The bar stays where the page put it: the preview's own geometry does not come back over it.
        await page.WaitForTimeoutAsync(300);
        var after = await SlotAsync(page, "w1");
        Assert.Equal(before[0] + 80, after[0], 0.5);
        Assert.Equal(0, after[2]);
    }

    [Fact]
    public async Task DraggingTheEndHandle_ResizesTheEnd()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var handle = (await page.Locator($"{Bar("w1")} .{Css.Classes.Gantt.HandleEnd}").BoundingBoxAsync())!;

        var before = await SlotAsync(page, "w1");

        await DragAsync(page, handle.X + handle.Width / 2, handle.Y + handle.Height / 2, 40, 0);

        await Expect(page.Locator("#result")).ToHaveTextAsync("w1@10-13..10-17");
        await page.WaitForTimeoutAsync(300);
        Assert.Equal(before[1] + 40, (await SlotAsync(page, "w1"))[1], 0.5);
    }

    [Fact]
    public async Task DraggingABarDown_MovesItToTheRowUnderThePointer()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var box = (await page.Locator(Bar("w1")).BoundingBoxAsync())!;

        await DragAsync(page, box.X + box.Width / 2, box.Y + box.Height / 2, 0, 30);

        await Expect(page.Locator("#result")).ToHaveTextAsync("w1@10-13..10-16>r2");
    }

    [Fact]
    public async Task DrawingFromTheLinkHandle_ToAnotherBar_CreatesALink()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator(Bar("w1")).HoverAsync();
        var grip = (await page.Locator($"{Bar("w1")} .{Css.Classes.Gantt.LinkHandle}").BoundingBoxAsync())!;
        var target = (await page.Locator(Bar("w4")).BoundingBoxAsync())!;
        var x = grip.X + grip.Width / 2;
        var y = grip.Y + grip.Height / 2;

        await DragAsync(page, x, y, target.X + target.Width / 2 - x, target.Y + target.Height / 2 - y);

        await Expect(page.Locator("#result")).ToHaveTextAsync("link w1>w4");
    }

    [Fact]
    public async Task CtrlArrowDown_MovesTheBarToTheNextRow_AndKeepsTheFocusOnIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator(Bar("w1")).FocusAsync();

        await page.Keyboard.PressAsync("Control+ArrowDown");

        await Expect(page.Locator("#result")).ToHaveTextAsync("w1@10-13..10-16>r2");
        await page.WaitForFunctionAsync("document.activeElement?.dataset.key === 'w1'");
        Assert.Equal(0, await page.EvaluateAsync<double>($"document.querySelector('#chart .{Css.Classes.Gantt.Scroll}').scrollTop"));
    }
}
