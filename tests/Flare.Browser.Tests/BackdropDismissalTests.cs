using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class BackdropDismissalTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("dialog", "#editor")]
    [InlineData("sheet", "#editor")]
    [InlineData("confirm", "." + Css.Classes.Dialog.Content)]
    [InlineData("alert", "." + Css.Classes.Msgbox.Content)]
    [InlineData("prompt", "." + Css.Classes.Msgbox.Root + " input")]
    [InlineData("provider", "." + Css.Classes.Confirmdialog.Body)]
    [InlineData("drawer", "#drawer-editor")]
    [InlineData("overlay", "#overlay-editor")]
    public async Task DragFromContentDoesNotDismiss_ButBackdropClickDoes(string kind, string content)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, kind);
        var box = await page.Locator(content).BoundingBoxAsync();
        Assert.NotNull(box);
        await page.Mouse.MoveAsync(box.X + box.Width / 2, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(890, 10, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await SyncAsync(page);
        await Expect(page.Locator("#active")).ToHaveTextAsync(kind);

        await page.Mouse.ClickAsync(890, 10);
        await Expect(page.Locator("#active")).ToBeEmptyAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DragFromBackdropToContentDoesNotDismiss(bool capture)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, "dialog");
        if (capture)
            await page.EvaluateAsync("selector => document.querySelector(selector).addEventListener('pointerdown', e => e.currentTarget.setPointerCapture(e.pointerId))",
                "." + Css.Classes.Dialog.Scrim);
        var box = await page.Locator("#editor").BoundingBoxAsync();
        Assert.NotNull(box);
        await page.Mouse.MoveAsync(890, 10);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + 20, box.Y + 20, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await SyncAsync(page);
        await Expect(page.Locator("#active")).ToHaveTextAsync("dialog");
        await page.Mouse.ClickAsync(890, 10);
        await Expect(page.Locator("#active")).ToBeEmptyAsync();
    }

    [Fact]
    public async Task TouchTapAndReopenStillDismiss()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { HasTouch = true });
        var page = await OpenAsync(context, "dialog");
        await page.Touchscreen.TapAsync(890, 10);
        await Expect(page.Locator("#active")).ToBeEmptyAsync();
        await page.Locator("#open-dialog").ClickAsync();
        await Expect(page.Locator("#editor")).ToBeVisibleAsync();
        await page.Touchscreen.TapAsync(890, 10);
        await Expect(page.Locator("#active")).ToBeEmptyAsync();
    }

    private async Task<IPage> OpenAsync(IBrowserContext context, string kind)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(900, 700);
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/backdrop-dismissal");
        await page.EvaluateAsync("() => import('./_content/Flare.Components/js/flare-theme.js')");
        await page.Locator($"#open-{kind}").ClickAsync();
        await Expect(page.Locator("#active")).ToHaveTextAsync(kind);
        return page;
    }

    private static async Task SyncAsync(IPage page)
    {
        var sync = page.Locator("#sync");
        var next = int.Parse((await sync.TextContentAsync())!) + 1;
        await sync.DispatchEventAsync("click");
        await Expect(sync).ToHaveTextAsync(next.ToString());
    }
}
