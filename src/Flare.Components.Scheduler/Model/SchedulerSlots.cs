namespace Flare.Components.Scheduler;

/// <summary>A real instant interval of a display-zone date, including both copies of a repeated clock time.</summary>
/// <param name="Date">Display date.</param>
/// <param name="Start">Inclusive instant with its display-zone offset.</param>
/// <param name="End">Exclusive instant with its display-zone offset.</param>
public sealed record SchedulerTimeSlot(DateOnly Date, DateTimeOffset Start, DateTimeOffset End);

/// <summary>Time-grid geometry based on actual instants, so daylight-saving days can contain 23 or 25 hours.</summary>
public static class SchedulerSlots
{
    /// <summary>Builds slots for one date. Gaps have no slots; repeated wall times have distinct offsets.</summary>
    /// <param name="date">Display date.</param>
    /// <param name="zone">Display zone.</param>
    /// <param name="duration">Five minutes to one day, dividing a nominal 24-hour day.</param>
    /// <returns>Real intervals; the final one can be shorter on an unusual offset transition.</returns>
    public static IReadOnlyList<SchedulerTimeSlot> ForDate(DateOnly date, TimeZoneInfo zone, TimeSpan duration)
    {
        if (duration < TimeSpan.FromMinutes(5) || duration > TimeSpan.FromDays(1)
            || TimeSpan.TicksPerDay % duration.Ticks != 0) throw new ArgumentOutOfRangeException(nameof(duration));
        var window = SchedulerClock.Window(new(date, date.AddDays(1)), zone);
        var slots = new List<SchedulerTimeSlot>();
        for (var start = window.Start; start < window.End;)
        {
            var end = start + duration < window.End ? start + duration : window.End;
            slots.Add(new(date, TimeZoneInfo.ConvertTime(start, zone), TimeZoneInfo.ConvertTime(end, zone)));
            start = end;
        }
        return slots;
    }
}
