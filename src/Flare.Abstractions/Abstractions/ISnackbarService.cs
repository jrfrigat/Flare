namespace Flare.Core.Abstractions;

/// <summary>Severity of a snackbar notification, controlling its accent color and icon.</summary>
public enum SnackbarSeverity
{
    /// <summary>Neutral notification with no severity accent.</summary>
    Normal,
    /// <summary>Informational notification.</summary>
    Info,
    /// <summary>Success notification.</summary>
    Success,
    /// <summary>Warning notification.</summary>
    Warning,
    /// <summary>Error notification.</summary>
    Error,
}

/// <summary>Anchor position of the snackbar stack on screen.</summary>
public enum SnackbarPosition
{
    /// <summary>Top-left corner.</summary>
    TopLeft,
    /// <summary>Top edge, centered.</summary>
    TopCenter,
    /// <summary>Top-right corner.</summary>
    TopRight,
    /// <summary>Bottom-left corner.</summary>
    BottomLeft,
    /// <summary>Bottom edge, centered.</summary>
    BottomCenter,
    /// <summary>Bottom-right corner.</summary>
    BottomRight,
}

/// <summary>A single snackbar notification payload.</summary>
/// <param name="Id">Unique id used to track and dismiss the notification.</param>
/// <param name="Text">The message text.</param>
/// <param name="Severity">Severity accent.</param>
/// <param name="DurationMs">Auto-dismiss delay in milliseconds.</param>
/// <param name="ActionText">Optional action-button label; null hides the action.</param>
/// <param name="OnAction">Optional callback invoked when the action button is pressed.</param>
/// <param name="ShowClose">Whether a manual close button is shown.</param>
public sealed record SnackbarMessage(
    Guid Id,
    string Text,
    SnackbarSeverity Severity,
    int DurationMs,
    string? ActionText = null,
    Func<Task>? OnAction = null,
    bool ShowClose = true);

/// <summary>
/// Imperative snackbar/toast service. Inject it and call <see cref="Show"/> to enqueue a
/// notification rendered by the host snackbar component.
/// </summary>
public interface ISnackbarService
{
    /// <summary>Raised when a new snackbar should be displayed.</summary>
    event Action<SnackbarMessage>? OnShow;

    /// <summary>Enqueues a snackbar notification.</summary>
    void Show(string text,
              SnackbarSeverity severity = SnackbarSeverity.Normal,
              int durationMs = 4000,
              string? actionText = null,
              Func<Task>? onAction = null,
              bool showClose = true);
}
