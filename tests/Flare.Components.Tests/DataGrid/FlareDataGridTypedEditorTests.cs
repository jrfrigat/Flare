using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridTypedEditorTests : FlareTestContext
{
    private enum Status { Open, Closed }
    private record Row(string Name, bool Active, int Score, Status State);

    private static readonly Row[] _rows =
    [
        new("Alice", true, 10, Status.Open),
        new("Bob", false, 20, Status.Closed),
    ];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        void Col(string title, Func<Row, object?> get)
        {
            inner.OpenComponent<FlareColumn<Row>>(s++);
            inner.AddAttribute(s++, "Title", title);
            inner.AddAttribute(s++, "Field", get);
            inner.AddAttribute(s++, "Editable", true);
            inner.CloseComponent();
        }
        Col("Name", r => r.Name);
        Col("Active", r => r.Active);
        Col("Score", r => r.Score);
        Col("State", r => r.State);
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderEditing()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.EditMode, DataGridEditMode.Inline)
            .Add(x => x.Columns, Cols()));
        // Begin editing the first row via its edit action button.
        cut.FindAll($"td.{Css.Classes.DataGrid.TdEditActions} button")[0].Click();
        return cut;
    }

    [Fact]
    public void BoolColumn_EditsWithCheckbox()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.NotEmpty(editRow.QuerySelectorAll($".{Css.Classes.Checkbox.Root}"));
    }

    [Fact]
    public void EnumColumn_EditsWithSelect()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.NotEmpty(editRow.QuerySelectorAll($".{Css.Classes.Select.Root}"));
    }

    [Fact]
    public void NumberColumn_EditsWithNumberInput()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0];
        var numberInputs = editRow.QuerySelectorAll("input[type=number]");
        Assert.NotEmpty(numberInputs);
    }

    [Fact]
    public void NumberSeed_IsInvariant()
    {
        var cut = RenderEditing();
        var values = cut.Instance.GetEditValues();
        Assert.Equal("10", values["Score"]);   // invariant
        Assert.Equal("True", values["Active"]); // bool round-trips
    }
}
