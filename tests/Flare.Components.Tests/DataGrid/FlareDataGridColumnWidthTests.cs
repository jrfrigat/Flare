using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class FlareDataGridColumnWidthTests : FlareTestContext
{
    private record Person(string Name, string Role, string City);

    private static readonly Person[] People = [new("Alice", "Eng", "Berlin")];

    private static void AddColumn(RenderTreeBuilder b, int seq, string title, Func<Person, object?> field,
        string? width = null)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        if (width is not null) b.AddAttribute(seq + 3, "Width", width);
        b.CloseComponent();
    }

    private static RenderFragment Columns(string? name, string? role, string? city) => cols =>
    {
        AddColumn(cols, 0, "Name", p => p.Name, name);
        AddColumn(cols, 10, "Role", p => p.Role, role);
        AddColumn(cols, 20, "City", p => p.City, city);
    };

    private static RenderFragment BandedColumns(string? name, string? role, string? city) => cols =>
    {
        AddColumn(cols, 0, "Name", p => p.Name, name);
        cols.OpenComponent<FlareColumnBand>(10);
        cols.AddAttribute(11, "Title", "Details");
        cols.AddAttribute(12, "ChildContent", (RenderFragment)(details =>
        {
            AddColumn(details, 0, "Role", p => p.Role, role);
            AddColumn(details, 10, "City", p => p.City, city);
        }));
        cols.CloseComponent();
    };

    private static IReadOnlyList<string> Classes(IRenderedComponent<FlareDataGrid<Person>> cut)
        => cut.Find("table").ClassList;

    [Fact]
    public void Every_column_with_an_absolute_width_keeps_its_sum()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, Columns("96px", "60px", "120px")));

        var classes = Classes(cut);
        Assert.Contains(Css.Classes.DataGrid.TableFixed, classes);
        Assert.Contains(Css.Classes.DataGrid.TableWidthsExact, classes);
        Assert.Equal("width:calc(96px + 60px + 120px)", cut.Find("table").GetAttribute("style"));
        Assert.Equal("width:96px", cut.FindAll("thead th[role='columnheader']")[0].GetAttribute("style"));
    }

    [Fact]
    public void A_column_without_width_shares_the_rest()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, Columns("96px", "60px", null)));

        var classes = Classes(cut);
        Assert.Contains(Css.Classes.DataGrid.TableFixed, classes);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableWidthsExact, classes);
    }

    [Fact]
    public void No_widths_keep_the_automatic_layout()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, Columns(null, null, null)));

        var classes = Classes(cut);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableFixed, classes);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableWidthsExact, classes);
    }

    [Fact]
    public void A_relative_width_keeps_the_grid_filling_its_container()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, Columns("96px", "30%", "120px")));

        var classes = Classes(cut);
        Assert.Contains(Css.Classes.DataGrid.TableFixed, classes);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableWidthsExact, classes);
        Assert.Null(cut.Find("table").GetAttribute("style"));
    }

    [Fact]
    public void A_css_function_is_a_share_not_an_exact_width()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, Columns("96px", "calc(4rem + 4px)", "120px")));

        Assert.DoesNotContain(Css.Classes.DataGrid.TableWidthsExact, Classes(cut));
        Assert.Null(cut.Find("table").GetAttribute("style"));
    }

    [Fact]
    public void A_hidden_column_does_not_count_for_the_exact_widths()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.HiddenColumns, new[] { "City" })
            .Add(g => g.Columns, Columns("96px", "60px", null)));

        Assert.Contains(Css.Classes.DataGrid.TableWidthsExact, Classes(cut));
    }

    [Fact]
    public void A_banded_header_keeps_its_own_layout()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, People.AsEnumerable())
            .Add(g => g.Columns, BandedColumns("96px", "60px", "120px")));

        var classes = Classes(cut);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableFixed, classes);
        Assert.DoesNotContain(Css.Classes.DataGrid.TableWidthsExact, classes);
    }
}
