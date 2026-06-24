namespace Flare.Components;

/// <summary>Access the confirm dialog via [CascadingParameter] FlareConfirmDialogProvider.</summary>
public interface IConfirmDialogService
{
    /// <summary>
    /// Shows a confirm dialog. Returns <see langword="true"/> when confirmed,
    /// <see langword="false"/> when the cancel button is clicked,
    /// or <see langword="null"/> when dismissed (e.g. Escape key).
    /// </summary>
    Task<bool?> ConfirmAsync(string title, string message,
        string? confirmLabel = null, string? cancelLabel = null);
}
