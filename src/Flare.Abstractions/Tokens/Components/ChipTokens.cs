using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareChip</c>.</summary>
public sealed record ChipTokens
{
    /// <summary>Corner radius of the chip container.</summary>
    [CssVar(Chip.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Small);

    /// <summary>Minimum height of the chip.</summary>
    [CssVar(Chip.Height)] public required string Height { get; init; }

    /// <summary>Container behind a filled chip. A language that draws the chip as a grey pill states
    /// the wash here; one that treats it as a surface points this at a surface role.</summary>
    [CssVar(Chip.FilledBg)] public string FilledBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHigh);

    /// <summary>Container behind an elevated chip - the one that carries a shadow instead of a border.</summary>
    [CssVar(Chip.ElevatedBg)] public string ElevatedBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);

    /// <summary>Border of an outlined chip that has no Color of its own. A Color on the chip still wins.</summary>
    [CssVar(Chip.BorderColor)] public string BorderColor { get; init; } = Var(Css.Tokens.Color.OutlineVariant);

    /// <summary>Label and trailing-icon colour of an unselected chip that has no Color of its own.</summary>
    [CssVar(Chip.LabelColor)] public string LabelColor { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);

    // Leading and trailing icons share one ramp: the spec sizes both ends alike, and a chip that
    // sized them differently would read as two components. The avatar has its own, because it is a
    // cropped picture filling a shape rather than a glyph sitting in one.
    /// <summary>Family of a chip label. Without it a chip could only ever be set in the theme's UI face,
    /// so a monospace micro-tag - the shape a chip most often takes in a tool built around identifiers -
    /// needed a stylesheet of the application's own.</summary>
    [CssVar(Chip.LabelFont)] public string LabelFont { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Font);

    /// <summary>Weight of a chip label.</summary>
    [CssVar(Chip.LabelWeight)] public string LabelWeight { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Weight);

    /// <summary>Letter-spacing of a chip label.</summary>
    [CssVar(Chip.LabelSpacing)] public string LabelSpacing { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Spacing);

    /// <summary>Label size at the extra-small step.</summary>
    [CssVar(Chip.LabelSize.Xs)] public string LabelSizeXs { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Size);
    /// <summary>Label size at the small step.</summary>
    [CssVar(Chip.LabelSize.Sm)] public string LabelSizeSm { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Size);
    /// <summary>Label size at the medium step.</summary>
    [CssVar(Chip.LabelSize.Md)] public string LabelSizeMd { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Size);
    /// <summary>Label size at the large step.</summary>
    [CssVar(Chip.LabelSize.Lg)] public string LabelSizeLg { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Size);
    /// <summary>Label size at the extra-large step.</summary>
    [CssVar(Chip.LabelSize.Xl)] public string LabelSizeXl { get; init; } = Var(Css.Tokens.Typography.TitleMedium.Size);

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
    /// <summary>How far the leading avatar tucks into the chip's start edge.</summary>
    [CssVar(Chip.AvatarInset)] public string AvatarInset { get; init; } = $"calc({Var(Css.Tokens.Spacing.S2)} * -1)";

    /// <summary>Inline padding of a chip at the xs size.</summary>
    [CssVar(Chip.PaddingInline.Xs)] public string PaddingInlineXs { get; init; } = Var(Css.Tokens.Spacing.S4);

    /// <summary>Inline padding of a chip at the sm size.</summary>
    [CssVar(Chip.PaddingInline.Sm)] public string PaddingInlineSm { get; init; } = Var(Css.Tokens.Spacing.S5);

    /// <summary>Inline padding of a chip at the md size.</summary>
    [CssVar(Chip.PaddingInline.Md)] public string PaddingInlineMd { get; init; } = Var(Css.Tokens.Spacing.S8);

    /// <summary>Inline padding of a chip at the lg size.</summary>
    [CssVar(Chip.PaddingInline.Lg)] public string PaddingInlineLg { get; init; } = Var(Css.Tokens.Spacing.S10);

    /// <summary>Inline padding of a chip at the xl size.</summary>
    [CssVar(Chip.PaddingInline.Xl)] public string PaddingInlineXl { get; init; } = Var(Css.Tokens.Spacing.S12);
}
