using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class GanttCompositionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync(fixture.BaseUrl + "/gantt-composition");
        await Expect(page.Locator("#primary")).ToBeVisibleAsync();
        await BrowserFonts.WaitAsync(page.Locator(".flare-gantt"));
        await ListenerWait.ForAsync(page, ".flare-gantt", "keydown");
        await ListenerWait.ForAsync(page, ".flare-gantt", "pointerdown");
        return page;
    }
    [Fact]
    public async Task InteractivePrimaryReusesSelectionAndAxisEditingWithoutDuplicateActivation()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#primary").ClickAsync();
        await Expect(page.Locator("#selected")).ToHaveTextAsync("1");
        await page.Locator("#primary").PressAsync("Control+ArrowDown");
        await Expect(page.Locator("#row")).ToHaveTextAsync("next");
        await Expect(page.Locator("#primary")).ToBeFocusedAsync();
        await page.Locator("#primary").PressAsync("Control+ArrowRight");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move");
        await Expect(page.Locator("#selected")).ToHaveTextAsync("1");
    }
    [Fact]
    public async Task FormFieldsAndMarkedActionsKeepTheirNativeBehaviorInsideAnEditableBar()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var input = page.Locator("#note");
        await input.PressAsync("End");
        await input.PressAsync("Control+ArrowLeft");
        Assert.Equal(0, await input.EvaluateAsync<int>("e => e.selectionStart"));
        var box = (await input.BoundingBoxAsync())!;
        await page.Mouse.MoveAsync(box.X + 10, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + 40, box.Y + box.Height / 2, new() { Steps = 6 });
        await page.Mouse.UpAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("");
        var action = (await page.Locator("#secondary").BoundingBoxAsync())!;
        await page.Mouse.MoveAsync(action.X + 5, action.Y + action.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(action.X + 20, action.Y + action.Height / 2, new() { Steps = 6 });
        await page.Mouse.UpAsync();
        await Expect(page.Locator("#change")).ToHaveTextAsync("");
        await Expect(page.Locator("#actions")).ToHaveTextAsync("1");
    }
}
