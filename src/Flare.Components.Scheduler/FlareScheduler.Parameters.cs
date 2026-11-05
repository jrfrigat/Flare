using System.Globalization;
using Flare.Components.Scheduler;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>Calendar scheduling over application-owned events, composed from Calendar and Gantt projections.</summary>
/// <typeparam name="TItem">Application event type, projected without mutation or inheritance.</typeparam>
public partial class FlareScheduler<TItem> : ISchedulerCommands<TItem>
{
    /// <summary>Selectors for the application's event model.</summary>
    [Parameter, EditorRequired] public SchedulerMapping<TItem> Mapping { get; set; } = default!;
    /// <summary>Ready events, or series when a recurrence adapter is supplied. Ignored when EventsProvider is set.</summary>
    [Parameter] public IEnumerable<TItem>? Events { get; set; }
    /// <summary>Loads only the visible window; obsolete results are ignored even if the provider ignores cancellation.</summary>
    [Parameter] public Func<SchedulerDataRequest, Task<IEnumerable<TItem>>>? EventsProvider { get; set; }
    /// <summary>Optional client recurrence expansion. The default displays provider-owned occurrences unchanged.</summary>
    [Parameter] public ISchedulerRecurrenceExpander? RecurrenceExpander { get; set; }
    /// <summary>Navigation date; default uses today's date in the display zone.</summary>
    [Parameter] public DateOnly Date { get; set; }
    /// <summary>Reports navigation. With a delegate, Date remains authoritative.</summary>
    [Parameter] public EventCallback<DateOnly> DateChanged { get; set; }
    /// <summary>Initial or controlled projection.</summary>
    [Parameter] public SchedulerView View { get; set; } = SchedulerView.Week;
    /// <summary>Reports view changes. With a delegate, View remains authoritative.</summary>
    [Parameter] public EventCallback<SchedulerView> ViewChanged { get; set; }
    /// <summary>Views available in the default toolbar.</summary>
    [Parameter] public IReadOnlyList<SchedulerView> Views { get; set; } = Enum.GetValues<SchedulerView>();
    /// <summary>Culture for dates and the first weekday. Interface strings use CurrentUICulture independently.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>Display zone. Timed events keep their instants; all-day dates do not move.</summary>
    [Parameter] public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;
    /// <summary>Duration of a time-grid slot; must divide one day and be at least five minutes.</summary>
    [Parameter] public TimeSpan SlotDuration { get; set; } = TimeSpan.FromMinutes(30);
    /// <summary>Weekdays shown by WorkWeek and used to shade nonworking slots.</summary>
    [Parameter] public IReadOnlySet<DayOfWeek> WorkDays { get; set; } = new HashSet<DayOfWeek>
        { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
    /// <summary>Start of working hours; earlier slots stay selectable but are shaded.</summary>
    [Parameter] public TimeOnly WorkStart { get; set; } = new(9, 0);
    /// <summary>Initial time-grid scroll target. Null uses WorkStart; midnight keeps the top. Existing user scroll is preserved.</summary>
    [Parameter] public TimeOnly? InitialScrollTime { get; set; }
    /// <summary>End of working hours, exclusive.</summary>
    [Parameter] public TimeOnly WorkEnd { get; set; } = new(18, 0);
    /// <summary>Resource rows. Unknown resource keys are retained as additional rows.</summary>
    [Parameter] public IReadOnlyList<SchedulerResource> Resources { get; set; } = [];
    /// <summary>Explicit occurrence safety limit, applied after window filtering.</summary>
    [Parameter] public int MaximumOccurrences { get; set; } = 10000;
    /// <summary>Explicit local recurrence evaluation limit.</summary>
    [Parameter] public int MaximumEvaluationSteps { get; set; } = 100000;
    /// <summary>Shows the default or templated navigation toolbar.</summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;
    /// <summary>Accessible name of this scheduling surface.</summary>
    [Parameter] public string? AriaLabel { get; set; }
    /// <summary>Custom event content, retaining the surrounding interaction control.</summary>
    [Parameter] public RenderFragment<SchedulerOccurrence<TItem>>? EventTemplate { get; set; }
    /// <summary>Replaces a built-in projection using the same loaded snapshot and owner callbacks.</summary>
    [Parameter] public RenderFragment<SchedulerViewContext<TItem>>? ViewTemplate { get; set; }
    /// <summary>Custom navigation content.</summary>
    [Parameter] public RenderFragment<SchedulerViewContext<TItem>>? ToolbarTemplate { get; set; }
    /// <summary>Reports event selection without mutating the item.</summary>
    [Parameter] public EventCallback<SchedulerOccurrence<TItem>> OnEventSelected { get; set; }
    /// <summary>Reports slot selection.</summary>
    [Parameter] public EventCallback<SchedulerSlot> OnSlotSelected { get; set; }
    /// <summary>Reports a provider/projection error; the built-in surface displays a localized failure.</summary>
    [Parameter] public EventCallback<Exception> OnLoadError { get; set; }
    /// <summary>Replaces the entire composition, receiving the same context as independent projections.</summary>
    [Parameter] public RenderFragment<SchedulerViewContext<TItem>>? ChildContent { get; set; }
    /// <summary>Selected interval; selecting an event clears it.</summary>
    public SchedulerSlot? SelectedSlot { get; private set; }
    /// <summary>Current immutable loaded snapshot, also available to custom projections.</summary>
    public SchedulerViewContext<TItem> Snapshot => new()
    {
        Commands = this, Window = _window, Occurrences = _occurrences,
        Date = _date, Culture = EffectiveCulture, Version = _version, View = _view,
        TimeZone = TimeZone, SlotDuration = SlotDuration, WorkDays = WorkDays, WorkStart = WorkStart,
        WorkEnd = WorkEnd, InitialScrollTime = InitialScrollTime, Resources = Resources,
        CanEdit = CanEdit, ShowResizeControls = ShowResizeControls, AriaLabel = AriaLabel,
        SelectedEvent = SelectedEvent, SelectedSlot = SelectedSlot, IsLoading = _loading, Error = _error,
        EventTemplate = EventTemplate, Focus = _focus
    };
    /// <summary>Current projection after controlled/uncontrolled resolution.</summary>
    public SchedulerView CurrentView => _view;
    /// <summary>Resolved date-formatting culture.</summary>
    public CultureInfo EffectiveCulture => Culture ?? FormattingCulture;
    /// <summary>Selected occurrence, or null after selecting a slot.</summary>
    public SchedulerOccurrence<TItem>? SelectedEvent { get; private set; }
}
