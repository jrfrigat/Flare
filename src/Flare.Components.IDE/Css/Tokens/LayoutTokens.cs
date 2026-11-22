namespace Flare.Css.Tokens.Ide;

// CSS custom-property name constants for the Flare IDE chrome, in the Flare.Css.Tokens.Ide
// sub-namespace. These are geometry variables (tool-window, command-bar, menu, status and breadcrumb
// metrics) -- NOT theme tokens; all visual theming still flows through the base Flare design tokens
// (--flare-color-*, --flare-shape-*, ...). Reference them as Css.Tokens.Ide.Layout.<Member>; wrap a
// name in var(...) via Css.Tokens.Vars.Var.

/// <summary>Command-bar, menu-bar, status-bar, breadcrumb and tool-window geometry (defaults live in <c>flare-ide.css</c>).</summary>
public static class Layout
{
    // Literal values, not interpolation: Flare.CssAudit reads these constants to prove every
    // --flare-ide-* declaration in flare-ide.css has a registry entry.

    /// <summary>Tool-window title row height.</summary>
    public const string ToolHeaderHeight = "--flare-ide-tool-header-height";
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
    /// <summary>Menu-bar height.</summary>
    public const string MenubarHeight = "--flare-ide-menubar-height";
    /// <summary>Gap between menu-bar items.</summary>
    public const string MenuBarGap = "--flare-ide-menubar-gap";
    /// <summary>Status-bar padding shorthand (block inline).</summary>
    public const string StatusBarPadding = "--flare-ide-statusbar-padding";
    /// <summary>Gap between status-bar segments.</summary>
    public const string StatusBarGap = "--flare-ide-statusbar-gap";
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
