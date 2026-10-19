using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// MaxLines has to mean that many visible lines. The cap is measured against the control's real line
/// height (whatever size or Typo gave it) and its real block padding, not against assumed literals.
/// </summary>
public sealed class TextAreaMaxLinesTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string[] Sizes = ["Xs", "Sm", "Md", "Lg", "Xl"];

    // [lineHeight, paddingTop, paddingBottom, maxHeight, scrollHeight, rectHeight, clientHeight]
    private const string MetricsScript = """
        el => {
            const s = getComputedStyle(el);
            const r = v => parseFloat(v) || 0;
            return [r(s.lineHeight), r(s.paddingTop), r(s.paddingBottom), r(s.maxHeight),
                    el.scrollHeight, el.getBoundingClientRect().height, el.clientHeight];
        }
        """;

    private static async Task<IPage> OpenAsync(IBrowserContext context, BrowserFixture fixture, string themeId)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/textarea-maxlines");
        await Expect(page.Locator("#cap-Md textarea")).ToBeVisibleAsync();
        await page.Locator(themeId == "fluent2" ? "#fluent" : "#md3e").ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Theme.Root}"))
            .ToHaveClassAsync(new Regex(Regex.Escape(Css.Classes.Theme.ForTheme(themeId))));
        // The theme's token bundle arrives with its stylesheet, which can land after the theme class;
        // until it does, the control's padding is 0 and every measurement below would be against the
        // unthemed box.
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && getComputedStyle(el).paddingTop !== '0px'; }",
            "#cap-Md textarea");
        return page;
    }

    private static Task<double[]> MetricsAsync(IPage page, string id)
        => page.Locator($"#{id} textarea").EvaluateAsync<double[]>(MetricsScript);

    // The textarea resolves line-height to "normal" in computed style; two uncapped lines give it away:
    // their client height is two line boxes plus the block padding.
    private static double LineHeightFrom(double[] twoLines)
        => (twoLines[5] - twoLines[1] - twoLines[2]) / 2;

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task TheCapEqualsTheRequestedLinesAtEverySize(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, themeId);

        foreach (var size in Sizes)
        {
            // The size grid scales the control's font, so each size has its own line height.
            var shortMetrics = await MetricsAsync(page, $"short-{size}");
            var lineHeight = LineHeightFrom(shortMetrics);
            Assert.True(lineHeight > 0, $"{themeId} {size}: could not measure the line height");

            var m = await MetricsAsync(page, $"cap-{size}");
            var expected = 3 * lineHeight + m[1] + m[2];
            Assert.True(Math.Abs(m[3] - expected) <= 1,
                $"{themeId} {size}: max-height {m[3]:0.##} is not three lines plus padding ({expected:0.##})");
            Assert.True(m[4] > m[6], $"{themeId} {size}: four lines should scroll inside a three-line cap");
        }
    }

    [Fact]
    public async Task TheCapFollowsAnExplicitTypoAndDoesNotClipShortContent()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, "md3-expressive");

        var shortLines = await MetricsAsync(page, "typo-short");
        var lineHeight = LineHeightFrom(shortLines);
        var typo = await MetricsAsync(page, "cap-typo");
        var expected = 2 * lineHeight + typo[1] + typo[2];

        Assert.True(Math.Abs(typo[3] - expected) <= 1,
            $"headline cap {typo[3]:0.##} is not two headline lines plus padding ({expected:0.##})");
        Assert.True(typo[4] > typo[6], "headline content should scroll inside a two-line cap");

        var shortContent = await MetricsAsync(page, "cap-short");
        Assert.True(shortContent[4] <= shortContent[6] + 1,
            "two lines of content should fit without scrolling under a three-line cap");
    }

    [Fact]
    public async Task ALongWrappedLineDoesNotOverflowTheNarrowColumn()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, "md3-expressive");
        await page.SetViewportSizeAsync(320, 900);

        var document = await page.EvaluateAsync<double[]>(
            "() => { const d = document.scrollingElement; return [d.scrollWidth, d.clientWidth]; }");
        Assert.True(document[0] <= document[1] + 1,
            $"page scrolls horizontally: {document[0]:0.#} > {document[1]:0.#}");

        var narrow = await MetricsAsync(page, "cap-narrow");
        Assert.True(narrow[4] > narrow[6], "wrapped content should scroll inside the cap");
    }
}
