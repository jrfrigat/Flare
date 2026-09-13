namespace Flare.Components;

/// <summary>
/// Where something sits relative to what it hangs off: a side, and how it lines up along that side.
/// Shared by every component that places a surface next to an anchor, so a popover and a speed-dial
/// menu read the same words and mean the same thing.
/// </summary>
/// <remarks>
/// A bare side (<see cref="Top"/>) centres on the anchor. <c>Start</c> and <c>End</c> name the edge
/// that lines up: on a top or bottom side, the left or right edges; on a left or right side, the top
/// or bottom edges. These are the same values the anchored-panel engine uses
/// (<see cref="Flare.Components.Services.PanelPlacement"/>), so a component that goes through the engine
/// maps them one to one.
/// </remarks>
public enum Placement
{
    /// <summary>Above, centred on the anchor.</summary>
    Top,
    /// <summary>Above, left edges aligned.</summary>
    TopStart,
    /// <summary>Above, right edges aligned.</summary>
    TopEnd,
    /// <summary>Below, centred on the anchor.</summary>
    Bottom,
    /// <summary>Below, left edges aligned.</summary>
    BottomStart,
    /// <summary>Below, right edges aligned.</summary>
    BottomEnd,
    /// <summary>To the left, centred on the anchor.</summary>
    Left,
    /// <summary>To the left, top edges aligned.</summary>
    LeftStart,
    /// <summary>To the left, bottom edges aligned.</summary>
    LeftEnd,
    /// <summary>To the right, centred on the anchor.</summary>
    Right,
    /// <summary>To the right, top edges aligned.</summary>
    RightStart,
    /// <summary>To the right, bottom edges aligned.</summary>
    RightEnd,
}
