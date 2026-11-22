using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A resizable handle reads changed limits, and a splitter with Value but no ValueVariable stays in
/// the value mode instead of silently resizing its neighbours.
/// </summary>
public sealed class ResizableSplitterTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Resizable_AppliesChangedLimits()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            "() => document.querySelector('#resizable-box [role=separator]')?.hasAttribute('aria-valuenow')");

        var handle = page.Locator("#resizable-box [role='separator']");
        await handle.FocusAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator($"#resizable-box .{Css.Classes.Resizable.Root}")).ToHaveCSSAsync("width", "300px");

        await page.Locator("#raise-max").ClickAsync();
        await Expect(page.Locator("#max-value")).ToHaveTextAsync("500px");
        await handle.FocusAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator($"#resizable-box .{Css.Classes.Resizable.Root}")).ToHaveCSSAsync("width", "500px");
    }

    [Fact]
    public async Task Splitter_ValueWithoutVariable_StaysInValueMode()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('#splitter-row [role=separator]')?.classList.contains('{Css.Classes.Splitter.Horizontal}')");

        var gutter = page.Locator("#splitter-row [role='separator']");
        await gutter.FocusAsync();
        await page.Keyboard.PressAsync("End");

        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "500");
        await Expect(page.Locator("#split-size")).ToHaveTextAsync("500px");
        Assert.Equal(300, await page.Locator("#pane-a").EvaluateAsync<double>("el => el.getBoundingClientRect().width"));
        Assert.Equal(300, await page.Locator("#pane-b").EvaluateAsync<double>("el => el.getBoundingClientRect().width"));
    }

    [Fact]
    public async Task Resizable_DragClampsToLimitsAndKeysStep()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            "() => document.querySelector('#resizable-box [role=separator]')?.hasAttribute('aria-valuenow')");

        var root = page.Locator($"#resizable-box .{Css.Classes.Resizable.Root}");
        var handle = page.Locator("#resizable-box [role='separator']");

        var box = await handle.BoundingBoxAsync();
        await page.Mouse.MoveAsync(box!.X + box.Width / 2, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + box.Width / 2 - 150, box.Y + box.Height / 2, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await Expect(root).ToHaveCSSAsync("width", "100px");

        box = await handle.BoundingBoxAsync();
        await page.Mouse.MoveAsync(box!.X + box.Width / 2, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + box.Width / 2 + 300, box.Y + box.Height / 2, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await Expect(root).ToHaveCSSAsync("width", "300px");

        await handle.FocusAsync();
        await page.Keyboard.PressAsync("Home");
        await Expect(root).ToHaveCSSAsync("width", "100px");
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(root).ToHaveCSSAsync("width", "116px");
        await page.Keyboard.PressAsync("End");
        await Expect(root).ToHaveCSSAsync("width", "300px");
    }

    [Fact]
    public async Task Splitter_HomeStepsShiftAndDragReportWithinLimits()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/resizable-splitter");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('#splitter-row [role=separator]')?.classList.contains('{Css.Classes.Splitter.Horizontal}')");

        var gutter = page.Locator("#splitter-row [role='separator']");
        var size = page.Locator("#split-size");

        await gutter.FocusAsync();
        await page.Keyboard.PressAsync("Home");
        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "200");
        await Expect(size).ToHaveTextAsync("200px");
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "216");
        await page.Keyboard.PressAsync("Shift+ArrowRight");
        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "256");
        await Expect(size).ToHaveTextAsync("256px");
        await page.Keyboard.PressAsync("End");
        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "500");

        var box = await gutter.BoundingBoxAsync();
        await page.Mouse.MoveAsync(box!.X + box.Width / 2, box.Y + box.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(box.X + box.Width / 2 - 100, box.Y + box.Height / 2, new() { Steps = 8 });
        await page.Mouse.UpAsync();
        await Expect(gutter).ToHaveAttributeAsync("aria-valuenow", "400");
        await Expect(size).ToHaveTextAsync("400px");
    }
}
