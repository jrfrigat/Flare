using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Shared month-calendar date math for the date pickers (FlareDatePicker / FlareDateTimePicker):
/// the weekday header names and the 6x7 day grid. The markup, selection, range-highlight and
/// keyboard navigation stay in each picker; only the identical date generation is centralised here.
/// </summary>
internal static class CalendarMath
{
    /// <summary>The seven weekday short-names ordered from the culture's first day of week.</summary>
    public static IReadOnlyList<string> DayHeaders(CultureInfo culture)
        => DayHeaders(culture, culture.DateTimeFormat.FirstDayOfWeek);

    /// <summary>The seven weekday short-names ordered from an explicit <paramref name="firstDayOfWeek"/>.</summary>
    public static IReadOnlyList<string> DayHeaders(CultureInfo culture, DayOfWeek firstDayOfWeek)
    {
        var first = (int)firstDayOfWeek;
        var fmt = culture.DateTimeFormat;
        return Enumerable.Range(0, 7)
            .Select(i => fmt.GetShortestDayName((DayOfWeek)((first + i) % 7)))
            .ToArray();
    }

    /// <summary>The week-of-year number for <paramref name="date"/>, honouring the culture's week rule
    /// and the given <paramref name="firstDayOfWeek"/> (used for the optional week-number column).</summary>
    public static int WeekOfYear(DateOnly date, CultureInfo culture, DayOfWeek firstDayOfWeek)
        => culture.Calendar.GetWeekOfYear(
            date.ToDateTime(TimeOnly.MinValue),
            culture.DateTimeFormat.CalendarWeekRule,
            firstDayOfWeek);

    /// <summary>
    /// The 42-cell (6 weeks x 7 days) month grid, starting on the first-day-of-week on or before the
    /// 1st of the given month, so a full leading/trailing week is always shown.
    /// </summary>
    public static IEnumerable<DateOnly> MonthGrid(int year, int month, DayOfWeek firstDayOfWeek)
    {
        year = Math.Clamp(year, 1, 9999);
        month = Math.Clamp(month, 1, 12);
        var first = new DateOnly(year, month, 1);
        int offset = ((int)first.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        // Work in day numbers so the leading/trailing weeks of January 0001 and December 9999 cannot
        // underflow / overflow DateOnly (TASK-105).
        var start = Math.Max(0, first.DayNumber - offset);
        var last = DateOnly.MaxValue.DayNumber;
        for (int i = 0; i < 42; i++)
            yield return DateOnly.FromDayNumber(Math.Min(start + i, last));
    }
}
