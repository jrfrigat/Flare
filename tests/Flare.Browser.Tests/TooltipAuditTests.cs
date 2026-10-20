using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A long plain tooltip has to wrap inside the viewport instead of running off it, the pointer has to
/// be able to move onto the bubble to read it, and Escape has to hide it until a fresh approach.
/// </summary>
public sealed class TooltipAuditTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string Bubble = $".{Css.Classes.Tooltip.Content}";

    private async Task<IPage> OpenAsync(IBrowserContext context, int width = 900)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.SetViewportSizeAsync(width, 800);
        await page.GotoAsync($"{fixture.BaseUrl}/tooltip-audit");
        await Expect(page.Locator("#tip-trigger")).ToBeVisibleAsync();
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && parseFloat(getComputedStyle(el).maxWidth) > 0; }",
            Bubble);
        return page;
    }

    [Fact]
    public async Task ALongTooltipWrapsInsideANarrowViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, width: 320);

        await page.Locator("#tip-trigger").ClickAsync();
        await Expect(page.Locator(Bubble)).ToHaveCSSAsync("visibility", "visible");

        var box = await page.Locator(Bubble).BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box!.Width <= 320, $"the bubble is {box.Width:0.#}px wide in a 320px viewport");
        Assert.True(box.Height > 40, "the long text did not wrap onto more than one line");
        Assert.True(box.X >= -0.5 && box.X + box.Width <= 320.5,
            $"the bubble leaves the viewport: {box.X:0.#}..{box.X + box.Width:0.#}");

        var overflow = await page.EvaluateAsync<int>(
            "document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"the page gained {overflow}px of horizontal scroll");
    }

    [Fact]
    public async Task ThePointerCanMoveOntoTheBubbleAndEscapeHidesItUntilAFreshApproach()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var trigger = page.Locator("#tip-trigger");

        await trigger.FocusAsync();
        await Expect(page.Locator(Bubble)).ToHaveCSSAsync("opacity", "1");

        // Crossing the offset gap onto the bubble is still hovering the trigger, so it must stay open.
        var bubble = await page.Locator(Bubble).BoundingBoxAsync();
        Assert.NotNull(bubble);
        await page.Mouse.MoveAsync(bubble!.X + bubble.Width / 2, bubble.Y + bubble.Height / 2, new() { Steps = 6 });
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);
        await Expect(page.Locator(Bubble)).ToHaveCSSAsync("opacity", "1");

        await page.Keyboard.PressAsync("Escape");
        await Expect(page.Locator(Bubble)).ToHaveCSSAsync("visibility", "hidden");

        // Leaving the trigger is the fresh intent: coming back shows it again.
        await page.Mouse.MoveAsync(0, 0);
        var triggerBox = await trigger.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        await page.Mouse.MoveAsync(triggerBox!.X + 10, triggerBox.Y + 10);
        await Expect(page.Locator(Bubble)).ToHaveCSSAsync("opacity", "1");
    }
}
