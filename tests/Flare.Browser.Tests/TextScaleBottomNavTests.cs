using Flare.Css.Classes;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A five-item bottom navigation bar in a 360px shell keeps its labels readable at 200% text in wrap
/// mode: each label uses at most two lines, the bar grows to the height those lines need, and the
/// spacer a pinned bar reserves grows with it.
/// </summary>
public sealed class TextScaleBottomNavTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static string BarSelector => $"nav.{Css.Classes.BottomNav.Root}";

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    public async Task WrappedLabelsStayReadable(int textScale)
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 360, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/text-scale-bottomnav");
        await Expect(page.Locator(BarSelector)).ToBeVisibleAsync();

        await page.EvaluateAsync($"document.documentElement.style.fontSize = '{textScale}%'");
        // The theme's tokens land after the first paint; until then the bar is measured unstyled.
        await page.WaitForFunctionAsync(
            "args => { const bar = document.querySelector(args.sel); if (!bar || document.fonts.status !== 'loaded') return false; "
            + "const root = getComputedStyle(document.documentElement); "
            + "return getComputedStyle(bar).height !== '0px' && root.getPropertyValue(args.token).trim() !== ''; }",
            new { sel = BarSelector, token = Css.Tokens.BottomNavField.BarHeight });

        var bar = page.Locator(BarSelector);
        var barHeight = await bar.EvaluateAsync<double>("el => el.getBoundingClientRect().height");
        var barOverflow = await bar.EvaluateAsync<double>("el => el.scrollWidth - el.clientWidth");
        Assert.True(barOverflow <= 1, $"the bar overflows by {barOverflow:0.#}px at {textScale}% text");

        // A pinned bar and the spacer that reserves its space must agree at every text scale.
        var spacerHeight = await page.Locator($"div.{Css.Classes.BottomNav.Spacer}")
            .EvaluateAsync<double>("el => el.getBoundingClientRect().height");
        Assert.True(Math.Abs(barHeight - spacerHeight) <= 1,
            $"the bar is {barHeight:0.#}px tall while its spacer reserves {spacerHeight:0.#}px at {textScale}% text");

        // Items never overflow: at 200% text the indicator pill would otherwise be wider than its item.
        var itemOverflow = await page.Locator($".{Css.Classes.BottomNav.Item}").EvaluateAllAsync<double[]>(
            "els => els.map(e => e.scrollWidth - e.clientWidth)");
        Assert.All(itemOverflow, overflow => Assert.True(overflow <= 1,
            $"an item overflows by {overflow:0.#}px at {textScale}% text"));

        // Every label is fully rendered: no horizontal cut, and no line hidden by the two-line clamp.
        var labels = await page.Locator($".{Css.Classes.BottomNav.ItemLabel}").EvaluateAllAsync<double[][]>(
            "els => els.map(l => { const r = document.createRange(); r.selectNodeContents(l); "
            + "const lines = [...r.getClientRects()].filter(x => x.width > 0).length; "
            + "return [l.clientWidth, l.scrollWidth, l.clientHeight, l.scrollHeight, lines]; })");
        Assert.Equal(5, labels.Length);
        foreach (var label in labels)
        {
            Assert.True(label[1] <= label[0] + 1,
                $"a label is cut horizontally at {textScale}% text");
            Assert.True(label[3] <= label[2] + 1,
                $"a label needs more lines than the bar reserves at {textScale}% text");
            Assert.True(label[4] <= 2,
                $"a label uses {label[4]} lines at {textScale}% text");
        }
        if (textScale == 200)
            Assert.Contains(labels, label => label[4] == 2);
    }
}
