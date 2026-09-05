using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridFrozenRightTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data = [new("A", 1), new("B", 2)];

    [Fact]
    public void FrozenRightColumn_GetsFrozenRightClass_OnHeaderAndCells()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
                inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "Value");
                inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Value));
                inner.AddAttribute(23, "FrozenRight", true); inner.CloseComponent();
            })));

        Assert.Single(cut.FindAll($"th.{Css.Classes.DataGrid.ThFrozenRight}"));
        Assert.Equal(2, cut.FindAll($"td.{Css.Classes.DataGrid.TdFrozenRight}").Count); // one per data row
        // The table opts into horizontal-scroll layout when any column is frozen (left or right).
        Assert.NotEmpty(cut.FindAll($"table.{Css.Classes.DataGrid.TableScrollX}"));
    }
}
