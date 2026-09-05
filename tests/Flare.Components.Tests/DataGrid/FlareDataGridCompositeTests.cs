using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridCompositeTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void Field(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field, int colSpan = 1)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        if (colSpan != 1) b.AddAttribute(seq + 3, "ColSpan", colSpan);
        b.CloseComponent();
    }

    private static void Row(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, RenderFragment content)
    {
        b.OpenComponent<FlareColumnRow>(seq);
        b.AddAttribute(seq + 1, "ChildContent", content);
        b.CloseComponent();
    }

    // Columns: [Employee composite: (Name, City) / (Role span2)] | Score
    private static RenderFragment CompositeGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(3, "CompositeMode", CompositeMode.Card);
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                Row(comp, 0, r =>
                {
                    Field(r, 0, "Name", p => p.Name);
                    Field(r, 10, "City", p => p.City);
                });
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
    public void Composite_RendersOneCellGridPerRow()
    {
        var cut = Render(CompositeGrid());
        // Two data rows -> two composite containers.
        Assert.Equal(2, cut.FindAll($".{Css.Classes.DataGrid.Composite}").Count);
        // The composite host plus the normal Score column = exactly two td cells per data row.
        var cells = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll($"td.{Css.Classes.DataGrid.Td}");
        Assert.Equal(2, cells.Length);
    }

    [Fact]
    public void Composite_RendersAllFieldValuesAndLabels()
    {
        var cut = Render(CompositeGrid());
        var firstComposite = cut.FindAll($".{Css.Classes.DataGrid.Composite}")[0];

        var values = firstComposite.QuerySelectorAll($".{Css.Classes.DataGrid.CompositeValue}").Select(v => v.TextContent.Trim()).ToList();
        Assert.Contains("Alice", values);
        Assert.Contains("Berlin", values);
        Assert.Contains("Eng", values);

        var labels = firstComposite.QuerySelectorAll($".{Css.Classes.DataGrid.CompositeLabel}").Select(l => l.TextContent.Trim()).ToList();
        Assert.Contains("Name", labels);
        Assert.Contains("Role", labels);
    }

    [Fact]
    public void Composite_FieldColSpan_AppliesGridColumnSpan()
    {
        var cut = Render(CompositeGrid());
        var roleField = cut.FindAll($".{Css.Classes.DataGrid.CompositeField}")
            .First(f => f.TextContent.Contains("Role"));
        Assert.Contains("span 2", roleField.GetAttribute("style") ?? "");
    }

    [Fact]
    public void CompositeFields_DoNotBecomeGridColumns()
    {
        var cut = Render(CompositeGrid());
        // Only the two top-level columns (Employee, Score) are real column headers.
        Assert.Equal(2, cut.FindAll("th[role=columnheader]").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid banded composite (records span several rows, DevExpress-style)
// ------------------------------------------------------------------------------
