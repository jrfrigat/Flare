using System.Text.RegularExpressions;
using Flare.Css.Classes;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Conditional formatting must survive the Stacked card surface. A row class and a cell class from
/// the public hooks paint their own background; the card chrome no longer wins by specificity.
/// </summary>
public sealed class DataGridStackedFormattingTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task RowAndCellClassBackgrounds_PaintOverTheStackedCard()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1200, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-stacked-formatting");
        await page.WaitForSelectorAsync("[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var root = page.Locator($"#grid-format .{DataGrid.Root}");
        await page.WaitForSelectorAsync($"#grid-format .{DataGrid.Stacked}");

        var row = root.Locator($"tbody tr.{DataGrid.Row}").First;
        Assert.Equal("rgb(255, 0, 0)", await row.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor"));

        var nameCell = row.Locator("td[data-label='Name']");
        var cityCell = row.Locator("td[data-label='City']");
        Assert.Equal("rgba(0, 0, 0, 0)", await nameCell.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor"));
        Assert.Equal("rgb(0, 0, 255)", await cityCell.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor"));

        // The second row keeps the card surface.
        var secondRow = root.Locator($"tbody tr.{DataGrid.Row}").Nth(1);
        Assert.NotEqual("rgb(255, 0, 0)", await secondRow.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor"));
    }
}
