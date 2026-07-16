namespace Flare.Components;

/// <summary>
/// Shared registration list for <see cref="FlareZone"/> children, implementing <see cref="IFlareZoneHost"/>.
/// A track-band host (slider / progress / meter) holds one instance, cascades it to its zone children, and
/// reads <see cref="Zones"/> to paint the bands. Centralizes the register / unregister / notify boilerplate
/// the three hosts would otherwise each repeat.
/// </summary>
internal sealed class ZoneCollection : IFlareZoneHost
{
    private readonly List<FlareZone> _zones = new();
    private readonly Action _onChanged;

    /// <summary>Creates a collection that invokes <paramref name="onChanged"/> whenever the set of zones,
    /// or a zone's parameters, change (typically the host's <c>StateHasChanged</c>).</summary>
    public ZoneCollection(Action onChanged) => _onChanged = onChanged;

    /// <summary>The registered zones, in child-declaration order.</summary>
    public IReadOnlyList<FlareZone> Zones => _zones;

    /// <inheritdoc />
    public void RegisterZone(FlareZone zone)
    {
        if (_zones.Contains(zone)) return;
        _zones.Add(zone);
        _onChanged();
    }

    /// <inheritdoc />
    public void UnregisterZone(FlareZone zone)
    {
        if (_zones.Remove(zone)) _onChanged();
    }

    /// <inheritdoc />
    public void NotifyZoneChanged() => _onChanged();
}
