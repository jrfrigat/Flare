namespace Flare.Components;

/// <summary>
/// Coordinates the top-level <see cref="FlareMenu"/> instances of one menu bar: at most one is open,
/// opening another closes the previous one without pulling the focus back, and the owner renders one
/// shared backdrop for the whole bar. A bar component cascades an instance, so a menu placed inside it
/// joins the coordination automatically; a host-owned bar must render the shared backdrop itself and
/// call <see cref="CloseOpenMenuAsync"/> when that backdrop is clicked.
/// </summary>
public sealed class FlareMenuBarContext
{
    private FlareMenu? _openMenu;

    /// <summary>Whether a coordinated menu is currently open. The owner uses this to decide whether
    /// the shared backdrop is rendered.</summary>
    public bool HasOpenMenu => _openMenu is not null;

    /// <summary>Raised when the open menu changes - one opened, closed or replaced - so the owner can
    /// render the shared backdrop again.</summary>
    public event Action? StateChanged;

    /// <summary>Closes the open menu, if there is one. Focus returns to that menu's own activator,
    /// which is what a click outside the bar should do.</summary>
    public Task CloseOpenMenuAsync() => _openMenu is { } menu ? menu.CloseAsync() : Task.CompletedTask;

    // The new menu is recorded before the previous one closes: its close notification then sees that it
    // is no longer the open menu and cannot clear the state the replacement just set.
    internal async Task NotifyOpenedAsync(FlareMenu menu)
    {
        var previous = _openMenu;
        _openMenu = menu;
        if (previous is not null && !ReferenceEquals(previous, menu))
            await previous.CloseFromMenuBarAsync();
        StateChanged?.Invoke();
    }

    internal void NotifyClosed(FlareMenu menu)
    {
        if (!ReferenceEquals(_openMenu, menu)) return;
        _openMenu = null;
        StateChanged?.Invoke();
    }
}
