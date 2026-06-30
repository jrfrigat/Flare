using Microsoft.AspNetCore.Components;

namespace Flare.Abstractions;

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

    /// <summary>
    /// Opens <typeparamref name="TComponent"/> as a modal dialog body and awaits its result. The
    /// component receives the supplied <paramref name="parameters"/> and a cascaded
    /// <see cref="FlareDialogInstance"/> it uses to close itself (<c>Dialog.Close(value)</c> /
    /// <c>Dialog.Cancel()</c>). A dismissed dialog (scrim/Escape) resolves as
    /// <see cref="DialogResult.Cancel"/>.
    /// </summary>
    /// <typeparam name="TComponent">The Blazor component rendered as the dialog body.</typeparam>
    /// <param name="title">The dialog title, or null for a header-less dialog.</param>
    /// <param name="parameters">Values bound to the body component's parameters, or null for none.</param>
    /// <param name="options">Presentation options, or null for <see cref="DialogOptions.Default"/>.</param>
    /// <returns>The <see cref="DialogResult"/> produced when the dialog closes.</returns>
    Task<DialogResult> ShowAsync<TComponent>(string? title = null,
        DialogParameters? parameters = null, DialogOptions? options = null)
        where TComponent : IComponent;

    /// <summary>
    /// Opens <typeparamref name="TComponent"/> as a modal dialog body and returns a
    /// <see cref="DialogReference"/> whose <see cref="DialogReference.Result"/> can be awaited and
    /// which can also close the dialog from the caller side. Use this overload when you need the
    /// handle (e.g. to close the dialog in response to an external event);
    /// otherwise prefer <see cref="ShowAsync{TComponent}"/>.
    /// </summary>
    /// <typeparam name="TComponent">The Blazor component rendered as the dialog body.</typeparam>
    /// <param name="title">The dialog title, or null for a header-less dialog.</param>
    /// <param name="parameters">Values bound to the body component's parameters, or null for none.</param>
    /// <param name="options">Presentation options, or null for <see cref="DialogOptions.Default"/>.</param>
    /// <returns>A handle to the opened dialog.</returns>
    DialogReference Show<TComponent>(string? title = null,
        DialogParameters? parameters = null, DialogOptions? options = null)
        where TComponent : IComponent;

    // Provider communication
    /// <summary>Raised when the pending request changes, so the host component can re-render.</summary>
    event Action? OnStateChanged;
    /// <summary>The request currently awaiting a response, or null when no dialog is open.</summary>
    DialogRequest? Current { get; }
    /// <summary>
    /// The component dialogs currently open, in display order. The dialog provider host renders these
    /// (it is not part of the typical app-facing API).
    /// </summary>
    IReadOnlyList<FlareDialogInstance> OpenDialogs { get; }
    /// <summary>Completes the pending request: true=confirmed, false=cancelled, null=dismissed.</summary>
    /// <param name="confirmed">The user's choice, or null when dismissed.</param>
    void Respond(bool? confirmed);
}
