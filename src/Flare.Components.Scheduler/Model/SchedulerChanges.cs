namespace Flare.Components.Scheduler;

/// <summary>Kind of application-owned event change requested by a scheduler interaction.</summary>
public enum SchedulerEditAction
{
    /// <summary>Create an event from a slot.</summary>
    Create,
    /// <summary>Update event details.</summary>
    Update,
    /// <summary>Move an event in time or to a different resource.</summary>
    Move,
    /// <summary>Change event duration.</summary>
    Resize,
    /// <summary>Delete an event or a chosen part of a series.</summary>
    Delete
}

/// <summary>Persistence scope of a recurring-event change; the application owns series splitting and storage.</summary>
public enum SchedulerEditScope
{
    /// <summary>Only the identified occurrence.</summary>
    Occurrence,
    /// <summary>This occurrence and the following occurrences.</summary>
    Following,
    /// <summary>The entire identified series.</summary>
    Series
}

/// <summary>One cancellable intent. The original item, occurrence identity and series metadata remain available.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed class SchedulerChangeEventArgs<TItem>
{
    /// <summary>Requested action.</summary>
    public required SchedulerEditAction Action { get; init; }
    /// <summary>Original occurrence; null only for creation.</summary>
    public SchedulerOccurrence<TItem>? Original { get; init; }
    /// <summary>Proposed immutable values; for deletion these describe the selected event.</summary>
    public required SchedulerEventData Data { get; init; }
    /// <summary>Part of the series to persist.</summary>
    public SchedulerEditScope Scope { get; init; }
    /// <summary>Set by the application to refuse this intent.</summary>
    public bool Cancel { get; set; }
}

/// <summary>Working copy supplied to the default or custom editor; cancelling never mutates an application item.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed class SchedulerEditContext<TItem>
{
    /// <summary>Scheduler owning the persistence callback.</summary>
    public required FlareScheduler<TItem> Owner { get; init; }
    /// <summary>Original occurrence, or null for creation.</summary>
    public SchedulerOccurrence<TItem>? Original { get; init; }
    /// <summary>Action proposed by the editor or gesture.</summary>
    public required SchedulerEditAction Action { get; init; }
    /// <summary>Editable immutable draft. Replace using a with-expression when binding individual fields.</summary>
    public required SchedulerEventData Draft { get; set; }
    /// <summary>Selected recurring-event scope.</summary>
    public SchedulerEditScope Scope { get; set; }
    /// <summary>A gesture has already proposed the values and only needs a scope choice.</summary>
    public bool ScopeOnly { get; init; }
    /// <summary>Whether a series scope must be offered.</summary>
    public bool IsRecurring => Original is { } original && (original.Data.SeriesKey is not null
        || !string.IsNullOrEmpty(original.Series.RecurrenceRule) || original.Series.RecurrenceDates.Count > 0
        || original.Series.RecurrenceDays.Count > 0);
    internal Func<Task<bool>>? SaveHandler { get; set; }
    internal Func<Task>? CancelHandler { get; set; }
    /// <summary>Validates, submits once and closes the editor on acceptance; false leaves the working copy open.</summary>
    public Task<bool> SaveAsync() => SaveHandler?.Invoke() ?? Task.FromResult(false);
    /// <summary>Discards the working copy and dismisses the editor.</summary>
    public Task CancelAsync() => CancelHandler?.Invoke() ?? Task.CompletedTask;
}
