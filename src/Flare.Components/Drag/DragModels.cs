namespace Flare.Components;

/// <summary>
/// What a <see cref="FlareDropZone"/> accepts, which is also what the drop resolves to.
/// </summary>
public enum DropPlacement
{
    /// <summary>The zone itself is the destination: a column that holds cards, a folder that holds
    /// files. The drop reports the zone and nothing else - the order inside it is not the caller's to
    /// decide here.</summary>
    Into,

    /// <summary>The zone holds an ordered list, and a drop resolves to a position in it. Each item is
    /// split in half: the near half puts the dragged item before it, the far half after.</summary>
    Between,

    /// <summary>Both: each item is split into thirds, so a drop can land before it, inside it, or after
    /// it. This is what a tree needs, where a node is at once a position in its parent's order and a
    /// container of its own.</summary>
    Both,
}

/// <summary>
/// Resolves a draggable id to its payload, for a component that renders its own rows rather than
/// wrapping them in <see cref="FlareDraggable"/>. A data grid's row is a <c>tr</c> and a tree's node is
/// an <c>li</c>; neither can take a wrapper element without producing invalid markup, so they emit the
/// model's data attributes on their own element and answer this instead.
/// </summary>
/// <typeparam name="TPayload">The type carried by the dragged item.</typeparam>
/// <param name="id">The value the element's <c>data-flare-drag</c> attribute carries.</param>
/// <param name="payload">The item that id stands for.</param>
/// <returns><c>true</c> when the id was recognised. An unrecognised id is refused rather than dropped
/// as a default, which is what makes this correct for a value-type payload too.</returns>
public delegate bool DragItemResolver<TPayload>(string id, out TPayload payload);

/// <summary>Where a drop landed relative to the item under the pointer.</summary>
public enum DropEdge
{
    /// <summary>In front of the item under the pointer.</summary>
    Before,

    /// <summary>On the item itself, or - when no item was under the pointer - in the zone.</summary>
    Into,

    /// <summary>Behind the item under the pointer.</summary>
    After,
}

/// <summary>
/// Where a drag ended. Raised once per completed drop by <see cref="FlareDragContext{TPayload}"/>.
/// </summary>
/// <typeparam name="TPayload">The type carried by the dragged item.</typeparam>
public sealed class FlareDropEventArgs<TPayload>
{
    /// <summary>The dragged item's payload.</summary>
    public required TPayload Payload { get; init; }

    /// <summary>The <see cref="FlareDropZone.Target"/> the drop landed in.</summary>
    public required string TargetId { get; init; }

    /// <summary>The zone the drag started from, or <c>null</c> when the item was not inside one.</summary>
    public string? SourceTargetId { get; init; }

    /// <summary>
    /// The position the item takes in the target zone, counted WITHOUT the dragged item - so it is the
    /// index the item ends up at, and it can be used directly with <c>List.Insert</c> after the item has
    /// been removed. <c>-1</c> when the drop was into a zone or onto an item rather than between items.
    /// </summary>
    public int Index { get; init; }

    /// <summary>Where the drop landed relative to <see cref="OverPayload"/>.</summary>
    public DropEdge Edge { get; init; }

    /// <summary>The payload of the item the pointer was over, when it was over one.</summary>
    public TPayload? OverPayload { get; init; }

    /// <summary>Whether an item was under the pointer at the drop, which is what makes
    /// <see cref="OverPayload"/> meaningful even for a payload type whose default is not null.</summary>
    public bool HasOverPayload { get; init; }
}
