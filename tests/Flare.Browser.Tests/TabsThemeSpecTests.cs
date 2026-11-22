using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// FluentUI2 tabs follow docs/spec/tabs/fluentui2-spec.md: body1 in rest, body1Strong selected,
/// neutral foreground2/1 labels, the 3px rounded inset indicator, the pending hover line and the
/// brand selected icon. The values are checked through the computed style, not through the tokens.
/// </summary>
public sealed class TabsThemeSpecTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task LightTabsFollowTheFluentSpec()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-theme-spec");

        var light = page.Locator("#tabs-light");
        var active = light.Locator("[role=tab][aria-selected=true]");
        var inactive = light.Locator("[role=tab][aria-selected=false]");
        await Expect(active).ToBeVisibleAsync();

        await Expect(active).ToHaveCSSAsync("font-weight", "600");
        await Expect(active).ToHaveCSSAsync("color", "rgb(36, 36, 36)");
        await Expect(active).ToHaveCSSAsync("height", "44px");
        await Expect(active).ToHaveCSSAsync("padding-left", "10px");
        await Expect(active).ToHaveCSSAsync("padding-top", "12px");
        await Expect(active).ToHaveCSSAsync("padding-bottom", "12px");
        await Expect(active).ToHaveCSSAsync("gap", "6px");
        await Expect(inactive).ToHaveCSSAsync("font-weight", "400");
        await Expect(inactive).ToHaveCSSAsync("color", "rgb(66, 66, 66)");
        await Expect(light.Locator($".{Css.Classes.Tabs.BarWrap}")).ToHaveCSSAsync("border-bottom-width", "0px");

        Assert.Equal("rgb(15, 108, 189)", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').backgroundColor"));
        Assert.Equal("3px", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').height"));
        Assert.Equal("12px", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').left"));
        Assert.Equal("12px", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').right"));
        Assert.NotEqual("0px", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').borderTopLeftRadius"));

        // The selected icon takes compound brand, not the label colour.
        await Expect(active.Locator($".{Css.Classes.Icon.Root}").First).ToHaveCSSAsync("color", "rgb(15, 108, 189)");

        // The pending indicator is the hover affordance; there is no hover fill.
        await inactive.HoverAsync();
        Assert.Equal("rgb(199, 199, 199)", await inactive.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::before').backgroundColor"));
        Assert.Equal("2px", await inactive.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::before').height"));
        await Expect(inactive).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");
    }

    [Fact]
    public async Task DarkTabsFollowTheFluentSpec()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-theme-spec");

        var dark = page.Locator("#tabs-dark");
        var active = dark.Locator("[role=tab][aria-selected=true]");
        var inactive = dark.Locator("[role=tab][aria-selected=false]");
        await Expect(active).ToBeVisibleAsync();

        await Expect(active).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(inactive).ToHaveCSSAsync("color", "rgb(214, 214, 214)");
        Assert.Equal("rgb(71, 158, 245)", await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').backgroundColor"));
    }

    // The gap and block padding are optional TabsTokens roles: a theme that states nothing keeps the
    // shared spacing steps, so only Fluent's own values may move.
    [Fact]
    public async Task DefaultsKeepTheSharedSpacingSteps()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-theme-spec");

        var active = page.Locator("#tabs-md3 [role=tab][aria-selected=true]");
        await Expect(active).ToBeVisibleAsync();

        await Expect(active).ToHaveCSSAsync("gap", "8px");
        await Expect(active).ToHaveCSSAsync("padding-top", "12px");
        await Expect(active).ToHaveCSSAsync("padding-bottom", "12px");
    }

    // Any Flare theme runs with any palette: the Fluent indicator is a compound brand paint, so the
    // Excel palette must move it away from the Blue one through the base primary role.
    [Fact]
    public async Task IndicatorFollowsTheSelectedPalette()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-theme-spec");

        var excel = page.Locator("#tabs-excel");
        var active = excel.Locator("[role=tab][aria-selected=true]");
        await Expect(active).ToBeVisibleAsync();
        // The scope applies its palette class after the first paint; wait for it before measuring.
        await page.WaitForFunctionAsync(
            "() => document.querySelector('#tabs-excel')?.closest('.flare-palette-fluent-excel') !== null");

        var indicator = await active.EvaluateAsync<string>(
            "el => getComputedStyle(el, '::after').backgroundColor");
        var primary = await page.EvaluateAsync<string>(
            "css => { const root = document.querySelector('#tabs-excel');" +
            " const probe = document.createElement('span'); probe.style.color = css;" +
            " root.append(probe); const color = getComputedStyle(probe).color; probe.remove(); return color; }",
            $"var({Css.Tokens.Color.Primary})");

        Assert.Equal(primary, indicator);
        Assert.NotEqual("rgb(15, 108, 189)", indicator);
    }

    // A hover is the preview of the selection: the pending line and the selected indicator must sit on
    // the same edge of a vertical tab, and the edge must face the panel (end edge for a left bar, start
    // edge for a right one).
    [Theory]
    [InlineData("tabs-vertical-left", "right")]
    [InlineData("tabs-vertical-right", "left")]
    public async Task VerticalTabsPaintHoverAndSelectionOnTheSameEdge(string scopeId, string edge)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/tabs-theme-spec");

        var scope = page.Locator($"#{scopeId}");
        var active = scope.Locator("[role=tab][aria-selected=true]");
        var inactive = scope.Locator("[role=tab][aria-selected=false]").First;
        await Expect(active).ToBeVisibleAsync();

        static string Edge(string left, string right) => left == "0px" ? "left" : right == "0px" ? "right" : "none";

        var selectedEdge = Edge(
            await active.EvaluateAsync<string>("el => getComputedStyle(el, '::after').left"),
            await active.EvaluateAsync<string>("el => getComputedStyle(el, '::after').right"));

        await inactive.HoverAsync();
        var hoverEdge = Edge(
            await inactive.EvaluateAsync<string>("el => getComputedStyle(el, '::before').left"),
            await inactive.EvaluateAsync<string>("el => getComputedStyle(el, '::before').right"));

        Assert.Equal(edge, selectedEdge);
        Assert.Equal(selectedEdge, hoverEdge);
    }
}
