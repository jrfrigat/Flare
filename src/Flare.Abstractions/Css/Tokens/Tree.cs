namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareTreeView</c> / <c>FlareTreeItem</c>.</summary>
public static class Tree
{
    private const string FlareTree = $"{Vars.Flare}-tree";

    /// <summary>CSS custom-property name for the child-level indent token.</summary>
    public const string Indent = $"{FlareTree}-indent";
    /// <summary>CSS custom-property name for the expander / drag-handle square size token.</summary>
    public const string ToggleSize = $"{FlareTree}-toggle-size";
    /// <summary>CSS custom-property name for the row icon glyph size token.</summary>
    public const string IconSize = $"{FlareTree}-icon-size";
    /// <summary>CSS custom-property name for the selected-row background token.</summary>
    public const string SelectedBg = $"{FlareTree}-selected-bg";
    /// <summary>CSS custom-property name for the selected-row foreground token.</summary>
    public const string SelectedColor = $"{FlareTree}-selected-color";
    /// <summary>CSS custom-property name for the drag-and-drop indicator color token.</summary>
    public const string DropIndicatorColor = $"{FlareTree}-drop-indicator-color";
}
