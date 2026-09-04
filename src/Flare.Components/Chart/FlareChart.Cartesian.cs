using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Cartesian renderers: line/area, bar, stacked bar, scatter/bubble and combo.
public partial class FlareChart
{
    private RenderFragment RenderLine() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var visVals = ScaleValues(series);
        if (visVals.Count == 0) return;
        var axis = ResolveAxis(visVals.Min(), visVals.Max(), _valueExtent);
        double min = axis.Min, max = axis.Max;
        int pts = series.Max(s => s.Values.Count);
        int seq = 0;

        if (_showGrid || _showY)
            builder.AddMarkupContent(seq++, GridLines(axis));
        if (ShowVerticalGrid)
            builder.AddMarkupContent(seq++, VerticalGrid(pts));

        double baseline = _padT + _plotH;
        string vectorEffect = _spark ? " vector-effect=\"non-scaling-stroke\"" : "";
        var marks = new System.Text.StringBuilder();

        for (int si = 0; si < series.Count; si++)
        {
            if (IsHidden(si)) continue;
            var vals = series[si].Values;
            if (vals.Count == 0) continue;
            // A gap keeps its slot in the list and carries NaN through as its Y, so the point still
            // owns its category position and Runs() can tell where the line has to break.
            var pointList = new List<(double X, double Y)>(vals.Count);
            for (int i = 0; i < vals.Count; i++)
            {
                double x = XOfIndex(i);
                double y = IsGap(vals[i])
                    ? double.NaN
                    : _padT + _plotH - (SafeValue(vals[i]) - min) / (max - min) * _plotH;
                pointList.Add((x, y));
            }
            var runs = Runs(pointList);
            if (runs.Count == 0) continue;
            var s = series[si];
            var color = GetColor(si);
            // One subpath per run: each starts with its own M, so concatenating them leaves the holes
            // unstroked instead of drawing a segment across.
            string linePath = string.Concat(runs.Select(r => SeriesSmooth(s) ? SmoothPath(r) : StraightPath(r)));

            // Area fill: a gradient from the series color down to transparent. Colors go through style
            // (var() is not resolved in SVG presentation attributes / stop-color, only in CSS values).
            if (SeriesArea(s))
            {
                string gid = $"{_uid}-a{si}";
                marks.Append(
                    $"<defs><linearGradient id=\"{gid}\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\">" +
                    $"<stop offset=\"0\" style=\"stop-color:{color};stop-opacity:var(--flare-chart-area-opacity)\"/>" +
                    $"<stop offset=\"1\" style=\"stop-color:{color};stop-opacity:0\"/></linearGradient></defs>");
                // Each run closes down to the baseline on its own; one shared close would fill the gap.
                string areaPath = string.Concat(runs.Select(r => string.Create(_inv,
                    $"{(SeriesSmooth(s) ? SmoothPath(r) : StraightPath(r))} L {r[^1].X:F1} {baseline:F1} L {r[0].X:F1} {baseline:F1} Z ")));
                marks.Append($"<path d=\"{areaPath}\" stroke=\"none\" fill=\"url(#{gid})\"/>");
            }

            // The line itself (stroked path).
            var lineStyle = SeriesLineStyle(s);
            marks.Append(
                $"<path class=\"{Css.Classes.Chart.Line}\"{DrawLength(lineStyle)} d=\"{linePath}\" fill=\"none\" style=\"stroke:{color}{DashStyle(lineStyle)}\"{vectorEffect}/>");

            double markerR = PointRadius();
            if (SeriesMarkers(s))
            {
                foreach (var run in runs)
                    foreach (var (x, y) in run)
                        marks.Append(string.Create(_inv, $"<circle cx=\"{x:F1}\" cy=\"{y:F1}\" r=\"{markerR:F2}\" style=\"fill:{color}\"/>"));
            }
            else
            {
                // A value alone between two gaps has no segment to be drawn as, so without a dot it
                // would be present in the data and invisible on the chart.
                foreach (var run in runs.Where(r => r.Count == 1))
                    marks.Append(string.Create(_inv, $"<circle cx=\"{run[0].X:F1}\" cy=\"{run[0].Y:F1}\" r=\"{markerR:F2}\" style=\"fill:{color}\"/>"));
            }
            if (TrendLine)
                marks.Append(TrendLineMarkup([.. runs.SelectMany(r => r)], color));
        }
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Under) + marks.ToString()));
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Over)));
        if (_showX)
            builder.AddMarkupContent(seq++, AxisLabels(min, max, pts));
        builder.AddMarkupContent(seq++, AxisTitles());
    };

    private static string StraightPath(IReadOnlyList<(double X, double Y)> pts)
    {
        if (pts.Count == 0) return "";
        var sb = new System.Text.StringBuilder(string.Create(_inv, $"M {pts[0].X:F1} {pts[0].Y:F1}"));
        for (int i = 1; i < pts.Count; i++)
            sb.Append(string.Create(_inv, $" L {pts[i].X:F1} {pts[i].Y:F1}"));
        return sb.ToString();
    }

    // Monotone cubic (Fritsch-Carlson) -> cubic Bezier for a smooth line through the points.
    // The tangent limiter keeps every segment inside the [y(i), y(i+1)] band, so a curve never
    // dips below a run of zeros or bulges past a peak the data does not contain.
    private static string SmoothPath(IReadOnlyList<(double X, double Y)> pts)
    {
        int n = pts.Count;
        if (n < 3) return StraightPath(pts);

        var m = new double[n];
        m[0] = Secant(pts, 0);
        m[n - 1] = Secant(pts, n - 2);
        for (int i = 1; i < n - 1; i++)
            m[i] = (Secant(pts, i - 1) + Secant(pts, i)) / 2.0;

        for (int i = 0; i < n - 1; i++)
        {
            double d = Secant(pts, i);
            if (d == 0.0) { m[i] = 0.0; m[i + 1] = 0.0; continue; }
            double a = m[i] / d, b = m[i + 1] / d;
            if (a < 0.0) { m[i] = 0.0; a = 0.0; }
            if (b < 0.0) { m[i + 1] = 0.0; b = 0.0; }
            double q = a * a + b * b;
            if (q > 9.0)
            {
                double t = 3.0 / Math.Sqrt(q);
                m[i] = t * a * d;
                m[i + 1] = t * b * d;
            }
        }

        var sb = new System.Text.StringBuilder(string.Create(_inv, $"M {pts[0].X:F1} {pts[0].Y:F1}"));
        for (int i = 0; i < n - 1; i++)
        {
            var p1 = pts[i];
            var p2 = pts[i + 1];
            double h = (p2.X - p1.X) / 3.0;
            sb.Append(string.Create(_inv,
                $" C {p1.X + h:F1} {p1.Y + m[i] * h:F1} {p2.X - h:F1} {p2.Y - m[i + 1] * h:F1} {p2.X:F1} {p2.Y:F1}"));
        }
        return sb.ToString();
    }

    // Slope of the segment starting at index i; a zero-width segment has no slope.
    private static double Secant(IReadOnlyList<(double X, double Y)> pts, int i)
    {
        double dx = pts[i + 1].X - pts[i].X;
        return dx == 0.0 ? 0.0 : (pts[i + 1].Y - pts[i].Y) / dx;
    }

    private RenderFragment RenderBar() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var visVals = ScaleValues(series);
        if (visVals.Count == 0) return;
        var axis = ResolveAxis(Math.Min(visVals.Min(), 0), Math.Max(visVals.Max(), 0), _valueExtent);
        double min = axis.Min, max = axis.Max;
        int pts = series.Max(s => s.Values.Count);
        int seq = 0;
        double barR = BarRadius();

        if (Horizontal)
        {
            if (_showGrid || _showY)
            {
                var g = new System.Text.StringBuilder();
                int steps = Math.Max(1, axis.Lines - 1);
                int decimals = AxisDecimals(min, (max - min) / steps);
                for (int t = 0; t <= steps; t++)
                {
                    double v = min + (max - min) * t / steps;
                    double gx = _padL + (v - min) / (max - min) * _plotW;
                    if (_showGrid) g.Append(string.Create(_inv, $"<line x1=\"{gx:F1}\" y1=\"{_padT}\" x2=\"{gx:F1}\" y2=\"{_padT + _plotH}\" style=\"{_gridStyle}\"/>"));
                    if (_showY) g.Append(string.Create(_inv, $"<text x=\"{gx:F1}\" y=\"{_padT + _plotH + 14:F1}\" text-anchor=\"middle\" style=\"{_labelStyle}\">{FmtY(v, decimals)}</text>"));
                }
                builder.AddMarkupContent(seq++, g.ToString());
            }
            double groupH = _plotH / (double)pts;
            // Slots, not series indices: under FitVisible a hidden series gives its slot up and the
            // group closes over it, under FitAll it keeps it and the gap stays. Positioning by the
            // absolute index would always leave the gap, which is what the combo renderer already
            // disagreed with.
            var slots = SlotIndices(series.Count);
            double slotH = groupH / (slots.Count + 1);
            double zeroX = _padL + (0 - min) / (max - min) * _plotW;
            for (int slot = 0; slot < slots.Count; slot++)
            {
                int si = slots[slot];
                if (IsHidden(si)) continue;
                var vals = series[si].Values; var color = GetColor(si);
                for (int i = 0; i < vals.Count; i++)
                {
                    // A gap draws nothing: the category keeps its slot, this series just has no bar in it.
                    if (IsGap(vals[i])) continue;
                    double sv = SafeValue(vals[i]);
                    double y = _padT + i * groupH + slot * slotH + slotH * 0.25;
                    double w = Math.Abs(sv / (max - min) * _plotW);
                    double x = sv >= 0 ? zeroX : zeroX - w;
                    double bh = slotH * BarWidthRatio;
                    builder.AddMarkupContent(seq++, string.Create(_inv, $"<rect x=\"{x:F1}\" y=\"{y:F1}\" width=\"{w:F1}\" height=\"{bh:F1}\" rx=\"{barR:F2}\" style=\"fill:{color}\"/>"));
                    if (ShowValues) builder.AddMarkupContent(seq++, string.Create(_inv, $"<text x=\"{x + w + 3:F1}\" y=\"{y + bh / 2 + 3:F1}\" style=\"{_valueStyle}\">{sv:G3}</text>"));
                }
            }
            if (_showX && Data?.Labels is { Count: > 0 } labels)
            {
                var t = new System.Text.StringBuilder();
                for (int i = 0; i < pts && i < labels.Count; i++)
                {
                    double y = _padT + i * groupH + groupH / 2 + 3;
                    t.Append(string.Create(_inv, $"<text x=\"{_padL - 4:F1}\" y=\"{y:F1}\" text-anchor=\"end\" style=\"{_labelStyle}\">{labels[i]}</text>"));
                }
                builder.AddMarkupContent(seq++, t.ToString());
            }
            builder.AddMarkupContent(seq++, AxisTitles());
            return;
        }

        double groupW = SlotWidth;
        var barSlots = SlotIndices(series.Count);
        double barW = groupW / (barSlots.Count + 1);
        if (_showGrid || _showY)
            builder.AddMarkupContent(seq++, GridLines(axis));
        if (ShowVerticalGrid)
            builder.AddMarkupContent(seq++, VerticalGrid(pts));

        var marks = new System.Text.StringBuilder();
        for (int slot = 0; slot < barSlots.Count; slot++)
        {
            int si = barSlots[slot];
            if (IsHidden(si)) continue;
            var vals = series[si].Values;
            var color = GetColor(si);
            for (int i = 0; i < vals.Count; i++)
            {
                if (IsGap(vals[i])) continue;
                double sv = SafeValue(vals[i]);
                double x = XOfIndex(i) - groupW / 2 + slot * barW + barW * 0.25;
                double zeroY = _padT + _plotH - (0 - min) / (max - min) * _plotH;
                double barH = Math.Abs(sv / (max - min) * _plotH);
                double y = sv >= 0 ? zeroY - barH : zeroY;
                double bw = barW * BarWidthRatio;
                marks.Append(
                    string.Create(_inv, $"<rect class=\"{Css.Classes.Chart.Bar}\" x=\"{x:F1}\" y=\"{y:F1}\" width=\"{bw:F1}\" height=\"{barH:F1}\" rx=\"{barR:F2}\" style=\"fill:{color}\"/>"));
                if (ShowValues)
                    marks.Append(string.Create(_inv, $"<text x=\"{x + bw / 2:F1}\" y=\"{(sv >= 0 ? y - 3 : y + barH + 9):F1}\" text-anchor=\"middle\" style=\"{_valueStyle}\">{sv:G3}</text>"));
            }
        }
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Under) + marks.ToString()));
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Over)));
        if (_showX)
            builder.AddMarkupContent(seq++, AxisLabels(min, max, pts));
        builder.AddMarkupContent(seq++, AxisTitles());
    };


    private RenderFragment RenderStackedBar() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        int pts = series.Max(s => s.Values.Count);
        if (pts == 0) return;
        // Stacked totals per category (positive stacking; hidden series excluded).
        double max = 0;
        for (int i = 0; i < pts; i++)
        {
            double sum = 0;
            for (int si = 0; si < series.Count; si++)
                // Under FitAll a hidden band still counts towards the tallest stack, so the axis keeps
                // room for it and the stacks that remain do not grow into the space it freed.
                if (CountsTowardsScale(si) && i < series[si].Values.Count && !IsGap(series[si].Values[i]))
                    sum += Math.Max(0, SafeValue(series[si].Values[i]));
            max = Math.Max(max, sum);
        }
        if (max <= 0) max = 1;
        // The stack grows from zero, so the lower bound is not the caller's to move: YMin is skipped here
        // and only the top is rounded.
        var axis = ResolveAxis(0, max, _plotH, honorMin: false);
        max = axis.Max;
        int seq = 0;
        double barR = BarRadius();
        if (_showGrid || _showY) builder.AddMarkupContent(seq++, GridLines(axis));
        if (ShowVerticalGrid) builder.AddMarkupContent(seq++, VerticalGrid(pts));

        double groupW = SlotWidth;
        double barW = groupW * 0.6 * BarWidthRatio;
        var marks = new System.Text.StringBuilder();
        for (int i = 0; i < pts; i++)
        {
            double x = XOfIndex(i) - barW / 2;
            double yCursor = _padT + _plotH;
            for (int si = 0; si < series.Count; si++)
            {
                // A gap contributes no band, and the stack closes over it: the series above keeps its
                // place in the order but sits directly on the one below.
                if (IsHidden(si) || i >= series[si].Values.Count || IsGap(series[si].Values[i])) continue;
                double v = Math.Max(0, SafeValue(series[si].Values[i]));
                double h = v / max * _plotH;
                yCursor -= h;
                marks.Append(string.Create(_inv, $"<rect class=\"{Css.Classes.Chart.Bar}\" x=\"{x:F1}\" y=\"{yCursor:F1}\" width=\"{barW:F1}\" height=\"{h:F1}\" rx=\"{barR:F2}\" style=\"fill:{GetColor(si)}\"/>"));
                if (ShowValues && h >= 12)
                    marks.Append(string.Create(_inv, $"<text x=\"{x + barW / 2:F1}\" y=\"{yCursor + h / 2 + 3:F1}\" text-anchor=\"middle\" style=\"{_valueOnFillStyle}\">{v:G3}</text>"));
            }
        }
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(0, max, pts, ChartAnnotationLayer.Under) + marks.ToString()));
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(0, max, pts, ChartAnnotationLayer.Over)));
        if (_showX) builder.AddMarkupContent(seq++, AxisLabels(0, max, pts));
        builder.AddMarkupContent(seq++, AxisTitles());
    };

    private RenderFragment RenderScatter() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var allPts = series.Where(s => s.Points is { Count: > 0 }).SelectMany(s => s.Points!).ToList();
        if (allPts.Count == 0) return;
        double xMin = allPts.Min(p => p.X), xMax = allPts.Max(p => p.X);
        double yMin = allPts.Min(p => p.Y), yMax = allPts.Max(p => p.Y);
        if (xMax == xMin) xMax = xMin + 1;
        if (yMax == yMin) yMax = yMin + 1;
        var axis = ResolveAxis(yMin, yMax, _plotH);
        yMin = axis.Min; yMax = axis.Max;
        int seq = 0;
        if (_showGrid || _showY) builder.AddMarkupContent(seq++, GridLines(axis));

        // Bubble: map the largest R weight onto the theme's bubble-radius range, so the biggest bubble is
        // the size the theme calls big. sqrt keeps the AREA proportional to the weight - a bubble scaled by
        // radius over-reads by the square, which is the classic bubble-chart lie.
        double rWeightMax = _bubble ? Math.Max(1e-9, allPts.Max(p => p.R)) : 0;
        double ptR = PointRadius();
        double bubbleMin = BubbleMinRadius();
        double bubbleSpan = Math.Max(0, BubbleMaxRadius() - bubbleMin);

        for (int si = 0; si < series.Count; si++)
        {
            if (IsHidden(si)) continue;
            var pointsList = series[si].Points;
            if (pointsList is null) continue;
            var color = GetColor(si);
            var pixels = new List<(double X, double Y)>(pointsList.Count);
            var sb = new System.Text.StringBuilder();
            foreach (var p in pointsList)
            {
                double x = XOfIndex(p.X);
                double y = _padT + _plotH - (p.Y - yMin) / (yMax - yMin) * _plotH;
                pixels.Add((x, y));
                double r = _bubble ? bubbleMin + bubbleSpan * Math.Sqrt(p.R / rWeightMax) : ptR;
                sb.Append(string.Create(_inv, $"<circle cx=\"{x:F1}\" cy=\"{y:F1}\" r=\"{r:F1}\" style=\"fill:{color};opacity:var(--flare-chart-point-opacity)\"/>"));
            }
            if (TrendLine) sb.Append(TrendLineMarkup(pixels, color));
            builder.AddMarkupContent(seq++, Clipped(sb.ToString()));
        }
        if (_showX)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i <= 4; i++)
            {
                double xv = _view.From + _view.Span * i / 4;
                double x = XOfIndex(xv);
                double y = _padT + _plotH + 14;
                sb.Append(string.Create(_inv, $"<text x=\"{x:F1}\" y=\"{y:F1}\" text-anchor=\"middle\" style=\"{_labelStyle}\">{xv:G3}</text>"));
            }
            builder.AddMarkupContent(seq++, sb.ToString());
        }
    };


    private RenderFragment RenderCombo() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var vis = Enumerable.Range(0, series.Count).Where(i => !IsHidden(i)).ToList();
        var visVals = ScaleValues(series);
        if (visVals.Count == 0) return;
        var axis = ResolveAxis(Math.Min(visVals.Min(), 0), Math.Max(visVals.Max(), 0), _valueExtent);
        double min = axis.Min, max = axis.Max;
        int pts = series.Max(s => s.Values.Count);
        int seq = 0;
        double barR = BarRadius();
        if (_showGrid || _showY) builder.AddMarkupContent(seq++, GridLines(axis));
        if (ShowVerticalGrid) builder.AddMarkupContent(seq++, VerticalGrid(pts));

        double groupW = SlotWidth;
        // Slots are owned by the series that count towards the plot, so the position of a bar is a
        // lookup rather than a running counter - which is also what makes it agree with the grouped
        // bar chart in both modes.
        var barIdx = SlotIndices(series.Count)
            .Where(i => (series[i].Kind ?? ChartSeriesKind.Bar) == ChartSeriesKind.Bar).ToList();
        double barW = barIdx.Count > 0 ? groupW / (barIdx.Count + 1) : 0;
        double LineX(int i) => XOfIndex(i);
        double baseline = _padT + _plotH;
        var marks = new System.Text.StringBuilder();

        foreach (int si in vis)
        {
            var kind = series[si].Kind ?? ChartSeriesKind.Bar;
            var vals = series[si].Values;
            var color = GetColor(si);
            if (kind == ChartSeriesKind.Bar)
            {
                int barSlot = barIdx.IndexOf(si);
                for (int i = 0; i < vals.Count; i++)
                {
                    if (IsGap(vals[i])) continue;
                    double sv = SafeValue(vals[i]);
                    double x = XOfIndex(i) - groupW / 2 + barSlot * barW + barW * 0.25;
                    double zeroY = baseline - (0 - min) / (max - min) * _plotH;
                    double barH = Math.Abs(sv / (max - min) * _plotH);
                    double y = sv >= 0 ? zeroY - barH : zeroY;
                    marks.Append(string.Create(_inv, $"<rect class=\"{Css.Classes.Chart.Bar}\" x=\"{x:F1}\" y=\"{y:F1}\" width=\"{barW * BarWidthRatio:F1}\" height=\"{barH:F1}\" rx=\"{barR:F2}\" style=\"fill:{color}\"/>"));
                }
            }
            else
            {
                var pl = new List<(double X, double Y)>(vals.Count);
                for (int i = 0; i < vals.Count; i++)
                    pl.Add((LineX(i), IsGap(vals[i])
                        ? double.NaN
                        : baseline - (SafeValue(vals[i]) - min) / (max - min) * _plotH));
                var comboRuns = Runs(pl);
                if (comboRuns.Count == 0) continue;
                bool comboSmooth = SeriesSmooth(series[si]);
                string linePath = string.Concat(comboRuns.Select(r => comboSmooth ? SmoothPath(r) : StraightPath(r)));
                if (kind == ChartSeriesKind.Area)
                {
                    string gid = $"{_uid}-c{si}";
                    marks.Append(
                        $"<defs><linearGradient id=\"{gid}\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0\" style=\"stop-color:{color};stop-opacity:var(--flare-chart-area-opacity)\"/><stop offset=\"1\" style=\"stop-color:{color};stop-opacity:0\"/></linearGradient></defs>");
                    string comboArea = string.Concat(comboRuns.Select(r => string.Create(_inv,
                        $"{(comboSmooth ? SmoothPath(r) : StraightPath(r))} L {r[^1].X:F1} {baseline:F1} L {r[0].X:F1} {baseline:F1} Z ")));
                    marks.Append($"<path d=\"{comboArea}\" stroke=\"none\" fill=\"url(#{gid})\"/>");
                }
                var comboStyle = SeriesLineStyle(series[si]);
                marks.Append($"<path class=\"{Css.Classes.Chart.Line}\"{DrawLength(comboStyle)} d=\"{linePath}\" fill=\"none\" style=\"stroke:{color}{DashStyle(comboStyle)}\"/>");
                // Same reason as the line chart: an isolated point has no segment of its own to show.
                foreach (var run in comboRuns.Where(r => r.Count == 1))
                    marks.Append(string.Create(_inv, $"<circle cx=\"{run[0].X:F1}\" cy=\"{run[0].Y:F1}\" r=\"{PointRadius():F2}\" style=\"fill:{color}\"/>"));
            }
        }
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Under) + marks.ToString()));
        builder.AddMarkupContent(seq++, Clipped(AnnotationsMarkup(min, max, pts, ChartAnnotationLayer.Over)));
        if (_showX) builder.AddMarkupContent(seq++, AxisLabels(min, max, pts));
        builder.AddMarkupContent(seq++, AxisTitles());
    };

    // Linear-regression trend line over pixel-space points (linearity is preserved by the pixel transform).
}
