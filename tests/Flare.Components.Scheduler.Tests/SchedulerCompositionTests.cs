using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;

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

    [Theory]
    [InlineData(SchedulerView.Day)]
    [InlineData(SchedulerView.Agenda)]
    [InlineData(SchedulerView.Month)]
    [InlineData(SchedulerView.Year)]
    public async Task InteractiveEventTemplateOwnsItsActionWithoutAnAncestorButton(SchedulerView view)
    {
        var item = new SchedulerEventData { Key = "one", Title = "One",
            Start = new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero), End = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero) };
        var edits = 0;
        var selects = 0;
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData).Add(x => x.Events, new[] { item })
            .Add(x => x.Date, Date).Add(x => x.View, view).Add(x => x.TimeZone, TimeZoneInfo.Utc)
            .Add(x => x.Editable, true).Add(x => x.OnChange, _ => { })
            .Add(x => x.OnEditRequested, _ => edits++).Add(x => x.OnEventSelected, _ => selects++)
            .Add(x => x.ChildContent, context => b =>
            {
                b.OpenComponent<FlareSchedulerSurface<SchedulerEventData>>(0);
                b.AddAttribute(1, "Context", context);
                b.AddAttribute(2, "EventTemplate", (RenderFragment<SchedulerEventContext<SchedulerEventData>>)(e => child =>
                {
                    child.OpenComponent<FlareButton>(0);
                    child.AddAttribute(1, "OnClick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, e.EditAsync));
                    child.AddAttribute(2, "ChildContent", (RenderFragment)(text => text.AddContent(0, "Custom edit")));
                    child.CloseComponent();
                }));
                b.CloseComponent();
            }));
        var button = cut.FindAll("button").Single(b => b.TextContent == "Custom edit");
        Assert.Null(button.ParentElement?.Closest("button,[role='button']"));
        await button.ClickAsync();
        Assert.Equal(1, edits);
        Assert.Equal(0, selects);
        Assert.Empty(cut.FindComponents<FlareSchedulerDialog<SchedulerEventData>>());
    }
}
