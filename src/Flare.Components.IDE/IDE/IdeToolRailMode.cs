namespace Flare.Components.IDE;

/// <summary>
/// How the rail of a collapsed or auto-hidden side region lists its tools in a
/// <see cref="FlareIdeLayout"/>.
/// </summary>
public enum IdeToolRailMode
{
    /// <summary>One button for the whole region: it labels every hosted tool and expands the region.
    /// This is the default and the behavior every consumer had before the option existed.</summary>
    Region,

    /// <summary>One entry per hosted tool in an auto-hide side region, with the tool's icon and
    /// rotated label. Selecting an entry opens that tool in the region's flyout; the active tool is
    /// marked and selecting it again closes the flyout. Regions in the docked mode, the bottom
    /// region and regions without a hosted tool group keep the region button.</summary>
    PerTool,
}
