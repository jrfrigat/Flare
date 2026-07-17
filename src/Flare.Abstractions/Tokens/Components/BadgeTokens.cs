using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme token values for <c>FlareBadge</c>.
/// </summary>
/// <remarks>
/// The box, the dot and the label ramp across the five sizes, one token per step - the same shape
/// <c>ButtonTokens</c> uses. It used to be a single set: the theme named the default size and
/// <c>badge.css</c> hardcoded the other four in literals, so four of five sizes were core's opinion and no
/// theme could touch them. Radius and the two offsets stay single because they do not vary by size.
/// </remarks>
public sealed record BadgeTokens
{
    /// <summary>Border radius of the badge indicator pill (typically the theme's full/pill shape).</summary>
    [CssVar(Badge.Radius)] public required string Radius { get; init; }

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
    [CssVar(Badge.PaddingX.Xs)] public required string PaddingXXs { get; init; }
    /// <summary>Horizontal padding inside a count / text indicator at the sm size.</summary>
    [CssVar(Badge.PaddingX.Sm)] public required string PaddingXSm { get; init; }
    /// <summary>Horizontal padding inside a count / text indicator at the md (default) size.</summary>
    [CssVar(Badge.PaddingX.Md)] public required string PaddingXMd { get; init; }
    /// <summary>Horizontal padding inside a count / text indicator at the lg size.</summary>
    [CssVar(Badge.PaddingX.Lg)] public required string PaddingXLg { get; init; }
    /// <summary>Horizontal padding inside a count / text indicator at the xl size.</summary>
    [CssVar(Badge.PaddingX.Xl)] public required string PaddingXXl { get; init; }

    /// <summary>Font size of the count / text at the xs size.</summary>
    [CssVar(Badge.LabelSize.Xs)] public required string LabelSizeXs { get; init; }
    /// <summary>Font size of the count / text at the sm size.</summary>
    [CssVar(Badge.LabelSize.Sm)] public required string LabelSizeSm { get; init; }
    /// <summary>Font size of the count / text at the md (default) size.</summary>
    [CssVar(Badge.LabelSize.Md)] public required string LabelSizeMd { get; init; }
    /// <summary>Font size of the count / text at the lg size.</summary>
    [CssVar(Badge.LabelSize.Lg)] public required string LabelSizeLg { get; init; }
    /// <summary>Font size of the count / text at the xl size.</summary>
    [CssVar(Badge.LabelSize.Xl)] public required string LabelSizeXl { get; init; }

    /// <summary>Offset of the indicator from the anchor edge (count/text variant).</summary>
    [CssVar(Badge.Offset)] public required string Offset { get; init; }

    /// <summary>Offset of the indicator from the anchor edge (dot variant - tighter).</summary>
    [CssVar(Badge.DotOffset)] public required string DotOffset { get; init; }
}
