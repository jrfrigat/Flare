using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Displays determinate or indeterminate progress as a bar or circular indicator.</summary>
public partial class FlareProgress
{
    // Generic geometry ports. Their public constants and values belong to the theme package that
    // implements a renderer; keeping them out of Flare.Abstractions prevents one theme's extension
    // from becoming a required token for every theme.
    private const string _waveHeightToken = "--flare-progress-wavy-height";
    private const string _waveLengthToken = "--flare-progress-wave-length";
    private const string _indeterminateWaveLengthToken = "--flare-progress-indeterminate-wave-length";
    private const string _waveAmplitudeToken = "--flare-progress-wave-amplitude";
    private const string _segmentedIndeterminateToken = "--flare-progress-linear-indeterminate-duration";
    private const string _ringCountToken = "--flare-progress-ring-waves";
    private const string _ringLengthToken = "--flare-progress-ring-wave-length";
    private const string _ringAmplitudeToken = "--flare-progress-ring-wave-amplitude";

    [Inject] private IProgressGeometryJsService Geometry { get; set; } = default!;

    private ElementReference _circularRef;
    private bool _geometryObserving;
    private bool _geometryDisposed;

    // Order is the contract with flare-progress-geometry.js. Names stay in the token registry.
    private static readonly string[] _circularMeasures =
    [
        "--_circ-width",
        Css.Tokens.ProgressField.CircularGap,
        _ringAmplitudeToken,
        _ringLengthToken,
        _ringCountToken,
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
