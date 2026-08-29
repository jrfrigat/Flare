using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the navigation drawer. Surface, elevation, radius, scrim, motion, padding and title
/// styling are NOT tokens here - drawer.css reuses the shared color/elevation/shape/motion/spacing/typescale
/// scales directly. What remains is the drawer-specific geometry and the two panel edges.
/// </summary>
public sealed record DrawerTokens
{
    /// <summary>Width of the drawer when open.</summary>
    [CssVar(DrawerPanel.Width)] public required string Width { get; init; }

    /// <summary>Width of the mini (rail) drawer variant.</summary>
    [CssVar(DrawerPanel.MiniWidth)] public required string MiniWidth { get; init; }

    /// <summary>Edge an always-visible drawer (permanent or mini) draws against the content, as a
    /// <c>border</c> shorthand. <c>none</c> for a language that relies on surface tone instead.</summary>
    [CssVar(DrawerPanel.Border)] public required string Border { get; init; }

    /// <summary>Divider between a side panel's fixed regions and its scrolling body, as a <c>border</c>
    /// shorthand. The drawer header and the nav menu's header and footer all read it, so the rule under a
    /// panel header is set once for every panel.</summary>
    [CssVar(DrawerPanel.SectionBorder)] public required string SectionBorder { get; init; }
}
