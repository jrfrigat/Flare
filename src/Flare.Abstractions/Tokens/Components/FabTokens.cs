using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    /// <summary>Space around the glyph at the sm size. The FAB has no explicit height, so the padding is
    /// what sets the overall size.</summary>
    [CssVar(Fab.Padding.Sm)] public string PaddingSm { get; init; } = "var(--flare-spacing-4)";
    /// <summary>Space around the glyph at the md size.</summary>
    [CssVar(Fab.Padding.Md)] public string PaddingMd { get; init; } = "var(--flare-spacing-8)";
    /// <summary>Space around the glyph at the lg size.</summary>
    [CssVar(Fab.Padding.Lg)] public required string PaddingLg { get; init; }

    /// <summary>Corner radius at the sm size.</summary>
    [CssVar(Fab.Radius.Sm)] public string RadiusSm { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Corner radius at the md size.</summary>
    [CssVar(Fab.Radius.Md)] public string RadiusMd { get; init; } = "var(--flare-shape-large)";
    /// <summary>Corner radius at the lg size.</summary>
    [CssVar(Fab.Radius.Lg)] public string RadiusLg { get; init; } = "var(--flare-shape-extra-large)";

    /// <summary>Space between the icon and the label on an extended FAB.</summary>
    [CssVar(Fab.Gap)] public string Gap { get; init; } = "var(--flare-spacing-6)";

    /// <summary>Shadow that lifts the FAB above the content it floats over.</summary>
    [CssVar(Fab.Shadow)] public string Shadow { get; init; } = "var(--flare-elevation-3)";
    /// <summary>Shadow the FAB lifts to on hover. A theme with a flat FAB matches it to
    /// <see cref="Shadow"/>.</summary>
    [CssVar(Fab.HoverShadow)] public string HoverShadow { get; init; } = "var(--flare-elevation-4)";

    /// <summary>Distance an anchored FAB sits from the screen edge.</summary>
    [CssVar(Fab.AnchorOffset)] public string AnchorOffset { get; init; } = "var(--flare-spacing-12)";

    /// <summary>Glyph size of the icon on a small FAB.</summary>
    [CssVar(Fab.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Glyph size of the icon on a baseline FAB.</summary>
    [CssVar(Fab.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Glyph size of the icon on a large FAB.</summary>
    [CssVar(Fab.IconSize.Lg)] public required string IconSizeLg { get; init; }
}
