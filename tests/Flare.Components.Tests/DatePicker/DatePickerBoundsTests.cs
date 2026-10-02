namespace Flare.Components.Tests;

/// <summary>
/// TASK-105: the calendars must survive the DateOnly bounds. January 0001 and December 9999 have partial
/// weeks outside the range, and navigation must not build year 0 or 10000.
/// </summary>
public class DatePickerBoundsTests : FlareTestContext
{
    [Fact]
    public void MonthGrid_AtTheMinimumDate_DoesNotUnderflow()
    {
        var days = CalendarMath.MonthGrid(1, 1, DayOfWeek.Sunday).ToList();

        Assert.Equal(42, days.Count);
        Assert.All(days, d => Assert.True(d >= DateOnly.MinValue));
    }

    [Fact]
    public void MonthGrid_AtTheMaximumDate_DoesNotOverflow()
    {
        var days = CalendarMath.MonthGrid(9999, 12, DayOfWeek.Sunday).ToList();

        Assert.Equal(42, days.Count);
        Assert.All(days, d => Assert.True(d <= DateOnly.MaxValue));
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
}
