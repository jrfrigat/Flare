using Flare.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Flare.Infrastructure;

/// <summary>Default <see cref="Flare.Abstractions.IDialogService"/> backed by a host dialog component.</summary>
public sealed class DialogService : IDialogService
{
    private TaskCompletionSource<bool?>? _tcs;
    private readonly List<FlareDialogInstance> _openDialogs = [];

    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    public DialogRequest? Current { get; private set; }
    /// <summary>The component dialogs currently open, in display order.</summary>
    public IReadOnlyList<FlareDialogInstance> OpenDialogs => _openDialogs;
    /// <summary>Raised when the pending request changes so the host can re-render.</summary>
    public event Action? OnStateChanged;

    private Task<bool?> Show(DialogRequest request)
    {
        _tcs = new TaskCompletionSource<bool?>();
        Current = request;
        OnStateChanged?.Invoke();
        return _tcs.Task;
    }

    /// <summary>Shows a confirm dialog; resolves true=confirmed, false=cancelled, null=dismissed.</summary>
    public Task<bool?> ConfirmAsync(string title, string message,
        string confirmLabel = "OK", string cancelLabel = "Cancel")
        => Show(new(title, message, confirmLabel, cancelLabel, ShowCancel: true));

    /// <summary>Shows an alert dialog with a single close button.</summary>
    public async Task AlertAsync(string title, string message, string closeLabel = "Close")
        => await Show(new(title, message, closeLabel, string.Empty, ShowCancel: false));

    /// <summary>Opens a component dialog and awaits its result.</summary>
    public Task<DialogResult> ShowAsync<TComponent>(string? title = null,
        DialogParameters? parameters = null, DialogOptions? options = null)
        where TComponent : IComponent
    {
        // Note: DialogReference.Result is the awaitable Task, not a blocking Task.Result.
        DialogReference reference = Show<TComponent>(title, parameters, options);
        return reference.Result;
    }

    /// <summary>Opens a component dialog and returns a handle whose result can be awaited.</summary>
    public DialogReference Show<TComponent>(string? title = null,
        DialogParameters? parameters = null, DialogOptions? options = null)
        where TComponent : IComponent
    {
        var instance = new FlareDialogInstance(
            Guid.NewGuid(),
            typeof(TComponent),
            title,
            parameters ?? new DialogParameters(),
            options ?? new DialogOptions(),
            Remove);
        _openDialogs.Add(instance);
        OnStateChanged?.Invoke();
        return new DialogReference(instance);
    }

    /// <summary>Opens a component dialog presented as a bottom sheet and awaits its result.</summary>
    public Task<DialogResult> ShowSheetAsync<TComponent>(string? title = null,
        DialogParameters? parameters = null, DialogOptions? options = null)
        where TComponent : IComponent
    {
        // Copy the caller's options (never mutate a shared/Default instance) and force the sheet
        // presentation with a grabber. For a grabber-less sheet, call ShowAsync with
        // Position = Bottom and ShowGrabber = false instead.
        var sheet = options is null
            ? new DialogOptions { Position = DialogPosition.Bottom, ShowGrabber = true }
            : new DialogOptions
            {
                Size = options.Size,
                AriaLabel = options.AriaLabel,
                CloseOnScrimClick = options.CloseOnScrimClick,
                CloseOnEsc = options.CloseOnEsc,
                Divider = options.Divider,
                PanelClass = options.PanelClass,
                ScrimClass = options.ScrimClass,
                ShowCloseButton = options.ShowCloseButton,
                CloseOnNavigation = options.CloseOnNavigation,
                BeforeClose = options.BeforeClose,
                ShowGrabber = true,
                Position = DialogPosition.Bottom,
            };
        return ShowAsync<TComponent>(title, parameters, sheet);
    }

    // Called by a FlareDialogInstance when it closes (button, scrim, Escape, or programmatic close).
    private void Remove(FlareDialogInstance instance)
    {
        if (_openDialogs.Remove(instance))
            OnStateChanged?.Invoke();
    }

    /// <summary>Completes the pending request with the user's choice.</summary>
    public void Respond(bool? confirmed)
    {
        var tcs = _tcs;
        _tcs = null;
        Current = null;
        OnStateChanged?.Invoke();
        tcs?.TrySetResult(confirmed);
    }
}
