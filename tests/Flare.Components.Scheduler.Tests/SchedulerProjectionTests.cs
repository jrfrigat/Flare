using Flare.Components.Scheduler;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerProjectionTests
{
    private static readonly SchedulerWindow Window = SchedulerClock.Window(new(new(2026, 11, 1), new(2026, 11, 8)), TimeZoneInfo.Utc);
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void BackendOccurrencesNeedNeitherAMasterNorAnRRuleParser()
    {
        var data = new SchedulerEventData { Key = "instance", Title = "Meeting", Start = Start, End = Start.AddHours(1),
            SeriesKey = "series", RecurrenceId = "server-instance-id", RecurrenceRule = "opaque backend metadata" };
        var result = SchedulerOccurrences.Resolve([data], SchedulerMappings.EventData, Window,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var only = Assert.Single(result);
        Assert.Same(data, only.Item);
        Assert.Equal("server-instance-id", only.Identity);
        Assert.Equal("series", only.Series.Key);
        Assert.Equal(data.Start, only.Data.Start);
    }

    [Fact]
    public void RecurrenceMetadataDoesNotGenerateExtraEventsByDefault()
    {
        var data = new SchedulerEventData { Key = "event", Title = "Meeting", Start = Start, End = Start.AddHours(1), RecurrenceRule = "FREQ=DAILY" };
        Assert.Single(SchedulerOccurrences.Resolve([data], SchedulerMappings.EventData, Window,
            cancellationToken: Xunit.TestContext.Current.CancellationToken));
    }

    [Fact]
    public void OwnModelsKeepTheirIdentityAndAreFilteredByTheWindow()
    {
        var inside = new Appointment(42, Start);
        var outside = new Appointment(43, Start.AddMonths(1));
        var mapping = new SchedulerMapping<Appointment> { Key = e => e.Id.ToString(), Title = _ => "Appointment", Start = e => e.At, End = e => e.At.AddHours(1) };
        var result = SchedulerOccurrences.Resolve([inside, outside], mapping, Window,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Same(inside, Assert.Single(result).Item);
    }

    private sealed record Appointment(int Id, DateTimeOffset At);
}
