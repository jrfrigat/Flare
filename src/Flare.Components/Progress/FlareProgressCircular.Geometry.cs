using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// The ring is the half of progress that needs measuring: its radius is the box less the stroke, and
/// the gap between arcs is a CSS length that only becomes a share of the circumference once the
/// browser has laid the ring out. That is what this observer is for, and why it lives with the
/// circular indicator alone - a linear bar resolves everything the CSS already knows.
/// </summary>
public partial class FlareProgressCircular
{
    [Inject] private IProgressGeometryJsService Geometry { get; set; } = default!;

    private ElementReference _ref;
    private bool _observing;
    private bool _disposed;

    // Order is the contract with flare-progress-geometry.js. Names stay in the token registry.
    private static readonly string[] _measures =
    [
        "--_circ-width",
        Css.Tokens.ProgressField.CircularGap,
    ];

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed || _observing) return;
        _observing = true;
        try
        {
            await Geometry.ObserveAsync(_ref);
            if (_disposed) await Geometry.UnobserveAsync(_ref);
        }
        catch (InvalidOperationException) { _observing = false; }
        catch (JSDisconnectedException) { _observing = false; }
        catch (JSException) { _observing = false; }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_observing)
        {
            try { await Geometry.UnobserveAsync(_ref); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _observing = false;
        }
        await base.DisposeAsync();
    }
}
