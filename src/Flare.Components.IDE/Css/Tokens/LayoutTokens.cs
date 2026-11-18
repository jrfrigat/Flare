namespace Flare.Css.Tokens.Ide;

// CSS custom-property name constants for the Flare IDE shell, in the Flare.Css.Tokens.Ide
// sub-namespace. These are layout-geometry variables (panel sizes, rail width, command-bar
// metrics) -- NOT theme tokens; all visual theming still flows through the base Flare design tokens
// (--flare-color-*, --flare-shape-*, ...). Reference them as Css.Tokens.Ide.<Holder>.<Member>; wrap
// a name in var(...) via Css.Tokens.Vars.Var.

/// <summary><c>FlareIdeLayout</c> region sizing and command-bar geometry (defaults live in <c>flare-ide.css</c>).</summary>
public static class Layout
{
    // Literal values, not interpolation: Flare.CssAudit reads these constants to prove every
    // --flare-ide-* declaration in flare-ide.css has a registry entry.

    /// <summary>Docked left tool-panel width.</summary>
    public const string LeftWidth = "--flare-ide-left-width";
    /// <summary>Docked right tool-panel width.</summary>
    public const string RightWidth = "--flare-ide-right-width";
    /// <summary>Docked bottom tool-panel height.</summary>
    public const string BottomHeight = "--flare-ide-bottom-height";
    /// <summary>Collapsed-panel rail thickness.</summary>
    public const string Rail = "--flare-ide-rail";
    /// <summary>Grid track the left docking column currently occupies.</summary>
    public const string LeftTrack = "--flare-ide-left-track";
    /// <summary>Grid track the right docking column currently occupies.</summary>
    public const string RightTrack = "--flare-ide-right-track";
    /// <summary>Gap between tracks of a docking layout.</summary>
    public const string Gap = "--flare-ide-layout-gap";
    /// <summary>Tool-window title row height.</summary>
    public const string ToolHeaderHeight = "--flare-ide-tool-header-height";
    /// <summary>Tool-window tab height.</summary>
    public const string ToolTabHeight = "--flare-ide-tool-tab-height";
    /// <summary>Command-bar height.</summary>
    public const string ToolbarHeight = "--flare-ide-toolbar-height";
    /// <summary>Command-bar height in the dense variant.</summary>
    public const string ToolbarDenseHeight = "--flare-ide-toolbar-dense-height";
    /// <summary>Command-bar padding shorthand (block inline).</summary>
    public const string ToolbarPadding = "--flare-ide-toolbar-padding";
    /// <summary>Command-bar padding shorthand in the dense variant.</summary>
    public const string ToolbarDensePadding = "--flare-ide-toolbar-dense-padding";
    /// <summary>Command-bar control-well height (the measured 24px total).</summary>
    public const string ToolbarWellHeight = "--flare-ide-toolbar-well-height";
    /// <summary>Glyph size of a document tab's close button.</summary>
    public const string DocTabCloseIcon = "--flare-ide-doc-tab-close-icon";
    /// <summary>Menu-bar height.</summary>
    public const string MenubarHeight = "--flare-ide-menubar-height";
    /// <summary>Gap between menu-bar items.</summary>
    public const string MenuBarGap = "--flare-ide-menubar-gap";
    /// <summary>Status-bar padding shorthand (block inline).</summary>
    public const string StatusBarPadding = "--flare-ide-statusbar-padding";
    /// <summary>Gap between status-bar segments.</summary>
    public const string StatusBarGap = "--flare-ide-statusbar-gap";
    /// <summary>Sheet-tab strip height.</summary>
    public const string SheetTabsHeight = "--flare-ide-sheet-tabs-height";
    /// <summary>Sheet-tab strip padding shorthand (block inline).</summary>
    public const string SheetTabsPadding = "--flare-ide-sheet-tabs-padding";
    /// <summary>Gap between sheet tabs.</summary>
    public const string SheetTabsGap = "--flare-ide-sheet-tabs-gap";
    /// <summary>Underline bar height under a sheet tab.</summary>
    public const string SheetTabsUnderlineHeight = "--flare-ide-sheet-tabs-underline-height";
    /// <summary>Editor breadcrumb row height.</summary>
    public const string BreadcrumbBarHeight = "--flare-ide-breadcrumb-bar-height";
    /// <summary>Inline padding of the editor breadcrumb area.</summary>
    public const string BreadcrumbBarPadding = "--flare-ide-breadcrumb-bar-padding";
    /// <summary>Inline padding of one editor breadcrumb segment.</summary>
    public const string BreadcrumbSegmentPadding = "--flare-ide-breadcrumb-segment-padding";
    /// <summary>Gap between a breadcrumb segment's icon, label and chevron.</summary>
    public const string BreadcrumbSegmentGap = "--flare-ide-breadcrumb-segment-gap";
    /// <summary>Icon size in an editor breadcrumb segment.</summary>
    public const string BreadcrumbIconSize = "--flare-ide-breadcrumb-icon-size";
    /// <summary>Chevron size in a dropdown editor breadcrumb segment.</summary>
    public const string BreadcrumbChevronSize = "--flare-ide-breadcrumb-chevron-size";
    /// <summary>Inline padding of the editor breadcrumb action area.</summary>
    public const string BreadcrumbActionsPadding = "--flare-ide-breadcrumb-actions-padding";
    /// <summary>Gap between commands in the editor breadcrumb action area.</summary>
    public const string BreadcrumbActionsGap = "--flare-ide-breadcrumb-actions-gap";
}
