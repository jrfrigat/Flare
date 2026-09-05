using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridBandedSortTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                comp.OpenComponent<FlareColumnRow>(0);
                comp.AddAttribute(1, "ChildContent", (RenderFragment)(r =>
                {
                    r.OpenComponent<FlareColumn<Person>>(0);
                    r.AddAttribute(1, "Title", "Name");
                    r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                    r.CloseComponent();
                    r.OpenComponent<FlareColumn<Person>>(10);
                    r.AddAttribute(11, "Title", "City");
                    r.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.City));
                    r.AddAttribute(13, "Sortable", true);
                    r.AddAttribute(14, "Filterable", true);
                    r.AddAttribute(15, "FilterDebounceMs", 0); // instant for the test
                    r.CloseComponent();
                }));
                comp.CloseComponent();
            }));
            cols.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void SortableCompositeField_SortsRecordsAndShowsIndicator()
    {
        var cut = Render(Grid());
        var cityTh = cut.FindAll($"th.{Css.Classes.DataGrid.ThComposite}").First(t => t.TextContent.Contains("City"));
        Assert.Contains(Css.Classes.DataGrid.ThSortable, cityTh.ClassName);

        cityTh.Click(); // ascending by City: Berlin (Alice), Paris (Bob)
        var firstAsc = cut.FindAll($"tr.{Css.Classes.DataGrid.RecordRow}")[0]
            .QuerySelectorAll($"td.{Css.Classes.DataGrid.TdComposite}")[0].TextContent.Trim();
        Assert.Equal("Alice", firstAsc);

        cut.FindAll($"th.{Css.Classes.DataGrid.ThComposite}").First(t => t.TextContent.Contains("City")).Click(); // descending
        var firstDesc = cut.FindAll($"tr.{Css.Classes.DataGrid.RecordRow}")[0]
            .QuerySelectorAll($"td.{Css.Classes.DataGrid.TdComposite}")[0].TextContent.Trim();
        Assert.Equal("Bob", firstDesc);

        Assert.NotEmpty(cut.FindAll($"th.{Css.Classes.DataGrid.ThComposite} .{Css.Classes.DataGrid.SortIcon}"));
    }

    [Fact]
    public void FilterableCompositeField_FiltersRecordsByFieldValue()
    {
        var cut = Render(Grid());
        // Both records visible initially.
        Assert.Equal(2, cut.FindAll($"tr.{Css.Classes.DataGrid.RecordFirst}").Count);

        // The City sub-header hosts an inline filter input.
        var cityTh = cut.FindAll($"th.{Css.Classes.DataGrid.ThComposite}").First(t => t.TextContent.Contains("City"));
        var input = cityTh.QuerySelector($".{Css.Classes.DataGrid.CompositeFilter} input");
        Assert.NotNull(input);

        input!.Input("Berlin"); // keep only Alice (Berlin)
        var records = cut.FindAll($"tr.{Css.Classes.DataGrid.RecordFirst}");
        Assert.Single(records);
        Assert.Contains("Alice", records[0].TextContent);
        Assert.DoesNotContain("Bob", cut.Markup);
    }

    [Fact]
    public void Virtual_BandedComposite_RendersWithoutThrowing()
    {
        // Each record spans several <tr>; Virtualize must take a positive per-record ItemSize.
        var cut = Render(b =>
        {
            b.OpenComponent<FlareDataGrid<Person>>(0);
            b.AddAttribute(1, "Items", _people.AsEnumerable());
            b.AddAttribute(2, "Virtual", true);
            b.AddAttribute(3, "Height", "300px");
            b.AddAttribute(4, "Columns", (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Employee");
                cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
                {
                    comp.OpenComponent<FlareColumnRow>(0);
                    comp.AddAttribute(1, "ChildContent", (RenderFragment)(r =>
                    {
                        r.OpenComponent<FlareColumn<Person>>(0);
                        r.AddAttribute(1, "Title", "Name");
                        r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                        r.CloseComponent();
                    }));
                    comp.CloseComponent();
                    comp.OpenComponent<FlareColumnRow>(10);
                    comp.AddAttribute(11, "ChildContent", (RenderFragment)(r =>
                    {
                        r.OpenComponent<FlareColumn<Person>>(0);
                        r.AddAttribute(1, "Title", "City");
                        r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.City));
                        r.CloseComponent();
                    }));
                    comp.CloseComponent();
                }));
                cols.CloseComponent();
            }));
            b.CloseComponent();
        });

        // Height caps the component so the table container has something to scroll in, and the banded
        // header still renders inside it.
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Bounded}"));
        Assert.NotEmpty(cut.FindAll($"th.{Css.Classes.DataGrid.ThComposite}"));
    }
}

// ------------------------------------------------------------------------------
// FlareColumn stable identity (Key = Id ?? SortKey ?? Title)
// ------------------------------------------------------------------------------
