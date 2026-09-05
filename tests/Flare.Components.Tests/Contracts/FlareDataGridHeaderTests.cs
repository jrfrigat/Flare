using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridHeaderTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "Mkt")];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(5, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            inner.CloseComponent();
            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    private static RenderFragment ReorderGrid(
        bool reorderableColumns = false,
        IReadOnlyList<string>? columnOrder = null,
        Action<IReadOnlyList<string>>? onColumnOrderChanged = null,
        bool rowReorderable = false,
        Action<DataGridRowReorder<Person>>? onRowReordered = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "ReorderableColumns", reorderableColumns);
        b.AddAttribute(3, "RowReorderable", rowReorderable);
        if (columnOrder is not null) b.AddAttribute(4, "ColumnOrder", columnOrder);
        if (onColumnOrderChanged is not null)
            b.AddAttribute(5, "OnColumnOrderChanged",
                EventCallback.Factory.Create(onColumnOrderChanged.Target!, onColumnOrderChanged));
        if (onRowReordered is not null)
            b.AddAttribute(6, "OnRowReordered",
                EventCallback.Factory.Create(onRowReordered.Target!, onRowReordered));
        b.AddAttribute(7, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            inner.CloseComponent();
            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Default_HeaderOrder_MatchesDeclaration()
    {
        var cut = Render(Grid());
        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Name", "Dept"], titles);
    }

    [Fact]
    public void RowAndColumnFuncs_ApplyInDefaultRenderPath()
    {
        // Regression: the default (non-virtualized, non-grouped) path must apply both the row-level
        // RowClassFunc/RowStyleFunc and the column-level ClassFunc/StyleFunc.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(x => x.Items, _people.AsEnumerable())
            .Add(x => x.RowClassFunc, item => item.Name == "Alice" ? "row-hi" : "")
            .Add(x => x.RowStyleFunc, item => item.Name == "Alice" ? "background:red" : "")
            .Add(x => x.Columns, inner =>
            {
                inner.OpenComponent<FlareColumn<Person>>(0);
                inner.AddAttribute(1, "Title", "Name");
                inner.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                inner.AddAttribute(3, "ClassFunc", (Func<Person, string>)(p => p.Name == "Alice" ? "cell-hi" : ""));
                inner.AddAttribute(4, "StyleFunc", (Func<Person, string>)(p => p.Name == "Alice" ? "font-weight:bold" : ""));
                inner.CloseComponent();
            }));

        var aliceRow = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").First(r => r.TextContent.Contains("Alice"));
        Assert.Contains("row-hi", aliceRow.ClassName);
        Assert.Contains("background:red", aliceRow.GetAttribute("style") ?? "");

        var aliceCell = aliceRow.QuerySelector($"td.{Css.Classes.DataGrid.Td}")!;
        Assert.Contains("cell-hi", aliceCell.ClassName);
        Assert.Contains("font-weight:bold", aliceCell.GetAttribute("style") ?? "");
    }

    [Fact]
    public void ReorderableColumns_AddsDraggableMarkup()
    {
        var cut = Render(ReorderGrid(reorderableColumns: true));
        var th = cut.FindAll("th[role=columnheader]")[0];
        Assert.Equal("c:Name", th.GetAttribute("data-flare-drag"));
        Assert.Equal("flare-datagrid-columns", th.GetAttribute("data-flare-drag-group"));
        Assert.Contains(Css.Classes.Drag.Item, th.ClassName);
    }

    [Fact]
    public void Columns_NotReorderable_HaveNoDraggableAttribute()
    {
        var cut = Render(ReorderGrid(reorderableColumns: false));
        var th = cut.FindAll("th[role=columnheader]")[0];
        Assert.Null(th.GetAttribute("data-flare-drag"));
    }

    [Fact]
    public void ColumnOrder_Param_ReordersHeaders()
    {
        var cut = Render(ReorderGrid(columnOrder: ["Dept", "Name"]));
        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Dept", "Name"], titles);
    }

    [Fact]
    public void DragDropColumn_ReordersAndRaisesCallback()
    {
        IReadOnlyList<string>? reported = null;
        var cut = Render(ReorderGrid(reorderableColumns: true, onColumnOrderChanged: o => reported = o));

        // Drag "Dept" onto the front of the header row. The browser reports the index the column ends
        // up at, counted with the dragged one already taken out.
        var drag = cut.FindComponent<FlareDragContext<object>>();
        cut.InvokeAsync(() => drag.Instance.OnDropAsync("c:Dept", "flare-datagrid-columns", 0, "before", "c:Name")).Wait();

        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Dept", "Name"], titles);
        Assert.NotNull(reported);
        Assert.Equal(["Dept", "Name"], reported!);
    }

    [Fact]
    public void RowReorderable_AddsDraggableMarkup()
    {
        var cut = Render(ReorderGrid(rowReorderable: true));
        var row = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.Equal("0", row.GetAttribute("data-flare-drag"));
        Assert.Equal("flare-datagrid-rows", row.GetAttribute("data-flare-drag-group"));
        Assert.Contains(Css.Classes.Drag.Item, row.ClassName);
    }

    [Fact]
    public void DragDropRow_RaisesOnRowReorderedWithIndices()
    {
        DataGridRowReorder<Person>? reported = null;
        var cut = Render(ReorderGrid(rowReorderable: true, onRowReordered: e => reported = e));

        // Drag row 1 (Bob) in front of row 0 (Alice).
        var drag = cut.FindComponent<FlareDragContext<object>>();
        cut.InvokeAsync(() => drag.Instance.OnDropAsync("1", "flare-datagrid-rows", 0, "before", "0")).Wait();

        Assert.NotNull(reported);
        Assert.Equal(1, reported!.OldIndex);
        Assert.Equal(0, reported.NewIndex);
        Assert.Equal("Bob", reported.Item.Name);
        Assert.Equal("Alice", reported.Target.Name);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid column bands (grouped headers via <FlareColumnBand>)
// ------------------------------------------------------------------------------
