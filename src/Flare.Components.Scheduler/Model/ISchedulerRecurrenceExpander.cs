namespace Flare.Components.Scheduler;

/// <summary>Optional local recurrence adapter. Without one, the UI projects ready occurrences supplied by the application/provider.</summary>
public interface ISchedulerRecurrenceExpander
{
    /// <summary>Expands one series for a visible window without changing the model. Excludes invalid dates,
    /// honors series time zones and raises an error instead of silently exceeding an evaluation limit.</summary>
    IEnumerable<SchedulerEventData> Expand(SchedulerEventData data, SchedulerWindow window,
        int maximumEvaluationSteps, CancellationToken cancellationToken);
}
