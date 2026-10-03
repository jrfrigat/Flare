using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-176: the month and year views turn off what lies wholly outside Min/Max, and an inline calendar follows a
/// value its parent sets to another month without undoing the user's own browsing.
/// </summary>
public class DatePickerViewRangeTests : FlareTestContext
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    private IRenderedComponent<FlareDatePicker> RenderInline(DateOnly value, DateOnly? min = null, DateOnly? max = null) =>
        Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, En).Add(x => x.Value, value)
            .Add(x => x.Min, min).Add(x => x.Max, max));

    private static void HeaderClick(IRenderedComponent<FlareDatePicker> cut) =>
        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").Click();

    [Fact]
    public void MonthView_DisablesMonthsWhollyOutsideMinAndMax()
    {
        var cut = RenderInline(new DateOnly(2026, 7, 1), new DateOnly(2026, 6, 15), new DateOnly(2026, 9, 10));
        HeaderClick(cut);
        var months = cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}");
        Assert.Equal(12, months.Count);
        var enabled = months.Select((b, i) => (b, m: i + 1)).Where(t => !t.b.HasAttribute("disabled")).Select(t => t.m);
        Assert.Equal(new[] { 6, 7, 8, 9 }, enabled);
    }

    [Fact]
    public void MonthView_PickingADisabledMonth_DoesNothing()
    {
        var cut = RenderInline(new DateOnly(2026, 7, 1), new DateOnly(2026, 6, 1));
        HeaderClick(cut);
        cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}")[0].Click();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}"));
    }

    [Fact]
    public void YearView_DisablesYearsOutsideMinAndMax()
    {
        var cut = RenderInline(new DateOnly(2026, 7, 1), new DateOnly(2025, 3, 1), new DateOnly(2027, 2, 1));
        HeaderClick(cut);
        HeaderClick(cut);
        var enabled = cut.FindAll($".{Css.Classes.DatePicker.YearBtn}").Where(b => !b.HasAttribute("disabled"))
            .Select(b => b.TextContent.Trim());
        Assert.Equal(new[] { "2025", "2026", "2027" }, enabled);
    }

    [Fact]
    public void Inline_FollowsAnOutsideValueInAnotherMonth()
    {
        var cut = RenderInline(new DateOnly(2026, 1, 15));
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2026, 8, 20)));
        Assert.Contains("August", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }

    [Fact]
    public void Inline_SameValueAgain_KeepsTheMonthTheUserBrowsedTo()
    {
        var cut = RenderInline(new DateOnly(2026, 1, 15));
        cut.FindAll($".{Css.Classes.DatePicker.Header} button[aria-label]").Last().Click();
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2026, 1, 15)).Add(x => x.Label, "Again"));
        Assert.Contains("February", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }
}
