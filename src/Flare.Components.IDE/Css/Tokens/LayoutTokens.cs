namespace Flare.Css.Tokens.Ide;

// CSS custom-property name constants for the Flare IDE shell, in the Flare.Css.Tokens.Ide
// sub-namespace. These are layout-geometry variables (panel sizes, rail width) -- NOT theme tokens;
// all visual theming still flows through the base Flare design tokens (--flare-color-*, --flare-shape-*,
// ...). Reference them as Css.Tokens.Ide.<Holder>.<Member>; wrap a name in var(...) via Css.Tokens.Vars.Var.

/// <summary><c>FlareIdeLayout</c> region-sizing variables (defaults live in <c>flare-ide.css</c>).</summary>
public static class Layout
{
    private const string Prefix = "--flare-ide";

    /// <summary>Docked left tool-panel width.</summary>
    public const string LeftWidth = $"{Prefix}-left-width";
    /// <summary>Docked right tool-panel width.</summary>
    public const string RightWidth = $"{Prefix}-right-width";
    /// <summary>Docked bottom tool-panel height.</summary>
    public const string BottomHeight = $"{Prefix}-bottom-height";
    /// <summary>Collapsed-panel rail thickness.</summary>
    public const string Rail = $"{Prefix}-rail";
}
