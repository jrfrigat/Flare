namespace Flare.Components.Scheduler;

/// <summary>Explicit event actions for custom interactive content inside the layout and gesture boundary.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
/// <param name="View">Snapshot providing state and commands.</param>
/// <param name="Occurrence">Rendered event occurrence.</param>
public sealed record SchedulerEventContext<TItem>(SchedulerViewContext<TItem> View, SchedulerOccurrence<TItem> Occurrence)
{
    internal Func<Microsoft.AspNetCore.Components.Web.KeyboardEventArgs, Task>? KeyHandler { get; init; }
    /// <summary>Handles F2, Delete and modified arrow shortcuts. Bind only to the primary event action, not to nested fields or menus.</summary>
    public Task HandleKeyAsync(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs args) => KeyHandler?.Invoke(args) ?? Task.CompletedTask;
    /// <summary>Whether this occurrence is selected.</summary>
    public bool IsSelected => View.SelectedEvent?.Identity == Occurrence.Identity;
    /// <summary>Selects this occurrence, for example to show an application preview.</summary>
    public Task SelectAsync() => View.Commands.SelectEventAsync(Occurrence);
    /// <summary>Requests an editing session through the configured application presentation.</summary>
    public Task EditAsync() => View.Commands.EditEventAsync(Occurrence);
    /// <summary>Requests deletion, retaining confirmation and recurrence-scope selection.</summary>
    public Task DeleteAsync() => View.Commands.RequestChangeAsync(Occurrence, Occurrence.Data, SchedulerEditAction.Delete);
}
