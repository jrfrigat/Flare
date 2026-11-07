using Flare.Components.Gantt;

namespace Flare.Components.Scheduler;

/// <summary>Conversions between independent all-day dates, instants and the display clock.</summary>
public static class SchedulerClock
{
    /// <summary>Resolves a wall time. A nonexistent DST time is rejected; an ambiguous time uses the preferred
    /// offset when valid, otherwise its earlier instant.</summary>
    public static DateTimeOffset At(DateOnly date, TimeOnly time, TimeZoneInfo zone, TimeSpan? preferredOffset = null)
    {
        var wall = date.ToDateTime(time, DateTimeKind.Unspecified);
        if (zone.IsInvalidTime(wall)) throw new ArgumentException("The local time does not exist in this time zone.", nameof(time));
        var offset = zone.GetUtcOffset(wall);
        if (zone.IsAmbiguousTime(wall))
        {
            var offsets = zone.GetAmbiguousTimeOffsets(wall);
            offset = preferredOffset is { } preferred && offsets.Contains(preferred) ? preferred : offsets.Max();
        }
        return new DateTimeOffset(wall, offset);
    }

    /// <summary>Builds an instant window for date-only bounds. A skipped midnight advances to the day's first valid minute.</summary>
    public static SchedulerWindow Window(SchedulerDateRange dates, TimeZoneInfo zone)
    {
        if (dates.End <= dates.Start) throw new ArgumentException("The window must contain at least one date.", nameof(dates));
        DateTimeOffset First(DateOnly day)
        {
            var wall = day.ToDateTime(TimeOnly.MinValue);
            while (zone.IsInvalidTime(wall)) wall = wall.AddMinutes(1);
            return At(DateOnly.FromDateTime(wall), TimeOnly.FromDateTime(wall), zone);
        }
        return new(First(dates.Start), First(dates.End), dates);
    }

    /// <summary>Projects an occurrence onto the existing Gantt clock. All-day dates never shift with the display zone.</summary>
    public static GanttSpan Span(SchedulerEventData data, TimeZoneInfo zone) => data.AllDay is { } day
        ? new(day.Start.ToDateTime(TimeOnly.MinValue), day.End.ToDateTime(TimeOnly.MinValue))
        : GanttSpan.FromOffsets(data.Start, data.End, zone);

    /// <summary>Uses the existing Gantt overlap algorithm, including point, identical and nested spans.</summary>
    public static GanttLaneLayout<string> Lanes<TItem>(IEnumerable<SchedulerOccurrence<TItem>> events, TimeZoneInfo zone) =>
        GanttLanes.Assign(events.Select(e => (e.Identity, Span(e.Data, zone))));
}
