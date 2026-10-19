using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A segmented toggle group is one framed run: the frame's ends belong to the first and last DOM
/// segments, and every segment carries the rule between it and the next. In RTL the first segment
/// sits on the right, so the frame, the seam and the rounded corners all have to follow the inline
/// direction rather than the physical side.
/// </summary>
public sealed class ToggleGroupRtlTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    // [leftWidth, rightWidth, topWidth, bottomWidth, topLeftRadius, topRightRadius, bottomRightRadius, bottomLeftRadius]
    private const string SegmentMetricsScript = """
        el => {
            const s = getComputedStyle(el);
            const r = v => parseFloat(v) || 0;
            return [r(s.borderLeftWidth), r(s.borderRightWidth), r(s.borderTopWidth), r(s.borderBottomWidth),
                    r(s.borderTopLeftRadius), r(s.borderTopRightRadius), r(s.borderBottomRightRadius), r(s.borderBottomLeftRadius)];
        }
        """;

    private static async Task<IPage> OpenAsync(IBrowserContext context, BrowserFixture fixture, string themeId)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/toggle-group-rtl");
        await Expect(page.Locator($"#h > .{Css.Classes.Button.Root}").First).ToBeVisibleAsync();
        // The button family animates its shape; a computed radius read mid-transition is the frame,
        // not the resting value.
        await page.AddStyleTagAsync(new() { Content = "*, *::before, *::after { transition: none !important; animation: none !important; }" });

        await page.Locator(themeId == "fluent2" ? "#fluent" : "#md3e").ClickAsync();
        var root = page.Locator($".{Css.Classes.Theme.Root}");
        await Expect(root).ToHaveClassAsync(new Regex(Regex.Escape(Css.Classes.Theme.ForTheme(themeId))));
        await page.Locator("#ltr").ClickAsync();
        await Expect(root).ToHaveAttributeAsync("dir", "ltr");
        // The theme's token bundle arrives with its stylesheet, which can land after the theme class
        // does; a radius read before it resolves is 0 for every corner.
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && getComputedStyle(el).borderTopLeftRadius !== '0px'; }",
            $"#h > .{Css.Classes.Button.Root}");
        // Park the pointer off the segments: a hover morph of an expressive button changes its corners.
        await page.Mouse.MoveAsync(0, 0);
        return page;
    }

    private static async Task SwitchToRtlAsync(IPage page)
    {
        await page.Locator("#rtl").ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Theme.Root}")).ToHaveAttributeAsync("dir", "rtl");
        await page.Mouse.MoveAsync(0, 0);
    }

    private static ILocator Segments(IPage page, string id) =>
        page.Locator($"#{id} > .{Css.Classes.Button.Root}");

    private static async Task<double[][]> MetricsAsync(IPage page, string id)
    {
        var segments = Segments(page, id);
        var count = await segments.CountAsync();
        var result = new double[count][];
        for (var i = 0; i < count; i++)
            result[i] = await segments.Nth(i).EvaluateAsync<double[]>(SegmentMetricsScript);
        return result;
    }

    private static void AssertNear(double expected, double actual, string what)
        => Assert.True(Math.Abs(expected - actual) <= 0.1, $"{what}: expected {expected:0.##}, got {actual:0.##}");

    // A horizontal mirror swaps the inline edges and the left/right corners of one segment; the block
    // edges and the top/bottom widths stay where they are.
    private static void AssertMirrored(double[][] ltr, double[][] rtl, string what)
    {
        Assert.Equal(ltr.Length, rtl.Length);
        for (var i = 0; i < ltr.Length; i++)
        {
            AssertNear(ltr[i][1], rtl[i][0], $"{what}[{i}] left width");
            AssertNear(ltr[i][0], rtl[i][1], $"{what}[{i}] right width");
            AssertNear(ltr[i][2], rtl[i][2], $"{what}[{i}] top width");
            AssertNear(ltr[i][3], rtl[i][3], $"{what}[{i}] bottom width");
            AssertNear(ltr[i][5], rtl[i][4], $"{what}[{i}] top-left radius");
            AssertNear(ltr[i][4], rtl[i][5], $"{what}[{i}] top-right radius");
            AssertNear(ltr[i][7], rtl[i][6], $"{what}[{i}] bottom-right radius");
            AssertNear(ltr[i][6], rtl[i][7], $"{what}[{i}] bottom-left radius");
        }
    }

    private static async Task<double[]> WidthsAsync(IPage page, string id)
        => await Segments(page, id).EvaluateAllAsync<double[]>("els => els.map(e => e.getBoundingClientRect().width)");

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task TheFrameAndCornersFollowTheInlineDirection(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, themeId);

        var ltr = new Dictionary<string, double[][]>();
        foreach (var id in new[] { "h", "v", "disabled", "full" })
            ltr[id] = await MetricsAsync(page, id);

        // LTR baseline: without these the mirror comparison below could pass on two identical zeroes.
        var h = ltr["h"];
        Assert.True(h[0][0] > 0 && h[0][1] > 0, "first segment must carry the frame's start edge and a divider");
        Assert.True(h[1][0] == 0 && h[1][1] > 0, "middle segment must carry only the divider on its inline end");
        Assert.True(h[2][0] == 0 && h[2][1] > 0, "last segment must carry the frame's end edge");
        Assert.True(h[0][4] > 0 && h[0][7] > 0 && h[0][5] == 0 && h[0][6] == 0,
            "first segment must round its outer start corners only");
        Assert.True(h[2][5] > 0 && h[2][6] > 0 && h[2][4] == 0 && h[2][7] == 0,
            "last segment must round its outer end corners only");
        Assert.True(h[1][4] == 0 && h[1][5] == 0 && h[1][6] == 0 && h[1][7] == 0,
            "middle segment must stay square");

        var v = ltr["v"];
        Assert.True(v[0][2] > 0 && v[0][3] > 0 && v[0][0] > 0 && v[0][1] > 0,
            "vertical first segment must carry the top, bottom and side edges");
        Assert.True(v[1][2] == 0 && v[1][3] > 0, "vertical middle segment must carry only the bottom divider");
        Assert.True(v[0][4] > 0 && v[0][5] > 0 && v[0][6] == 0 && v[0][7] == 0,
            "vertical first segment must round its top corners only");
        Assert.True(v[2][6] > 0 && v[2][7] > 0 && v[2][4] == 0 && v[2][5] == 0,
            "vertical last segment must round its bottom corners only");

        var ltrWidths = await WidthsAsync(page, "full");
        Assert.True(ltrWidths.Max() - ltrWidths.Min() <= 1, "full LTR segments are not split evenly");

        await SwitchToRtlAsync(page);

        // The DOM's first segment is the rightmost one now, and the frame has to be on that side of it.
        var firstBox = await Segments(page, "h").First.BoundingBoxAsync();
        var lastBox = await Segments(page, "h").Last.BoundingBoxAsync();
        Assert.True(firstBox!.X > lastBox!.X, "RTL should place the first segment at the inline start (the right)");

        foreach (var id in new[] { "h", "v", "disabled", "full" })
            AssertMirrored(ltr[id], await MetricsAsync(page, id), $"{themeId} {id}");

        var rtlWidths = await WidthsAsync(page, "full");
        Assert.True(rtlWidths.Max() - rtlWidths.Min() <= 1, "full RTL segments are not split evenly");
    }

    [Theory]
    [InlineData("md3-expressive")]
    [InlineData("fluent2")]
    public async Task ASelectedSegmentKeepsTheGroupsOuterCorners(string themeId)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, themeId);

        Assert.Equal("true", await Segments(page, "selected-first").First.GetAttributeAsync("aria-pressed"));
        Assert.Equal("true", await Segments(page, "selected-last").Last.GetAttributeAsync("aria-pressed"));
        Assert.Equal("true", await Segments(page, "single-selected").First.GetAttributeAsync("aria-pressed"));
        Assert.Equal("true", await Segments(page, "single-vertical").First.GetAttributeAsync("aria-pressed"));

        var selectedFirst = await MetricsAsync(page, "selected-first");
        var selectedLast = await MetricsAsync(page, "selected-last");
        var single = await MetricsAsync(page, "single");
        var singleSelected = await MetricsAsync(page, "single-selected");
        var singleVertical = await MetricsAsync(page, "single-vertical");

        Assert.True(selectedFirst[0][4] > 0 && selectedFirst[0][7] > 0
            && selectedFirst[0][5] == 0 && selectedFirst[0][6] == 0,
            "a selected first segment keeps its outer start corners");
        Assert.True(selectedLast[1][5] > 0 && selectedLast[1][6] > 0
            && selectedLast[1][4] == 0 && selectedLast[1][7] == 0,
            "a selected last segment keeps its outer end corners");
        Assert.True(single[0][4] > 0 && single[0][5] > 0 && single[0][6] > 0 && single[0][7] > 0,
            "a lone horizontal segment keeps all four corners");
        Assert.True(singleSelected[0][4] > 0 && singleSelected[0][5] > 0
            && singleSelected[0][6] > 0 && singleSelected[0][7] > 0,
            "a lone selected horizontal segment keeps all four corners");
        Assert.True(singleVertical[0][4] > 0 && singleVertical[0][5] > 0
            && singleVertical[0][6] > 0 && singleVertical[0][7] > 0,
            "a lone vertical segment keeps all four corners");

        await SwitchToRtlAsync(page);

        var rtlFirst = await MetricsAsync(page, "selected-first");
        var rtlLast = await MetricsAsync(page, "selected-last");
        Assert.True(rtlFirst[0][5] > 0 && rtlFirst[0][6] > 0 && rtlFirst[0][4] == 0 && rtlFirst[0][7] == 0,
            "in RTL a selected first segment rounds its right corners");
        Assert.True(rtlLast[1][4] > 0 && rtlLast[1][7] > 0 && rtlLast[1][5] == 0 && rtlLast[1][6] == 0,
            "in RTL a selected last segment rounds its left corners");
    }

    [Fact]
    public async Task TheFrameScrollsWithTheSegments()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, "md3-expressive");
        var scroller = page.Locator("#scroll");

        // Nothing of the frame lives on the scroller itself - a border there would stay put while the
        // segments slide underneath it.
        var scrollerBorders = await scroller.EvaluateAsync<double[]>(
            "el => { const s = getComputedStyle(el); return [parseFloat(s.borderLeftWidth), parseFloat(s.borderRightWidth), parseFloat(s.borderTopWidth), parseFloat(s.borderBottomWidth)]; }");
        Assert.All(scrollerBorders, w => Assert.Equal(0d, w));

        var first = Segments(page, "scroll").First;
        var size = await scroller.EvaluateAsync<double[]>("el => [el.scrollWidth, el.clientWidth]");
        Assert.True(size[0] > size[1], "the scroll group has to overflow its box for this test to mean anything");

        var ltrStart = await first.EvaluateAsync<double>("el => el.getBoundingClientRect().left");
        var ltrMetrics = await MetricsAsync(page, "scroll");
        Assert.True(ltrMetrics[0][4] > 0 && ltrMetrics[0][7] > 0, "LTR first segment must round its left corners");

        await scroller.EvaluateAsync("el => el.scrollLeft = 80");
        var ltrScrolled = await first.EvaluateAsync<double>("el => el.getBoundingClientRect().left");
        AssertNear(ltrStart - 80, ltrScrolled, "LTR frame should move with the scrolled segments");

        await SwitchToRtlAsync(page);
        await scroller.EvaluateAsync("el => el.scrollLeft = 0");

        var firstBox = await first.BoundingBoxAsync();
        var scrollerBox = await scroller.BoundingBoxAsync();
        AssertNear(scrollerBox!.X + scrollerBox.Width, firstBox!.X + firstBox.Width,
            "RTL scroll starts with the first segment at the right edge");
        var rtlMetrics = await MetricsAsync(page, "scroll");
        Assert.True(rtlMetrics[0][5] > 0 && rtlMetrics[0][6] > 0 && rtlMetrics[0][4] == 0 && rtlMetrics[0][7] == 0,
            "RTL first segment must round its right corners");

        // RTL scrollLeft is negative: moving it toward the end slides the content to the right.
        await scroller.EvaluateAsync("el => el.scrollLeft = -80");
        var rtlScrolled = await first.EvaluateAsync<double>("el => el.getBoundingClientRect().left");
        AssertNear(firstBox.X + 80, rtlScrolled, "RTL frame should move with the scrolled segments");
        var afterMetrics = await MetricsAsync(page, "scroll");
        AssertNear(rtlMetrics[0][5], afterMetrics[0][5], "the rounded corner stays on the segment while scrolling");
    }
}
