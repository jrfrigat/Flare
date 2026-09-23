using Flare.Components.Services;

namespace Flare.Components;

/// <summary>Maps the shared <see cref="Placement"/> onto the anchored-panel engine's placement strings.</summary>
internal static class PlacementExtensions
{
    /// <summary>The engine placement (side plus alignment), e.g. <c>top-start</c>.</summary>
    public static string ToPanelPlacement(this Placement placement) => placement switch
    {
        Placement.Top        => PanelPlacement.Top,
        Placement.TopStart   => PanelPlacement.TopStart,
        Placement.TopEnd     => PanelPlacement.TopEnd,
        Placement.Bottom     => PanelPlacement.Bottom,
        Placement.BottomEnd  => PanelPlacement.BottomEnd,
        Placement.Left       => PanelPlacement.Left,
        Placement.LeftStart  => PanelPlacement.LeftStart,
        Placement.LeftEnd    => PanelPlacement.LeftEnd,
        Placement.Right      => PanelPlacement.Right,
        Placement.RightStart => PanelPlacement.RightStart,
        Placement.RightEnd   => PanelPlacement.RightEnd,
        _                    => PanelPlacement.BottomStart,
    };

    /// <summary>The side alone - top, bottom, left or right - which is what an arrow points away from.</summary>
    public static string ToPanelSide(this Placement placement) => placement.ToPanelPlacement().Split('-')[0];
}
