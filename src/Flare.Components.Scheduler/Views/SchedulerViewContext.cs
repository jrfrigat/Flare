using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>One immutable view snapshot. Custom views use the same events and owner callbacks as built-in projections.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public sealed record SchedulerViewContext<TItem>
{
    /// <summary>Host providing navigation, selection and edit callbacks.</summary>
    public required FlareScheduler<TItem> Owner { get; init; }
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
