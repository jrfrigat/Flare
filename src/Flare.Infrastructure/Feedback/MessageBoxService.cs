using Flare.Abstractions;

namespace Flare.Infrastructure;

/// <summary>Default <see cref="Flare.Abstractions.IMessageBoxService"/> backed by a host message-box component.</summary>
public sealed class MessageBoxService : IMessageBoxService
{
    // Requests in call order; the head is the one on screen.
    private readonly Queue<(MessageBoxRequest Request, TaskCompletionSource<string?> Tcs)> _pending = new();

    /// <summary>
    /// The request currently awaiting a response, or null when no dialog is open. Requests made while one
    /// is open wait their turn and become current, in call order, as each is answered.
    /// </summary>
    public MessageBoxRequest? Current => _pending.TryPeek(out var head) ? head.Request : null;
    /// <summary>Raised when the pending request changes so the host can re-render.</summary>
    public event Action? OnStateChanged;

    /// <summary>Shows a prompt dialog with a text input; resolves to the entered text or null when cancelled.</summary>
    public Task<string?> PromptAsync(string title, string label = "", string defaultValue = "",
        string confirmLabel = "OK", string cancelLabel = "Cancel")
        => Show(new(title, label, defaultValue, confirmLabel, cancelLabel, MessageBoxKind.Prompt));

    /// <summary>Shows a confirmation dialog; resolves true when confirmed, false when cancelled.</summary>
    public async Task<bool> ConfirmAsync(string title, string message = "",
        string confirmLabel = "Yes", string cancelLabel = "No")
    {
        var result = await Show(
            new(title, message, string.Empty, confirmLabel, cancelLabel, MessageBoxKind.Confirm));
        return result is not null;
    }

    /// <summary>Shows an information alert; resolves when dismissed.</summary>
    public async Task AlertAsync(string title, string message = "", string confirmLabel = "OK")
        => await Show(new(title, message, string.Empty, confirmLabel, string.Empty, MessageBoxKind.Alert));

    /// <summary>
    /// Completes the current request with the given value (input text or null when cancelled) and brings up
    /// the next queued one.
    /// </summary>
    public void Respond(string? value)
    {
        if (!_pending.TryDequeue(out var answered))
            return;
        OnStateChanged?.Invoke();
        answered.Tcs.TrySetResult(value);
    }

    private Task<string?> Show(MessageBoxRequest request)
    {
        var tcs = new TaskCompletionSource<string?>();
        _pending.Enqueue((request, tcs));
        // A queued request changes nothing on screen until the ones before it are answered.
        if (_pending.Count == 1)
            OnStateChanged?.Invoke();
        return tcs.Task;
    }
}
