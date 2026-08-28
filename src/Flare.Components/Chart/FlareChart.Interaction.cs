using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// Everything that reacts to the browser rather than to the data: the fluid width measurement, and the
// zoom/pan window with its pointer, wheel and toolbar gestures.
public partial class FlareChart
{
    [Inject] private IBrowserViewportService Viewport { get; set; } = default!;

    private ElementReference _plotRef;
    private IAsyncDisposable? _sizeSubscription;
    private int? _measuredWidth;
    private bool _observing;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!(_fluidActive || Zoomable) || _observing) return;
        _observing = true;
        try
        {
            _sizeSubscription = await Viewport.ObserveElementAsync(_plotRef, OnPlotResized,
                new ElementObserveOptions { ThrottleMs = 100 });
        }
        catch (InvalidOperationException) { _observing = false; }
        catch (JSDisconnectedException) { _observing = false; }
        catch (JSException) { _observing = false; }
    }

    // Only an integer change matters: the viewBox is written in whole units, so re-rendering on sub-pixel
    // jitter would repaint the whole chart for a drawing that cannot differ.
    private void OnPlotResized(ElementBoundingRect rect)
    {
        var width = (int)Math.Round(rect.Width);
        if (width <= 0 || width == _measuredWidth) return;
        _measuredWidth = width;
        StateHasChanged();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_sizeSubscription is not null)
        {
            try { await _sizeSubscription.DisposeAsync(); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            _sizeSubscription = null;
        }
        await base.DisposeAsync();
    }

    // ---- Zoom window -------------------------------------------------------------------------------

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
    private (double Min, double Max) _pointXRange
    {
        get
        {
            var xs = Data?.Series.SelectMany(s => s.Points ?? []).Select(p => p.X).ToList();
            if (xs is not { Count: > 0 }) return (0, 1);
            double min = xs.Min(), max = xs.Max();
            return max > min ? (min, max) : (min, min + 1);
        }
    }

    // Never let a zoom collapse onto a single point: two category slots is the narrowest useful window.
    private const double _minZoomSpan = 1.0;

    private ChartZoom? _internalZoom;
    private ChartZoom? _lastBoundZoom;

    /// <summary>The window actually drawn: the bound Zoom, else the user's own, else the full domain.</summary>
    private ChartZoom _view =>
        (Zoom ?? _internalZoom ?? new ChartZoom(_domainFrom, _domainTo)).Clamp(_domainFrom, _domainTo, _minZoomSpan);

    private bool _zoomed => _zoomActive
        && (_view.From > _domainFrom + 1e-9 || _view.To < _domainTo - 1e-9);

    private bool _zoomActive => Zoomable && !_spark && _pts > 1
        && Type is ChartType.Line or ChartType.Area or ChartType.Bar or ChartType.StackedBar
                or ChartType.Combo or ChartType.Scatter or ChartType.Bubble;

    private bool _showZoomToolbar => _zoomActive && ShowZoomToolbar;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
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
        _internalZoom = window;
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
        return SetZoomAsync(new ChartZoom(from, from + span).Clamp(_domainFrom, _domainTo, _minZoomSpan));
    }

    // ---- Pointer gestures --------------------------------------------------------------------------

    // Drag state. A drag that starts on the plot either selects a range (zoomed out) or pans (zoomed in),
    // decided once at pointer-down so the gesture cannot change meaning halfway through.
    private double? _dragStartX;
    private double _dragCurrentX;
    private bool _dragIsPan;
    private ChartZoom _dragStartView;

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

    // Pointer coordinates arrive in CSS pixels relative to the plot element. In fluid mode one viewBox
    // unit is one pixel, so they are already viewBox units; otherwise the drawing is scaled to the
    // element and the offset has to be scaled back by the same ratio.
    private double ToPlotX(MouseEventArgs e)
    {
        var scale = _fluidActive || _measuredWidth is not { } w || w <= 0 ? 1.0 : _svgW / (double)w;
        return e.OffsetX * scale;
    }

    private double IndexOfX(double x) =>
        _view.From + (x - _padL) / Math.Max(1, _plotW) * _view.Span;

    private void OnPlotPointerDown(MouseEventArgs e)
    {
        if (!_zoomActive) return;
        _dragStartX = ToPlotX(e);
        _dragCurrentX = _dragStartX.Value;
        _dragIsPan = _zoomed;
        _dragStartView = _view;
    }

    private void OnPlotPointerMove(MouseEventArgs e)
    {
        if (!_zoomActive || _dragStartX is null) return;
        _dragCurrentX = ToPlotX(e);
        if (!_dragIsPan) { StateHasChanged(); return; }

        var deltaIndex = (_dragCurrentX - _dragStartX.Value) / Math.Max(1, _plotW) * _dragStartView.Span;
        var from = _dragStartView.From - deltaIndex;
        _internalZoom = new ChartZoom(from, from + _dragStartView.Span).Clamp(_domainFrom, _domainTo, _minZoomSpan);
        StateHasChanged();
    }

    private async Task OnPlotPointerUp(MouseEventArgs e)
    {
        if (!_zoomActive || _dragStartX is null) return;
        var start = _dragStartX.Value;
        var end = ToPlotX(e);
        var wasPan = _dragIsPan;
        _dragStartX = null;

        if (wasPan) { await SetZoomAsync(_internalZoom); return; }
        if (Math.Abs(end - start) <= 2) { StateHasChanged(); return; }

        var a = IndexOfX(Math.Min(start, end));
        var b = IndexOfX(Math.Max(start, end));
        await SetZoomAsync(new ChartZoom(a, b).Clamp(_domainFrom, _domainTo, _minZoomSpan));
    }

    private void OnPlotPointerLeave()
    {
        ClearHover();
        if (_dragStartX is null) return;
        _dragStartX = null;
        StateHasChanged();
    }

    // Ctrl is required so an unmodified wheel still scrolls the page - a chart that swallows the scroll
    // is the single most complained-about behaviour in every charting library that does not.
    private Task OnPlotWheel(WheelEventArgs e)
    {
        if (!_zoomActive || !e.CtrlKey) return Task.CompletedTask;
        var anchor = IndexOfX(ToPlotX(e));
        return ZoomAroundAsync(e.DeltaY > 0 ? 1.25 : 0.8, anchor);
    }
}
