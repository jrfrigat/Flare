namespace Flare.Components;

/// <summary>The kind of chart <see cref="FlareChart"/> renders from its data.</summary>
public enum ChartType
{
    /// <summary>Line chart connecting data points.</summary>
    Line,
    /// <summary>Vertical bar chart (grouped when multiple series).</summary>
    Bar,
    /// <summary>Pie chart (full circle of proportional slices).</summary>
    Pie,
    /// <summary>Donut chart (pie with a hollow center).</summary>
    Donut,
    /// <summary>Line chart with the area under each line filled (shorthand for a line with <c>Area</c>).</summary>
    Area,
    /// <summary>Bar chart with series stacked on top of each other within each category.</summary>
    StackedBar,
    /// <summary>Scatter plot of independent X/Y points (uses <see cref="ChartSeries.Points"/>).</summary>
    Scatter,
    /// <summary>Radar (spider) chart: each category is an axis radiating from the center.</summary>
    Radar,
    /// <summary>Heat map: a colored grid where each series is a row and each label is a column.</summary>
    HeatMap,
    /// <summary>Bubble chart: X/Y points sized by <see cref="ChartPoint.R"/>.</summary>
    Bubble,
    /// <summary>Rose (Nightingale) chart: equal-angle sectors whose radius encodes the value.</summary>
    Rose,
    /// <summary>Polar-area chart: equal-angle wedges on a radial grid, radius encodes the value.</summary>
    PolarArea,
    /// <summary>Combo chart: each series draws as its own <see cref="ChartSeries.Kind"/> (bar / line / area).</summary>
    Combo,
}

/// <summary>How a single series draws in a <see cref="ChartType.Combo"/> chart.</summary>
public enum ChartSeriesKind
{
    /// <summary>Draw as vertical bars (the default).</summary>
    Bar,
    /// <summary>Draw as a line.</summary>
    Line,
    /// <summary>Draw as a filled area line.</summary>
    Area,
}

/// <summary>
/// Stroke pattern of a line, trend line or annotation. The dash arrays behind the patterned values are
/// theme tokens (<c>--flare-chart-line-dash-*</c>), so a theme retunes them without touching a call site.
/// </summary>
public enum ChartLineStyle
{
    /// <summary>Unbroken stroke.</summary>
    Solid,
    /// <summary>Evenly spaced dashes.</summary>
    Dashed,
    /// <summary>Round-ended dots.</summary>
    Dotted,
    /// <summary>Alternating dash and dot.</summary>
    DashDot,
}

/// <summary>The kind of overlay a <see cref="ChartAnnotation"/> draws.</summary>
public enum ChartAnnotationKind
{
    /// <summary>A horizontal line at <see cref="ChartAnnotation.Y"/> (a target or threshold).</summary>
    HorizontalLine,
    /// <summary>A vertical line at <see cref="ChartAnnotation.X"/> (category index, or X value on scatter).</summary>
    VerticalLine,
    /// <summary>A shaded band between <see cref="ChartAnnotation.Y"/> and <see cref="ChartAnnotation.Y2"/>.</summary>
    HorizontalBand,
    /// <summary>A shaded band between <see cref="ChartAnnotation.X"/> and <see cref="ChartAnnotation.X2"/>.</summary>
    VerticalBand,
    /// <summary>A free line from (X,Y) to (X2,Y2) in data coordinates.</summary>
    Segment,
    /// <summary>A <see cref="Segment"/> with an arrowhead at (X2,Y2) - a trend or a callout leader.</summary>
    Arrow,
    /// <summary>A marked point at (X,Y) with an optional label.</summary>
    Point,
}

/// <summary>What the plot does when a series is switched off in the legend.</summary>
public enum ChartScaleMode
{
    /// <summary>
    /// The plot refits to what is left: the value axis is measured from the visible series, its grid
    /// lines are recomputed, and a grouped bar chart closes the gap the hidden series left. Every
    /// remaining mark moves (the default).
    /// </summary>
    FitVisible,
    /// <summary>
    /// The plot holds still: the value axis spans every series, hidden ones included, its grid lines
    /// stay where the full data put them, and a grouped bar chart keeps each series' slot. Switching a
    /// series off then removes that series and nothing else - which is what the reader asked for when
    /// they clicked the legend to see the rest without it.
    /// </summary>
    FitAll,
}

