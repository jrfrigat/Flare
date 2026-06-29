namespace Flare.Components;

/// <summary>
/// Controls how a <see cref="FlareLayout"/> navigation drawer behaves, in particular what its
/// collapsed state looks like and how the user reaches grouped destinations from it. Supersedes the
/// legacy <see cref="FlareLayout.MiniRail"/> flag: set <see cref="FlareLayout.Mode"/> to pick a mode
/// explicitly, or leave it unset to fall back to <c>MiniRail</c>.
/// </summary>
public enum DrawerMode
{
    /// <summary>
    /// The drawer is always shown at full width and never collapses; the app-bar toggle is hidden.
    /// Use when the navigation should stay permanently visible.
    /// </summary>
    Expanded,

    /// <summary>
    /// The toggle hides the drawer completely (off-canvas), leaving only the content. This is the
    /// default and matches the historical behavior of a layout without a mini-rail.
    /// </summary>
    Collapsible,

    /// <summary>
    /// The toggle collapses the drawer to a static icon rail. Icons stay visible, but grouped
    /// destinations are only reachable after re-opening the drawer.
    /// </summary>
    Rail,

    /// <summary>
    /// An icon rail that expands into a full-width overlay while the pointer or keyboard focus is
    /// inside it, then collapses again on leave. The overlay floats over the content without
    /// reflowing it, so nested groups stay reachable without re-opening the drawer.
    /// </summary>
    RailHoverExpand,

    /// <summary>
    /// An icon rail where each top-level group opens its children in an anchored flyout menu on
    /// hover or focus, mirroring the Material Design 3 navigation rail. The rail itself stays
    /// compact while the flyout floats next to the hovered group's icon.
    /// </summary>
    RailFlyout,
}
