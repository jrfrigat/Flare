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
        var snapshot = source.Instance.Snapshot with { Occurrences = events };
        var cut = Render<FlareSchedulerTimeGrid<SchedulerEventData>>(p => p.Add(x => x.Context, snapshot));
        var passes = events.Enumerations;
        var dates = cut.Instance.VisibleDates;
        Assert.True(passes > 0);
        cut.Render(p => p.Add(x => x.Context, snapshot with { SelectedEvent = events[0] }));
        cut.Render(p => p.Add(x => x.Context, snapshot with { Culture = CultureInfo.GetCultureInfo("ru-RU") }));
        Assert.Same(dates, cut.Instance.VisibleDates);
        Assert.Equal(passes, events.Enumerations);
        Assert.Contains("Planning", cut.Markup);
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
