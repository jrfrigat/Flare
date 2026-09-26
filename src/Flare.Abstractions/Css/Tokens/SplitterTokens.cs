namespace Flare.Css.Tokens;

/// <summary>CSS custom properties for <c>FlareSplitter</c> (gutter thickness, grip geometry, colour and
/// shape, idle/hover colors, centre-icon size/color). Every one of these is supplied by the active theme through
/// <c>SplitterTokens</c>; the stylesheet holds no value of its own.</summary>
public static class Splitter
{
    /// <summary>CSS custom-property name for the grip length token.</summary>
    public const string GripLength = "--flare-splitter-grip-length";
    /// <summary>CSS custom-property name for the grip thickness token.</summary>
    public const string GripThickness = "--flare-splitter-grip-thickness";
    /// <summary>CSS custom-property name for the gutter thickness token.</summary>
    public const string GutterSize = "--flare-splitter-gutter-size";
    /// <summary>CSS custom-property name for the idle gutter colour token.</summary>
    public const string Color = "--flare-splitter-color";
    /// <summary>CSS custom-property name for the hover/focus gutter colour token.</summary>
    public const string HoverColor = "--flare-splitter-hover-color";
    /// <summary>CSS custom-property name for the centre-icon size token.</summary>
    public const string IconSize = "--flare-splitter-icon-size";
    /// <summary>CSS custom-property name for the centre-icon colour token.</summary>
    public const string IconColor = "--flare-splitter-icon-color";
    /// <summary>CSS custom-property name for the grip colour token.</summary>
    public const string GripColor = "--flare-splitter-grip-color";
    /// <summary>CSS custom-property name for the grip corner-radius token.</summary>
    public const string GripRadius = "--flare-splitter-grip-radius";
    /// <summary>CSS custom-property name for the grip colour while the handle is pressed or dragged.</summary>
    public const string GripPressedColor = "--flare-splitter-grip-pressed-color";
    /// <summary>CSS custom-property name for the grip thickness while the handle is pressed or dragged.</summary>
    public const string GripPressedThickness = "--flare-splitter-grip-pressed-thickness";
    /// <summary>CSS custom-property name for the grip length while the handle is pressed or dragged.</summary>
    public const string GripPressedLength = "--flare-splitter-grip-pressed-length";
    /// <summary>CSS custom-property name for the grip corner radius while the handle is pressed or dragged.</summary>
    public const string GripPressedRadius = "--flare-splitter-grip-pressed-radius";
}
