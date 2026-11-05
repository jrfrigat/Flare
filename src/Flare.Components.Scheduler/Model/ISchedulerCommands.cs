namespace Flare.Components.Scheduler;

/// <summary>Per-instance navigation and interaction port shared by independent scheduler projections.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public interface ISchedulerCommands<TItem>
{
    /// <summary>Reloads the current data window.</summary>
    Task ReloadAsync();
    /// <summary>Requests the previous or next window according to the sign of direction.</summary>
    Task NavigateAsync(int direction);
    /// <summary>Requests today's date in the display zone.</summary>
    Task TodayAsync();
    /// <summary>Requests a navigation date; the application's controlled value remains authoritative.</summary>
    Task GoToAsync(DateOnly date);
    /// <summary>Requests a projection without mounting inactive projections.</summary>
    Task ShowViewAsync(SchedulerView view);
    /// <summary>Selects an occurrence without changing application data.</summary>
    Task SelectEventAsync(SchedulerOccurrence<TItem> occurrence);
    /// <summary>Selects a half-open timed or all-day interval.</summary>
    Task SelectSlotAsync(SchedulerSlot slot);
    /// <summary>Requests an editing session for an existing occurrence.</summary>
    Task EditEventAsync(SchedulerOccurrence<TItem> occurrence);
    /// <summary>Requests a new-event editing session for an interval.</summary>
    Task CreateEventAsync(SchedulerSlot slot);
    /// <summary>Requests a move, resize or delete, including a recurrence-scope choice when necessary.</summary>
    Task RequestChangeAsync(SchedulerOccurrence<TItem> occurrence, SchedulerEventData data, SchedulerEditAction action);
    /// <summary>Validates and submits application-owned persistence; false leaves the proposed values uncommitted.</summary>
    Task<bool> ApplyChangeAsync(SchedulerEditAction action, SchedulerOccurrence<TItem>? original,
        SchedulerEventData data, SchedulerEditScope scope = SchedulerEditScope.Occurrence);
}
