namespace Flare.Components.Scheduler;

/// <summary>Ready-made mappings for the optional immutable library model.</summary>
public static class SchedulerMappings
{
    /// <summary>Identity mapping for SchedulerEventData. Applications with their own types supply SchedulerMapping instead.</summary>
    public static SchedulerMapping<SchedulerEventData> EventData { get; } = new()
    {
        Key = e => e.Key, Title = e => e.Title, Start = e => e.Start, End = e => e.End, AllDay = e => e.AllDay,
        TimeZoneId = e => e.TimeZoneId, ResourceKey = e => e.ResourceKey, Description = e => e.Description, Color = e => e.Color,
        RecurrenceRule = e => e.RecurrenceRule, ExceptionDates = e => e.ExceptionDates, ExceptionDays = e => e.ExceptionDays,
        RecurrenceDates = e => e.RecurrenceDates, RecurrenceDays = e => e.RecurrenceDays,
        SeriesKey = e => e.SeriesKey, RecurrenceId = e => e.RecurrenceId
    };
}
