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
    [CssVar(DrawerPanel.SurfaceColor)] public string SurfaceColor { get; init; } = Vars.Var(Color.SurfaceContainerLow);

    /// <summary>Width of the drawer in its default state.</summary>
    [CssVar(DrawerPanel.Width)] public string Width { get; init; } = "360px";

    /// <summary>Width of the mini variant drawer.</summary>
    [CssVar(DrawerPanel.MiniWidth)] public string MiniWidth { get; init; } = "72px";

    /// <summary>Width of the drawer when expanded to a specific breakpoint (sm/md/lg/xl).</summary>
    [CssVar(DrawerPanel.BreakpointSmWidth)] public string BreakpointSmWidth { get; init; } = "256px";

    /// <summary>Width of the drawer at the md breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointMdWidth)] public string BreakpointMdWidth { get; init; } = "256px";

    /// <summary>Width of the drawer at the lg breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointLgWidth)] public string BreakpointLgWidth { get; init; } = "360px";

    /// <summary>Width of the drawer at the xl breakpoint.</summary>
    [CssVar(DrawerPanel.BreakpointXlWidth)] public string BreakpointXlWidth { get; init; } = "360px";

    /// <summary>Elevation (box-shadow) of the drawer.</summary>
    [CssVar(DrawerPanel.Elevation)] public string Elevation { get; init; } = Vars.Var(Flare.Css.Tokens.Elevation.Level1);

    /// <summary>Border radius of the drawer (for top/bottom variants).</summary>
    [CssVar(DrawerPanel.Radius)] public string Radius { get; init; } = Vars.Var(Shape.ExtraLarge);

    /// <summary>Background color of the scrim (overlay).</summary>
    [CssVar(DrawerPanel.ScrimColor)] public string ScrimColor { get; init; } = Vars.Var(Color.Scrim);

    /// <summary>Opacity of the scrim.</summary>
    [CssVar(DrawerPanel.ScrimOpacity)] public string ScrimOpacity { get; init; } = "0.32";

    /// <summary>Transition duration for drawer open/close.</summary>
    [CssVar(DrawerPanel.TransitionDuration)] public string TransitionDuration { get; init; } = Vars.Var(Motion.DurationMedium2);

    /// <summary>Transition easing for drawer open/close.</summary>
    [CssVar(DrawerPanel.TransitionEasing)] public string TransitionEasing { get; init; } = Vars.Var(Motion.EasingStandard);

    /// <summary>Padding inside the drawer header.</summary>
    [CssVar(DrawerPanel.HeaderPadding)] public string HeaderPadding { get; init; } = "16px";

    /// <summary>Padding inside the drawer content.</summary>
    [CssVar(DrawerPanel.ContentPadding)] public string ContentPadding { get; init; } = "8px 0";

    /// <summary>Color of the drawer title text.</summary>
    [CssVar(DrawerPanel.TitleColor)] public string TitleColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Font family of the drawer title.</summary>
    [CssVar(DrawerPanel.TitleFontFamily)] public string TitleFontFamily { get; init; } = Vars.Var(Typography.Font("title-large"));

    /// <summary>Font size of the drawer title.</summary>
    [CssVar(DrawerPanel.TitleFontSize)] public string TitleFontSize { get; init; } = Vars.Var(Typography.Size("title-large"));
}
