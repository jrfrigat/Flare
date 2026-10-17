using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme token values for <c>FlareBadge</c>.
/// </summary>
/// <remarks>
/// The box, the dot and the label ramp across the five sizes, one token per step - the same shape
/// <c>ButtonTokens</c> uses - so every size is the theme's to set, not core's. Radius and the two
/// offsets stay single because they do not vary by size.
/// </remarks>
public sealed record BadgeTokens
{
    /// <summary>Border radius of the badge indicator pill (typically the theme's full/pill shape).</summary>
    [CssVar(Badge.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Full);

    /// <summary>Minimum width of a count / text indicator at the xs size.</summary>
    [CssVar(Badge.MinWidth.Xs)] public required string MinWidthXs { get; init; }
    /// <summary>Minimum width of a count / text indicator at the sm size.</summary>
    [CssVar(Badge.MinWidth.Sm)] public required string MinWidthSm { get; init; }
    /// <summary>Minimum width of a count / text indicator at the md (default) size.</summary>
    [CssVar(Badge.MinWidth.Md)] public required string MinWidthMd { get; init; }
    /// <summary>Minimum width of a count / text indicator at the lg size.</summary>
    [CssVar(Badge.MinWidth.Lg)] public required string MinWidthLg { get; init; }
    /// <summary>Minimum width of a count / text indicator at the xl size.</summary>
    [CssVar(Badge.MinWidth.Xl)] public required string MinWidthXl { get; init; }

    /// <summary>Height of a count / text indicator at the xs size.</summary>
    [CssVar(Badge.Height.Xs)] public required string HeightXs { get; init; }
    /// <summary>Height of a count / text indicator at the sm size.</summary>
    [CssVar(Badge.Height.Sm)] public required string HeightSm { get; init; }
    /// <summary>Height of a count / text indicator at the md (default) size.</summary>
    [CssVar(Badge.Height.Md)] public required string HeightMd { get; init; }
    /// <summary>Height of a count / text indicator at the lg size.</summary>
    [CssVar(Badge.Height.Lg)] public required string HeightLg { get; init; }
    /// <summary>Height of a count / text indicator at the xl size.</summary>
    [CssVar(Badge.Height.Xl)] public required string HeightXl { get; init; }

    /// <summary>Diameter of the dot variant at the xs size.</summary>
    [CssVar(Badge.DotSize.Xs)] public required string DotSizeXs { get; init; }
    /// <summary>Diameter of the dot variant at the sm size.</summary>
    [CssVar(Badge.DotSize.Sm)] public required string DotSizeSm { get; init; }
    /// <summary>Diameter of the dot variant at the md (default) size.</summary>
    [CssVar(Badge.DotSize.Md)] public required string DotSizeMd { get; init; }
    /// <summary>Diameter of the dot variant at the lg size.</summary>
    [CssVar(Badge.DotSize.Lg)] public required string DotSizeLg { get; init; }
    /// <summary>Diameter of the dot variant at the xl size.</summary>
    [CssVar(Badge.DotSize.Xl)] public required string DotSizeXl { get; init; }

    /// <summary>Horizontal padding inside a count / text indicator at the xs size.</summary>
    [CssVar(Badge.PaddingX.Xs)] public string PaddingXXs { get; init; } = $"calc({Var(Css.Tokens.Spacing.S3)} / 2)";
    /// <summary>Horizontal padding inside a count / text indicator at the sm size.</summary>
    [CssVar(Badge.PaddingX.Sm)] public string PaddingXSm { get; init; } = $"calc({Var(Css.Tokens.Spacing.S3)} / 2)";
    /// <summary>Horizontal padding inside a count / text indicator at the md (default) size.</summary>
    [CssVar(Badge.PaddingX.Md)] public string PaddingXMd { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Horizontal padding inside a count / text indicator at the lg size.</summary>
    [CssVar(Badge.PaddingX.Lg)] public string PaddingXLg { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Horizontal padding inside a count / text indicator at the xl size.</summary>
    [CssVar(Badge.PaddingX.Xl)] public string PaddingXXl { get; init; } = Var(Css.Tokens.Spacing.S4);

    /// <summary>Font size of the count / text at the xs size.</summary>
    [CssVar(Badge.LabelSize.Xs)] public string LabelSizeXs { get; init; } = Var(Css.Tokens.Typography.Size2Xs);
    /// <summary>Font size of the count / text at the sm size.</summary>
    [CssVar(Badge.LabelSize.Sm)] public string LabelSizeSm { get; init; } = Var(Css.Tokens.Typography.SizeXs);
    /// <summary>Font size of the count / text at the md (default) size.</summary>
    [CssVar(Badge.LabelSize.Md)] public string LabelSizeMd { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Size);
    /// <summary>Font size of the count / text at the lg size.</summary>
    [CssVar(Badge.LabelSize.Lg)] public string LabelSizeLg { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Size);
    /// <summary>Font size of the count / text at the xl size.</summary>
    [CssVar(Badge.LabelSize.Xl)] public string LabelSizeXl { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Size);

    /// <summary>Offset of the indicator from the anchor edge (count/text variant).</summary>
    [CssVar(Badge.Offset)] public required string Offset { get; init; }

    /// <summary>Offset of the indicator from the anchor edge (dot variant - tighter).</summary>
    [CssVar(Badge.DotOffset)] public string DotOffset { get; init; } = Var(Css.Tokens.Spacing.S0);
}
