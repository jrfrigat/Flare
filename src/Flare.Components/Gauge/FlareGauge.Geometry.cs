using System.Globalization;

namespace Flare.Components;

// The projection from a value on the scale to a place on screen, and the viewBox that just contains the
// result. Everything here is pure geometry in viewBox units; every colour and thickness is a token read
// by gauge.css, so a theme can change how the gauge looks without any of these numbers moving.
public partial class FlareGauge
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private const double BoxWidth = 200;

    /// <summary>
    /// How far past the track's CENTRELINE the drawing reaches: half the track, then the tick gap, the
    /// ticks, and a scale label's own height when one is shown. Derived from the tokens rather than fixed,
    /// because every one of those is a theme's to change - a constant margin left the end labels of an arc
    /// outside the viewBox for any theme whose ticks were longer than it guessed.
    /// </summary>
    private double Margin
    {
        get
        {
            var reach = TrackWidthUnits / 2 + TickGapUnits + TickLengthUnits;
            return ShowScaleLabels ? reach + LabelSizeUnits * 1.6 : reach;
        }
    }

    /// <summary>The value clamped into the scale, so a reading outside it pins rather than escapes.</summary>
    private double ClampedValue => Math.Clamp(Value, Math.Min(Min, Max), Math.Max(Min, Max));

    /// <summary>Where a value sits on the scale, 0 at <see cref="Min"/> and 1 at <see cref="Max"/>.</summary>
    private double Fraction(double value)
    {
        var span = Max - Min;
        return Math.Abs(span) < double.Epsilon ? 0 : Math.Clamp((value - Min) / span, 0, 1);
    }

    // -- Radial and arc ------------------------------------------------------

    /// <summary>Start and end of the sweep in radians, in the frame <see cref="ArcGeometry"/> uses.</summary>
    private (double From, double To) Sweep =>
        (ArcGeometry.Rad(Start) + ArcGeometry.Top, ArcGeometry.Rad(End) + ArcGeometry.Top);

    /// <summary>The angle a value points at.</summary>
    private double AngleOf(double value)
    {
        var (from, to) = Sweep;
        return from + (to - from) * Fraction(value);
    }

    /// <summary>
    /// The viewBox and dial placement, sized so the swept arc exactly fills it. Computed rather than fixed
    /// because <c>StartAngle</c>/<c>EndAngle</c> are parameters: a quarter arc left in a square box would
    /// be a small drawing floating in three quarters of empty space.
    /// </summary>
    private (double W, double H, double Cx, double Cy, double R) Dial
    {
        get
        {
            var (from, to) = Sweep;
            var (lo, hi) = from <= to ? (from, to) : (to, from);

            // The extremes of an arc are its endpoints plus any axis crossing it passes through - that is
            // what makes the box tight for a sweep that stops partway round.
            double minX = 0, maxX = 0, minY = 0, maxY = 0; // the centre is always in: the readout sits there
            void Include(double angle)
            {
                var (x, y) = ArcGeometry.Point(0, 0, 1, angle);
                minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
            }

            Include(lo);
            Include(hi);
            // Walk the quarter turns inside the sweep. Start below lo and step until past hi, so a sweep
            // that begins mid-quadrant still picks up the crossings it actually passes.
            var first = Math.Ceiling(lo / (Math.PI / 2)) * (Math.PI / 2);
            for (var a = first; a <= hi + 1e-9; a += Math.PI / 2) Include(a);

            var spanX = Math.Max(maxX - minX, 1e-6);
            var spanY = Math.Max(maxY - minY, 1e-6);
            var r = (BoxWidth - 2 * Margin) / spanX;
            var h = spanY * r + 2 * Margin;
            return (BoxWidth, h, Margin - minX * r, Margin - minY * r, r);
        }
    }

    /// <summary>The whole scale as a stroked arc - the unfilled track.</summary>
    private string TrackPath
    {
        get
        {
            var (w, h, cx, cy, r) = Dial;
            _ = (w, h);
            var (from, to) = Sweep;
            return ArcGeometry.Stroke(cx, cy, r, from, to);
        }
    }

    /// <summary>The arc between two values - a band.</summary>
    private string ArcBetween(double start, double end)
    {
        var (_, _, cx, cy, r) = Dial;
        return ArcGeometry.Stroke(cx, cy, r, AngleOf(start), AngleOf(end));
    }

    /// <summary>Degrees to rotate the needle by, given that it is drawn pointing at the scale's start.</summary>
    private double NeedleRotation
    {
        get
        {
            var (from, to) = Sweep;
            return (to - from) * Fraction(ClampedValue) * 180.0 / Math.PI;
        }
    }

    // -- Linear --------------------------------------------------------------

    // A linear gauge is the same scale projected onto a straight line, and its cross axis needs the same
    // reach on both sides of the track that a dial reserves around its arc.
    private double LinearThickness => 2 * Margin;

    /// <summary>The viewBox for the linear shape, and the two ends of its track.</summary>
    private (double W, double H, double X1, double Y1, double X2, double Y2) Bar
    {
        get
        {
            var length = BoxWidth;
            return Vertical
                ? (LinearThickness, length, LinearThickness / 2, length - Margin, LinearThickness / 2, Margin)
                : (length, LinearThickness, Margin, LinearThickness / 2, length - Margin, LinearThickness / 2);
        }
    }

    /// <summary>The point on the linear track a value sits at.</summary>
    private (double X, double Y) PointOn(double value)
    {
        var (_, _, x1, y1, x2, y2) = Bar;
        var t = Fraction(value);
        return (x1 + (x2 - x1) * t, y1 + (y2 - y1) * t);
    }

    // -- Ticks ---------------------------------------------------------------

    /// <summary>
    /// The values a tick is drawn at, major and minor. Intervals are read off the scale rather than a
    /// count, so the labels land on round numbers the reader recognises.
    /// </summary>
    private IEnumerable<(double Value, bool Major)> Ticks
    {
        get
        {
            var lo = Math.Min(Min, Max);
            var hi = Math.Max(Min, Max);
            var major = TickInterval > 0 ? TickInterval : (hi - lo) / 5;
            if (major <= 0) yield break;

            var minor = MinorTickInterval > 0 ? MinorTickInterval : 0;
            // A pathological interval against a wide scale would emit thousands of nodes; the cap is a
            // rendering limit, not a design opinion.
            var step = minor > 0 ? Math.Min(minor, major) : major;
            var count = (hi - lo) / step;
            if (count > 400) yield break;

            for (var i = 0; ; i++)
            {
                var v = lo + i * step;
                if (v > hi + step * 1e-6) yield break;
                v = Math.Min(v, hi);
                // Floating point makes "is this also a major tick" a tolerance question, not an equality one.
                var isMajor = minor <= 0 || Math.Abs(Math.Round((v - lo) / major) * major - (v - lo)) < major * 1e-6;
                yield return (v, isMajor);
                if (v >= hi) yield break;
            }
        }
    }

    private static string F(double v) => v.ToString("F2", Inv);
}
