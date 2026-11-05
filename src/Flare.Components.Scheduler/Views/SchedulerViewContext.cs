using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>Immutable loaded snapshot and interaction port, independent of any visual scheduler shell.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed record SchedulerViewContext<TItem>
{
    /// <summary>Per-instance navigation and interaction commands.</summary>
    public required ISchedulerCommands<TItem> Commands { get; init; }
    /// <summary>Active projection.</summary>
    public required SchedulerView View { get; init; }
    /// <summary>Display zone; all-day date ranges are independent of it.</summary>
    public TimeZoneInfo TimeZone { get; init; } = TimeZoneInfo.Local;
    /// <summary>Duration of one time-grid slot.</summary>
    public TimeSpan SlotDuration { get; init; } = TimeSpan.FromMinutes(30);
    /// <summary>Working weekdays, also used by WorkWeek.</summary>
    public IReadOnlySet<DayOfWeek> WorkDays { get; init; } = new HashSet<DayOfWeek>
        { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
    /// <summary>Start of working hours.</summary>
    public TimeOnly WorkStart { get; init; } = new(9, 0);
    /// <summary>Exclusive end of working hours.</summary>
    public TimeOnly WorkEnd { get; init; } = new(18, 0);
    /// <summary>Initial scroll target; null uses WorkStart.</summary>
    public TimeOnly? InitialScrollTime { get; init; }
    /// <summary>Resource rows; these do not identify calendars.</summary>
    public IReadOnlyList<SchedulerResource> Resources { get; init; } = [];
    /// <summary>Whether the application accepts editing commands.</summary>
    public bool CanEdit { get; init; }
    /// <summary>Displays the shared resizable's single-click controls.</summary>
    public bool ShowResizeControls { get; init; }
    /// <summary>Accessible name for a projection.</summary>
    public string? AriaLabel { get; init; }
    /// <summary>Selected occurrence, independent of the rendered projection.</summary>
    public SchedulerOccurrence<TItem>? SelectedEvent { get; init; }
    /// <summary>Selected interval, or null after selecting an event.</summary>
    public SchedulerSlot? SelectedSlot { get; init; }
    /// <summary>Whether this window is being loaded.</summary>
    public bool IsLoading { get; init; }
    /// <summary>Latest provider or projection error.</summary>
    public Exception? Error { get; init; }
    /// <summary>Optional event caption content.</summary>
    public Microsoft.AspNetCore.Components.RenderFragment<SchedulerOccurrence<TItem>>? EventTemplate { get; init; }
    internal SchedulerFocus Focus { get; init; } = new();
    /// <summary>Visible instant/date window.</summary>
    public required SchedulerWindow Window { get; init; }
    /// <summary>Expanded, windowed occurrences retaining their application items.</summary>
    public required IReadOnlyList<SchedulerOccurrence<TItem>> Occurrences { get; init; }
    /// <summary>Current navigation date.</summary>
    public required DateOnly Date { get; init; }
    /// <summary>Culture used by this view.</summary>
    public required CultureInfo Culture { get; init; }
    /// <summary>Version of the loaded snapshot, including a rejected edit refresh.</summary>
    public required int Version { get; init; }
}

internal sealed class SchedulerFocus
{
    internal string? Identity { get; set; }
    internal bool Resize { get; set; }
    internal bool Editing { get; set; }
    internal Action? Reject { get; set; }
    internal void ReturnFocusTo(string identity, bool resize = false) { Identity = identity; Resize = resize; }
    internal bool HasEventFocus(string identity) => Identity == identity;
    internal bool HasResizeFocus(string identity) => Identity == identity && Resize;
    internal bool TakeEventFocus(string identity, out bool resize)
    {
        resize = Resize;
        if (Editing || Identity != identity) return false;
        Identity = null;
        return true;
    }
}
