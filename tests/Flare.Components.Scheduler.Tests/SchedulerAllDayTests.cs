using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerAllDayTests : FlareTestContext
{
    private static readonly DateOnly Monday = new(2026, 11, 2);
    private static readonly DateOnly[] Dates = Enumerable.Range(0, 7).Select(Monday.AddDays).ToArray();
    private static SchedulerEventData Data(string key, int first, int end) => new()
    { Key = key, Title = key, AllDay = new(Monday.AddDays(first), Monday.AddDays(end)) };
    private static SchedulerOccurrence<SchedulerEventData> Occurrence(string key, int first, int end)
    {
        var data = Data(key, first, end);
        return new(data, data, key, data);
    }
    public SchedulerAllDayTests() => Services.AddFlareScheduler();

    [Fact]
    public void MultiDaySegmentsUseSharedLanesAndExclusiveEnds()
    {
        var segments = SchedulerAllDayLayout.Arrange(
            new[] { Occurrence("long", -2, 3), Occurrence("overlap", 1, 4), Occurrence("touch", 3, 5) }, Dates);
        var longest = segments.Single(s => s.Occurrence.Identity == "long");
        var overlap = segments.Single(s => s.Occurrence.Identity == "overlap");
        var touching = segments.Single(s => s.Occurrence.Identity == "touch");
        Assert.Equal((0, 3), (longest.Column, longest.ColumnCount));
        Assert.NotEqual(longest.Lane, overlap.Lane);
        Assert.Equal(longest.Lane, touching.Lane);
        Assert.Equal((3, 2), (touching.Column, touching.ColumnCount));
    }

    [Fact]
    public void HiddenWorkWeekDatesDoNotInventColumnsOrEvents()
    {
        var dates = new[] { Monday.AddDays(4), Monday.AddDays(7), Monday.AddDays(8) };
        var segments = SchedulerAllDayLayout.Arrange(new[]
        { Occurrence("weekend", 5, 7), Occurrence("through", 4, 8), Occurrence("next", 8, 9) }, dates);
        Assert.DoesNotContain(segments, s => s.Occurrence.Identity == "weekend");
        Assert.Equal((0, 2), (segments[0].Column, segments[0].ColumnCount));
        Assert.Equal((2, 1), (segments[1].Column, segments[1].ColumnCount));
        Assert.Equal(segments[0].Lane, segments[1].Lane);
        Assert.Empty(SchedulerAllDayLayout.Arrange(new[] { Occurrence("out", 7, 8) }, Dates));
        Assert.Throws<ArgumentException>(() => SchedulerAllDayLayout.Arrange(new[] { Occurrence("a", 0, 1) }, new[] { Monday, Monday }));
    }

    private IRenderedComponent<FlareScheduler<SchedulerEventData>> Scope(params SchedulerEventData[] events) =>
        Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, events).Add(x => x.Date, Monday).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Scope")));

    [Fact]
    public void EmptyAndGrowingBandsKeepDateTargetsAndRenderOneMultiDayEvent()
    {
        var scope = Scope();
        var empty = Render<FlareSchedulerAllDay<SchedulerEventData>>(p => p
            .Add(x => x.Context, scope.Instance.Snapshot).Add(x => x.Dates, Dates));
        Assert.Equal(7, empty.FindAll(".flare-scheduler__all-day-slot").Count);
        Assert.Empty(empty.FindAll(".flare-scheduler__all-day-event"));
        var fullScope = Scope(Data("multi", 0, 4));
        empty.Render(p => p.Add(x => x.Context, fullScope.Instance.Snapshot));
        Assert.Single(empty.FindAll(".flare-scheduler__all-day-event"));
        Assert.Contains("span 4", empty.Find(".flare-scheduler__all-day-event").GetAttribute("style"));
        Assert.DoesNotContain("overflow:auto", empty.Find(".flare-scheduler__all-day").GetAttribute("style") ?? "");
    }

    [Fact]
    public void LimitedBandOffersAllHiddenEventsAndExpandsWithoutAnInternalScrollArea()
    {
        var scope = Scope(Data("a", 0, 2), Data("b", 0, 2), Data("c", 0, 2));
        var cut = Render<FlareSchedulerAllDay<SchedulerEventData>>(p => p
            .Add(x => x.Context, scope.Instance.Snapshot).Add(x => x.Dates, Dates).Add(x => x.MaxRows, 1));
        Assert.Single(cut.FindAll(".flare-scheduler__all-day-event"));
        Assert.Equal(2, cut.FindAll(".flare-scheduler__all-day-more").Count);
        cut.Find(".flare-scheduler__all-day-more button").Click();
        Assert.Equal(3, cut.FindAll(".flare-scheduler__all-day-event").Count);
        Assert.Empty(cut.FindAll(".flare-scheduler__all-day-more"));
    }

    [Fact]
    public void ApplicationCanOwnOverflowPresentationAndDateSelectionDoesNotOpenAnEditor()
    {
        var scope = Scope(Data("a", 0, 2), Data("b", 0, 2));
        SchedulerAllDayOverflow<SchedulerEventData>? seen = null;
        var cut = Render<FlareSchedulerAllDay<SchedulerEventData>>(p => p
            .Add(x => x.Context, scope.Instance.Snapshot).Add(x => x.Dates, Dates).Add(x => x.MaxRows, 0)
            .Add(x => x.OnOverflow, args => seen = args));
        cut.Find(".flare-scheduler__all-day-more button").Click();
        Assert.Equal(Monday, seen!.Date);
        Assert.Equal(2, seen.Occurrences.Count);
        Assert.Empty(cut.FindAll(".flare-scheduler__all-day-event"));
        cut.Find(".flare-scheduler__all-day-slot").Click();
        Assert.Equal(new SchedulerDateRange(Monday, Monday.AddDays(1)), scope.Instance.SelectedSlot!.AllDay);
        Assert.Null(scope.Instance.ActiveEdit);
    }
}
