namespace Flare.Components.Tests;

/// <summary>
/// The board was the only one of the four drag surfaces that worked on a touch screen, and it worked
/// because it carried a second, hand-written implementation next to the HTML5 one: an
/// ontouchstart/ontouchend pair and a JS hit-test helper nothing else used. Both are gone - it declares
/// the shared model now, and gains reordering WITHIN a column, which the old drop-into-a-column
/// handler could not express.
/// </summary>
public sealed class KanbanDragTests : FlareTestContext
{
    private static readonly KanbanColumn[] _columns =
    [
        new("todo", "To do"),
        new("done", "Done"),
    ];

    private static List<KanbanCard> Cards() =>
    [
        new("a", "todo", "A"),
        new("b", "todo", "B"),
        new("c", "done", "C"),
    ];

    private IRenderedComponent<FlareKanban> RenderBoard(Action<IReadOnlyList<KanbanCard>> changed) =>
        Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, Cards())
            .Add(x => x.CardsChanged, changed));

    [Fact]
    public void EveryColumnIsADropZoneAndEveryCardIsDraggable()
    {
        var cut = RenderBoard(_ => { });

        var zones = cut.FindAll("[data-flare-drop]");
        Assert.Equal(2, zones.Count);
        Assert.Equal("todo", zones[0].GetAttribute("data-flare-drop"));
        Assert.Contains("flare-kanban__column", zones[0].ClassName);
        Assert.Equal("kanban", zones[0].GetAttribute("data-flare-drag-group"));

        var cards = cut.FindAll("[data-flare-drag]");
        Assert.Equal(3, cards.Count);
        Assert.Contains("flare-kanban__card", cards[0].ClassName);
    }

    // The old markup had draggable="true" plus ondragstart/ondrop, which is the API that fires nothing
    // at all on a phone. Nothing may put it back.
    [Fact]
    public void NoHtml5DragAttributesRemain()
    {
        var cut = RenderBoard(_ => { });
        Assert.Empty(cut.FindAll("[draggable]"));
    }

    [Fact]
    public async Task ACardDroppedInAnotherColumnChangesColumn()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("a", "done", 0, "before", "c"));

        Assert.NotNull(changed);
        Assert.Equal("done", changed!.Single(c => c.Id == "a").ColumnId);
    }

    // Reordering inside one column is new: the board used to filter a flat list by column id and had
    // nowhere to put a position, so a drop within a column did nothing at all.
    [Fact]
    public async Task ACardCanBeReorderedWithinItsOwnColumn()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("b", "todo", 0, "before", "a"));

        var todo = changed!.Where(c => c.ColumnId == "todo").Select(c => c.Id).ToArray();
        Assert.Equal(["b", "a"], todo);
    }

    // The index is reported without the dragged card, so appending is "the length of what is left"
    // rather than an off-by-one the caller has to reason about.
    [Fact]
    public async Task ACardDroppedPastTheLastOneGoesLast()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("a", "done", 1, "after", "c"));

        var done = changed!.Where(c => c.ColumnId == "done").Select(c => c.Id).ToArray();
        Assert.Equal(["c", "a"], done);
    }
}
