using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    /// <summary>Space around the glyph at the sm size. The FAB has no explicit height, so the padding is
    /// what sets the overall size.</summary>
    [CssVar(Fab.Padding.Sm)] public string PaddingSm { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Space around the glyph at the md size.</summary>
    [CssVar(Fab.Padding.Md)] public string PaddingMd { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Space around the glyph at the lg size.</summary>
    [CssVar(Fab.Padding.Lg)] public string PaddingLg { get; init; } = Var(Css.Tokens.Spacing.S16);

    /// <summary>Corner radius at the sm size.</summary>
    [CssVar(Fab.Radius.Sm)] public string RadiusSm { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Corner radius at the md size.</summary>
    [CssVar(Fab.Radius.Md)] public string RadiusMd { get; init; } = Var(Css.Tokens.Shape.Large);
    /// <summary>Corner radius at the lg size.</summary>
    [CssVar(Fab.Radius.Lg)] public string RadiusLg { get; init; } = Var(Css.Tokens.Shape.ExtraLarge);

    /// <summary>Space between the icon and the label on an extended FAB.</summary>
    [CssVar(Fab.Gap)] public string Gap { get; init; } = Var(Css.Tokens.Spacing.S6);

    /// <summary>Space above and below an extended FAB's content. With the icon it sets the extended
    /// height, which is why a theme whose redline states a different height moves this step.</summary>
    [CssVar(Fab.ExtendedPaddingBlock)] public string ExtendedPaddingBlock { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Space between an extended FAB's side edges and its icon/label.</summary>
    [CssVar(Fab.ExtendedPaddingInline)] public string ExtendedPaddingInline { get; init; } = Var(Css.Tokens.Spacing.S8);

    /// <summary>Shadow that lifts the FAB above the content it floats over.</summary>
    [CssVar(Fab.Shadow)] public string Shadow { get; init; } = Var(Css.Tokens.Elevation.Level3);
    /// <summary>Shadow the FAB lifts to on hover. A theme with a flat FAB matches it to
    /// <see cref="Shadow"/>.</summary>
    [CssVar(Fab.HoverShadow)] public string HoverShadow { get; init; } = Var(Css.Tokens.Elevation.Level4);

    /// <summary>Distance an anchored FAB sits from the screen edge.</summary>
    [CssVar(Fab.AnchorOffset)] public string AnchorOffset { get; init; } = Var(Css.Tokens.Spacing.S12);

    /// <summary>Distance between the FAB trigger and the nearest action of its menu. A theme may widen or
    /// close the gap; leaving it unset keeps the spacing scale's step.</summary>
    [CssVar(Fab.MenuOffset)] public string MenuOffset { get; init; } = Var(Css.Tokens.Spacing.S6);

    /// <summary>Glyph size of the icon on a small FAB.</summary>
    [CssVar(Fab.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Glyph size of the icon on a baseline FAB.</summary>
    [CssVar(Fab.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Glyph size of the icon on a large FAB.</summary>
    [CssVar(Fab.IconSize.Lg)] public required string IconSizeLg { get; init; }
}
