using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridAlignmentTests : FlareTestContext
{
    private record Row(string Name, bool Active, decimal Amount);

    private static readonly Row[] _rows = [new("Alice", true, 10m), new("Bob", false, 20m)];

    // Renders Name (text), Active (bool), Amount (number); optionally overrides Amount's Align.
    private static RenderFragment Grid(ColumnAlign? amountAlign = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(20);
            inner.AddAttribute(21, "Title", "Active");
            inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Active));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(30);
            inner.AddAttribute(31, "Title", "Amount");
            inner.AddAttribute(32, "Field", (Func<Row, object?>)(r => r.Amount));
            if (amountAlign is not null) inner.AddAttribute(33, "Align", amountAlign.Value);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void NumberColumn_AutoAlignsEnd()
    {
        var cut = Render(Grid());
        var cells = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll("td");
        Assert.Contains(Css.Classes.DataGrid.AlignEnd, cells[2].ClassName);   // Amount
        Assert.DoesNotContain($"{Css.Classes.DataGrid.Root}__cell--", cells[0].ClassName); // Name (text) = leading, no class
    }

    [Fact]
    public void BoolColumn_AutoAlignsCenter()
    {
        var cut = Render(Grid());
        var cells = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll("td");
        Assert.Contains(Css.Classes.DataGrid.AlignCenter, cells[1].ClassName); // Active
    }

    [Fact]
    public void Header_MatchesCellAlignment()
    {
        var cut = Render(Grid());
        var ths = cut.FindAll($"th.{Css.Classes.DataGrid.Th}");
        Assert.Contains(Css.Classes.DataGrid.AlignCenter, ths[1].ClassName); // Active header
        Assert.Contains(Css.Classes.DataGrid.AlignEnd, ths[2].ClassName);    // Amount header
    }

    [Fact]
    public void ExplicitAlign_OverridesTypeDefault()
    {
        // A number column forced to Start must not get the auto end-alignment.
        var cut = Render(Grid(amountAlign: ColumnAlign.Start));
        var amountCell = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll("td")[2];
        Assert.DoesNotContain(Css.Classes.DataGrid.AlignEnd, amountCell.ClassName);
    }
}