/// <summary>Which side of the data an annotation is drawn on.</summary>
public enum ChartAnnotationLayer
{
    /// <summary>Over the series, above lines, bars and markers (the default).</summary>
    Over,
    /// <summary>Under the series, over the plot background and the grid. What a shaded period marker
    /// wants: a translucent band over the data washes out the values it is meant to frame, while the
    /// same band behind them reads as the background of the period.</summary>
    Under,
}

/// <summary>Where an annotation's label sits relative to the annotation itself.</summary>
public enum ChartAnnotationLabelPosition
{
    /// <summary>Chosen from the annotation kind: outside-end for lines, above for points and segments.</summary>
    Auto,
    /// <summary>At the annotation's start (leading edge / first point).</summary>
    Start,
    /// <summary>At the annotation's end (trailing edge / last point).</summary>
    End,
}

/// <summary>
/// An overlay drawn over a cartesian chart in DATA coordinates: a threshold, a target, a shaded range, a
/// directional segment or arrow, or a labelled point. Build one through a factory
/// (<see cref="Threshold"/>, <see cref="Band"/>, <see cref="Arrow"/>, ...) rather than by hand - the
/// factories name which coordinates each kind reads.
/// </summary>
public sealed record ChartAnnotation
{
    /// <summary>Which overlay to draw; decides which of the coordinates below are read.</summary>
    public required ChartAnnotationKind Kind { get; init; }

    /// <summary>Primary X: a category index on a category chart, an X value on a scatter/bubble chart.</summary>
    public double X { get; init; }

    /// <summary>Primary Y, on the value axis.</summary>
    public double Y { get; init; }

    /// <summary>Second X, for <see cref="ChartAnnotationKind.VerticalBand"/>, <see cref="ChartAnnotationKind.Segment"/>
    /// and <see cref="ChartAnnotationKind.Arrow"/>.</summary>
    public double X2 { get; init; }

    /// <summary>Second Y, for <see cref="ChartAnnotationKind.HorizontalBand"/>, <see cref="ChartAnnotationKind.Segment"/>
    /// and <see cref="ChartAnnotationKind.Arrow"/>.</summary>
    public double Y2 { get; init; }

    /// <summary>Optional text drawn next to the annotation.</summary>
    public string? Label { get; init; }

    /// <summary>Where the label sits. Default <see cref="ChartAnnotationLabelPosition.Auto"/>.</summary>
    public ChartAnnotationLabelPosition LabelPosition { get; init; }

    /// <summary>Annotation color. Left at <see cref="FlareColor.Default"/> the theme's annotation color is used.</summary>
    public FlareColor Color { get; init; }

    /// <summary>Stroke pattern of the line kinds. Null uses the theme's annotation dash.</summary>
    public ChartLineStyle? LineStyle { get; init; }

    /// <summary>Whether the annotation draws over the series or behind them. Default
    /// <see cref="ChartAnnotationLayer.Over"/>; the band kinds usually want
    /// <see cref="ChartAnnotationLayer.Under"/>. The label follows the annotation onto its layer.</summary>
    public ChartAnnotationLayer Layer { get; init; }

    /// <summary>A horizontal threshold / target line at <paramref name="y"/>.</summary>
    /// <param name="y">Value-axis position of the line.</param>
    /// <param name="label">Optional text drawn at the line's end.</param>
    /// <param name="color">Line color; default uses the theme's annotation color.</param>
    public static ChartAnnotation Threshold(double y, string? label = null, FlareColor color = default) =>
        new() { Kind = ChartAnnotationKind.HorizontalLine, Y = y, Label = label, Color = color };

    /// <summary>A vertical marker line at category index (or scatter X) <paramref name="x"/>.</summary>
    /// <param name="x">Category index, or X value on a scatter/bubble chart.</param>
    /// <param name="label">Optional text drawn beside the line.</param>
    /// <param name="color">Line color; default uses the theme's annotation color.</param>
    public static ChartAnnotation Marker(double x, string? label = null, FlareColor color = default) =>
        new() { Kind = ChartAnnotationKind.VerticalLine, X = x, Label = label, Color = color };

