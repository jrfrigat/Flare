using Flare.Abstractions;
using Flare.Infrastructure;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class DialogCloseGuardTests : FlareTestContext
{
    private readonly DialogService _dialogs = new();

    public DialogCloseGuardTests() => Services.AddSingleton<IDialogService>(_dialogs);

    [Fact]
    public async Task ProgrammaticCancelConsultsBeforeClose()
    {
        var service = new DialogService();
        var calls = 0;
        var reference = service.Show<TestDialogBody>(options: new DialogOptions
        {
            BeforeClose = _ => { calls++; return ValueTask.FromResult(false); }
        });

        Assert.False(await reference.CancelAsync());

        Assert.Equal(1, calls);
        Assert.False(reference.Result.IsCompleted);
        Assert.Single(service.OpenDialogs);
    }

    [Fact]
    public void InternalNavigationCanBeVetoed()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        var original = navigation.Uri;
        var closed = false;
        Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .Add(x => x.BeforeClose, _ => ValueTask.FromResult(false))
            .Add(x => x.VisibleChanged, (bool visible) => closed = !visible));

        navigation.NavigateTo("guarded-destination");

        Assert.Equal(original, navigation.Uri);
        Assert.False(closed);
    }

    [Theory]
    [InlineData(DialogCloseReason.ScrimClick)]
    [InlineData(DialogCloseReason.EscapeKey)]
    [InlineData(DialogCloseReason.CloseButton)]
    [InlineData(DialogCloseReason.Programmatic)]
    [InlineData(DialogCloseReason.Navigation)]
    public async Task EveryInlineClosePathUsesTheSameGuard(DialogCloseReason reason)
    {
        var observed = new List<DialogCloseReason>();
        var closed = false;
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .Add(x => x.ShowCloseButton, true)
            .Add(x => x.VisibleChanged, (bool visible) => closed = !visible)
            .AddChildContent<FlareDialogCloseGuard>(g => g
                .Add(x => x.HasUnsavedChanges, true)
                .Add(x => x.ConfirmClose, r => { observed.Add(r); return ValueTask.FromResult(false); })));

        switch (reason)
        {
            case DialogCloseReason.ScrimClick: cut.Find($".{Css.Classes.Dialog.Scrim}").Click(); break;
            case DialogCloseReason.EscapeKey: await cut.InvokeAsync(cut.Instance.CloseFromEsc); break;
            case DialogCloseReason.CloseButton: cut.Find($".{Css.Classes.Dialog.CloseButton}").Click(); break;
            case DialogCloseReason.Programmatic: await cut.InvokeAsync(() => cut.Instance.CloseAsync()); break;
            case DialogCloseReason.Navigation: Services.GetRequiredService<NavigationManager>().NavigateTo("next"); break;
        }

        Assert.Equal(new[] { reason }, observed);
        Assert.False(closed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DefaultPromptOnlyExplicitConfirmationCloses(bool accept)
    {
        var closed = false;
        var provider = Render<FlareDialogProvider>();
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .Add(x => x.VisibleChanged, (bool visible) => closed = !visible)
            .AddChildContent<FlareDialogCloseGuard>(g => g.Add(x => x.HasUnsavedChanges, true)));
        Task<bool> pending = null!;
        await cut.InvokeAsync(() => { pending = cut.Instance.CloseAsync(); });
        provider.WaitForState(() => provider.FindAll("button").Count == 2);
        Assert.Equal(FlareStrings.DialogGuard_Title, Assert.Single(_dialogs.OpenDialogs).Title);
        Assert.Contains(FlareStrings.DialogGuard_Message, provider.Markup);

        provider.FindAll("button").Single(b => b.TextContent == (accept
            ? FlareStrings.DialogGuard_Discard : FlareStrings.DialogGuard_KeepEditing)).Click();

        Assert.Equal(accept, await pending.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken));
        Assert.Equal(accept, closed);
        Assert.Empty(_dialogs.OpenDialogs);
    }

    [Fact]
    public async Task EditContextIsReadAtClose_AndSavingClearsTheGuard()
    {
        var model = new FormModel();
        var form = new EditContext(model);
        var prompts = 0;
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .AddCascadingValue(form)
            .AddChildContent<FlareDialogCloseGuard>(g => g.Add(x => x.ConfirmClose,
                _ => { prompts++; return ValueTask.FromResult(false); })));

        Assert.True(await cut.InvokeAsync(() => cut.Instance.CloseAsync()));
        form.NotifyFieldChanged(new FieldIdentifier(model, nameof(FormModel.Name)));
        Assert.False(await cut.InvokeAsync(() => cut.Instance.CloseAsync()));
        Assert.Equal(1, prompts);
        form.MarkAsUnmodified();
        Assert.True(await cut.InvokeAsync(() => cut.Instance.CloseAsync()));
        Assert.Equal(1, prompts);
    }

    [Fact]
    public async Task ExplicitCleanStateOverridesModifiedEditContext()
    {
        var model = new FormModel();
        var form = new EditContext(model);
        form.NotifyFieldChanged(new FieldIdentifier(model, nameof(FormModel.Name)));
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true).AddCascadingValue(form)
            .AddChildContent<FlareDialogCloseGuard>(g => g.Add(x => x.HasUnsavedChanges, false)));
        Assert.True(await cut.InvokeAsync(() => cut.Instance.CloseAsync()));
        Assert.Empty(_dialogs.OpenDialogs);
    }

    [Fact]
    public async Task ServiceHandleAndUiShareGuards_AndDoNotDoubleCheck()
    {
        var calls = 0;
        var provider = Render<FlareDialogProvider>();
        var reference = _dialogs.Show<TestDialogBody>(options: new DialogOptions
        {
            BeforeClose = _ => { calls++; return ValueTask.FromResult(true); }
        });
        var registeredCalls = 0;
        var allow = false;
        using var registration = reference.Instance.CloseContext.RegisterGuard(_ =>
        {
            registeredCalls++;
            return ValueTask.FromResult(allow);
        });

        Assert.False(await reference.CloseAsync(DialogResult.Ok("data")));
        Assert.False(reference.Result.IsCompleted);
        allow = true;
        provider.Find($".{Css.Classes.Dialog.Scrim}").Click();

        Assert.True((await reference.Result).Cancelled);
        Assert.Equal(2, calls);
        Assert.Equal(2, registeredCalls);
    }

    [Fact]
    public async Task ConcurrentClosesHaveOnePrompt_AndPreserveTheFirstResult()
    {
        var answer = new TaskCompletionSource<bool>();
        var calls = 0;
        var reference = _dialogs.Show<TestDialogBody>(options: new DialogOptions
        {
            BeforeClose = _ => { calls++; return new ValueTask<bool>(answer.Task); }
        });
        var first = reference.CloseAsync(DialogResult.Ok("first"));
        Assert.False(await reference.CancelAsync());
        Assert.Equal(1, calls);
        answer.SetResult(true);
        Assert.True(await first);
        Assert.Equal("first", (await reference.Result).Data);
    }

    [Fact]
    public async Task GuardRegistrationCanBeRemoved_AndExceptionsDoNotLockClosing()
    {
        var reference = _dialogs.Show<TestDialogBody>();
        var registration = reference.Instance.CloseContext.RegisterGuard(_ => throw new InvalidOperationException("guard"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => reference.CancelAsync());
        registration.Dispose();
        Assert.True(await reference.CancelAsync());
    }

    [Fact]
    public async Task DisposingGuardCancelsItsPromptWithoutClosingTheOwnerOrOtherDialogs()
    {
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .AddChildContent<FlareDialogCloseGuard>(g => g.Add(x => x.HasUnsavedChanges, true)));
        var other = _dialogs.Show<TestDialogBody>("Other");
        Task<bool> pending = null!;
        await cut.InvokeAsync(() => { pending = cut.Instance.CloseAsync(); });
        Assert.Equal(2, _dialogs.OpenDialogs.Count);
        await cut.InvokeAsync(cut.FindComponent<FlareDialogCloseGuard>().Instance.Dispose);
        Assert.False(await pending.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken));
        Assert.Equal(other.Id, Assert.Single(_dialogs.OpenDialogs).Id);
    }

    [Fact]
    public async Task LateApprovalCannotCloseAReopenedDialog()
    {
        var answer = new TaskCompletionSource<bool>();
        var closed = false;
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .Add(x => x.BeforeClose, _ => new ValueTask<bool>(answer.Task))
            .Add(x => x.VisibleChanged, (bool visible) => closed = !visible));
        Task<bool> pending = null!;
        await cut.InvokeAsync(() => { pending = cut.Instance.CloseAsync(); });
        cut.Render(p => p.Add(x => x.Visible, false));
        cut.Render(p => p.Add(x => x.Visible, true));
        answer.SetResult(true);
        Assert.False(await pending);
        Assert.False(closed);
    }

    [Fact]
    public async Task CustomPromptTextReachesOnlyItsOwnConfirmation()
    {
        var cut = Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .AddChildContent<FlareDialogCloseGuard>(g => g.Add(x => x.HasUnsavedChanges, true)
                .Add(x => x.Title, "Leave draft?").Add(x => x.Message, "Discard this draft.")
                .Add(x => x.ConfirmLabel, "Leave").Add(x => x.CancelLabel, "Stay")));
        var provider = Render<FlareDialogProvider>();
        Task<bool> pending = null!;
        await cut.InvokeAsync(() => { pending = cut.Instance.CloseAsync(); });
        provider.WaitForState(() => provider.FindAll("button").Count == 2);
        Assert.Equal("Leave draft?", Assert.Single(_dialogs.OpenDialogs).Title);
        Assert.Contains("Discard this draft.", provider.Markup);
        provider.FindAll("button").Single(b => b.TextContent == "Stay").Click();
        Assert.False(await pending);
    }

    [Fact]
    public void AnotherNavigationHandlerCanKeepAnApprovedDialogOpen()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        var original = navigation.Uri;
        var closed = false;
        Render<FlareDialog>(p => p.Add(x => x.Visible, true)
            .Add(x => x.BeforeClose, _ => ValueTask.FromResult(true))
            .Add(x => x.VisibleChanged, (bool visible) => closed = !visible));
        using var other = navigation.RegisterLocationChangingHandler(context =>
        {
            context.PreventNavigation();
            return ValueTask.CompletedTask;
        });
        navigation.NavigateTo("blocked-elsewhere");
        Assert.Equal(original, navigation.Uri);
        Assert.False(closed);
    }

    private sealed class FormModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
