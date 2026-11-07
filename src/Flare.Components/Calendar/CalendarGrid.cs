namespace Flare.Components;

/// <summary>The Gregorian month geometry shared by calendar projections; culture controls the first weekday and labels.</summary>
public static class CalendarGrid
{
    /// <summary>Returns the 42 dates starting on the first weekday on or before the month's first day.
    /// Throws when a leading or trailing date lies outside the DateOnly range.</summary>
    public static IReadOnlyList<DateOnly> Days(DateOnly month, DayOfWeek firstDayOfWeek) =>
        CalendarMath.MonthGrid(month.Year, month.Month, firstDayOfWeek)
            .Select(date => date ?? throw new ArgumentOutOfRangeException(nameof(month))).ToArray();
}
