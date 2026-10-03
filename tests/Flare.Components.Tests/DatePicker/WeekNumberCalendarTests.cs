using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-177, TASK-182: the week-number column counts weeks on the calendar the grid is drawn on - the Persian or
/// Um al-Qura one for fa-IR and ar-SA, the Gregorian one when the picker is given it.
/// </summary>
public class WeekNumberCalendarTests : FlareTestContext
{
    private string FirstWeek(string culture, int year, int month, Calendar? calendar = null)
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, year).Add(x => x.ViewMonth, month)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo(culture)).Add(x => x.Calendar, calendar).Add(x => x.ShowWeekNumbers, true));
        return cut.FindAll("[role=rowheader]")[0].TextContent.Trim();
    }

    // The first row starts on the culture's first day of week on or before the month's first day.
    private static string Week(string culture, Calendar calendar, int year, int month)
    {
        var fmt = CultureInfo.GetCultureInfo(culture).DateTimeFormat;
        var first = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
        var cell = first.AddDays(-(((int)first.DayOfWeek - (int)fmt.FirstDayOfWeek + 7) % 7));
        return calendar.GetWeekOfYear(cell, fmt.CalendarWeekRule, fmt.FirstDayOfWeek).ToString();
    }

    [Theory]
    [InlineData("fa-IR", 1405, 1)]
    [InlineData("fa-IR", 1405, 7)]
    [InlineData("ar-SA", 1448, 4)]
    public void WeekNumbers_FollowTheCulturesCalendar(string culture, int year, int month) =>
        Assert.Equal(Week(culture, CultureInfo.GetCultureInfo(culture).DateTimeFormat.Calendar, year, month), FirstWeek(culture, year, month));

    [Theory]
    [InlineData("fa-IR", 2026, 1)]
    [InlineData("ar-SA", 2026, 10)]
    public void WeekNumbers_OnAGivenGregorianCalendar_AreGregorianWeeks(string culture, int year, int month) =>
        Assert.Equal(Week(culture, new GregorianCalendar(), year, month), FirstWeek(culture, year, month, new GregorianCalendar()));

    [Theory]
    [InlineData("ru-RU", 2026, 1)]
    [InlineData("th-TH", 2026, 7)]
    public void WeekNumbers_OfGregorianMonthCultures_AreGregorianWeeks(string culture, int year, int month) =>
        Assert.Equal(Week(culture, new GregorianCalendar(), year, month), FirstWeek(culture, year, month));
}
