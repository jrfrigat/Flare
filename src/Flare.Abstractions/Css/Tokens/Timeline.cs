namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareTimeline</c> / <c>FlareTimelineItem</c>.</summary>
public static class Timeline
{
    private const string FlareTimeline = $"{Vars.Flare}-timeline";

    /// <summary>CSS custom-property name for the node dot diameter token.</summary>
    public const string DotSize = $"{FlareTimeline}-dot-size";
    /// <summary>CSS custom-property name for the node dot background token.</summary>
    public const string DotBg = $"{FlareTimeline}-dot-bg";
    /// <summary>CSS custom-property name for the node dot border width token.</summary>
    public const string DotBorderWidth = $"{FlareTimeline}-dot-border-width";
    /// <summary>CSS custom-property name for the dot icon glyph size token.</summary>
    public const string DotIconSize = $"{FlareTimeline}-dot-icon-size";
    /// <summary>CSS custom-property name for the connector line thickness token.</summary>
    public const string LineWidth = $"{FlareTimeline}-line-width";
    /// <summary>CSS custom-property name for the connector line color token.</summary>
    public const string LineColor = $"{FlareTimeline}-line-color";
    /// <summary>CSS custom-property name for the connector column width token.</summary>
    public const string ConnectorWidth = $"{FlareTimeline}-connector-width";
}
