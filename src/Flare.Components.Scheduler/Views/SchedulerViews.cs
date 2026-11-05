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

/// <summary>Independent event form over a scheduler editing session; usable inside any presentation.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerEditor<TItem> { }

/// <summary>Optional dialog presenter for an independent scheduler editing session.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerDialog<TItem> { }

/// <summary>Optional scheduling frame, state presentations and active projection switch.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerSurface<TItem> { }

/// <summary>Optional standard navigation and event commands, composable anywhere in a scheduling scope.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerToolbar<TItem> { }

/// <summary>Independent date-only event band with shared collision lanes and configurable overflow actions.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public partial class FlareSchedulerAllDay<TItem> { }
