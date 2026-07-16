namespace Flare.Components;

/// <summary>
/// Host contract for the track-band family (<see cref="FlareSlider"/>, <see cref="FlareProgress"/>,
/// <see cref="FlareMeter"/>): a component that draws one or more colored <see cref="FlareZone"/> regions
/// on a track. A zone child registers itself with the nearest host through a cascaded value, and the host
/// reads the registered zones to paint them. Sharing this contract (and <c>ZoneCollection</c>) lets the
/// three hosts reuse one zone child and one registration path instead of each reinventing them.
/// </summary>
public interface IFlareZoneHost
{
    /// <summary>Registers a zone child with the host.</summary>
    void RegisterZone(FlareZone zone);
    /// <summary>Removes a previously registered zone child.</summary>
    void UnregisterZone(FlareZone zone);
    /// <summary>Notifies the host that a registered zone's parameters changed, so it can repaint.</summary>
    void NotifyZoneChanged();
}
