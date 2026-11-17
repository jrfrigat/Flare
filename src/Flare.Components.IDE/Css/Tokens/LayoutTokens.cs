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
    /// <summary>Command-bar height.</summary>
    public const string ToolbarHeight = "--flare-ide-toolbar-height";
    /// <summary>Command-bar height in the dense variant.</summary>
    public const string ToolbarDenseHeight = "--flare-ide-toolbar-dense-height";
    /// <summary>Command-bar padding shorthand (block inline).</summary>
    public const string ToolbarPadding = "--flare-ide-toolbar-padding";
    /// <summary>Command-bar padding shorthand in the dense variant.</summary>
    public const string ToolbarDensePadding = "--flare-ide-toolbar-dense-padding";
    /// <summary>Glyph size of a document tab's close button.</summary>
    public const string DocTabCloseIcon = "--flare-ide-doc-tab-close-icon";
    /// <summary>Menu-bar height.</summary>
    public const string MenubarHeight = "--flare-ide-menubar-height";
    /// <summary>Status-bar padding shorthand (block inline).</summary>
    public const string StatusBarPadding = "--flare-ide-statusbar-padding";
    /// <summary>Gap between status-bar segments.</summary>
    public const string StatusBarGap = "--flare-ide-statusbar-gap";
}
