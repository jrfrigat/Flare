namespace Flare.Components.Tests;

public sealed class KanbanTemplateTests : FlareTestContext
{
    private static readonly KanbanColumn[] _columns = [new("todo", "To do")];
    private static readonly KanbanCard[] _cards = [new("card-1", "todo", "Default title", "Default description", "Default tag")];

    [Fact]
    public void DefaultRenderingRemainsInsideDragSurface()
    {
        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, _cards));

        var card = cut.Find("[data-flare-drag='card-1']");
        Assert.Equal("listitem", card.GetAttribute("role"));
        Assert.Contains("Default tag", card.TextContent);
        Assert.Contains("Default title", card.TextContent);
        Assert.Contains("Default description", card.TextContent);
        Assert.Contains("To do", cut.Find(".flare-kanban__col-header").TextContent);
    }

    [Fact]
    public void TemplatesReceiveTypedCardAndColumnContext()
    {
        KanbanCard? renderedCard = null;
        KanbanColumnTemplateContext? renderedColumn = null;

        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, _columns)
            .Add(x => x.Cards, _cards)
            .Add(x => x.CardTemplate, card => builder =>
            {
                renderedCard = card;
                builder.AddContent(0, $"custom-card:{card.Id}");
            })
            .Add(x => x.ColumnTemplate, column => builder =>
            {
                renderedColumn = column;
                builder.AddContent(0, $"custom-column:{column.Column.Id}:{column.Cards.Count}");
            }));

        var cardSurface = cut.Find("[data-flare-drag='card-1']");
        Assert.Equal("listitem", cardSurface.GetAttribute("role"));
        Assert.Contains("custom-card:card-1", cardSurface.TextContent);
        Assert.DoesNotContain("Default title", cardSurface.TextContent);
        Assert.Contains("custom-column:todo:1", cut.Find(".flare-kanban__col-header").TextContent);
        Assert.Single(cut.FindAll("[data-flare-drop='todo']"));
        Assert.Equal(_cards[0], renderedCard);
        Assert.Equal(_columns[0], renderedColumn?.Column);
        Assert.Equal(_cards, renderedColumn?.Cards);
    }
}
