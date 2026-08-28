using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Plot geometry: the horizontal projection, category slots, slice paths, the value scale, and
// hover/tooltip state.
public partial class FlareChart
{
    // THE horizontal projection: category index -> viewBox x. Every cartesian renderer goes through it,
    // which is what makes zooming a change of one window rather than a change in each renderer. Without a
    // zoom the visible window is the whole domain, so this reduces to the plain even spacing.
    private double XOfIndex(double index)
    {
        var span = _view.Span;
        if (span <= 0) return _padL + _plotW / 2.0;
        return _padL + (index - _view.From) / span * _plotW;
    }

    // Width of one category slot in viewBox units - the group width for bars, the point spacing for lines.
    private double SlotWidth => _view.Span <= 0 ? _plotW : _plotW / _view.Span;

    // Data marks are clipped ONLY while a zoom window is in force, and only HORIZONTALLY.
    //
    // Only-when-zoomed: an unzoomed chart draws nothing outside its plot by construction, so clipping it
    // buys nothing and costs the marks that legitimately overhang - the marker circle centred on the plot
    // edge, the bubble at the extreme X.
    //
    // Horizontally-only: zoom moves the X axis and nothing else, so the vertical edges never need
    // cutting - and cutting them removes the value label of a bar that reaches the top of the plot,
    // which sits ABOVE its bar by design.
    private bool _clipping => _zoomed;

    // The group wraps a whole renderer's output in one markup blob rather than per mark: an unbalanced
    // tag handed to AddMarkupContent would be repaired by the parser and the clip would land elsewhere.
    private string Clipped(string markup) =>
        markup.Length == 0 || !_clipping ? markup : $"<g clip-path=\"url(#{_uid}-clip)\">{markup}</g>";

    // The clip rectangle itself, emitted once per chart next to the plot.
    private string ClipPathDef => !_clipping ? "" : string.Create(_inv,
        $"<defs><clipPath id=\"{_uid}-clip\"><rect x=\"{_padL}\" y=\"0\" width=\"{_plotW}\" height=\"{_svgH}\"/></clipPath></defs>");

    // Index range worth drawing: everything outside is clipped away, so computing it costs nothing and
    // skipping it keeps a zoomed chart from building marks nobody can see.
    private (int From, int To) VisibleIndexRange(int pts)
    {
        if (pts <= 0) return (0, -1);
        var from = (int)Math.Floor(_view.From) - 1;
        var to = (int)Math.Ceiling(_view.To) + 1;
        return (Math.Max(0, from), Math.Min(pts - 1, to));
    }

    private IEnumerable<(int Index, double XLeft, double Width, double XCenter)> _categories
    {
        get
        {
            int pts = _pts;
            if (pts <= 0) yield break;
            var (from, to) = VisibleIndexRange(pts);
            double width = SlotWidth;
            for (int i = from; i <= to; i++)
            {
                double center = XOfIndex(i);
                yield return (i, center - width / 2, width, center);
            }
        }
    }

    // Slice geometry (pie/donut) shared by the visual paths and the hit zones.
    private IEnumerable<(int Index, string Path, double Cx, double Cy)> _slices
    {
        get
        {
            if (Data?.Series is not { Count: > 0 } series) yield break;
            bool donut = Type == ChartType.Donut;
            double cx = _svgW / 2.0, cy = _svgH / 2.0;
            double r = Math.Min(cx, cy) - 16;
            double inner = donut ? r * Math.Clamp(DonutRingRatio, 0, 0.95) : 0;
            double total = series.Sum(s => s.Values.FirstOrDefault());
            if (total == 0) yield break;

            double startAngle = -Math.PI / 2;
            for (int i = 0; i < series.Count; i++)
            {
                double val = SafeValue(series[i].Values.FirstOrDefault());
                double sweep = val / total * 2 * Math.PI;
                double endAngle = startAngle + sweep;
                double x1 = cx + r * Math.Cos(startAngle), y1 = cy + r * Math.Sin(startAngle);
                double x2 = cx + r * Math.Cos(endAngle), y2 = cy + r * Math.Sin(endAngle);
                double ix1 = cx + inner * Math.Cos(startAngle), iy1 = cy + inner * Math.Sin(startAngle);
                double ix2 = cx + inner * Math.Cos(endAngle), iy2 = cy + inner * Math.Sin(endAngle);
                int largeArc = sweep > Math.PI ? 1 : 0;
                string path = donut
                    ? string.Create(_inv, $"M {x1:F2} {y1:F2} A {r:F2} {r:F2} 0 {largeArc} 1 {x2:F2} {y2:F2} L {ix2:F2} {iy2:F2} A {inner:F2} {inner:F2} 0 {largeArc} 0 {ix1:F2} {iy1:F2} Z")
                    : string.Create(_inv, $"M {cx:F2} {cy:F2} L {x1:F2} {y1:F2} A {r:F2} {r:F2} 0 {largeArc} 1 {x2:F2} {y2:F2} Z");
                double mid = startAngle + sweep / 2;
                double rMid = donut ? (r + inner) / 2 : r * 0.6;
                yield return (i, path, cx + rMid * Math.Cos(mid), cy + rMid * Math.Sin(mid));
                startAngle = endAngle;
            }
        }
    }

    private (double Min, double Max) _scale()
    {
        var all = Data!.Series.Where((s, i) => !IsHidden(i)).SelectMany(s => s.Values).ToList();
        if (all.Count == 0) return (0, 1);
        double min = all.Min(), max = all.Max();
        if (Type is ChartType.Bar or ChartType.StackedBar) { max = Math.Max(max, 0); min = Math.Min(min, 0); }
        return ApplyBounds(min, max);
    }

    private double _yOf(double v, double min, double max) =>
        _padT + _plotH - (SafeValue(v) - min) / (max - min) * _plotH;

    private void SetHoverCategory(int index, double xCenter)
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var label = (Data?.Labels is { } ls && index < ls.Count) ? ls[index] : index.ToString();
        var parts = series.Where(s => index < s.Values.Count)
            .Select(s => string.Create(_inv, $"{s.Label}: {s.Values[index]:G4}"));
        _tooltipText = $"{label}\n{string.Join("\n", parts)}";

        var (min, max) = _scale();
        double yTop = series.Where(s => index < s.Values.Count)
            .Select(s => _yOf(s.Values[index], min, max))
            .DefaultIfEmpty(_padT).Min();

        _hoverXPct = xCenter / _svgW * 100;
        _hoverYPct = yTop / _svgH * 100;
        _tooltipVisible = true;
    }

    private void SetHoverSlice(int index, double cx, double cy)
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        double total = series.Sum(s => s.Values.FirstOrDefault());
        double pct = total > 0 ? series[index].Values.FirstOrDefault() / total * 100 : 0;
        _tooltipText = string.Create(_inv, $"{series[index].Label}: {pct:F1}%");
        _hoverXPct = cx / _svgW * 100;
        _hoverYPct = cy / _svgH * 100;
        _tooltipVisible = true;
    }

    private void ClearHover() => _tooltipVisible = false;
}
