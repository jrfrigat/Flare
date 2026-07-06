namespace Flare.Css.Tokens;

/// <summary>CSS custom properties for <c>FlareSplitter</c> (handle thickness, idle/hover colors,
/// centre-icon size/color). Defaults live in the splitter stylesheet; set these to customize.</summary>
public static class Splitter
{
    /// <summary>Handle thickness (CSS length).</summary>
    public const string GutterSize = "--flare-splitter-gutter-size";
    /// <summary>Handle color when idle.</summary>
    public const string Color = "--flare-splitter-color";
    /// <summary>Handle color on hover/focus.</summary>
    public const string HoverColor = "--flare-splitter-hover-color";
    /// <summary>Centre-icon size.</summary>
    public const string IconSize = "--flare-splitter-icon-size";
    /// <summary>Centre-icon color.</summary>
    public const string IconColor = "--flare-splitter-icon-color";
}
