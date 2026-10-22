namespace Flare.Components;

/// <summary>
/// Where something sits relative to what it hangs off: a side, and how it lines up along that side.
/// Shared by every component that places a surface next to an anchor, so a popover and a speed-dial
/// menu read the same words and mean the same thing.
/// </summary>
/// <remarks>
/// A bare side (<see cref="Top"/>) centres on the anchor. On a top or bottom side, <c>Start</c> and
/// <c>End</c> name the edge along the INLINE axis: the writing direction's start or end edge, which is
/// the left/right edge in LTR and the right/left edge in RTL. On a left or right side they name the
/// BLOCK edges - the top or bottom edge - which the writing direction never moves. <see cref="Left"/>
/// and <see cref="Right"/> are physical sides and are never mirrored. These are the same values the
/// anchored-panel engine uses (<see cref="Flare.Components.Services.PanelPlacement"/>), so a component
/// that goes through the engine maps them one to one.
/// </remarks>
public enum Placement
{
    /// <summary>Above, centred on the anchor.</summary>
    Top,
    /// <summary>Above, aligned to the writing direction's start edge (left in LTR, right in RTL).</summary>
    TopStart,
    /// <summary>Above, aligned to the writing direction's end edge (right in LTR, left in RTL).</summary>
    TopEnd,
    /// <summary>Below, centred on the anchor.</summary>
    Bottom,
    /// <summary>Below, aligned to the writing direction's start edge (left in LTR, right in RTL).</summary>
    BottomStart,
    /// <summary>Below, aligned to the writing direction's end edge (right in LTR, left in RTL).</summary>
    BottomEnd,
    /// <summary>To the left, centred on the anchor.</summary>
    Left,
    /// <summary>To the left, top edges aligned (the block start; the writing direction does not move it).</summary>
    LeftStart,
    /// <summary>To the left, bottom edges aligned (the block end; the writing direction does not move it).</summary>
    LeftEnd,
    /// <summary>To the right, centred on the anchor.</summary>
    Right,
    /// <summary>To the right, top edges aligned (the block start; the writing direction does not move it).</summary>
    RightStart,
    /// <summary>To the right, bottom edges aligned (the block end; the writing direction does not move it).</summary>
    RightEnd,
}
