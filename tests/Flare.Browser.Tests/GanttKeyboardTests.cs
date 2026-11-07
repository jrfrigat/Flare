using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class GanttKeyboardTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task TreeRowsNavigateCollapseAndLeaveInputsAlone()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt-keyboard");
        await ListenerWait.ForAsync(page, "[role=treegrid]", "keydown");
        ILocator Row(string key) => page.Locator($"[role=row][data-row='{key}']");
        await Expect(page.Locator("[role=row][tabindex='0']")).ToHaveCountAsync(1);
        await Row("a").FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");
        await Expect(page.Locator("[role=row]")).ToHaveCountAsync(2);
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page.Locator("[role=row]")).ToHaveCountAsync(5);
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(Row("a1")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(Row("a2")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(Row("a21")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowLeft");
        await Expect(Row("a2")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(Row("b")).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Home");
        await Expect(Row("a")).ToBeFocusedAsync();
        var input = page.GetByRole(AriaRole.Textbox, new() { Name = "Edit a", Exact = true });
        await input.FocusAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await Expect(input).ToBeFocusedAsync();
    }

    [Fact]
    public async Task EndAndHomeFocusRowsOutsideTheVirtualWindow()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/gantt");
        await ListenerWait.ForAsync(page, "[role=grid]", "keydown");
        var first = page.Locator("[role=row][data-row='r0']");
        await first.FocusAsync();
        await page.Keyboard.PressAsync("End");
        await Expect(page.Locator("[role=row][data-row='r499']")).ToBeFocusedAsync();
        Assert.True(await page.Locator("[role=row]").CountAsync() < 60);
        await page.Keyboard.PressAsync("Home");
        await Expect(first).ToBeFocusedAsync();
        await Expect(page.Locator("[role=row][tabindex='0']")).ToHaveCountAsync(1);
    }
}
