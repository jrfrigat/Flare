using System.Globalization;
using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

/// <summary>
/// The month projection is a scheduler surface: a multi-day event is one continuous bar per week,
/// single-day events stay in the day cells, and the start time appears only where the event begins.
/// </summary>
public sealed class SchedulerMonthViewTests : FlareTestContext
{
    private static readonly DateOnly Date = new(2026, 11, 4);
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

    public SchedulerMonthViewTests() => Services.AddFlareScheduler();

    private IRenderedComponent<FlareScheduler<SchedulerEventData>> Month(params SchedulerEventData[] events)
        => RenderMonth(events, null);

    private IRenderedComponent<FlareScheduler<SchedulerEventData>> RenderMonth(
        SchedulerEventData[] events, Action<SchedulerOccurrence<SchedulerEventData>>? selected)
        => Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, events)
            .Add(x => x.Date, Date)
            .Add(x => x.View, SchedulerView.Month)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.Culture, Culture)
            .Add(x => x.OnEditRequested, _ => { })
            .Add(x => x.OnEventSelected, e => selected?.Invoke(e)));

    private static SchedulerEventData Timed(string key, string title, int startHour, int endHour = 10,
        int startDay = 4, int endDay = 4) => new()
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
    public void MultiDayEvent_IsOneBarWithTimeOnlyOnItsFirstDay()
    {
        var cut = Month(Timed("multi", "Conference", 9, 10, 4, 6));

        var bar = Assert.Single(cut.FindAll(".flare-scheduler__month-bar"));
        Assert.Contains("grid-column:4 / span 3", bar.GetAttribute("style"), StringComparison.Ordinal);
        Assert.Single(cut.FindAll(".flare-scheduler__event-time"));
        Assert.Empty(cut.FindAll("." + Flare.Css.Classes.Calendar.Event));
    }

    [Fact]
    public void MultiDayEvent_AcrossAWeekBoundary_ProducesTwoSegments()
    {
        var cut = Month(Timed("long", "Offsite", 9, 10, 4, 10));

        var bars = cut.FindAll(".flare-scheduler__month-bar");
        Assert.Equal(2, bars.Count);
        Assert.Contains("grid-row:1;grid-column:4 / span 4", bars[0].GetAttribute("style"), StringComparison.Ordinal);
        Assert.Contains("grid-row:1;grid-column:1 / span 3", bars[1].GetAttribute("style"), StringComparison.Ordinal);
        Assert.Single(cut.FindAll(".flare-scheduler__event-time"));
    }

    [Fact]
    public void OverlappingMultiDayEvents_TakeSeparateBands()
    {
        var cut = Month(
            Timed("first", "First", 9, 10, 4, 6),
            Timed("second", "Second", 11, 12, 4, 5));

        var bars = cut.FindAll(".flare-scheduler__month-bar");
        Assert.Equal(2, bars.Count);
        Assert.Contains("grid-row:1", bars[0].GetAttribute("style"), StringComparison.Ordinal);
        Assert.Contains("grid-row:2", bars[1].GetAttribute("style"), StringComparison.Ordinal);
    }

    [Fact]
    public void AllDayMultiDayEvent_IsABarWithoutTime()
    {
        var item = new SchedulerEventData
        {
            Key = "all-day",
            Title = "Holiday",
            AllDay = new SchedulerDateRange(new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 7)),
        };

        var cut = Month(item);

        Assert.Single(cut.FindAll(".flare-scheduler__month-bar"));
        Assert.Empty(cut.FindAll(".flare-scheduler__event-time"));
    }

    [Fact]
    public void AllDaySingleDayEvent_StaysAChipWithoutTime()
    {
        var item = new SchedulerEventData
        {
            Key = "one-day",
            Title = "Holiday",
            AllDay = new SchedulerDateRange(new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 5)),
        };

        var cut = Month(item);

        Assert.Empty(cut.FindAll(".flare-scheduler__month-bar"));
        Assert.Single(cut.FindAll("." + Flare.Css.Classes.Calendar.Event));
        Assert.Empty(cut.FindAll(".flare-scheduler__event-time"));
    }

    [Fact]
    public void SingleDayEvents_StayInTheCell()
    {
        var events = Enumerable.Range(0, 5)
            .Select(i => Timed("e" + i, "Event " + i, 9 + i, 10 + i))
            .ToArray();

        var cut = Month(events);

        Assert.Equal(5, cut.FindAll("." + Flare.Css.Classes.Calendar.Event).Count);
        Assert.Empty(cut.FindAll(".flare-scheduler__month-bar"));
    }

    [Fact]
    public void BarClick_SelectsTheOccurrence()
    {
        SchedulerOccurrence<SchedulerEventData>? selected = null;
        var cut = RenderMonth(new[] { Timed("multi", "Conference", 9, 10, 4, 6) }, e => selected = e);

        cut.Find(".flare-scheduler__month-bar button").Click();

        Assert.NotNull(selected);
        Assert.Equal("multi", selected!.Data.Key);
    }

    [Fact]
    public void MonthAndYear_TakeTheirOwnSurfaceClasses()
    {
        var month = Month(Timed("one", "Planning", 9));
        Assert.Contains("flare-scheduler__month", month.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-scheduler__month--year", month.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);
        Assert.Single(month.FindAll(".flare-scheduler__month-panel"));

        var year = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Timed("one", "Planning", 9) })
            .Add(x => x.Date, Date)
            .Add(x => x.View, SchedulerView.Year)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.Culture, Culture)
            .Add(x => x.OnEditRequested, _ => { }));
        Assert.Contains("flare-scheduler__month--year", year.Find(".flare-scheduler__month").ClassName, StringComparison.Ordinal);
        Assert.Equal(12, year.FindAll(".flare-scheduler__month-panel").Count);
    }

    [Fact]
    public void MonthGrid_HasGridCellsWithDates()
    {
        var cut = Month(Timed("one", "Planning", 9));

        Assert.Single(cut.FindAll(".flare-scheduler__month [role='grid']"));
        var label = Date.ToString("D", Culture);
        var cell = cut.FindAll("[role='gridcell']").Single(c => c.GetAttribute("aria-label") == label);
        Assert.Equal("false", cell.GetAttribute("aria-selected"));
    }

    [Fact]
    public void MonthTemplates_StillRender()
    {
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Timed("one", "Planning", 9) })
            .Add(x => x.Date, Date)
            .Add(x => x.View, SchedulerView.Month)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.Culture, Culture)
            .Add(x => x.OnEditRequested, _ => { })
            .Add(x => x.ChildContent, context => b =>
            {
                b.OpenComponent<FlareSchedulerSurface<SchedulerEventData>>(0);
                b.AddAttribute(1, "Context", context);
                b.AddAttribute(2, "ChildContent", (RenderFragment<SchedulerViewContext<SchedulerEventData>>)(view => child =>
                {
                    child.OpenComponent<FlareSchedulerCalendar<SchedulerEventData>>(0);
                    child.AddAttribute(1, "Context", view);
                    child.AddAttribute(2, "MonthHeaderTemplate",
                        (RenderFragment<DateOnly>)(month => x => x.AddContent(0, "MONTH-" + month.Month)));
                    child.AddAttribute(3, "DateTemplate",
                        (RenderFragment<DateOnly>)(date => x => x.AddContent(0, "DATE-" + date.Day)));
                    child.AddAttribute(4, "DayHeaderTemplate",
                        (RenderFragment<DayOfWeek>)(day => x => x.AddContent(0, "DAY-" + day)));
                    child.CloseComponent();
                }));
                b.CloseComponent();
            }));

        Assert.Contains("MONTH-11", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("DATE-4", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("DAY-Wednesday", cut.Markup, StringComparison.Ordinal);
    }
}
