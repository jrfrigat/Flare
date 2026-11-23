using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The split pane turns the handle's pixel drag into the measured share of the container. The host
/// persists that fraction, so a real drag has to move both the pane and the reported value.
/// </summary>
public sealed class SplitPaneTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task DraggingTheHandleUpdatesThePaneAndTheFraction()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/split-pane");

        var first = page.Locator("#first-pane");
        await Expect(first).ToBeVisibleAsync();
        var before = await first.BoundingBoxAsync();
        Assert.NotNull(before);
        Assert.InRange(before!.Width, 280, 320);

        var handle = page.Locator("#pane-root [role=separator]");
        var box = await handle.BoundingBoxAsync();
        Assert.NotNull(box);
        var x = box!.X + box.Width / 2;
        var y = box.Y + box.Height / 2;

        await page.Mouse.MoveAsync(x, y);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(x + 120, y, new() { Steps = 8 });
        await page.Mouse.UpAsync();

        await Expect(page.Locator("#fraction")).Not.ToHaveTextAsync("0.5");
        var after = await first.BoundingBoxAsync();
        Assert.NotNull(after);
        Assert.True(after!.Width > before.Width + 60, $"first pane stayed {after.Width}px wide");

        var fraction = double.Parse(await page.Locator("#fraction").TextContentAsync() ?? "0",
            System.Globalization.CultureInfo.InvariantCulture);
        Assert.True(fraction > 0.6, $"fraction was {fraction}");
    }

    [Fact]
    public async Task HorizontalLimitsKeepTheSecondPaneAtItsMinimum()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/split-pane");

        var split = page.Locator("#horizontal-limits");
        var handle = split.GetByRole(AriaRole.Separator);
        var second = split.Locator($".{Css.Classes.SplitPane.Second}");
        await Expect(handle).ToHaveAttributeAsync("aria-valuemin", "100");
        Assert.InRange((await second.BoundingBoxAsync())!.Width, 99, 101);
        await handle.PressAsync("ArrowRight");
        Assert.InRange((await second.BoundingBoxAsync())!.Width, 99, 101);

        var previousMaximum = await handle.GetAttributeAsync("aria-valuemax");
        await split.EvaluateAsync("split => split.style.width = '350px'");
        await Expect(handle).Not.ToHaveAttributeAsync("aria-valuemax", previousMaximum!);
        Assert.InRange((await second.BoundingBoxAsync())!.Width, 99, 101);
    }

    [Fact]
    public async Task VerticalLimitsApplyToTheInitialSizeAndKeyboardResizing()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/split-pane");

        var split = page.Locator("#vertical-limits");
        var handle = split.GetByRole(AriaRole.Separator);
        var first = split.Locator("[data-flare-splitpane-first]");
        await Expect(handle).ToHaveAttributeAsync("aria-valuemax", "150");
        Assert.InRange((await first.BoundingBoxAsync())!.Height, 149, 151);
        await handle.PressAsync("Home");
        Assert.InRange((await first.BoundingBoxAsync())!.Height, 59, 61);
        await handle.PressAsync("End");
        Assert.InRange((await first.BoundingBoxAsync())!.Height, 149, 151);
    }

    [Fact]
    public async Task ARejectedZeroFractionLeavesTheControlledPaneUnchanged()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/split-pane");

        var split = page.Locator("#controlled-limits");
        var handle = split.GetByRole(AriaRole.Separator);
        var root = split.Locator("[data-flare-splitpane]");
        var first = split.Locator("[data-flare-splitpane-first]");
        await Expect(handle).ToHaveAttributeAsync("aria-valuemin", "0");
        var initial = (await first.BoundingBoxAsync())!.Width;
        await handle.PressAsync("Home");

        await Expect(page.Locator("#declined")).ToHaveTextAsync("0");
        await Expect(root).Not.ToHaveAttributeAsync("style", new System.Text.RegularExpressions.Regex("--_flare-splitpane-preview:"));
        Assert.InRange((await first.BoundingBoxAsync())!.Width, initial - 1, initial + 1);
    }
}
