using Flare.Components.Gantt;

namespace Flare.Components.Scheduler;

/// <summary>A date-only event clipped to visible columns, with an assigned collision lane.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
/// <param name="Occurrence">Original visible occurrence.</param>
/// <param name="Column">First visible column, starting at zero.</param>
/// <param name="ColumnCount">Number of adjacent visible columns covered by the event.</param>
/// <param name="Lane">Collision lane, starting at zero.</param>
public sealed record SchedulerAllDaySegment<TItem>(SchedulerOccurrence<TItem> Occurrence, int Column, int ColumnCount, int Lane);

/// <summary>Shared all-day interval partitioning over the actual visible dates, including work-week gaps.</summary>
public static class SchedulerAllDayLayout
{
    /// <summary>Clips half-open event dates to sorted, unique visible dates and reuses the Gantt lane engine.</summary>
    public static IReadOnlyList<SchedulerAllDaySegment<TItem>> Arrange<TItem>(
        IReadOnlyList<SchedulerOccurrence<TItem>> occurrences, IReadOnlyList<DateOnly> dates)
    {
        ArgumentNullException.ThrowIfNull(occurrences);
        ArgumentNullException.ThrowIfNull(dates);
        for (var i = 1; i < dates.Count; i++)
            if (dates[i] <= dates[i - 1]) throw new ArgumentException("Visible dates must be strictly increasing.", nameof(dates));
        if (dates.Count == 0) return [];
        int LowerBound(DateOnly value)
        {
            var low = 0;
            var high = dates.Count;
            while (low < high)
            {
                var mid = low + (high - low) / 2;
                if (dates[mid] < value) low = mid + 1;
                else high = mid;
            }
            return low;
        }
        var spans = new List<(SchedulerOccurrence<TItem> Occurrence, int First, int End)>();
        foreach (var occurrence in occurrences)
        {
            if (occurrence.Data.AllDay is not { } days) continue;
            var first = LowerBound(days.Start);
            var end = LowerBound(days.End);
            if (first < end) spans.Add((occurrence, first, end));
        }
        var origin = DateTime.MinValue;
        var lanes = GanttLanes.Assign(spans.Select(s => (s.Occurrence.Identity,
            new GanttSpan(origin.AddDays(s.First), origin.AddDays(s.End)))));
        return spans.Select(s => new SchedulerAllDaySegment<TItem>(s.Occurrence, s.First, s.End - s.First,
            lanes.Lanes[s.Occurrence.Identity])).ToArray();
    }
}

/// <summary>Hidden occurrences on a date, with the configured reveal action for custom overflow controls.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
/// <param name="Date">Visible date owning this overflow action.</param>
/// <param name="Occurrences">Hidden events on that date.</param>
/// <param name="ShowAsync">Expands the band or invokes the application's overflow handler.</param>
public sealed record SchedulerAllDayOverflow<TItem>(DateOnly Date, IReadOnlyList<SchedulerOccurrence<TItem>> Occurrences, Func<Task> ShowAsync);
