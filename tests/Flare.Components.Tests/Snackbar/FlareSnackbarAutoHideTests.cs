using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// Auto-hide is paused while the user reads or acts: hover, keyboard focus and an async action keep the
/// remaining time, and removing, clearing, navigating or disposing stops the timer instead of letting a
/// stale delay run.
/// </summary>
public class FlareSnackbarAutoHideTests : FlareTestContext
{
    public FlareSnackbarAutoHideTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    [Fact]
    public async Task HoverPausesTheTimerAndLeavingResumesIt()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("Read me", new SnackbarOptions { DurationMs = 250 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        cut.Find($".{Css.Classes.Snackbar.Root}").MouseEnter();
        await Task.Delay(400, Xunit.TestContext.Current.CancellationToken);
        Assert.Single(cut.FindAll($".{Css.Classes.Snackbar.Root}"));

        cut.Find($".{Css.Classes.Snackbar.Root}").MouseLeave();
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
    }

    [Fact]
    public async Task FocusPausesTheTimerAndLeavingFocusResumesIt()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("Read me", new SnackbarOptions { DurationMs = 250 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        cut.Find($".{Css.Classes.Snackbar.Root}").FocusIn();
        await Task.Delay(400, Xunit.TestContext.Current.CancellationToken);
        Assert.Single(cut.FindAll($".{Css.Classes.Snackbar.Root}"));

        cut.Find($".{Css.Classes.Snackbar.Root}").FocusOut();
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
    }

    [Fact]
    public async Task AsyncActionKeepsTheToastAliveUntilItCompletes()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("Action", new SnackbarOptions
        {
            DurationMs = 200,
            ActionText = "Undo",
            OnAction = () => gate.Task,
        });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        cut.Find($".{Css.Classes.Snackbar.ActionBtn}").Click();
        await Task.Delay(350, Xunit.TestContext.Current.CancellationToken);
        Assert.Single(cut.FindAll($".{Css.Classes.Snackbar.Root}"));

        gate.SetResult(true);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
    }

    [Fact]
    public async Task RemoveCancelsTheTimer()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        var id = Guid.NewGuid();
        service.Show(new SnackbarMessage(id, "Remove me", SnackbarSeverity.Normal, 150, ShowClose: false));
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        service.Remove(id);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);

        // The cancelled delay must not fire a later dismiss or throw on the renderer.
        await Task.Delay(250, Xunit.TestContext.Current.CancellationToken);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public async Task ClearCancelsEveryTimer()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("One", new SnackbarOptions { DurationMs = 10_000 });
        service.Show("Two", new SnackbarOptions { DurationMs = 10_000 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 2);

        service.Clear();
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);

        await Task.Delay(100, Xunit.TestContext.Current.CancellationToken);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public async Task NavigationRemovesACloseAfterNavigationToastAndStopsItsTimer()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        var nav = Services.GetRequiredService<NavigationManager>();
        service.Show("Route", new SnackbarOptions { DurationMs = 10_000, CloseAfterNavigation = true });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        await cut.InvokeAsync(() => nav.NavigateTo("/elsewhere"));
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);

        await Task.Delay(100, Xunit.TestContext.Current.CancellationToken);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public async Task PersistentToastNeverAutoHides()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("Stay", new SnackbarOptions { DurationMs = 0 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);

        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);
        Assert.Single(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }
}
