using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class DialogServiceComponentTests
{
    [Fact]
    public void Show_AddsToOpenDialogs_AndRaisesStateChanged()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        var reference = service.Show<TestDialogBody>("Edit",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "x"),
            new DialogOptions { Size = DialogSize.Sm });

        Assert.Single(service.OpenDialogs);
        Assert.Equal("Edit", service.OpenDialogs[0].Title);
        Assert.Equal(DialogSize.Sm, service.OpenDialogs[0].Options.Size);
        Assert.Equal(typeof(TestDialogBody), service.OpenDialogs[0].ContentType);
        Assert.False(reference.Result.IsCompleted);
        Assert.Equal(1, stateChanges);
    }

    [Fact]
    public async Task ClosingInstance_ResolvesResultWithPayload_AndRemovesDialog()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Instance.Close("payload-42");

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("payload-42", result.GetData<string>());
        Assert.Empty(service.OpenDialogs);
        Assert.Equal(2, stateChanges); // one for show, one for close
    }

    [Fact]
    public async Task Cancel_ResolvesCancelled_AndRemovesDialog()
    {
        var service = new DialogService();

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Cancel();

        var result = await reference.Result;
        Assert.True(result.Cancelled);
        Assert.Null(result.Data);
        Assert.Empty(service.OpenDialogs);
    }

    [Fact]
    public async Task SecondClose_IsIgnored_FirstResultWins()
    {
        var service = new DialogService();

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Instance.Close("first");
        reference.Instance.Cancel(); // should be a no-op

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("first", result.GetData<string>());
        Assert.True(reference.Instance.IsClosed);
    }

    [Fact]
    public async Task ShowAsync_ReturnsAwaitableResult()
    {
        var service = new DialogService();

        var task = service.ShowAsync<TestDialogBody>("Edit");
        Assert.False(task.IsCompleted);

        service.OpenDialogs[0].Close("done");

        var result = await task;
        Assert.Equal("done", result.GetData<string>());
    }

    [Fact]
    public void Show_MultipleDialogs_StackInOrder()
    {
        var service = new DialogService();

        var first = service.Show<TestDialogBody>("First");
        var second = service.Show<TestDialogBody>("Second");

        Assert.Equal(2, service.OpenDialogs.Count);
        Assert.Equal("First", service.OpenDialogs[0].Title);
        Assert.Equal("Second", service.OpenDialogs[1].Title);

        first.Cancel();
        Assert.Single(service.OpenDialogs);
        Assert.Equal("Second", service.OpenDialogs[0].Title);
        second.Cancel();
        Assert.Empty(service.OpenDialogs);
    }

    [Fact]
    public async Task ConfirmWhileAnotherIsOpen_WaitsItsTurn_AndEachCallerGetsItsOwnAnswer()
    {
        var service = new DialogService();

        var first = service.ConfirmAsync("First", "a");
        var second = service.AlertAsync("Second", "b");
        var third = service.ConfirmAsync("Third", "c");

        Assert.Equal("First", service.Current?.Title);

        service.Respond(true);
        Assert.True(first.IsCompleted);
        Assert.True(await first);
        Assert.False(second.IsCompleted);
        Assert.Equal("Second", service.Current?.Title);

        service.Respond(null);
        // AlertAsync is an async wrapper whose continuation may be posted, so wait with a bound.
        await second.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Equal("Third", service.Current?.Title);

        service.Respond(false);
        Assert.True(third.IsCompleted);
        Assert.False(await third);
        Assert.Null(service.Current);
    }

    [Fact]
    public void QueuedConfirm_DoesNotRaiseStateChanged_UntilItIsShown()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        _ = service.ConfirmAsync("First", "a");
        _ = service.ConfirmAsync("Second", "b");
        Assert.Equal(1, stateChanges);

        service.Respond(true);
        Assert.Equal(2, stateChanges);
    }

    [Fact]
    public void Respond_WithNothingOpen_DoesNothing()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        service.Respond(true);

        Assert.Null(service.Current);
        Assert.Equal(0, stateChanges);
    }
}
