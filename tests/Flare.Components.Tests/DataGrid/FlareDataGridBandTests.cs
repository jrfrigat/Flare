using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridBandTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void AddColumn(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        b.CloseComponent();
    }

    // Header tree: Name | [Details: Role, [Location: City]] | Score
    private static RenderFragment BandedGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            AddColumn(cols, 0, "Name", p => p.Name);
            cols.OpenComponent<FlareColumnBand>(10);
            cols.AddAttribute(11, "Title", "Details");
            cols.AddAttribute(12, "ChildContent", (RenderFragment)(d =>
            {
                AddColumn(d, 0, "Role", p => p.Role);
                d.OpenComponent<FlareColumnBand>(10);
                d.AddAttribute(11, "Title", "Location");
                d.AddAttribute(12, "ChildContent", (RenderFragment)(l => AddColumn(l, 0, "City", p => p.City)));
                d.CloseComponent();
            }));
            cols.CloseComponent();
            AddColumn(cols, 20, "Score", p => p.Score);
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Bands_RenderTitleCellsWithColspan()
    {
        var cut = Render(BandedGrid());

        var bandCells = cut.FindAll($"th.{Css.Classes.DataGrid.ThBand}");
        Assert.Contains(bandCells, th => th.TextContent.Trim() == "Details");
        Assert.Contains(bandCells, th => th.TextContent.Trim() == "Location");

        // Details spans the two leaves under it (Role + City).
        var details = bandCells.First(th => th.TextContent.Trim() == "Details");
        Assert.Equal("2", details.GetAttribute("colspan"));
    }

    [Fact]
    public void Bands_ProduceThreeHeaderRows()
    {
        var cut = Render(BandedGrid());
        // Nesting depth 2 -> 2 band rows + 1 leaf row = 3 header rows.
        Assert.Equal(3, cut.FindAll("thead tr").Count);
    }

    [Fact]
    public void FreeColumn_SpansFullHeaderHeight()
    {
        var cut = Render(BandedGrid());
        var nameTh = cut.FindAll("th[role=columnheader]").First(t => t.TextContent.Contains("Name"));
        Assert.Equal("3", nameTh.GetAttribute("rowspan"));
    }

    [Fact]
    public void BandedColumns_StillRenderAlignedDataCells()
    {
        var cut = Render(BandedGrid());
        // Four leaf columns -> four data cells per row, in declaration order.
        var firstRowCells = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll($"td.{Css.Classes.DataGrid.Td}");
        Assert.Equal(4, firstRowCells.Length);
        Assert.Equal("Alice", firstRowCells[0].TextContent.Trim());
        Assert.Equal("Eng", firstRowCells[1].TextContent.Trim());
        Assert.Equal("Berlin", firstRowCells[2].TextContent.Trim());
        Assert.Equal("90", firstRowCells[3].TextContent.Trim());
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid composite columns (stacked fields in one cell)
// ------------------------------------------------------------------------------
