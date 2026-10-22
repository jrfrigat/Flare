using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// Everything that reacts to the browser rather than to the data: the fluid width measurement, the
// tooltip's place in the page, and the zoom/pan window with its pointer, wheel and toolbar gestures.
public partial class FlareChart
{
    [Inject] private IBrowserViewportService Viewport { get; set; } = default!;
    [Inject] private IOverlayJsService Overlay { get; set; } = default!;
    [Inject] private IChartMotionJsService Motion { get; set; } = default!;

    private ElementReference _plotRef;
    private ElementReference _tooltipRef;
    private IAsyncDisposable? _sizeSubscription;
    private int? _measuredWidth;
    private bool _observing;
    private bool _motionObserving;
    private long _drawingRevision;

    // Distance from the pointer to the bubble. A number rather than a token because it is an input to
    // the placement engine, which works in pixels - the same reason ICollisionService.Offset is one.
    private const int TooltipGap = 8;

    // The shared anchored layer keeps the bubble beside the pointer and inside the viewport.
    private readonly AnchoredLayer _tooltipLayer = new();
    private double _placedX = double.NaN;
    private double _placedY = double.NaN;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await SyncInputAsync();
        await SyncTooltipLayerAsync();
        await SyncMotionAsync();
        if (!(_fluidActive || Zoomable) || _observing) return;
        _observing = true;
        try
        {
            _sizeSubscription = await Viewport.ObserveElementAsync(_plotRef, OnPlotResized,
                new ElementObserveOptions { DebounceMs = 100 });
        }
        catch (InvalidOperationException) { _observing = false; }
        catch (JSDisconnectedException) { _observing = false; }
        catch (JSException) { _observing = false; }
    }

    // Started and stopped where the parameter changes, not once at first render, so a dashboard can turn
    // the motion on with the same switch that starts its timer. Watching costs one call for the life of
    // the chart and nothing per update: the browser reads the geometry it wrote itself, so an update
    // that moves rather than jumps marshals nothing at all.
    private async Task SyncMotionAsync()
    {
        if (AnimateUpdates == _motionObserving) return;
        _motionObserving = AnimateUpdates;
        try
        {
            if (AnimateUpdates)
                await Motion.ObservePlotAsync(_plotRef, Css.Tokens.Motion.DurationEnterMedium, Css.Tokens.Motion.EasingStandard);
            else await Motion.UnobservePlotAsync(_plotRef);
        }
        catch (InvalidOperationException) { _motionObserving = false; }
        catch (JSDisconnectedException) { _motionObserving = false; }
        catch (JSException) { _motionObserving = false; }
    }

    // Client coordinates and AnchorRect both use CSS pixels in the viewport, independent of the SVG
    // viewBox, page scroll and transformed ancestors. The engine handles clipping and edge collisions.
    private async Task SyncTooltipLayerAsync()
    {
        if (_tooltipVisible && !string.IsNullOrEmpty(_tooltipText))
        {
            if (_tooltipLayer.Placed && _placedX == _hoverX && _placedY == _hoverY) return;
            await _tooltipLayer.ForceAsync(Overlay, _plotRef, _tooltipRef,
                new AnchoredPanelOptions
                {
                    Placement = PanelPlacement.Top,
                    Gap = TooltipGap,
                    AnchorRect = new PanelAnchorRect(_hoverX, _hoverY),
                });
            _placedX = _hoverX;
            _placedY = _hoverY;
        }
        else if (_tooltipLayer.Placed)
        {
            await _tooltipLayer.ReleaseAsync(Overlay);
            _placedX = _placedY = double.NaN;
        }
    }

    // Only an integer change matters: the viewBox is written in whole units, so re-rendering on sub-pixel
    // jitter would repaint the whole chart for a drawing that cannot differ.
    private void OnPlotResized(ElementBoundingRect rect)
    {
        var width = (int)Math.Round(rect.Width);
        if (width <= 0 || width == _measuredWidth) return;
        _measuredWidth = width;
        _drawingRevision++;
        StateHasChanged();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await ReleaseInputAsync();
        await _tooltipLayer.ReleaseAsync(Overlay);
        if (_sizeSubscription is not null)
        {
            try { await _sizeSubscription.DisposeAsync(); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _sizeSubscription = null;
        }
        if (_motionObserving)
        {
            try { await Motion.UnobservePlotAsync(_plotRef); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _motionObserving = false;
        }
        await base.DisposeAsync();
    }

    // The zoom domain is in the chart's own horizontal units. Point-style charts (line/area) put data ON
    // the category indices, so their full domain is 0..pts-1; bar-style charts put data in SLOTS around
    // them, so theirs is -0.5..pts-0.5 and a bar still sits centered on its index. Scatter and bubble
    // have no categories at all, so their domain is the X range of the data itself.
    private bool _barDomain => Type is ChartType.Bar or ChartType.StackedBar or ChartType.Combo;
    private bool _valueDomain => Type is ChartType.Scatter or ChartType.Bubble;

    private double _domainFrom => _valueDomain ? _pointXRange.Min : _barDomain ? -0.5 : 0;
    private double _domainTo => _valueDomain ? _pointXRange.Max
        : _barDomain ? Math.Max(1, _pts) - 0.5 : Math.Max(1, _pts) - 1;

    // X extent of the scatter/bubble points, widened when every point shares one X so the span is never 0.
    private long _pointRangeRevision = -1;
    private (double Min, double Max) _cachedPointRange;
    private (double Min, double Max) _pointXRange
    {
        get
        {
            if (_pointRangeRevision == _drawingRevision) return _cachedPointRange;
            _pointRangeRevision = _drawingRevision;
            var xs = ScalePoints().Select(p => p.X).ToList();
            if (xs is not { Count: > 0 }) return _cachedPointRange = (0, 1);
            double min = xs.Min(), max = xs.Max();
            return _cachedPointRange = max > min ? (min, max) : (min, min + 1);
        }
    }

    // Never let a zoom collapse onto a single point: two category slots is the narrowest useful window.
    // A scatter/bubble X range narrower than that cannot fit it, so it gets half its own range instead.
    private double MinZoomSpan
    {
        get
        {
            var full = _domainTo - _domainFrom;
            return _valueDomain && double.IsFinite(full) && full < 2
                ? Math.Max(full / 2, 1e-9)
                : 1.0;
        }
    }

    private ChartZoom? _internalZoom;
    private ChartZoom? _lastBoundZoom;

    /// <summary>The window actually drawn: the bound Zoom, else the user's own, else the full domain.</summary>
    private ChartZoom _view =>
        (Zoom ?? _internalZoom ?? new ChartZoom(_domainFrom, _domainTo)).Clamp(_domainFrom, _domainTo, MinZoomSpan);

    private bool _zoomed => _zoomActive
        && (_view.From > _domainFrom + 1e-9 || _view.To < _domainTo - 1e-9);

    private bool _zoomActive => Zoomable && !_spark && !(Type == ChartType.Bar && Horizontal)
        && (_valueDomain ? Data?.Series.SelectMany(s => s.Points ?? []).Where(IsValidPoint).Take(2).Count() > 1 : _pts > 1)
        && Type is ChartType.Line or ChartType.Area or ChartType.Bar or ChartType.StackedBar
                or ChartType.Combo or ChartType.Scatter or ChartType.Bubble;

    private bool _showZoomToolbar => _zoomActive && ShowZoomToolbar;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _drawingRevision++;
        // Zoom follows the two-way state contract: controlled tracks the parameter, uncontrolled keeps
        // the user's window until the parameter itself moves.
        if (ZoomChanged.HasDelegate)
            _internalZoom = Zoom;
        else if (!Nullable.Equals(Zoom, _lastBoundZoom))
            _internalZoom = Zoom;
        _lastBoundZoom = Zoom;
    }

    private async Task SetZoomAsync(ChartZoom? window)
    {
        // A detail is anchored to viewport coordinates of the old window; after a zoom it points at
        // nothing the reader can still name.
        DismissDetail();
        _internalZoom = window;
        _drawingRevision++;
        if (ZoomChanged.HasDelegate) await ZoomChanged.InvokeAsync(window);
        StateHasChanged();
    }

    private Task ResetZoomAsync() => SetZoomAsync(null);

    // Zooms around the window's own center by a factor: < 1 narrows (in), > 1 widens (out).
    private Task ZoomByAsync(double factor) => ZoomAroundAsync(factor, (_view.From + _view.To) / 2);

    private Task ZoomAroundAsync(double factor, double anchor)
    {
        var span = _view.Span * factor;
        var full = _domainTo - _domainFrom;
        if (span >= full) return ResetZoomAsync();
        // Keep the anchor at the same relative position, which is what makes wheel-zoom feel anchored
        // to the pointer rather than to the middle of the chart.
        var t = _view.Span <= 0 ? 0.5 : (anchor - _view.From) / _view.Span;
        var from = anchor - t * span;
        return SetZoomAsync(new ChartZoom(from, from + span).Clamp(_domainFrom, _domainTo, MinZoomSpan));
    }

    // Drag state. A drag that starts on the plot either selects a range (zoomed out) or pans (zoomed in),
    // decided once at pointer-down so the gesture cannot change meaning halfway through.
    private double? _dragStartX;
    private double _dragCurrentX;
    private bool _dragIsPan;
    private ChartZoom _dragStartView;
    private ChartZoom? _dragStartZoom;

    private bool _selecting => _dragStartX is not null && !_dragIsPan
        && Math.Abs(_dragCurrentX - _dragStartX.Value) > 2;

    private (double X, double Width) _selectionRect
    {
        get
        {
            var a = Math.Clamp(_dragStartX ?? 0, _padL, _padL + _plotW);
            var b = Math.Clamp(_dragCurrentX, _padL, _padL + _plotW);
            return (Math.Min(a, b), Math.Abs(b - a));
        }
    }

    private double IndexOfX(double x) =>
        _view.From + (x - _padL) / Math.Max(1, _plotW) * _view.Span;

    /// <summary>Updates a mouse or pen zoom gesture using SVG viewBox coordinates from the input adapter.</summary>
    /// <param name="phase">The gesture phase: start, move, end, or cancel.</param>
    /// <param name="x">The horizontal position in SVG viewBox units.</param>
    [JSInvokable]
    public Task ChartZoomGesture(string phase, double x) => InvokeAsync(async () =>
    {
        if (!_zoomActive || !double.IsFinite(x)) { _dragStartX = null; return; }
        if (phase == "start")
        {
            _dragStartX = x;
            _dragCurrentX = x;
            _dragIsPan = _zoomed;
            _dragStartView = _view;
            _dragStartZoom = Zoom ?? _internalZoom;
            return;
        }
        if (_dragStartX is not { } start) return;
        if (phase == "cancel")
        {
            _dragStartX = null;
            await SetZoomAsync(_dragStartZoom);
            return;
        }
        if (phase is not ("move" or "end")) return;
        _dragCurrentX = x;
        if (Math.Abs(x - start) > 2) DismissDetail();
        if (_dragIsPan)
        {
            var delta = (x - start) / Math.Max(1, _plotW) * _dragStartView.Span;
            var from = _dragStartView.From - delta;
            if (phase == "end") _dragStartX = null;
            await SetZoomAsync(new ChartZoom(from, from + _dragStartView.Span).Clamp(_domainFrom, _domainTo, MinZoomSpan));
            return;
        }
        // The selection rectangle is its own overlay, so a move repaints it without touching the SVG,
        // the hit zones or the data table; only the end commits a new window and bumps the revision.
        if (phase == "move") { StateHasChanged(); return; }
        _dragStartX = null;
        if (Math.Abs(x - start) <= 2) { StateHasChanged(); return; }
        var a = IndexOfX(Math.Min(start, x));
        var b = IndexOfX(Math.Max(start, x));
        await SetZoomAsync(new ChartZoom(a, b).Clamp(_domainFrom, _domainTo, MinZoomSpan));
    });

    /// <summary>Moves visible details without rebuilding chart geometry. The input adapter coalesces pointer events.</summary>
    /// <param name="x">The pointer's horizontal viewport coordinate in CSS pixels.</param>
    /// <param name="y">The pointer's vertical viewport coordinate in CSS pixels.</param>
    [JSInvokable]
    public Task ChartPointerMoved(double x, double y) => InvokeAsync(async () =>
    {
        if (!_tooltipVisible || !double.IsFinite(x) || !double.IsFinite(y)) return;
        (_hoverX, _hoverY) = (x, y);
        await SyncTooltipLayerAsync();
    });

    private void OnPlotPointerLeave()
    {
        ClearHover();
    }

    /// <summary>Zooms around an SVG position after the input adapter accepts a Ctrl+wheel gesture.</summary>
    /// <param name="delta">The wheel's vertical delta.</param>
    /// <param name="x">The anchor's horizontal position in SVG viewBox units.</param>
    [JSInvokable]
    public Task ChartZoomWheel(double delta, double x) => InvokeAsync(async () =>
    {
        if (!_zoomActive || !double.IsFinite(delta) || !double.IsFinite(x) || delta == 0) return;
        await ZoomAroundAsync(delta > 0 ? 1.25 : 0.8, IndexOfX(x));
    });
}
