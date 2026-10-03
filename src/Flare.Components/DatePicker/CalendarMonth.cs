using System.Globalization;

namespace Flare.Components;

/// <summary>
/// One month of the calendar a date picker's grid is drawn on (TASK-182): the year and month are that
/// calendar's own numbers (1405/7 on the Persian calendar, 5787/6 for Adar I on the Hebrew one), and the
/// month's days are <see cref="DateOnly"/> values. Navigation stays inside the days both the calendar and
/// <see cref="DateOnly"/> can name (Um al-Qura covers 1900-2077, Hebrew 1583-2239).
/// </summary>
internal readonly record struct CalendarMonth(Calendar Calendar, int Year, int Month)
{
    /// <summary>The first day <paramref name="calendar"/> and <see cref="DateOnly"/> can both name.</summary>
    public static DateOnly FirstDay(Calendar calendar) => DateOnly.FromDateTime(calendar.MinSupportedDateTime);

    /// <summary>The last day <paramref name="calendar"/> and <see cref="DateOnly"/> can both name.</summary>
    public static DateOnly LastDay(Calendar calendar) => Min(DateOnly.MaxValue, DateOnly.FromDateTime(calendar.MaxSupportedDateTime));

    /// <summary>The month holding <paramref name="day"/>, or the nearest month the calendar can name.</summary>
    public static CalendarMonth Of(DateOnly day, Calendar calendar)
    {
        var d = Clamp(day, FirstDay(calendar), LastDay(calendar)).ToDateTime(TimeOnly.MinValue);
        return new CalendarMonth(calendar, calendar.GetYear(d), calendar.GetMonth(d));
    }

    /// <summary>The given month of the given year, moved into the calendar's range: a year past either end
    /// gives the first or last month, and a month past the year's last (a 13th month in a common Hebrew
    /// year) gives that year's last month.</summary>
    public static CalendarMonth Create(Calendar calendar, int year, int month)
    {
        var first = Of(FirstDay(calendar), calendar);
        var last = Of(LastDay(calendar), calendar);
        if (year < first.Year || (year == first.Year && month < first.Month)) return first;
        if (year > last.Year || (year == last.Year && month > last.Month)) return last;
        return new CalendarMonth(calendar, year, Math.Clamp(month, 1, calendar.GetMonthsInYear(year)));
    }

    /// <summary>The month's first day the calendar can name.</summary>
    public DateOnly Start
    {
        get
        {
            try { return Max(FirstDay(Calendar), DateOnly.FromDateTime(Calendar.ToDateTime(Year, Month, 1, 0, 0, 0, 0))); }
            catch (ArgumentOutOfRangeException) { return FirstDay(Calendar); }
        }
    }

    /// <summary>The month's last day the calendar can name.</summary>
    public DateOnly End
    {
        get
        {
            try
            {
                var last = Calendar.ToDateTime(Year, Month, Calendar.GetDaysInMonth(Year, Month), 0, 0, 0, 0);
                return Min(LastDay(Calendar), DateOnly.FromDateTime(last));
            }
            catch (ArgumentOutOfRangeException) { return LastDay(Calendar); }
        }
    }

    /// <summary>Whether <paramref name="day"/> falls in this month.</summary>
    public bool Contains(DateOnly day) => day >= Start && day <= End;

    /// <summary>The following month, or null past the calendar's last one.</summary>
    public CalendarMonth? Next => End >= LastDay(Calendar) ? null : Of(End.AddDays(1), Calendar);

    /// <summary>The preceding month, or null before the calendar's first one.</summary>
    public CalendarMonth? Prev => Start <= FirstDay(Calendar) ? null : Of(Start.AddDays(-1), Calendar);

    /// <summary>How many months this month's year has (13 in a Hebrew leap year).</summary>
    public int MonthsInYear => Calendar.GetMonthsInYear(Year);

    /// <summary>The first and last years the calendar can name inside the <see cref="DateOnly"/> range.</summary>
    public static (int First, int Last) Years(Calendar calendar) =>
        (Of(FirstDay(calendar), calendar).Year, Of(LastDay(calendar), calendar).Year);

    /// <summary>Whether month <paramref name="month"/> of <paramref name="year"/> cannot be picked: it is not a
    /// month the calendar can name there, or it lies wholly outside [<paramref name="min"/>; <paramref name="max"/>].</summary>
    public static bool MonthUnavailable(Calendar calendar, int year, int month, DateOnly? min, DateOnly? max)
    {
        var m = Create(calendar, year, month);
        if (m.Year != year || m.Month != month) return true;
        return (min is { } lo && m.End < lo) || (max is { } hi && m.Start > hi);
    }

    /// <summary>Whether <paramref name="year"/> cannot be picked: outside the calendar's years or wholly outside
    /// [<paramref name="min"/>; <paramref name="max"/>].</summary>
    public static bool YearUnavailable(Calendar calendar, int year, DateOnly? min, DateOnly? max)
    {
        var (first, last) = Years(calendar);
        if (year < first || year > last) return true;
        var start = Create(calendar, year, 1).Start;
        var end = Create(calendar, year, calendar.GetMonthsInYear(year)).End;
        return (min is { } lo && end < lo) || (max is { } hi && start > hi);
    }

    private static DateOnly Max(DateOnly a, DateOnly b) => a > b ? a : b;
    private static DateOnly Min(DateOnly a, DateOnly b) => a < b ? a : b;
    private static DateOnly Clamp(DateOnly d, DateOnly lo, DateOnly hi) => d < lo ? lo : d > hi ? hi : d;
}
