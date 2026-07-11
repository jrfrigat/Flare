namespace Flare.Components;

/// <summary>Optical edge alignment for an icon/toggle button: a negative inline margin that pulls the
/// button toward the container edge so its icon lines up with adjacent content (app bars, toolbars,
/// list-item leading/trailing slots).</summary>
public enum ButtonEdge
{
    /// <summary>No edge offset (default).</summary>
    None,
    /// <summary>Pull toward the leading (inline-start) edge.</summary>
    Start,
    /// <summary>Pull toward the trailing (inline-end) edge.</summary>
    End,
}
