using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Drag and resize are pointer gestures with a keyboard twin, and neither may put the title, the close
/// button or the actions out of reach - even when the gesture ends past the edge of the screen.
/// </summary>
public sealed class DialogInteractionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string Panel = $".{Css.Classes.Dialog.Root}";

    private async Task<IPage> OpenAsync(IBrowserContext context, int width = 900, int height = 700)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.SetViewportSizeAsync(width, height);
        await page.GotoAsync($"{fixture.BaseUrl}/dialog-audit");
        await page.Locator("#open-dialog").ClickAsync();
        await Expect(page.Locator(Panel)).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && el.getBoundingClientRect().width > 0; }",
            Panel);
        return page;
    }

    private static async Task<double[]> RectAsync(IPage page) =>
        await page.Locator(Panel).EvaluateAsync<double[]>(
            "el => { const r = el.getBoundingClientRect(); return [r.left, r.top, r.right, r.bottom]; }");

    [Fact]
    public async Task DraggingAndResizingPastTheEdgeLeaveThePanelOnScreen()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var viewport = await page.EvaluateAsync<double[]>(
            "() => [document.documentElement.clientWidth, document.documentElement.clientHeight]");

        var header = await page.Locator($".{Css.Classes.Dialog.Header}").BoundingBoxAsync();
        Assert.NotNull(header);
        await page.Mouse.MoveAsync(header!.X + 20, header.Y + 10);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync((float)(viewport[0] + 300), (float)(viewport[1] + 300), new() { Steps = 10 });
        await page.Mouse.UpAsync();

        var dragged = await RectAsync(page);
        Assert.True(dragged[0] >= -0.5 && dragged[1] >= -0.5,
            $"the panel was dragged off the top-left: {dragged[0]:0.#},{dragged[1]:0.#}");
        Assert.True(dragged[2] <= viewport[0] + 0.5 && dragged[3] <= viewport[1] + 0.5,
            $"the panel was dragged off the bottom-right: {dragged[2]:0.#},{dragged[3]:0.#}");

        // The keyboard twin moves the same panel and obeys the same bounds.
        await page.Locator($".{Css.Classes.Dialog.Header}").FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");
        var nudged = await RectAsync(page);
        Assert.True(nudged[0] >= -0.5 && nudged[0] < dragged[0], "the header arrow did not move the panel");

        var resizer = await page.Locator($".{Css.Classes.Dialog.Resizer}").BoundingBoxAsync();
        Assert.NotNull(resizer);
        await page.Mouse.MoveAsync(resizer!.X + 4, resizer.Y + 4);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync((float)(viewport[0] + 300), (float)(viewport[1] + 300), new() { Steps = 10 });
        await page.Mouse.UpAsync();

        var resized = await RectAsync(page);
        Assert.True(resized[2] <= viewport[0] + 0.5 && resized[3] <= viewport[1] + 0.5,
            $"the resized panel left the viewport: {resized[2]:0.#},{resized[3]:0.#}");
        await Expect(page.Locator($".{Css.Classes.Dialog.Header}")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task ANarrowDialogKeepsTheCloseAndActionsReachable()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width: 320, height: 600);

        var overflow = await page.EvaluateAsync<int>(
            "document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"the page gained {overflow}px of horizontal scroll");

        await Expect(page.Locator($".{Css.Classes.Dialog.CloseButton}")).ToBeVisibleAsync();
        await Expect(page.Locator("#dialog-cancel")).ToBeVisibleAsync();
        await Expect(page.Locator("#dialog-ok")).ToBeVisibleAsync();

        var title = await page.Locator($".{Css.Classes.Dialog.Title}").BoundingBoxAsync();
        var close = await page.Locator($".{Css.Classes.Dialog.CloseButton}").BoundingBoxAsync();
        Assert.NotNull(title);
        Assert.NotNull(close);
        Assert.True(title!.X + title.Width <= close!.X + 1,
            "the title runs under the close button instead of stopping before it");
    }
}
