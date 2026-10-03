using System.Collections.Concurrent;
using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Shared month-calendar date math for the date pickers (FlareDatePicker / FlareDateTimePicker /
/// FlareDateRangePicker): the weekday header names, the 6x7 day grid and the day a navigation key moves
/// the cursor to. The markup, selection and range-highlight stay in each picker.
/// </summary>
internal static class CalendarMath
{
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CultureInfo, ConcurrentDictionary<Type, CultureInfo>> OnCalendars = new();
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CultureInfo, CultureInfo> GregorianCultures = new();
    internal static readonly GregorianCalendar Gregorian = new();

    /// <summary>
    /// The culture a picker formats, parses and labels with: <paramref name="culture"/> on its own calendar
    /// (Persian for fa-IR, Um al-Qura for ar-SA), or on <paramref name="calendar"/> when one is given and the culture
    /// offers that kind of calendar among its optional ones (Gregorian or Hijri for ar-SA, Hebrew for he-IL). A
    /// calendar the culture does not offer is ignored. Cached per culture object and calendar type (TASK-182).
    /// </summary>
    public static CultureInfo PickerCulture(CultureInfo culture, Calendar? calendar = null)
    {
        if (calendar is null || culture.DateTimeFormat.Calendar.GetType() == calendar.GetType()) return culture;
        var type = calendar.GetType();
        if (culture.OptionalCalendars.FirstOrDefault(c => c.GetType() == type) is not { } offered) return culture;
        return OnCalendars.GetValue(culture, _ => new ConcurrentDictionary<Type, CultureInfo>())
            .GetOrAdd(type, _ =>
            {
                var copy = (CultureInfo)culture.Clone();
                copy.DateTimeFormat.Calendar = offered;
                return copy;
            });
    }

    /// <summary>
    /// The calendar a picker's grid is drawn on for <paramref name="culture"/>: its own calendar when the months
    /// differ from the Gregorian ones (Persian, Hijri, Um al-Qura, Hebrew); the Gregorian calendar when only the
    /// year number differs (Thai Buddhist, Japanese, Korean, Taiwan), so an era change cannot break the year view
    /// and the culture still writes its own year in the labels.
    /// </summary>
    public static Calendar GridCalendar(CultureInfo culture)
    {
        var calendar = culture.DateTimeFormat.Calendar;
        return calendar is GregorianCalendar or ThaiBuddhistCalendar or JapaneseCalendar or KoreanCalendar or TaiwanCalendar
            ? Gregorian
            : calendar;
    }

    /// <summary>The date written with <paramref name="format"/> in <paramref name="culture"/>; on the culture's
    /// Gregorian calendar when its own calendar cannot name the day (Um al-Qura before 1900).</summary>
    public static string FormatSafe(DateOnly date, string format, CultureInfo culture)
    {
        try { return date.ToString(format, culture); }
        catch (ArgumentOutOfRangeException) { return date.ToString(format, GregorianCultures.GetValue(culture, OnGregorian)); }
    }

    /// <summary>The moment written with <paramref name="format"/> in <paramref name="culture"/>; on the culture's
    /// Gregorian calendar when its own calendar cannot name the day.</summary>
    public static string FormatSafe(DateTimeOffset moment, string format, CultureInfo culture)
    {
        try { return moment.ToString(format, culture); }
        catch (ArgumentOutOfRangeException) { return moment.ToString(format, GregorianCultures.GetValue(culture, OnGregorian)); }
    }

    /// <summary>Whether the culture's calendar writes dates in digits; the Hebrew calendar writes them in letters,
    /// so a digit mask cannot edit them.</summary>
    public static bool WritesDigits(CultureInfo culture) => culture.DateTimeFormat.Calendar is not HebrewCalendar;

    private static CultureInfo OnGregorian(CultureInfo culture)
    {
        var gregorian = culture.OptionalCalendars.OfType<GregorianCalendar>().FirstOrDefault();
        if (gregorian is null) return CultureInfo.InvariantCulture;
        var copy = (CultureInfo)culture.Clone();
        copy.DateTimeFormat.Calendar = gregorian;
        return copy;
    }

    /// <summary>The year <paramref name="year"/> of the grid calendar as the culture writes it: 2569 for 2026 in
    /// th-TH, 1405 in fa-IR, letters on the Hebrew calendar; the plain number when the culture's calendar cannot
    /// name the year (the Japanese calendar starts in 1868).</summary>
    public static string YearLabel(int year, CultureInfo culture)
    {
        var grid = GridCalendar(culture);
        var (first, last) = CalendarMonth.Years(grid);
        var day = CalendarMonth.Create(grid, Math.Clamp(year, first, last), 1).Start;
        var mid = grid is GregorianCalendar ? new DateOnly(day.Year, 7, 1) : day;
        var calendar = culture.DateTimeFormat.Calendar;
        var at = mid.ToDateTime(TimeOnly.MinValue);
        return at >= calendar.MinSupportedDateTime && at <= calendar.MaxSupportedDateTime
            ? mid.ToString("yyyy", culture)
            : year.ToString(CultureInfo.InvariantCulture);
    }

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
    /// and the given <paramref name="firstDayOfWeek"/> (used for the optional week-number column). Counted on the
    /// calendar the grid is drawn on - the Gregorian one <see cref="PickerCulture"/> swaps in, not the culture's
    /// native Persian or Hijri calendar.</summary>
    public static int WeekOfYear(DateOnly date, CultureInfo culture, DayOfWeek firstDayOfWeek)
        => culture.DateTimeFormat.Calendar.GetWeekOfYear(
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
        => MonthGrid(CalendarMonth.Create(Gregorian, year, month), firstDayOfWeek);

    /// <summary>The 42-cell grid of <paramref name="month"/> on its calendar; a cell that calendar or
    /// <see cref="DateOnly"/> cannot name is <c>null</c>.</summary>
    public static IEnumerable<DateOnly?> MonthGrid(CalendarMonth month, DayOfWeek firstDayOfWeek)
    {
        var first = month.Start;
        int offset = ((int)first.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        // Day numbers, so the cells around the range bounds are never built as dates (TASK-105).
        var start = first.DayNumber - offset;
        var lo = CalendarMonth.FirstDay(month.Calendar).DayNumber;
        var hi = CalendarMonth.LastDay(month.Calendar).DayNumber;
        for (int i = 0; i < 42; i++)
        {
            var n = start + i;
            yield return n < lo || n > hi ? null : DateOnly.FromDayNumber(n);
        }
    }

    /// <summary>
    /// The day a calendar key press moves the cursor to from <paramref name="from"/>, or null when the key
    /// is not a navigation key or nothing available lies that way. Arrows move by a day or a week, Home/End
    /// to the start/end of the week, PageUp/PageDown by a month (with Shift by a year, clamping the day to the
    /// target month's length). A disabled target is skipped in the direction of travel, up to 400 days. The month
    /// and the year are the ones of <paramref name="calendar"/>, the Gregorian calendar when it is null.
    /// </summary>
    public static DateOnly? KeyTarget(DateOnly from, string key, bool shift, DayOfWeek firstDayOfWeek,
        Func<DateOnly, bool>? disabled, Calendar? calendar = null)
    {
        calendar ??= Gregorian;
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
                var by = key == "PageDown" ? 1 : -1;
                // The calendar keeps the day of month and clamps it to the target month's length.
                DateTime moved;
                try
                {
                    var at = from.ToDateTime(TimeOnly.MinValue);
                    moved = shift ? calendar.AddYears(at, by) : calendar.AddMonths(at, by);
                }
                catch (ArgumentException) { return null; }
                var landed = DateOnly.FromDateTime(moved);
                if (landed < CalendarMonth.FirstDay(calendar) || landed > CalendarMonth.LastDay(calendar)) return null;
                target = landed.DayNumber;
                step = by;
                break;
            default: return null;
        }

        // Arrows keep their own stride past disabled days; the jumps search day by day from where they land.
        var first = CalendarMonth.FirstDay(calendar).DayNumber;
        var last = CalendarMonth.LastDay(calendar).DayNumber;
        for (var i = 0; i < 400; i++, target += step)
        {
            if (target < first || target > last) return null;
            var day = DateOnly.FromDayNumber(target);
            if (!(disabled?.Invoke(day) ?? false)) return day;
        }
        return null;
    }
}
