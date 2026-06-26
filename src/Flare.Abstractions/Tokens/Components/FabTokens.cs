using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareFloatingActionButton</c>. Size is currently expressed via
/// padding around the glyph (S/M/L), with per-size corner radius and elevation tokens.</summary>
public sealed record FabTokens
{
    // --- 1. PADDING вокруг глифа (задаёт габарит) под 3 размера ---
    /// <summary>Padding sm token (<c>0.5rem</c>).</summary>
    [CssVar(Fab.Padding.Sm)] public string PaddingSm { get; init; } = "0.5rem";
    /// <summary>Padding md token (<c>1rem</c>).</summary>
    [CssVar(Fab.Padding.Md)] public string PaddingMd { get; init; } = "1rem";
    /// <summary>Padding lg token (<c>1.75rem</c>).</summary>
    [CssVar(Fab.Padding.Lg)] public string PaddingLg { get; init; } = "1.75rem";

    // --- 2. РАДИУС под 3 размера (имена совпадают с легаси-переменными CSS) ---
    /// <summary>Radius sm token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(Fab.Radius.Sm)] public string RadiusSm { get; init; } = Vars.Var(Shape.Medium);
    /// <summary>Radius md token (<c>var(--flare-shape-large)</c>).</summary>
    [CssVar(Fab.Radius.Md)] public string RadiusMd { get; init; } = Vars.Var(Shape.Large);
    /// <summary>Radius lg token (<c>var(--flare-shape-extra-large)</c>).</summary>
    [CssVar(Fab.Radius.Lg)] public string RadiusLg { get; init; } = Vars.Var(Shape.ExtraLarge);

    // --- 3. ЗАЗОР иконка-метка (extended FAB) ---
    /// <summary>Gap token (<c>0.75rem</c>).</summary>
    [CssVar(Fab.Gap)] public string Gap { get; init; } = "0.75rem";

    // --- 4. ТЕНИ (rest / hover) ---
    /// <summary>Shadow token (<c>var(--flare-elevation-3)</c>).</summary>
    [CssVar(Fab.Shadow)] public string Shadow { get; init; } = Vars.Var(Elevation.Level3);
    /// <summary>Hover shadow token (<c>var(--flare-elevation-4)</c>).</summary>
    [CssVar(Fab.HoverShadow)] public string HoverShadow { get; init; } = Vars.Var(Elevation.Level4);

    // --- 5. ОТСТУП якоря от края экрана ---
    /// <summary>Anchor offset token (<c>1.5rem</c>).</summary>
    [CssVar(Fab.AnchorOffset)] public string AnchorOffset { get; init; } = "1.5rem";
}
