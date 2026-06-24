namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for badge.</summary>
public static class Badge
{
    private const string FlareBadge = $"{Vars.Flare}-badge";

    /// <summary>Border radius of the badge pill indicator.</summary>
    public const string Radius = $"{FlareBadge}-radius";

    /// <summary>Minimum width of the count / text indicator.</summary>
    public const string MinWidth = $"{FlareBadge}-min-width";

    /// <summary>Height of the count / text indicator.</summary>
    public const string Height = $"{FlareBadge}-height";

    /// <summary>Diameter of the dot variant.</summary>
    public const string DotSize = $"{FlareBadge}-dot-size";

    /// <summary>Horizontal padding inside the indicator.</summary>
    public const string PaddingX = $"{FlareBadge}-padding-x";

    /// <summary>Offset of the indicator from the anchor corner (count/text).</summary>
    public const string Offset = $"{FlareBadge}-offset";

    /// <summary>Offset of the indicator from the anchor corner (dot).</summary>
    public const string DotOffset = $"{FlareBadge}-dot-offset";
}
