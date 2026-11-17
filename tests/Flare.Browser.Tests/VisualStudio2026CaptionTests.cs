using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The VS 2026 title-bar caption controls, read from the rendered shell: a 44x30 control in the
/// title row with a 10px glyph, the chrome hover and pressed surfaces, the accent focus line, the
/// faded disabled state, the Windows close red, and a mousedown that never starts a title-bar drag.
/// </summary>
public sealed class VisualStudio2026CaptionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
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

    private async Task<IPage> OpenMetricsAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/vs2026-metrics");
        await page.WaitForFunctionAsync("""
            () => document.documentElement.classList.contains('flare-mode-dark')
                && getComputedStyle(document.querySelector('#metric-caption-minimize')).color === 'rgb(255, 255, 255)'
            """);
        return page;
    }

    [Fact]
    public async Task CaptionControls_AreTheMeasuredTitleBarBoxes()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);

        var minimize = page.Locator("#metric-caption-minimize");
        await Expect(minimize).ToHaveCSSAsync("width", "44px");
        await Expect(minimize).ToHaveCSSAsync("height", "30px");
        await Expect(minimize).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");
        await Expect(minimize).ToHaveCSSAsync("color", "rgb(255, 255, 255)");

        var glyph = minimize.Locator($".{Css.Classes.Ide.CaptionButton.Icon} .{Css.Classes.Icon.Root}");
        await Expect(glyph).ToHaveCSSAsync("width", "10px");
        await Expect(glyph).ToHaveCSSAsync("height", "10px");

        Assert.Equal("minimize", await minimize.GetAttributeAsync("data-flare-caption"));
        Assert.Equal("maximize", await page.Locator("#metric-caption-maximize").GetAttributeAsync("data-flare-caption"));
        Assert.Equal("restore", await page.Locator("#metric-caption-restore").GetAttributeAsync("data-flare-caption"));
        Assert.Equal("close", await page.Locator("#metric-caption-close").GetAttributeAsync("data-flare-caption"));

        var closeClass = await page.Locator("#metric-caption-close").GetAttributeAsync("class") ?? string.Empty;
        var minimizeClass = await minimize.GetAttributeAsync("class") ?? string.Empty;
        Assert.Contains(Css.Classes.Ide.CaptionButton.Close, closeClass);
        Assert.DoesNotContain(Css.Classes.Ide.CaptionButton.Close, minimizeClass);
    }

    [Fact]
    public async Task HoverAndPress_TakeTheChromeSurfaces()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);
        var minimize = page.Locator("#metric-caption-minimize");

        await minimize.HoverAsync();
        await Expect(minimize).ToHaveCSSAsync("background-color", "rgb(56, 56, 56)");
        await Expect(minimize).ToHaveCSSAsync("color", "rgb(255, 255, 255)");

        await page.Mouse.DownAsync();
        await Expect(minimize).ToHaveCSSAsync("background-color", "rgb(69, 69, 69)");
        await Expect(minimize).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await page.Mouse.UpAsync();
    }

    [Fact]
    public async Task Close_TakesTheWindowsRedOnHoverAndPress()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);
        var close = page.Locator("#metric-caption-close");

        await close.HoverAsync();
        await Expect(close).ToHaveCSSAsync("background-color", "rgb(196, 43, 28)");
        await Expect(close).ToHaveCSSAsync("color", "rgb(255, 255, 255)");

        await page.Mouse.DownAsync();
        await Expect(close).ToHaveCSSAsync("background-color", "rgb(178, 42, 26)");
        await Expect(close).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await page.Mouse.UpAsync();
    }

    [Fact]
    public async Task Focus_DrawsTheSingleAccentLine()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);
        var minimize = page.Locator("#metric-caption-minimize");

        await minimize.FocusAsync();

        await Expect(minimize).ToHaveCSSAsync("outline-color", "rgb(145, 132, 238)");
        await Expect(minimize).ToHaveCSSAsync("outline-style", "solid");
        await Expect(minimize).ToHaveCSSAsync("outline-width", "1px");
        await Expect(minimize).ToHaveCSSAsync("outline-offset", "-2px");
    }

    [Fact]
    public async Task Disabled_FadesAndKeepsTheRestingFill()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);
        var disabled = page.Locator("#metric-caption-disabled");

        Assert.True(await disabled.IsDisabledAsync());
        await Expect(disabled).ToHaveCSSAsync("opacity", "0.4");

        await disabled.HoverAsync();
        await Expect(disabled).ToHaveCSSAsync("background-color", "rgba(0, 0, 0, 0)");
    }

    [Fact]
    public async Task CaptionPress_DoesNotStartTheTitleBarDrag()
    {
        await using var context = await OpenDarkContextAsync();
        var page = await OpenMetricsAsync(context);

        await page.Locator("#metric-caption-minimize").ClickAsync();
        await Expect(page.Locator("#metric-caption-mousedowns")).ToHaveTextAsync("0");

        await page.Locator("#metric-caption-title").ClickAsync();
        await Expect(page.Locator("#metric-caption-mousedowns")).ToHaveTextAsync("1");
    }
}
