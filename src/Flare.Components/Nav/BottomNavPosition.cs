namespace Flare.Components;

/// <summary>How <c>FlareBottomNav</c> sits in the page.</summary>
public enum BottomNavPosition
{
    /// <summary>In the normal flow - the consumer's layout decides where the bar goes.</summary>
    Static,

    /// <summary>Sticks to the bottom of its scroll container once it would scroll out of view.</summary>
    Sticky,

    /// <summary>
    /// Pinned to the bottom of the viewport, above the content and below overlays. This is the PWA case:
    /// the bar keeps its safe-area inset and publishes its own height, so the layout can reserve space
    /// for it instead of hiding the last row of content behind it.
    /// </summary>
    Fixed,
}
