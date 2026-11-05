using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerCompositionTests : FlareTestContext
{
    private static readonly DateOnly Date = new(2026, 11, 4);

    public SchedulerCompositionTests()
    {
        Services.AddFlareScheduler();
    }

    [Fact]
    public async Task HeadlessCompositionHasNoToolbarOrInactiveProjectionAndUsesItsOwnCommands()
    {
        SchedulerViewContext<SchedulerEventData>? snapshot = null;
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData).Add(x => x.Date, Date)
            .Add(x => x.ChildContent, context => b =>
            {
                snapshot = context;
                b.OpenComponent<FlareSchedulerAgenda<SchedulerEventData>>(0);
                b.AddAttribute(1, "Context", context);
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll(".flare-scheduler__toolbar"));
        Assert.Empty(cut.FindAll(".flare-scheduler__time-grid"));
        Assert.Single(cut.FindComponents<FlareSchedulerAgenda<SchedulerEventData>>());
        Assert.IsAssignableFrom<ISchedulerCommands<SchedulerEventData>>(snapshot!.Commands);
        await cut.InvokeAsync(() => snapshot.Commands.GoToAsync(Date.AddDays(2)));
        Assert.Equal(Date.AddDays(2), snapshot.Date);
    }

    [Fact]
    public async Task SeparateScopesDoNotShareDateOrSelection()
    {
        IRenderedComponent<FlareScheduler<SchedulerEventData>> Scope() =>
            Render<FlareScheduler<SchedulerEventData>>(p => p
                .Add(x => x.Mapping, SchedulerMappings.EventData).Add(x => x.Date, Date)
                .Add(x => x.ChildContent, _ => b => b.AddContent(0, "Scope")));
        var first = Scope();
        var second = Scope();
        await first.InvokeAsync(() => first.Instance.Snapshot.Commands.GoToAsync(Date.AddDays(3)));
        await first.InvokeAsync(() => first.Instance.Snapshot.Commands.SelectSlotAsync(
            new(new(2026, 11, 7, 9, 0, 0, TimeSpan.Zero), new(2026, 11, 7, 10, 0, 0, TimeSpan.Zero))));
        Assert.NotNull(first.Instance.Snapshot.SelectedSlot);
        Assert.Null(second.Instance.Snapshot.SelectedSlot);
        Assert.Equal(Date, second.Instance.Snapshot.Date);
        Assert.NotSame(first.Instance.Snapshot.Commands, second.Instance.Snapshot.Commands);
    }
}
