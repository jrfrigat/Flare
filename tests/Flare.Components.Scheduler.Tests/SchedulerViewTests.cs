using System.Globalization;
using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerViewTests : FlareTestContext
{
    [Fact]
    public void CalendarIndexesEventsOnceWhenRenderingTwelveMonths()
    {
        var events = new CountingEvents([new("One", new(2026, 11, 4))]);
        Render<FlareCalendar>(p => p.Add(x => x.ViewDate, new DateOnly(2026, 1, 1))
            .Add(x => x.MonthCount, 12).Add(x => x.Events, events));
        Assert.Equal(1, events.Enumerations);
    }
    private sealed class CountingEvents(CalendarEvent[] events) : IReadOnlyList<CalendarEvent>
    {
        public int Enumerations { get; private set; }
        public int Count => events.Length;
        public CalendarEvent this[int index] => events[index];
        public IEnumerator<CalendarEvent> GetEnumerator() { Enumerations++; return ((IEnumerable<CalendarEvent>)events).GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
    [Fact]
    public void FallBackGridDisplaysBothOffsetsAndPositiveCrossFoldDuration()
    {
        var item = Event with { Start = new(2026, 11, 1, 1, 30, 0, TimeSpan.FromHours(-4)),
            End = new(2026, 11, 1, 1, 15, 0, TimeSpan.FromHours(-5)) };
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { item }).Add(x => x.Date, new DateOnly(2026, 11, 1))
            .Add(x => x.View, SchedulerView.Day).Add(x => x.TimeZone, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")));
        Assert.Equal(50, cut.FindAll($".{Css.Classes.Scheduler.Slot}").Count);
        Assert.Contains("-04:00", cut.Markup);
        Assert.Contains("-05:00", cut.Markup);
        Assert.Contains("*1.5)", cut.Find($".{Css.Classes.Scheduler.Event}").GetAttribute("style"));
    }
    private static readonly DateOnly Date = new(2026, 11, 4);
    private static readonly SchedulerEventData Event = new()
    {
        Key = "one", Title = "Planning", Start = new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero),
        End = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero)
    };
    public SchedulerViewTests()
    {
        Services.AddFlareScheduler();
        Services.AddSingleton<Flare.Abstractions.IDialogService, Flare.Infrastructure.DialogService>();
    }

    [Theory]
    [InlineData(SchedulerView.Day, 1)]
    [InlineData(SchedulerView.Week, 7)]
    [InlineData(SchedulerView.WorkWeek, 5)]
    [InlineData(SchedulerView.Month, 0)]
    [InlineData(SchedulerView.Year, 0)]
    [InlineData(SchedulerView.Agenda, 0)]
    [InlineData(SchedulerView.Timeline, 0)]
    public void EveryProjectionRetainsApplicationEvents(SchedulerView view, int columns)
    {
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Event }).Add(x => x.Date, Date).Add(x => x.View, view)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc).Add(x => x.Culture, CultureInfo.GetCultureInfo("en-GB")));
        if (view != SchedulerView.Timeline) Assert.Contains("Planning", cut.Markup);
        Assert.Same(Event, Assert.Single(cut.Instance.Snapshot.Occurrences).Item);
        Assert.Equal(columns, cut.FindAll($".{Css.Classes.Scheduler.Day}").Count);
        if (view is SchedulerView.Month or SchedulerView.Year)
            Assert.Equal(view == SchedulerView.Year ? 12 : 1, cut.FindAll($".{Css.Classes.Calendar.Month}").Count);
        if (view == SchedulerView.Timeline) Assert.Single(cut.FindComponents<FlareGantt>());
    }

    [Fact]
    public async Task UncontrolledNavigationSurvivesParentRenderAndControlledNavigationCanBeVetoed()
    {
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Day));
        await cut.InvokeAsync(() => cut.Instance.NavigateAsync(1));
        cut.Render();
        Assert.Equal(Date.AddDays(1), cut.Instance.Snapshot.Date);
        cut.Render(p => p.Add(x => x.DateChanged, (DateOnly _) => { }));
        await cut.InvokeAsync(() => cut.Instance.NavigateAsync(1));
        Assert.Equal(Date, cut.Instance.Snapshot.Date);
    }

    [Fact]
    public async Task StaleProviderResultCannotReplaceANewerWindow()
    {
        var old = new TaskCompletionSource<IEnumerable<SchedulerEventData>>();
        var requests = new List<SchedulerDataRequest>();
        Task<IEnumerable<SchedulerEventData>> Provider(SchedulerDataRequest request)
        {
            requests.Add(request);
            return requests.Count == 1 ? Task.FromResult<IEnumerable<SchedulerEventData>>([])
                : requests.Count == 2 ? old.Task : Task.FromResult<IEnumerable<SchedulerEventData>>([Event]);
        }
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Week).Add(x => x.EventsProvider, Provider)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc));
        Task? pending = null;
        await cut.InvokeAsync(() => { pending = cut.Instance.ReloadAsync(); });
        cut.WaitForAssertion(() => Assert.Contains("Loading", cut.Markup));
        await cut.InvokeAsync(() => cut.Instance.ReloadAsync());
        Assert.True(requests[1].CancellationToken.IsCancellationRequested);
        old.SetResult([]);
        await pending!;
        Assert.Same(Event, Assert.Single(cut.Instance.Snapshot.Occurrences).Item);
    }

    [Fact]
    public void FiftyThousandItemsAreWindowedBeforeRendering()
    {
        var all = Enumerable.Range(0, 50000).Select(i => Event with
        {
            Key = i.ToString(CultureInfo.InvariantCulture), Start = Event.Start.AddDays(i), End = Event.End.AddDays(i)
        }).ToArray();
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, all).Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Day)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc));
        Assert.Single(cut.FindAll($".{Css.Classes.Scheduler.EventBody}"));
        Assert.Single(cut.Instance.Snapshot.Occurrences);
    }

    [Fact]
    public void CalendarControlledMonthAndTemplatePreserveExistingEventSelection()
    {
        CalendarEvent? selected = null;
        var item = new CalendarEvent("Task", Date);
        var cut = Render<FlareCalendar>(p => p.Add(x => x.ViewDate, Date).Add(x => x.Events, new[] { item })
            .Add(x => x.ShowNavigation, false).Add(x => x.OnEventClick, (CalendarEvent value) => selected = value)
            .Add(x => x.EventTemplate, value => builder => builder.AddContent(0, "Custom " + value.Title)));
        Assert.Contains("Custom Task", cut.Markup);
        cut.Find($".{Css.Classes.Calendar.Event}").KeyDown("Enter");
        Assert.Same(item, selected);
        cut.Render(p => p.Add(x => x.ViewDate, Date.AddMonths(1)));
        Assert.Empty(cut.FindAll($".{Css.Classes.Calendar.Event}"));
    }
}
