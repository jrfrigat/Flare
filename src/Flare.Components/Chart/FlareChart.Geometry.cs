using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Plot geometry: the horizontal projection, category slots, slice paths, the value scale, and
// hover/tooltip state.
public partial class FlareChart
{
    private List<int> VisibleSeriesIndices(int count) =>
        [.. Enumerable.Range(0, count).Where(i => !IsHidden(i))];

    private double VisibleSliceTotal => Data?.Series.Where((_, i) => !IsHidden(i))
        .Sum(s => SafeValue(s.Values.FirstOrDefault())) ?? 0;

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
            double total = VisibleSliceTotal;
            if (total == 0) yield break;

            double startAngle = ArcGeometry.Top;
            for (int i = 0; i < series.Count; i++)
            {
                if (IsHidden(i)) continue;
                double val = SafeValue(series[i].Values.FirstOrDefault());
                double sweep = val / total * 2 * Math.PI;
                double endAngle = startAngle + sweep;
                string path = ArcGeometry.Wedge(cx, cy, r, inner, startAngle, endAngle);
                double mid = startAngle + sweep / 2;
                double rMid = donut ? (r + inner) / 2 : r * 0.6;
                var (lx, ly) = ArcGeometry.Point(cx, cy, rMid, mid);
                yield return (i, path, lx, ly);
                startAngle = endAngle;
            }
        }
    }

    private (double Min, double Max) _scale()
    {
        var all = ScaleValues(Data!.Series);
        if (all.Count == 0) return (0, 1);
        double min = all.Min(), max = all.Max();
        if (Type is ChartType.Bar or ChartType.StackedBar) { max = Math.Max(max, 0); min = Math.Min(min, 0); }
        return ApplyBounds(min, max);
    }

    private double _yOf(double v, double min, double max) =>
        _padT + _plotH - (SafeValue(v) - min) / (max - min) * _plotH;

    private void SetHoverCategory(int index, MouseEventArgs e)
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var label = (Data?.Labels is { } ls && index < ls.Count) ? ls[index] : index.ToString();
        // A series with no value here is left out of the tooltip entirely rather than reported as 0.
        var present = series.Where((s, i) => !IsHidden(i) && index < s.Values.Count && !IsGap(s.Values[index])).ToList();
        if (present.Count == 0) { ClearHover(); return; }
        var parts = present.Select(s => string.Create(_inv, $"{s.Label}: {s.Values[index]:G4}"));
        _tooltipText = $"{label}\n{string.Join("\n", parts)}";

        SetHoverPosition(e);
        _tooltipVisible = true;
    }

    private void SetHoverSlice(int index, MouseEventArgs e)
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        if (IsHidden(index)) { ClearHover(); return; }
        double total = VisibleSliceTotal;
        double pct = total > 0 ? SafeValue(series[index].Values.FirstOrDefault()) / total * 100 : 0;
        _tooltipText = string.Create(_inv, $"{series[index].Label}: {pct:F1}%");
        SetHoverPosition(e);
        _tooltipVisible = true;
    }

    private void SetHoverPosition(MouseEventArgs e)
    {
        _hoverX = e.ClientX;
        _hoverY = e.ClientY;
    }

    private void ClearHover() => _tooltipVisible = false;
    /// <summary>Clamps NaN/Infinity data values to 0 to prevent SVG coordinate corruption. Gaps are
    /// recognized before this point; anything reaching it is a value the renderer decided to draw.</summary>
    private static double SafeValue(double v) =>
        double.IsNaN(v) || double.IsInfinity(v) ? 0.0 : v;

    /// <summary>A missing point: the series has no value in this category, which is not a value of zero.</summary>
    private static bool IsGap(double v) => !double.IsFinite(v);

    // Values that take part in the axis range. Gaps are absent, so they must neither stretch the scale
    // nor poison it - Enumerable.Min over doubles returns NaN as soon as one appears in the sequence.
    private static List<double> Plotted(IEnumerable<double> values) =>
        values.Where(v => !IsGap(v)).ToList();

    // Splits a point list into the runs of consecutive present points. A gap ends the run it follows,
    // so each run becomes its own subpath and the line breaks instead of crossing the hole.
    private static List<List<(double X, double Y)>> Runs(IReadOnlyList<(double X, double Y)> pts)
    {
        var runs = new List<List<(double X, double Y)>>();
        List<(double X, double Y)>? run = null;
        foreach (var p in pts)
        {
            if (IsGap(p.Y)) { run = null; continue; }
            if (run is null) runs.Add(run = []);
            run.Add(p);
        }
        return runs;
    }

    private int _pts => Data?.Series is { Count: > 0 } s ? s.Max(x => x.Values.Count) : 0;
}
