using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// RTL fixes: Pagination's chevrons follow the reading direction while its flex order mirrors, and
/// its rows-per-page spacing uses a logical margin.
/// </summary>
public sealed class RtlComponentsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Pagination_ArrowsFollowTheReadingDirection()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 },
            DeviceScaleFactor = 2
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/rtl-components");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.documentElement).getPropertyValue('{Css.Tokens.Spacing.S4}').trim() !== ''");
        await page.EvaluateAsync("() => document.fonts.ready");

        var rtlPrev = page.Locator("#rtl-pagination button[aria-label='Previous page']");
        var rtlNext = page.Locator("#rtl-pagination button[aria-label='Next page']");
        await Expect(rtlPrev).ToBeVisibleAsync();
        Assert.Equal("rtl", await page.Locator($"#rtl-pagination .{Css.Classes.Pagination.Root}")
            .EvaluateAsync<string>("el => getComputedStyle(el).direction"));

        // What the reader sees: in RTL "previous" sits on the right and points right.
        Assert.Equal("right", await GlyphPointsAsync(page, rtlPrev));
        Assert.Equal("left", await GlyphPointsAsync(page, rtlNext));

        var rtlRows = page.Locator($"#rtl-pagination .{Css.Classes.Pagination.Rows}");
        Assert.True(await rtlRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)") > 0);
        Assert.Equal(0, await rtlRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)"));

        var ltrPrev = page.Locator("#ltr-pagination button[aria-label='Previous page']");
        var ltrNext = page.Locator("#ltr-pagination button[aria-label='Next page']");
        Assert.Equal("left", await GlyphPointsAsync(page, ltrPrev));
        Assert.Equal("right", await GlyphPointsAsync(page, ltrNext));
        var ltrRows = page.Locator($"#ltr-pagination .{Css.Classes.Pagination.Rows}");
        Assert.True(await ltrRows.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)") > 0);
    }

    // Reads the rendered chevron from a screenshot of the button: the ink nearest the text colour is
    // found column by column, and a chevron's two arms make its open side taller than its tip.
    private static async Task<string> GlyphPointsAsync(IPage page, ILocator button)
    {
        var png = Convert.ToBase64String(await button.ScreenshotAsync());
        var color = await button.EvaluateAsync<string>("el => getComputedStyle(el).color");
        return await page.EvaluateAsync<string>(
            """
            async ([png, color]) => {
                const img = new Image();
                img.src = 'data:image/png;base64,' + png;
                await img.decode();
                const canvas = document.createElement('canvas');
                canvas.width = img.width;
                canvas.height = img.height;
                const ctx = canvas.getContext('2d');
                ctx.drawImage(img, 0, 0);
                const data = ctx.getImageData(0, 0, img.width, img.height).data;
                const [r, g, b] = color.match(/\d+(\.\d+)?/g).map(Number);
                const edge = Math.round(Math.min(img.width, img.height) * 0.15);
                const spans = [];
                for (let x = edge; x < img.width - edge; x++) {
                    let top = -1, bottom = -1;
                    for (let y = edge; y < img.height - edge; y++) {
                        const i = (y * img.width + x) * 4;
                        const d = Math.abs(data[i] - r) + Math.abs(data[i + 1] - g) + Math.abs(data[i + 2] - b);
                        if (d < 120) { if (top < 0) top = y; bottom = y; }
                    }
                    if (top >= 0) spans.push(bottom - top);
                }
                if (spans.length < 3) return 'none';
                const first = spans[0] + spans[1];
                const last = spans[spans.length - 1] + spans[spans.length - 2];
                return first > last ? 'right' : first < last ? 'left' : 'none';
            }
            """,
            new object[] { png, color });
    }

    [Fact]
    public async Task NavMenu_MirrorsIndentAndActiveBar()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1200, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/rtl-components");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.documentElement).getPropertyValue('{Css.Tokens.Spacing.S8}').trim() !== ''");

        var rtlActive = page.Locator($"#rtl-nav .{Css.Classes.Navigation.NavLinkActive}");
        var rtlInner = page.Locator($"#rtl-nav .{Css.Classes.Navigation.GroupItemsInner}");
        Assert.Equal(3, await rtlActive.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).borderRightWidth)"));
        Assert.Equal(0, await rtlActive.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).borderLeftWidth)"));
        Assert.True(await rtlInner.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).paddingRight)") > 0);
        Assert.Equal(0, await rtlInner.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).paddingLeft)"));

        var ltrActive = page.Locator($"#ltr-nav .{Css.Classes.Navigation.NavLinkActive}");
        var ltrInner = page.Locator($"#ltr-nav .{Css.Classes.Navigation.GroupItemsInner}");
        Assert.Equal(3, await ltrActive.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).borderLeftWidth)"));
        Assert.Equal(0, await ltrActive.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).borderRightWidth)"));
        Assert.True(await ltrInner.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).paddingLeft)") > 0);
        Assert.Equal(0, await ltrInner.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).paddingRight)"));
    }

    [Fact]
    public async Task Chip_MirrorsLeadingAvatarAndCloseSpacing()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1200, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/rtl-components");
        await page.WaitForFunctionAsync(
            $"() => getComputedStyle(document.documentElement).getPropertyValue('{Css.Tokens.Spacing.S2}').trim() !== ''");

        var rtlAvatar = page.Locator($"#rtl-chip .{Css.Classes.Chip.Avatar}");
        var rtlClose = page.Locator($"#rtl-chip .{Css.Classes.Chip.Close}");
        Assert.True(await rtlAvatar.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)") < 0);
        Assert.Equal(0, await rtlAvatar.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)"));
        Assert.True(await rtlClose.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)") > 0);
        Assert.Equal(0, await rtlClose.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)"));

        var ltrAvatar = page.Locator($"#ltr-chip .{Css.Classes.Chip.Avatar}");
        var ltrClose = page.Locator($"#ltr-chip .{Css.Classes.Chip.Close}");
        Assert.True(await ltrAvatar.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)") < 0);
        Assert.Equal(0, await ltrAvatar.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)"));
        Assert.True(await ltrClose.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginLeft)") > 0);
        Assert.Equal(0, await ltrClose.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).marginRight)"));
    }
}
