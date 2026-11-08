using System.Text.RegularExpressions;
using Flare.Css.Classes;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Auto is measured against the grid's own container: the narrow host stacks while the wide one
/// stays a table in the same page. The stacked cells are flex label/value lines whose labels come
/// from data-label, and the table must not overflow horizontally.
/// </summary>
public sealed class DataGridStackedTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task NarrowContainer_Stacks_AndWideContainer_KeepsTheTable()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1200, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/datagrid-stacked");
        await page.WaitForSelectorAsync("[data-flare-theme]");
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");

        var narrowRoot = page.Locator($"#grid-narrow .{DataGrid.Root}");
        var wideRoot = page.Locator($"#grid-wide .{DataGrid.Root}");

        await page.WaitForSelectorAsync($"#grid-narrow .{DataGrid.Stacked}");
        await Expect(narrowRoot).ToHaveClassAsync(new Regex(DataGrid.Stacked));
        await Expect(wideRoot).Not.ToHaveClassAsync(new Regex(DataGrid.Stacked));

        var overflows = await narrowRoot.EvaluateAsync<bool>(
            $"el => {{ const w = el.querySelector('.{DataGrid.Wrapper}'); return w.scrollWidth > w.clientWidth + 1; }}");
        Assert.False(overflows);

        var tableMinWidth = await narrowRoot.Locator("table").EvaluateAsync<string>("el => getComputedStyle(el).minWidth");
        Assert.Equal("0px", tableMinWidth);

        var headerRowDisplay = await narrowRoot.Locator("thead > tr").First
            .EvaluateAsync<string>("el => getComputedStyle(el).display");
        Assert.Equal("flex", headerRowDisplay);

        var cell = narrowRoot.Locator($"tbody tr.{DataGrid.Row} td[data-label='Name']").First;
        var computed = await cell.EvaluateAsync<string[]>(
            "el => [getComputedStyle(el).display, getComputedStyle(el, '::after').content, getComputedStyle(el, '::after').order]");
        Assert.Equal("flex", computed[0]);
        Assert.Contains("Name", computed[1]);
        Assert.Equal("-1", computed[2]);

        var card = narrowRoot.Locator($"tbody tr.{DataGrid.Row}").First;
        Assert.Equal("solid", await card.EvaluateAsync<string>("el => getComputedStyle(el).borderTopStyle"));
        Assert.NotEqual("0px", await card.EvaluateAsync<string>("el => getComputedStyle(el).marginTop"));

        // The header remains the control bar: sorting still works after the layout changes. The locator
        // is not filtered by aria-sort, because the value it filters on changes with the click.
        var sortableHeader = narrowRoot.Locator($"thead th.{DataGrid.ThSortable}").First;
        await sortableHeader.ClickAsync();
        await Expect(sortableHeader).ToHaveAttributeAsync("aria-sort", "ascending");
        await Expect(narrowRoot).ToHaveClassAsync(new Regex(DataGrid.Stacked));

        // Selection and filtering keep working: the record checkbox still selects, and the filter row
        // is part of the control bar above the records.
        var firstRow = narrowRoot.Locator($"tbody tr.{DataGrid.Row}").First;
        await firstRow.Locator("input[type=checkbox]").First.ClickAsync();
        await Expect(firstRow).ToHaveAttributeAsync("aria-selected", "true");

        var filterInput = narrowRoot.Locator($"thead tr.{DataGrid.FilterRow} input").First;
        await filterInput.FillAsync("Ada");
        await Expect(narrowRoot.Locator($"tbody tr.{DataGrid.Row}")).ToHaveCountAsync(1);

        var wideCell = wideRoot.Locator($"tbody tr.{DataGrid.Row} td").First;
        Assert.Equal("table-cell", await wideCell.EvaluateAsync<string>("el => getComputedStyle(el).display"));
    }
}
