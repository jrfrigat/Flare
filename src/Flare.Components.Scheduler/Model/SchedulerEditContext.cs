using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>Outcome of the latest attempt to save an editing session.</summary>
public enum SchedulerEditStatus
{
    /// <summary>No failure is being shown.</summary>
    None,
    /// <summary>The proposed values are invalid.</summary>
    Invalid,
    /// <summary>The application refused the change.</summary>
    Refused,
    /// <summary>Persistence threw an exception; the draft is retained for retry.</summary>
    Failed
}

/// <summary>Independent working copy and submission lifecycle. It does not own a dialog or mutate an application item.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed class SchedulerEditContext<TItem>
{
    /// <summary>Original occurrence, or null for creation.</summary>
    public SchedulerOccurrence<TItem>? Original { get; init; }
    /// <summary>Action proposed by the editor or gesture.</summary>
    public required SchedulerEditAction Action { get; init; }
    /// <summary>Editable immutable draft; replace it with a with-expression when binding fields.</summary>
    public required SchedulerEventData Draft { get; set; }
    /// <summary>Submits a captured proposal. Return false to keep the session open.</summary>
    public required Func<SchedulerChangeEventArgs<TItem>, Task<bool>> Submit { get; init; }
    /// <summary>Selected recurring-event scope.</summary>
    public SchedulerEditScope Scope { get; set; }
    /// <summary>A gesture has already proposed values and only needs confirmation or a scope choice.</summary>
    public bool ScopeOnly { get; init; }
    /// <summary>Zone used when converting editor wall times to instants.</summary>
    public TimeZoneInfo TimeZone { get; init; } = TimeZoneInfo.Local;
    /// <summary>Explicit formatting culture, or null to follow the form's localization scope.</summary>
    public CultureInfo? Culture { get; init; }
    /// <summary>Optional resource choices, independent of calendar identity.</summary>
    public IReadOnlyList<SchedulerResource> Resources { get; init; } = [];
    /// <summary>Whether a series scope must be offered.</summary>
    public bool IsRecurring => Original is { } original && (original.Data.SeriesKey is not null
        || !string.IsNullOrEmpty(original.Series.RecurrenceRule) || original.Series.RecurrenceDates.Count > 0
        || original.Series.RecurrenceDays.Count > 0);
    /// <summary>Whether a captured proposal is awaiting persistence. Repeated saves are ignored.</summary>
    public bool IsSaving { get; private set; }
    /// <summary>Whether the session has been accepted or discarded.</summary>
    public bool IsClosed { get; private set; }
    /// <summary>Whether persistence accepted the final proposal.</summary>
    public bool IsSaved { get; private set; }
    /// <summary>Failure classification, resolved to localized text by the presentation.</summary>
    public SchedulerEditStatus Status { get; private set; }
    /// <summary>Latest persistence exception, available to application error presentations.</summary>
    public Exception? Error { get; private set; }
    /// <summary>Notifies forms and presenters when submission or session state changes.</summary>
    public event Action? Changed;
    internal Func<Task>? CloseHandler { get; set; }

    /// <summary>Marks editor-only validation failure without submitting an older valid draft.</summary>
    public void RejectValidation()
    {
        if (IsClosed || IsSaving) return;
        Status = SchedulerEditStatus.Invalid;
        Error = null;
        Changed?.Invoke();
    }

    /// <summary>Validates and submits once. Refusal or an exception retains the draft and permits retry.</summary>
    public async Task<bool> SaveAsync()
    {
        if (IsSaving || IsClosed) return false;
        if (!SchedulerChangeValidation.IsValid(Action, Original, Draft, Scope))
        { RejectValidation(); return false; }
        var proposal = new SchedulerChangeEventArgs<TItem> { Action = Action, Original = Original, Data = Draft, Scope = Scope };
        IsSaving = true;
        Status = SchedulerEditStatus.None;
        Error = null;
        Changed?.Invoke();
        try
        {
            var accepted = await Submit(proposal);
            if (IsClosed) return false;
            if (!accepted || proposal.Cancel) { Status = SchedulerEditStatus.Refused; return false; }
            IsSaved = true;
            await CloseAsync();
            return true;
        }
        catch (Exception error)
        {
            if (!IsClosed) { Error = error; Status = SchedulerEditStatus.Failed; }
            return false;
        }
        finally { IsSaving = false; Changed?.Invoke(); }
    }

    /// <summary>Discards the working copy. An in-flight persistence operation must finish before cancellation.</summary>
    public Task CancelAsync() => IsSaving ? Task.CompletedTask : CloseAsync();

    internal async Task CloseAsync()
    {
        if (IsClosed) return;
        IsClosed = true;
        Changed?.Invoke();
        if (CloseHandler is not null) await CloseHandler();
    }
}

internal static class SchedulerChangeValidation
{
    internal static bool IsValid<TItem>(SchedulerEditAction action, SchedulerOccurrence<TItem>? original,
        SchedulerEventData data, SchedulerEditScope scope)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (!Enum.IsDefined(action) || !Enum.IsDefined(scope)) throw new ArgumentOutOfRangeException(nameof(action));
        if ((action == SchedulerEditAction.Create) != (original is null))
            throw new ArgumentException("Creation requires no original; other changes require one.", nameof(original));
        if (original is not null && original.Data.Key != data.Key)
            throw new ArgumentException("An update must retain its event key.", nameof(data));
        return !string.IsNullOrWhiteSpace(data.Title) && !string.IsNullOrWhiteSpace(data.Key)
            && (data.AllDay is { } days ? days.End > days.Start : data.End >= data.Start);
    }
}
