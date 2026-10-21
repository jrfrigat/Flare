namespace Flare.Components;

/// <summary>How <c>FlareBottomNav</c> lays out an item's label.</summary>
public enum BottomNavLabelMode
{
    /// <summary>
    /// One line at the theme's bar height; a label that does not fit ends with an ellipsis. This is the
    /// default and keeps the bar exactly as tall as the theme says.
    /// </summary>
    SingleLine,

    /// <summary>
    /// Up to two lines, and the bar grows (with the space it reserves) to the height those two lines
    /// need. At a large root font size a five-item bar can no longer show its labels on one line, and
    /// this mode keeps them readable instead of ending each with an ellipsis.
    /// </summary>
    Wrap,
}
