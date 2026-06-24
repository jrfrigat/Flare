namespace Flare.Core.Tokens.Components;

/// <summary>
/// Design tokens for Drawer component.
/// These control the geometry, spacing, and appearance of navigation drawers.
/// </summary>
public sealed record DrawerTokens
{
    /// <summary>Background color of the drawer surface.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-surface-container-low)";

    /// <summary>Width of the drawer in its default state.</summary>
    public string Width { get; init; } = "360px";

    /// <summary>Width of the mini variant drawer.</summary>
    public string MiniWidth { get; init; } = "72px";

    /// <summary>Width of the drawer when expanded to a specific breakpoint (sm/md/lg/xl).</summary>
    public string BreakpointSmWidth { get; init; } = "256px";

    /// <summary>Width of the drawer at the md breakpoint.</summary>
    public string BreakpointMdWidth { get; init; } = "256px";

    /// <summary>Width of the drawer at the lg breakpoint.</summary>
    public string BreakpointLgWidth { get; init; } = "360px";

    /// <summary>Width of the drawer at the xl breakpoint.</summary>
    public string BreakpointXlWidth { get; init; } = "360px";

    /// <summary>Elevation (box-shadow) of the drawer.</summary>
    public string Elevation { get; init; } = "var(--flare-elevation-1)";

    /// <summary>Border radius of the drawer (for top/bottom variants).</summary>
    public string Radius { get; init; } = "var(--flare-shape-extra-large)";

    /// <summary>Background color of the scrim (overlay).</summary>
    public string ScrimColor { get; init; } = "var(--flare-color-scrim)";

    /// <summary>Opacity of the scrim.</summary>
    public string ScrimOpacity { get; init; } = "0.32";

    /// <summary>Transition duration for drawer open/close.</summary>
    public string TransitionDuration { get; init; } = "var(--flare-motion-duration-medium2)";

    /// <summary>Transition easing for drawer open/close.</summary>
    public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Padding inside the drawer header.</summary>
    public string HeaderPadding { get; init; } = "16px";

    /// <summary>Padding inside the drawer content.</summary>
    public string ContentPadding { get; init; } = "8px 0";

    /// <summary>Color of the drawer title text.</summary>
    public string TitleColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the drawer title.</summary>
    public string TitleFontFamily { get; init; } = "var(--flare-typescale-title-large-font)";

    /// <summary>Font size of the drawer title.</summary>
    public string TitleFontSize { get; init; } = "var(--flare-typescale-title-large-size)";
}
