namespace Flare.Components;

/// <summary>Day, week and working-week projection using the shared Gantt axis and lane calculations.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerTimeGrid<TItem> { }

/// <summary>Month or year projection composed from the existing calendar.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerCalendar<TItem> { }

/// <summary>Chronological list of the loaded event occurrences.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerAgenda<TItem> { }

/// <summary>Virtual resource projection composed from the existing Gantt timeline.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerTimeline<TItem> { }

/// <summary>Shared event selection control that preserves application templates.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerEvent<TItem> { }

/// <summary>Standard dialog body over a scheduler working copy, replaceable through EditorTemplate.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerEditor<TItem> { }
