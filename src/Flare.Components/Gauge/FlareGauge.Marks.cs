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

    private const string MinorTickClass = Css.Classes.Gauge.Tick + " " + Css.Classes.Gauge.TickMinor;

    private readonly record struct ScaleKey(GaugeShape Shape, bool Vertical, double Min, double Max,
        double Start, double End, double Interval, double MinorInterval, bool Ticks, bool Labels,
        double TrackWidth, double Gap, double Length, double MinorLength, double LabelSize, string? Format);

    private readonly record struct ScaleMark(string? TickClass, string? X1, string? Y1, string? X2, string? Y2,
        string? LabelX, string? LabelY, string? Label);

    private ScaleKey _scaleKey;
    private CultureInfo? _scaleCulture;
    private RenderFragment? _scale;

    private RenderFragment? Scale()
    {
        if (!ShowTicks && !ShowScaleLabels)
        {
            _scale = null;
            _scaleCulture = null;
            return null;
        }

        var key = new ScaleKey(Shape, Vertical, Min, Max, Start, End, TickInterval, MinorTickInterval,
            ShowTicks, ShowScaleLabels, TrackWidthUnits, TickGapUnits, TickLengthUnits, TickMinorLengthUnits,
            LabelSizeUnits, Format);
        var culture = ShowScaleLabels ? FormattingCulture : null;
        if (_scale is not null && (culture is null || culture.IsReadOnly)
            && ReferenceEquals(culture, _scaleCulture) && key == _scaleKey)
            return _scale;

        var marks = new List<ScaleMark>();
        foreach (var (value, major) in Ticks)
        {
            var linear = Shape == GaugeShape.Linear;
            var angle = linear ? 0 : AngleOf(value);
            var (x1, y1) = linear ? PointOn(value) : TickInner(angle);
            string? tickClass = null, endX = null, endY = null, labelX = null, labelY = null, label = null;
            if (ShowTicks)
            {
                var (x2, y2) = linear ? LinearTickEnd(x1, y1, major) : TickOuter(angle, major);
                tickClass = major ? Css.Classes.Gauge.Tick : MinorTickClass;
                endX = F(x2);
                endY = F(y2);
            }
            if (ShowScaleLabels && major)
            {
                var (lx, ly) = linear ? LinearScaleLabelPoint(x1, y1) : ScaleLabelPoint(angle);
                labelX = F(lx);
                labelY = F(ly);
                label = Display(value);
            }
            marks.Add(new(tickClass, ShowTicks ? F(x1) : null, ShowTicks ? F(y1) : null,
                endX, endY, labelX, labelY, label));
        }

        _scaleCulture = culture;
        _scaleKey = key;
        return _scale = DrawScale(marks);
    }

    private static RenderFragment DrawScale(List<ScaleMark> marks) => builder =>
    {
        foreach (var mark in marks)
        {
            if (mark.TickClass is not null)
            {
                builder.OpenElement(0, "line");
                builder.AddAttribute(1, "class", mark.TickClass);
                builder.AddAttribute(2, "x1", mark.X1);
                builder.AddAttribute(3, "y1", mark.Y1);
                builder.AddAttribute(4, "x2", mark.X2);
                builder.AddAttribute(5, "y2", mark.Y2);
                builder.CloseElement();
            }
            if (mark.Label is not null)
            {
                builder.OpenElement(6, "text");
                builder.AddAttribute(7, "class", Css.Classes.Gauge.ScaleLabel);
                builder.AddAttribute(8, "x", mark.LabelX);
                builder.AddAttribute(9, "y", mark.LabelY);
                builder.AddContent(10, mark.Label);
                builder.CloseElement();
            }
        }
    };


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
