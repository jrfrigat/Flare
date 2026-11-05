using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class SchedulerInteractionTests(BrowserFixture fixture, ITestOutputHelper output) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task StandardEditorCreatesUpdatesAndDeletesApplicationEvents()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator(".flare-scheduler__slot[data-flare-tree-index='20']").DblClickAsync();
        var dialog = page.GetByRole(AriaRole.Dialog);
        await dialog.GetByRole(AriaRole.Textbox, new() { Name = "Title", Exact = true }).FillAsync("New appointment");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).ClickAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("Create:10:00-10:30:Occurrence:False");
        var item = page.GetByRole(AriaRole.Button, new() { Name = "New appointment", Exact = true });
        await item.DblClickAsync();
        await Expect(dialog.GetByRole(AriaRole.Textbox, new() { Name = "Title", Exact = true })).ToHaveValueAsync("New appointment");
        await dialog.GetByRole(AriaRole.Textbox, new() { Name = "Title", Exact = true }).FillAsync("Updated appointment");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).ClickAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("Update:10:00-10:30:Occurrence:False");
        var updated = page.GetByRole(AriaRole.Button, new() { Name = "Updated appointment", Exact = true });
        await updated.PressAsync("Delete");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Delete", Exact = true }).ClickAsync();
        await Expect(updated).ToHaveCountAsync(0);
        await Expect(page.Locator(".flare-scheduler__slot[data-flare-tree-index='20']")).ToBeFocusedAsync();
    }
    [Fact]
    public async Task TrustedTouchMovesAnEventThroughTheSharedDragContext()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { HasTouch = true });
        var page = await OpenAsync(context);
        var from = (await page.Locator($".{Css.Classes.Drag.Handle}").BoundingBoxAsync())!;
        var to = (await page.Locator(".flare-scheduler__slot[data-flare-tree-index='22']").BoundingBoxAsync())!;
        var x = from.X + from.Width / 2;
        var y = from.Y + from.Height / 2;
        var cdp = await context.NewCDPSessionAsync(page);
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchStart", ["touchPoints"] = new[] { new { x, y } } });
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchMove", ["touchPoints"] = new[] { new { x, y = y + 10 } } });
        await page.WaitForTimeoutAsync(40);
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchMove", ["touchPoints"] = new[] { new { x = to.X + to.Width / 2, y = to.Y + to.Height / 2 } } });
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchEnd", ["touchPoints"] = Array.Empty<object>() });
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:11:00-12:00:Occurrence:False");
        await page.Locator($".{Css.Classes.Resizable.Handle}").ScrollIntoViewIfNeededAsync();
        await ListenerWait.ForAsync(page, $".{Css.Classes.Resizable.Handle}", "pointerdown");
        var grip = (await page.Locator($".{Css.Classes.Resizable.Handle}").BoundingBoxAsync())!;
        var gx = grip.X + grip.Width / 2;
        var gy = grip.Y + grip.Height / 2;
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchStart", ["touchPoints"] = new[] { new { x = gx, y = gy } } });
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchMove", ["touchPoints"] = new[] { new { x = gx, y = gy + 40 } } });
        await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchEnd", ["touchPoints"] = Array.Empty<object>() });
        await Expect(page.Locator("#change")).ToHaveTextAsync("Resize:11:00-12:30:Occurrence:False");
        await cdp.DetachAsync();
    }

    [Fact]
    public async Task ResourceTimelineUsesGanttEditing()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#timeline").ClickAsync();
        var bar = page.Locator($".{Css.Classes.Gantt.Bar}[data-key='one']");
        await Expect(bar).ToBeVisibleAsync();
        await bar.Locator("button[data-scheduler-event]").PressAsync("Control+ArrowRight");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:10:00-11:00:Occurrence:False");
    }
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        page.Console += (_, message) => { if (message.Type == "error") output.WriteLine(message.Text); };
        page.PageError += (_, error) => output.WriteLine(error);
        await page.GotoAsync($"{fixture.BaseUrl}/scheduler");
        await Expect(page.Locator($".{Css.Classes.Scheduler.EventBody}")).ToHaveTextAsync("Planning");
        await Expect(page.Locator(".flare-scheduler__slot[data-flare-tree-index='0']")).ToHaveCSSAsync("height", "40px");
        await ListenerWait.ForAsync(page, $".{Css.Classes.Drag.Context}", "pointerdown");
        await page.Locator($".{Css.Classes.Scheduler.Viewport}").EvaluateAsync("e => e.scrollTop = 600");
        return page;
    }
    private static async Task DragAsync(IPage page, string from, string to)
    {
        var source = (await page.Locator(from).BoundingBoxAsync())!;
        var target = (await page.Locator(to).BoundingBoxAsync())!;
        var x = source.X + source.Width / 2;
        var y = source.Y + source.Height / 2;
        await page.Mouse.MoveAsync(x, y);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(x + 8, y + 8, new() { Steps = 3 });
        await page.WaitForTimeoutAsync(40);
        await page.Mouse.MoveAsync(target.X + target.Width / 2, target.Y + target.Height / 2, new() { Steps = 8 });
        await page.Mouse.UpAsync();
    }

    [Fact]
    public async Task MouseMoveAndResizeUseSharedComponentsAndHonorRefusal()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await DragAsync(page, $".{Css.Classes.Drag.Handle}", ".flare-scheduler__slot[data-flare-tree-index='22']");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:11:00-12:00:Occurrence:False");
        var handle = page.Locator($".{Css.Classes.Resizable.Handle}");
        await handle.ScrollIntoViewIfNeededAsync();
        await ListenerWait.ForAsync(page, $".{Css.Classes.Resizable.Handle}", "pointerdown");
        var box = (await handle.BoundingBoxAsync())!;
        await page.Mouse.MoveAsync(box.X + box.Width / 2, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + box.Width / 2, box.Y + box.Height / 2 + 40, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("Resize:11:00-12:30:Occurrence:False");

        await handle.PressAsync("ArrowDown");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Resize:11:00-13:00:Occurrence:False");
        await Expect(handle).ToBeFocusedAsync();
        await ListenerWait.ForAsync(page, $".{Css.Classes.Resizable.Handle}", "keydown");
        await handle.PressAsync("ArrowDown");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Resize:11:00-13:30:Occurrence:False");
        await Expect(handle).ToBeFocusedAsync();
        await page.Locator("#reject").ClickAsync();
        var button = page.Locator($".{Css.Classes.Scheduler.EventBody}");
        await button.PressAsync("Control+ArrowDown");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:11:30-14:00:Occurrence:True");
        await Expect(button).ToBeFocusedAsync();
    }

    [Fact]
    public async Task RecurrenceScopeAndEditorCancellationKeepBackendOwnership()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#series").ClickAsync();
        var button = page.Locator($".{Css.Classes.Scheduler.EventBody}");
        await Expect(button).ToHaveAttributeAsync("data-scheduler-event", "provider-instance");
        await button.PressAsync("Control+ArrowDown");
        var dialog = page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();
        var scope = dialog.GetByRole(AriaRole.Combobox);
        await scope.ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "This and following", Exact = true }).ClickAsync();
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).ClickAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:09:30-10:30:Following:False");
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(button).ToBeFocusedAsync();
        await button.PressAsync("F2");
        await Expect(dialog).ToBeVisibleAsync();
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Cancel", Exact = true }).ClickAsync();
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(button).ToBeFocusedAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:09:30-10:30:Following:False");
    }

    [Fact]
    public async Task SlotsUseOneTabStopAndNativeSpaceSelection()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await ListenerWait.ForAsync(page, ".flare-scheduler__time-grid", "keydown");
        var slot = page.Locator(".flare-scheduler__slot[data-flare-tree-index='20']");
        await slot.FocusAsync();
        await slot.PressAsync("ArrowDown");
        await Expect(page.Locator(".flare-scheduler__slot[data-flare-tree-index='21']")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Space");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await page.Keyboard.PressAsync("F2");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Dialog).GetByRole(AriaRole.Button, new() { Name = "Cancel", Exact = true }).ClickAsync();
        await Expect(page.Locator(".flare-scheduler__slot[data-flare-tree-index][tabindex='0']")).ToHaveCountAsync(1);
    }
}
