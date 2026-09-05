using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridColumnIdentityTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "QA")];

    [Fact]
    public void ColumnState_IsKeyedByKey_NotTitle()
    {
        // A column with a SortKey has Key != Title; the grid's column state must use the Key.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.Columns, (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Name");
                cols.AddAttribute(2, "Field", (Func<Person, object?>)(x => x.Name));
                cols.CloseComponent();
                cols.OpenComponent<FlareColumn<Person>>(10);
                cols.AddAttribute(11, "Title", "Department");
                cols.AddAttribute(12, "Field", (Func<Person, object?>)(x => x.Department));
                cols.AddAttribute(13, "SortKey", "dept");
                cols.CloseComponent();
            })));

        var order = cut.Instance.CurrentState.ColumnOrder;
        Assert.Contains("dept", order);            // the key, not the display title
        Assert.DoesNotContain("Department", order);
    }

    [Fact]
    public void DuplicateTitles_DisambiguatedById_SortIndependently()
    {
        // Two columns share a title; Id keeps their sort state distinct.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.Columns, (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Value");
                cols.AddAttribute(2, "Id", "name");
                cols.AddAttribute(3, "Field", (Func<Person, object?>)(x => x.Name));
                cols.AddAttribute(4, "Sortable", true);
                cols.CloseComponent();
                cols.OpenComponent<FlareColumn<Person>>(10);
                cols.AddAttribute(11, "Title", "Value");
                cols.AddAttribute(12, "Id", "dept");
                cols.AddAttribute(13, "Field", (Func<Person, object?>)(x => x.Department));
                cols.AddAttribute(14, "Sortable", true);
                cols.CloseComponent();
            })));

        // Click the first "Value" header; only it gets a sort indicator (state keyed by Id "name").
        cut.FindAll($"th.{Css.Classes.DataGrid.ThSortable}")[0].Click();
        var sortIcons = cut.FindAll($"th .{Css.Classes.DataGrid.SortIcon}");
        Assert.Single(sortIcons);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid interactive column visibility (ShowColumnPicker)
// ------------------------------------------------------------------------------
