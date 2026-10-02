namespace Flare.Components.Tests;

/// <summary>
/// TASK-105: the calendars must survive the DateOnly bounds. January 0001 and December 9999 have partial
/// weeks outside the range, and navigation must not build year 0 or 10000.
/// </summary>
public class DatePickerBoundsTests : FlareTestContext
{
    [Fact]
    public void MonthGrid_AtTheMinimumDate_KeepsWeekdayColumnsAndLeavesTheGapEmpty()
    {
        var days = CalendarMath.MonthGrid(1, 1, DayOfWeek.Sunday).ToList();

        Assert.Equal(42, days.Count);
        // 0001-01-01 is a Monday: with Sunday first, the Sunday cell before it lies outside DateOnly.
        Assert.Null(days[0]);
        Assert.Equal(DateOnly.MinValue, days[1]);
        Assert.Equal(DayOfWeek.Monday, days[1]!.Value.DayOfWeek);
        var dates = days.Where(d => d.HasValue).ToList();
        Assert.Equal(dates.Count, dates.Distinct().Count());
    }

    [Fact]
    public void MonthGrid_AtTheMaximumDate_HasNoRepeatedLastDay()
    {
        var days = CalendarMath.MonthGrid(9999, 12, DayOfWeek.Sunday).ToList();

        Assert.Equal(42, days.Count);
        var dates = days.Where(d => d.HasValue).ToList();
        Assert.Equal(dates.Count, dates.Distinct().Count());
        Assert.Equal(DateOnly.MaxValue, dates[^1]);
        Assert.All(days.SkipWhile(d => d != DateOnly.MaxValue).Skip(1), d => Assert.Null(d));
    }

    [Fact]
    public void InlineGrid_AtTheMinimumDate_RendersEachDayOnce()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday)
            .Add(x => x.Value, DateOnly.MinValue));

        var labels = cut.FindAll($".{Css.Classes.Picker.Day}").Select(b => b.GetAttribute("aria-label")).ToList();
        Assert.Equal(labels.Count, labels.Distinct().Count());
        Assert.Equal(41, labels.Count);
    }

    [Fact]
    public void PrevAtTheMinimumDate_DoesNotThrow()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Value, DateOnly.MinValue));

        cut.FindAll($".{Css.Classes.DatePicker.Header} button").First().Click();

        Assert.Contains("0001", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }

    [Fact]
    public void NextAtTheMaximumDate_DoesNotThrow()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Value, DateOnly.MaxValue));

        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();

        Assert.Contains("9999", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }

    [Fact]
    public void DateTimePicker_PrevMonth_AtTheMinimumDate_DoesNotThrow()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, new DateTimeOffset(new DateTime(1, 1, 1), TimeSpan.Zero)));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Nav} button").First().Click();

        Assert.Contains("0001", cut.Markup);
    }

    // TASK-129: the range calendar stops at the DateOnly bounds as well.
    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(9999, 12, false)]
    public void RangeCalendar_AtTheBounds_DisablesNavigationAndDoesNotThrow(int year, int month, bool prev)
    {
        var day = new DateOnly(year, month, 1);
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.Mode, DateRangePickerMode.Calendar)
            .Add(x => x.StartDate, day)
            .Add(x => x.EndDate, day));

        var buttons = cut.FindAll($".{Css.Classes.DatePicker.Header} button");
        var nav = prev ? buttons.First() : buttons.Last();
        Assert.True(nav.HasAttribute("disabled"));

        nav.Click();

        Assert.Contains(year.ToString("0000"), cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }
}
