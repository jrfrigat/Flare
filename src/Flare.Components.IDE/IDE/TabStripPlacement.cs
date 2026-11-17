namespace Flare.Components.IDE;

/// <summary>
/// Which edge of a <see cref="FlareToolPanelGroup"/> carries its tool tab strip.
/// </summary>
public enum TabStripPlacement
{
    /// <summary>The tab strip sits in the group header, above the panel body. Default.</summary>
    Top,

    /// <summary>The tab strip sits below the panel body; the header keeps the active tool's title and
    /// icon, the host actions and the region collapse button.</summary>
    Bottom,
}
