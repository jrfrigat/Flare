namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for <c>FlareChart</c>. Charts carry a token surface no other component needs: a
/// CATEGORICAL palette. Series color is a different design axis from brand color - a theme that wants six
/// distinguishable series cannot express that by re-pointing the primary role, and reusing the semantic
/// roles for it means the fourth series is drawn in the error color. The palette lives here so the theme
/// owns it, and the sequential ramp beside it serves the value-encoded surfaces (heat map cells today,
/// treemap and contour later).
/// </summary>
public static class Chart
{
    /// <summary>Number of categorical series colors a theme supplies; series past this wrap around.</summary>
    public const int SeriesColorCount = 12;

    /// <summary>CSS custom-property name for categorical series color 1.</summary>
    public const string Series1 = "--flare-chart-series-1";
    /// <summary>CSS custom-property name for categorical series color 2.</summary>
    public const string Series2 = "--flare-chart-series-2";
    /// <summary>CSS custom-property name for categorical series color 3.</summary>
    public const string Series3 = "--flare-chart-series-3";
    /// <summary>CSS custom-property name for categorical series color 4.</summary>
    public const string Series4 = "--flare-chart-series-4";
    /// <summary>CSS custom-property name for categorical series color 5.</summary>
    public const string Series5 = "--flare-chart-series-5";
    /// <summary>CSS custom-property name for categorical series color 6.</summary>
    public const string Series6 = "--flare-chart-series-6";
    /// <summary>CSS custom-property name for categorical series color 7.</summary>
    public const string Series7 = "--flare-chart-series-7";
    /// <summary>CSS custom-property name for categorical series color 8.</summary>
    public const string Series8 = "--flare-chart-series-8";
    /// <summary>CSS custom-property name for categorical series color 9.</summary>
    public const string Series9 = "--flare-chart-series-9";
    /// <summary>CSS custom-property name for categorical series color 10.</summary>
    public const string Series10 = "--flare-chart-series-10";
    /// <summary>CSS custom-property name for categorical series color 11.</summary>
    public const string Series11 = "--flare-chart-series-11";
    /// <summary>CSS custom-property name for categorical series color 12.</summary>
    public const string Series12 = "--flare-chart-series-12";

    // Indexable view of the palette. The component walks it by series index, so the wrap-around lives in
    // one place instead of in every renderer.
    private static readonly string[] _series =
    [
        Series1, Series2, Series3, Series4, Series5, Series6,
        Series7, Series8, Series9, Series10, Series11, Series12,
    ];

    /// <summary>
    /// The categorical series-color variable NAME for a zero-based series index, wrapping after
    /// <see cref="SeriesColorCount"/>.
    /// </summary>
    /// <param name="index">Zero-based series index; negative values are treated as 0.</param>
    /// <returns>The <c>--flare-chart-series-N</c> custom-property name.</returns>
    public static string Series(int index) =>
        _series[(index < 0 ? 0 : index) % _series.Length];

    /// <summary>
    /// The <c>var(--flare-chart-series-N)</c> REFERENCE for a zero-based series index, ready to drop into
    /// a style value.
    /// </summary>
    /// <param name="index">Zero-based series index; negative values are treated as 0.</param>
    /// <returns>A <c>var()</c> reference to the series color.</returns>
    public static string SeriesVar(int index) => $"var({Series(index)})";

