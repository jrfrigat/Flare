using System.Globalization;
using Flare.Components.Scheduler;

namespace Flare.Components.Scheduler.Ical.Tests;

public sealed class SchedulerModelTests
{
    public static readonly SchedulerMapping<SchedulerEventData> Mapping = new()
    {
        Key = e => e.Key, Title = e => e.Title, Start = e => e.Start, End = e => e.End, AllDay = e => e.AllDay,
        TimeZoneId = e => e.TimeZoneId, RecurrenceRule = e => e.RecurrenceRule, ExceptionDates = e => e.ExceptionDates,
        ExceptionDays = e => e.ExceptionDays, RecurrenceDates = e => e.RecurrenceDates, RecurrenceDays = e => e.RecurrenceDays,
        SeriesKey = e => e.SeriesKey, RecurrenceId = e => e.RecurrenceId, ResourceKey = e => e.ResourceKey
    };
    private static readonly TimeZoneInfo NewYork = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
    private static DateTimeOffset At(int year, int month, int day, int hour = 9) =>
        SchedulerClock.At(new(year, month, day), new(hour, 0), NewYork);
    private static SchedulerWindow Window(DateOnly from, DateOnly to, TimeZoneInfo? zone = null) =>
        SchedulerClock.Window(new(from, to), zone ?? NewYork);
    private static SchedulerEventData Event(DateTimeOffset at, string? rule = null) => new()
    { Key = "series", Title = "Meeting", Start = at, End = at.AddHours(1), TimeZoneId = NewYork.Id, RecurrenceRule = rule };
    private static IReadOnlyList<SchedulerOccurrence<SchedulerEventData>> Expand(IEnumerable<SchedulerEventData> events,
        SchedulerWindow window, int maximum = 10000) => SchedulerOccurrences.Resolve(events, Mapping, window, maximum, Xunit.TestContext.Current.CancellationToken, recurrenceExpander: IcalSchedulerRecurrenceExpander.Default);

