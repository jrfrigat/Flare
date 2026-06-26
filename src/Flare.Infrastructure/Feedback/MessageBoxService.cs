using Flare.Abstractions;

namespace Flare.Infrastructure;

/// <summary>Default <see cref="Flare.Abstractions.IMessageBoxService"/> backed by a host message-box component.</summary>
public sealed class MessageBoxService : IMessageBoxService
{
    private TaskCompletionSource<string?>? _tcs;

    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    public MessageBoxRequest? Current { get; private set; }
    /// <summary>Raised when the pending request changes so the host can re-render.</summary>
    public event Action? OnStateChanged;

    /// <summary>Shows a prompt dialog with a text input; resolves to the entered text or null when cancelled.</summary>
    public Task<string?> PromptAsync(string title, string label = "", string defaultValue = "",
        string confirmLabel = "OK", string cancelLabel = "Cancel")
    {
        _tcs = new TaskCompletionSource<string?>();
        Current = new(title, label, defaultValue, confirmLabel, cancelLabel, MessageBoxKind.Prompt);
        OnStateChanged?.Invoke();
        return _tcs.Task;
    }

    /// <summary>Shows a confirmation dialog; resolves true when confirmed, false when cancelled.</summary>
    public async Task<bool> ConfirmAsync(string title, string message = "",
        string confirmLabel = "Yes", string cancelLabel = "No")
    {
        _tcs = new TaskCompletionSource<string?>();
        Current = new(title, message, string.Empty, confirmLabel, cancelLabel, MessageBoxKind.Confirm);
        OnStateChanged?.Invoke();
        var result = await _tcs.Task;
        return result is not null;
    }

    /// <summary>Shows an information alert; resolves when dismissed.</summary>
    public async Task AlertAsync(string title, string message = "", string confirmLabel = "OK")
    {
        _tcs = new TaskCompletionSource<string?>();
        Current = new(title, message, string.Empty, confirmLabel, string.Empty, MessageBoxKind.Alert);
        OnStateChanged?.Invoke();
        await _tcs.Task;
    }

    /// <summary>Completes the pending request with the given value (input text or null when cancelled).</summary>
    public void Respond(string? value)
    {
        var tcs = _tcs;
        _tcs = null;
        Current = null;
        OnStateChanged?.Invoke();
        tcs?.TrySetResult(value);
    }
}
