namespace Flare.Components.Scheduler;

/// <summary>Built-in scheduling projections. ViewTemplate can replace any projection.</summary>
public enum SchedulerView
{
    /// <summary>One date with timed slots.</summary>
    Day,
    /// <summary>Seven dates starting on the culture's first weekday.</summary>
    Week,
    /// <summary>The selected working weekdays within the current week.</summary>
    WorkWeek,
    /// <summary>Month grid composed from FlareCalendar.</summary>
    Month,
    /// <summary>Chronological event list.</summary>
    Agenda,
    /// <summary>Virtual resource rows on the existing FlareGantt timeline.</summary>
    Timeline,
    /// <summary>Twelve calendar months.</summary>
    Year
}

/// <summary>A selected slot. AllDay dates stay independent of display-zone changes.</summary>
/// <param name="Start">Timed slot start.</param>
/// <param name="End">Timed slot end.</param>
/// <param name="AllDay">Date range when selected in a calendar view.</param>
/// <param name="ResourceKey">Optional resource.</param>
public sealed record SchedulerSlot(DateTimeOffset Start, DateTimeOffset End, SchedulerDateRange? AllDay = null, string? ResourceKey = null);
