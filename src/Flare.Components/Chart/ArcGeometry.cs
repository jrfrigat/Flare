using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Polar-to-cartesian conversion and SVG arc path construction, shared by every radial surface: the
/// chart's pie and donut slices and the gauge's track, bands and fill. It lived inside the chart's slice
/// property, which meant the gauge would have had to copy the same large-arc-flag and sweep-direction
/// reasoning - the two places most likely to disagree.
///
/// Angles are radians measured the SVG way: 0 points right (+x) and positive turns clockwise, because
/// SVG's y axis grows downward. Callers that think in "twelve o'clock" use <see cref="Top"/>.
/// </summary>
internal static class ArcGeometry
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    /// <summary>The angle of the top of the circle - the usual start for a dial or a pie.</summary>
    public const double Top = -Math.PI / 2;

    /// <summary>Converts degrees to radians in the same frame the arc builders use.</summary>
    public static double Rad(double degrees) => degrees * Math.PI / 180.0;

    /// <summary>The point at <paramref name="angle"/> on a circle of radius <paramref name="r"/>.</summary>
    public static (double X, double Y) Point(double cx, double cy, double r, double angle) =>
        (cx + r * Math.Cos(angle), cy + r * Math.Sin(angle));

    /// <summary>
    /// An open arc as a stroked path: one <c>M</c> and one <c>A</c>, no fill. This is the shape a gauge
    /// track, band and fill all use, so their thickness comes from the stroke width rather than from a
    /// second radius - which is what lets a theme change gauge thickness with one token.
    /// </summary>
    public static string Stroke(double cx, double cy, double r, double startAngle, double endAngle)
    {
        // A full turn cannot be drawn as one arc: the start and end points coincide and the renderer draws
        // nothing. Two half turns are the standard way round it.
        var sweep = endAngle - startAngle;
        if (Math.Abs(sweep) >= 2 * Math.PI - 1e-6)
        {
            var half = startAngle + Math.PI * Math.Sign(sweep);
            return Stroke(cx, cy, r, startAngle, half) + " " + StrokeTail(cx, cy, r, half, startAngle + sweep);
        }

        var (x1, y1) = Point(cx, cy, r, startAngle);
        var (x2, y2) = Point(cx, cy, r, endAngle);
        var large = Math.Abs(sweep) > Math.PI ? 1 : 0;
        var dir = sweep >= 0 ? 1 : 0;
        return string.Create(Inv, $"M {x1:F2} {y1:F2} A {r:F2} {r:F2} 0 {large} {dir} {x2:F2} {y2:F2}");
    }

    // The continuation half of a full turn: same arc without the leading move.
    private static string StrokeTail(double cx, double cy, double r, double startAngle, double endAngle)
    {
        var (x2, y2) = Point(cx, cy, r, endAngle);
        var sweep = endAngle - startAngle;
        var large = Math.Abs(sweep) > Math.PI ? 1 : 0;
        var dir = sweep >= 0 ? 1 : 0;
        return string.Create(Inv, $"A {r:F2} {r:F2} 0 {large} {dir} {x2:F2} {y2:F2}");
    }

    /// <summary>
    /// A closed wedge: a pie slice when <paramref name="inner"/> is zero, a donut segment otherwise. The
    /// chart's slices are built from this.
    /// </summary>
    public static string Wedge(double cx, double cy, double r, double inner, double startAngle, double endAngle)
    {
        var sweep = endAngle - startAngle;
        var (x1, y1) = Point(cx, cy, r, startAngle);
        var (x2, y2) = Point(cx, cy, r, endAngle);
        var large = Math.Abs(sweep) > Math.PI ? 1 : 0;

        if (inner <= 0)
            return string.Create(Inv, $"M {cx:F2} {cy:F2} L {x1:F2} {y1:F2} A {r:F2} {r:F2} 0 {large} 1 {x2:F2} {y2:F2} Z");

        var (ix1, iy1) = Point(cx, cy, inner, startAngle);
        var (ix2, iy2) = Point(cx, cy, inner, endAngle);
        return string.Create(Inv,
            $"M {x1:F2} {y1:F2} A {r:F2} {r:F2} 0 {large} 1 {x2:F2} {y2:F2} L {ix2:F2} {iy2:F2} A {inner:F2} {inner:F2} 0 {large} 0 {ix1:F2} {iy1:F2} Z");
    }
}
