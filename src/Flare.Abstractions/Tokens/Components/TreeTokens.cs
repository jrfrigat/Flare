using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareTreeView</c> / <c>FlareTreeItem</c> (indent, expander/handle size,
/// icon size, selection and drag-drop indicator). Row radius and hover reuse the shared
/// <c>--flare-nav-item-radius</c> and <c>--flare-state-hover-opacity</c>; row typography and padding
/// reuse the shared typescale/spacing tokens.
/// </summary>
public sealed record TreeTokens
{
    /// <summary>Horizontal indent applied per nesting level.</summary>
    [CssVar(Tree.Indent)] public required string Indent { get; init; }

    /// <summary>Square size of the expander toggle, its placeholder and the drag handle.</summary>
    [CssVar(Tree.ToggleSize)] public required string ToggleSize { get; init; }

    /// <summary>Glyph size of the row's expander and leading icon.</summary>
    [CssVar(Tree.IconSize)] public required string IconSize { get; init; }

    /// <summary>Background of a selected row.</summary>
    [CssVar(Tree.SelectedBg)] public required string SelectedBg { get; init; }

    /// <summary>Foreground (text/icon) color of a selected row.</summary>
    [CssVar(Tree.SelectedColor)] public required string SelectedColor { get; init; }

    /// <summary>Color of the drag-and-drop insertion indicator (before/after lines).</summary>
    [CssVar(Tree.DropIndicatorColor)] public required string DropIndicatorColor { get; init; }
}
