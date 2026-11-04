using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ComponentTokensTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task CanvasInkUsesTheCurrentBaseRoleAtTheStartOfEachStroke()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/component-tokens");
        var canvas = page.Locator("#signature canvas");
        await ListenerWait.ForAsync(page, "#signature canvas", "pointerdown");
        await page.Locator("#ink").ClickAsync();
        await page.WaitForFunctionAsync($"getComputedStyle(document.querySelector('#signature canvas')).getPropertyValue('{Css.Tokens.Color.OnSurface}').trim() === 'rgb(1, 128, 192)'");
        await canvas.ScrollIntoViewIfNeededAsync();
        var box = (await canvas.BoundingBoxAsync())!;
        await page.Mouse.MoveAsync(box.X + 10, box.Y + 10);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + 60, box.Y + 10, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        var pixel = await canvas.EvaluateAsync<int[]>("c => { const d=c.getContext('2d').getImageData(0,0,c.width,c.height).data; for(let i=0;i<d.length;i+=4) if(d[i+3]===255) return [...d.slice(i,i+4)]; return []; }");
        Assert.Equal(4, pixel.Length);
        Assert.Equal("#0180c0", await canvas.EvaluateAsync<string>("c => c.getContext('2d').strokeStyle"));
        var expected = new[] { 1, 128, 192 };
        for (var i = 0; i < expected.Length; i++) Assert.InRange(pixel[i], expected[i] - 1, expected[i] + 1);
        Assert.Equal(255, pixel[3]);
    }
    [Fact]
    public async Task ThemeServiceOverridesReachIndependentComponents()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/component-tokens");
        var bar = page.Locator($"#chart .{Css.Classes.Gantt.Bar}");
        await Expect(bar).ToBeVisibleAsync();
        await page.Locator("#base").ClickAsync();
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(12, 34, 56)");
        await Expect(page.Locator("#board .flare-kanban__card")).ToHaveCSSAsync("background-color", "rgb(56, 34, 12)");
        await page.Locator("#customize").ClickAsync();
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(18, 52, 86)");
        await Expect(page.Locator("#board .flare-kanban__card")).ToHaveCSSAsync("background-color", "rgb(86, 52, 18)");
    }
}
