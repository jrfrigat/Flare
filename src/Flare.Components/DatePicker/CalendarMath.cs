using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Shared month-calendar date math for the date pickers (FlareDatePicker / FlareDateTimePicker /
/// FlareDateRangePicker): the weekday header names, the 6x7 day grid and the day a navigation key moves
/// the cursor to. The markup, selection and range-highlight stay in each picker.
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
    /// 1st of the given month, so a full leading/trailing week is always shown. A cell that falls outside
    /// the <see cref="DateOnly"/> range (the leading days of January 0001, the trailing days of December
    /// 9999) is <c>null</c>, so every day keeps its weekday column.
    /// </summary>
    public static IEnumerable<DateOnly?> MonthGrid(int year, int month, DayOfWeek firstDayOfWeek)
    {
        year = Math.Clamp(year, 1, 9999);
        month = Math.Clamp(month, 1, 12);
        var first = new DateOnly(year, month, 1);
        int offset = ((int)first.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        // Day numbers, so the cells around the DateOnly bounds are never built as dates (TASK-105).
        var start = first.DayNumber - offset;
        var last = DateOnly.MaxValue.DayNumber;
        for (int i = 0; i < 42; i++)
        {
            var n = start + i;
            yield return n < 0 || n > last ? null : DateOnly.FromDayNumber(n);
        }
    }

    /// <summary>
    /// The day a calendar key press moves the cursor to from <paramref name="from"/>, or null when the key
    /// is not a navigation key or nothing available lies that way. Arrows move by a day or a week, Home/End
    /// to the start/end of the week, PageUp/PageDown by a month (with Shift by a year, clamping the day to the
    /// target month's length). A disabled target is skipped in the direction of travel, up to 400 days.
    /// </summary>
    public static DateOnly? KeyTarget(DateOnly from, string key, bool shift, DayOfWeek firstDayOfWeek,
        Func<DateOnly, bool>? disabled)
    {
        var weekday = ((int)from.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        int target, step;
        switch (key)
        {
            case "ArrowRight": target = from.DayNumber + 1; step = 1; break;
            case "ArrowLeft": target = from.DayNumber - 1; step = -1; break;
            case "ArrowDown": target = from.DayNumber + 7; step = 7; break;
            case "ArrowUp": target = from.DayNumber - 7; step = -7; break;
            case "Home": target = from.DayNumber - weekday; step = 1; break;
            case "End": target = from.DayNumber + 6 - weekday; step = -1; break;
            case "PageUp":
            case "PageDown":
                var months = (key == "PageDown" ? 1 : -1) * (shift ? 12 : 1);
                var index = from.Year * 12 + from.Month - 1 + months;
                var year = index / 12;
                if (index < 12 || year > 9999) return null;
                var month = index % 12 + 1;
                target = new DateOnly(year, month, Math.Min(from.Day, DateTime.DaysInMonth(year, month))).DayNumber;
                step = months > 0 ? 1 : -1;
                break;
            default: return null;
        }

        // Arrows keep their own stride past disabled days; the jumps search day by day from where they land.
        var last = DateOnly.MaxValue.DayNumber;
        for (var i = 0; i < 400; i++, target += step)
        {
            if (target < 0 || target > last) return null;
            var day = DateOnly.FromDayNumber(target);
            if (!(disabled?.Invoke(day) ?? false)) return day;
        }
        return null;
    }
}
