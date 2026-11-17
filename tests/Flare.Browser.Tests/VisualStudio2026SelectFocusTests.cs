using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The VS 2026 focus indicator on the select family. FlareSelect and FlareMultiSelect put the
/// focusable trigger on the shared field well, so the field's 1px accent outline must survive both
/// keyboard and pointer focus, while the inner input and search controls stay ring-free.
/// </summary>
public sealed class VisualStudio2026SelectFocusTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string Accent = "rgb(145, 132, 238)";

    private async Task<IBrowserContext> OpenDarkContextAsync()
    {
        var context = await fixture.Browser.NewContextAsync();
        await context.AddInitScriptAsync("""
            localStorage.setItem('flare-theme', 'visualstudio');
            localStorage.setItem('flare-palette', 'vs-blue');
            localStorage.setItem('flare-mode', 'dark');
            """);
        return context;
    }

    private static async Task WaitForFieldTokensAsync(IPage page, string fieldSelector)
    {
        // The theme class arrives before its token stylesheet; read a computed value only once the
        // VS 2026 tokens are live, otherwise the well still has its unthemed transparent fill.
        await page.WaitForFunctionAsync("""
            sel => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector(sel)).backgroundColor === 'rgb(53, 53, 53)'
            """, fieldSelector);
    }

    private static async Task TabUntilFocusedAsync(IPage page, string startSelector, ILocator target)
    {
        await page.Locator(startSelector).FocusAsync();
        for (var press = 0; press < 8; press++)
        {
            if (await target.EvaluateAsync<bool>("el => el === document.activeElement")) break;
            await page.Keyboard.PressAsync("Tab");
        }
        await Expect(target).ToBeFocusedAsync();
    }

    private static async Task AssertFieldOutlineAsync(ILocator field)
    {
        await Expect(field).ToHaveCSSAsync("outline-style", "solid");
        await Expect(field).ToHaveCSSAsync("outline-width", "1px");
        await Expect(field).ToHaveCSSAsync("outline-color", Accent);
        await Expect(field).ToHaveCSSAsync("outline-offset", "0px");
        await Expect(field).ToHaveCSSAsync("box-shadow", "none");
    }

    [Fact]
    public async Task Select_KeyboardFocusKeepsTheFieldOutline()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var trigger = page.Locator($"#metric-combo .{Css.Classes.Select.Control}");
        await WaitForFieldTokensAsync(page, $"#metric-combo .{Css.Classes.Input.Field}");

        await TabUntilFocusedAsync(page, "#metric-field input", trigger);

        Assert.True(await trigger.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await AssertFieldOutlineAsync(trigger);
    }

    [Fact]
    public async Task Select_PointerFocusKeepsTheFieldOutline()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var trigger = page.Locator($"#metric-combo .{Css.Classes.Select.Control}");
        await WaitForFieldTokensAsync(page, $"#metric-combo .{Css.Classes.Input.Field}");

        await trigger.ClickAsync();

        await Expect(trigger).ToBeFocusedAsync();
        Assert.False(await trigger.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await AssertFieldOutlineAsync(trigger);
    }

    [Fact]
    public async Task MultiSelect_KeyboardFocusKeepsTheFieldOutline()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var trigger = page.Locator($"#metric-multiselect .{Css.Classes.Multiselect.Control}");
        await WaitForFieldTokensAsync(page, $"#metric-multiselect .{Css.Classes.Input.Field}");

        await TabUntilFocusedAsync(page, "#metric-field input", trigger);

        Assert.True(await trigger.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await AssertFieldOutlineAsync(trigger);
    }

    [Fact]
    public async Task MultiSelect_PointerFocusKeepsTheFieldOutline()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var trigger = page.Locator($"#metric-multiselect .{Css.Classes.Multiselect.Control}");
        await WaitForFieldTokensAsync(page, $"#metric-multiselect .{Css.Classes.Input.Field}");

        await trigger.ClickAsync();

        await Expect(trigger).ToBeFocusedAsync();
        Assert.False(await trigger.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await AssertFieldOutlineAsync(trigger);
    }

    [Fact]
    public async Task TextField_KeepsTheFieldOutlineWithoutASecondInnerRing()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var field = page.Locator($"#metric-field .{Css.Classes.Input.Field}");
        var input = page.Locator($"#metric-field .{Css.Classes.Input.Control}");
        await WaitForFieldTokensAsync(page, $"#metric-field .{Css.Classes.Input.Field}");

        await input.ClickAsync();

        await Expect(input).ToBeFocusedAsync();
        await AssertFieldOutlineAsync(field);
        await Expect(input).ToHaveCSSAsync("outline-style", "none");
    }

    [Fact]
    public async Task SearchableSelect_KeepsTheFieldOutlineWithoutASecondInnerRing()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var field = page.Locator($"#metric-search-combo .{Css.Classes.Input.Field}");
        var search = page.Locator($"#metric-search-combo .{Css.Classes.Select.Search}");
        await WaitForFieldTokensAsync(page, $"#metric-search-combo .{Css.Classes.Input.Field}");

        await TabUntilFocusedAsync(page, "#metric-field input", search);

        Assert.True(await search.EvaluateAsync<bool>("el => el.matches(':focus-visible')"));
        await AssertFieldOutlineAsync(field);
        await Expect(search).ToHaveCSSAsync("outline-style", "none");
    }
}
