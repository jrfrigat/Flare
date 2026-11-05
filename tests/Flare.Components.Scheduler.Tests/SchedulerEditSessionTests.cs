using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerEditSessionTests : FlareTestContext
{
    private static readonly SchedulerEventData Item = new()
    {
        Key = "event", Title = "Original", Start = new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero),
        End = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero)
    };

    [Fact]
    public async Task RefusedOrFailedSaveRetainsDraftAndCanBeRetriedWithoutAForm()
    {
        var calls = 0;
        var session = new SchedulerEditContext<SchedulerEventData>
        {
            Action = SchedulerEditAction.Create, Draft = Item,
            Submit = _ => ++calls switch
            {
                1 => Task.FromResult(false),
                2 => Task.FromException<bool>(new InvalidOperationException("Offline")),
                _ => Task.FromResult(true)
            }
        };
        session.Draft = Item with { Title = "Draft" };
        Assert.False(await session.SaveAsync());
        Assert.Equal(SchedulerEditStatus.Refused, session.Status);
        Assert.False(session.IsClosed);
        Assert.False(await session.SaveAsync());
        Assert.Equal(SchedulerEditStatus.Failed, session.Status);
        Assert.IsType<InvalidOperationException>(session.Error);
        Assert.Equal("Draft", session.Draft.Title);
        Assert.Equal("Original", Item.Title);
        Assert.True(await session.SaveAsync());
        Assert.True(session.IsClosed);
        Assert.True(session.IsSaved);
        Assert.Null(session.Error);
        Assert.False(await session.SaveAsync());
        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task BusySessionSubmitsOnlyOnceAndCannotDiscardAnInFlightProposal()
    {
        var pending = new TaskCompletionSource<bool>();
        var calls = 0;
        var session = new SchedulerEditContext<SchedulerEventData>
        {
            Action = SchedulerEditAction.Create, Draft = Item,
            Submit = _ => { calls++; return pending.Task; }
        };
        var save = session.SaveAsync();
        Assert.True(session.IsSaving);
        Assert.False(await session.SaveAsync());
        await session.CancelAsync();
        Assert.False(session.IsClosed);
        pending.SetResult(false);
        Assert.False(await save);
        Assert.False(session.IsSaving);
        await session.CancelAsync();
        Assert.True(session.IsClosed);
        Assert.False(session.IsSaved);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task CustomPresentationReceivesOneSessionAndPreservesGestureCommandsWithoutDialogs()
    {
        Services.AddFlareScheduler();
        SchedulerEditContext<SchedulerEventData>? session = null;
        var requests = 0;
        var saves = 0;
        var cut = Render<FlareScheduler<SchedulerEventData>>(p => p
            .Add(x => x.Mapping, SchedulerMappings.EventData).Add(x => x.Events, new[] { Item })
            .Add(x => x.Date, new DateOnly(2026, 11, 4)).Add(x => x.View, SchedulerView.Agenda)
            .Add(x => x.TimeZone, TimeZoneInfo.Utc).Add(x => x.Editable, true)
            .Add(x => x.OnChange, _ => saves++)
            .Add(x => x.OnEditRequested, value => { requests++; session = value; }));
        var original = Assert.Single(cut.Instance.Snapshot.Occurrences);
        await cut.InvokeAsync(() => cut.Instance.SelectSlotAsync(new(Item.Start, Item.End)));
        Assert.Equal(0, requests);
        await cut.InvokeAsync(() => cut.Instance.EditEventAsync(original));
        await cut.InvokeAsync(() => cut.Instance.EditEventAsync(original));
        Assert.Equal(1, requests);
        Assert.Empty(cut.FindComponents<FlareSchedulerDialog<SchedulerEventData>>());
        session!.Draft = Item with { Title = "Discarded" };
        await cut.InvokeAsync(session.CancelAsync);
        Assert.Null(cut.Instance.ActiveEdit);
        Assert.Equal(0, saves);
        await cut.InvokeAsync(() => cut.Instance.RequestChangeAsync(original,
            Item with { Start = Item.Start.AddHours(1), End = Item.End.AddHours(1) }, SchedulerEditAction.Move));
        Assert.Equal(1, saves);
        Assert.Equal(1, requests);
        Assert.Equal("Original", Item.Title);
    }

    [Fact]
    public async Task InvalidDraftDoesNotReachPersistence()
    {
        var calls = 0;
        var session = new SchedulerEditContext<SchedulerEventData>
        {
            Action = SchedulerEditAction.Create, Draft = Item with { Title = " " },
            Submit = _ => { calls++; return Task.FromResult(true); }
        };
        Assert.False(await session.SaveAsync());
        Assert.Equal(SchedulerEditStatus.Invalid, session.Status);
        Assert.Equal(0, calls);
        await session.CancelAsync();
        Assert.Equal("Original", Item.Title);
    }

    [Fact]
    public void IndependentFormDisplaysTheActualStringFieldsAndResource()
    {
        Services.AddFlareScheduler();
        var session = new SchedulerEditContext<SchedulerEventData>
        {
            Action = SchedulerEditAction.Create, Draft = Item with { Description = "Details", ResourceKey = "room" },
            Resources = [new("room", "Meeting room")], Submit = _ => Task.FromResult(true)
        };
        var cut = Render<FlareSchedulerEditor<SchedulerEventData>>(p => p.Add(x => x.Context, session));
        var inputs = cut.FindAll("input[type=text]");
        Assert.Contains(inputs, input => input.GetAttribute("value") == "Original");
        Assert.Contains(inputs, input => input.GetAttribute("value") == "Details");
        Assert.Contains("Meeting room", cut.Find("[role=combobox]").TextContent);
        Assert.DoesNotContain("Context.Draft.", cut.Markup);
    }
}
