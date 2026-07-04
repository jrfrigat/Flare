using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Drawer component.
/// These control the geometry, spacing, and appearance of navigation drawers.
/// </summary>
public sealed record DrawerTokens
{
    /// <summary>Background color of the drawer surface.</summary>
    [CssVar(DrawerPanel.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Width of the drawer in its default state.</summary>
    [CssVar(DrawerPanel.Width)] public required string Width { get; init; }

    /// <summary>Width of the mini variant drawer.</summary>
    [CssVar(DrawerPanel.MiniWidth)] public required string MiniWidth { get; init; }

    /// <summary>Width of the drawer when expanded to a specific breakpoint (sm/md/lg/xl).</summary>
    [CssVar(DrawerPanel.BreakpointSmWidth)] public required string BreakpointSmWidth { get; init; }

    /// <summary>Width of the drawer at the md breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointMdWidth)] public required string BreakpointMdWidth { get; init; }

    /// <summary>Width of the drawer at the lg breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointLgWidth)] public required string BreakpointLgWidth { get; init; }

    /// <summary>Width of the drawer at the xl breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointXlWidth)] public required string BreakpointXlWidth { get; init; }

    /// <summary>Elevation (box-shadow) of the drawer.</summary>
    [CssVar(DrawerPanel.Elevation)] public required string Elevation { get; init; }

    /// <summary>Border radius of the drawer (for top/bottom variants).</summary>
    [CssVar(DrawerPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Background color of the scrim (overlay).</summary>
    [CssVar(DrawerPanel.ScrimColor)] public required string ScrimColor { get; init; }

    /// <summary>Opacity of the scrim.</summary>
    [CssVar(DrawerPanel.ScrimOpacity)] public required string ScrimOpacity { get; init; }

    /// <summary>Transition duration for drawer open/close.</summary>
    [CssVar(DrawerPanel.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for drawer open/close.</summary>
    [CssVar(DrawerPanel.TransitionEasing)] public required string TransitionEasing { get; init; }

    /// <summary>Padding inside the drawer header.</summary>
    [CssVar(DrawerPanel.HeaderPadding)] public required string HeaderPadding { get; init; }

    /// <summary>Padding inside the drawer content.</summary>
    [CssVar(DrawerPanel.ContentPadding)] public required string ContentPadding { get; init; }

    /// <summary>Color of the drawer title text.</summary>
    [CssVar(DrawerPanel.TitleColor)] public required string TitleColor { get; init; }

    /// <summary>Font family of the drawer title.</summary>
    [CssVar(DrawerPanel.TitleFontFamily)] public required string TitleFontFamily { get; init; }

    /// <summary>Font size of the drawer title.</summary>
    [CssVar(DrawerPanel.TitleFontSize)] public required string TitleFontSize { get; init; }
}
