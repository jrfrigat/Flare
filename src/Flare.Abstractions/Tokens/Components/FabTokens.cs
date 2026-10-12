using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    /// <summary>Space around the glyph at the sm size. The FAB has no explicit height, so the padding is
    /// what sets the overall size.</summary>
    [CssVar(Fab.Padding.Sm)] public required string PaddingSm { get; init; }
    /// <summary>Space around the glyph at the md size.</summary>
    [CssVar(Fab.Padding.Md)] public required string PaddingMd { get; init; }
    /// <summary>Space around the glyph at the lg size.</summary>
    [CssVar(Fab.Padding.Lg)] public required string PaddingLg { get; init; }

    /// <summary>Corner radius at the sm size.</summary>
    [CssVar(Fab.Radius.Sm)] public required string RadiusSm { get; init; }
    /// <summary>Corner radius at the md size.</summary>
    [CssVar(Fab.Radius.Md)] public required string RadiusMd { get; init; }
    /// <summary>Corner radius at the lg size.</summary>
    [CssVar(Fab.Radius.Lg)] public required string RadiusLg { get; init; }

    /// <summary>Space between the icon and the label on an extended FAB.</summary>
    [CssVar(Fab.Gap)] public required string Gap { get; init; }

    /// <summary>Shadow that lifts the FAB above the content it floats over.</summary>
    [CssVar(Fab.Shadow)] public required string Shadow { get; init; }
    /// <summary>Shadow the FAB lifts to on hover. A theme with a flat FAB matches it to
    /// <see cref="Shadow"/>.</summary>
    [CssVar(Fab.HoverShadow)] public required string HoverShadow { get; init; }

    /// <summary>Distance an anchored FAB sits from the screen edge.</summary>
    [CssVar(Fab.AnchorOffset)] public required string AnchorOffset { get; init; }

    /// <summary>Glyph size of the icon on a small FAB.</summary>
    [CssVar(Fab.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Glyph size of the icon on a baseline FAB.</summary>
    [CssVar(Fab.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Glyph size of the icon on a large FAB.</summary>
    [CssVar(Fab.IconSize.Lg)] public required string IconSizeLg { get; init; }
}
