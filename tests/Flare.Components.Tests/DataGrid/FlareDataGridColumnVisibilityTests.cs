using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridColumnVisibilityTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "QA")];

    private static RenderFragment Cols() => cols =>
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
    };

    [Fact]
    public void ShowColumnPicker_RendersToolbarButton()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.ShowColumnPicker, true)
            .Add(g => g.Columns, Cols()));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.ColumnPickerWrap}"));
    }

    [Fact]
    public void ColumnPicker_TogglesColumnVisibilityByKey()
    {
        HashSet<string>? reported = null;
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.ShowColumnPicker, true)
            .Add(g => g.HiddenColumnsChanged, (IReadOnlyCollection<string> h) => reported = [.. h])
            .Add(g => g.Columns, Cols()));

        Assert.Equal(2, cut.FindAll("thead th[role='columnheader']").Count);

        cut.Find($".{Css.Classes.DataGrid.ColumnPickerWrap} button").Click(); // open the picker
        cut.FindAll($".{Css.Classes.DataGrid.ColumnPicker} input[type=checkbox]")[1].Change(false); // hide Department

        Assert.Single(cut.FindAll("thead th[role='columnheader']"));
        Assert.NotNull(reported);
        Assert.Contains("dept", reported!); // tracked by key, not title
    }
}

// ------------------------------------------------------------------------------
// FlareColumn per-column strategies: custom SortComparison + FilterFunc
// ------------------------------------------------------------------------------
