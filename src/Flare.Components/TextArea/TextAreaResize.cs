namespace Flare.Components;

/// <summary>
/// Controls the user-drag resize affordance of a <see cref="FlareTextArea"/> (the CSS <c>resize</c>
/// property). Ignored when <see cref="FlareTextArea.AutoGrow"/> is on (auto-sizing owns the height).
/// </summary>
public enum TextAreaResize
{
    /// <summary>The theme default (no explicit <c>resize</c> override).</summary>
    Default,
    /// <summary>Not user-resizable (<c>resize: none</c>).</summary>
    None,
    /// <summary>Resizable vertically only (<c>resize: vertical</c>).</summary>
    Vertical,
    /// <summary>Resizable horizontally only (<c>resize: horizontal</c>).</summary>
    Horizontal,
    /// <summary>Resizable in both directions (<c>resize: both</c>).</summary>
    Both,
}
