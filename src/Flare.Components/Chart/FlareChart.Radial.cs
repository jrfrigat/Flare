using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Radial and matrix renderers: pie/donut, radar, heat map, rose and polar area.
public partial class FlareChart
{
    private RenderFragment RenderPie(bool donut) => builder =>
    {
        // donut vs pie is already encoded in Type, which _slices reads.
        int seq = 0;
        foreach (var slice in _slices)
        {
            builder.AddMarkupContent(seq++,
                $"<path d=\"{slice.Path}\" style=\"fill:{GetColor(slice.Index)};{_sliceStrokeStyle}\"/>");
        }
        if (ShowValues && Data?.Series is { Count: > 0 } series)
        {
            double total = series.Sum(s => s.Values.FirstOrDefault());
            if (total > 0)
                foreach (var slice in _slices)
                {
                    double pct = series[slice.Index].Values.FirstOrDefault() / total * 100;
                    if (pct < 4) continue; // skip labels on tiny slivers
                    builder.AddMarkupContent(seq++, string.Create(_inv, $"<text x=\"{slice.Cx:F1}\" y=\"{slice.Cy + 3:F1}\" text-anchor=\"middle\" style=\"{_valueOnFillStyle}\">{pct:F0}%</text>"));
                }
        }
    };


    private RenderFragment RenderRadar() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        int axes = series.Max(s => s.Values.Count);
        if (axes < 3) return; // a radar needs at least 3 axes
        double max = ScaleValues(series).DefaultIfEmpty(0).Max();
        if (max <= 0) max = 1;
        double cx = _svgW / 2.0, cy = _svgH / 2.0;
        double r = Math.Min(cx, cy) - 24;
        int seq = 0;

        if (_showGrid)
        {
            var g = new System.Text.StringBuilder();
            const int levels = 4;
            for (int lv = 1; lv <= levels; lv++)
            {
                double rr = r * lv / levels;
                var poly = new System.Text.StringBuilder();
                for (int k = 0; k < axes; k++)
                {
                    double ang = -Math.PI / 2 + 2 * Math.PI * k / axes;
                    poly.Append(string.Create(_inv, $"{cx + rr * Math.Cos(ang):F1},{cy + rr * Math.Sin(ang):F1} "));
                }
                g.Append($"<polygon points=\"{poly.ToString().Trim()}\" fill=\"none\" style=\"{_gridStyle}\"/>");
            }
            for (int k = 0; k < axes; k++)
            {
                double ang = -Math.PI / 2 + 2 * Math.PI * k / axes;
                g.Append(string.Create(_inv, $"<line x1=\"{cx:F1}\" y1=\"{cy:F1}\" x2=\"{cx + r * Math.Cos(ang):F1}\" y2=\"{cy + r * Math.Sin(ang):F1}\" style=\"{_gridStyle}\"/>"));
            }
            builder.AddMarkupContent(seq++, g.ToString());
        }

        if (_showX && Data?.Labels is { Count: > 0 } labels)
        {
            var t = new System.Text.StringBuilder();
            for (int k = 0; k < axes && k < labels.Count; k++)
            {
                double ang = -Math.PI / 2 + 2 * Math.PI * k / axes;
                double lx = cx + (r + 10) * Math.Cos(ang), ly = cy + (r + 10) * Math.Sin(ang);
                string anchor = Math.Abs(Math.Cos(ang)) < 0.3 ? "middle" : (Math.Cos(ang) > 0 ? "start" : "end");
                t.Append(string.Create(_inv, $"<text x=\"{lx:F1}\" y=\"{ly + 3:F1}\" text-anchor=\"{anchor}\" style=\"{_labelStyle}\">{labels[k]}</text>"));
            }
            builder.AddMarkupContent(seq++, t.ToString());
        }

