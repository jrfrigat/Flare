namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareBadge</c>.</summary>
public sealed record BadgeTokens
{
    /// <summary>Border radius of the badge indicator pill.</summary>
    public string Radius { get; init; } = "9999px";

    /// <summary>Minimum width of a count / text indicator (not dot).</summary>
    public string MinWidth { get; init; } = "1rem";

    /// <summary>Height of a count / text indicator (not dot).</summary>
    public string Height { get; init; } = "1rem";

    /// <summary>Diameter of the dot variant.</summary>
    public string DotSize { get; init; } = "0.375rem";

    /// <summary>Horizontal padding inside a count / text indicator.</summary>
    public string PaddingX { get; init; } = "0.25rem";

    /// <summary>Offset of the indicator from the anchor edge (count/text variant).</summary>
    public string Offset { get; init; } = "0.375rem";

    /// <summary>Offset of the indicator from the anchor edge (dot variant - tighter).</summary>
    public string DotOffset { get; init; } = "0";
}
