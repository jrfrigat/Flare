using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>One occurrence reduced to the dates it covers, before month and week clipping.</summary>
internal sealed record SchedulerMonthEvent<TItem>(
    SchedulerOccurrence<TItem> Occurrence,
    DateOnly Start,
    DateOnly End,
    bool AllDay,
    string? TimeText)
{
    public bool IsMultiDay => End > Start;
}

/// <summary>One bar of a multi-day event inside a single week.</summary>
internal sealed record SchedulerMonthBar<TItem>(
    SchedulerMonthEvent<TItem> Event,
    DateOnly Start,
    DateOnly End,
    int StartColumn,
    int Length,
    int Band,
    bool ShowsTime);

/// <summary>One week of a month panel: seven dates, its spanning bars and the single-day events per date.</summary>
internal sealed record SchedulerMonthWeek<TItem>(
    IReadOnlyList<DateOnly> Dates,
    IReadOnlyList<SchedulerMonthBar<TItem>> Bars,
    IReadOnlyList<IReadOnlyList<SchedulerMonthEvent<TItem>>> Days,
    int BandCount);

/// <summary>One month of the month projection.</summary>
internal sealed record SchedulerMonthPanel<TItem>(
    DateOnly Month,
    IReadOnlyList<SchedulerMonthWeek<TItem>> Weeks);

/// <summary>
/// Layout of the scheduler month: turns occurrences into dates, then into week bars and day chips.
/// A multi-day event becomes one bar per week it crosses; bars are greedily placed into non-overlapping
/// bands so several spanning events can share a week.
/// </summary>
internal static class SchedulerMonthLayout
{
    public static List<SchedulerMonthEvent<TItem>> Project<TItem>(
        IReadOnlyList<SchedulerOccurrence<TItem>> occurrences, TimeZoneInfo zone, CultureInfo culture)
    {
        var events = new List<SchedulerMonthEvent<TItem>>(occurrences.Count);
        foreach (var occurrence in occurrences)
        {
            var span = SchedulerClock.Span(occurrence.Data, zone);
            var start = DateOnly.FromDateTime(span.Start);
            var end = DateOnly.FromDateTime(span.End > span.Start ? span.End.AddTicks(-1) : span.End);
            var allDay = occurrence.Data.AllDay is not null;
            events.Add(new SchedulerMonthEvent<TItem>(occurrence, start, end, allDay,
                allDay ? null : span.Start.ToString("HH:mm", culture)));
        }
        return events;
    }

    public static SchedulerMonthPanel<TItem> BuildMonth<TItem>(
        DateOnly month, DayOfWeek firstDayOfWeek, IReadOnlyList<SchedulerMonthEvent<TItem>> events)
    {
        var monthStart = new DateOnly(month.Year, month.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // The visible weeks of a month carry the adjacent days too, but a calendar hides their events;
        // clip the events to this month so a bar never starts or ends on a faded day.
        var visible = new List<SchedulerMonthEvent<TItem>>();
        foreach (var item in events)
        {
            if (item.End < monthStart || item.Start > monthEnd) continue;
            var startClipped = item.Start < monthStart;
            var start = startClipped ? monthStart : item.Start;
            var end = item.End > monthEnd ? monthEnd : item.End;
            visible.Add(item with
            {
                Start = start,
                End = end,
                // A bar that entered the month from the previous one has no start time to show.
                TimeText = startClipped ? null : item.TimeText,
            });
        }

        var grid = CalendarGrid.Days(monthStart, firstDayOfWeek);
        var weeks = new List<SchedulerMonthWeek<TItem>>(grid.Count / 7);
        for (var offset = 0; offset < grid.Count; offset += 7)
        {
            var dates = new DateOnly[7];
            for (var i = 0; i < 7; i++) dates[i] = grid[offset + i];

            var bars = PlaceBars(visible, dates[0], dates[6]);
            var days = new IReadOnlyList<SchedulerMonthEvent<TItem>>[7];
            for (var i = 0; i < 7; i++)
                days[i] = visible.Where(item => !item.IsMultiDay && item.Start == dates[i]).ToList();

            var bandCount = bars.Count == 0 ? 0 : bars.Max(bar => bar.Band) + 1;
            weeks.Add(new SchedulerMonthWeek<TItem>(dates, bars, days, bandCount));
        }
        return new SchedulerMonthPanel<TItem>(monthStart, weeks);
    }

    private static List<SchedulerMonthBar<TItem>> PlaceBars<TItem>(
        IReadOnlyList<SchedulerMonthEvent<TItem>> events, DateOnly weekStart, DateOnly weekEnd)
    {
        var segments = new List<Segment<TItem>>();
        foreach (var item in events)
        {
            if (!item.IsMultiDay) continue;
            var start = item.Start < weekStart ? weekStart : item.Start;
            var end = item.End > weekEnd ? weekEnd : item.End;
            if (end < start) continue;
            segments.Add(new Segment<TItem>(item, start, end,
                start.DayNumber - weekStart.DayNumber, end.DayNumber - start.DayNumber + 1));
        }
        if (segments.Count == 0) return [];

        // Long bars first inside a start column keeps the rows stable and compact.
        segments.Sort(static (a, b) =>
        {
            var byStart = a.StartColumn.CompareTo(b.StartColumn);
            if (byStart != 0) return byStart;
            var byLength = b.Length.CompareTo(a.Length);
            return byLength != 0 ? byLength : a.Event.Occurrence.Data.Start.CompareTo(b.Event.Occurrence.Data.Start);
        });

        var bands = new List<List<(DateOnly Start, DateOnly End)>>();
        var result = new List<SchedulerMonthBar<TItem>>(segments.Count);
        foreach (var segment in segments)
        {
            var band = 0;
            while (band < bands.Count && bands[band].Any(taken => Overlaps(taken, segment.Start, segment.End)))
                band++;
            if (band == bands.Count) bands.Add([]);
            bands[band].Add((segment.Start, segment.End));
            result.Add(new SchedulerMonthBar<TItem>(segment.Event, segment.Start, segment.End,
                segment.StartColumn, segment.Length, band,
                !segment.Event.AllDay && segment.Start == segment.Event.Start && segment.Event.TimeText is not null));
        }
        return result;
    }

    private static bool Overlaps((DateOnly Start, DateOnly End) taken, DateOnly start, DateOnly end) =>
        taken.Start <= end && start <= taken.End;

    private sealed record Segment<TItem>(SchedulerMonthEvent<TItem> Event, DateOnly Start, DateOnly End,
        int StartColumn, int Length);
}
