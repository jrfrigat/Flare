namespace Flare.Components;

/// <summary>
/// Horizontal text alignment for <c>FlareText</c>. <see cref="Default"/> inherits the
/// surrounding alignment; the other values map to <c>text-align</c> via shared
/// <c>flare-text--align-*</c> classes (no inline style needed).
/// </summary>
public enum TextAlign
{
    /// <summary>Inherit the surrounding text alignment (no override).</summary>
    Default,
    /// <summary>Align to the start (writing-direction aware).</summary>
    Start,
    /// <summary>Align to the left.</summary>
    Left,
    /// <summary>Center.</summary>
    Center,
    /// <summary>Align to the right.</summary>
    Right,
    /// <summary>Align to the end (writing-direction aware).</summary>
    End,
    /// <summary>Justify (stretch lines to both edges).</summary>
    Justify,
}
