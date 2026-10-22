namespace Flare.Components.Tests;

/// <summary>
/// A virtualized column renders a window of its cards, so the browser's drop index counts from wherever
/// that window starts and a card scrolled out of it has no component left to resolve. The board places
/// a drop by the card it landed beside and resolves cards from its own list.
/// </summary>
public sealed class KanbanVirtualTests : FlareTestContext
{
    private static readonly KanbanColumn[] _columns =
    [
        new("todo", "To do"),
        new("done", "Done"),
    ];

    private static List<KanbanCard> ManyCards()
    {
        var cards = new List<KanbanCard>();
        for (var i = 0; i < 500; i++) cards.Add(new($"t{i}", "todo", $"Todo {i}"));
        cards.Add(new("d0", "done", "Done 0"));
        cards.Add(new("d1", "done", "Done 1"));
        return cards;
    }

    private IRenderedComponent<FlareKanban> RenderBoard(bool virtualized, Action<IReadOnlyList<KanbanCard>> changed) =>
        Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, ManyCards())
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.CardsChanged, changed));

    private static string[] Column(IReadOnlyList<KanbanCard> cards, string column) =>
        cards.Where(c => c.ColumnId == column).Select(c => c.Id).ToArray();

    [Fact]
    public void AVirtualBoardVirtualizesEveryColumn()
    {
        var cut = RenderBoard(true, _ => { });

        Assert.Contains(Css.Classes.Kanban.Virtual, cut.Find("[role=group]").ClassName);
        // How many cards a Virtualize renders with no browser to measure differs between runtimes; the
        // window itself is measured in the browser suite.
        Assert.Equal(2, cut.FindComponents<Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize<KanbanCard>>().Count);
    }

    // The same Cards reference means the same column lists: a board re-render for its own reasons must not
    // hand Virtualize new Items and make every column load its window again.
    [Fact]
    public void AReRenderWithTheSameCardsKeepsTheColumnItemLists()
    {
        var cards = ManyCards();
        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, cards)
            .Add(x => x.Virtual, true));
        ICollection<KanbanCard>?[] Items() => cut
            .FindComponents<Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize<KanbanCard>>()
            .Select(v => v.Instance.Items)
            .ToArray();

        var before = Items();
        cut.Render(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, cards)
            .Add(x => x.Virtual, true));
        var after = Items();

        Assert.Same(before[0], after[0]);
        Assert.Same(before[1], after[1]);
    }

    [Fact]
    public void WithoutVirtualEveryCardIsRendered()
    {
        var cut = RenderBoard(false, _ => { });

        Assert.DoesNotContain(Css.Classes.Kanban.Virtual, cut.Find("[role=group]").ClassName);
        Assert.Equal(502, cut.FindAll("[data-flare-drag]").Count);
    }

    // The browser counted 3 rendered cards before the target, but the window started at card 248.
    [Fact]
    public async Task ADropIsPlacedByTheCardItLandedBesideNotByTheRenderedIndex()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(true, c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("d0", "todo", 3, "after", "t250"));

        var todo = Column(changed!, "todo");
        Assert.Equal("d0", todo[251]);
        Assert.Equal("t250", todo[250]);
    }

    [Fact]
    public async Task ACardOutsideTheRenderedWindowCanStillBeDropped()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(true, c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();
        // Unrendered on net9+ (bUnit gives Virtualize a default window); the browser suite covers net8 too.

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("t400", "done", 0, "before", "d0"));

        Assert.NotNull(changed);
        Assert.Equal(["t400", "d0", "d1"], Column(changed!, "done"));
    }

    // Past the rendered cards there is no card to land beside, and the browser's index only says
    // "after what was rendered" - the end of the column is the one place that means.
    [Fact]
    public async Task AVirtualDropWithoutACardBesideItGoesToTheEndOfTheColumn()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = RenderBoard(true, c => changed = c);
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("d0", "todo", 5, "into", null));

        Assert.Equal("d0", Column(changed!, "todo")[^1]);
    }
}
