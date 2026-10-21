using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// The column counter announces a localized count: English singular and plural, and the Russian forms
/// for one, two-to-four and five-plus, including the 11-14 exception.
/// </summary>
public sealed class KanbanCardCountTests : FlareTestContext
{
    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;
        public CultureScope(string name) => CultureInfo.CurrentUICulture = new CultureInfo(name);
        public void Dispose() => CultureInfo.CurrentUICulture = _previous;
    }

    private string CountLabel(int count)
    {
        var columns = new[] { new KanbanColumn("todo", "To do") };
        var cards = Enumerable.Range(1, count)
            .Select(i => new KanbanCard($"card-{i}", "todo", $"Card {i}", "Description", "Tag"))
            .ToArray();
        var cut = Render<FlareKanban>(p => p
            .Add(x => x.Columns, columns)
            .Add(x => x.Cards, cards));
        return cut.Find($".{Css.Classes.Kanban.Count}").GetAttribute("aria-label") ?? string.Empty;
    }

    [Theory]
    [InlineData(0, "0 cards")]
    [InlineData(1, "1 card")]
    [InlineData(3, "3 cards")]
    public void English_CountsTheCards(int count, string expected)
        => Assert.Equal(expected, CountLabel(count));

    [Theory]
    [InlineData(1, "1 карточка")]
    [InlineData(3, "3 карточки")]
    [InlineData(5, "5 карточек")]
    [InlineData(11, "11 карточек")]
    [InlineData(21, "21 карточка")]
    public void Russian_UsesThePluralForms(int count, string expected)
    {
        using var _ = new CultureScope("ru");
        Assert.Equal(expected, CountLabel(count));
    }
}
