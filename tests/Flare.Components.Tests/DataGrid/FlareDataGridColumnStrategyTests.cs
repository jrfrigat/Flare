using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridColumnStrategyTests : FlareTestContext
{
    private record Ticket(string Name, string Priority);

    private static readonly Ticket[] _tickets =
    [
        new("A", "Low"),
        new("B", "High"),
        new("C", "Medium"),
    ];

    private static int Rank(string p) => p switch { "High" => 0, "Medium" => 1, "Low" => 2, _ => 3 };

    private static RenderFragment Grid(Comparison<Ticket>? sortComparison = null, Func<Ticket, string, bool>? filterFunc = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Ticket>>(0);
        b.AddAttribute(1, "Items", _tickets.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Ticket>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Ticket, object?>)(t => t.Name));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Ticket>>(20);
            inner.AddAttribute(21, "Title", "Priority");
            inner.AddAttribute(22, "Field", (Func<Ticket, object?>)(t => t.Priority));
            if (sortComparison is not null)
            {
                inner.AddAttribute(23, "Sortable", true);
                inner.AddAttribute(24, "SortComparison", sortComparison);
            }
            if (filterFunc is not null)
            {
                inner.AddAttribute(25, "Filterable", true);
                inner.AddAttribute(26, "FilterDebounceMs", 0);
                inner.AddAttribute(27, "FilterFunc", filterFunc);
            }
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void SortComparison_OrdersByDomainRule()
    {
        // Lexical sort would give High, Low, Medium; the custom comparison sorts by priority rank.
        var cut = Render(Grid(sortComparison: (a, b) => Rank(a.Priority).CompareTo(Rank(b.Priority))));
        cut.FindAll($"th.{Css.Classes.DataGrid.ThSortable}").First(t => t.TextContent.Contains("Priority")).Click();

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal("B", rows[0].QuerySelectorAll("td")[0].TextContent.Trim()); // High
        Assert.Equal("C", rows[1].QuerySelectorAll("td")[0].TextContent.Trim()); // Medium
        Assert.Equal("A", rows[2].QuerySelectorAll("td")[0].TextContent.Trim()); // Low
    }

    [Fact]
    public void FilterFunc_AppliesCustomPredicate()
    {
        // Custom predicate: keep tickets whose priority rank is <= the typed number.
        var cut = Render(Grid(filterFunc: (t, text) => int.TryParse(text, out var max) && Rank(t.Priority) <= max));
        cut.Find($".{Css.Classes.DataGrid.FilterRow} .{Css.Classes.Input.Control}").Input("0"); // only High

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Single(rows);
        Assert.Contains("B", rows[0].TextContent);
    }
}

// ------------------------------------------------------------------------------
// Type-aware cell rendering + auto-detection (ColumnDataType / DataGridValueFormatter)
// ------------------------------------------------------------------------------
