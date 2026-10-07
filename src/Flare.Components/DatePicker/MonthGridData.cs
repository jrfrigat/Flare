using System.Globalization;
using System.Runtime.CompilerServices;

namespace Flare.Components;

// Only calendar geometry and labels are shared. Selection, disabled days, templates and focus stay local.
internal sealed class MonthGridData
{
    private static readonly ConditionalWeakTable<CultureInfo, RecentMonths> Cache = new();

    internal CalendarMonth Month { get; }
    internal DateOnly?[][] Weeks { get; }
    internal string?[] Labels { get; } = new string?[42];

    private MonthGridData(CultureInfo culture, int year, int month, DayOfWeek first)
    {
        Month = CalendarMonth.Create(CalendarMath.GridCalendar(culture), year, month);
        Weeks = CalendarMath.MonthGrid(Month, first).Chunk(7).ToArray();
        var index = 0;
        foreach (var week in Weeks)
            foreach (var day in week)
                Labels[index++] = day is { } date ? CalendarMath.FormatSafe(date, "D", culture) : null;
    }

    internal static MonthGridData Get(CultureInfo culture, int year, int month, DayOfWeek first)
    {
        if (!culture.IsReadOnly || !culture.DateTimeFormat.IsReadOnly || !culture.DateTimeFormat.Calendar.IsReadOnly)
            return new MonthGridData(culture, year, month, first);
        return Cache.GetValue(culture, static _ => new RecentMonths()).Get(culture, year, month, first);
    }

    private sealed class RecentMonths
    {
        private readonly Dictionary<(int Year, int Month, DayOfWeek First), MonthGridData> _entries = new();
        private readonly Queue<(int Year, int Month, DayOfWeek First)> _order = new();

        internal MonthGridData Get(CultureInfo culture, int year, int month, DayOfWeek first)
        {
            lock (_entries)
            {
                var key = (year, month, first);
                if (_entries.TryGetValue(key, out var data)) return data;
                data = new MonthGridData(culture, year, month, first);
                if (_entries.Count == 8) _entries.Remove(_order.Dequeue());
                _entries.Add(key, data);
                _order.Enqueue(key);
                return data;
            }
        }
    }
}
