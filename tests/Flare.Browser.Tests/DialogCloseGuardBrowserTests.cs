using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class DialogCloseGuardBrowserTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("inline", "scrim")]
    [InlineData("inline", "escape")]
    [InlineData("inline", "cross")]
    [InlineData("inline", "body")]
    [InlineData("inline", "navigation")]
    [InlineData("service", "scrim")]
    [InlineData("service", "escape")]
    [InlineData("service", "cross")]
    [InlineData("service", "body")]
    [InlineData("service", "navigation")]
    public async Task DirtyFormCanCancelThenConfirmEveryClosePath(string mode, string trigger)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, mode);
        await page.Locator("#guard-name").FillAsync("Changed draft");
        await page.Locator("#guard-name").PressAsync("Tab");
        var editor = page.GetByRole(AriaRole.Dialog, new() { Name = mode == "inline" ? "Inline editor" : "Service editor", Exact = true });
        var prompt = page.GetByRole(AriaRole.Dialog, new() { Name = "Discard unsaved changes?", Exact = true });

        await CloseAsync(page, editor, trigger);
        await Expect(prompt).ToBeVisibleAsync();
        await Expect(prompt.Locator(":focus")).ToHaveCountAsync(1);
        await prompt.GetByRole(AriaRole.Button, new() { Name = "Keep editing", Exact = true }).ClickAsync();
        await Expect(prompt).ToBeHiddenAsync();
        await Expect(editor).ToBeVisibleAsync();
        await Expect(page.Locator("#guard-name")).ToHaveValueAsync("Changed draft");
        await Expect(page).ToHaveURLAsync($"{fixture.BaseUrl}/dialog-guard");

        await CloseAsync(page, editor, trigger);
        await Expect(prompt).ToBeVisibleAsync();
        await prompt.GetByRole(AriaRole.Button, new() { Name = "Discard changes", Exact = true }).ClickAsync();
        await Expect(prompt).ToBeHiddenAsync();
        await Expect(editor).ToBeHiddenAsync();
        if (trigger == "navigation") await Expect(page).ToHaveURLAsync($"{fixture.BaseUrl}/dialog-guard/next");
    }

    [Theory]
    [InlineData("inline", false)]
    [InlineData("inline", true)]
    [InlineData("service", false)]
    [InlineData("service", true)]
    public async Task CleanOrSavedFormClosesWithoutConfirmation(string mode, bool save)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, mode);
        if (save)
        {
            await page.Locator("#guard-name").FillAsync("Saved draft");
            await page.Locator("#guard-name").PressAsync("Tab");
        }
        await page.Locator(save ? "#body-save" : "#body-close").ClickAsync();
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
    }

    private static Task CloseAsync(IPage page, ILocator editor, string trigger) => trigger switch
    {
        "scrim" => page.Mouse.ClickAsync(890, 10),
        "escape" => page.Keyboard.PressAsync("Escape"),
        "cross" => editor.Locator($".{Css.Classes.Dialog.CloseButton}").ClickAsync(),
        "navigation" => editor.Locator("#body-navigate").ClickAsync(),
        _ => editor.Locator("#body-close").ClickAsync()
    };

    private async Task<IPage> OpenAsync(IBrowserContext context, string mode)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(900, 700);
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.GotoAsync($"{fixture.BaseUrl}/dialog-guard");
        await page.EvaluateAsync("() => import('./_content/Flare.Components/js/flare-theme.js')");
        await page.Locator($"#open-{mode}").ClickAsync();
        await Expect(page.Locator("#guard-name")).ToBeVisibleAsync();
        return page;
    }
}
