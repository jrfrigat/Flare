namespace Flare.Components;

/// <summary>How a <see cref="FlareMenu"/> is opened from its activator.</summary>
public enum MenuActivation
{
    /// <summary>A normal left click / tap toggles the menu (default).</summary>
    LeftClick,
    /// <summary>A right click (context-menu gesture) opens the menu and suppresses the browser menu -
    /// pair with <see cref="FlareMenu.PositionAtCursor"/> for a true context menu.</summary>
    RightClick,
}

/// <summary>
/// Corner of the activator that the <see cref="FlareMenu"/> dropdown panel aligns to,
/// and the direction in which it opens. Logical (RTL-aware) aliases are also provided.
/// </summary>
public enum MenuAnchor
{
    /// <summary>Panel opens below the activator, aligned to the left / start edge.</summary>
    BottomLeft,
    /// <summary>Panel opens below the activator, aligned to the right / end edge.</summary>
    BottomRight,
    /// <summary>Panel opens above the activator, aligned to the left / start edge.</summary>
    TopLeft,
    /// <summary>Panel opens above the activator, aligned to the right / end edge.</summary>
    TopRight,

    // Logical (RTL-aware) aliases
    /// <summary>Alias for <see cref="BottomLeft"/> - panel opens below, start-aligned.</summary>
    BottomStart = BottomLeft,
    /// <summary>Alias for <see cref="BottomRight"/> - panel opens below, end-aligned.</summary>
    BottomEnd = BottomRight,
    /// <summary>Alias for <see cref="TopLeft"/> - panel opens above, start-aligned.</summary>
    TopStart = TopLeft,
    /// <summary>Alias for <see cref="TopRight"/> - panel opens above, end-aligned.</summary>
    TopEnd = TopRight,
}
