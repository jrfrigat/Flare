namespace Flare.Components;

/// <summary>
/// Cascading context that connects FlareLayout, FlareLayoutAppBar, and FlareLayoutDrawer
/// so the drawer can be toggled from the app bar without manual @bind wiring.
/// </summary>
public class FlareLayoutContext
{
    private bool _drawerOpen = true;
    private bool _miniRail;
    private bool _railHoverExpand;
    private bool _railHoverExpanded;
    private bool _isMobile;

    /// <summary>Whether the drawer is open. Setting it notifies subscribers.</summary>
    public bool DrawerOpen
    {
        get => _drawerOpen;
        set
        {
            if (_drawerOpen == value) return;
            _drawerOpen = value;
            // Opening (or otherwise leaving the collapsed rail) drops any pending hover overlay so it
            // never lingers over the full-width drawer.
            if (value) _railHoverExpanded = false;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// Whether collapsing the drawer leaves a narrow icon rail rather than hiding it entirely. Set by
    /// <see cref="FlareLayout"/> from its <c>MiniRail</c> parameter. Setting it notifies subscribers.
    /// </summary>
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
    /// Whether the collapsed mini-rail may temporarily expand into a full-width overlay while the
    /// pointer or keyboard focus is inside it (opt-in). Set by <see cref="FlareLayout"/> from its
    /// <c>RailHoverExpand</c> parameter. Setting it notifies subscribers.
    /// </summary>
    public bool RailHoverExpand
    {
        get => _railHoverExpand;
        set
        {
            if (_railHoverExpand == value) return;
            _railHoverExpand = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// Whether the layout is currently in its mobile (off-canvas) breakpoint. Set by
    /// <see cref="FlareLayout"/> from the responsive breakpoint watcher. Setting it notifies
    /// subscribers.
    /// </summary>
    public bool IsMobile
    {
        get => _isMobile;
        set
        {
            if (_isMobile == value) return;
            _isMobile = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// True when the drawer is collapsed into the mini icon rail. Equivalent to
    /// <c>MiniRail &amp;&amp; !DrawerOpen</c>. This is the physical drawer state and ignores any
    /// temporary hover overlay; use <see cref="RailIconOnly"/> to decide how a nested
    /// <c>FlareNavMenu</c> should render.
    /// </summary>
    public bool RailCollapsed => _miniRail && !_drawerOpen;

    /// <summary>
    /// Requests the collapsed mini-rail to temporarily expand into a full-width overlay so its
    /// nested groups become reachable. <see cref="FlareLayoutDrawer"/> sets this while the pointer
    /// or keyboard focus is inside the rail. Only takes effect when <see cref="RailHoverExpand"/> is
    /// enabled and <see cref="RailCollapsed"/> is true. Setting it notifies subscribers.
    /// </summary>
    public bool RailHoverExpanded
    {
        get => _railHoverExpanded;
        set
        {
            if (_railHoverExpanded == value) return;
            _railHoverExpanded = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// True when the collapsed mini-rail is currently expanded into its hover/focus overlay
    /// (<see cref="RailHoverExpand"/> enabled, collapsed, and hovered/focused). The layout uses this
    /// to float the drawer at full width over the content without reflowing it.
    /// </summary>
    public bool RailOverlayOpen => _railHoverExpand && RailCollapsed && _railHoverExpanded;

    /// <summary>
    /// True when a nested <c>FlareNavMenu</c> should render icon-only: the drawer is collapsed into
    /// the mini-rail and is not currently expanded by hover or focus. While the overlay is open the
    /// menu renders in full so its labels and nested groups are usable.
    /// </summary>
    public bool RailIconOnly => RailCollapsed && !RailOverlayOpen;

    /// <summary>Raised when the drawer or mini-rail state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Toggles the drawer open/closed and notifies subscribers.</summary>
    public void ToggleDrawer() => DrawerOpen = !DrawerOpen;
}
