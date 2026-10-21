using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Avatar and its GROUP (the overlapping stack and its "+N" overflow badge).
/// Color and shape stay with the shared color-role and shape systems; the diameter and text size of
/// each size variant are the component's own scale and are set by the theme.
/// </summary>
public sealed record AvatarTokens
{
    /// <summary>Overlap between stacked avatars (a negative inline margin).</summary>
    [CssVar(AvatarField.GroupSpacing)] public required string GroupSpacing { get; init; }

    /// <summary>Width of the ring drawn around each stacked avatar to separate it from its neighbour.</summary>
    [CssVar(AvatarField.GroupBorderWidth)] public string GroupBorderWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);

    /// <summary>Color of the ring drawn around each stacked avatar.</summary>
    [CssVar(AvatarField.GroupBorderColor)] public string GroupBorderColor { get; init; } = Var(Css.Tokens.Color.Surface);

    /// <summary>Background color of the "+N" overflow badge.</summary>
    [CssVar(AvatarField.OverflowBg)] public string OverflowBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHighest);

    /// <summary>Text color of the "+N" overflow badge.</summary>
    [CssVar(AvatarField.OverflowColor)] public string OverflowColor { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);

    /// <summary>Diameter of the extra-small avatar.</summary>
    [CssVar(AvatarField.SizeXs)] public required string SizeXs { get; init; }
    /// <summary>Diameter of the small avatar.</summary>
    [CssVar(AvatarField.SizeSm)] public required string SizeSm { get; init; }
    /// <summary>Diameter of the medium avatar.</summary>
    [CssVar(AvatarField.SizeMd)] public required string SizeMd { get; init; }
    /// <summary>Diameter of the large avatar.</summary>
    [CssVar(AvatarField.SizeLg)] public required string SizeLg { get; init; }
    /// <summary>Diameter of the extra-large avatar.</summary>
    [CssVar(AvatarField.SizeXl)] public required string SizeXl { get; init; }
    /// <summary>Text size of the extra-small avatar.</summary>
    [CssVar(AvatarField.FontXs)] public required string FontXs { get; init; }
    /// <summary>Text size of the small avatar.</summary>
    [CssVar(AvatarField.FontSm)] public required string FontSm { get; init; }
    /// <summary>Text size of the medium avatar.</summary>
    [CssVar(AvatarField.FontMd)] public required string FontMd { get; init; }
    /// <summary>Text size of the large avatar.</summary>
    [CssVar(AvatarField.FontLg)] public required string FontLg { get; init; }
    /// <summary>Text size of the extra-large avatar.</summary>
    [CssVar(AvatarField.FontXl)] public required string FontXl { get; init; }
}
