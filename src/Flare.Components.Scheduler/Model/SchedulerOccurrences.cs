using System.Globalization;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;
using IcalEvent = Ical.Net.CalendarComponents.CalendarEvent;

namespace Flare.Components.Scheduler;

/// <summary>Windowed RFC 5545 expansion. No Ical.Net types cross this public boundary.</summary>
public static class SchedulerOccurrences
{
    /// <summary>Expands events, exclusions, additional dates and overrides for one visible window. Limits are
    /// explicit failures, never silent truncation. The supplied items and their models remain unchanged.</summary>
    public static IReadOnlyList<SchedulerOccurrence<TItem>> Expand<TItem>(IEnumerable<TItem> items,
        SchedulerMapping<TItem> mapping, SchedulerWindow window, int maximumOccurrences = 10000,
        CancellationToken cancellationToken = default, int maximumEvaluationSteps = 100000)
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
        var overrides = projections.Where(p => p.Data.SeriesKey is not null).ToArray();
        var identities = overrides.Select(p => p.Data.RecurrenceId!).ToHashSet(StringComparer.Ordinal);
        if (identities.Count != overrides.Length) throw new ArgumentException("Occurrence overrides must be unique.", nameof(items));
        var result = new List<SchedulerOccurrence<TItem>>();
        void Add(TItem item, SchedulerEventData data, string identity, SchedulerEventData series)
        {
            if (!Intersects(data, window)) return;
            if (result.Count >= maximumOccurrences) throw new InvalidOperationException("The occurrence limit was exceeded; request a smaller window or raise the limit.");
            result.Add(new(item, data, identity, series));
        }
        foreach (var (item, data) in projections.Where(p => p.Data.SeriesKey is null))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(data.RecurrenceRule) && data.RecurrenceDates.Count == 0 && data.RecurrenceDays.Count == 0
                && data.ExceptionDates.Count == 0 && data.ExceptionDays.Count == 0)
            {
                var identity = Identity(data.Key, data.Start, data.AllDay?.Start);
                if (!identities.Contains(identity)) Add(item, data, identity, data);
                continue;
            }
            foreach (var occurrence in ExpandSeries(data, window, maximumEvaluationSteps, cancellationToken))
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

    private static IEnumerable<SchedulerEventData> ExpandSeries(SchedulerEventData data, SchedulerWindow window,
        int maximumEvaluationSteps, CancellationToken cancellationToken)
    {
        var allDay = data.AllDay;
        var zone = TimeZoneInfo.FindSystemTimeZoneById(data.TimeZoneId ?? "UTC");
        // Expand calendar rules on the wall clock, then resolve valid instants ourselves. The dependency
        // otherwise shifts a missing DST time forward before the caller can exclude it.
        CalDateTime Timed(DateTimeOffset at) => new(TimeZoneInfo.ConvertTime(at, zone).DateTime);
        CalDateTime Day(DateOnly at) => new(at.ToDateTime(TimeOnly.MinValue), false);
        var ev = new IcalEvent
        {
            DtStart = allDay is { } dates ? Day(dates.Start) : Timed(data.Start),
            DtEnd = allDay is { } range ? Day(range.End) : Timed(data.End)
        };
        var pattern = string.IsNullOrWhiteSpace(data.RecurrenceRule) ? null : new RecurrencePattern(data.RecurrenceRule);
        var count = pattern?.Count;
        var ruleUntil = pattern?.Until;
        if (count is not null && ruleUntil is not null) throw new ArgumentException("RRULE cannot combine COUNT and UNTIL.");
        if (allDay is null && ruleUntil is { IsUtc: false }) throw new ArgumentException("Timed UNTIL must be UTC.");
        if (pattern is not null) { pattern.Count = null; pattern.Until = null; ev.RecurrenceRule = pattern; }
        var duration = allDay is { } d ? TimeSpan.FromDays(d.End.DayNumber - d.Start.DayNumber) : data.End - data.Start;
        var from = allDay is not null
            ? Day(window.Dates.Start.AddDays(-Math.Min(window.Dates.Start.DayNumber, Math.Max(0, (int)duration.TotalDays))))
            : Timed(window.Start.AddTicks(-Math.Min(window.Start.UtcTicks, duration.Ticks)));
        var endClock = Timed(window.End);
        // On a repeated clock hour the later window end can precede an earlier instant on the wall clock.
        if (allDay is null && zone.IsAmbiguousTime(endClock.Value)) endClock = new(endClock.Value.AddHours(2));
        var until = allDay is not null ? Day(window.Dates.End) : endClock;
        var options = new EvaluationOptions { MaxUnmatchedIncrementsLimit = 10000 };
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var valid = 0;
        var evaluated = 0;
        var candidates = ev.GetOccurrences(count is null ? from : ev.DtStart, options).TakeWhileBefore(until)
            .Select(o => o.Period.StartTime);
        IEnumerable<SchedulerEventData> RuleDates()
        {
            foreach (var at in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (++evaluated > maximumEvaluationSteps) throw new InvalidOperationException("The recurrence evaluation limit was exceeded.");
                if (allDay is null && zone.IsInvalidTime(at.Value)) continue;
                if (count is { } limit && valid >= limit) yield break;
                if (allDay is not null)
                {
                    var date = DateOnly.FromDateTime(at.Value);
                    if (ruleUntil is not null && date > DateOnly.FromDateTime(ruleUntil.Value)) yield break;
                    valid++;
                    yield return data with { AllDay = new(date, date.AddDays((int)duration.TotalDays)) };
                    continue;
                }
                var start = SchedulerClock.At(DateOnly.FromDateTime(at.Value), TimeOnly.FromDateTime(at.Value), zone);
                if (ruleUntil is not null && start.UtcDateTime > ruleUntil.AsUtc) yield break;
                valid++;
                yield return data with { Start = start, End = start + duration };
            }
        }
        var additional = allDay is not null
            ? data.RecurrenceDays.Select(date => data with { AllDay = new(date, date.AddDays((int)duration.TotalDays)) })
            : data.RecurrenceDates.Select(start => data with { Start = start, End = start + duration });
        foreach (var occurrence in RuleDates().Concat(additional))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (occurrence.AllDay is { } occurrenceDays)
            {
                if (data.ExceptionDays.Contains(occurrenceDays.Start) || !seen.Add(Identity(data.Key, default, occurrenceDays.Start))) continue;
            }
            else
            {
                if (data.ExceptionDates.Contains(occurrence.Start) || !seen.Add(Identity(data.Key, occurrence.Start))) continue;
            }
            yield return occurrence;
        }
    }
}
