using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

/// <summary>
/// The month projection is a scheduler surface: it fills the width, shows the start time on the day
/// an event begins, and keeps every event of a day in the cell instead of truncating them.
/// </summary>
public sealed class SchedulerMonthViewTests : FlareTestContext
{
    private static readonly DateOnly Date = new(2026, 11, 4);

    public SchedulerMonthViewTests() => Services.AddFlareScheduler();

    private IRenderedComponent<FlareScheduler<SchedulerEventData>> Month(params SchedulerEventData[] events)
        => Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, events)
            .Add(x => x.Date, Date)
            .Add(x => x.View, SchedulerView.Month)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.OnEditRequested, _ => { }));

    private static SchedulerEventData Timed(string key, string title, int startHour,
        int endHour = 10, int startDay = 4, int endDay = 4) => new()
        {
            Key = key,
            Title = title,
            Start = new(2026, 11, startDay, startHour, 0, 0, TimeSpan.Zero),
            End = new(2026, 11, endDay, endHour, 0, 0, TimeSpan.Zero),
        };

    [Fact]
    public void MonthEvent_ShowsItsStartTime()
    {
        var cut = Month(Timed("one", "Planning", 9));

        var time = cut.Find(".flare-scheduler__event-time");
        Assert.Equal("09:00", time.TextContent.Trim());
        Assert.Contains("Planning", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiDayEvent_ShowsTimeOnlyOnItsFirstDay()
    {
        var cut = Month(Timed("multi", "Conference", 9, 10, 4, 6));

        Assert.Equal(3, cut.FindAll("." + Flare.Css.Classes.Calendar.Event).Count);
        Assert.Single(cut.FindAll(".flare-scheduler__event-time"));
    }

    [Fact]
    public void AllDayEvent_HasNoTime()
    {
        var item = new SchedulerEventData
        {
            Key = "all-day",
            Title = "Holiday",
            AllDay = new SchedulerDateRange(new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 5)),
        };

        var cut = Month(item);

        Assert.Contains("Holiday", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".flare-scheduler__event-time"));
    }

    [Fact]
    public void MonthAndYear_TakeTheirOwnSurfaceClasses()
    {
        var month = Month(Timed("one", "Planning", 9));
        Assert.Contains("flare-scheduler__month", month.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-scheduler__month--year", month.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);

        var year = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Timed("one", "Planning", 9) })
            .Add(x => x.Date, Date)
            .Add(x => x.View, SchedulerView.Year)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.OnEditRequested, _ => { }));
        Assert.Contains("flare-scheduler__month--year", year.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void EveryEventOfADay_StaysInTheCell()
    {
        var events = Enumerable.Range(0, 5)
            .Select(i => Timed("e" + i, "Event " + i, 9 + i, 10 + i))
            .ToArray();

        var cut = Month(events);

        Assert.Equal(5, cut.FindAll("." + Flare.Css.Classes.Calendar.Event).Count);
    }
}
