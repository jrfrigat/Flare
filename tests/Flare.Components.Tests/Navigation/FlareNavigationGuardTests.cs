using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// FlareNavigationGuard is the only thing between a half-filled form and a lost draft. Its two halves
/// are independent: the in-app prompt is a dialog the guard owns, while leaving the site is the
/// browser's own prompt, so the C# contract ends at the call that switches it on.
/// </summary>
public class FlareNavigationGuardTests : FlareTestContext
{
    private readonly StubDialogs _dialogs = new();
    private readonly StubUi _ui = new();
    private int _confirmed;
    private int _cancelled;

    public FlareNavigationGuardTests()
    {
        Services.AddSingleton<IDialogService>(_dialogs);
        Services.AddSingleton<IUiJsService>(_ui);
    }

    private IRenderedComponent<FlareNavigationGuard> RenderGuard(bool dirty, bool guardInApp = true,
        string? title = null, string? message = null, string? leave = null, string? stay = null)
        => Render<FlareNavigationGuard>(p =>
        {
            p.Add(x => x.IsDirty, dirty);
            p.Add(x => x.GuardInAppNavigation, guardInApp);
            if (title is not null) p.Add(x => x.Title, title);
            if (message is not null) p.Add(x => x.Message, message);
            if (leave is not null) p.Add(x => x.LeaveLabel, leave);
            if (stay is not null) p.Add(x => x.StayLabel, stay);
            p.Add(x => x.OnConfirm, EventCallback.Factory.Create(this, () => _confirmed++));
            p.Add(x => x.OnCancel, EventCallback.Factory.Create(this, () => _cancelled++));
        });

    private NavigationManager Nav => Services.GetRequiredService<NavigationManager>();

    [Fact]
    public void CleanNavigation_LeavesWithoutAsking()
    {
        RenderGuard(dirty: false);

        Nav.NavigateTo("/next");

        Assert.Equal(0, _dialogs.ConfirmCalls);
        Assert.Equal(0, _confirmed);
        Assert.Equal(0, _cancelled);
        Assert.EndsWith("/next", Nav.Uri);
    }

    [Theory]
    [InlineData(true, 1, 0)]
    [InlineData(false, 0, 1)]
    [InlineData(null, 0, 1)]
    public void DirtyNavigation_OnlyAnExplicitYesLeaves(bool? answer, int confirms, int cancels)
    {
        _dialogs.Result = answer;
        RenderGuard(dirty: true);

        Nav.NavigateTo("/next");

        Assert.Equal(1, _dialogs.ConfirmCalls);
        Assert.Equal(confirms, _confirmed);
        Assert.Equal(cancels, _cancelled);
        if (answer == true) Assert.EndsWith("/next", Nav.Uri);
        else Assert.DoesNotContain("/next", Nav.Uri);
    }

    [Fact]
    public void Cancel_PreventsNavigationAndKeepsTheCurrentUri()
    {
        _dialogs.Result = false;
        RenderGuard(dirty: true);
        var before = Nav.Uri;

        Nav.NavigateTo("/next");

        Assert.Equal(before, Nav.Uri);
        Assert.Equal(1, _cancelled);
    }

    [Fact]
    public void CustomLabelsAndText_ReachTheDialog()
    {
        _dialogs.Result = false;
        RenderGuard(dirty: true, title: "Leave the form?", message: "The draft will be lost.",
            leave: "Discard", stay: "Keep editing");

        Nav.NavigateTo("/next");

        Assert.Equal("Leave the form?", _dialogs.Title);
        Assert.Equal("The draft will be lost.", _dialogs.Message);
        Assert.Equal("Discard", _dialogs.ConfirmLabel);
        Assert.Equal("Keep editing", _dialogs.CancelLabel);
    }

    [Fact]
    public void DefaultText_IsLocalized()
    {
        _dialogs.Result = false;
        RenderGuard(dirty: true);

        Nav.NavigateTo("/next");

        Assert.Equal(FlareStrings.NavGuard_Title, _dialogs.Title);
        Assert.Equal(FlareStrings.NavGuard_Message, _dialogs.Message);
        Assert.Equal(FlareStrings.NavGuard_Leave, _dialogs.ConfirmLabel);
        Assert.Equal(FlareStrings.NavGuard_Stay, _dialogs.CancelLabel);
    }

    [Fact]
    public void DirtyChange_TogglesTheUnloadPrompt()
    {
        var cut = RenderGuard(dirty: false);
        Assert.Empty(_ui.Prompts);

        cut.Render(p => p.Add(x => x.IsDirty, true));
        cut.Render(p => p.Add(x => x.IsDirty, true));
        cut.Render(p => p.Add(x => x.IsDirty, false));

        Assert.Equal(new[] { true, false }, _ui.Prompts);
    }

