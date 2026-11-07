using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ShortcutIsolationTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task TwoServerCircuitsAndNestedOwnersDispatchIndependently()
    {
        await using var firstContext = await fixture.Browser.NewContextAsync();
        await using var secondContext = await fixture.Browser.NewContextAsync();
        var first = await firstContext.NewPageAsync();
        var second = await secondContext.NewPageAsync();
        var errors = new List<string>();
        foreach (var page in new[] { first, second })
        {
            page.PageError += (_, error) => errors.Add(error);
            page.Console += (_, message) => { if (message.Type == "error") errors.Add(message.Text); };
            await page.GotoAsync(fixture.BaseUrl + "/shortcuts-test");
            await Expect(page.Locator("#main")).ToHaveTextAsync("0");
        }
        await first.Keyboard.PressAsync("Control+Alt+J");
        await Expect(first.Locator("#main")).ToHaveTextAsync("1");
        await second.Keyboard.PressAsync("Control+Alt+J");
        await Expect(second.Locator("#main")).ToHaveTextAsync("1");
        await Expect(first.Locator("#main")).ToHaveTextAsync("1");
        await first.Keyboard.PressAsync("Control+Alt+K");
        await Expect(first.Locator("#nested")).ToHaveTextAsync("1");
        await Expect(second.Locator("#nested")).ToHaveTextAsync("0");
        await first.Keyboard.PressAsync("Control+Alt+I");
        await Expect(first.Locator("#sibling")).ToHaveTextAsync("1");
        await first.Locator("#remove-sibling").ClickAsync();
        await first.Locator("#remove-nested").ClickAsync();
        await first.Keyboard.PressAsync("Control+Alt+J");
        await Expect(first.Locator("#main")).ToHaveTextAsync("2");
        await first.Locator("#change").ClickAsync();
        await first.Keyboard.PressAsync("Control+Alt+J");
        await first.Keyboard.PressAsync("Control+Alt+K");
        await first.Keyboard.PressAsync("Control+Alt+I");
        await first.Keyboard.PressAsync("Control+Alt+M");
        await Expect(first.Locator("#main")).ToHaveTextAsync("3");
        await Expect(first.Locator("#nested")).ToHaveTextAsync("1");
        await Expect(first.Locator("#sibling")).ToHaveTextAsync("1");
        await Expect(second.Locator("#main")).ToHaveTextAsync("1");
        await firstContext.CloseAsync();
        await second.Keyboard.PressAsync("Control+Alt+J");
        await Expect(second.Locator("#main")).ToHaveTextAsync("2");
        Assert.Empty(errors);
    }
}
