using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareBadge</c>.</summary>
public sealed record BadgeTokens
{
    /// <summary>Border radius of the badge indicator pill. Defaults to the shared full shape token
    /// (the pill vocabulary used by Chip/Nav/Tabs) instead of a hardcoded pill radius.</summary>
    [CssVar(Badge.Radius)] public string Radius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Minimum width of a count / text indicator (not dot).</summary>
    [CssVar(Badge.MinWidth)] public string MinWidth { get; init; } = "1rem";

    /// <summary>Height of a count / text indicator (not dot).</summary>
    [CssVar(Badge.Height)] public string Height { get; init; } = "1rem";

    /// <summary>Diameter of the dot variant.</summary>
    [CssVar(Badge.DotSize)] public string DotSize { get; init; } = "0.375rem";

    /// <summary>Horizontal padding inside a count / text indicator.</summary>
    [CssVar(Badge.PaddingX)] public string PaddingX { get; init; } = "0.25rem";

    /// <summary>Offset of the indicator from the anchor edge (count/text variant).</summary>
    [CssVar(Badge.Offset)] public string Offset { get; init; } = "0.375rem";

    /// <summary>Offset of the indicator from the anchor edge (dot variant - tighter).</summary>
    [CssVar(Badge.DotOffset)] public string DotOffset { get; init; } = "0";
}
