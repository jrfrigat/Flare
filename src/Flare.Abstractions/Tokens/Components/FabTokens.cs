using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    // --- 1. PADDING around the glyph (sets the overall size) for 3 sizes ---
    /// <summary>Space around the glyph at the sm size. The FAB has no explicit height, so the padding is
    /// what sets the overall size.</summary>
    [CssVar(Fab.Padding.Sm)] public required string PaddingSm { get; init; }
    /// <summary>Space around the glyph at the md size.</summary>
    [CssVar(Fab.Padding.Md)] public required string PaddingMd { get; init; }
    /// <summary>Space around the glyph at the lg size.</summary>
    [CssVar(Fab.Padding.Lg)] public required string PaddingLg { get; init; }

    // --- 2. RADIUS for 3 sizes (names match the legacy CSS variables) ---
    /// <summary>Corner radius at the sm size.</summary>
    [CssVar(Fab.Radius.Sm)] public required string RadiusSm { get; init; }
    /// <summary>Corner radius at the md size.</summary>
    [CssVar(Fab.Radius.Md)] public required string RadiusMd { get; init; }
    /// <summary>Corner radius at the lg size.</summary>
    [CssVar(Fab.Radius.Lg)] public required string RadiusLg { get; init; }

    // --- 3. GAP between icon and label (extended FAB) ---
    /// <summary>Space between the icon and the label on an extended FAB.</summary>
    [CssVar(Fab.Gap)] public required string Gap { get; init; }

    // --- 4. SHADOWS (rest / hover) ---
    /// <summary>Shadow that lifts the FAB above the content it floats over.</summary>
    [CssVar(Fab.Shadow)] public required string Shadow { get; init; }
    /// <summary>Shadow the FAB lifts to on hover. A theme with a flat FAB matches it to
    /// <see cref="Shadow"/>.</summary>
    [CssVar(Fab.HoverShadow)] public required string HoverShadow { get; init; }

    // --- 5. Anchor offset from the screen edge ---
    /// <summary>Distance an anchored FAB sits from the screen edge.</summary>
    [CssVar(Fab.AnchorOffset)] public required string AnchorOffset { get; init; }
}
