using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A grid row's plain Space, Enter and arrows already select and navigate, so its reorder gesture is
/// the Alt chord: Alt+ArrowUp/Down moves the focused row one position.
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
    public void AltArrowMovesTheFocusedRow()
    {
        var moves = new List<DataGridRowReorder<Row>>();
        var cut = RenderGrid(moves.Add);

        Press(cut, Key("ArrowDown")); // gives the grid an active row
        Press(cut, Key("ArrowDown", alt: true)); // Alpha below Beta
        Press(cut, Key("ArrowDown", alt: true)); // and on below Gamma

        Assert.Equal(2, moves.Count);
        Assert.Equal(("Alpha", "Beta", 0, 1), (moves[0].Item.Name, moves[0].Target.Name, moves[0].OldIndex, moves[0].NewIndex));
        Assert.Equal((1, 2), (moves[1].OldIndex, moves[1].NewIndex));
    }

    [Fact]
    public void AltArrowAtTheEdgeDoesNotReorder()
    {
        var moves = 0;
        var cut = RenderGrid(_ => moves++);

        Press(cut, Key("ArrowDown"));
        Press(cut, Key("ArrowUp", alt: true));

        Assert.Equal(0, moves);
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
    public void AltSpaceDoesNotStartAMove()
    {
        var moves = 0;
        var cut = RenderGrid(_ => moves++);

        Press(cut, Key("ArrowDown"));
        Press(cut, Key(" ", alt: true));
        Press(cut, Key("ArrowDown")); // plain navigation only

        Assert.Equal(0, moves);
    }
}
