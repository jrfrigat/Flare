namespace Flare.Components;

/// <summary>
/// The registry a <see cref="FlareDragContext{TPayload}"/> cascades to the draggables and drop zones
/// beneath it. Non-generic on purpose: it is what lets <see cref="FlareDraggable"/> and
/// <see cref="FlareDropZone"/> stay non-generic in markup, so a zone that carries no payload of its own
/// never has to spell out a type argument it cannot infer. The payload type is enforced where it is
/// declared and where it is consumed - on the context and on its <c>OnDrop</c>.
///
/// It holds no in-flight drag state. While the pointer is moving, the browser owns the gesture (see
/// <c>flare-dragdrop.js</c>); this side is asked two questions per drag, at the start and at the drop.
/// </summary>
internal sealed class DragCoordinator
{
    private readonly Dictionary<string, FlareDraggable> _items = new(StringComparer.Ordinal);
    private readonly Dictionary<string, FlareDropZone> _zones = new(StringComparer.Ordinal);

    /// <summary>The context's group, inherited by any draggable or zone that does not name its own.</summary>
    public string Group { get; init; } = "";

    /// <summary>Registered zones, for the "which targets accept this item" answer.</summary>
    public IEnumerable<FlareDropZone> Zones => _zones.Values;

    public void RegisterItem(FlareDraggable item) => _items[item.ItemId] = item;

    // The key is passed in rather than read back off the component: an item whose Id parameter changed
    // has to be removed under the id it was REGISTERED with, not the one it now reports.
    // The reference check guards the other order of events - when a list is re-keyed the replacement
    // registers before the old component is disposed, and an unguarded remove would delete it.
    public void UnregisterItem(FlareDraggable item, string key)
    {
        if (_items.TryGetValue(key, out var current) && ReferenceEquals(current, item))
            _items.Remove(key);
    }

    public void RegisterZone(FlareDropZone zone) => _zones[zone.Target] = zone;

    public void UnregisterZone(FlareDropZone zone, string key)
    {
        if (_zones.TryGetValue(key, out var current) && ReferenceEquals(current, zone))
            _zones.Remove(key);
    }

    public FlareDraggable? Item(string? id) =>
        id is not null && _items.TryGetValue(id, out var item) ? item : null;

    public FlareDropZone? Zone(string? target) =>
        target is not null && _zones.TryGetValue(target, out var zone) ? zone : null;
}
