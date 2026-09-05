using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for the shared drag-and-drop model: what the item left behind looks like, what the
/// preview that follows the pointer looks like, and how a drop target announces itself.
///
/// One family rather than one per surface. A kanban card, a data-grid row and a tree node are the same
/// gesture, and before this each was painted from its own component tokens - so a dragged card and a
/// dragged row did not look related, and a theme had to restyle every surface separately to fix that.
///
/// Deliberately absent: the widths. The drop-zone outline and the insertion line both read
/// <c>--flare-border-width-emphasis</c>, whose own documentation already names drop targets as its
/// purpose, so a second length here would be a duplicate a theme could set inconsistently. So is the
/// preview's corner radius - the preview is a clone of the item, and it keeps the item's own shape.
/// </summary>
public sealed record DragTokens
{
    /// <summary>Opacity of the item left in place while its copy is in flight.</summary>
    [CssVar(Drag.SourceOpacity)] public required string SourceOpacity { get; init; }

    /// <summary>Shadow cast by the preview that follows the pointer.</summary>
    [CssVar(Drag.PreviewElevation)] public required string PreviewElevation { get; init; }

    /// <summary>Opacity of the preview that follows the pointer.</summary>
    [CssVar(Drag.PreviewOpacity)] public required string PreviewOpacity { get; init; }

    /// <summary>Background of the drop zone the pointer is currently over.</summary>
    [CssVar(Drag.ZoneActiveBackground)] public required string ZoneActiveBackground { get; init; }

    /// <summary>Outline color of the drop zone the pointer is currently over.</summary>
    [CssVar(Drag.ZoneActiveOutline)] public required string ZoneActiveOutline { get; init; }

    /// <summary>Color of the insertion line drawn where an ordered drop would land.</summary>
    [CssVar(Drag.IndicatorColor)] public required string IndicatorColor { get; init; }
}
