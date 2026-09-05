using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridAdvancedTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people =
    [
        new("Alice", "Engineering"),
        new("Bob", "Marketing"),
        new("Carol", "Engineering"),
    ];

    private static RenderFragment DataGridWith(
        bool filterable = false,
        bool frozen = false,
        RenderFragment? toolbarContent = null,
        RenderFragment<Person>? rowDetailTemplate = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        if (toolbarContent != null)
            b.AddAttribute(3, "ToolbarContent", toolbarContent);
        if (rowDetailTemplate != null)
            b.AddAttribute(4, "RowDetailTemplate", rowDetailTemplate);
        b.AddAttribute(5, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            if (filterable)
            {
                inner.AddAttribute(13, "Filterable", true);
                // Apply the filter synchronously in tests (no debounce timer).
                inner.AddAttribute(15, "FilterDebounceMs", 0);
            }
            if (frozen) inner.AddAttribute(14, "Frozen", true);
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void FilterableColumns_RenderFilterRow()
    {
        // In the default Simple filter mode the filter row appears automatically
        // when at least one column is Filterable.
        var cut = Render(DataGridWith(filterable: true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.FilterRow}"));
    }

    [Fact]
    public void FilterRow_FiltersData()
    {
        var cut = Render(DataGridWith(filterable: true));

        // The text filter reuses FlareField (Immediate) in the filter row.
        cut.Find($".{Css.Classes.DataGrid.FilterRow} .{Css.Classes.Input.Control}").Input("Alice");

        var rows = cut.FindAll($".{Css.Classes.DataGrid.Row}");
        Assert.Single(rows);
        Assert.Contains("Alice", rows[0].TextContent);
    }

    [Fact]
    public void ToolbarContent_Renders()
    {
        var toolbar = (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"custom-toolbar\">Toolbar</span>"));
        var cut = Render(DataGridWith(toolbarContent: toolbar));

        Assert.NotNull(cut.Find(".custom-toolbar"));
    }

    [Fact]
    public void RowDetailTemplate_RendersToggleButton()
    {
        var detail = (RenderFragment<Person>)(p => b =>
            b.AddMarkupContent(0, $"<span class=\"detail-text\">{p.Name} details</span>"));

        var cut = Render(DataGridWith(rowDetailTemplate: detail));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.DetailBtn}"));
    }

    [Fact]
    public void FrozenColumn_AppliesFrozenClassToHeader()
    {
        var cut = Render(DataGridWith(frozen: true));

        var frozenHeaders = cut.FindAll($".{Css.Classes.DataGrid.ThFrozen}");
        Assert.NotEmpty(frozenHeaders);
    }

    [Fact]
    public void FilterableColumn_HasInputInFilterRow()
    {
        var cut = Render(DataGridWith(filterable: true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.FilterRow} .{Css.Classes.Input.Control}"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid column picker / extended  (7 tests from Wave9)
// ------------------------------------------------------------------------------