    [Fact]
    public void RecurrenceSkipsTheNonexistentSpringTime()
    {
        var data = Event(SchedulerClock.At(new(2026, 3, 7), new(2, 30), NewYork), "FREQ=DAILY;COUNT=3");
        var result = Expand([data], Window(new(2026, 3, 7), new(2026, 3, 12)));
        Assert.Equal(new[] { 7, 9, 10 }, result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).Day));
    }

    // RFC 5545 section 3.8.5.3, every other week on Tuesday and Thursday, eight occurrences.
    [Fact]
    public void PublishedWeeklyVectorKeepsDatesAcrossDst()
    {
        var data = Event(At(1997, 9, 2), "FREQ=WEEKLY;INTERVAL=2;COUNT=8;WKST=SU;BYDAY=TU,TH");
        var result = Expand([data], Window(new(1997, 9, 1), new(1997, 11, 1)));
        Assert.Equal(new[] { "09-02", "09-04", "09-16", "09-18", "09-30", "10-02", "10-14", "10-16" },
            result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).ToString("MM-dd", CultureInfo.InvariantCulture)));
    }

    // RFC 5545 section 3.8.5.3, first Friday of each month.
    [Fact]
    public void PublishedOrdinalMonthlyVectorPreservesWallTimeThroughDst()
    {
        var data = Event(At(1997, 9, 5), "FREQ=MONTHLY;COUNT=4;BYDAY=1FR");
        var result = Expand([data], Window(new(1997, 9, 1), new(1998, 1, 1)));
        Assert.Equal(new[] { "09-05", "10-03", "11-07", "12-05" },
            result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).ToString("MM-dd", CultureInfo.InvariantCulture)));
        Assert.All(result, e => Assert.Equal(9, TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).Hour));
        Assert.Equal(new[] { -4d, -4d, -5d, -5d }, result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).Offset.TotalHours));
    }

    [Fact]
    public void MonthEndRulesSkipInvalidDatesInsteadOfClamping()
    {
        var data = Event(At(2026, 1, 31), "FREQ=MONTHLY;COUNT=3;BYMONTHDAY=31");
        var result = Expand([data], Window(new(2026, 1, 1), new(2026, 7, 1)));
        Assert.Equal(new[] { 1, 3, 5 }, result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).Month));
    }

    [Fact]
    public void ExclusionsAdditionalDatesAndMovedOverridesKeepIdentity()
    {
        var data = Event(At(2026, 11, 2), "FREQ=DAILY;COUNT=3") with
        { ExceptionDates = [At(2026, 11, 3)], RecurrenceDates = [At(2026, 11, 6)] };
        var identity = SchedulerOccurrences.Identity(data.Key, At(2026, 11, 4));
        var exception = Event(At(2026, 11, 5)) with { Key = "exception", SeriesKey = data.Key, RecurrenceId = identity };
        var result = Expand([data, exception], Window(new(2026, 11, 1), new(2026, 11, 8)));
        Assert.Equal(new[] { 2, 5, 6 }, result.Select(e => TimeZoneInfo.ConvertTime(e.Data.Start, NewYork).Day));
        Assert.Equal(identity, result[1].Identity);
        Assert.Same(exception, result[1].Item);
        Assert.Equal(data.Key, result[1].Series.Key);
    }

    [Fact]
    public void AllDayDatesDoNotMoveWhenTheDisplayZoneChanges()
    {
        var data = new SchedulerEventData { Key = "all-day", Title = "Holiday", AllDay = new(new(2026, 11, 1), new(2026, 11, 3)),
            RecurrenceRule = "FREQ=WEEKLY;COUNT=2", ExceptionDays = [new(2026, 11, 8)] };
        var zone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Kiritimati");
        var result = Expand([data], Window(new(2026, 11, 1), new(2026, 11, 15), zone));
        var only = Assert.Single(result);
        Assert.Equal(data.AllDay, only.Data.AllDay);
        Assert.Equal(new DateTime(2026, 11, 1), SchedulerClock.Span(only.Data, zone).Start);
    }

    [Fact]
    public void LongEventsBeginningBeforeTheWindowAreIncludedAndLimitsAreExplicit()
    {
        var data = Event(At(2026, 11, 1), "FREQ=DAILY;COUNT=20") with { End = At(2026, 11, 3) };
        var result = Expand([data], Window(new(2026, 11, 3), new(2026, 11, 4)));
        Assert.Equal(3, result.Count);
        Assert.Throws<InvalidOperationException>(() => Expand([data],
            Window(new(2026, 11, 1), new(2026, 12, 1)), maximum: 2));
        using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
        Assert.Throws<OperationCanceledException>(() => SchedulerOccurrences.Resolve([data], Mapping,
            Window(new(2026, 11, 1), new(2026, 12, 1)), cancellationToken: cancellation.Token, recurrenceExpander: IcalSchedulerRecurrenceExpander.Default));
    }

    [Fact]
    public void ExistingGanttLanesHandleIdenticalNestedChainedAndPointEvents()
    {
        var at = At(2026, 11, 3);
        var data = new[] { Event(at) with { Key = "a", End = at.AddHours(4) },
            Event(at) with { Key = "b", End = at.AddHours(4) }, Event(at.AddHours(1)) with { Key = "c" },
            Event(at.AddHours(4)) with { Key = "d" }, Event(at.AddHours(5)) with { Key = "p", End = at.AddHours(5) } };
        var events = Expand(data, Window(new(2026, 11, 3), new(2026, 11, 4)));
        var lanes = SchedulerClock.Lanes(events, NewYork);
        Assert.Equal(3, lanes.Count);
        Assert.Equal(0, lanes.Lanes[events.Single(e => e.Data.Key == "d").Identity]);
    }

    [Fact]
    public void AmbiguousTimesRetainAnExplicitOffsetAndInvalidTimesAreRejected()
    {
        Assert.Throws<ArgumentException>(() => SchedulerClock.At(new(2026, 3, 8), new(2, 30), NewYork));
        var early = SchedulerClock.At(new(2026, 11, 1), new(1, 30), NewYork);
        var late = SchedulerClock.At(new(2026, 11, 1), new(1, 30), NewYork, TimeSpan.FromHours(-5));
        Assert.Equal(TimeSpan.FromHours(1), late - early);
    }

    [Fact]
    public void CountIncludesExcludedDatesAndUntilIsInclusiveInALaterWindow()
    {
        var data = Event(At(2026, 11, 1), "FREQ=DAILY;COUNT=3") with { ExceptionDates = [At(2026, 11, 2)] };
        Assert.Equal(2, Expand([data], Window(new(2026, 11, 1), new(2026, 11, 8))).Count);
        Assert.Empty(Expand([data], Window(new(2026, 11, 4), new(2026, 11, 8))));
        var until = data with { RecurrenceRule = "FREQ=DAILY;UNTIL=20261103T140000Z", ExceptionDates = [] };
        Assert.Equal(3, Expand([until], Window(new(2026, 11, 1), new(2026, 11, 8))).Count);
    }

    [Fact]
    public void RDatePreservesTheExplicitLateOffsetInAnAmbiguousHour()
    {
        var late = SchedulerClock.At(new(2026, 11, 1), new(1, 30), NewYork, TimeSpan.FromHours(-5));
        var early = SchedulerClock.At(new(2026, 11, 1), new(1, 30), NewYork);
        var data = Event(early) with { RecurrenceDates = [late] };
        var result = Expand([data], Window(new(2026, 11, 1), new(2026, 11, 2)));
        Assert.Equal(new[] { early, late }, result.Select(e => e.Data.Start));
    }
}
