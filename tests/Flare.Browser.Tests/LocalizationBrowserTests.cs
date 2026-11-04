using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class LocalizationBrowserTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private readonly List<string> _errors = [];
    private async Task<IPage> OpenAsync(IBrowserContext context, string ui = "en")
    {
        var page = await context.NewPageAsync();
        page.PageError += (_, message) => _errors.Add(message);
        page.Console += (_, message) => { if (message.Type == "error") _errors.Add(message.Text); };
        await page.GotoAsync($"{fixture.BaseUrl}/localization-test?ui={ui}");
        await Expect(page.Locator("#ui")).ToHaveTextAsync(ui);
        return page;
    }

    [Fact]
    public async Task ServerCircuitsKeepIndependentLanguagesAndFormatOverrides()
    {
        await using var first = await fixture.Browser.NewContextAsync();
        await using var second = await fixture.Browser.NewContextAsync();
        var english = await OpenAsync(first);
        var russian = await OpenAsync(second, "ru");
        await english.Locator("#fr").ClickAsync();
        await english.Locator("#format").ClickAsync();
        await Expect(english.Locator("#ui")).ToHaveTextAsync("fr");
        await Expect(russian.Locator("#ui")).ToHaveTextAsync("ru");
        await Expect(english.Locator("#number input")).ToHaveValueAsync("1.234,50");
        await Expect(english.Locator("#explicit input")).ToHaveValueAsync("1,234.50");
        await Expect(russian.Locator("#number input")).ToHaveValueAsync("1,234.50");
        Assert.Empty(_errors);
    }

    [Fact]
    public async Task OpenEditorKeepsNativeInputFocusCaretAndUnsavedValueAcrossRepeatedSwitches()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#open").ClickAsync();
        var input = page.Locator("#draft input");
        await input.FillAsync("Unsaved draft");
        await input.PressAsync("ArrowLeft");
        var position = await input.EvaluateAsync<int>("x => x.selectionStart");
        await input.EvaluateAsync("x => window.localeInput = x");
        await page.EvaluateAsync("window.localeDocument = document");
        foreach (var culture in new[] { "ru", "fr", "en", "ru" })
        {
            await input.PressAsync("Control+Alt+L");
            await Expect(page.Locator("#ui")).ToHaveTextAsync(culture);
            await Expect(input).ToHaveValueAsync("Unsaved draft");
            await Expect(input).ToBeFocusedAsync();
            Assert.Equal(position, await input.EvaluateAsync<int>("x => x.selectionStart"));
            Assert.True(await input.EvaluateAsync<bool>("x => x === window.localeInput && document === window.localeDocument"));
        }
        await Expect(page.Locator("[role=dialog]")).ToBeVisibleAsync();
        Assert.Empty(_errors);
    }

    [Fact]
    public async Task OpenPopupAndDynamicallyMountedPickersFollowCultureAfterDispose()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#dynamic").ClickAsync();
        await page.Locator("#dynamic").ClickAsync();
        await page.Locator("#fr").ClickAsync();
        await page.Locator("#dynamic").ClickAsync();
        await Expect(page.Locator("#dynamic-picker")).ToContainTextAsync("Effacer");
        await page.Locator("#en").ClickAsync();
        await page.Locator("#popup-open").ClickAsync();
        var popup = page.Locator("#locale-popup");
        await Expect(popup).ToBeVisibleAsync();
        var month = popup.Locator($".{Css.Classes.Calendar.Header}");
        await popup.GetByRole(AriaRole.Button, new() { Name = "Next month", Exact = true }).ClickAsync();
        await Expect(month).ToHaveTextAsync("December 2026");
        var caption = await month.InnerTextAsync();
        await page.Locator("#popup-language").ClickAsync();
        await Expect(page.Locator("#ui")).ToHaveTextAsync("ru");
        await Expect(popup).ToBeVisibleAsync();
        await Expect(month).ToHaveTextAsync(caption);
        await Expect(page.Locator("#popup-language")).ToBeFocusedAsync();
        await Expect(popup.GetByRole(AriaRole.Button, new() { Name = "\u0421\u043b\u0435\u0434\u0443\u044e\u0449\u0438\u0439 \u043c\u0435\u0441\u044f\u0446", Exact = true })).ToBeVisibleAsync();
        Assert.Empty(_errors);
    }
}
