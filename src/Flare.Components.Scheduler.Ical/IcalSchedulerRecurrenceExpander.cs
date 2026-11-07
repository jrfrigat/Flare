using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;
using IcalEvent = Ical.Net.CalendarComponents.CalendarEvent;

namespace Flare.Components.Scheduler.Ical;

/// <summary>Optional local RFC 5545 expansion for standalone clients. Server-backed schedulers can supply ready occurrences instead.</summary>
public sealed class IcalSchedulerRecurrenceExpander : ISchedulerRecurrenceExpander
{
    /// <summary>Stateless reusable adapter instance.</summary>
    public static IcalSchedulerRecurrenceExpander Default { get; } = new();

    /// <inheritdoc />
    public IEnumerable<SchedulerEventData> Expand(SchedulerEventData data, SchedulerWindow window,
        int maximumEvaluationSteps, CancellationToken cancellationToken)
    {
        if (maximumEvaluationSteps < 1) throw new ArgumentOutOfRangeException(nameof(maximumEvaluationSteps));
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
        var options = new EvaluationOptions { MaxUnmatchedIncrementsLimit = maximumEvaluationSteps };
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
                if (data.ExceptionDays.Contains(occurrenceDays.Start) || !seen.Add(SchedulerOccurrences.Identity(data.Key, default, occurrenceDays.Start))) continue;
            }
            else
            {
                if (data.ExceptionDates.Contains(occurrence.Start) || !seen.Add(SchedulerOccurrences.Identity(data.Key, occurrence.Start))) continue;
            }
            yield return occurrence;
        }
    }
}

