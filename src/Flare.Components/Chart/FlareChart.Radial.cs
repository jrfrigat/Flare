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
        if (ShowValues)
        {
            foreach (var slice in _slices)
            {
                double pct = slice.Percent;
                if (pct < 4) continue; // skip labels on tiny slivers
                builder.AddMarkupContent(seq++, string.Create(_inv, $"<text x=\"{slice.Cx:F1}\" y=\"{slice.Cy + 3:F1}\" text-anchor=\"middle\" style=\"{_valueOnFillStyle}\">{pct:F0}%</text>"));
            }
        }
    };


    private RenderFragment RenderRadar() => builder =>
    {
        if (_radarLayout is not { } layout) return;
        var series = Data!.Series;
        int axes = layout.Axes;
        double max = layout.Max, cx = layout.Cx, cy = layout.Cy, r = layout.Radius;
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
                    double ang = RadarAngle(k, axes);
                    poly.Append(string.Create(_inv, $"{cx + rr * Math.Cos(ang):F1},{cy + rr * Math.Sin(ang):F1} "));
                }
                g.Append($"<polygon points=\"{poly.ToString().Trim()}\" fill=\"none\" style=\"{_gridStyle}\"/>");
            }
            for (int k = 0; k < axes; k++)
            {
                double ang = RadarAngle(k, axes);
                g.Append(string.Create(_inv, $"<line x1=\"{cx:F1}\" y1=\"{cy:F1}\" x2=\"{cx + r * Math.Cos(ang):F1}\" y2=\"{cy + r * Math.Sin(ang):F1}\" style=\"{_gridStyle}\"/>"));
            }
            builder.AddMarkupContent(seq++, g.ToString());
        }

        if (_showX && Data?.Labels is { Count: > 0 } labels)
        {
            var t = new System.Text.StringBuilder();
            for (int k = 0; k < axes && k < labels.Count; k++)
            {
                double ang = RadarAngle(k, axes);
                double lx = cx + (r + 10) * Math.Cos(ang), ly = cy + (r + 10) * Math.Sin(ang);
                string anchor = Math.Abs(Math.Cos(ang)) < 0.3 ? "middle" : (Math.Cos(ang) > 0 ? "start" : "end");
                t.Append(string.Create(_inv, $"<text x=\"{lx:F1}\" y=\"{ly + 3:F1}\" text-anchor=\"{anchor}\" style=\"{_labelStyle}\">{SvgText(labels[k])}</text>"));
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
                double v = k < vals.Count ? RadialWeight(vals[k]) : 0;
                double rr = v / max * r;
                double ang = RadarAngle(k, axes);
                poly.Append(string.Create(_inv, $"{cx + rr * Math.Cos(ang):F1},{cy + rr * Math.Sin(ang):F1} "));
            }
            builder.AddMarkupContent(seq++, $"<polygon points=\"{poly.ToString().Trim()}\" style=\"fill:{color};stroke:{color};fill-opacity:var({Css.Tokens.Chart.RadarFillOpacity});stroke-width:var({Css.Tokens.Chart.LineWidth});stroke-linejoin:round\"/>");
        }
    };

    private RenderFragment RenderHeatMap() => builder =>
    {
        if (_cellLayout is not { } layout) return;
        var series = Data!.Series;
        var visible = layout.Rows;
        int rows = visible.Count;
        int cols = layout.Columns;
        double min = layout.Min, max = layout.Max;
        int seq = 0;

        double cellW = layout.CellWidth, cellH = layout.CellHeight;
        // The ramp is the theme's, not the component's: base color, floor and ceiling all come from tokens,
        // so a theme can run the intensity over its own hue - or over a fixed range that never reaches full
        // opacity, which is what a light-on-dark heat map needs. The floor keeps a zero cell readable as a
        // cell rather than as a hole in the grid.
        double cellR = CellRadius();
        double opMin = RampMinOpacity(), opSpan = RampMaxOpacity() - opMin;
        foreach (var cell in layout.Marks)
        {
            double t = (cell.Value - min) / (max - min);
            builder.AddMarkupContent(seq++, string.Create(_inv,
                $"<rect class=\"{Css.Classes.Chart.Cell}\" x=\"{cell.X:F1}\" y=\"{cell.Y:F1}\" width=\"{cell.Width:F1}\" height=\"{cell.Height:F1}\" rx=\"{cellR:F2}\" style=\"fill:var({Css.Tokens.Chart.RampColor});fill-opacity:{(opMin + opSpan * t):F2}\"/>"));
            if (ShowValues && cellH >= 14)
                builder.AddMarkupContent(seq++, string.Create(_inv,
                    $"<text x=\"{_padL + cell.Column * cellW + cellW / 2:F1}\" y=\"{_padT + cell.Row * cellH + cellH / 2 + 3:F1}\" text-anchor=\"middle\" style=\"{_valueStyle}\">{cell.Value:G3}</text>"));
        }
        // Row labels (series names) on the left.
        if (_showY)
        {
            var t = new System.Text.StringBuilder();
            for (int r = 0; r < rows; r++)
                t.Append(string.Create(_inv, $"<text x=\"{_padL - 4:F1}\" y=\"{_padT + r * cellH + cellH / 2 + 3:F1}\" text-anchor=\"end\" style=\"{_labelStyle}\">{SvgText(series[visible[r]].Label)}</text>"));
            builder.AddMarkupContent(seq++, t.ToString());
        }
        // Column labels along the bottom.
        if (_showX && Data?.Labels is { Count: > 0 } labels)
        {
            var t = new System.Text.StringBuilder();
            for (int c = 0; c < cols && c < labels.Count; c++)
                t.Append(string.Create(_inv, $"<text x=\"{_padL + c * cellW + cellW / 2:F1}\" y=\"{_padT + _plotH + 14:F1}\" text-anchor=\"middle\" style=\"{_labelStyle}\">{SvgText(labels[c])}</text>"));
            builder.AddMarkupContent(seq++, t.ToString());
        }
    };

    private RenderFragment RenderRose() => builder =>
    {
        if (_sectorLayout is not { } layout) return;
        int seq = 0;
        foreach (var sector in layout.Marks)
        {
            builder.AddMarkupContent(seq++, $"<path d=\"{sector.Path}\" style=\"fill:{GetColor(sector.Index)};{_sliceStrokeStyle}\"/>");
        }
    };

    private RenderFragment RenderPolarArea() => builder =>
    {
        if (_sectorLayout is not { } layout) return;
        double cx = layout.Cx, cy = layout.Cy, rMax = layout.Radius;
        int seq = 0;
        if (_showGrid)
        {
            var g = new System.Text.StringBuilder();
            for (int lv = 1; lv <= 4; lv++)
                g.Append(string.Create(_inv, $"<circle cx=\"{cx:F1}\" cy=\"{cy:F1}\" r=\"{rMax * lv / 4:F1}\" fill=\"none\" style=\"{_gridStyle}\"/>"));
            builder.AddMarkupContent(seq++, g.ToString());
        }
        foreach (var sector in layout.Marks)
        {
            builder.AddMarkupContent(seq++, $"<path d=\"{sector.Path}\" style=\"fill:{GetColor(sector.Index)};stroke:{GetColor(sector.Index)};fill-opacity:var({Css.Tokens.Chart.WedgeOpacity});stroke-width:var({Css.Tokens.Chart.SliceStrokeWidth})\"/>");
        }
    };

}
