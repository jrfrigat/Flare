using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Shared base for a colored region drawn on a track. Two concrete kinds derive from it, because the hosts
/// place a region in genuinely different coordinate systems:
/// <list type="bullet">
/// <item><see cref="FlareZone"/> - an absolute <c>[Start, End]</c> range on a scale the HOST owns
/// (<see cref="FlareSlider"/>'s Min..Max, <see cref="FlareProgress"/>'s 0-100).</item>
/// <item><see cref="FlareMeterSegment"/> - a proportional <c>Value</c> weight on <see cref="FlareMeter"/>,
/// where the parts themselves DEFINE the whole.</item>
/// </list>
/// This base owns the fill color and the registration handshake with the host, so both kinds - and all three
/// hosts - share one mechanism while each concrete type exposes only the parameters that apply to it.
/// A region renders no DOM of its own; the host paints the band.
/// </summary>
public abstract class FlareZoneBase : ComponentBase, IDisposable
{
    [CascadingParameter] private IFlareZoneHost? Host { get; set; }

    /// <summary>Fill color. Role (e.g. <see cref="FlareColor.Success"/>) -> shared class; custom
    /// (<c>FlareColor.Custom("#...")</c>) -> inline token. Defaults to <see cref="FlareColor.Primary"/>.</summary>
    [Parameter] public FlareColor Color { get; set; } = FlareColor.Primary;

    /// <summary>The parameter values that affect rendering. The base compares it between parameter sets and
    /// pokes the host only on a real change, so the host's re-render does not loop.</summary>
    protected abstract object Signature { get; }

    private bool _registered;
    private object? _last;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Host?.RegisterZone(this);
        _registered = Host is not null;
        _last = Signature;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var sig = Signature;
        if (_registered && _last is not null && !_last.Equals(sig))
            Host?.NotifyZoneChanged();
        _last = sig;
    }

    /// <inheritdoc />
    public void Dispose() => Host?.UnregisterZone(this);
}