    /// <summary>A shaded horizontal band between two values on the value axis.</summary>
    /// <param name="from">First value-axis bound.</param>
    /// <param name="to">Second value-axis bound.</param>
    /// <param name="label">Optional text drawn at the band's top edge.</param>
    /// <param name="color">Fill color; default uses the theme's annotation color.</param>
    /// <param name="layer">Draw over the series (default) or behind them.</param>
    public static ChartAnnotation Band(
        double from, double to, string? label = null, FlareColor color = default,
        ChartAnnotationLayer layer = ChartAnnotationLayer.Over) =>
        new() { Kind = ChartAnnotationKind.HorizontalBand, Y = from, Y2 = to, Label = label, Color = color, Layer = layer };

    /// <summary>A shaded vertical band between two category indices (or scatter X values).</summary>
    /// <param name="from">First category index / X value.</param>
    /// <param name="to">Second category index / X value.</param>
    /// <param name="label">Optional text drawn at the band's leading edge.</param>
    /// <param name="color">Fill color; default uses the theme's annotation color.</param>
    /// <param name="layer">Draw over the series (default) or behind them.</param>
    public static ChartAnnotation VerticalBand(
        double from, double to, string? label = null, FlareColor color = default,
        ChartAnnotationLayer layer = ChartAnnotationLayer.Over) =>
        new() { Kind = ChartAnnotationKind.VerticalBand, X = from, X2 = to, Label = label, Color = color, Layer = layer };

    /// <summary>A free line between two data points.</summary>
    /// <param name="x1">Start category index / X value.</param>
    /// <param name="y1">Start value.</param>
    /// <param name="x2">End category index / X value.</param>
    /// <param name="y2">End value.</param>
    /// <param name="label">Optional text drawn at the end point.</param>
    /// <param name="color">Line color; default uses the theme's annotation color.</param>
    public static ChartAnnotation Segment(
        double x1, double y1, double x2, double y2, string? label = null, FlareColor color = default) =>
        new() { Kind = ChartAnnotationKind.Segment, X = x1, Y = y1, X2 = x2, Y2 = y2, Label = label, Color = color };

    /// <summary>A directional line with an arrowhead at the end point - a trend or a callout leader.</summary>
    /// <param name="x1">Start category index / X value.</param>
    /// <param name="y1">Start value.</param>
    /// <param name="x2">End category index / X value, where the arrowhead is drawn.</param>
    /// <param name="y2">End value.</param>
    /// <param name="label">Optional text drawn at the arrowhead.</param>
    /// <param name="color">Line color; default uses the theme's annotation color.</param>
    public static ChartAnnotation Arrow(
        double x1, double y1, double x2, double y2, string? label = null, FlareColor color = default) =>
        new() { Kind = ChartAnnotationKind.Arrow, X = x1, Y = y1, X2 = x2, Y2 = y2, Label = label, Color = color };

    /// <summary>A marked, labelled point at a data coordinate.</summary>
    /// <param name="x">Category index / X value.</param>
    /// <param name="y">Value.</param>
    /// <param name="label">Optional text drawn above the point.</param>
    /// <param name="color">Marker color; default uses the theme's annotation color.</param>
    public static ChartAnnotation At(double x, double y, string? label = null, FlareColor color = default) =>
        new() { Kind = ChartAnnotationKind.Point, X = x, Y = y, Label = label, Color = color };
}

/// <summary>A single X/Y point for a scatter or bubble series.</summary>
/// <param name="X">The horizontal value.</param>
/// <param name="Y">The vertical value.</param>
/// <param name="R">The bubble radius weight (bubble charts only); ignored by scatter.</param>
public readonly record struct ChartPoint(double X, double Y, double R = 0);

/// <summary>Where the legend sits relative to the plot in <see cref="FlareChart"/>.</summary>
public enum ChartLegendPosition
{
    /// <summary>Below the plot (the default).</summary>
    Bottom,
    /// <summary>Above the plot.</summary>
    Top,
    /// <summary>To the left of the plot.</summary>
    Left,
    /// <summary>To the right of the plot.</summary>
    Right,
    /// <summary>No legend.</summary>
    None,
}

