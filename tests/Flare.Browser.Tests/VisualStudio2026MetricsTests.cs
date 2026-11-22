using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The measured VS 2026 base-component metrics, read from the rendered shell: a field on the VS button
/// height grid (28px at the default step) with a #353535 fill and a #454545 stroke on 4px corners, a
/// 24px command button carrying a 16px glyph, a single 1px #9184EE focus line without a glow, a 2px
/// square scrollbar thumb, and the tokens the theme leaves to an optional application shell.
/// </summary>
public sealed class VisualStudio2026MetricsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IBrowserContext> OpenDarkContextAsync(
        string theme = "visualstudio", string palette = "vs-blue")
    {
        var context = await fixture.Browser.NewContextAsync();
        await context.AddInitScriptAsync($$"""
            localStorage.setItem('flare-theme', '{{theme}}');
            localStorage.setItem('flare-palette', '{{palette}}');
            localStorage.setItem('flare-mode', 'dark');
            """);
        return context;
    }

    [Fact]
    public async Task FieldWell_IsTwentyEightPixelsOnFourPixelCorners()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-field .{Css.Classes.Input.Field}");

        await Expect(well).ToHaveCSSAsync("height", "28px");
        await Expect(well).ToHaveCSSAsync("border-radius", "4px");
        await Expect(well).ToHaveCSSAsync("background-color", "rgb(53, 53, 53)");
        await Expect(well).ToHaveCSSAsync("border-top-color", "rgb(69, 69, 69)");
    }

    [Fact]
    public async Task Focus_IsOneAccentLineWithoutAGlow()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-field .{Css.Classes.Input.Field}");

        await page.Locator("#metric-field input").FocusAsync();

        await Expect(well).ToHaveCSSAsync("outline-color", "rgb(145, 132, 238)");
        await Expect(well).ToHaveCSSAsync("outline-style", "solid");
        await Expect(well).ToHaveCSSAsync("outline-width", "1px");
        await Expect(well).ToHaveCSSAsync("box-shadow", "none");
    }

    [Fact]
    public async Task ComboWell_MatchesTheFieldHeight()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-combo .{Css.Classes.Input.Field}");

        await Expect(well).ToHaveCSSAsync("height", "28px");
        await Expect(well).ToHaveCSSAsync("border-radius", "4px");
        await Expect(well).ToHaveCSSAsync("background-color", "rgb(53, 53, 53)");
    }

    [Fact]
    public async Task TreeRows_AreTwentyFourPixelsWithSixteenPixelGlyphs()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var panel = page.Locator("#metric-tree");

        var row = panel.Locator($".{Css.Classes.Tree.ItemRow}").First;
        await Expect(row).ToHaveCSSAsync("height", "24px");

        var toggle = panel.Locator($".{Css.Classes.TreeView.Toggle}").First;
        await Expect(toggle).ToHaveCSSAsync("width", "24px");
        await Expect(toggle).ToHaveCSSAsync("height", "24px");

        var icon = panel.Locator($".{Css.Classes.Tree.ItemIcon}").First;
        await Expect(icon).ToHaveCSSAsync("font-size", "16px");
    }

    [Fact]
    public async Task CommandButton_IsTwentyFourPixelsWithASixteenPixelIcon()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var button = page.Locator("#metric-command");

        await Expect(button).ToHaveCSSAsync("height", "24px");
        await Expect(button).ToHaveCSSAsync("width", "24px");
        await Expect(button).ToHaveCSSAsync("border-radius", "4px");

        var icon = button.Locator($".{Css.Classes.Button.Icon}");
        await Expect(icon).ToHaveCSSAsync("width", "16px");
        await Expect(icon).ToHaveCSSAsync("height", "16px");
    }

    [Fact]
    public async Task DropdownMenu_IsTheMeasuredPanelAndItemRhythm()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
            """);
        var title = page.Locator($"#metric-menu .{Css.Classes.Button.Root}").First;
        await title.ClickAsync();

        var panel = page.Locator($".{Css.Classes.Menu.Panel}").First;
        await Expect(panel).ToBeVisibleAsync();
        await Expect(panel).ToHaveCSSAsync("background-color", "rgb(44, 44, 44)");
        await Expect(panel).ToHaveCSSAsync("border-top-left-radius", "6px");
        await Expect(panel).ToHaveCSSAsync("border-top-color", "rgb(14, 14, 14)");
        await Expect(panel).ToHaveCSSAsync("padding", "1px");
        await Expect(panel).ToHaveCSSAsync("row-gap", "2px");

        var first = panel.Locator($".{Css.Classes.Menu.Item}").First;
        await Expect(first).ToHaveCSSAsync("height", "22px");
        await Expect(first).ToHaveCSSAsync("border-radius", "4px");
        await Expect(first).ToHaveCSSAsync("padding-left", "12px");
        await Expect(first).ToHaveCSSAsync("padding-right", "8px");
        await Expect(first).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(first.Locator($".{Css.Classes.Menu.ItemShortcut}")).ToHaveCSSAsync("color", "rgb(133, 133, 133)");

        await first.HoverAsync();
        var pill = await first.EvaluateAsync<string>("el => getComputedStyle(el, '::before').backgroundColor");
        Assert.Equal("rgb(56, 56, 56)", pill);

        var disabled = panel.Locator($".{Css.Classes.Menu.ItemDisabled}").First;
        await Expect(disabled).ToHaveCSSAsync("color", "rgb(133, 133, 133)");

        var separator = panel.Locator($".{Css.Classes.Menu.Separator}");
        await Expect(separator).ToHaveCSSAsync("border-top-color", "rgb(61, 61, 61)");
        await Expect(separator).ToHaveCSSAsync("margin-top", "5px");

        var check = panel.Locator($".{Css.Classes.Menu.ItemCheckIcon}");
        await Expect(check).ToHaveCSSAsync("color", "rgb(145, 132, 238)");

        // The checked row reserves the 16px check slot plus the 2px gap: its label starts 32px from
        // the panel edge, and its shortcut ends 10px from the panel's right edge.
        var offsets = await page.EvaluateAsync<double[]>("""
            () => {
              const panel = document.querySelector('.flare-menu__panel');
              const row = panel.querySelector('.flare-menu-item[aria-checked="true"]');
              const label = row.querySelector('.flare-menu-item__label');
              const shortcut = row.querySelector('.flare-menu-item__shortcut');
              const p = panel.getBoundingClientRect();
              return [
                label.getBoundingClientRect().left - p.left,
                p.right - shortcut.getBoundingClientRect().right,
              ];
            }
            """);
        Assert.Equal(32, offsets[0], 0.5);
        Assert.Equal(10, offsets[1], 0.5);
    }

    [Fact]
    public async Task Scrollbar_UsesTheTwoPixelWebKitPath()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var scroller = page.Locator("#metric-scroller");

        // Chromium paints ::-webkit-scrollbar only while the standard properties stay at auto; the
        // headless browser uses overlay scrollbars, so the layout width is not a usable measurement,
        // but the mechanism and every token that feeds it are.
        await Expect(scroller).ToHaveCSSAsync("scrollbar-width", "auto");
        await Expect(scroller).ToHaveCSSAsync("scrollbar-color", "auto");
        await page.WaitForFunctionAsync($"() => document.documentElement.classList.contains('{Css.Classes.Theme.ModeDark}')");

        var values = await page.EvaluateAsync<string[]>("""
            () => {
                const s = getComputedStyle(document.querySelector('#metric-scroller'));
                return [
                    s.getPropertyValue('--flare-scrollbar-size').trim(),
                    s.getPropertyValue('--flare-scrollbar-thumb').trim(),
                    s.getPropertyValue('--flare-scrollbar-track').trim(),
                    s.getPropertyValue('--flare-scrollbar-radius').trim(),
                ];
            }
            """);
        Assert.Equal("2px", values[0]);
        Assert.Equal("#9D9D9D", values[1]);
        Assert.Equal("#282828", values[2]);
        Assert.Equal("0px", values[3]);
    }

    [Fact]
    public async Task Tabs_KeepTheirExplicitHiddenScrollbarUnderVisualStudio()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var bar = page.Locator($"#metric-tabs .{Css.Classes.Tabs.Bar}");
        await Expect(bar).ToHaveCSSAsync("scrollbar-width", "none");
    }

    [Fact]
    public async Task NestedTheme_KeepsItsOwnScrollbarWidthInsideVisualStudio()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var scroller = page.Locator("#metric-nested-md3");
        await Expect(scroller).ToHaveCSSAsync("scrollbar-width", "thin");
    }

    [Fact]
    public async Task NestedVisualStudioScope_GetsTheThemesScrollbarWidth()
    {
        await using var context = await OpenDarkContextAsync("md3", "md3-violet");
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var scroller = page.Locator("#metric-nested-vs");
        await Expect(scroller).ToHaveCSSAsync("scrollbar-width", "auto");
    }

    [Fact]
    public async Task IdeComponents_UseTheirOwnDefaultsUnderVisualStudio()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var bar = page.Locator("#metric-ide-default .flare-statusbar");
        await Expect(bar).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");

        // The theme does not declare the IDE hook; the component falls back to its own base-token
        // default, and the application is the only one that can supply a measured value.
        Assert.Equal(string.Empty, await TokenAsync(page, "#metric-ide-default", "--flare-ide-statusbar-surface"));
    }

    [Fact]
    public async Task IdeComponentToken_CanBeOverriddenByTheApplication()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var bar = page.Locator("#metric-ide-overridden .flare-statusbar");
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(1, 2, 3)");
        Assert.Equal("rgb(1, 2, 3)", await TokenAsync(page, "#metric-ide-overridden", "--flare-ide-statusbar-surface"));
    }

    [Fact]
    public async Task IdeComponents_UseTheirOwnDefaultsUnderAnotherTheme()
    {
        await using var context = await OpenDarkContextAsync("md3", "md3-violet");
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await WaitForThemeAsync(page);

        var bar = page.Locator("#metric-ide-default .flare-statusbar");
        await Expect(bar).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");
        Assert.Equal(string.Empty, await TokenAsync(page, "#metric-ide-default", "--flare-ide-statusbar-surface"));
    }

    private static async Task WaitForThemeAsync(IPage page) =>
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.documentElement).getPropertyValue('--flare-spacing-6').trim() !== ''
            """);

    private static async Task<string> TokenAsync(IPage page, string selector, string name) =>
        await page.EvaluateAsync<string>(
            $"sel => getComputedStyle(document.querySelector(sel)).getPropertyValue('{name}').trim()",
            selector);
}
