using System.Globalization;

namespace Flare.Components.Scheduler;

/// <summary>Windowed projections over application events, with optional local recurrence expansion.</summary>
public static class SchedulerOccurrences
{
    /// <summary>Projects ready occurrences, or expands series when a recurrence adapter is supplied. Limits are
    /// explicit failures, never silent truncation. The supplied items and their models remain unchanged.</summary>
    public static IReadOnlyList<SchedulerOccurrence<TItem>> Resolve<TItem>(IEnumerable<TItem> items,
        SchedulerMapping<TItem> mapping, SchedulerWindow window, int maximumOccurrences = 10000,
        CancellationToken cancellationToken = default, int maximumEvaluationSteps = 100000,
        ISchedulerRecurrenceExpander? recurrenceExpander = null)
    {
        if (maximumOccurrences < 1) throw new ArgumentOutOfRangeException(nameof(maximumOccurrences));
        if (maximumEvaluationSteps < 1) throw new ArgumentOutOfRangeException(nameof(maximumEvaluationSteps));
        if (window.End <= window.Start || window.Dates.End <= window.Dates.Start) throw new ArgumentException("Invalid window.", nameof(window));
        var projections = items.Select(item => (Item: item, Data: mapping.Project(item))).ToArray();
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (_, data) in projections)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Validate(data);
            if (!keys.Add(data.Key)) throw new ArgumentException("Event keys must be unique.", nameof(items));
        }
        var overrides = projections.Where(p => recurrenceExpander is not null && p.Data.SeriesKey is not null).ToArray();
        var identities = overrides.Select(p => p.Data.RecurrenceId!).ToHashSet(StringComparer.Ordinal);
        if (identities.Count != overrides.Length) throw new ArgumentException("Occurrence overrides must be unique.", nameof(items));
        var result = new List<SchedulerOccurrence<TItem>>();
        void Add(TItem item, SchedulerEventData data, string identity, SchedulerEventData series)
        {
            Validate(data);
            if (!Intersects(data, window)) return;
            if (result.Count >= maximumOccurrences) throw new InvalidOperationException("The occurrence limit was exceeded; request a smaller window or raise the limit.");
            result.Add(new(item, data, identity, series));
        }
        foreach (var (item, data) in projections.Where(p => recurrenceExpander is null || p.Data.SeriesKey is null))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (recurrenceExpander is null || (string.IsNullOrWhiteSpace(data.RecurrenceRule) && data.RecurrenceDates.Count == 0 && data.RecurrenceDays.Count == 0
                && data.ExceptionDates.Count == 0 && data.ExceptionDays.Count == 0))
            {
                var identity = data.RecurrenceId ?? data.Key;
                if (!identities.Contains(identity)) Add(item, data, identity, data with { Key = data.SeriesKey ?? data.Key });
                continue;
            }
            foreach (var occurrence in recurrenceExpander!.Expand(data, window, maximumEvaluationSteps, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var identity = Identity(data.Key, occurrence.Start, occurrence.AllDay?.Start);
                if (!identities.Contains(identity)) Add(item, occurrence, identity, data);
            }
        }
        foreach (var (item, data) in overrides)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var master = projections.FirstOrDefault(p => p.Data.Key == data.SeriesKey).Data;
            if (master is null) throw new ArgumentException("An override requires its recurrence master in the provider result.", nameof(items));
            Add(item, data, data.RecurrenceId!, master);
        }
        return result.OrderBy(e => e.Data.AllDay?.Start.ToDateTime(TimeOnly.MinValue) ?? e.Data.Start.UtcDateTime)
            .ThenBy(e => e.Identity, StringComparer.Ordinal).ToArray();
    }

    /// <summary>Stable occurrence identity; keep it unchanged when an exception is moved to another time.</summary>
    public static string Identity(string seriesKey, DateTimeOffset start, DateOnly? date = null) =>
        seriesKey + "/" + (date is { } day ? day.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            : start.UtcDateTime.ToString("yyyyMMdd'T'HHmmssfffffff'Z'", CultureInfo.InvariantCulture));

    private static void Validate(SchedulerEventData data)
    {
        if (string.IsNullOrWhiteSpace(data.Key)) throw new ArgumentException("An event key is required.");
        if (data.AllDay is { } dates ? dates.End <= dates.Start : data.End < data.Start)
            throw new ArgumentException("An event end must not precede its start; all-day events require at least one date.");
        if ((data.SeriesKey is null) != (data.RecurrenceId is null)) throw new ArgumentException("An override requires SeriesKey and RecurrenceId.");
    }

    private static bool Intersects(SchedulerEventData data, SchedulerWindow window) => data.AllDay is { } days
        ? days.Start < window.Dates.End && days.End > window.Dates.Start
        : data.Start == data.End ? data.Start >= window.Start && data.Start < window.End
        : data.Start < window.End && data.End > window.Start;

}
