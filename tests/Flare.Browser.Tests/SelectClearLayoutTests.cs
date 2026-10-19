using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class SelectClearLayoutTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private sealed record Rects(double FieldLeft, double FieldRight, double ClearLeft, double ClearRight,
        double ArrowLeft, double ArrowRight)
    {
        public bool HasClear => ClearLeft >= 0;
        public double ClearToArrowGap => ArrowLeft - ClearRight;
        public double ArrowToFieldRight => FieldRight - ArrowRight;
        public double ArrowToFieldLeft => ArrowLeft - FieldLeft;
    }

    private static readonly string FieldSelector =
        $".{Css.Classes.Select.Control}, .{Css.Classes.Multiselect.Control}, .{Css.Classes.Input.Field}";
    private static readonly string ClearSelector = $".{Css.Classes.Autocomplete.Clear}";
    private static readonly string ArrowSelector =
        $".{Css.Classes.Input.Arrow}, .{Css.Classes.Autocomplete.Icon}";

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/select-layout");
        await Expect(page.Locator("[data-testid=wide-plain]")).ToBeVisibleAsync();
        return page;
    }

    private static async Task<Rects> MeasureAsync(IPage page, string testId)
    {
        var m = await page.EvaluateAsync<double[]>(
            "({ root, field, clear, arrow }) => {"
            + " const scope = document.querySelector(root);"
            + " const f = scope.querySelector(field).getBoundingClientRect();"
            + " const c = scope.querySelector(clear)?.getBoundingClientRect();"
            + " const arrows = scope.querySelectorAll(arrow);"
            + " const a = arrows[arrows.length - 1].getBoundingClientRect();"
            + " return [f.left, f.right, c ? c.left : -1, c ? c.right : -1, a.left, a.right]; }",
            new { root = $"[data-testid='{testId}']", field = FieldSelector, clear = ClearSelector, arrow = ArrowSelector });
        return new Rects(m[0], m[1], m[2], m[3], m[4], m[5]);
    }

    private static async Task ApplyThemeAsync(IPage page, string theme, bool dark)
    {
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync(theme);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-theme", theme);
        var mode = dark ? "Dark" : "Light";
        await page.GetByLabel("Mode", new() { Exact = true }).SelectOptionAsync(mode);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-mode", mode);
        await page.EvaluateAsync("async () => { await Promise.all(document.getAnimations().map(a => a.finished.catch(() => {}))); }");
    }

    private static void AssertClearIsNextToTheArrow(Rects r, string subject)
    {
        Assert.True(r.HasClear, $"{subject}: expected a clear button");
        Assert.True(r.ClearToArrowGap is >= -1 and <= 40,
            $"{subject}: clear button is {r.ClearToArrowGap:F1}px from the arrow instead of next to it");
        Assert.True(r.ArrowToFieldRight <= 24,
            $"{subject}: arrow is {r.ArrowToFieldRight:F1}px from the trailing edge");
    }

    [Theory]
    [InlineData("fluent2", false)]
    [InlineData("fluent2", true)]
    [InlineData("md3-expressive", false)]
    [InlineData("aero", true)]
    public async Task TheClearButtonStaysNextToTheArrow_AcrossThemes(string theme, bool dark)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await ApplyThemeAsync(page, theme, dark);

        AssertClearIsNextToTheArrow(await MeasureAsync(page, "wide-plain"), $"{theme}/{dark}");
    }

    [Fact]
    public async Task EveryClearableSelectKeepsTheTrailingButtonsTogether()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await ApplyThemeAsync(page, "fluent2", dark: false);

        AssertClearIsNextToTheArrow(await MeasureAsync(page, "wide-search"), "searchable select");
        AssertClearIsNextToTheArrow(await MeasureAsync(page, "narrow"), "narrow select");
        AssertClearIsNextToTheArrow(await MeasureAsync(page, "multiselect"), "multi-select");
        AssertClearIsNextToTheArrow(await MeasureAsync(page, "combobox"), "combobox");
    }

    [Fact]
    public async Task Rtl_PutsTheClearButtonNextToTheArrowAtTheTrailingEdge()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var r = await MeasureAsync(page, "wide-rtl");

        Assert.True(r.HasClear, "RTL select: expected a clear button");
        Assert.True(r.ClearLeft - r.ArrowRight is >= -1 and <= 40,
            $"RTL select: clear button is {r.ClearLeft - r.ArrowRight:F1}px from the arrow");
        Assert.True(r.ArrowToFieldLeft <= 24, $"RTL select: arrow is {r.ArrowToFieldLeft:F1}px from the leading edge");
    }

    [Fact]
    public async Task EmptyReadOnlyAndDisabledFields_ShowNoClearButton()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        foreach (var id in new[] { "empty", "readonly", "disabled" })
        {
            var r = await MeasureAsync(page, id);
            Assert.False(r.HasClear, $"{id}: no clear button expected");
            Assert.True(r.ArrowToFieldRight <= 24, $"{id}: arrow is {r.ArrowToFieldRight:F1}px from the trailing edge");
        }
    }
}
