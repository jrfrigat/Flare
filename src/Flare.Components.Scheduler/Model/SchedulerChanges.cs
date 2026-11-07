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
