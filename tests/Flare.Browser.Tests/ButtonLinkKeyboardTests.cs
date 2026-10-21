using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A link-shaped FlareButton carries role="button", so Space must activate it and must not scroll the
/// page. Enter stays the browser's own link activation and must not fire the action twice.
/// </summary>
public sealed class ButtonLinkKeyboardTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task LinkButton_AnswersSpaceWithoutScrolling()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/button-link-keyboard");
        await page.WaitForFunctionAsync("() => !!globalThis.__flareLinkButtonKeyboard");

        var act = page.GetByRole(AriaRole.Button, new() { Name = "Act", Exact = true });
        await Expect(act).ToBeVisibleAsync();
        await act.FocusAsync();
        await Expect(page.Locator("#clicks")).ToHaveTextAsync("0");
        var scrollBefore = await page.EvaluateAsync<double>("window.scrollY");

        await page.Keyboard.PressAsync("Space");
        await Expect(page.Locator("#clicks")).ToHaveTextAsync("1");
        Assert.Equal(scrollBefore, await page.EvaluateAsync<double>("window.scrollY"));

        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#clicks")).ToHaveTextAsync("2");

        var blocked = page.Locator("a[aria-disabled='true']");
        await blocked.FocusAsync();
        var scrollBeforeBlocked = await page.EvaluateAsync<double>("window.scrollY");
        await page.Keyboard.PressAsync("Space");
        await Expect(page.Locator("#disabled-clicks")).ToHaveTextAsync("0");
        Assert.Equal(scrollBeforeBlocked, await page.EvaluateAsync<double>("window.scrollY"));
        Assert.Equal("none", await blocked.EvaluateAsync<string>("el => getComputedStyle(el).pointerEvents"));
        var opacity = await blocked.EvaluateAsync<double>("el => parseFloat(getComputedStyle(el).opacity)");
        var activeColor = await act.EvaluateAsync<string>("el => getComputedStyle(el).color");
        var disabledColor = await blocked.EvaluateAsync<string>("el => getComputedStyle(el).color");
        Assert.True(opacity < 1 || disabledColor != activeColor, "the disabled link keeps the active paint");
    }
}
