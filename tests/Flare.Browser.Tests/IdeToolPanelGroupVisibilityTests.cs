using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A tool panel group keeps every panel mounted but paints only the active one. The panel root
/// sets <c>display: flex</c> in the library CSS, so the <c>hidden</c> attribute alone does not
/// remove an inactive panel from the layout; a markup-only test cannot see that contract.
/// </summary>
public sealed class IdeToolPanelGroupVisibilityTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task InactivePanelIsNotPaintedAndKeepsItsStateAcrossTabSwitches()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-toolpanel-group");

        var panels = page.Locator("#group .flare-toolpanel-group__body > .flare-toolpanel");
        await Expect(panels).ToHaveCountAsync(2);
        await Expect(panels.Nth(0)).ToHaveCSSAsync("display", "flex");
        await Expect(panels.Nth(1)).ToHaveCSSAsync("display", "none");

        var input = page.Locator("#explorer-input");
        await input.FillAsync("Kept draft");
        await page.GetByRole(AriaRole.Tab, new() { Name = "Properties", Exact = true }).ClickAsync();

        await Expect(panels.Nth(0)).ToHaveCSSAsync("display", "none");
        await Expect(panels.Nth(1)).ToHaveCSSAsync("display", "flex");
        await Expect(page.Locator("#properties-text")).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Tab, new() { Name = "Explorer", Exact = true }).ClickAsync();

        await Expect(panels.Nth(0)).ToHaveCSSAsync("display", "flex");
        await Expect(panels.Nth(1)).ToHaveCSSAsync("display", "none");
        await Expect(input).ToHaveValueAsync("Kept draft");
    }
}
