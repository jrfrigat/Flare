using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ModalFocusTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    public static IEnumerable<object[]> Cases =>
        from theme in new[] { "md3-expressive", "fluent2" }
        from drawer in new[] { false, true }
        from content in new[] { "text", "empty", "headed" }
        select new object[] { theme, drawer, content };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task PanelWithoutControls_ReceivesAndKeepsFocus_ThenRestoresOpener(string theme, bool drawer, string content)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await OpenScreen(page, theme, drawer, content);
        var opener = page.Locator("#opener");
        await opener.PressAsync("Enter");
        var panel = page.GetByRole(AriaRole.Dialog, new() { Name = "Subject", Exact = true });
        await Expect(panel).ToBeFocusedAsync();
        await panel.PressAsync("Tab");
        await Expect(panel).ToBeFocusedAsync();
        await panel.PressAsync("Shift+Tab");
        await Expect(panel).ToBeFocusedAsync();
        await page.Locator("#background").FocusAsync();
        await Expect(panel).ToBeFocusedAsync();
        await panel.PressAsync("Escape");
        await Expect(panel).ToBeHiddenAsync();
        await Expect(opener).ToBeFocusedAsync();
        await opener.PressAsync("Tab");
        await Expect(page.Locator("#background")).ToBeFocusedAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RemovingAllControls_TabRecoversPanelFocus(bool drawer)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await OpenScreen(page, "md3-expressive", drawer, "text", true);
        await page.Locator("#opener").PressAsync("Enter");
        await Expect(page.Locator("#remove")).ToBeFocusedAsync();
        await page.Locator("#remove").PressAsync("Enter");
        await Expect(page.Locator("#remove")).ToHaveCountAsync(0);
        await page.Keyboard.PressAsync("Tab");
        var panel = page.GetByRole(AriaRole.Dialog, new() { Name = "Subject", Exact = true });
        await Expect(panel).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Shift+Tab");
        await Expect(panel).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(panel).ToBeHiddenAsync();
        await Expect(page.Locator("#opener")).ToBeFocusedAsync();
    }

    [Fact]
    public async Task EmptyNestedPanel_RestoresOuterControl_ThenOriginalOpener()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await OpenScreen(page, "md3-expressive", false, "text", true);
        await page.Locator("#opener").PressAsync("Enter");
        await Expect(page.Locator("#remove")).ToBeFocusedAsync();
        await page.Locator("#nested").PressAsync("Enter");
        var inner = page.GetByRole(AriaRole.Dialog, new() { Name = "Inner", Exact = true });
        await Expect(inner).ToBeFocusedAsync();
        await inner.PressAsync("Tab");
        await Expect(inner).ToBeFocusedAsync();
        await inner.PressAsync("Escape");
        await Expect(inner).ToBeHiddenAsync();
        await Expect(page.Locator("#nested")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(page.Locator("#opener")).ToBeFocusedAsync();
    }

    private async Task OpenScreen(IPage page, string theme, bool drawer, string content, bool interactive = false)
    {
        await page.GotoAsync($"{fixture.BaseUrl}/modal-focus?drawer={drawer}&content={content}&interactive={interactive}");
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync(theme);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-theme", theme);
    }
}
