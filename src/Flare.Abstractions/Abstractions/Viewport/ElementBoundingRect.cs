namespace Flare.Components;

/// <summary>
/// The geometry of an observed element (its <c>getBoundingClientRect()</c>) together with the ambient
/// viewport size and scroll offset, so callers can reason about the element's position relative to the
/// visible viewport without a second round-trip.
/// </summary>
/// <param name="Top">Distance from the viewport top edge to the element's top, in px.</param>
/// <param name="Left">Distance from the viewport left edge to the element's left, in px.</param>
/// <param name="Width">Element width in px.</param>
/// <param name="Height">Element height in px.</param>
/// <param name="WindowWidth">Viewport width in px at the time of measurement.</param>
/// <param name="WindowHeight">Viewport height in px at the time of measurement.</param>
/// <param name="ScrollX">Document horizontal scroll offset in px.</param>
/// <param name="ScrollY">Document vertical scroll offset in px.</param>
public readonly record struct ElementBoundingRect(
    double Top,
    double Left,
    double Width,
    double Height,
    double WindowWidth,
    double WindowHeight,
    double ScrollX,
    double ScrollY)
{
    /// <summary>The element's bottom edge relative to the viewport (<see cref="Top"/> + <see cref="Height"/>).</summary>
    public double Bottom => Top + Height;

    /// <summary>The element's right edge relative to the viewport (<see cref="Left"/> + <see cref="Width"/>).</summary>
    public double Right => Left + Width;

    /// <summary>The element's left edge in document coordinates (accounting for horizontal scroll).</summary>
    public double AbsoluteLeft => Left + ScrollX;

    /// <summary>The element's top edge in document coordinates (accounting for vertical scroll).</summary>
    public double AbsoluteTop => Top + ScrollY;

    /// <summary>True when the element extends past the bottom of the viewport.</summary>
    public bool IsOutsideBottom => Bottom > WindowHeight;

    /// <summary>True when the element extends past the top of the viewport.</summary>
    public bool IsOutsideTop => Top < 0;

    /// <summary>True when the element extends past the left of the viewport.</summary>
    public bool IsOutsideLeft => Left < 0;

    /// <summary>True when the element extends past the right of the viewport.</summary>
    public bool IsOutsideRight => Right > WindowWidth;
}
