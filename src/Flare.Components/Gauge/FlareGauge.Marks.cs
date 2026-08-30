using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// Where the ticks, labels, needle and target marker land. These need the theme's geometry as NUMBERS -
// a tick starts a token's gap outside the track and runs a token's length further out - so they read the
// tokens through ReadTokenNum, the same way FlareChart reads its point radius and line width. The
// fallbacks are the no-theme-at-all case; every shipped theme sets all of them.
public partial class FlareGauge
{
    private double TrackWidthUnits => ReadTokenNum(Css.Tokens.Gauge.TrackWidth, 14);
    private double TickGapUnits => ReadTokenNum(Css.Tokens.Gauge.TickGap, 5);
    private double TickLengthUnits => ReadTokenNum(Css.Tokens.Gauge.TickLength, 7);
    private double TickMinorLengthUnits => ReadTokenNum(Css.Tokens.Gauge.TickMinorLength, 4);
    private double LabelSizeUnits => ReadTokenNum(Css.Tokens.Gauge.LabelSize, 9);
    private double NeedleLengthFraction => ReadTokenNum(Css.Tokens.Gauge.NeedleLength, 0.78);

    /// <summary>Pivot radius as an SVG attribute value; the disc is dropped when a theme zeroes it.</summary>
    private string PivotRadiusRef =>
        ReadTokenNum(Css.Tokens.Gauge.PivotRadius, 5).ToString("F2", CultureInfo.InvariantCulture);

    // The track's outer edge - where anything drawn beside the scale has to start from.
    private double TrackOuterRadius
    {
        get
        {
            var (_, _, _, _, r) = Dial;
            return r + TrackWidthUnits / 2;
        }
    }

    private (double X, double Y) TickInner(double angle)
    {
        var (_, _, cx, cy, _) = Dial;
        return ArcGeometry.Point(cx, cy, TrackOuterRadius + TickGapUnits, angle);
    }

    private (double X, double Y) TickOuter(double angle, bool major)
    {
        var (_, _, cx, cy, _) = Dial;
        var length = major ? TickLengthUnits : TickMinorLengthUnits;
        return ArcGeometry.Point(cx, cy, TrackOuterRadius + TickGapUnits + length, angle);
    }

    // Labels clear the longest tick and then their own half-height, so a label never overlaps the tick it
    // belongs to whatever the theme sets either to.
    private (double X, double Y) ScaleLabelPoint(double angle)
    {
        var (_, _, cx, cy, _) = Dial;
        var radius = TrackOuterRadius + TickGapUnits + TickLengthUnits + LabelSizeUnits * 0.8;
        return ArcGeometry.Point(cx, cy, radius, angle);
    }

    // The target crosses the whole track rather than sitting beside it: it is a reading on the same scale,
    // so it belongs on the same band the fill is drawn in.
    private (double X, double Y) TargetInner(double angle)
    {
        var (_, _, cx, cy, r) = Dial;
        return ArcGeometry.Point(cx, cy, r - TrackWidthUnits / 2, angle);
    }

    private (double X, double Y) TargetOuter(double angle)
    {
        var (_, _, cx, cy, r) = Dial;
        return ArcGeometry.Point(cx, cy, r + TrackWidthUnits / 2, angle);
    }

    /// <summary>
    /// The needle's tip, drawn pointing at the scale's START. The rendered rotation then carries the
    /// value, which is what makes the sweep a transform transition instead of a redraw.
    /// </summary>
    private (double X, double Y) NeedleTip
    {
        get
        {
            var (_, _, cx, cy, r) = Dial;
            var (from, _) = Sweep;
            return ArcGeometry.Point(cx, cy, r * NeedleLengthFraction, from);
        }
    }

    // -- Linear --------------------------------------------------------------

    // Ticks hang below a horizontal bar and to the right of a vertical one - the side the scale labels
    // read from in each orientation.
    private (double X, double Y) LinearTickEnd(double x, double y, bool major)
    {
        var length = (major ? TickLengthUnits : TickMinorLengthUnits) * 0.6;
        var offset = TrackWidthUnits / 2 + TickGapUnits * 0.4;
        return Vertical ? (x + offset + length, y) : (x, y + offset + length);
    }

    private (double X, double Y) LinearScaleLabelPoint(double x, double y)
    {
        var offset = TrackWidthUnits / 2 + TickGapUnits * 0.4 + TickLengthUnits * 0.6 + LabelSizeUnits * 0.8;
        return Vertical ? (x + offset, y) : (x, y + offset);
    }

    private (double X1, double Y1, double X2, double Y2) LinearTargetSpan(double x, double y)
    {
        var half = TrackWidthUnits / 2;
        return Vertical ? (x - half, y, x + half, y) : (x, y - half, x, y + half);
    }

    /// <summary>
    /// One scale label. Razor reserves the tag name <c>text</c> for its own control blocks, so an SVG text
    /// node carrying attributes cannot be written as markup and is emitted here instead.
    /// </summary>
    private RenderFragment ScaleText(double x, double y, string content) => builder =>
    {
        builder.OpenElement(0, "text");
        builder.AddAttribute(1, "class", Css.Classes.Gauge.ScaleLabel);
        builder.AddAttribute(2, "x", F(x));
        builder.AddAttribute(3, "y", F(y));
        builder.AddContent(4, content);
        builder.CloseElement();
    };

    // -- Class and style helpers --------------------------------------------

    // A zone's colour arrives either as a role class (which sets --fc-main) or, for a custom colour, as an
    // inline channel; both ends are read by one var() in the stylesheet.
    private static string BandClass(FlareZone band) => string.IsNullOrEmpty(band.Color.CssClass)
        ? Css.Classes.Gauge.Band
        : $"{Css.Classes.Gauge.Band} {band.Color.CssClass}";

    private string FillClass => string.IsNullOrEmpty(Color.CssClass)
        ? Css.Classes.Gauge.Fill
        : $"{Css.Classes.Gauge.Fill} {Color.CssClass}";

    private string NeedleClass => string.IsNullOrEmpty(Color.CssClass)
        ? Css.Classes.Gauge.Needle
        : $"{Css.Classes.Gauge.Needle} {Color.CssClass}";

    /// <summary>
    /// The dash offset that reveals the fill: the path is normalised to 100 units by <c>pathLength</c>, so
    /// the offset is exactly the percentage of the scale still to fill.
    /// </summary>
    private string FillStyle
    {
        get
        {
            var hidden = (1 - Fraction(ClampedValue)) * 100;
            var offset = $"stroke-dashoffset:{hidden.ToString("F2", CultureInfo.InvariantCulture)};";
            return Color.StyleMain() is { Length: > 0 } main ? main + offset : offset;
        }
    }
}