    /// <summary>CSS custom-property name for the stroke width of line series.</summary>
    public const string LineWidth = "--flare-chart-line-width";
    /// <summary>CSS custom-property name for the line cap of line series (butt/round/square).</summary>
    public const string LineCap = "--flare-chart-line-cap";
    /// <summary>CSS custom-property name for the marker/scatter point radius, in viewBox units.</summary>
    public const string PointRadius = "--flare-chart-point-radius";
    /// <summary>CSS custom-property name for the marker/scatter point fill opacity.</summary>
    public const string PointOpacity = "--flare-chart-point-opacity";
    /// <summary>CSS custom-property name for the smallest bubble radius, in viewBox units.</summary>
    public const string BubbleMinRadius = "--flare-chart-bubble-min-radius";
    /// <summary>CSS custom-property name for the largest bubble radius, in viewBox units.</summary>
    public const string BubbleMaxRadius = "--flare-chart-bubble-max-radius";
    /// <summary>CSS custom-property name for the bar corner radius, in viewBox units.</summary>
    public const string BarRadius = "--flare-chart-bar-radius";
    /// <summary>CSS custom-property name for the opacity at the top of an area-series gradient.</summary>
    public const string AreaOpacity = "--flare-chart-area-opacity";
    /// <summary>CSS custom-property name for the fill opacity of a radar polygon.</summary>
    public const string RadarFillOpacity = "--flare-chart-radar-fill-opacity";
    /// <summary>CSS custom-property name for the fill opacity of a polar-area wedge.</summary>
    public const string WedgeOpacity = "--flare-chart-wedge-opacity";
    /// <summary>CSS custom-property name for the color separating pie/donut/rose slices.</summary>
    public const string SliceStrokeColor = "--flare-chart-slice-stroke-color";
    /// <summary>CSS custom-property name for the width separating pie/donut/rose slices.</summary>
    public const string SliceStrokeWidth = "--flare-chart-slice-stroke-width";

    /// <summary>CSS custom-property name for the base color of the sequential (value-encoded) ramp.</summary>
    public const string RampColor = "--flare-chart-ramp-color";
    /// <summary>CSS custom-property name for the ramp opacity at the minimum value.</summary>
    public const string RampMinOpacity = "--flare-chart-ramp-min-opacity";
    /// <summary>CSS custom-property name for the ramp opacity at the maximum value.</summary>
    public const string RampMaxOpacity = "--flare-chart-ramp-max-opacity";
    /// <summary>CSS custom-property name for the heat-map cell corner radius, in viewBox units.</summary>
    public const string CellRadius = "--flare-chart-cell-radius";
    /// <summary>CSS custom-property name for the gap between heat-map cells, in viewBox units.</summary>
    public const string CellGap = "--flare-chart-cell-gap";
    /// <summary>CSS custom-property name for the opacity of a hovered heat-map cell.</summary>
    public const string CellHoverOpacity = "--flare-chart-cell-hover-opacity";

    /// <summary>CSS custom-property name for the grid-line color.</summary>
    public const string GridColor = "--flare-chart-grid-color";
    /// <summary>CSS custom-property name for the grid-line width.</summary>
    public const string GridWidth = "--flare-chart-grid-width";
    /// <summary>CSS custom-property name for the grid-line dash pattern (<c>none</c> for solid).</summary>
    public const string GridDash = "--flare-chart-grid-dash";
    /// <summary>CSS custom-property name for the axis tick-label color.</summary>
    public const string LabelColor = "--flare-chart-label-color";
    /// <summary>CSS custom-property name for the axis tick-label font size, in viewBox units.</summary>
    public const string LabelSize = "--flare-chart-label-size";
    /// <summary>CSS custom-property name for the color of a data value drawn outside its mark.</summary>
    public const string ValueColor = "--flare-chart-value-color";
    /// <summary>CSS custom-property name for the color of a data value drawn on top of its mark.</summary>
    public const string ValueOnFillColor = "--flare-chart-value-on-fill-color";
    /// <summary>CSS custom-property name for the data-value font size, in viewBox units.</summary>
    public const string ValueSize = "--flare-chart-value-size";
    /// <summary>CSS custom-property name for the axis-title color.</summary>
    public const string AxisTitleColor = "--flare-chart-axis-title-color";
    /// <summary>CSS custom-property name for the axis-title font size, in viewBox units.</summary>
    public const string AxisTitleSize = "--flare-chart-axis-title-size";

