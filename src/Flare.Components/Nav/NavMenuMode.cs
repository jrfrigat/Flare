namespace Flare.Components;

/// <summary>
/// How a <see cref="FlareNavMenu"/> presents its items. Set it explicitly to control the menu
/// independently of any surrounding <c>FlareLayout</c>; leave it unset to keep the legacy behavior
/// (driven by <see cref="FlareNavMenu.Rail"/> or the collapsed layout drawer).
/// </summary>
public enum NavMenuMode
{
    /// <summary>The full menu: labels, group titles, expand chevrons and badges are all shown.</summary>
    Full,

    /// <summary>
    /// Mini-rail: every item is trimmed to its icon only -- link/group text, the group expand
    /// chevron and link badges are hidden and the icons are centered.
    /// </summary>
    Rail,
}
