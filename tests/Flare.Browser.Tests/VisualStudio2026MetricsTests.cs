using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The measured VS 2026 component metrics, read from the rendered shell: a 23px input well with a
/// #353535 fill and a #454545 stroke on 4px corners, a 24px command button carrying a 16px glyph, a
/// single 1px #9184EE focus line without a glow, and a 2px square scrollbar thumb.
/// </summary>
public sealed class VisualStudio2026MetricsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
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

    [Fact]
    public async Task FieldWell_IsTwentyThreePixelsOnFourPixelCorners()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var well = page.Locator($"#metric-field .{Css.Classes.Input.Field}");

        await Expect(well).ToHaveCSSAsync("height", "23px");
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

        await Expect(well).ToHaveCSSAsync("height", "23px");
        await Expect(well).ToHaveCSSAsync("border-radius", "4px");
        await Expect(well).ToHaveCSSAsync("background-color", "rgb(53, 53, 53)");
    }

    [Fact]
    public async Task TreeRows_AreTwentyFourPixelsWithSixteenPixelGlyphs()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var panel = page.Locator("#metric-toolpanel");

        var row = panel.Locator($".{Css.Classes.Tree.ItemRow}").First;
        await Expect(row).ToHaveCSSAsync("height", "24px");

        var toggle = panel.Locator($".{Css.Classes.TreeView.Toggle}").First;
        await Expect(toggle).ToHaveCSSAsync("width", "24px");
        await Expect(toggle).ToHaveCSSAsync("height", "24px");

        var icon = panel.Locator($".{Css.Classes.Tree.ItemIcon}").First;
        await Expect(icon).ToHaveCSSAsync("font-size", "16px");
    }

    [Fact]
    public async Task ChromeText_IsTwelvePixelsAndToolTitlesAreSentenceCase()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");

        var title = page.Locator($"#metric-toolpanel .{Css.Classes.Ide.ToolPanel.Title}");
        await Expect(title).ToHaveCSSAsync("font-size", "12px");
        await Expect(title).ToHaveCSSAsync("text-transform", "none");

        var status = page.Locator($"#metric-statusbar .{Css.Classes.Ide.StatusBar.Root}");
        await Expect(status).ToHaveCSSAsync("font-size", "12px");
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
    public async Task CommandBar_IsTheMeasuredTwentyEightPixelStrip()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var bar = page.Locator($"#metric-commandbar .{Css.Classes.Ide.Toolbar.Root}");
        await bar.WaitForAsync();
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector('#metric-commandbar .flare-toolbar'))
                    .getPropertyValue('--flare-ide-toolbar-height').trim() === '1.75rem'
            """);

        // The measured strip is 28px between its two 1px hard edges, so the border box is 30px.
        Assert.Equal(28, await bar.EvaluateAsync<int>("bar => bar.clientHeight"));
        await Expect(bar).ToHaveCSSAsync("border-top-width", "1px");
        await Expect(bar).ToHaveCSSAsync("border-bottom-width", "1px");
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(28, 28, 28)");
        await Expect(bar).ToHaveCSSAsync("gap", "4px");
        await Expect(bar).ToHaveCSSAsync("border-top-color", "rgb(14, 14, 14)");
        await Expect(bar).ToHaveCSSAsync("border-bottom-color", "rgb(14, 14, 14)");

        var group = bar.Locator($".{Css.Classes.Ide.Toolbar.Group}").First;
        await Expect(group).ToHaveCSSAsync("height", "28px");
        await Expect(group).ToHaveCSSAsync("background-color", "rgb(38, 38, 38)");

        var button = group.Locator($".{Css.Classes.Button.Root}").First;
        await Expect(button).ToHaveCSSAsync("height", "24px");
        await Expect(button).ToHaveCSSAsync("border-radius", "4px");

        var icon = button.Locator($".{Css.Classes.Button.Icon}");
        await Expect(icon).ToHaveCSSAsync("width", "16px");
        await Expect(icon).ToHaveCSSAsync("height", "16px");
    }

    [Fact]
    public async Task CommandBar_KeepsTheGroupDragHandle()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var handle = page.Locator($"#metric-commandbar .{Css.Classes.Ide.Toolbar.Handle}");

        await Expect(handle).ToHaveCountAsync(2);
        await Expect(handle.First).ToBeVisibleAsync();
        Assert.Equal("true", await handle.First.GetAttributeAsync("draggable"));
    }

    [Fact]
    public async Task DocumentTabs_AreTheMeasuredFlowingStrip()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var strip = page.Locator($"#metric-doctabs .{Css.Classes.Ide.DocTabs.Strip}");
        await strip.WaitForAsync();
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector('#metric-doctabs .flare-doc-tabs'))
                    .getPropertyValue('--flare-ide-doc-tab-close-icon').trim() === '0.625rem'
            """);

        Assert.Equal(24, await strip.EvaluateAsync<int>("el => el.clientHeight"));
        await Expect(strip).ToHaveCSSAsync("background-color", "rgb(38, 38, 38)");
        var stripShadow = await strip.EvaluateAsync<string>("el => getComputedStyle(el).boxShadow");
        Assert.Contains("inset", stripShadow);
        Assert.Contains("rgb(145, 132, 238)", stripShadow);

        var bar = page.Locator($"#metric-doctabs .{Css.Classes.Ide.DocTabs.Bar}");
        await Expect(bar).ToHaveCSSAsync("gap", "2px");
        await Expect(bar).ToHaveCSSAsync("padding-left", "6px");

        var active = page.Locator($"#metric-doctabs .{Css.Classes.Ide.DocTabs.TabActive}");
        await Expect(active).ToHaveCSSAsync("height", "24px");
        await Expect(active).ToHaveCSSAsync("background-color", "rgb(40, 40, 40)");
        await Expect(active).ToHaveCSSAsync("border-top-left-radius", "4px");
        await Expect(active).ToHaveCSSAsync("border-bottom-left-radius", "0px");
        await Expect(active).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(active).ToHaveCSSAsync("font-weight", "600");
        var activeShadow = await active.EvaluateAsync<string>("el => getComputedStyle(el).boxShadow");
        Assert.Contains("inset", activeShadow);
        Assert.Contains("rgb(145, 132, 238)", activeShadow);

        var inactive = page.Locator($"#metric-doctabs .{Css.Classes.Ide.DocTabs.Tab}").Nth(1);
        await Expect(inactive).ToHaveCSSAsync("color", "rgb(207, 207, 207)");

        var close = active.Locator($".{Css.Classes.Ide.DocTabs.Close}");
        await Expect(close).ToHaveCSSAsync("width", "20px");
        await Expect(close).ToHaveCSSAsync("height", "20px");
        var glyph = close.Locator($".{Css.Classes.Icon.Svg}");
        await Expect(glyph).ToHaveCSSAsync("width", "10px");
    }

    [Fact]
    public async Task MenuBar_IsTheMeasuredThirtyPixelBarWithAPill()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var bar = page.Locator($"#metric-menubar .{Css.Classes.Ide.MenuBar.Root}");
        await bar.WaitForAsync();
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector('#metric-menubar .flare-menubar'))
                    .getPropertyValue('--flare-ide-menubar-height').trim() === '1.875rem'
            """);

        Assert.Equal(30, await bar.EvaluateAsync<int>("el => el.getBoundingClientRect().height"));
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(28, 28, 28)");
        await Expect(bar).ToHaveCSSAsync("border-bottom-color", "rgb(14, 14, 14)");
        await Expect(bar).ToHaveCSSAsync("gap", "2px");

        var title = bar.Locator($".{Css.Classes.Button.Root}").First;
        await Expect(title).ToHaveCSSAsync("height", "24px");
        await Expect(title).ToHaveCSSAsync("border-radius", "4px");
        await Expect(title).ToHaveCSSAsync("padding-left", "8px");
        await Expect(title).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(title).ToHaveCSSAsync("font-weight", "400");

        await title.HoverAsync();
        var pill = await title.EvaluateAsync<string>("el => getComputedStyle(el, '::before').backgroundColor");
        Assert.Equal("rgb(41, 41, 41)", pill);
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
        var title = page.Locator($"#metric-menubar .{Css.Classes.Button.Root}").First;
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
    public async Task StatusBar_IsTheMeasuredTwentyFourPixelStrip()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        var bar = page.Locator($"#metric-statusbar .{Css.Classes.Ide.StatusBar.Root}");
        await bar.WaitForAsync();
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector('#metric-statusbar .flare-statusbar'))
                    .getPropertyValue('--flare-ide-statusbar-padding').trim() === '0 0.5rem'
            """);

        Assert.Equal(24, await bar.EvaluateAsync<int>("el => el.clientHeight"));
        await Expect(bar).ToHaveCSSAsync("background-color", "rgb(20, 20, 20)");
        await Expect(bar).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(bar).ToHaveCSSAsync("font-size", "12px");
        await Expect(bar).ToHaveCSSAsync("padding-left", "8px");
        await Expect(bar).ToHaveCSSAsync("padding-right", "8px");

        var left = bar.Locator($".{Css.Classes.Ide.StatusBar.Left}");
        var center = bar.Locator($".{Css.Classes.Ide.StatusBar.Center}");
        var right = bar.Locator($".{Css.Classes.Ide.StatusBar.Right}");
        await Expect(left).ToHaveCSSAsync("gap", "12px");
        await Expect(right).ToHaveCSSAsync("gap", "12px");
        await Expect(center).ToBeVisibleAsync();
        await Expect(left).ToHaveCSSAsync("border-right-width", "0px");

        var icon = left.Locator($".{Css.Classes.Icon.Svg}");
        await Expect(icon).ToHaveCSSAsync("width", "16px");
        await Expect(icon).ToHaveCSSAsync("height", "16px");
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
}
