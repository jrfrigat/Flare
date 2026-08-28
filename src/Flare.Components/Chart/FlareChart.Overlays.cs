using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Chrome drawn over or beside the plot: legend, trend line, annotations, grid, axes and the a11y data table.
public partial class FlareChart
{
    private RenderFragment RenderLegend() => builder =>
    {
        var series = Data!.Series;
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", Css.Classes.Chart.Legend);
        for (int i = 0; i < series.Count; i++)
        {
            int idx = i;
            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", IsHidden(idx)
                ? $"{Css.Classes.Chart.LegendItem} {Css.Classes.Chart.LegendItemOff}"
                : Css.Classes.Chart.LegendItem);
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => ToggleSeries(idx)));
            builder.OpenElement(5, "span");
            builder.AddAttribute(6, "class", Css.Classes.Chart.LegendDot);
            builder.AddAttribute(7, "style", $"--flare-chart-dot:{GetColor(idx)}");
            builder.CloseElement();
            builder.OpenElement(8, "span");
            builder.AddContent(9, series[idx].Label);
            builder.CloseElement();
            builder.CloseElement();
        }
        builder.CloseElement();
    };

    // Category hit zones (line/bar), in viewBox units.

    private string TrendLineMarkup(IReadOnlyList<(double X, double Y)> pts, string color)
    {
        if (pts.Count < 2) return "";
        double n = pts.Count, sx = 0, sy = 0, sxx = 0, sxy = 0;
        foreach (var (x, y) in pts) { sx += x; sy += y; sxx += x * x; sxy += x * y; }
        double denom = n * sxx - sx * sx;
        if (Math.Abs(denom) < 1e-9) return "";
        double b = (n * sxy - sx * sy) / denom;
        double a = (sy - b * sx) / n;
        double x1 = pts.Min(p => p.X), x2 = pts.Max(p => p.X);
        return string.Create(_inv, $"<line x1=\"{x1:F1}\" y1=\"{a + b * x1:F1}\" x2=\"{x2:F1}\" y2=\"{a + b * x2:F1}\" style=\"stroke:{color};stroke-width:var(--flare-chart-trend-width);stroke-dasharray:var(--flare-chart-trend-dash);opacity:var(--flare-chart-trend-opacity)\"/>");
    }

    // Overlays drawn in DATA coordinates: thresholds and bands on either axis, free segments, arrows and
    // labelled points. X goes through the same projection as the data, so an annotation tracks a zoom.
    private string AnnotationsMarkup(double min, double max, int pts)
    {
        if (Annotations is not { Count: > 0 } anns) return "";
        var sb = new System.Text.StringBuilder();
        double YOf(double v) => _padT + _plotH - (v - min) / (max - min) * _plotH;
        double right = _padL + _plotW;
        double bottom = _padT + _plotH;
        double arrow = ReadTokenNum(Css.Tokens.Chart.AnnotationArrowSize, 7);
        double pointR = ReadTokenNum(Css.Tokens.Chart.AnnotationPointRadius, 3.5);
        var arrowMarkers = new Dictionary<string, string>();

        foreach (var a in anns)
        {
            var color = a.Color.CssValue ?? "var(--flare-chart-annotation-color)";
            var stroke = $"stroke:{color};stroke-width:var(--flare-chart-annotation-width);stroke-dasharray:"
                + (a.LineStyle is { } ls ? DashOf(ls) : "var(--flare-chart-annotation-dash)");
            var textStyle = $"fill:{color};font-size:var(--flare-chart-label-size)";

            switch (a.Kind)
            {
                case ChartAnnotationKind.HorizontalLine:
                {
                    double hy = YOf(a.Y);
                    sb.Append(string.Create(_inv, $"<line x1=\"{_padL}\" y1=\"{hy:F1}\" x2=\"{right:F1}\" y2=\"{hy:F1}\" style=\"{stroke}\"/>"));
                    AppendLabel(sb, a, a.LabelPosition == ChartAnnotationLabelPosition.Start ? _padL + 3 : right,
                        hy - 3, a.LabelPosition == ChartAnnotationLabelPosition.Start ? "start" : "end", textStyle);
                    break;
                }
                case ChartAnnotationKind.HorizontalBand:
                {
                    double b1 = YOf(a.Y), b2 = YOf(a.Y2);
                    double top = Math.Min(b1, b2), bh = Math.Abs(b2 - b1);
                    sb.Append(string.Create(_inv, $"<rect x=\"{_padL}\" y=\"{top:F1}\" width=\"{_plotW:F1}\" height=\"{bh:F1}\" style=\"fill:{color};fill-opacity:var(--flare-chart-annotation-band-opacity)\"/>"));
                    AppendLabel(sb, a, right, top - 3, "end", textStyle);
                    break;
                }
                case ChartAnnotationKind.VerticalLine:
                {
                    double vx = XOfIndex(a.X);
                    sb.Append(string.Create(_inv, $"<line x1=\"{vx:F1}\" y1=\"{_padT}\" x2=\"{vx:F1}\" y2=\"{bottom:F1}\" style=\"{stroke}\"/>"));
                    AppendLabel(sb, a, vx + 3, _padT + 10, "start", textStyle);
                    break;
                }
                case ChartAnnotationKind.VerticalBand:
                {
                    double v1 = XOfIndex(a.X), v2 = XOfIndex(a.X2);
                    double left = Math.Min(v1, v2), bw = Math.Abs(v2 - v1);
                    sb.Append(string.Create(_inv, $"<rect x=\"{left:F1}\" y=\"{_padT}\" width=\"{bw:F1}\" height=\"{_plotH:F1}\" style=\"fill:{color};fill-opacity:var(--flare-chart-annotation-band-opacity)\"/>"));
                    AppendLabel(sb, a, left + 3, _padT + 10, "start", textStyle);
                    break;
                }
                case ChartAnnotationKind.Segment:
                case ChartAnnotationKind.Arrow:
                {
                    double x1 = XOfIndex(a.X), y1 = YOf(a.Y), x2 = XOfIndex(a.X2), y2 = YOf(a.Y2);
                    string marker = "";
                    if (a.Kind == ChartAnnotationKind.Arrow)
                    {
                        // One marker def per color: a marker inherits nothing from the line that uses it,
                        // so the head has to carry the color itself, and re-emitting the same def per
                        // annotation would multiply ids inside one document.
                        if (!arrowMarkers.TryGetValue(color, out var id))
                        {
                            id = $"{_uid}-arw{arrowMarkers.Count}";
                            arrowMarkers[color] = id;
                            sb.Append(string.Create(_inv,
                                $"<defs><marker id=\"{id}\" viewBox=\"0 0 10 10\" refX=\"9\" refY=\"5\" markerWidth=\"{arrow:F1}\" markerHeight=\"{arrow:F1}\" orient=\"auto-start-reverse\">"
                                + $"<path d=\"M 0 0 L 10 5 L 0 10 z\" style=\"fill:{color}\"/></marker></defs>"));
                        }
                        marker = $" marker-end=\"url(#{id})\"";
                    }
                    sb.Append(string.Create(_inv, $"<line x1=\"{x1:F1}\" y1=\"{y1:F1}\" x2=\"{x2:F1}\" y2=\"{y2:F1}\" style=\"{stroke}\"{marker}/>"));
                    AppendLabel(sb, a, x2 + 4, y2 - 4, "start", textStyle);
                    break;
                }
                case ChartAnnotationKind.Point:
                {
                    double px = XOfIndex(a.X), py = YOf(a.Y);
                    sb.Append(string.Create(_inv, $"<circle cx=\"{px:F1}\" cy=\"{py:F1}\" r=\"{pointR:F1}\" style=\"fill:{color}\"/>"));
                    AppendLabel(sb, a, px, py - pointR - 4, "middle", textStyle);
                    break;
                }
            }
        }
        return sb.ToString();
    }

    private static void AppendLabel(
        System.Text.StringBuilder sb, ChartAnnotation a, double x, double y, string anchor, string style)
    {
        if (string.IsNullOrEmpty(a.Label)) return;
        sb.Append(string.Create(_inv, $"<text x=\"{x:F1}\" y=\"{y:F1}\" text-anchor=\"{anchor}\" style=\"{style}\">{a.Label}</text>"));
    }

    // Visually-hidden data table (a11y fallback): categories x series.
    private RenderFragment RenderDataTable() => builder =>
    {
        var series = Data!.Series;
        var labels = Data!.Labels;
        int seq = 0;
        builder.OpenElement(seq++, "table");
        builder.AddAttribute(seq++, "class", Css.Classes.Chart.Table);
        if (Title is not null)
        {
            builder.OpenElement(seq++, "caption");
            builder.AddContent(seq++, Title);
            builder.CloseElement();
        }
        builder.OpenElement(seq++, "thead");
        builder.OpenElement(seq++, "tr");
        builder.OpenElement(seq++, "th");
        builder.CloseElement();
        foreach (var s in series)
        {
            builder.OpenElement(seq++, "th");
            builder.AddContent(seq++, s.Label);
            builder.CloseElement();
        }
        builder.CloseElement();
        builder.CloseElement();

        builder.OpenElement(seq++, "tbody");
        int rows = labels is { Count: > 0 } ? labels.Count : (series.Count > 0 ? series.Max(s => s.Values.Count) : 0);
        for (int r = 0; r < rows; r++)
        {
            builder.OpenElement(seq++, "tr");
            builder.OpenElement(seq++, "th");
            builder.AddContent(seq++, labels is { } l && r < l.Count ? l[r] : (r + 1).ToString());
            builder.CloseElement();
            foreach (var s in series)
            {
                builder.OpenElement(seq++, "td");
                builder.AddContent(seq++, r < s.Values.Count ? s.Values[r].ToString(_inv) : string.Empty);
                builder.CloseElement();
            }
            builder.CloseElement();
        }
        builder.CloseElement();
        builder.CloseElement();
    };

    // Horizontal grid and the Y labels that sit on it. Both come off the SAME resolved axis, so a label
    // can never name a value its line is not drawn at.
    private string GridLines(AxisScale axis)
    {
        var sb = new System.Text.StringBuilder();
        double min = axis.Min, span = axis.Max - axis.Min;
        if (span <= 0) span = 1;
        int steps = Math.Max(1, axis.Lines - 1);
        double right = _svgW - _padR;
        double band = _plotH / (double)steps;
        int decimals = AxisDecimals(min, span / steps);
        int minor = Math.Clamp(YAxisMinorTicks, 0, 10);
        for (int i = 0; i <= steps; i++)
        {
            double v = min + span * i / steps;
            double y = _padT + _plotH - band * i;
            if (_showGrid)
                sb.Append(string.Create(_inv, $"<line x1=\"{_padL}\" y1=\"{y:F1}\" x2=\"{right}\" y2=\"{y:F1}\" style=\"{_gridStyle}\"/>"));
            if (_showY)
                sb.Append(string.Create(_inv, $"<text x=\"{_padL - 4}\" y=\"{y + 4:F1}\" text-anchor=\"end\" style=\"{_labelStyle}\">{FmtY(v, decimals)}</text>"));
            if (_showGrid && minor > 0 && i < steps)
                for (int m = 1; m <= minor; m++)
                {
                    double my = y - band * m / (minor + 1);
                    sb.Append(string.Create(_inv, $"<line x1=\"{_padL}\" y1=\"{my:F1}\" x2=\"{right}\" y2=\"{my:F1}\" style=\"{_gridMinorStyle}\"/>"));
                }
        }
        return sb.ToString();
    }

    // Category grid: one line per LABELLED slot, on the same projection and the same label budget as
    // AxisLabels, so every line stands under a label and both track a zoom together.
    private string VerticalGrid(int pts)
    {
        if (!_showGrid || !ShowVerticalGrid || pts <= 0) return "";
        var (from, to) = VisibleIndexRange(pts);
        if (to < from) return "";
        var sb = new System.Text.StringBuilder();
        int step = LabelStep(to - from + 1);
        double top = _padT, bottom = _padT + _plotH;
        for (int i = from; i <= to; i += step)
        {
            double x = XOfIndex(i);
            if (x < _padL - 1 || x > _padL + _plotW + 1) continue;
            sb.Append(string.Create(_inv, $"<line x1=\"{x:F1}\" y1=\"{top}\" x2=\"{x:F1}\" y2=\"{bottom}\" style=\"{_gridStyle}\"/>"));
        }
        return sb.ToString();
    }

    // Y-axis label formatting: the .NET YAxisFormat when set, else fixed point at the precision the axis
    // actually carries. A general format cannot do this job - "G3" turns 1002 into "1E+03", so an axis over
    // data in the thousands used to print the same unreadable label on every line.
    private string FmtY(double v, int decimals)
    {
        if (!string.IsNullOrEmpty(YAxisFormat)) return v.ToString(YAxisFormat, _inv);
        var text = v.ToString("F" + decimals.ToString(_inv), _inv);
        // The label lives in the left padding; past about eight glyphs it runs off the viewBox, so a value
        // that large falls back to the compact form rather than overflowing the plot.
        return text.Length <= 8 ? text : v.ToString("G3", _inv);
    }

    // Optional X/Y axis titles for the cartesian charts.
    private string AxisTitles()
    {
        if (_spark) return "";
        var sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(XAxisTitle))
            sb.Append(string.Create(_inv, $"<text x=\"{_padL + _plotW / 2.0:F1}\" y=\"{_svgH - 2:F1}\" text-anchor=\"middle\" style=\"{_axisTitleStyle}\">{XAxisTitle}</text>"));
        if (!string.IsNullOrEmpty(YAxisTitle))
        {
            double cy = _padT + _plotH / 2.0;
            sb.Append(string.Create(_inv, $"<text x=\"10\" y=\"{cy:F1}\" text-anchor=\"middle\" transform=\"rotate(-90 10 {cy:F1})\" style=\"{_axisTitleStyle}\">{YAxisTitle}</text>"));
        }
        return sb.ToString();
    }

    // Ticks are derived from the VISIBLE window, not from the full data: zooming in therefore reveals
    // more labels rather than stretching the same six, which is the behaviour that makes a zoom useful.
    //
    // How MANY comes from the available width rather than a fixed count, because the width is no longer
    // fixed: a fluid chart in a wide column has room for every label, and a 400-unit one does not. One
    // label per _labelSlot units of plot is the budget; the step is whatever meets it.
    private const double _labelSlot = 56;

    // How many slots to skip between two labels so they fit the width budget. Shared with the vertical
    // grid: a line drawn on a different step from the labels stands next to nothing.
    private int LabelStep(int visible) =>
        Math.Max(1, (int)Math.Ceiling(visible / (double)Math.Max(2, (int)(_plotW / _labelSlot))));

    private string AxisLabels(double min, double max, int pts)
    {
        if (Data?.Labels is not { Count: > 0 } labels) return "";
        var sb = new System.Text.StringBuilder();
        var (from, to) = VisibleIndexRange(pts);
        int visible = to - from + 1;
        if (visible <= 0) return "";
        int step = LabelStep(visible);
        double y = _padT + _plotH + 14;
        for (int i = from; i <= to; i += step)
        {
            if (i >= labels.Count) break;
            double x = XOfIndex(i);
            if (x < _padL - 1 || x > _padL + _plotW + 1) continue;
            sb.Append(string.Create(_inv, $"<text x=\"{x:F1}\" y=\"{y:F1}\" text-anchor=\"middle\" style=\"{_labelStyle}\">{labels[i]}</text>"));
        }
        return sb.ToString();
    }
}
