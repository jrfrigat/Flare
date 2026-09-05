namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for the shared drag-and-drop model (<c>FlareDragContext</c>,
/// <c>FlareDraggable</c>, <c>FlareDropZone</c>). One family for every surface that reorders, so a
/// dragged card, a dragged row and a dragged tree node read as the same gesture.
/// </summary>
public static class Drag
{
    /// <summary>CSS custom-property name for the opacity of the item left in place while it is in flight.</summary>
    public const string SourceOpacity = "--flare-drag-source-opacity";
    /// <summary>CSS custom-property name for the drag preview's shadow.</summary>
    public const string PreviewElevation = "--flare-drag-preview-elevation";
    /// <summary>CSS custom-property name for the drag preview's opacity.</summary>
    public const string PreviewOpacity = "--flare-drag-preview-opacity";
    /// <summary>CSS custom-property name for the background of a drop zone the pointer is over.</summary>
    public const string ZoneActiveBackground = "--flare-drag-zone-active-background";
    /// <summary>CSS custom-property name for the outline color of a drop zone the pointer is over.</summary>
    public const string ZoneActiveOutline = "--flare-drag-zone-active-outline";
    /// <summary>CSS custom-property name for the insertion line's color.</summary>
    public const string IndicatorColor = "--flare-drag-indicator-color";
}
