using System.Globalization;
using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerRenderingTests : FlareTestContext
{
    private static readonly DateOnly Date = new(2026, 11, 4);
    private static readonly SchedulerEventData Item = new()
    {
        Key = "one", Title = "Planning", Start = new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero),
        End = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero)
    };
    public SchedulerRenderingTests() => Services.AddFlareScheduler();

    [Theory]
    [InlineData(SchedulerView.Day)]
    [InlineData(SchedulerView.Week)]
    [InlineData(SchedulerView.WorkWeek)]
    [InlineData(SchedulerView.Month)]
    [InlineData(SchedulerView.Year)]
    [InlineData(SchedulerView.Agenda)]
    [InlineData(SchedulerView.Timeline)]
    public async Task ServerStaticRenderingDoesNotRequireJavascriptOrADialogService(SchedulerView projection)
    {
        var js = new ForbiddenJs();
        Services.AddSingleton<IJSRuntime>(js);
        await using var renderer = new HtmlRenderer(Services, NullLoggerFactory.Instance);
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                ["Mapping"] = SchedulerMappings.EventData, ["Events"] = new[] { Item }, ["Date"] = Date,
                ["View"] = projection, ["TimeZone"] = TimeZoneInfo.Utc,
                ["Culture"] = CultureInfo.GetCultureInfo("en-GB"),
                ["ChildContent"] = (RenderFragment<SchedulerViewContext<SchedulerEventData>>)(view => b =>
                {
                    b.OpenComponent(0, projection == SchedulerView.Timeline
                        ? typeof(FlareSchedulerTimeline<SchedulerEventData>) : typeof(FlareSchedulerSurface<SchedulerEventData>));
                    b.AddAttribute(1, "Context", view);
                    if (projection == SchedulerView.Timeline) b.AddAttribute(2, "Virtualize", false);
                    b.CloseComponent();
                })
            });
            var result = await renderer.RenderComponentAsync<FlareScheduler<SchedulerEventData>>(parameters);
            return result.ToHtmlString();
        });
        Assert.Contains("Planning", html);
        if (projection == SchedulerView.Agenda) Assert.Contains("04/11/2026", html);
        Assert.DoesNotContain("flare-scheduler__toolbar", html);
        Assert.Equal(0, js.Calls);
    }

    [Fact]
    public void SelectionAndFormattingRendersReuseTheWindowedEventAndSlotIndexes()
    {
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Item }).Add(x => x.Date, Date).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var events = new CountingOccurrences(source.Instance.Snapshot.Occurrences);
        var snapshot = source.Instance.Snapshot with { Occurrences = events, CanEdit = true };
        var cut = Render<FlareSchedulerTimeGrid<SchedulerEventData>>(p => p
            .Add(x => x.Context, snapshot).Add(x => x.ShowAllDay, false));
        var passes = events.Enumerations;
        var dates = cut.Instance.VisibleDates;
        var dropZone = cut.FindComponents<FlareDropZone>()[0];
        var slotRenders = dropZone.RenderCount;
        Assert.True(passes > 0);
        cut.Render(p => p.Add(x => x.Context, snapshot with { SelectedEvent = events[0] }));
        Assert.Equal(slotRenders, dropZone.RenderCount);
        cut.Render(p => p.Add(x => x.Context, snapshot with { Culture = CultureInfo.GetCultureInfo("ru-RU") }));
        Assert.True(dropZone.RenderCount > slotRenders);
        Assert.Same(dates, cut.Instance.VisibleDates);
        Assert.Equal(passes, events.Enumerations);
        Assert.Contains("Planning", cut.Markup);

        var label = "First slot label";
        RenderFragment<SchedulerTimeSlot> template = _ => builder => builder.AddContent(0, label);
        cut.Render(p => p.Add(x => x.SlotTemplate, template));
        Assert.Equal(label, cut.Find("[data-flare-tree-index='0']").TextContent.Trim());
        label = "Updated slot label";
        cut.Render();
        Assert.Equal(label, cut.Find("[data-flare-tree-index='0']").TextContent.Trim());
    }

    [Fact]
    public void TimeGridUpdatesDropTargetsWhenEditabilityChangesWithoutChangingTheWindow()
    {
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Day).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var snapshot = source.Instance.Snapshot;
        var cut = Render<FlareSchedulerTimeGrid<SchedulerEventData>>(p => p
            .Add(x => x.Context, snapshot).Add(x => x.ShowAllDay, false));
        const string firstSlot = "[data-flare-tree-index='0']";
        Assert.Equal("0", cut.Find(firstSlot).GetAttribute("tabindex"));
        Assert.Empty(cut.FindAll("[data-flare-drop]"));

        cut.Render(p => p.Add(x => x.Context, snapshot with { CanEdit = true }));

        Assert.StartsWith("t:", cut.Find(firstSlot).ParentElement!.GetAttribute("data-flare-drop"));

        cut.Render(p => p.Add(x => x.Context, snapshot));

        Assert.Equal("0", cut.Find(firstSlot).GetAttribute("tabindex"));
        Assert.Empty(cut.FindAll("[data-flare-drop]"));
    }

    [Fact]
    public void AgendaReformatsLabelsWhenAMutableCultureChanges()
    {
        var culture = new CultureInfo("en-US");
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Item }).Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Agenda)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc).Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var agenda = Render<FlareSchedulerAgenda<SchedulerEventData>>(p => p
            .Add(x => x.Context, source.Instance.Snapshot with { Culture = culture }));
        Assert.Equal(Item.Start.ToString("g", culture), agenda.Find($".{Css.Classes.Scheduler.AgendaItem} .{Css.Classes.Text.Root}").TextContent);

        culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        agenda.Render();

        Assert.Equal(Item.Start.ToString("g", culture), agenda.Find($".{Css.Classes.Scheduler.AgendaItem} .{Css.Classes.Text.Root}").TextContent);
    }

    [Fact]
    public async Task TimeGridUpdatesTheCachedSlotsWhenKeyboardFocusMoves()
    {
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Week).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var cut = Render<FlareSchedulerTimeGrid<SchedulerEventData>>(p => p
            .Add(x => x.Context, source.Instance.Snapshot).Add(x => x.ShowAllDay, false));
        Assert.Equal("0", cut.Find("[data-flare-tree-index='0']").GetAttribute("tabindex"));

        await cut.InvokeAsync(() => cut.Instance.HandleTreeKeyAsync(0, "ArrowDown"));

        Assert.Equal("-1", cut.Find("[data-flare-tree-index='0']").GetAttribute("tabindex"));
        Assert.Equal("0", cut.Find("[data-flare-tree-index='1']").GetAttribute("tabindex"));
    }

    [Fact]
    public void YearKeepsTheDefaultProjectionUntilItsInputsChange()
    {
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Item }).Add(x => x.Date, Date).Add(x => x.View, SchedulerView.Year)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc).Add(x => x.Culture, CultureInfo.GetCultureInfo("en-GB"))
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var snapshot = source.Instance.Snapshot;
        var year = Render<FlareSchedulerCalendar<SchedulerEventData>>(p => p.Add(x => x.Context, snapshot));
        var renders = year.RenderCount;
        Assert.Contains("November 2026", year.Markup);

        year.Render(p => p.Add(x => x.Context, source.Instance.Snapshot));
        Assert.Equal(renders, year.RenderCount);

        year.Render(p => p.Add(x => x.Context, snapshot with { Culture = CultureInfo.GetCultureInfo("de-DE") })
            .Add(x => x.MonthHeaderTemplate, month => b => b.AddContent(0, month.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("de-DE")))));
        Assert.True(year.RenderCount > renders);
        Assert.Contains("Dezember 2026", year.Markup);
        Assert.Contains("Planning", year.Markup);
    }

    [Fact]
    public async Task RefusedAndFailedGesturesDoNotOpenAnEditorAndFailuresAreObservable()
    {
        var fail = false;
        var requests = 0;
        Exception? error = null;
        var source = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Events, new[] { Item }).Add(x => x.Date, Date).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.Editable, true).Add(x => x.OnEditRequested, _ => requests++)
            .Add(x => x.OnChangeError, value => error = value)
            .Add(x => x.OnChange, args => { if (fail) throw new InvalidOperationException("Unavailable"); args.Cancel = true; })
            .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Source")));
        var original = Assert.Single(source.Instance.Snapshot.Occurrences);
        var proposed = Item with { Start = Item.Start.AddHours(1), End = Item.End.AddHours(1) };
        await source.InvokeAsync(() => source.Instance.RequestChangeAsync(original, proposed, SchedulerEditAction.Move));
        fail = true;
        await source.InvokeAsync(() => source.Instance.RequestChangeAsync(original, proposed, SchedulerEditAction.Move));
        Assert.IsType<InvalidOperationException>(error);
        Assert.Null(source.Instance.ActiveEdit);
        Assert.Equal(0, requests);
        Assert.Equal(9, Item.Start.Hour);
    }

    private sealed class CountingOccurrences(IReadOnlyList<SchedulerOccurrence<SchedulerEventData>> source)
        : IReadOnlyList<SchedulerOccurrence<SchedulerEventData>>
    {
        public int Enumerations { get; private set; }
        public int Count => source.Count;
        public SchedulerOccurrence<SchedulerEventData> this[int index] => source[index];
        public IEnumerator<SchedulerOccurrence<SchedulerEventData>> GetEnumerator() { Enumerations++; return source.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
    private sealed class ForbiddenJs : IJSRuntime
    {
        public int Calls { get; private set; }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        { Calls++; throw new InvalidOperationException("Static rendering invoked JavaScript."); }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }
}