    /// <summary>CSS custom-property name for the chart container background.</summary>
    public const string Surface = "--flare-chart-surface";
    /// <summary>CSS custom-property name for the chart container corner radius.</summary>
    public const string Radius = "--flare-chart-radius";
    /// <summary>CSS custom-property name for the chart container border width.</summary>
    public const string BorderWidth = "--flare-chart-border-width";
    /// <summary>CSS custom-property name for the chart container border color.</summary>
    public const string BorderColor = "--flare-chart-border-color";
    /// <summary>CSS custom-property name for the chart container padding.</summary>
    public const string Padding = "--flare-chart-padding";
    /// <summary>CSS custom-property name for the gap between title, plot and legend.</summary>
    public const string Gap = "--flare-chart-gap";

    /// <summary>CSS custom-property name for the gap between legend entries.</summary>
    public const string LegendGap = "--flare-chart-legend-gap";
    /// <summary>CSS custom-property name for the gap between a legend swatch and its label.</summary>
    public const string LegendItemGap = "--flare-chart-legend-item-gap";
    /// <summary>CSS custom-property name for the legend swatch size.</summary>
    public const string LegendDotSize = "--flare-chart-legend-dot-size";
    /// <summary>CSS custom-property name for the legend swatch corner radius (square swatches use 0).</summary>
    public const string LegendDotRadius = "--flare-chart-legend-dot-radius";
    /// <summary>CSS custom-property name for the legend label font size.</summary>
    public const string LegendSize = "--flare-chart-legend-size";
    /// <summary>CSS custom-property name for the legend label color.</summary>
    public const string LegendColor = "--flare-chart-legend-color";
    /// <summary>CSS custom-property name for the opacity of a legend entry whose series is toggled off.</summary>
    public const string LegendOffOpacity = "--flare-chart-legend-off-opacity";

    /// <summary>CSS custom-property name for the trend-line width.</summary>
    public const string TrendWidth = "--flare-chart-trend-width";
    /// <summary>CSS custom-property name for the trend-line dash pattern.</summary>
    public const string TrendDash = "--flare-chart-trend-dash";
    /// <summary>CSS custom-property name for the trend-line opacity.</summary>
    public const string TrendOpacity = "--flare-chart-trend-opacity";
    /// <summary>CSS custom-property name for the default annotation color (overridable per annotation).</summary>
    public const string AnnotationColor = "--flare-chart-annotation-color";
    /// <summary>CSS custom-property name for the annotation line width.</summary>
    public const string AnnotationWidth = "--flare-chart-annotation-width";
    /// <summary>CSS custom-property name for the annotation line dash pattern.</summary>
    public const string AnnotationDash = "--flare-chart-annotation-dash";
    /// <summary>CSS custom-property name for the fill opacity of an annotation band.</summary>
    public const string AnnotationBandOpacity = "--flare-chart-annotation-band-opacity";

    /// <summary>CSS custom-property name for the dash array of a <c>Dashed</c> line.</summary>
    public const string LineDashDashed = "--flare-chart-line-dash-dashed";
    /// <summary>CSS custom-property name for the dash array of a <c>Dotted</c> line.</summary>
    public const string LineDashDotted = "--flare-chart-line-dash-dotted";
    /// <summary>CSS custom-property name for the dash array of a <c>DashDot</c> line.</summary>
    public const string LineDashDashDot = "--flare-chart-line-dash-dash-dot";

    /// <summary>CSS custom-property name for the arrowhead length of a directional annotation, in viewBox units.</summary>
    public const string AnnotationArrowSize = "--flare-chart-annotation-arrow-size";
    /// <summary>CSS custom-property name for the radius of a point annotation's marker, in viewBox units.</summary>
    public const string AnnotationPointRadius = "--flare-chart-annotation-point-radius";

    /// <summary>CSS custom-property name for the fill of the drag-to-zoom selection rectangle.</summary>
    public const string ZoomSelectionFill = "--flare-chart-zoom-selection-fill";
    /// <summary>CSS custom-property name for the stroke of the drag-to-zoom selection rectangle.</summary>
    public const string ZoomSelectionStroke = "--flare-chart-zoom-selection-stroke";
}
