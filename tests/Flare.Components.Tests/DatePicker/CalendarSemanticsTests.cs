using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-175: the weekday headers are column headers inside the calendar grid and carry the full day name, the
/// month label is a polite live region, the header button says what it switches to, and the month and year views
/// mark the selected button for assistive tech.
/// </summary>
public class CalendarSemanticsTests : FlareTestContext
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    [Fact]
    public void WeekdayHeaders_AreColumnHeadersInsideTheGrid_WithTheFullName()
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10).Add(x => x.Culture, En));
        var headers = cut.FindAll("[role=grid] > [role=row] > [role=columnheader]");
        Assert.Equal(7, headers.Count);
        Assert.Equal("Sunday", headers[0].GetAttribute("aria-label"));
        Assert.NotEqual("Sunday", headers[0].TextContent.Trim());
        Assert.NotEmpty(headers[0].TextContent.Trim());
        Assert.Equal(6, cut.FindAll("[role=grid] > [role=rowgroup] > [role=row]").Count);
    }

    [Fact]
    public void WeekNumberColumn_HasItsOwnHeader()
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10).Add(x => x.Culture, En)
            .Add(x => x.ShowWeekNumbers, true).Add(x => x.FirstDayOfWeek, DayOfWeek.Monday));
        var headers = cut.FindAll("[role=grid] [role=columnheader]");
        Assert.Equal(8, headers.Count);
        Assert.Equal(FlareStrings.Picker_Week, headers[0].GetAttribute("aria-label"));
        Assert.Equal("Monday", headers[1].GetAttribute("aria-label"));
    }

    [Fact]
    public void KeysSentToTheGrid_ReachTheHost()
    {
        string? key = null;
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.OnKeyDown, (KeyboardEventArgs e) => key = e.Key));
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("ArrowRight", key);
    }

    [Fact]
    public void MonthLabels_ArePoliteLiveRegions()
    {
        var date = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 10, 15)));
        Assert.Equal("polite", date.Find($".{Css.Classes.DatePicker.MonthLabel}").GetAttribute("aria-live"));

        var range = Render<FlareDateRangePicker>();
        range.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal("polite", range.Find($".{Css.Classes.DatePicker.MonthLabel}").GetAttribute("aria-live"));

        var dateTime = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels));
        dateTime.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal("polite", dateTime.Find($".{Css.Classes.DateTimePicker.NavLabel}").GetAttribute("aria-live"));
    }

    [Fact]
    public void HeaderButton_SaysWhatItSwitchesTo()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 10, 15)));
        string Hint() => cut.Find($".{Css.Classes.DatePicker.MonthLabel}").GetAttribute("title") ?? "";
        Assert.Equal(FlareStrings.Picker_ChooseMonth, Hint());
        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").Click();
        Assert.Equal(FlareStrings.Picker_ChooseYear, Hint());
        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").Click();
        Assert.Equal(FlareStrings.Picker_ShowDays, Hint());
    }

    [Fact]
    public void MonthAndYearViews_MarkTheSelectedButton()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 7, 1)));
        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").Click();
        var months = cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}");
        Assert.Equal(12, months.Count);
        Assert.Equal(new[] { 7 }, months.Select((b, i) => (b, i)).Where(t => t.b.GetAttribute("aria-pressed") == "true").Select(t => t.i + 1));
        Assert.All(months.Where((_, i) => i != 6), b => Assert.Equal("false", b.GetAttribute("aria-pressed")));

        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").Click();
        var years = cut.FindAll($".{Css.Classes.DatePicker.YearBtn}");
        var pressed = years.Where(b => b.GetAttribute("aria-pressed") == "true").ToList();
        Assert.Single(pressed);
        Assert.Contains("2026", pressed[0].TextContent);
    }
}
