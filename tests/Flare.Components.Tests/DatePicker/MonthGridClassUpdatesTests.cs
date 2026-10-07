using System.Globalization;

namespace Flare.Components.Tests;

public class MonthGridClassUpdatesTests : FlareTestContext
{
    [Fact]
    public void Rerender_UpdatesTodayDisabledAndCustomDayClasses()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var first = new DateOnly(2026, 10, 15);
        var second = first.AddDays(1);
        var disabled = first;
        var extra = "old-day";
        var cut = Render<FlareMonthGrid>(p => p
            .Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, culture).Add(x => x.Today, first)
            .Add(x => x.Disabled, day => day == disabled)
            .Add(x => x.DayClass, _ => extra));
        string Cell(DateOnly day) => $"[role=gridcell][aria-label='{day.ToString("D", culture)}']";
        Assert.Contains(Css.Classes.Picker.DayToday, cut.Find(Cell(first)).ClassList);
        Assert.Contains(Css.Classes.Picker.DayDisabled, cut.Find(Cell(first)).ClassList);

        disabled = second;
        extra = "new-day";
        cut.Render(p => p.Add(x => x.Today, second));

        var previous = cut.Find(Cell(first));
        Assert.DoesNotContain(Css.Classes.Picker.DayToday, previous.ClassList);
        Assert.DoesNotContain(Css.Classes.Picker.DayDisabled, previous.ClassList);
        Assert.False(previous.HasAttribute("disabled"));
        var current = cut.Find(Cell(second));
        Assert.Contains(Css.Classes.Picker.DayToday, current.ClassList);
        Assert.Contains(Css.Classes.Picker.DayDisabled, current.ClassList);
        Assert.True(current.HasAttribute("disabled"));
        Assert.Empty(cut.FindAll(".old-day"));
        Assert.Equal(42, cut.FindAll("[role=gridcell].new-day").Count);
    }
}
