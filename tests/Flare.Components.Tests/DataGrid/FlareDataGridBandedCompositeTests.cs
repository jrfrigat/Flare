using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridBandedCompositeTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void Field(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field, int colSpan = 1, bool sortable = false)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        if (colSpan != 1) b.AddAttribute(seq + 3, "ColSpan", colSpan);
        if (sortable) b.AddAttribute(seq + 4, "Sortable", true);
        b.CloseComponent();
    }

    private static void Row(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, RenderFragment content)
    {
        b.OpenComponent<FlareColumnRow>(seq);
        b.AddAttribute(seq + 1, "ChildContent", content);
        b.CloseComponent();
    }

    // Columns: [Employee banded: (Name, City sortable) / (Role span2)] | Score (plain)
    private static RenderFragment BandedGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                Row(comp, 0, r => { Field(r, 0, "Name", p => p.Name); Field(r, 10, "City", p => p.City, sortable: true); });
                Row(comp, 10, r => Field(r, 0, "Role", p => p.Role, colSpan: 2));
            }));
            cols.CloseComponent();
            cols.OpenComponent<FlareColumn<Person>>(20);
            cols.AddAttribute(21, "Title", "Score");
            cols.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Score));
            cols.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Banded_HeaderHasOneRowPerRecordRow()
    {
        var cut = Render(BandedGrid());
        // Composite has 2 rows -> 2 header rows.
        Assert.Equal(2, cut.FindAll("thead tr").Count);
    }

    [Fact]
    public void Banded_PlainColumnSpansAllRecordRows()
    {
        var cut = Render(BandedGrid());
        var scoreTh = cut.FindAll("th[role=columnheader]").First(t => t.TextContent.Contains("Score"));
        Assert.Equal("2", scoreTh.GetAttribute("rowspan"));
    }

    [Fact]
    public void Banded_EachRecordSpansTwoBodyRows()
    {
        var cut = Render(BandedGrid());
        // 2 records x 2 rows = 4 record rows.
        Assert.Equal(4, cut.FindAll($"tr.{Css.Classes.DataGrid.RecordRow}").Count);
    }

    [Fact]
    public void Banded_RendersFieldValuesAndColSpan()
    {
        var cut = Render(BandedGrid());
        var firstRow = cut.FindAll($"tr.{Css.Classes.DataGrid.RecordRow}")[0];
        var composites = firstRow.QuerySelectorAll($"td.{Css.Classes.DataGrid.TdComposite}");
        var texts = composites.Select(c => c.TextContent.Trim()).ToList();
        Assert.Contains("Alice", texts);
        Assert.Contains("Berlin", texts);

        // The Role cell on the second record-row spans 2 columns.
        var secondRow = cut.FindAll($"tr.{Css.Classes.DataGrid.RecordRow}")[1];
        var roleCell = secondRow.QuerySelectorAll($"td.{Css.Classes.DataGrid.TdComposite}")[0];
        Assert.Equal("Eng", roleCell.TextContent.Trim());
        Assert.Equal("2", roleCell.GetAttribute("colspan"));
    }
}
