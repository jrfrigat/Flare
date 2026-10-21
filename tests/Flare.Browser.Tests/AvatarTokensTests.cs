using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Avatar geometry is the theme's: each size variant reads its diameter and text size from the token,
/// a consumer can override one, and the group's overlap comes from the theme unless the caller sets it.
/// </summary>
public sealed class AvatarTokensTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task AvatarSizesAndGroupSpacing_ComeFromTokens()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/avatar-tokens");
        await Expect(page.Locator("#md")).ToHaveCSSAsync("width", "40px");

        await Expect(page.Locator("#xs")).ToHaveCSSAsync("width", "24px");
        await Expect(page.Locator("#xs")).ToHaveCSSAsync("font-size", "10px");
        await Expect(page.Locator("#sm")).ToHaveCSSAsync("width", "32px");
        await Expect(page.Locator("#sm")).ToHaveCSSAsync("font-size", "12px");
        await Expect(page.Locator("#md")).ToHaveCSSAsync("height", "40px");
        await Expect(page.Locator("#md")).ToHaveCSSAsync("font-size", "16px");
        await Expect(page.Locator("#lg")).ToHaveCSSAsync("width", "48px");
        await Expect(page.Locator("#lg")).ToHaveCSSAsync("font-size", "18px");
        await Expect(page.Locator("#xl")).ToHaveCSSAsync("width", "64px");
        await Expect(page.Locator("#xl")).ToHaveCSSAsync("font-size", "24px");

        // The token itself is overridable: the wrapper redefines it and the avatar follows.
        await Expect(page.Locator("#overridden")).ToHaveCSSAsync("width", "48px");

        // Theme overlap (default token) and the caller's explicit value.
        var group = page.Locator($"#group .{Css.Classes.Avatar.Root}");
        Assert.Equal("0px", await MarginAsync(group.Nth(0)));
        Assert.Equal("-12px", await MarginAsync(group.Nth(1)));

        var spaced = page.Locator($"#group-spaced .{Css.Classes.Avatar.Root}");
        Assert.Equal("-8px", await MarginAsync(spaced.Nth(1)));
    }

    private static Task<string> MarginAsync(ILocator avatar)
        => avatar.EvaluateAsync<string>("el => getComputedStyle(el).marginInlineStart");
}
