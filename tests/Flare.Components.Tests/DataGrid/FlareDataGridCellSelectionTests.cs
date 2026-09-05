using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridCellSelectionTests : FlareTestContext
{
    private sealed class CapturingClipboard : IFlareClipboard
    {
        public string? Text;
        public string ReadText = string.Empty;
        public ValueTask CopyAsync(string text) { Text = text; return default; }
        public ValueTask<string> ReadAsync() => new(ReadText);
    }

    private record Row(string Name, int A, int B);
    private static readonly Row[] _rows = [new("r1", 1, 2), new("r2", 3, 4), new("r3", 5, 6)];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        foreach (var (title, get) in new (string, Func<Row, object?>)[]
            { ("Name", r => r.Name), ("A", r => r.A), ("B", r => r.B) })
        {
            inner.OpenComponent<FlareColumn<Row>>(s++);
            inner.AddAttribute(s++, "Title", title);
            inner.AddAttribute(s++, "Field", get);
            inner.CloseComponent();
        }
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid(CapturingClipboard? clip = null)
    {
        if (clip is not null) Services.AddScoped<IFlareClipboard>(_ => clip);
        return Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.CellSelection, true)
            .Add(x => x.Columns, Cols()));
    }

    private static void Key(IRenderedComponent<FlareDataGrid<Row>> cut, string key, bool shift = false, bool ctrl = false)
        => cut.Find($".{Css.Classes.DataGrid.Table}").KeyDown(new KeyboardEventArgs { Key = key, ShiftKey = shift, CtrlKey = ctrl });

    [Fact]
    public void ShiftArrow_ExtendsRange_HighlightsCells()
    {
        var cut = RenderGrid();
        Key(cut, "ArrowDown");                 // init active cell (0,0)
        Key(cut, "ArrowRight", shift: true);   // extend to (0,1)
        Key(cut, "ArrowDown", shift: true);    // extend to (1,1) -> 2x2 block

        // 4 body cells in the 2x2 block are highlighted.
        Assert.Equal(4, cut.FindAll($"td.{Css.Classes.DataGrid.CellRange}").Count);
    }

    [Fact]
    public void CtrlC_CopiesRangeAsTsv()
    {
        var clip = new CapturingClipboard();
        var cut = RenderGrid(clip);
        Key(cut, "ArrowDown");                 // (0,0)
        Key(cut, "ArrowRight", shift: true);   // -> (0,1): columns Name, A
        Key(cut, "ArrowDown", shift: true);    // -> (1,1): rows r1, r2
        Key(cut, "c", ctrl: true);

        Assert.Equal("r1\t1\nr2\t3", clip.Text);
    }

    [Fact]
    public void PlainArrow_CollapsesRange()
    {
        var cut = RenderGrid();
        Key(cut, "ArrowDown");                 // (0,0)
        Key(cut, "ArrowRight", shift: true);   // range (0,0)-(0,1)
        Key(cut, "ArrowDown");                 // plain move collapses to single cell (1,1)

        Assert.Single(cut.FindAll($"td.{Css.Classes.DataGrid.CellRange}"));
    }

    [Fact]
    public void MouseDrag_SelectsRectangularRange()
    {
        var cut = RenderGrid();
        cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[0].QuerySelectorAll("td")[0]
            .TriggerEvent("onmousedown", new MouseEventArgs()); // start at (0,0)
        // Re-query after the re-render so the handler IDs are current.
        cut.FindAll($"tr.{Css.Classes.DataGrid.Row}")[1].QuerySelectorAll("td")[1]
            .TriggerEvent("onmouseenter", new MouseEventArgs()); // drag to (1,1)

        Assert.Equal(4, cut.FindAll($"td.{Css.Classes.DataGrid.CellRange}").Count); // 2x2 block
    }

    [Fact]
    public async Task CtrlV_PastesTsv_AndRaisesOnPaste()
    {
        var clip = new CapturingClipboard { ReadText = "x\ty\nz\tw" };
        Services.AddScoped<IFlareClipboard>(_ => clip);
        DataGridPaste<Row>? received = null;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.CellSelection, true)
            .Add(x => x.OnPaste, EventCallback.Factory.Create<DataGridPaste<Row>>(this, dp => received = dp))
            .Add(x => x.Columns, Cols()));

        cut.Find($".{Css.Classes.DataGrid.Table}").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" }); // active cell (0,0)
        await cut.Find($".{Css.Classes.DataGrid.Table}").KeyDownAsync(new KeyboardEventArgs { Key = "v", CtrlKey = true });

        Assert.NotNull(received);
        Assert.Equal(4, received!.Cells.Count);
        // Pasted block maps onto Name (col 0) and A (col 1) of rows r1, r2.
        Assert.Contains(received.Cells, c => c.ColumnKey == "Name" && c.Value == "x");
        Assert.Contains(received.Cells, c => c.ColumnKey == "A" && c.Value == "y");
        Assert.Contains(received.Cells, c => c.ColumnKey == "A" && c.Value == "w");
    }
}
