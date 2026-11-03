namespace Flare.Components.Scheduler;

/// <summary>A half-open date range for an all-day event, independent of time zones.</summary>
/// <param name="Start">First included date.</param>
/// <param name="End">First excluded date; must follow Start.</param>
public readonly record struct SchedulerDateRange(DateOnly Start, DateOnly End);

/// <summary>An immutable projection of an application's event. The scheduler never writes to the original item.</summary>
public sealed record SchedulerEventData
{
    /// <summary>Stable, unique application key.</summary>
    public required string Key { get; init; }
    /// <summary>Event caption.</summary>
    public required string Title { get; init; }
    /// <summary>Inclusive timed start; ignored when AllDay is supplied.</summary>
    public DateTimeOffset Start { get; init; }
    /// <summary>Exclusive timed end; equal to Start for a point.</summary>
    public DateTimeOffset End { get; init; }
    /// <summary>Date-only span; null for a timed event.</summary>
    public SchedulerDateRange? AllDay { get; init; }
    /// <summary>Series time zone (IANA or Windows id). Null repeats on UTC; display-zone changes never move the series.</summary>
    public string? TimeZoneId { get; init; }
    /// <summary>Resource identity, or null for an unassigned event.</summary>
    public string? ResourceKey { get; init; }
    /// <summary>Optional event details.</summary>
    public string? Description { get; init; }
    /// <summary>Theme role or custom event accent.</summary>
    public FlareColor Color { get; init; }
    /// <summary>RFC 5545 RRULE value, without the RRULE: prefix; null for one event.</summary>
    public string? RecurrenceRule { get; init; }
    /// <summary>EXDATE instants, compared by UTC instant.</summary>
    public IReadOnlyList<DateTimeOffset> ExceptionDates { get; init; } = [];
    /// <summary>EXDATE dates for an all-day series.</summary>
    public IReadOnlyList<DateOnly> ExceptionDays { get; init; } = [];
    /// <summary>Additional RDATE instants.</summary>
    public IReadOnlyList<DateTimeOffset> RecurrenceDates { get; init; } = [];
    /// <summary>Additional RDATE dates for an all-day series.</summary>
    public IReadOnlyList<DateOnly> RecurrenceDays { get; init; } = [];
    /// <summary>Master key when this item overrides one occurrence; null for a master or standalone event.</summary>
    public string? SeriesKey { get; init; }
    /// <summary>Opaque occurrence identity copied from SchedulerOccurrence.Identity when creating an override.</summary>
    public string? RecurrenceId { get; init; }
}

/// <summary>A visible occurrence retaining its original application item and recurrence identity.</summary>
/// <typeparam name="TItem">Application model.</typeparam>
/// <param name="Item">Original item; an override item when one is present.</param>
/// <param name="Data">Projected occurrence dates and presentation.</param>
/// <param name="Identity">Stable series key plus original UTC start or all-day date.</param>
/// <param name="Series">Original master projection, used to describe edits.</param>
public sealed record SchedulerOccurrence<TItem>(TItem Item, SchedulerEventData Data, string Identity, SchedulerEventData Series);

/// <summary>Visible half-open instant window plus its date-only bounds in the display zone.</summary>
/// <param name="Start">Inclusive instant.</param>
/// <param name="End">Exclusive instant.</param>
/// <param name="Dates">Displayed date range.</param>
public sealed record SchedulerWindow(DateTimeOffset Start, DateTimeOffset End, SchedulerDateRange Dates);

/// <summary>A row of the resource timeline.</summary>
/// <param name="Key">Stable resource identity.</param>
/// <param name="Title">Resource caption.</param>
public sealed record SchedulerResource(string Key, string Title);

/// <summary>Provider request. Include intersecting events, relevant recurrence masters and their overrides.</summary>
/// <param name="Window">Visible range; data outside it is unnecessary except recurrence masters.</param>
/// <param name="CancellationToken">Canceled when a newer range is requested or the component is disposed.</param>
public sealed record SchedulerDataRequest(SchedulerWindow Window, CancellationToken CancellationToken);
