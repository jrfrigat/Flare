namespace Flare.Components;

/// <summary>Which edge of a <c>FlareResizable</c> carries the drag handle.</summary>
public enum ResizableEdge
{
    /// <summary>Drag the trailing edge to change the width (the default).</summary>
    Right,
    /// <summary>Drag the bottom edge to change the height.</summary>
    Bottom,
    /// <summary>Drag the leading edge to change the width.</summary>
    Left,
    /// <summary>Drag the top edge to change the height.</summary>
    Top,
}
