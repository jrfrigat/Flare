using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A standalone badge is the same box as a corner one, only placed inline: its height, horizontal
/// padding and dot diameter have to come from the theme's size scale, never from an override inside the
/// standalone rule. The scale is read from the same tokens and compared with the laid-out box.
/// </summary>
public sealed class BadgeStandaloneSizeTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static readonly string[] Sizes = ["Xs", "Sm", "Md", "Lg", "Xl"];

    // [height, paddingLeft, paddingRight, width, radius, tokenHeight, tokenPaddingX, tokenDotSize,
    //  circleCheck]
    private const string MetricsScript = """
        (el, size) => {
            const s = getComputedStyle(el);
            const ind = el.querySelector(':scope > span');
            const is = getComputedStyle(ind);
            const px = raw => {
                raw = (raw || '').trim();
                if (!raw) return 0;
                const probe = document.createElement('div');
                probe.style.cssText = 'position:absolute;visibility:hidden;width:' + raw;
                document.body.appendChild(probe);
                const w = probe.getBoundingClientRect().width;
                probe.remove();
                return w;
            };
            const suffix = size.toLowerCase();
            const num = v => v === 'auto' || v === 'none' ? 0 : parseFloat(v) || 0;
            const width = num(is.width);
            const radius = is.borderTopLeftRadius.trim();
            const circle = radius.endsWith('%') ? parseFloat(radius) >= 50 : parseFloat(radius) >= width / 2 - 0.5;
            return [num(is.height), num(is.paddingLeft), num(is.paddingRight), width, parseFloat(radius),
                    px(s.getPropertyValue('--flare-badge-height-' + suffix)),
                    px(s.getPropertyValue('--flare-badge-padding-x-' + suffix)),
                    px(s.getPropertyValue('--flare-badge-dot-size-' + suffix)),
                    circle ? 1 : 0];
        }
        """;

    private static async Task<IPage> OpenAsync(IBrowserContext context, BrowserFixture fixture, string themeId)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/badge-standalone");
        await Expect(page.Locator("#count-Md")).ToBeVisibleAsync();
        await page.Locator(themeId == "fluent2" ? "#fluent" : "#md3e").ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Theme.Root}"))
            .ToHaveClassAsync(new Regex(Regex.Escape(Css.Classes.Theme.ForTheme(themeId))));
        // The token bundle arrives with the theme stylesheet; measure only once it is live.
        await page.WaitForFunctionAsync(
            "() => getComputedStyle(document.querySelector('#count-Md')).getPropertyValue('--flare-badge-height-md').trim().length > 0");
        return page;
    }

    private static Task<double[]> MetricsAsync(IPage page, string id, string size)
        => page.Locator($"#{id}").EvaluateAsync<double[]>(MetricsScript, size);

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task CountAndTextBadgesFollowTheSizeTokens(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, themeId);

        var heights = new List<double>();
        foreach (var size in Sizes)
        {
            foreach (var kind in new[] { "count", "text" })
            {
                var m = await MetricsAsync(page, $"{kind}-{size}", size);
                Assert.True(m[5] > 0, $"{themeId} {size}: height token is missing");
                Assert.True(Math.Abs(m[0] - m[5]) <= 1,
                    $"{themeId} {kind}-{size}: height {m[0]:0.##} is not the token height {m[5]:0.##}");
                Assert.True(Math.Abs(m[1] - m[6]) <= 1,
                    $"{themeId} {kind}-{size}: padding-left {m[1]:0.##} is not the token padding {m[6]:0.##}");
                Assert.True(Math.Abs(m[2] - m[6]) <= 1,
                    $"{themeId} {kind}-{size}: padding-right {m[2]:0.##} is not the token padding {m[6]:0.##}");
                heights.Add(m[0]);
            }
        }

        Assert.True(heights.Max() - heights.Min() > 0.5, $"{themeId}: the size scale does not change the badge height");
    }

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task StandaloneDotsStayRoundAtEverySize(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, themeId);

        foreach (var size in Sizes)
        {
            var m = await MetricsAsync(page, $"dot-{size}", size);
            Assert.True(m[7] > 0, $"{themeId} {size}: dot-size token is missing");
            Assert.True(Math.Abs(m[0] - m[7]) <= 1,
                $"{themeId} dot-{size}: height {m[0]:0.##} is not the dot size {m[7]:0.##}");
            Assert.True(Math.Abs(m[3] - m[7]) <= 1,
                $"{themeId} dot-{size}: width {m[3]:0.##} is not the dot size {m[7]:0.##}");
            Assert.True(m[8] == 1,
                $"{themeId} dot-{size}: radius {m[4]:0.##} does not make a circle of width {m[3]:0.##}");
        }
    }
}
