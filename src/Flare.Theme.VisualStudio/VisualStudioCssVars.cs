namespace Flare.Theme.VisualStudio;

/// <summary>
/// Visual Studio theme-private CSS custom-property names (focus glow + the IDE tab strip). These are the
/// theme's OWN tokens (not part of the Flare core contract), so they live in the theme project - the core
/// stays theme-agnostic. The scoped VS CSS reads them and <see cref="VisualStudioTokens"/> assigns values.
/// </summary>
public static class VisualStudioCssVars
{
    /// <summary>Focus glow color.</summary>
    public const string Focus = "--flare-vs-focus";
    /// <summary>Tab strip background.</summary>
    public const string TabStripBg = "--flare-vs-tab-strip-bg";
    /// <summary>Gap between tabs.</summary>
    public const string TabGap = "--flare-vs-tab-gap";
    /// <summary>Tab corner radius.</summary>
    public const string TabRadius = "--flare-vs-tab-radius";
    /// <summary>Active tab background.</summary>
    public const string TabActiveBg = "--flare-vs-tab-active-bg";

    /// <summary>Menu and title bar surface.</summary>
    public const string ChromeTitleBar = "--flare-vs-chrome-title-bar";
    /// <summary>Command bar surface - the toolbar strip under the menu bar.</summary>
    public const string ChromeCommandBar = "--flare-vs-chrome-command-bar";
    /// <summary>Document tab strip surface.</summary>
    public const string ChromeTabStrip = "--flare-vs-chrome-tab-strip";
    /// <summary>Active document tab surface.</summary>
    public const string ChromeTabActive = "--flare-vs-chrome-tab-active";
    /// <summary>Editor surface.</summary>
    public const string ChromeEditor = "--flare-vs-chrome-editor";
    /// <summary>Editor breadcrumb bar surface.</summary>
    public const string ChromeBreadcrumb = "--flare-vs-chrome-breadcrumb";
    /// <summary>Editor breadcrumb top/bottom edge and its action-area surface.</summary>
    public const string ChromeBreadcrumbEdge = "--flare-vs-chrome-breadcrumb-edge";
    /// <summary>Tool window surface.</summary>
    public const string ChromeToolWindow = "--flare-vs-chrome-tool-window";
    /// <summary>Tool window tab strip and title surface.</summary>
    public const string ChromeToolTab = "--flare-vs-chrome-tool-tab";
    /// <summary>Tool search field and input well surface.</summary>
    public const string ChromeField = "--flare-vs-chrome-field";
    /// <summary>Bottom panel tab strip surface.</summary>
    public const string ChromeBottomPanel = "--flare-vs-chrome-bottom-panel";
    /// <summary>Underline bar under a bottom-panel tab.</summary>
    public const string ChromeBottomTabUnderline = "--flare-vs-chrome-bottom-tab-underline";
    /// <summary>Status bar surface.</summary>
    public const string ChromeStatusBar = "--flare-vs-chrome-status-bar";
    /// <summary>Hard edge line under the title and command bars.</summary>
    public const string ChromeBarEdge = "--flare-vs-chrome-bar-edge";
    /// <summary>Separator line between neighbouring chrome regions.</summary>
    public const string ChromeDivider = "--flare-vs-chrome-divider";
    /// <summary>Outline stroke around chrome controls.</summary>
    public const string ChromeOutline = "--flare-vs-chrome-outline";
    /// <summary>Dropdown menu panel surface.</summary>
    public const string ChromeMenu = "--flare-vs-chrome-menu";
    /// <summary>Hover surface inside menus and panels.</summary>
    public const string ChromeHover = "--flare-vs-chrome-hover";
    /// <summary>Hover surface of a menu bar title.</summary>
    public const string ChromeMenuBarHover = "--flare-vs-chrome-menubar-hover";
    /// <summary>Accent used for focus outlines and selection lines.</summary>
    public const string ChromeAccent = "--flare-vs-chrome-accent";
    /// <summary>Primary chrome text color.</summary>
    public const string ChromeText = "--flare-vs-chrome-text";
    /// <summary>Secondary chrome text color.</summary>
    public const string ChromeTextSecondary = "--flare-vs-chrome-text-secondary";
    /// <summary>Disabled chrome text and shortcut color.</summary>
    public const string ChromeTextDisabled = "--flare-vs-chrome-text-disabled";
    /// <summary>Scrollbar thumb color.</summary>
    public const string ChromeScrollbarThumb = "--flare-vs-chrome-scrollbar-thumb";
    /// <summary>Scrollbar track color, kept on the panel surface it sits over.</summary>
    public const string ChromeScrollbarTrack = "--flare-vs-chrome-scrollbar-track";
    /// <summary>Corner radius of a tool search field (larger than a regular input).</summary>
    public const string ChromeSearchRadius = "--flare-vs-chrome-search-radius";
}
