namespace Flare.Components;

/// <summary>
/// Cascading context that connects FlareLayout, FlareLayoutAppBar, and FlareLayoutDrawer
/// so the drawer can be toggled from the app bar without manual @bind wiring.
/// </summary>
public class FlareLayoutContext
{
    private bool _drawerOpen = true;
    private DrawerMode _mode = DrawerMode.Collapsible;
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
            // Opening (or otherwise leaving the collapsed rail) drops any pending hover overlay so
            // it can never linger over the full-width drawer.
            if (value) _railHoverExpanded = false;
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// How the drawer behaves, especially in its collapsed state. <see cref="DrawerMode.Expanded"/>
    /// pins the drawer open. Setting it notifies subscribers.
    /// </summary>
    public DrawerMode Mode
    {
        get => _mode;
        set
        {
            if (_mode == value) return;
            _mode = value;
            // Expanded mode keeps the drawer permanently open; never leave it collapsed.
            if (value == DrawerMode.Expanded) { _drawerOpen = true; _railHoverExpanded = false; }
            StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// Whether the layout is currently in its mobile (off-canvas) breakpoint. Set by
    /// <see cref="FlareLayout"/> from the responsive breakpoint watcher; used so the app-bar toggle
    /// stays available on mobile even in <see cref="DrawerMode.Expanded"/>. Setting it notifies
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
    /// True when the app-bar drawer toggle should be hidden: only in
    /// <see cref="DrawerMode.Expanded"/> on desktop, where the drawer is permanently pinned open. On
    /// mobile the toggle is always shown because the drawer is an off-canvas overlay.
    /// </summary>
    public bool ToggleHidden => _mode == DrawerMode.Expanded && !_isMobile;

    /// <summary>
    /// True when the drawer collapses to an icon rail rather than disappearing -- any of
    /// <see cref="DrawerMode.Rail"/>, <see cref="DrawerMode.RailHoverExpand"/> or
    /// <see cref="DrawerMode.RailFlyout"/>. Back-compat alias for the former <c>MiniRail</c> flag.
    /// </summary>
    public bool MiniRail => _mode is DrawerMode.Rail or DrawerMode.RailHoverExpand or DrawerMode.RailFlyout;

    /// <summary>
    /// True when the drawer is collapsed into the mini icon rail. Equivalent to
    /// <c>MiniRail &amp;&amp; !DrawerOpen</c>. This is the physical drawer state and ignores any
    /// temporary hover overlay; use <see cref="RailIconOnly"/> to decide how a nested
    /// <c>FlareNavMenu</c> should render.
    /// </summary>
    public bool RailCollapsed => MiniRail && !_drawerOpen;

    /// <summary>
    /// Requests the collapsed mini-rail to temporarily expand into a full-width overlay so its
    /// nested groups become reachable. <see cref="FlareLayoutDrawer"/> sets this while the pointer
    /// or keyboard focus is inside the rail. Only takes effect in
    /// <see cref="DrawerMode.RailHoverExpand"/> while <see cref="RailCollapsed"/> is true. Setting
    /// it notifies subscribers.
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
    /// (<see cref="DrawerMode.RailHoverExpand"/>, collapsed, and hovered/focused). The layout uses
    /// this to float the drawer at full width over the content without reflowing it.
    /// </summary>
    public bool RailOverlayOpen => _mode == DrawerMode.RailHoverExpand && RailCollapsed && _railHoverExpanded;

    /// <summary>
    /// True when a nested <c>FlareNavMenu</c> should render icon-only: the drawer is collapsed into
    /// the mini-rail and is not currently expanded by hover or focus. While the overlay is open the
    /// menu renders in full so its labels and nested groups are usable.
    /// </summary>
    public bool RailIconOnly => RailCollapsed && !RailOverlayOpen;

    /// <summary>
    /// True when collapsed top-level groups should open their children in an anchored flyout
    /// (<see cref="DrawerMode.RailFlyout"/> while <see cref="RailCollapsed"/>). <c>FlareNavGroup</c>
    /// reads this to switch a top-level group from inline expansion to a floating menu.
    /// </summary>
    public bool RailFlyout => _mode == DrawerMode.RailFlyout && RailCollapsed;

    /// <summary>Raised when the drawer or mini-rail state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Toggles the drawer open/closed and notifies subscribers. No-op while the drawer is
    /// pinned open (<see cref="DrawerMode.Expanded"/> on desktop); still works on mobile, where even
    /// an expanded drawer is an off-canvas overlay.</summary>
    public void ToggleDrawer()
    {
        if (ToggleHidden) return;
        DrawerOpen = !DrawerOpen;
    }
}
