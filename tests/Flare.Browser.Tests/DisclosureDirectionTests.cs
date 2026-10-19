using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Disclosure headers, menu item labels and the submenu chevron are written from the start edge: a
/// wrapped header has to break lines at the same edge in RTL as in LTR, a menu item's text has to sit
/// against the item's start, and the chevron has to point at the side the flyout opens on.
/// </summary>
public sealed class DisclosureDirectionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string TextLineRectsScript = """
        el => {
            const walker = document.createTreeWalker(el, NodeFilter.SHOW_TEXT);
            let node = null;
            while ((node = walker.nextNode())) {
                if (node.nodeValue.trim().length > 0) break;
                node = null;
            }
            if (!node) return [];
            const range = document.createRange();
            range.selectNodeContents(node);
            return Array.from(range.getClientRects()).map(r => [r.left, r.right]);
        }
        """;

    private const string BoxAndPaddingScript = """
        el => {
            const s = getComputedStyle(el);
            const r = el.getBoundingClientRect();
            return [r.left, r.right, parseFloat(s.paddingLeft), parseFloat(s.paddingRight)];
        }
        """;

    private const string MatrixScript = """
        el => {
            const t = getComputedStyle(el).transform;
            const round = v => Math.round(v * 1000) / 1000;
            if (t === 'none') return [1, 0, 0, 1];
            const m = new DOMMatrixReadOnly(t);
            return [round(m.a), round(m.b), round(m.c), round(m.d)];
        }
        """;

    private static async Task<IPage> OpenAsync(IBrowserContext context, BrowserFixture fixture, bool rtl, string themeId)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/disclosure-menu");
        await Expect(page.Locator("#acc button[aria-expanded]").First).ToBeVisibleAsync();
        // Text metrics decide where the lines break; wait for the real font before measuring them.
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        // The animated transforms below are read as they settle; measurement is about the resting
        // value, not the frame the transition happens to be on.
        await page.AddStyleTagAsync(new() { Content = "*, *::before, *::after { transition: none !important; animation: none !important; }" });

        await page.Locator(themeId == "fluent2" ? "#fluent" : "#md3e").ClickAsync();
        var root = page.Locator($".{Css.Classes.Theme.Root}");
        await Expect(root).ToHaveClassAsync(new Regex(Regex.Escape(Css.Classes.Theme.ForTheme(themeId))));
        await page.Locator(rtl ? "#rtl" : "#ltr").ClickAsync();
        await Expect(root).ToHaveAttributeAsync("dir", rtl ? "rtl" : "ltr");
        // The token bundle arrives with the theme stylesheet; the header's padding is 0 until it does.
        await page.WaitForFunctionAsync(
            "sel => { const el = document.querySelector(sel); return !!el && parseFloat(getComputedStyle(el).paddingLeft) > 0; }",
            $"#acc .{Css.Classes.Accordion.Header}");
        return page;
    }

    private static async Task<double[][]> LinesAsync(ILocator text)
    {
        var rects = await text.EvaluateAsync<double[][]>(TextLineRectsScript);
        Assert.NotNull(rects);
        return rects;
    }

    private static async Task AssertWrapsToStartEdgeAsync(ILocator text, bool rtl, string what)
    {
        var lines = await LinesAsync(text);
        Assert.True(lines.Length >= 2, $"{what}: expected the header to wrap, got {lines.Length} line(s)");

        var align = await text.EvaluateAsync<string>("el => getComputedStyle(el).textAlign");
        Assert.Equal("start", align);

        var box = await text.EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.left, r.right]; }");
        var middle = (box[0] + box[1]) / 2;

        if (rtl)
        {
            var spread = lines.Max(r => r[1]) - lines.Min(r => r[1]);
            Assert.True(spread <= 1, $"{what}: RTL lines do not share their right edge (spread {spread:0.##}px)");
            Assert.True(lines[0][1] >= middle, $"{what}: RTL text does not start at the right edge");
        }
        else
        {
            var spread = lines.Max(r => r[0]) - lines.Min(r => r[0]);
            Assert.True(spread <= 1, $"{what}: LTR lines do not share their left edge (spread {spread:0.##}px)");
            Assert.True(lines[0][0] <= middle, $"{what}: LTR text does not start at the left edge");
        }
    }

    [Theory]
    [InlineData("md3-expressive", false)]
    [InlineData("md3-expressive", true)]
    [InlineData("fluent2", false)]
    [InlineData("fluent2", true)]
    public async Task WrappedDisclosureHeadersBreakAtTheStartEdge(string themeId, bool rtl)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, rtl, themeId);

        var panels = page.Locator($"#acc .{Css.Classes.Accordion.Panel}");
        await AssertWrapsToStartEdgeAsync(
            panels.Nth(0).Locator($".{Css.Classes.Accordion.PanelHeaderText}"), rtl, $"{themeId} accordion");
        // A right-to-left script in the same header, proving the start edge is the paragraph's and not
        // the script's.
        await AssertWrapsToStartEdgeAsync(
            panels.Nth(1).Locator($".{Css.Classes.Accordion.PanelHeaderText}"), rtl, $"{themeId} hebrew accordion");
        await AssertWrapsToStartEdgeAsync(
            page.Locator($"#collapse .{Css.Classes.Collapse.HeaderText}"), rtl, $"{themeId} collapse");
    }

    [Theory]
    [InlineData("md3-expressive", false)]
    [InlineData("md3-expressive", true)]
    [InlineData("fluent2", false)]
    [InlineData("fluent2", true)]
    public async Task TheMenuItemAndSubmenuChevronSitOnTheStartAndEndEdges(string themeId, bool rtl)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, rtl, themeId);

        await page.GetByRole(AriaRole.Button, new() { Name = "Actions", Exact = true }).ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Menu.Panel}")).ToBeVisibleAsync();

        var item = page.Locator($".{Css.Classes.Menu.Panel} .{Css.Classes.Menu.Item}").First;
        var label = item.Locator($".{Css.Classes.Menu.ItemLabel}");
        var icon = page.Locator($".{Css.Classes.Menu.SubmenuIcon}");

        var align = await item.EvaluateAsync<string>("el => getComputedStyle(el).textAlign");
        Assert.Equal("start", align);

        var box = await item.EvaluateAsync<double[]>(BoxAndPaddingScript);
        var labelLines = await LinesAsync(label);
        Assert.Single(labelLines);

        if (rtl)
        {
            Assert.True(Math.Abs(labelLines[0][1] - (box[1] - box[3])) <= 1.5,
                $"{themeId} item: label right edge {labelLines[0][1]:0.##} is not the start padding edge {box[1] - box[3]:0.##}");
            Assert.Equal(new[] { -1, 0, 0, 1 }, await icon.EvaluateAsync<int[]>(MatrixScript));
            var iconBox = await icon.EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.left, r.right]; }");
            Assert.True(Math.Abs(iconBox[0] - (box[0] + box[2])) <= 1.5,
                $"{themeId} item: chevron left edge {iconBox[0]:0.##} is not the end padding edge {box[0] + box[2]:0.##}");

            await page.Locator($".{Css.Classes.Menu.SubmenuActivator}").HoverAsync();
            await Expect(page.Locator($".{Css.Classes.Menu.Submenu}")).ToHaveAttributeAsync("aria-expanded", "true");
            Assert.Equal(new[] { 0, -1, -1, 0 }, await icon.EvaluateAsync<int[]>(MatrixScript));
        }
        else
        {
            Assert.True(Math.Abs(labelLines[0][0] - (box[0] + box[2])) <= 1.5,
                $"{themeId} item: label left edge {labelLines[0][0]:0.##} is not the start padding edge {box[0] + box[2]:0.##}");
            Assert.Equal(new[] { 1, 0, 0, 1 }, await icon.EvaluateAsync<int[]>(MatrixScript));
            var iconBox = await icon.EvaluateAsync<double[]>("el => { const r = el.getBoundingClientRect(); return [r.left, r.right]; }");
            Assert.True(Math.Abs(iconBox[1] - (box[1] - box[3])) <= 1.5,
                $"{themeId} item: chevron right edge {iconBox[1]:0.##} is not the end padding edge {box[1] - box[3]:0.##}");

            await page.Locator($".{Css.Classes.Menu.SubmenuActivator}").HoverAsync();
            await Expect(page.Locator($".{Css.Classes.Menu.Submenu}")).ToHaveAttributeAsync("aria-expanded", "true");
            Assert.Equal(new[] { 0, 1, -1, 0 }, await icon.EvaluateAsync<int[]>(MatrixScript));
        }
    }

    [Fact]
    public async Task ALongHeaderDoesNotOverflowANarrowViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, fixture, rtl: true, "md3-expressive");
        await page.SetViewportSizeAsync(320, 900);

        var header = page.Locator($"#acc .{Css.Classes.Accordion.Header}").First;
        var size = await header.EvaluateAsync<double[]>("el => [el.scrollWidth, el.clientWidth]");
        Assert.True(size[0] <= size[1] + 1, $"accordion header overflows: {size[0]:0.#} > {size[1]:0.#}");

        var documentSize = await page.EvaluateAsync<double[]>(
            "() => { const d = document.scrollingElement; return [d.scrollWidth, d.clientWidth]; }");
        Assert.True(documentSize[0] <= documentSize[1] + 1,
            $"page scrolls horizontally: {documentSize[0]:0.#} > {documentSize[1]:0.#}");
    }
}