    [Fact]
    public void GuardInAppNavigationFalse_KeepsOnlyTheBrowserPrompt()
    {
        _dialogs.Result = false;
        RenderGuard(dirty: true, guardInApp: false);

        Nav.NavigateTo("/next");

        Assert.Equal(0, _dialogs.ConfirmCalls);
        Assert.EndsWith("/next", Nav.Uri);
        Assert.Equal(new[] { true }, _ui.Prompts);
    }

    [Fact]
    public async Task Dispose_RemovesTheHandlerAndThePrompt()
    {
        _dialogs.Result = false;
        var cut = RenderGuard(dirty: true);

        await cut.Instance.DisposeAsync();
        Nav.NavigateTo("/after-dispose");

        Assert.Equal(0, _dialogs.ConfirmCalls);
        Assert.EndsWith("/after-dispose", Nav.Uri);
        Assert.False(_ui.Prompts[^1]);
    }

    [Theory]
    [InlineData("js")]
    [InlineData("disconnected")]
    [InlineData("invalid")]
    public async Task JavascriptFailure_DoesNotBreakRenderOrDispose(string kind)
    {
        _ui.ThrowOnPrompt = kind switch
        {
            "js" => new JSException("no window"),
            "disconnected" => new JSDisconnectedException("circuit gone"),
            _ => new InvalidOperationException("no renderer"),
        };
        var cut = RenderGuard(dirty: true);

        await cut.Instance.DisposeAsync();

        Assert.Equal(new[] { true, false }, _ui.Prompts);
    }

    private sealed class StubDialogs : IDialogService
    {
        public bool? Result { get; set; }
        public int ConfirmCalls { get; private set; }
        public string? Title { get; private set; }
        public string? Message { get; private set; }
        public string? ConfirmLabel { get; private set; }
        public string? CancelLabel { get; private set; }

        public Task<bool?> ConfirmAsync(string title, string message,
            string confirmLabel = "OK", string cancelLabel = "Cancel")
        {
            ConfirmCalls++;
            Title = title;
            Message = message;
            ConfirmLabel = confirmLabel;
            CancelLabel = cancelLabel;
            return Task.FromResult(Result);
        }

        public Task AlertAsync(string title, string message, string closeLabel = "Close") => Task.CompletedTask;

        public Task<DialogResult> ShowAsync<T>(string? title = null, DialogParameters? parameters = null,
            DialogOptions? options = null) where T : IComponent => throw new NotSupportedException();

        public DialogReference Show<T>(string? title = null, DialogParameters? parameters = null,
            DialogOptions? options = null) where T : IComponent => throw new NotSupportedException();

        public Task<DialogResult> ShowSheetAsync<T>(string? title = null, DialogParameters? parameters = null,
            DialogOptions? options = null) where T : IComponent => throw new NotSupportedException();

        public event Action? OnStateChanged { add { } remove { } }

        public DialogRequest? Current => null;

        public IReadOnlyList<FlareDialogInstance> OpenDialogs => Array.Empty<FlareDialogInstance>();

        public void Respond(bool? confirmed) { }
    }

    private sealed class StubUi : IUiJsService
    {
        public List<bool> Prompts { get; } = new();
        public Exception? ThrowOnPrompt { get; set; }

        public ValueTask SetUnloadPromptAsync(bool enabled)
        {
            Prompts.Add(enabled);
            if (ThrowOnPrompt is { } error) throw error;
            return ValueTask.CompletedTask;
        }

        public ValueTask RegisterTabScrollerAsync<T>(ElementReference bar, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask ScrollTabsAsync(ElementReference bar, int direction) => ValueTask.CompletedTask;

        public ValueTask RevealActiveLinkTabAsync(ElementReference bar) => ValueTask.CompletedTask;

        public ValueTask RemoveTabScrollerAsync(ElementReference bar) => ValueTask.CompletedTask;

        public ValueTask RegisterButtonGroupCollapseAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask ApplyButtonGroupOverflowAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask RemoveButtonGroupCollapseAsync(ElementReference root) => ValueTask.CompletedTask;

        public ValueTask RegisterShortcutsAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class
            => ValueTask.CompletedTask;

        public ValueTask RemoveShortcutsAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> SupportsEyeDropperAsync() => ValueTask.FromResult(false);

        public ValueTask<string?> OpenEyeDropperAsync() => ValueTask.FromResult<string?>(null);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
