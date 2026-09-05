namespace Flare.Css.Classes;

/// <summary>CSS classes for the shared drag-and-drop model.</summary>
public static class Drag
{
    /// <summary>The <c>flare-drag-context</c> CSS class.</summary>
    public const string Context = "flare-drag-context";
    /// <summary>The <c>flare-draggable</c> CSS class.</summary>
    public const string Item = "flare-draggable";
    /// <summary>The <c>flare-draggable--dragging</c> CSS class.</summary>
    public const string ItemDragging = "flare-draggable--dragging";
    /// <summary>The <c>flare-draggable--disabled</c> CSS class.</summary>
    public const string ItemDisabled = "flare-draggable--disabled";
    /// <summary>The <c>flare-drop-zone</c> CSS class.</summary>
    public const string Zone = "flare-drop-zone";
    /// <summary>The <c>flare-drop-zone--candidate</c> CSS class, applied to every zone that accepts the
    /// item currently in flight.</summary>
    public const string ZoneCandidate = "flare-drop-zone--candidate";
    /// <summary>The <c>flare-drop-zone--over</c> CSS class, applied to the one zone under the pointer.</summary>
    public const string ZoneOver = "flare-drop-zone--over";
    /// <summary>The <c>flare-drag-preview</c> CSS class.</summary>
    public const string Preview = "flare-drag-preview";
    /// <summary>The <c>flare-drag-indicator</c> CSS class (the insertion line).</summary>
    public const string Indicator = "flare-drag-indicator";
    /// <summary>The <c>flare-drag-indicator--horizontal</c> CSS class, for a row of items.</summary>
    public const string IndicatorHorizontal = "flare-drag-indicator--horizontal";
}
