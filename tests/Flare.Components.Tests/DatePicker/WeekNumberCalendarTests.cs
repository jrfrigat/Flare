using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-177: the week-number column counts weeks on the Gregorian calendar the grid is drawn on, also for a culture
/// whose own calendar is Persian or Hijri.
/// </summary>
public class WeekNumberCalendarTests : FlareTestContext
{
    private string FirstWeek(string culture, int year, int month)
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, year).Add(x => x.ViewMonth, month)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo(culture)).Add(x => x.ShowWeekNumbers, true));
        return cut.FindAll("[role=rowheader]")[0].TextContent.Trim();
    }

    private static string GregorianWeek(string culture, DateTime firstCell)
    {
        var fmt = CultureInfo.GetCultureInfo(culture).DateTimeFormat;
        return new GregorianCalendar().GetWeekOfYear(firstCell, fmt.CalendarWeekRule, fmt.FirstDayOfWeek).ToString();
    }

    // The first row of a month starts on the culture's first day of week on or before the 1st.
    private static DateTime FirstCell(string culture, int year, int month)
    {
        var first = new DateTime(year, month, 1);
        var start = (int)CultureInfo.GetCultureInfo(culture).DateTimeFormat.FirstDayOfWeek;
        return first.AddDays(-(((int)first.DayOfWeek - start + 7) % 7));
    }

    [Theory]
    [InlineData("fa-IR", 2026, 1)]
    [InlineData("fa-IR", 2026, 7)]
    [InlineData("ar-SA", 2026, 1)]
    [InlineData("ar-SA", 2026, 10)]
    [InlineData("ru-RU", 2026, 1)]
    [InlineData("th-TH", 2026, 7)]
    public void WeekNumbers_AreTheGridsGregorianWeeks(string culture, int year, int month) =>
        Assert.Equal(GregorianWeek(culture, FirstCell(culture, year, month)), FirstWeek(culture, year, month));
}
