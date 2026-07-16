namespace Flare.Components;

/// <summary>
/// Shared registration list for <see cref="FlareZoneBase"/> children, implementing
/// <see cref="IFlareZoneHost"/>. A track host (slider / progress / meter) holds one instance, cascades it to
/// its children, and reads them back through <see cref="Typed{T}"/>. Centralizes the register / unregister /
/// notify boilerplate the three hosts would otherwise each repeat.
/// </summary>
internal sealed class ZoneCollection : IFlareZoneHost
{
    private readonly List<FlareZoneBase> _zones = new();
    private readonly Action _onChanged;

    /// <summary>Creates a collection that invokes <paramref name="onChanged"/> whenever the set of zones, or a
    /// zone's parameters, change (typically the host's <c>StateHasChanged</c>).</summary>
    public ZoneCollection(Action onChanged) => _onChanged = onChanged;

    /// <summary>Number of registered children, whatever their concrete kind.</summary>
    public int Count => _zones.Count;

    /// <summary>
    /// The registered children as <typeparamref name="T"/>, in declaration order. Throws when a region of a
    /// different kind was declared inside this host - a slider/progress takes ranged zones while a meter takes
    /// weighted segments, and silently dropping the mismatch would leave the author with an invisible band and
    /// no explanation.
    /// </summary>
    /// <typeparam name="T">The region kind this host paints.</typeparam>
    /// <param name="hostName">Host component name, used in the error message.</param>
    /// <param name="expected">How this host's children should be declared, used in the error message.</param>
    public IEnumerable<T> Typed<T>(string hostName, string expected) where T : FlareZoneBase
    {
        foreach (var zone in _zones)
        {
            if (zone is not T typed)
                throw new InvalidOperationException(
                    $"<{zone.GetType().Name}> cannot be used inside <{hostName}>. {hostName} takes {expected}.");
            yield return typed;
        }
    }

    /// <inheritdoc />
    public void RegisterZone(FlareZoneBase zone)
    {
        if (_zones.Contains(zone)) return;
        _zones.Add(zone);
        _onChanged();
    }

    /// <inheritdoc />
    public void UnregisterZone(FlareZoneBase zone)
    {
        if (_zones.Remove(zone)) _onChanged();
    }

    /// <inheritdoc />
    public void NotifyZoneChanged() => _onChanged();
}
