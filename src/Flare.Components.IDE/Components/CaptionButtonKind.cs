namespace Flare.Components.IDE;

/// <summary>The command a title-bar caption control represents. The kind selects the semantic
/// marker and the close control's danger paint; the host still supplies the visible glyph, the
/// accessible label and the command.</summary>
public enum CaptionButtonKind
{
    /// <summary>Minimize the window.</summary>
    Minimize,
    /// <summary>Maximize the window.</summary>
    Maximize,
    /// <summary>Restore the window from its maximized state.</summary>
    Restore,
    /// <summary>Close the window. This is the only kind that takes the danger paint.</summary>
    Close,
}
