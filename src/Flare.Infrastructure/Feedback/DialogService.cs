using Flare.Core.Abstractions;

namespace Flare.Core.Services;

/// <summary>Default <see cref="Flare.Core.Abstractions.IDialogService"/> backed by a host dialog component.</summary>
public sealed class DialogService : IDialogService
{
    private TaskCompletionSource<bool?>? _tcs;

    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    public DialogRequest? Current { get; private set; }
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
