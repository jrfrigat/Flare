using Flare.Abstractions;
using Flare.Infrastructure;

namespace Flare.Components.Tests;

public class MessageBoxServiceTests
{
    [Fact]
    public async Task Prompt_ResolvesWithTheEnteredText()
    {
        var service = new MessageBoxService();

        var pending = service.PromptAsync("Name", "Your name", "Ann");
        Assert.Equal(MessageBoxKind.Prompt, service.Current?.Kind);
        Assert.Equal("Ann", service.Current?.DefaultValue);

        service.Respond("Bob");

        Assert.Equal("Bob", await pending);
        Assert.Null(service.Current);
    }

    [Fact]
    public async Task RequestWhileAnotherIsOpen_WaitsItsTurn_AndEachCallerGetsItsOwnAnswer()
    {
        var service = new MessageBoxService();

        var prompt = service.PromptAsync("First");
        var confirm = service.ConfirmAsync("Second");
        var alert = service.AlertAsync("Third");

        Assert.Equal("First", service.Current?.Title);

        service.Respond("typed");
        Assert.True(prompt.IsCompleted);
        Assert.Equal("typed", await prompt);
        Assert.False(confirm.IsCompleted);
        Assert.Equal("Second", service.Current?.Title);

        service.Respond(null);
        // ConfirmAsync and AlertAsync are async wrappers whose continuations may be posted.
        Assert.False(await confirm.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken));
        Assert.Equal("Third", service.Current?.Title);

        service.Respond("ok");
        await alert.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Null(service.Current);
    }

    [Fact]
    public void QueuedRequest_DoesNotRaiseStateChanged_UntilItIsShown()
    {
        var service = new MessageBoxService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        _ = service.AlertAsync("First");
        _ = service.AlertAsync("Second");
        Assert.Equal(1, stateChanges);

        service.Respond("ok");
        Assert.Equal(2, stateChanges);
        Assert.Equal("Second", service.Current?.Title);
    }

    [Fact]
    public void Respond_WithNothingOpen_DoesNothing()
    {
        var service = new MessageBoxService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        service.Respond("ok");

        Assert.Null(service.Current);
        Assert.Equal(0, stateChanges);
    }
}
