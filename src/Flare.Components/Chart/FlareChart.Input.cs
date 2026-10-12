using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

public partial class FlareChart
{
    [Inject] private IChartInputJsService Input { get; set; } = default!;
    private DotNetObjectReference<FlareChart>? _inputRef;
    private bool _inputObserving;
    private string? _activeHitKey;
    private string? _focusedHitKey;
    private double _focusX, _focusY;
    private List<ChartHit> _renderedHits = [];

    private enum HitShape { Rectangle, Path, Circle }
    private sealed record ChartHit(string Key, int Index, string Text, HitShape Shape,
        double X = 0, double Y = 0, double Width = 0, double Height = 0, double Radius = 0, string? Path = null);

    private List<ChartHit> RenderedHits()
    {
        _renderedHits = BuildHitZones();
        if (_tooltipVisible)
        {
            var active = _renderedHits.FirstOrDefault(h => h.Key == _activeHitKey);
            if (active is null) DismissDetail();
            else _tooltipText = active.Text;
        }
        return _renderedHits;
    }

    private List<ChartHit> BuildHitZones()
    {
        var hits = new List<ChartHit>();
        if (_spark || Data is null) return hits;
        if (Type is ChartType.Line or ChartType.Area or ChartType.Bar or ChartType.StackedBar or ChartType.Combo)
        {
            foreach (var category in _categories)
            {
                var text = CategoryDescription(category.Index);
                if (text is null) continue;
                bool horizontal = Horizontal && Type == ChartType.Bar;
                double rowHeight = _plotH / (double)Math.Max(1, _pts);
                hits.Add(new($"category:{category.Index}", category.Index, text, HitShape.Rectangle,
                    horizontal ? _padL : category.XLeft,
                    horizontal ? _padT + category.Index * rowHeight : _padT,
                    horizontal ? _plotW : category.Width, horizontal ? rowHeight : _plotH));
            }
        }
        else if (Type is ChartType.Pie or ChartType.Donut)
        {
            foreach (var slice in _slices)
                hits.Add(new($"slice:{slice.Index}", slice.Index,
                    string.Create(_inv, $"{Data.Series[slice.Index].Label}: {slice.Percent:F1}%"), HitShape.Path, Path: slice.Path));
        }
        return hits;
    }

    private string? CategoryDescription(int index)
    {
        if (Data is null) return null;
        var present = Data.Series.Where((s, i) => !IsHidden(i) && index < s.Values.Count && !IsGap(s.Values[index])).ToList();
        if (present.Count == 0) return null;
        var label = Data.Labels is { } labels && index < labels.Count ? labels[index] : index.ToString(_inv);
        return $"{label}\n{string.Join("\n", present.Select(s => string.Create(_inv, $"{s.Label}: {s.Values[index]:G4}")))}";
    }

    private void ShowHit(ChartHit hit, MouseEventArgs e)
    {
        _activeHitKey = hit.Key;
        _tooltipText = hit.Text;
        SetHoverPosition(e);
        _tooltipVisible = true;
    }

    private async Task ActivateHitAsync(ChartHit hit, MouseEventArgs e)
    {
        ShowHit(hit, e);
        await HandlePointClick(hit.Index);
    }

    /// <summary>Receives a focused hit's viewport anchor from the chart input adapter.</summary>
    /// <param name="key">The rendered hit's stable identity.</param>
    /// <param name="x">The anchor's horizontal viewport coordinate in CSS pixels.</param>
    /// <param name="y">The anchor's vertical viewport coordinate in CSS pixels.</param>
    [JSInvokable]
    public Task ChartHitFocused(string key, double x, double y) => InvokeAsync(() =>
    {
        var hit = _renderedHits.FirstOrDefault(h => h.Key == key);
        if (hit is null || !double.IsFinite(x) || !double.IsFinite(y)) return;
        _focusedHitKey = key;
        (_focusX, _focusY) = (x, y);
        ShowHit(hit, new MouseEventArgs { ClientX = x, ClientY = y });
        StateHasChanged();
    });

    /// <summary>Dismisses chart details when keyboard focus leaves or Escape is pressed.</summary>
    [JSInvokable]
    public Task ChartDetailDismissed() => InvokeAsync(() => { DismissDetail(); StateHasChanged(); });

    private void DismissDetail() { _focusedHitKey = null; _tooltipVisible = false; }

    private void ClearHover()
    {
        var focused = _renderedHits.FirstOrDefault(h => h.Key == _focusedHitKey);
        if (focused is null) _tooltipVisible = false;
        else ShowHit(focused, new MouseEventArgs { ClientX = _focusX, ClientY = _focusY });
    }

    private async Task SyncInputAsync()
    {
        if (_inputObserving == !_spark) return;
        try
        {
            if (_spark) await Input.UnobservePlotAsync(_plotRef);
            else
            {
                _inputRef ??= DotNetObjectReference.Create(this);
                await Input.ObservePlotAsync(_plotRef, _inputRef);
            }
            _inputObserving = !_spark;
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    private async Task ReleaseInputAsync()
    {
        if (_inputObserving)
        {
            try { await Input.UnobservePlotAsync(_plotRef); }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        _inputRef?.Dispose();
        _inputRef = null;
    }
}
