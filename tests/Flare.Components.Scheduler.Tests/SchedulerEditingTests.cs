using Flare.Abstractions;
using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Flare.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerEditingTests : FlareTestContext
{
    private static readonly SchedulerEventData Item = new()
    {
        Key = "booking", Title = "Room", Start = new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero),
        End = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero)
    };
    public SchedulerEditingTests()
    {
        Services.AddFlareScheduler();
        Services.AddSingleton<IDialogService, DialogService>();
    }
    private IRenderedComponent<FlareScheduler<SchedulerEventData>> Scheduler(
        Action<SchedulerChangeEventArgs<SchedulerEventData>>? change = null, SchedulerEventData? item = null, bool editable = true) =>
        Render<FlareScheduler<SchedulerEventData>>(p =>
        {
            p.Add(x => x.Mapping, SchedulerMappings.EventData).Add(x => x.Events, new[] { item ?? Item })
                .Add(x => x.Date, new DateOnly(2026, 11, 4)).Add(x => x.View, SchedulerView.Agenda)
                .Add(x => x.TimeZone, TimeZoneInfo.Utc).Add(x => x.Editable, editable);
            if (change is not null) p.Add(x => x.OnChange, change);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancellableIntentsNeverMutateApplicationItems(bool refuse)
    {
        SchedulerChangeEventArgs<SchedulerEventData>? seen = null;
        var cut = Scheduler(e => { seen = e; e.Cancel = refuse; });
        var original = Assert.Single(cut.Instance.Snapshot.Occurrences);
        var proposed = Item with { Start = Item.Start.AddHours(1), End = Item.End.AddHours(1) };
        var accepted = await cut.InvokeAsync(() => cut.Instance.ApplyChangeAsync(SchedulerEditAction.Move, original, proposed));
        Assert.Equal(!refuse, accepted);
        Assert.Same(Item, seen!.Original!.Item);
        Assert.Equal(9, Item.Start.Hour);
        Assert.Equal(10, seen.Data.Start.Hour);
    }

    [Fact]
    public async Task ReadOnlyAndMissingHandlersRefuseEditing()
    {
        var count = 0;
        var readOnly = Scheduler(_ => count++, editable: false);
        var noHandler = Scheduler();
        foreach (var cut in new[] { readOnly, noHandler })
            Assert.False(await cut.InvokeAsync(() => cut.Instance.ApplyChangeAsync(SchedulerEditAction.Update,
                cut.Instance.Snapshot.Occurrences[0], Item)));
        Assert.Equal(0, count);
    }

    [Theory]
    [InlineData(SchedulerEditScope.Occurrence)]
    [InlineData(SchedulerEditScope.Following)]
    [InlineData(SchedulerEditScope.Series)]
    public async Task ReadyBackendSeriesRetainOpaqueIdentityAndChosenScope(SchedulerEditScope scope)
    {
        var item = Item with { SeriesKey = "series", RecurrenceId = "server-instance" };
        SchedulerChangeEventArgs<SchedulerEventData>? seen = null;
        var cut = Scheduler(e => seen = e, item);
        await cut.InvokeAsync(() => cut.Instance.ApplyChangeAsync(SchedulerEditAction.Update,
            cut.Instance.Snapshot.Occurrences[0], item with { Title = "Changed" }, scope));
        Assert.Equal(scope, seen!.Scope);
        Assert.Equal("series", seen.Original!.Series.Key);
        Assert.Equal("server-instance", seen.Original.Identity);
        Assert.Equal("Room", item.Title);
    }

    [Fact]
    public void StandaloneIdentitySurvivesTimeChanges()
    {
        var cut = Scheduler();
        var identity = cut.Instance.Snapshot.Occurrences[0].Identity;
        cut.Render(p => p.Add(x => x.Events, new[] { Item with { Start = Item.Start.AddHours(2), End = Item.End.AddHours(2) } }));
        Assert.Equal(identity, cut.Instance.Snapshot.Occurrences[0].Identity);
    }

    [Fact]
    public async Task DefaultEditorCancelDiscardsTheWorkingCopy()
    {
        var calls = 0;
        var cut = Scheduler(_ => calls++);
        var provider = Render<FlareDialogProvider>();
        Task? pending = null;
        await cut.InvokeAsync(() => { pending = cut.Instance.EditEventAsync(cut.Instance.Snapshot.Occurrences[0]); });
        provider.WaitForAssertion(() => Assert.NotEmpty(provider.FindAll("[role=dialog]")));
        var editor = provider.FindComponent<FlareSchedulerEditor<SchedulerEventData>>();
        editor.Instance.Context.Draft = Item with { Title = "Discard me" };
        await provider.InvokeAsync(() => editor.Instance.Context.CancelAsync());
        await pending!;
        Assert.Equal(0, calls);
        Assert.Equal("Room", Item.Title);
        Assert.Empty(Services.GetRequiredService<IDialogService>().OpenDialogs);
    }

    [Fact]
    public async Task DefaultPresenterVetoesDismissalDuringPersistenceWithoutReplacingTheForm()
    {
        var pending = new TaskCompletionSource();
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p.Add(x => x.Mapping, SchedulerMappings.EventData)
            .Add(x => x.Date, new DateOnly(2026, 11, 4)).Add(x => x.Editable, true)
            .Add(x => x.OnChange, _ => pending.Task));
        var provider = Render<FlareDialogProvider>();
        await cut.InvokeAsync(() => cut.Instance.CreateEventAsync(new(Item.Start, Item.End)));
        var editor = provider.FindComponent<FlareSchedulerEditor<SchedulerEventData>>();
        editor.Instance.Context.Draft = Item;
        Task<bool>? saving = null;
        await cut.InvokeAsync(() => { saving = editor.Instance.Context.SaveAsync(); });
        var dialog = Assert.Single(Services.GetRequiredService<IDialogService>().OpenDialogs);
        Assert.False(await cut.InvokeAsync(dialog.CancelAsync));
        Assert.Same(editor.Instance, provider.FindComponent<FlareSchedulerEditor<SchedulerEventData>>().Instance);
        pending.SetResult();
        Assert.True(await saving!);
        Assert.Empty(Services.GetRequiredService<IDialogService>().OpenDialogs);
    }
}
