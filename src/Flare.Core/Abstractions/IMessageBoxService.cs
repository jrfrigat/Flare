namespace Flare.Core.Abstractions;

/// <summary>
/// Imperative message-box service for prompt, confirm and alert dialogs, surfaced by a single
/// host component placed at the app root. Inject it to request a dialog from code.
/// </summary>
public interface IMessageBoxService
{
    /// <summary>Shows a prompt dialog with a text input. Returns null if cancelled.</summary>
    Task<string?> PromptAsync(string title, string label = "", string defaultValue = "",
        string confirmLabel = "OK", string cancelLabel = "Cancel");

    /// <summary>Shows a confirmation dialog. Returns true when the user confirms, false when cancelled.</summary>
    Task<bool> ConfirmAsync(string title, string message = "",
        string confirmLabel = "Yes", string cancelLabel = "No");

    /// <summary>Shows an information alert dialog. Returns when the user dismisses it.</summary>
    Task AlertAsync(string title, string message = "", string confirmLabel = "OK");

    // Provider communication
    /// <summary>Raised when the pending request changes, so the host component can re-render.</summary>
    event Action? OnStateChanged;
    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    MessageBoxRequest? Current { get; }
    /// <summary>Completes the pending request with the given value (the input text, "ok", or null when cancelled).</summary>
    /// <param name="value">The user's response, or null when the dialog was dismissed/cancelled.</param>
    void Respond(string? value);
}

/// <summary>The kind of message box to display.</summary>
public enum MessageBoxKind
{
    /// <summary>A dialog with a text input that returns the entered string.</summary>
    Prompt,
    /// <summary>A yes/no confirmation dialog that returns a boolean.</summary>
    Confirm,
    /// <summary>An information dialog with a single dismiss button.</summary>
    Alert,
}

/// <summary>An immutable description of a pending message-box request.</summary>
/// <param name="Title">Dialog title.</param>
/// <param name="Label">Input label (prompt) or body message.</param>
/// <param name="DefaultValue">Initial value for the prompt input.</param>
/// <param name="ConfirmLabel">Label for the confirm/OK button.</param>
/// <param name="CancelLabel">Label for the cancel button.</param>
/// <param name="Kind">Which message-box variant to render.</param>
public sealed record MessageBoxRequest(
    string Title,
    string Label,
    string DefaultValue,
    string ConfirmLabel,
    string CancelLabel,
    MessageBoxKind Kind = MessageBoxKind.Prompt);
