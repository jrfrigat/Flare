using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Group-wide tree state that <see cref="FlareTreeView"/> shares with every descendant
/// <see cref="FlareTreeItem"/> through a single cascading value: whether drag-and-drop is enabled, and
/// the registry an item puts itself in so a drop can name it. Per-item nesting depth stays a separate
/// <c>Level</c> cascade because it changes for each row, whereas everything here is constant for the
/// whole tree.
///
/// The drop callback and the announcement travel here for the keyboard gesture: unlike a pointer drop,
/// which the view resolves, a keyboard move starts on one item and has to reach its neighbours, so the
/// item asks this context for the sibling and raises the same callback the pointer path uses.
/// </summary>
internal sealed class FlareTreeContext
{
    /// <summary>Whether drag-and-drop reordering is enabled for the tree.</summary>
    public required bool Draggable { get; init; }
    /// <summary>Maps each item's drag id to the item it stands for.</summary>
    public required TreeDragRegistry DragRegistry { get; init; }
    /// <summary>The drop callback the pointer path raises; the keyboard path raises the same one.</summary>
    public required EventCallback<TreeDropEventArgs> OnItemDrop { get; init; }
    /// <summary>Writes into the view's live region, so a keyboard move with no preview is spoken.</summary>
    public required Action<string> Announce { get; init; }

    /// <summary>Whether items offer the keyboard gesture: the tree is draggable and an owner listens
    /// for the move.</summary>
    public bool KeyboardReorder => Draggable && OnItemDrop.HasDelegate;
}
