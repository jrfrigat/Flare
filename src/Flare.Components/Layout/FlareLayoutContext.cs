namespace Flare.Components;

/// <summary>
/// Cascading context that connects FlareLayout, FlareLayoutAppBar, and FlareLayoutDrawer
/// so the drawer can be toggled from the app bar without manual @bind wiring.
/// </summary>
public class FlareLayoutContext
{
    private bool _drawerOpen = true;
    private bool _miniRail;

    /// <summary>Whether the drawer is open. Setting it notifies subscribers.</summary>
    public bool DrawerOpen
    {
        get => _drawerOpen;
        set
        {
            if (_drawerOpen == value) return;
            _drawerOpen = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>Whether the layout keeps a narrow icon rail when the drawer is collapsed (FlareLayout.MiniRail).</summary>
    public bool MiniRail
    {
        get => _miniRail;
        set
        {
            if (_miniRail == value) return;
            _miniRail = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// True when the drawer is collapsed into the mini icon rail, so a nested <c>FlareNavMenu</c>
    /// renders icon-only. Equivalent to <c>MiniRail &amp;&amp; !DrawerOpen</c>.
    /// </summary>
    public bool RailCollapsed => _miniRail && !_drawerOpen;

    /// <summary>Raised when the drawer or mini-rail state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Toggles the drawer open/closed and notifies subscribers.</summary>
    public void ToggleDrawer() => DrawerOpen = !DrawerOpen;
}
