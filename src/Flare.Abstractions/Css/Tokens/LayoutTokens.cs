namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for layout.</summary>
public static class LayoutField
{
    /// <summary>CSS custom-property name for the app bar height token.</summary>
    public const string AppBarHeight = "--flare-layout-appbar-height";
    /// <summary>CSS custom-property name for the dense app bar height token.</summary>
    public const string AppBarHeightDense = "--flare-layout-appbar-height-dense";
    /// <summary>CSS custom-property name for the app bar background token (defaults to the surface role).</summary>
    public const string AppBarBg = "--flare-layout-appbar-bg";
    /// <summary>CSS custom-property name for the content padding token.</summary>
    public const string ContentPadding = "--flare-layout-content-padding";
    /// <summary>CSS custom-property name for the content padding mobile token.</summary>
    public const string ContentPaddingMobile = "--flare-layout-content-padding-mobile";
    /// <summary>CSS custom-property name for the drawer rail width token.</summary>
    public const string DrawerRailWidth = "--flare-layout-drawer-rail-width";
    /// <summary>CSS custom-property name for the drawer width token.</summary>
    public const string DrawerWidth = "--flare-layout-drawer-width";
    /// <summary>CSS custom-property name for the shell app bar's bottom edge, as a <c>border</c>
    /// shorthand. Set to <c>none</c> for a theme that separates the bar by tone or elevation.</summary>
    public const string AppBarBorder = "--flare-layout-appbar-border";
    /// <summary>CSS custom-property name for the shell drawer's edge against the content, as a
    /// <c>border</c> shorthand. Applied to the trailing edge, or the leading edge of an end-anchored
    /// drawer.</summary>
    public const string DrawerBorder = "--flare-layout-drawer-border";
    /// <summary>CSS custom-property name for the app bar shadow token.</summary>
    public const string AppBarShadow = "--flare-layout-appbar-shadow";
    /// <summary>CSS custom-property name for the shell's own plane - the page behind every other one.</summary>
    public const string ShellBg = "--flare-layout-shell-bg";
    /// <summary>CSS custom-property name for the drawer plane.</summary>
    public const string DrawerBg = "--flare-layout-drawer-bg";
    /// <summary>CSS custom-property name for the collapsed rail's plane, so a theme may set the rail
    /// apart from the expanded drawer. Point it at the drawer's own plane to keep them the same.</summary>
    public const string RailBg = "--flare-layout-rail-bg";
    /// <summary>CSS custom-property name for the content plane.</summary>
    public const string ContentBg = "--flare-layout-content-bg";
    /// <summary>CSS custom-property name for the inline offset of the drawer's edge shadow. Core gives it
    /// its sign, so the shadow lands on the edge facing the content whichever side the drawer is anchored
    /// to and whichever way the document reads.</summary>
    public const string DrawerShadowOffset = "--flare-layout-drawer-shadow-offset";
    /// <summary>CSS custom-property name for the blur of the drawer's edge shadow.</summary>
    public const string DrawerShadowBlur = "--flare-layout-drawer-shadow-blur";
    /// <summary>CSS custom-property name for the colour of the drawer's edge shadow.</summary>
    public const string DrawerShadowColor = "--flare-layout-drawer-shadow-color";
}
