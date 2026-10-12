namespace Flare.Components;

public partial class FlareChart
{
    private sealed record PointMark(int Series, int Index, ChartPoint Value, double X, double Y, double Radius);
    private sealed record PointLayout(AxisScale Axis, List<PointMark> Marks);
    private PointLayout? _pointLayout;

    private sealed record CellMark(int Series, int Row, int Column, double Value, double X, double Y, double Width, double Height);
    private sealed record CellLayout(List<int> Rows, int Columns, double CellWidth, double CellHeight,
        double Min, double Max, List<CellMark> Marks);
    private CellLayout? _cellLayout;

    // Prepared once per render, before either the visual fragment or input zones read the geometry.
    private PointLayout? ProjectPoints()
    {
        if (Data is null) return null;
        var all = ScalePoints();
        if (all.Count == 0) return null;
        var axis = ResolveAxis(all.Min(p => p.Y), all.Max(p => p.Y), _plotH);
        double weightMax = _bubble ? Math.Max(1e-9, all.Max(p => BubbleWeight(p.R))) : 0;
        double pointRadius = PointRadius(), bubbleMin = BubbleMinRadius();
        double bubbleSpan = Math.Max(0, BubbleMaxRadius() - bubbleMin);
        var marks = new List<PointMark>();
        for (int si = 0; si < Data.Series.Count; si++)
        {
            if (IsHidden(si) || Data.Series[si].Points is not { } points) continue;
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (!IsValidPoint(point)) continue;
                double radius = _bubble ? bubbleMin + bubbleSpan * Math.Sqrt(BubbleWeight(point.R) / weightMax) : pointRadius;
                marks.Add(new(si, i, point, XOfIndex(point.X),
                    _padT + _plotH - (point.Y - axis.Min) / (axis.Max - axis.Min) * _plotH, radius));
            }
        }
        return new(axis, marks);
    }

    private CellLayout? ProjectCells()
    {
        if (Data?.Series is not { Count: > 0 } series) return null;
        var rows = VisibleSeriesIndices(series.Count);
        int columns = series.Max(s => s.Values.Count);
        if (rows.Count == 0 || columns == 0) return null;
        var values = ScaleValues(series);
        if (values.Count == 0) return null;
        double min = values.Min(), max = values.Max();
        if (max == min) max = min + 1;
        double cellW = _plotW / (double)columns, cellH = _plotH / (double)rows.Count, gap = CellGap();
        var marks = new List<CellMark>();
        for (int row = 0; row < rows.Count; row++)
        {
            int si = rows[row];
            for (int col = 0; col < series[si].Values.Count; col++)
            {
                double value = series[si].Values[col];
                if (IsGap(value)) continue;
                marks.Add(new(si, row, col, value, _padL + col * cellW + gap / 2, _padT + row * cellH + gap / 2,
                    Math.Max(0, cellW - gap), Math.Max(0, cellH - gap)));
            }
        }
        return new(rows, columns, cellW, cellH, min, max, marks);
    }
}