        for (int si = 0; si < series.Count; si++)
        {
            if (IsHidden(si)) continue;
            var vals = series[si].Values;
            if (vals.Count < 3) continue;
            var color = GetColor(si);
            var poly = new System.Text.StringBuilder();
            for (int k = 0; k < axes; k++)
            {
                double v = k < vals.Count ? SafeValue(vals[k]) : 0;
                double rr = v / max * r;
                double ang = -Math.PI / 2 + 2 * Math.PI * k / axes;
                poly.Append(string.Create(_inv, $"{cx + rr * Math.Cos(ang):F1},{cy + rr * Math.Sin(ang):F1} "));
            }
            builder.AddMarkupContent(seq++, $"<polygon points=\"{poly.ToString().Trim()}\" style=\"fill:{color};stroke:{color};fill-opacity:var(--flare-chart-radar-fill-opacity);stroke-width:var(--flare-chart-line-width);stroke-linejoin:round\"/>");
        }
    };

    private RenderFragment RenderHeatMap() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        int rows = series.Count;
        int cols = series.Max(s => s.Values.Count);
        if (cols == 0) return;
        var allVals = series.SelectMany(s => s.Values).ToList();
        double min = allVals.Min(), max = allVals.Max();
        if (max == min) max = min + 1;
        int seq = 0;

        double cellW = _plotW / (double)cols;
        double cellH = _plotH / (double)rows;
        // The ramp is the theme's, not the component's: base color, floor and ceiling all come from tokens,
        // so a theme can run the intensity over its own hue - or over a fixed range that never reaches full
        // opacity, which is what a light-on-dark heat map needs. The floor keeps a zero cell readable as a
        // cell rather than as a hole in the grid.
        double gap = CellGap(), cellR = CellRadius();
        double opMin = RampMinOpacity(), opSpan = RampMaxOpacity() - opMin;
        for (int r = 0; r < rows; r++)
        {
            var vals = series[r].Values;
            for (int c = 0; c < cols; c++)
            {
                double v = c < vals.Count ? SafeValue(vals[c]) : min;
                double t = (v - min) / (max - min);
                double x = _padL + c * cellW, y = _padT + r * cellH;
                builder.AddMarkupContent(seq++, string.Create(_inv,
                    $"<rect class=\"{Css.Classes.Chart.Cell}\" x=\"{x + gap / 2:F1}\" y=\"{y + gap / 2:F1}\" width=\"{Math.Max(0, cellW - gap):F1}\" height=\"{Math.Max(0, cellH - gap):F1}\" rx=\"{cellR:F2}\" style=\"fill:var(--flare-chart-ramp-color);fill-opacity:{(opMin + opSpan * t):F2}\"/>"));
                if (ShowValues && cellH >= 14)
                    builder.AddMarkupContent(seq++, string.Create(_inv,
                        $"<text x=\"{x + cellW / 2:F1}\" y=\"{y + cellH / 2 + 3:F1}\" text-anchor=\"middle\" style=\"{_valueStyle}\">{v:G3}</text>"));
            }
        }
        // Row labels (series names) on the left.
        if (_showY)
        {
            var t = new System.Text.StringBuilder();
            for (int r = 0; r < rows; r++)
                t.Append(string.Create(_inv, $"<text x=\"{_padL - 4:F1}\" y=\"{_padT + r * cellH + cellH / 2 + 3:F1}\" text-anchor=\"end\" style=\"{_labelStyle}\">{series[r].Label}</text>"));
            builder.AddMarkupContent(seq++, t.ToString());
        }
        // Column labels along the bottom.
        if (_showX && Data?.Labels is { Count: > 0 } labels)
        {
            var t = new System.Text.StringBuilder();
            for (int c = 0; c < cols && c < labels.Count; c++)
                t.Append(string.Create(_inv, $"<text x=\"{_padL + c * cellW + cellW / 2:F1}\" y=\"{_padT + _plotH + 14:F1}\" text-anchor=\"middle\" style=\"{_labelStyle}\">{labels[c]}</text>"));
            builder.AddMarkupContent(seq++, t.ToString());
        }
    };

    private RenderFragment RenderRose() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var vals = series.Select(s => SafeValue(s.Values.FirstOrDefault())).ToList();
        double max = vals.Count > 0 ? vals.Max() : 1;
        if (max <= 0) max = 1;
        double cx = _svgW / 2.0, cy = _svgH / 2.0;
        double rMax = Math.Min(cx, cy) - 16;
        int n = series.Count;
        double step = 2 * Math.PI / n;
        int seq = 0;
        for (int i = 0; i < n; i++)
        {
            double rr = Math.Sqrt(vals[i] / max) * rMax; // sqrt => area-proportional sectors
            double a0 = -Math.PI / 2 + i * step, a1 = a0 + step;
            double x0 = cx + rr * Math.Cos(a0), y0 = cy + rr * Math.Sin(a0);
            double x1 = cx + rr * Math.Cos(a1), y1 = cy + rr * Math.Sin(a1);
            int large = step > Math.PI ? 1 : 0;
            string path = string.Create(_inv, $"M {cx:F1} {cy:F1} L {x0:F1} {y0:F1} A {rr:F1} {rr:F1} 0 {large} 1 {x1:F1} {y1:F1} Z");
            builder.AddMarkupContent(seq++, $"<path d=\"{path}\" style=\"fill:{GetColor(i)};{_sliceStrokeStyle}\"/>");
        }
    };

    private RenderFragment RenderPolarArea() => builder =>
    {
        if (Data?.Series is not { Count: > 0 } series) return;
        var vals = series.Select(s => SafeValue(s.Values.FirstOrDefault())).ToList();
        double max = vals.Count > 0 ? vals.Max() : 1;
        if (max <= 0) max = 1;
        double cx = _svgW / 2.0, cy = _svgH / 2.0;
        double rMax = Math.Min(cx, cy) - 16;
        int n = series.Count;
        double step = 2 * Math.PI / n;
        int seq = 0;
        if (_showGrid)
        {
            var g = new System.Text.StringBuilder();
            for (int lv = 1; lv <= 4; lv++)
                g.Append(string.Create(_inv, $"<circle cx=\"{cx:F1}\" cy=\"{cy:F1}\" r=\"{rMax * lv / 4:F1}\" fill=\"none\" style=\"{_gridStyle}\"/>"));
            builder.AddMarkupContent(seq++, g.ToString());
        }
        for (int i = 0; i < n; i++)
        {
            double rr = vals[i] / max * rMax; // linear radius (Chart.js style)
            double a0 = -Math.PI / 2 + i * step, a1 = a0 + step;
            double x0 = cx + rr * Math.Cos(a0), y0 = cy + rr * Math.Sin(a0);
            double x1 = cx + rr * Math.Cos(a1), y1 = cy + rr * Math.Sin(a1);
            int large = step > Math.PI ? 1 : 0;
            string path = string.Create(_inv, $"M {cx:F1} {cy:F1} L {x0:F1} {y0:F1} A {rr:F1} {rr:F1} 0 {large} 1 {x1:F1} {y1:F1} Z");
            builder.AddMarkupContent(seq++, $"<path d=\"{path}\" style=\"fill:{GetColor(i)};stroke:{GetColor(i)};fill-opacity:var(--flare-chart-wedge-opacity);stroke-width:var(--flare-chart-slice-stroke-width)\"/>");
        }
    };

}
