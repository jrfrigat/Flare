using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the navigation drawer. Surface, elevation, radius, scrim, motion, padding and title
/// styling are NOT tokens here - drawer.css reuses the shared color/elevation/shape/motion/spacing/typescale
/// scales directly. Only the two drawer-specific widths are tokens.
/// </summary>
public sealed record DrawerTokens
{
    /// <summary>Width of the drawer when open.</summary>
    [CssVar(DrawerPanel.Width)] public required string Width { get; init; }

    /// <summary>Width of the mini (rail) drawer variant.</summary>
    [CssVar(DrawerPanel.MiniWidth)] public required string MiniWidth { get; init; }
}
