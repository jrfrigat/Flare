using Flare.Components.Scheduler;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

public partial class FlareScheduler<TItem>
{
    private bool _submitting;
    private readonly SchedulerFocus _focus = new();
    private string? _focusEvent { get => _focus.Identity; set => _focus.Identity = value; }
    internal void RejectChange()
    {
        _announcementKey = "Invalid";
        _version++;
        StateHasChanged();
    }
    internal void ReturnFocusTo(string identity, bool resize = false) => _focus.ReturnFocusTo(identity, resize);

    /// <summary>Enables event changes when an application persistence callback is supplied.</summary>
    [Parameter] public bool Editable { get; set; }
    /// <summary>Shows the shared resizable's single-click controls in addition to its keyboard handle.</summary>
    [Parameter] public bool ShowResizeControls { get; set; }
    /// <summary>Whether changes are enabled and have an application persistence callback.</summary>
    public bool CanEdit => Editable && OnChange.HasDelegate;
    /// <summary>Receives validated proposals. Persist in the application; set Cancel to refuse.</summary>
    [Parameter] public EventCallback<SchedulerChangeEventArgs<TItem>> OnChange { get; set; }
    /// <summary>Owns presentation of an editing session when supplied. No built-in dialog is opened.</summary>
    [Parameter] public EventCallback<SchedulerEditContext<TItem>> OnEditRequested { get; set; }
    /// <summary>Current editing session, independent of any form or surface.</summary>
    public SchedulerEditContext<TItem>? ActiveEdit { get; private set; }

    /// <summary>Requests an event-details session without mutating the application item.</summary>
    public Task EditEventAsync(SchedulerOccurrence<TItem> occurrence) => OpenEditorAsync(
        NewSession(SchedulerEditAction.Update, occurrence.Data, occurrence));

    /// <summary>Requests a new-event session for a selected timed or all-day interval.</summary>
    public Task CreateEventAsync(SchedulerSlot slot) => OpenEditorAsync(NewSession(SchedulerEditAction.Create, new()
    {
        Key = Guid.NewGuid().ToString("N"), Title = "", Start = slot.Start, End = slot.End,
        AllDay = slot.AllDay, ResourceKey = slot.ResourceKey, TimeZoneId = TimeZone.Id
    }));

    private SchedulerEditContext<TItem> NewSession(SchedulerEditAction action, SchedulerEventData data,
        SchedulerOccurrence<TItem>? original = null, bool scopeOnly = false) => new()
    {
        Action = action, Draft = data, Original = original, ScopeOnly = scopeOnly,
        TimeZone = TimeZone, Culture = Culture, Resources = Resources,
        Submit = change => ApplyChangeAsync(change.Action, change.Original, change.Data, change.Scope)
    };

    /// <summary>Requests a gesture change, asking for recurrence scope or delete confirmation when needed.</summary>
    public async Task RequestChangeAsync(SchedulerOccurrence<TItem> occurrence, SchedulerEventData data, SchedulerEditAction action)
    {
        if (!CanEdit || ActiveEdit is not null || _disposed) return;
        if (action is not (SchedulerEditAction.Move or SchedulerEditAction.Resize or SchedulerEditAction.Delete))
            throw new ArgumentOutOfRangeException(nameof(action));
        if (action is SchedulerEditAction.Move or SchedulerEditAction.Resize && data == occurrence.Data)
        { _version++; StateHasChanged(); return; }
        var session = NewSession(action, data, occurrence, scopeOnly: true);
        if (session.IsRecurring || action == SchedulerEditAction.Delete) await OpenEditorAsync(session);
        else if (!await session.SaveAsync())
        {
            if (session.Status == SchedulerEditStatus.Invalid) RejectChange();
            else await OpenEditorAsync(session);
        }
    }

    private async Task OpenEditorAsync(SchedulerEditContext<TItem> session)
    {
        if (!CanEdit || ActiveEdit is not null || _disposed) return;
        ActiveEdit = session;
        _focus.Editing = true;
        if (session.Original is { } original) ReturnFocusTo(original.Identity, session.Action == SchedulerEditAction.Resize);
        session.CloseHandler = async () =>
        {
            if (ActiveEdit != session) return;
            ActiveEdit = null;
            _focus.Editing = false;
            if (!_disposed)
            {
                _announcementKey = session.IsSaved ? "Changed" : "Cancelled";
                _version++;
                await InvokeAsync(StateHasChanged);
            }
        };
        StateHasChanged();
        await OnEditRequested.InvokeAsync(session);
    }

    /// <summary>Validates and submits a change; false means read-only, invalid, busy or refused.</summary>
    /// <param name="action">Requested action.</param>
    /// <param name="original">Original occurrence; null only for creation.</param>
    /// <param name="data">Proposed immutable values.</param>
    /// <param name="scope">Application persistence scope.</param>
    /// <returns>Whether the application accepted the change.</returns>
    public async Task<bool> ApplyChangeAsync(SchedulerEditAction action, SchedulerOccurrence<TItem>? original,
        SchedulerEventData data, SchedulerEditScope scope = SchedulerEditScope.Occurrence)
    {
        if (!CanEdit || _submitting || _disposed) return false;
        if (!SchedulerChangeValidation.IsValid(action, original, data, scope)) { RejectChange(); return false; }
        _submitting = true;
        try
        {
            var change = new SchedulerChangeEventArgs<TItem> { Action = action, Original = original, Data = data, Scope = scope };
            await OnChange.InvokeAsync(change);
            if (_disposed) return false;
            _announcementKey = change.Cancel ? "Cancelled" : "Changed";
            if (!change.Cancel && action == SchedulerEditAction.Delete) _focusEvent = null;
            if (!change.Cancel) await ReloadAsync();
            else { _version++; StateHasChanged(); }
            return !change.Cancel;
        }
        finally { _submitting = false; }
    }
}
