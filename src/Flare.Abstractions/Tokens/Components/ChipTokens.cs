using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareChip</c>.</summary>
public sealed record ChipTokens
{
    /// <summary>Corner radius of the chip container.</summary>
    [CssVar(Chip.Radius)] public required string Radius { get; init; }

    /// <summary>Minimum height of the chip.</summary>
    [CssVar(Chip.Height)] public required string Height { get; init; }

    /// <summary>Container behind a filled chip. A language that draws the chip as a grey pill states
    /// the wash here; one that treats it as a surface points this at a surface role.</summary>
    [CssVar(Chip.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Container behind an elevated chip - the one that carries a shadow instead of a border.</summary>
    [CssVar(Chip.ElevatedBg)] public required string ElevatedBg { get; init; }

    // Leading and trailing icons share one ramp: the spec sizes both ends alike, and a chip that
    // sized them differently would read as two components. The avatar has its own, because it is a
    // cropped picture filling a shape rather than a glyph sitting in one.
    /// <summary>Leading and trailing icon glyph size at the xs size.</summary>
    [CssVar(Chip.IconSize.Xs)] public required string IconSizeXs { get; init; }
    /// <summary>Leading and trailing icon glyph size at the sm size.</summary>
    [CssVar(Chip.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Leading and trailing icon glyph size at the md size.</summary>
    [CssVar(Chip.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Leading and trailing icon glyph size at the lg size.</summary>
    [CssVar(Chip.IconSize.Lg)] public required string IconSizeLg { get; init; }
    /// <summary>Leading and trailing icon glyph size at the xl size.</summary>
    [CssVar(Chip.IconSize.Xl)] public required string IconSizeXl { get; init; }
    /// <summary>Leading avatar box at the xs size.</summary>
    [CssVar(Chip.AvatarSize.Xs)] public required string AvatarSizeXs { get; init; }
    /// <summary>Leading avatar box at the sm size.</summary>
    [CssVar(Chip.AvatarSize.Sm)] public required string AvatarSizeSm { get; init; }
    /// <summary>Leading avatar box at the md size.</summary>
    [CssVar(Chip.AvatarSize.Md)] public required string AvatarSizeMd { get; init; }
    /// <summary>Leading avatar box at the lg size.</summary>
    [CssVar(Chip.AvatarSize.Lg)] public required string AvatarSizeLg { get; init; }
    /// <summary>Leading avatar box at the xl size.</summary>
    [CssVar(Chip.AvatarSize.Xl)] public required string AvatarSizeXl { get; init; }
}
