namespace Flare.Components.Tests;

public sealed class KanbanReadOnlyTests : FlareTestContext
{
    private static readonly KanbanColumn[] _columns =
    [
        new("todo", "To do"),
        new("done", "Done"),
    ];

    private static readonly KanbanCard[] _cards =
    [
        new("a", "todo", "A"),
        new("b", "done", "B"),
    ];

    [Fact]
    public void ReadOnlyCardsRemainFocusableWithoutDragAffordances()
    {
        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, _cards)
            .Add(x => x.ReadOnly, true)
            .Add(x => x.CardTemplate, card => builder =>
                builder.AddContent(0, $"custom:{card.Id}")));

        Assert.Empty(cut.FindAll("[data-flare-drag]"));
        Assert.Empty(cut.FindAll("[aria-roledescription]"));
        Assert.Empty(cut.FindAll("[aria-grabbed]"));
        Assert.Empty(cut.FindAll("[role='status']"));

        var cards = cut.FindAll(".flare-kanban__card");
        Assert.Equal(2, cards.Count);
        Assert.All(cards, card =>
        {
            Assert.Equal("listitem", card.GetAttribute("role"));
            Assert.Equal("0", card.GetAttribute("tabindex"));
        });
        Assert.Contains("custom:a", cards[0].TextContent);
        Assert.Contains("custom:b", cards[1].TextContent);
        Assert.Equal(2, cut.FindAll("[data-flare-drop]").Count);
    }

    [Fact]
    public async Task ReadOnlySuppressesDirectDropMutationAndCallback()
    {
        IReadOnlyList<KanbanCard>? changed = null;
        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, _cards)
            .Add(x => x.ReadOnly, true)
            .Add(x => x.CardsChanged, cards => changed = cards));
        var context = cut.FindComponent<FlareDragContext<KanbanCard>>();

        await cut.InvokeAsync(() => context.Instance.OnDropAsync("a", "done", 0, "before", "b"));
        await cut.InvokeAsync(() => context.Instance.OnDrop.InvokeAsync(new FlareDropEventArgs<KanbanCard>
        {
            Payload = _cards[0],
            TargetId = "done",
            Index = 0,
            Edge = DropEdge.Before,
            OverPayload = _cards[1],
            HasOverPayload = true,
        }));

        Assert.Null(changed);
        Assert.Contains("A", cut.Find(".flare-kanban__column[aria-label='To do']").TextContent);
        Assert.DoesNotContain("A", cut.Find(".flare-kanban__column[aria-label='Done']").TextContent);
    }
}
