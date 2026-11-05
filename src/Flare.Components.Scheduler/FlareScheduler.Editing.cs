using Flare.Abstractions;
using Flare.Components.Scheduler;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

public partial class FlareScheduler<TItem>
{
    [Inject] private IDialogService Dialogs { get; set; } = default!;
    private bool _editorOpen, _submitting;
    private DialogReference? _editorReference;
    private readonly SchedulerFocus _focus = new();
    private string? _focusEvent { get => _focus.Identity; set => _focus.Identity = value; }
    internal void RejectChange()
    {
        _announcementKey = "Invalid";
        _version++;
        StateHasChanged();
    }
    internal void ReturnFocusTo(string identity, bool resize = false) => _focus.ReturnFocusTo(identity, resize);

    /// <summary>Enables creation, editor, move, resize and delete. The application must handle OnChange.</summary>
    [Parameter] public bool Editable { get; set; }
    /// <summary>Shows the shared resizable's single-click buttons in addition to its keyboard-accessible handle.</summary>
    [Parameter] public bool ShowResizeControls { get; set; }
    /// <summary>Whether UI editing is enabled and has an application persistence callback.</summary>
    public bool CanEdit => Editable && OnChange.HasDelegate;
    /// <summary>Receives accepted UI intents. Persist in the application or backend; set Cancel to refuse.</summary>
    [Parameter] public EventCallback<SchedulerChangeEventArgs<TItem>> OnChange { get; set; }
    /// <summary>Replaces the dialog body. Use its working copy and SaveAsync/CancelAsync commands.</summary>
    [Parameter] public RenderFragment<SchedulerEditContext<TItem>>? EditorTemplate { get; set; }

    /// <summary>Opens the event-details editor without mutating the original item.</summary>
    /// <param name="occurrence">Occurrence to edit.</param>
    public Task EditEventAsync(SchedulerOccurrence<TItem> occurrence) => OpenEditorAsync(new()
    {
        Owner = this, Original = occurrence, Draft = occurrence.Data, Action = SchedulerEditAction.Update
    });

    /// <summary>Opens a new-event editor for a selected timed or all-day slot.</summary>
    /// <param name="slot">Initial dates and resource.</param>
    public Task CreateEventAsync(SchedulerSlot slot) => OpenEditorAsync(new()
    {
        Owner = this, Action = SchedulerEditAction.Create,
        Draft = new()
        {
            Key = Guid.NewGuid().ToString("N"), Title = "", Start = slot.Start, End = slot.End,
            AllDay = slot.AllDay, ResourceKey = slot.ResourceKey, TimeZoneId = TimeZone.Id
        }
    });

    /// <summary>Requests a gesture change, asking for series scope when needed.</summary>
    /// <param name="occurrence">Original occurrence.</param>
    /// <param name="data">Proposed dates or resource.</param>
    /// <param name="action">Move, resize or delete.</param>
    public async Task RequestChangeAsync(SchedulerOccurrence<TItem> occurrence, SchedulerEventData data, SchedulerEditAction action)
    {
        if (!Editable || !OnChange.HasDelegate || _editorOpen || _disposed) return;
        if (action is SchedulerEditAction.Move or SchedulerEditAction.Resize && data == occurrence.Data)
        { _version++; StateHasChanged(); return; }
        var context = new SchedulerEditContext<TItem>
        {
            Owner = this, Original = occurrence, Draft = data, Action = action, ScopeOnly = true
        };
        if (context.IsRecurring || action == SchedulerEditAction.Delete) await OpenEditorAsync(context);
        else await ApplyChangeAsync(action, occurrence, data);
    }

    private async Task OpenEditorAsync(SchedulerEditContext<TItem> context)
    {
        if (!Editable || !OnChange.HasDelegate || _editorOpen || _disposed) return;
        _editorOpen = true;
        _focus.Editing = true;
        if (context.Original is { } original) ReturnFocusTo(original.Identity, context.ScopeOnly && context.Action == SchedulerEditAction.Resize);
        try
        {
            _editorReference = Dialogs.Show<FlareSchedulerEditor<TItem>>(null,
                new DialogParameters().Add(nameof(FlareSchedulerEditor<TItem>.Context), context),
                new DialogOptions { TitleProvider = () => SchedulerText[context.Action == SchedulerEditAction.Create
                    ? "Create" : context.Action == SchedulerEditAction.Delete ? "Delete" : "Edit"].Value });
            var result = await _editorReference.Result;
            if (result.Cancelled && !_disposed) { _announcementKey = "Cancelled"; _version++; StateHasChanged(); }
        }
        finally { _editorReference = null; _editorOpen = false; _focus.Editing = false; if (!_disposed) StateHasChanged(); }
    }

    /// <summary>Validates and submits an explicit change; false means read-only, invalid, busy or refused.</summary>
    /// <param name="action">Requested action.</param>
    /// <param name="original">Original occurrence; null only for creation.</param>
    /// <param name="data">Proposed values, retaining the original key for updates.</param>
    /// <param name="scope">Application persistence scope.</param>
    /// <returns>Whether the application accepted the intent.</returns>
    public async Task<bool> ApplyChangeAsync(SchedulerEditAction action, SchedulerOccurrence<TItem>? original,
        SchedulerEventData data, SchedulerEditScope scope = SchedulerEditScope.Occurrence)
    {
        if (!Editable || !OnChange.HasDelegate || _submitting || _disposed) return false;
        if (!Enum.IsDefined(action) || !Enum.IsDefined(scope)) throw new ArgumentOutOfRangeException(nameof(action));
        if ((action == SchedulerEditAction.Create) != (original is null)) throw new ArgumentException("Creation requires no original; other changes require one.", nameof(original));
        if (original is not null && original.Data.Key != data.Key) throw new ArgumentException("An update must retain its event key.", nameof(data));
        if (string.IsNullOrWhiteSpace(data.Title) || string.IsNullOrWhiteSpace(data.Key)
            || (data.AllDay is { } days ? days.End <= days.Start : data.End < data.Start))
        { RejectChange(); return false; }
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
