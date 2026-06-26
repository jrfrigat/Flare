namespace Flare.Core.Abstractions;

/// <summary>An immutable description of a pending confirm/alert dialog request.</summary>
/// <param name="Title">Dialog title.</param>
/// <param name="Message">Dialog body message.</param>
/// <param name="ConfirmLabel">Label for the confirm button.</param>
/// <param name="CancelLabel">Label for the cancel button.</param>
/// <param name="ShowCancel">Whether the cancel button is shown (false for a pure alert).</param>
public sealed record DialogRequest(
    string Title,
    string Message,
    string ConfirmLabel,
    string CancelLabel,
    bool ShowCancel);

/// <summary>
/// Imperative confirm/alert dialog service, surfaced by a single host component at the app root.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a confirm dialog. Returns <see langword="true"/> when confirmed,
    /// <see langword="false"/> when cancelled, or <see langword="null"/> when dismissed (Escape).
    /// </summary>
    Task<bool?> ConfirmAsync(string title, string message,
        string confirmLabel = "OK", string cancelLabel = "Cancel");
    /// <summary>Shows an alert dialog with a single close button. Returns when dismissed.</summary>
    Task AlertAsync(string title, string message, string closeLabel = "Close");

    // Provider communication
    /// <summary>Raised when the pending request changes, so the host component can re-render.</summary>
    event Action? OnStateChanged;
    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    DialogRequest? Current { get; }
    /// <summary>Completes the pending request: true=confirmed, false=cancelled, null=dismissed.</summary>
    /// <param name="confirmed">The user's choice, or null when dismissed.</param>
    void Respond(bool? confirmed);
}
