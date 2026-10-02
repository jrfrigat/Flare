using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-133: after the month is changed or a day is clicked, the arrow keys move from the day the user sees
/// focused, not from a cursor the host kept from before. Home/End and PageUp/PageDown follow the same
/// cursor in all three calendar hosts.
/// </summary>
public class CalendarCursorTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");

    private static string Label(int y, int m, int d) => new DateOnly(y, m, d).ToString("D", Ru);

    private static string Tabbable<T>(IRenderedComponent<T> cut) where T : IComponent =>
        cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']").GetAttribute("aria-label")!;

    [Fact]
    public void DatePicker_ArrowAfterNextMonth_StartsFromTheTabbableDay()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2000, 1, 15)));

        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();
        Assert.Equal(Label(2000, 2, 1), Tabbable(cut));
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        Assert.Equal(Label(2000, 2, 2), Tabbable(cut));
    }

    [Fact]
    public void RangePicker_ArrowAfterNextMonth_StartsFromTheTabbableDay()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar)
            .Add(x => x.StartDate, new DateOnly(2000, 1, 15)));

        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        Assert.Equal(new DateOnly(2000, 2, 2).ToString("D", CultureInfo.CurrentCulture), Tabbable(cut));
    }

    [Fact]
    public void DateTimePicker_ArrowAfterNextMonth_StartsFromTheTabbableDay()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateTimeOffset(2000, 1, 15, 12, 30, 0, TimeSpan.Zero)));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Nav} button").Last().Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        Assert.Equal(Label(2000, 2, 2), Tabbable(cut));
    }

    [Fact]
    public void DatePicker_ArrowAfterClick_StartsFromTheClickedDay()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");   // cursor 16.10
        cut.Find($"[aria-label='{Label(2026, 10, 5)}']").Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowDown");

        Assert.Equal(Label(2026, 10, 12), Tabbable(cut));
    }

    [Theory]
    [InlineData("Home", false, 12)]       // ru-RU weeks start on Monday: Thu 15.10 -> Mon 12.10
    [InlineData("End", false, 18)]
    public void DatePicker_HomeEnd_MoveWithinTheWeek(string key, bool shift, int day)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown(new KeyboardEventArgs { Key = key, ShiftKey = shift });

        Assert.Equal(Label(2026, 10, day), Tabbable(cut));
    }

    [Theory]
    [InlineData("PageDown", false, 2026, 2, 28)]
    [InlineData("PageUp", false, 2025, 12, 31)]
    [InlineData("PageDown", true, 2027, 1, 31)]
    public void DatePicker_PageKeys_MoveByMonthOrYear(string key, bool shift, int y, int m, int d)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 1, 31)));

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown(new KeyboardEventArgs { Key = key, ShiftKey = shift });

        Assert.Equal(Label(y, m, d), Tabbable(cut));
    }

    [Fact]
    public void EnterOnTheCursor_PicksExactlyThatDay()
    {
        DateOnly? picked = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2000, 1, 15)).Add(x => x.ValueChanged, (DateOnly? v) => picked = v));

        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");
        // Enter is the native click of the focused (tabbable) day button.
        cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']").Click();

        Assert.Equal(new DateOnly(2000, 2, 2), picked);
    }

    [Fact]
    public void KeyTarget_SkipsDisabledDaysAndStopsAtTheRangeEnds()
    {
        var from = new DateOnly(2026, 10, 15);
        Assert.Equal(new DateOnly(2026, 10, 17),
            CalendarMath.KeyTarget(from, "ArrowRight", false, DayOfWeek.Monday, d => d.Day == 16));
        Assert.Null(CalendarMath.KeyTarget(DateOnly.MaxValue, "ArrowRight", false, DayOfWeek.Monday, null));
        Assert.Null(CalendarMath.KeyTarget(DateOnly.MinValue, "PageUp", false, DayOfWeek.Monday, null));
        Assert.Null(CalendarMath.KeyTarget(from, "Tab", false, DayOfWeek.Monday, null));
    }
}
