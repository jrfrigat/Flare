using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Displays determinate or indeterminate progress as a bar or circular indicator.</summary>
public partial class FlareProgress
{
    [Inject] private IProgressGeometryJsService Geometry { get; set; } = default!;

    private ElementReference _circularRef;
    private bool _geometryObserving;
    private bool _geometryDisposed;

    // Order is the contract with flare-progress-geometry.js. Names stay in the token registry.
    private static readonly string[] _circularMeasures =
    [
        "--_circ-width",
        Css.Tokens.ProgressField.CircularGap,
        Css.Tokens.ProgressField.RingWaveAmplitude,
        Css.Tokens.ProgressField.RingWaveLength,
        Css.Tokens.ProgressField.RingWaves,
    ];

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var circular = Variant == ProgressVariant.Circular;
        if (_geometryDisposed || circular == _geometryObserving) return;
        _geometryObserving = circular;
        try
        {
            if (circular)
            {
                await Geometry.ObserveAsync(_circularRef);
                if (_geometryDisposed) await Geometry.UnobserveAsync(_circularRef);
            }
            else await Geometry.UnobserveAsync(_circularRef);
        }
        catch (InvalidOperationException) { _geometryObserving = false; }
        catch (JSDisconnectedException) { _geometryObserving = false; }
        catch (JSException) { _geometryObserving = false; }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _geometryDisposed = true;
        if (_geometryObserving)
        {
            try { await Geometry.UnobserveAsync(_circularRef); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _geometryObserving = false;
        }
        await base.DisposeAsync();
    }
}
