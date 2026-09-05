using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridTypedCellTests : FlareTestContext
{
    private record Row(string Name, bool Active, DateTime Created, decimal Amount);

    private static readonly Row[] _rows =
    [
        new("Alice", true, new DateTime(2026, 6, 21, 9, 30, 0), 1234.5m),
        new("Bob", false, new DateTime(2026, 1, 2, 0, 0, 0), 7.25m),
    ];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Active");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Active));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    // The cell is an inline SVG (FlareIconView), not an icon-font ligature span, so identity is read
    // from the accessible label and the state class - never from the element's text, which is empty.
    [Fact]
    public void BoolColumn_AutoDetected_RendersCheckboxIcon()
    {
        var cut = Render(Grid());
        var icons = cut.FindAll($".{Css.Classes.DataGrid.BoolCell}");
        Assert.Equal(2, icons.Count);
        Assert.All(icons, i => Assert.Equal("svg", i.NodeName, ignoreCase: true));
        Assert.Contains(icons, i => i.GetAttribute("aria-label") == "true");   // Alice = true
        Assert.Contains(icons, i => i.GetAttribute("aria-label") == "false");  // Bob = false
    }

    [Fact]
    public void BoolColumn_True_GetsOnStateClass()
    {
        var cut = Render(Grid());
        Assert.Contains(cut.FindAll($".{Css.Classes.DataGrid.BoolOn}"), i => i.GetAttribute("aria-label") == "true");
        Assert.Contains(cut.FindAll($".{Css.Classes.DataGrid.BoolOff}"), i => i.GetAttribute("aria-label") == "false");
    }

    [Fact]
    public void DateColumn_WithType_DropsTimeComponent()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10);
                inner.AddAttribute(11, "Title", "Created");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Created));
                inner.AddAttribute(13, "Type", ColumnDataType.Date);
                inner.CloseComponent();
            })));

        var firstCell = cut.FindAll($"tr.{Css.Classes.DataGrid.Row} td")[0].TextContent.Trim();
        Assert.Equal("06/21/2026", firstCell);
        Assert.DoesNotContain(":", firstCell); // no time
    }

    [Fact]
    public void NumberColumn_WithFormat_AppliesFormatString()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10);
                inner.AddAttribute(11, "Title", "Amount");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Amount));
                inner.AddAttribute(13, "Format", "N2");
                inner.CloseComponent();
            })));

        Assert.Equal("1,234.50", cut.FindAll($"tr.{Css.Classes.DataGrid.Row} td")[0].TextContent.Trim());
    }
}
