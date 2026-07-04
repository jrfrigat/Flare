using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    // --- 1. PADDING around the glyph (sets the overall size) for 3 sizes ---
    /// <summary>Padding sm token (<c>0.5rem</c>).</summary>
    [CssVar(Fab.Padding.Sm)] public required string PaddingSm { get; init; }
    /// <summary>Padding md token (<c>1rem</c>).</summary>
    [CssVar(Fab.Padding.Md)] public required string PaddingMd { get; init; }
    /// <summary>Padding lg token (<c>1.75rem</c>).</summary>
    [CssVar(Fab.Padding.Lg)] public required string PaddingLg { get; init; }

    // --- 2. RADIUS for 3 sizes (names match the legacy CSS variables) ---
    /// <summary>Radius sm token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(Fab.Radius.Sm)] public required string RadiusSm { get; init; }
    /// <summary>Radius md token (<c>var(--flare-shape-large)</c>).</summary>
    [CssVar(Fab.Radius.Md)] public required string RadiusMd { get; init; }
    /// <summary>Radius lg token (<c>var(--flare-shape-extra-large)</c>).</summary>
    [CssVar(Fab.Radius.Lg)] public required string RadiusLg { get; init; }

    // --- 3. GAP between icon and label (extended FAB) ---
    /// <summary>Gap token (<c>0.75rem</c>).</summary>
    [CssVar(Fab.Gap)] public required string Gap { get; init; }

    // --- 4. SHADOWS (rest / hover) ---
    /// <summary>Shadow token (<c>var(--flare-elevation-3)</c>).</summary>
    [CssVar(Fab.Shadow)] public required string Shadow { get; init; }
    /// <summary>Hover shadow token (<c>var(--flare-elevation-4)</c>).</summary>
    [CssVar(Fab.HoverShadow)] public required string HoverShadow { get; init; }

    // --- 5. Anchor offset from the screen edge ---
    /// <summary>Anchor offset token (<c>1.5rem</c>).</summary>
    [CssVar(Fab.AnchorOffset)] public required string AnchorOffset { get; init; }
}