/// <summary>
/// The visible horizontal window of a zoomable chart, in the chart's own X units - category index for a
/// category chart, X value for a scatter/bubble chart. A window of the full domain is "zoomed out".
/// </summary>
/// <param name="From">Left edge of the visible window.</param>
/// <param name="To">Right edge of the visible window.</param>
public readonly record struct ChartZoom(double From, double To)
{
    /// <summary>The window's width; zero or negative means the window is empty.</summary>
    public double Span => To - From;

    /// <summary>Returns the window clamped inside <paramref name="min"/>..<paramref name="max"/>, never
    /// narrower than <paramref name="minSpan"/>.</summary>
    /// <param name="min">Lower bound of the full domain.</param>
    /// <param name="max">Upper bound of the full domain.</param>
    /// <param name="minSpan">Narrowest window allowed, so a zoom cannot collapse to a point.</param>
    public ChartZoom Clamp(double min, double max, double minSpan)
    {
        var span = Math.Max(Math.Min(Span, max - min), minSpan);
        var from = Math.Clamp(From, min, Math.Max(min, max - span));
        return new ChartZoom(from, from + span);
    }
}

/// <summary>A single named data series plotted on a chart.</summary>
/// <param name="Label">Series name shown in the legend and tooltips.</param>
/// <param name="Values">The numeric values, one per category (line/bar/pie/radar).
/// <see cref="double.NaN"/> marks a MISSING value, which is not the same as zero: the line breaks
/// rather than dropping to the axis, no bar is drawn, the point is left out of the axis range and out
/// of the tooltip. That is what lets a series live on part of a shared category axis - a seasonal
/// product against twelve months - without a run of zeros reading as "sold none". Build the list from
/// nullable data with <see cref="ChartSeries.Gaps"/>. Gaps apply to the cartesian types (line, area,
/// bar, stacked bar, combo); the radial types treat a gap as zero.</param>
/// <param name="Color">Explicit series color - a semantic role (<c>FlareColor.Error</c>) or any CSS color
/// string, which converts implicitly. Left at <see cref="FlareColor.Default"/> the series takes the next
/// color from the theme's categorical palette.</param>
/// <param name="Points">X/Y points for a <see cref="ChartType.Scatter"/> series (used instead of <paramref name="Values"/>).</param>
/// <param name="Kind">In a <see cref="ChartType.Combo"/> chart, how this series draws (bar/line/area);
/// null defaults to bar.</param>
/// <param name="Smooth">Draws this line series as a smooth curve. Null falls back to the chart's
/// <c>Smooth</c>, so one chart can mix straight and smoothed lines.</param>
/// <param name="Area">Fills under this line series. Null falls back to the chart's <c>Area</c>.</param>
/// <param name="LineStyle">Stroke pattern of this line series. Null falls back to the chart's <c>LineStyle</c>.</param>
/// <param name="ShowMarkers">Draws a marker dot at each of this series' points. Null falls back to the
/// chart's <c>ShowMarkers</c>.</param>
public sealed record ChartSeries(
    string Label,
    IReadOnlyList<double> Values,
    FlareColor Color = default,
    IReadOnlyList<ChartPoint>? Points = null,
    ChartSeriesKind? Kind = null,
    bool? Smooth = null,
    bool? Area = null,
    ChartLineStyle? LineStyle = null,
    bool? ShowMarkers = null)
{
    /// <summary>
    /// Converts nullable data into the value list a series takes, turning every <c>null</c> into the
    /// <see cref="double.NaN"/> a chart reads as a gap. Lets a partial series be written the way it is
    /// held - <c>[null, null, 12, 18, null]</c> - instead of hand-encoding the missing points.
    /// </summary>
    /// <param name="values">Values in category order; <c>null</c> where the series has no value.</param>
    /// <returns>The same values with <c>null</c> replaced by <see cref="double.NaN"/>.</returns>
    public static IReadOnlyList<double> Gaps(IReadOnlyList<double?> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var result = new double[values.Count];
        for (int i = 0; i < values.Count; i++) result[i] = values[i] ?? double.NaN;
        return result;
    }
}

/// <summary>The data plotted by <see cref="FlareChart"/>: one or more series and optional category labels.</summary>
/// <param name="Series">The series to plot.</param>
/// <param name="Labels">Optional category (x-axis) labels shared by all series.</param>
public sealed record ChartData(
    IReadOnlyList<ChartSeries> Series,
    IReadOnlyList<string>? Labels = null);
