namespace Flare.Components;

/// <summary>
/// Placement mode for a <see cref="FlareFloatingActionButton"/>.
/// </summary>
public enum FabPosition
{
    /// <summary>Anchored to the bottom-right corner of the viewport. The default.</summary>
    BottomRight,

    /// <summary>Anchored to the bottom-left corner of the viewport.</summary>
    BottomLeft,

    /// <summary>Rendered inline in normal document flow (not anchored).</summary>
    Static,
}
