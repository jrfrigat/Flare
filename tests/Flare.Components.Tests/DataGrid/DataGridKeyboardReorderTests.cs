using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A grid row's plain Space, Enter and arrows already select and navigate, so its reorder gesture is
/// the Alt chord: Alt+Space picks the focused row up and puts it down, Alt+ArrowUp/Down moves it.
/// </summary>
public class DataGridKeyboardReorderTests : FlareTestContext
{
    private record Row(string Name);

    private static readonly Row[] Rows = [new("Alpha"), new("Beta"), new("Gamma")];

    private static KeyboardEventArgs Key(string key, bool alt = false) => new() { Key = key, AltKey = alt };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid(Action<DataGridRowReorder<Row>> onReorder) =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows.AsEnumerable())
            .Add(x => x.RowReorderable, true)
            .Add(x => x.OnRowReordered, onReorder)
            .Add(x => x.Columns, (RenderFragment)(columns =>
            {
                columns.OpenComponent<FlareColumn<Row>>(0);
                columns.AddAttribute(1, "Title", "Name");
                columns.AddAttribute(2, "Field", (Func<Row, object?>)(row => row.Name));
                columns.CloseComponent();
            })));

    private static void Press(IRenderedComponent<FlareDataGrid<Row>> cut, KeyboardEventArgs args) =>
        cut.Find("table").KeyDown(args);

    [Fact]
    public void AltSpacePicksUpAndAltArrowMovesTheFocusedRow()
    {
        DataGridRowReorder<Row>? moved = null;
        var cut = RenderGrid(reorder => moved = reorder);

        Press(cut, Key("ArrowDown")); // gives the grid an active row
        Press(cut, Key(" ", alt: true)); // pick up Alpha
        Press(cut, Key("ArrowDown", alt: true)); // move it below Beta

        Assert.NotNull(moved);
        Assert.Equal("Alpha", moved!.Item.Name);
        Assert.Equal("Beta", moved.Target.Name);
        Assert.Equal(0, moved.OldIndex);
        Assert.Equal(1, moved.NewIndex);

        Press(cut, Key(" ", alt: true)); // put it down
    }

    [Fact]
    public void PlainSpaceDoesNotReorder()
    {
        var moves = 0;
        var cut = RenderGrid(_ => moves++);

        Press(cut, Key("ArrowDown"));
        Press(cut, Key(" "));

        Assert.Equal(0, moves);
    }

    [Fact]
    public void EscapeStopsTheMoveWithoutFurtherCallbacks()
    {
        var moves = 0;
        var cut = RenderGrid(_ => moves++);

        Press(cut, Key("ArrowDown"));
        Press(cut, Key(" ", alt: true));
        Press(cut, Key("Escape", alt: true));
        Press(cut, Key("ArrowDown", alt: true));

        Assert.Equal(0, moves);
    }
}
