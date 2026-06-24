using Flare.Core.Abstractions;

namespace Flare.Core.Services;

/// <summary>Default <see cref="Flare.Core.Abstractions.ISnackbarService"/> that relays notifications to the host snackbar component.</summary>
public sealed class SnackbarService : ISnackbarService
{
    /// <summary>Raised when a new snackbar should be displayed.</summary>
    public event Action<SnackbarMessage>? OnShow;

    /// <summary>Enqueues a snackbar notification for the host component to display.</summary>
    public void Show(string text,
                     SnackbarSeverity severity = SnackbarSeverity.Normal,
                     int durationMs = 4000,
                     string? actionText = null,
                     Func<Task>? onAction = null,
                     bool showClose = true)
        => OnShow?.Invoke(new SnackbarMessage(Guid.NewGuid(), text, severity, durationMs, actionText, onAction, showClose));
}
