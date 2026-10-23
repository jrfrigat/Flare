using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class SwitchRtlTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    // [trackLeft, trackRight, handleLeft, handleRight]
    private static Task<double[]> MeasureAsync(IPage page, string selector)
        => page.EvaluateAsync<double[]>(
            $"s => {{ const t = document.querySelector(s + ' .{Css.Classes.Switch.Thumb}').getBoundingClientRect();"
            + $" const h = document.querySelector(s + ' .{Css.Classes.Switch.Handle}').getBoundingClientRect();"
            + " return [t.left, t.right, h.left, h.right]; }",
            selector);

    private static double Center(double start, double end) => (start + end) / 2;

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/switch");
        await Expect(page.Locator($"#rtl .{Css.Classes.Switch.Handle}")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task TheHandleFollowsTheInlineDirection()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var ltrOn = await MeasureAsync(page, "#ltr");
        var rtlOn = await MeasureAsync(page, "#rtl");
        var rtlOff = await MeasureAsync(page, "#rtl-off");

        Assert.True(Center(ltrOn[2], ltrOn[3]) > Center(ltrOn[0], ltrOn[1]),
            "checked LTR handle should sit at the inline end");
        Assert.True(Center(rtlOn[2], rtlOn[3]) < Center(rtlOn[0], rtlOn[1]),
            "checked RTL handle should sit at the inline end");
        Assert.True(Center(rtlOff[2], rtlOff[3]) > Center(rtlOff[0], rtlOff[1]),
            "unchecked RTL handle should sit at the inline start");
    }

    [Fact]
    public async Task PressingTheCheckedRtlHandle_KeepsItsCentre()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var resting = await MeasureAsync(page, "#rtl");
        await page.Locator($"#rtl .{Css.Classes.Switch.Track}").HoverAsync();
        await page.Mouse.DownAsync();
        await Task.Delay(50, Xunit.TestContext.Current.CancellationToken);
        var pressed = await MeasureAsync(page, "#rtl");
        await page.Mouse.UpAsync();

        Assert.Equal(Center(resting[2], resting[3]), Center(pressed[2], pressed[3]), tolerance: 1.5);
    }
}
