using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>A panel cannot be visible before its first outside click and Escape can dismiss it.</summary>
public sealed class PopupOpeningDismissTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("registerDismiss(id, element, dotNetRef, method)")]
    [InlineData("pushPopupLayer(id, dotNetRef, method)")]
    public async Task FirstOutsideClickAfterVisibility_ClosesThePopup(string operation)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        await context.AddInitScriptAsync("window.delayPopupSync = true;");
        await context.RouteAsync("**/flare-overlay.js*", async route =>
        {
            var response = await route.FetchAsync();
            var source = await response.TextAsync();
            var signature = $"export function {operation} {{";
            Assert.Contains(signature, source);
            source = source.Replace(signature, $"export async function {operation} {{" + """

                if (window.delayPopupSync) {
                    window.popupSyncWaiting = true;
                    await new Promise(resolve => window.releasePopupSync = resolve);
                    window.popupSyncWaiting = false;
                }
                """);
            await route.FulfillAsync(new() { Response = response, Body = source });
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-filter");
        var trigger = page.Locator($"button.{Css.Classes.DataGrid.FilterTrigger}").First;
        var menu = page.Locator($".{Css.Classes.DataGrid.FilterMenu}");
        await trigger.ClickAsync();
        await page.WaitForFunctionAsync("() => window.popupSyncWaiting === true");
        try
        {
            await Expect(menu).ToBeHiddenAsync();
        }
        finally
        {
            await page.EvaluateAsync("() => { window.delayPopupSync = false; window.releasePopupSync(); }");
        }
        await Expect(menu).ToBeVisibleAsync();
        Assert.Equal("fixed", await menu.EvaluateAsync<string>("element => getComputedStyle(element).position"));
        await page.Locator("#page-background").ClickAsync();
        await Expect(menu).ToBeHiddenAsync();
        await trigger.ClickAsync();
        await Expect(menu).ToBeVisibleAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(menu).ToBeHiddenAsync();
    }
}
