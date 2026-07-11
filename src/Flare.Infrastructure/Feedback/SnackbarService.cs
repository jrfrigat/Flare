using Flare.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Flare.Infrastructure;

/// <summary>Default <see cref="Flare.Abstractions.ISnackbarService"/> that relays notifications to the host snackbar component.</summary>
public sealed class SnackbarService : ISnackbarService
{
    /// <summary>Raised when a new snackbar should be displayed.</summary>
    public event Action<SnackbarMessage>? OnShow;

    /// <summary>Raised when an existing snackbar should be replaced in place.</summary>
    public event Action<SnackbarMessage>? OnUpdate;

    /// <summary>Raised when a single snackbar (matched by id) should be dismissed.</summary>
    public event Action<Guid>? OnRemove;

    /// <summary>Raised when every shown snackbar should be dismissed.</summary>
    public event Action? OnClear;

    /// <summary>Enqueues a snackbar notification for the host component to display.</summary>
    public void Show(string text,
                     SnackbarSeverity severity = SnackbarSeverity.Normal,
                     int durationMs = 4000,
                     string? actionText = null,
                     Func<Task>? onAction = null,
                     bool showClose = true)
        => OnShow?.Invoke(new SnackbarMessage(Guid.NewGuid(), text, severity, durationMs, actionText, onAction, showClose));

    /// <summary>Enqueues a snackbar notification configured by a <see cref="SnackbarOptions"/> bag.</summary>
    public void Show(string text, SnackbarOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        OnShow?.Invoke(new SnackbarMessage(
            Guid.NewGuid(), text, options.Severity, options.DurationMs,
            options.ActionText, options.OnAction, options.ShowClose, options.ShowProgress,
            options.CssClass, options.CloseAfterNavigation, options.PreventDuplicate));
    }

    /// <summary>Enqueues a snackbar whose body is a custom <see cref="RenderFragment"/> instead of text.</summary>
    public void Show(RenderFragment content, SnackbarOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        var o = options ?? new SnackbarOptions();
        OnShow?.Invoke(new SnackbarMessage(
            Guid.NewGuid(), string.Empty, o.Severity, o.DurationMs,
            o.ActionText, o.OnAction, o.ShowClose, o.ShowProgress,
            o.CssClass, o.CloseAfterNavigation, o.PreventDuplicate, content));
    }

    /// <summary>Enqueues a pre-built snackbar (the caller owns its <see cref="SnackbarMessage.Id"/>).</summary>
    public void Show(SnackbarMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        OnShow?.Invoke(message);
    }

    /// <summary>Asks the host component to replace the snackbar with the same id in place.</summary>
    public void Update(SnackbarMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        OnUpdate?.Invoke(message);
    }

    /// <summary>Asks the host component to dismiss the snackbar with the given id.</summary>
    public void Remove(Guid id) => OnRemove?.Invoke(id);

    /// <summary>Asks the host component to dismiss every shown snackbar.</summary>
    public void Clear() => OnClear?.Invoke();
}
