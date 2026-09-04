namespace Flare.Components.Services;

/// <summary>
/// Where an anchored panel sits relative to its anchor. The side flips to the opposite one when the
/// panel does not fit and the opposite side has more room; the alignment never flips, it is clamped
/// to the viewport. A bare side (<see cref="Bottom"/>) centres the panel on the anchor.
/// </summary>
public static class PanelPlacement
{
    /// <summary>Below the anchor, left edges aligned. The default.</summary>
    public const string BottomStart = "bottom-start";
    /// <summary>Below the anchor, centred on it.</summary>
    public const string Bottom = "bottom";
    /// <summary>Below the anchor, right edges aligned.</summary>
    public const string BottomEnd = "bottom-end";
    /// <summary>Above the anchor, left edges aligned.</summary>
    public const string TopStart = "top-start";
    /// <summary>Above the anchor, centred on it.</summary>
    public const string Top = "top";
    /// <summary>Above the anchor, right edges aligned.</summary>
    public const string TopEnd = "top-end";
    /// <summary>To the right of the anchor, top edges aligned.</summary>
    public const string RightStart = "right-start";
    /// <summary>To the right of the anchor, centred on it.</summary>
    public const string Right = "right";
    /// <summary>To the right of the anchor, bottom edges aligned.</summary>
    public const string RightEnd = "right-end";
    /// <summary>To the left of the anchor, top edges aligned.</summary>
    public const string LeftStart = "left-start";
    /// <summary>To the left of the anchor, centred on it.</summary>
    public const string Left = "left";
    /// <summary>To the left of the anchor, bottom edges aligned.</summary>
    public const string LeftEnd = "left-end";
}

/// <summary>
/// A point inside the anchor element, as a percentage of its box, used when a panel anchors to a
/// position rather than to the element itself - a chart tooltip on a data point. The element is
/// re-measured on scroll, so the point stays correct without the caller re-sending it.
/// </summary>
/// <param name="XPct">Horizontal position, 0-100, measured from the anchor's left edge.</param>
/// <param name="YPct">Vertical position, 0-100, measured from the anchor's top edge.</param>
public readonly record struct PanelAnchorPoint(double XPct, double YPct);

/// <summary>
/// A point inside the anchor element in pixels from its top-left corner, for a position the caller
/// already measured that way - a code editor's caret. Like <see cref="PanelAnchorPoint"/>, the element
/// is re-measured on scroll, so the offset stays correct without being re-sent.
/// </summary>
/// <param name="X">Distance from the anchor's left edge, in pixels.</param>
/// <param name="Y">Distance from the anchor's top edge, in pixels.</param>
public readonly record struct PanelAnchorOffset(double X, double Y);

/// <summary>
/// An anchor with no element behind it, in viewport coordinates - a context menu pinned to the
/// pointer. It does not follow scrolling, because there is nothing to re-measure.
/// </summary>
/// <param name="X">Distance from the viewport's left edge, in pixels.</param>
/// <param name="Y">Distance from the viewport's top edge, in pixels.</param>
/// <param name="Width">Anchor width in pixels; zero for a point.</param>
/// <param name="Height">Anchor height in pixels; zero for a point.</param>
public readonly record struct PanelAnchorRect(double X, double Y, double Width = 0, double Height = 0);

/// <summary>
/// Placement options for <see cref="IOverlayJsService.PositionAnchoredPanelAsync"/>. Every floating
/// surface in the library goes through it, so the panel escapes whatever clips its component - a
/// card's <c>overflow: hidden</c>, a grid's scroll container, a dialog - and paints above the page.
/// </summary>
public sealed class AnchoredPanelOptions
{
    /// <summary>Side and alignment; one of the <see cref="PanelPlacement"/> values.</summary>
    public string Placement { get; init; } = PanelPlacement.BottomStart;

    /// <summary>Distance between the anchor and the panel, in pixels.</summary>
    public int Gap { get; init; } = 4;

    /// <summary>
    /// Keeps the panel at least as wide as the anchor. It still grows past that when its content needs
    /// the room, and never past the viewport.
    /// </summary>
    public bool MatchWidth { get; init; }

    /// <summary>Anchors to a point inside the anchor element instead of to its whole box.</summary>
    public PanelAnchorPoint? AnchorPoint { get; init; }

    /// <summary>Anchors to a point inside the anchor element given in pixels rather than percent.</summary>
    public PanelAnchorOffset? AnchorOffset { get; init; }

    /// <summary>Anchors to viewport coordinates instead of to an element.</summary>
    public PanelAnchorRect? AnchorRect { get; init; }

    /// <summary>
    /// Whether to promote the panel to the browser's top layer, where nothing on the page can paint
    /// over it or clip it. Leave it on unless the panel must stay inside its component's own stacking
    /// order. A browser without the popover API keeps the plain fixed panel either way.
    /// </summary>
    public bool TopLayer { get; init; } = true;
}
