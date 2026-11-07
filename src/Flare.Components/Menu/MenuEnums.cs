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
/// Logical edge of the activator that the <see cref="FlareMenu"/> dropdown panel aligns to,
/// and the direction in which it opens. Start is left in LTR and right in RTL; end is the opposite.
/// Prefer the Start/End names. The Left/Right names are retained for compatibility and also follow text direction.
/// </summary>
public enum MenuAnchor
{
    /// <summary>Compatibility name for <see cref="BottomStart"/>. Opens below at logical start: left in LTR, right in RTL.</summary>
    BottomLeft,
    /// <summary>Compatibility name for <see cref="BottomEnd"/>. Opens below at logical end: right in LTR, left in RTL.</summary>
    BottomRight,
    /// <summary>Compatibility name for <see cref="TopStart"/>. Opens above at logical start: left in LTR, right in RTL.</summary>
    TopLeft,
    /// <summary>Compatibility name for <see cref="TopEnd"/>. Opens above at logical end: right in LTR, left in RTL.</summary>
    TopRight,

    /// <summary>Opens below at logical start: left in LTR, right in RTL.</summary>
    BottomStart = BottomLeft,
    /// <summary>Opens below at logical end: right in LTR, left in RTL.</summary>
    BottomEnd = BottomRight,
    /// <summary>Opens above at logical start: left in LTR, right in RTL.</summary>
    TopStart = TopLeft,
    /// <summary>Opens above at logical end: right in LTR, left in RTL.</summary>
    TopEnd = TopRight,
}
