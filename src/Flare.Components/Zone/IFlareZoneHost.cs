namespace Flare.Components;

/// <summary>
/// Host contract for a component that draws colored regions on a track (<see cref="FlareSlider"/>,
/// <see cref="FlareProgress"/>, <see cref="FlareMeter"/>). A <see cref="FlareZoneBase"/> child registers
/// itself with the nearest host through a cascaded value, and the host reads the registered regions to paint
/// them. Shared (with <c>ZoneCollection</c>) so the hosts reuse one registration path.
/// </summary>
public interface IFlareZoneHost
{
    /// <summary>Registers a zone child with the host.</summary>
    void RegisterZone(FlareZoneBase zone);
    /// <summary>Removes a previously registered zone child.</summary>
    void UnregisterZone(FlareZoneBase zone);
    /// <summary>Notifies the host that a registered zone's parameters changed, so it can repaint.</summary>
    void NotifyZoneChanged();
}
