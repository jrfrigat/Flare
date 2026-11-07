namespace Flare.Components.Scheduler;

/// <summary>Selectors over an application's own model. No inheritance or mutable adapter model is required.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed record SchedulerMapping<TItem>
{
    /// <summary>Stable unique key selector.</summary>
    public required Func<TItem, string> Key { get; init; }
    /// <summary>Caption selector.</summary>
    public required Func<TItem, string> Title { get; init; }
    /// <summary>Timed start selector; unused for all-day items.</summary>
    public Func<TItem, DateTimeOffset> Start { get; init; } = _ => default;
    /// <summary>Timed end selector; unused for all-day items.</summary>
    public Func<TItem, DateTimeOffset> End { get; init; } = _ => default;
    /// <summary>Date range selector; null means timed.</summary>
    public Func<TItem, SchedulerDateRange?>? AllDay { get; init; }
    /// <summary>Optional resource key selector.</summary>
    public Func<TItem, string?>? ResourceKey { get; init; }
    /// <summary>Optional series time-zone selector; null means UTC recurrence.</summary>
    public Func<TItem, string?>? TimeZoneId { get; init; }
    /// <summary>Optional details selector.</summary>
    public Func<TItem, string?>? Description { get; init; }
    /// <summary>Optional semantic/custom accent selector.</summary>
    public Func<TItem, FlareColor>? Color { get; init; }
    /// <summary>Optional RFC 5545 RRULE selector.</summary>
    public Func<TItem, string?>? RecurrenceRule { get; init; }
    /// <summary>Optional EXDATE instants selector.</summary>
    public Func<TItem, IReadOnlyList<DateTimeOffset>>? ExceptionDates { get; init; }
    /// <summary>Optional date-only EXDATE selector.</summary>
    public Func<TItem, IReadOnlyList<DateOnly>>? ExceptionDays { get; init; }
    /// <summary>Optional RDATE instants selector.</summary>
    public Func<TItem, IReadOnlyList<DateTimeOffset>>? RecurrenceDates { get; init; }
    /// <summary>Optional date-only RDATE selector.</summary>
    public Func<TItem, IReadOnlyList<DateOnly>>? RecurrenceDays { get; init; }
    /// <summary>Master key selector for exception items.</summary>
    public Func<TItem, string?>? SeriesKey { get; init; }
    /// <summary>Original occurrence identity selector for exception items.</summary>
    public Func<TItem, string?>? RecurrenceId { get; init; }
    /// <summary>Projects one item without changing it.</summary>
    public SchedulerEventData Project(TItem item) => new()
    {
        Key = Key(item), Title = Title(item), Start = Start(item), End = End(item), AllDay = AllDay?.Invoke(item),
        ResourceKey = ResourceKey?.Invoke(item), TimeZoneId = TimeZoneId?.Invoke(item), Description = Description?.Invoke(item),
        Color = Color?.Invoke(item) ?? default, RecurrenceRule = RecurrenceRule?.Invoke(item),
        ExceptionDates = ExceptionDates?.Invoke(item) ?? [], ExceptionDays = ExceptionDays?.Invoke(item) ?? [],
        RecurrenceDates = RecurrenceDates?.Invoke(item) ?? [], RecurrenceDays = RecurrenceDays?.Invoke(item) ?? [],
        SeriesKey = SeriesKey?.Invoke(item), RecurrenceId = RecurrenceId?.Invoke(item)
    };
}
