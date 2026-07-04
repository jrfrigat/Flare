using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareBadge</c>.</summary>
public sealed record BadgeTokens
{
    /// <summary>Border radius of the badge indicator pill (typically the theme's full/pill shape).</summary>
    [CssVar(Badge.Radius)] public required string Radius { get; init; }

    /// <summary>Minimum width of a count / text indicator (not dot).</summary>
    [CssVar(Badge.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Height of a count / text indicator (not dot).</summary>
    [CssVar(Badge.Height)] public required string Height { get; init; }

    /// <summary>Diameter of the dot variant.</summary>
    [CssVar(Badge.DotSize)] public required string DotSize { get; init; }

    /// <summary>Horizontal padding inside a count / text indicator.</summary>
    [CssVar(Badge.PaddingX)] public required string PaddingX { get; init; }

    /// <summary>Offset of the indicator from the anchor edge (count/text variant).</summary>
    [CssVar(Badge.Offset)] public required string Offset { get; init; }

    /// <summary>Offset of the indicator from the anchor edge (dot variant - tighter).</summary>
    [CssVar(Badge.DotOffset)] public required string DotOffset { get; init; }
}
