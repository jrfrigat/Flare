namespace Flare.Components;

public partial class FlareChart
{
    private sealed record RadarLayout(int Axes, double Max, double Cx, double Cy, double Radius);
    private sealed record SectorMark(int Index, double Value, string Path);
    private sealed record SectorLayout(double Cx, double Cy, double Radius, List<SectorMark> Marks);
    private RadarLayout? _radarLayout;
    private SectorLayout? _sectorLayout;

    private (double Cx, double Cy, double Radius) RadialBounds(int padding)
    {
        double cx = _svgW / 2.0, cy = _svgH / 2.0;
        return (cx, cy, Math.Max(0, Math.Min(cx, cy) - padding));
    }

    private static double RadarAngle(int index, int axes) => ArcGeometry.Top + 2 * Math.PI * index / axes;

    private RadarLayout? ProjectRadar()
    {
        if (Data?.Series is not { Count: > 0 } series) return null;
        int axes = series.Max(s => s.Values.Count);
        if (axes < 3) return null;
        double max = ScaleValues(series).DefaultIfEmpty(0).Max();
        var (cx, cy, radius) = RadialBounds(24);
        return new(axes, max > 0 ? max : 1, cx, cy, radius);
    }

    private SectorLayout? ProjectSectors()
    {
        if (Data is null) return null;
        var visible = VisibleRadialIndices();
        var (cx, cy, radius) = RadialBounds(16);
        if (visible.Count == 0 || radius == 0) return null;
        double max = visible.Max(i => Data.Series[i].Values[0]);
        double step = 2 * Math.PI / visible.Count;
        var marks = new List<SectorMark>(visible.Count);
        for (int slot = 0; slot < visible.Count; slot++)
        {
            int index = visible[slot];
            double value = Data.Series[index].Values[0];
            double ratio = value / max;
            double r = (Type == ChartType.Rose ? Math.Sqrt(ratio) : ratio) * radius;
            double start = ArcGeometry.Top + slot * step;
            marks.Add(new(index, value, ArcGeometry.Wedge(cx, cy, r, 0, start, start + step)));
        }
        return new(cx, cy, radius, marks);
    }
}
