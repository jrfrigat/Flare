using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for <c>FlareChart</c>. The categorical palette is the reason this record exists: series
/// color is an independent design axis, and a chart that borrows the semantic roles instead paints its
/// fourth series in the error color. Geometry that a renderer writes into SVG (radii in viewBox units)
/// sits beside the CSS-resolved values, because a chart draws in two languages at once.
///
/// Sizes expressed "in viewBox units" are plain numbers, not lengths: the SVG viewBox is 400 wide and the
/// component scales to its container, so a 2 here is 2/400 of the chart width at any rendered size.
/// </summary>
public sealed record ChartTokens
{
    /// <summary>First categorical series color.</summary>
    [CssVar(Chart.Series1)] public string Series1 { get; init; } = "var(--flare-color-primary)";
    /// <summary>Second categorical series color.</summary>
    [CssVar(Chart.Series2)] public string Series2 { get; init; } = "var(--flare-color-tertiary)";
    /// <summary>Third categorical series color.</summary>
    [CssVar(Chart.Series3)] public string Series3 { get; init; } = "var(--flare-color-secondary)";
    /// <summary>Fourth categorical series color.</summary>
    [CssVar(Chart.Series4)] public string Series4 { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-tertiary))";
    /// <summary>Fifth categorical series color.</summary>
    [CssVar(Chart.Series5)] public string Series5 { get; init; } = "color-mix(in srgb, var(--flare-color-secondary) 50%, var(--flare-color-tertiary))";
    /// <summary>Sixth categorical series color.</summary>
    [CssVar(Chart.Series6)] public string Series6 { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-secondary))";
    /// <summary>Seventh categorical series color.</summary>
    [CssVar(Chart.Series7)] public string Series7 { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 65%, var(--flare-color-on-surface))";
    /// <summary>Eighth categorical series color.</summary>
    [CssVar(Chart.Series8)] public string Series8 { get; init; } = "color-mix(in srgb, var(--flare-color-tertiary) 65%, var(--flare-color-on-surface))";
    /// <summary>Ninth categorical series color.</summary>
    [CssVar(Chart.Series9)] public string Series9 { get; init; } = "color-mix(in srgb, var(--flare-color-secondary) 65%, var(--flare-color-on-surface))";
    /// <summary>Tenth categorical series color.</summary>
    [CssVar(Chart.Series10)] public string Series10 { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 55%, var(--flare-color-surface))";
    /// <summary>Eleventh categorical series color.</summary>
    [CssVar(Chart.Series11)] public string Series11 { get; init; } = "color-mix(in srgb, var(--flare-color-tertiary) 55%, var(--flare-color-surface))";
    /// <summary>Twelfth categorical series color; series past this wrap back to the first.</summary>
    [CssVar(Chart.Series12)] public string Series12 { get; init; } = "color-mix(in srgb, var(--flare-color-secondary) 55%, var(--flare-color-surface))";

    /// <summary>Stroke width of a line series, in viewBox units.</summary>
    [CssVar(Chart.LineWidth)] public required string LineWidth { get; init; }
    /// <summary>Line cap and join treatment of a line series (<c>butt</c>, <c>round</c>, <c>square</c>).</summary>
    [CssVar(Chart.LineCap)] public required string LineCap { get; init; }
    /// <summary>Radius of a line marker or scatter point, in viewBox units.</summary>
    [CssVar(Chart.PointRadius)] public required string PointRadius { get; init; }
    /// <summary>Fill opacity of a marker or scatter point, so dense clouds stay readable.</summary>
    [CssVar(Chart.PointOpacity)] public required string PointOpacity { get; init; }
    /// <summary>Radius of the smallest bubble, in viewBox units.</summary>
    [CssVar(Chart.BubbleMinRadius)] public required string BubbleMinRadius { get; init; }
    /// <summary>Radius of the largest bubble, in viewBox units.</summary>
    [CssVar(Chart.BubbleMaxRadius)] public required string BubbleMaxRadius { get; init; }
    /// <summary>Corner radius of a bar or column, in viewBox units.</summary>
    [CssVar(Chart.BarRadius)] public required string BarRadius { get; init; }
    /// <summary>Opacity at the top of an area-series gradient; it fades to zero at the baseline.</summary>
    [CssVar(Chart.AreaOpacity)] public required string AreaOpacity { get; init; }
    /// <summary>Fill opacity of a radar polygon, which must stay low enough to read stacked series.</summary>
    [CssVar(Chart.RadarFillOpacity)] public required string RadarFillOpacity { get; init; }
    /// <summary>Fill opacity of a polar-area wedge.</summary>
    [CssVar(Chart.WedgeOpacity)] public required string WedgeOpacity { get; init; }
    /// <summary>Color of the separator drawn between pie, donut and rose slices.</summary>
    [CssVar(Chart.SliceStrokeColor)] public string SliceStrokeColor { get; init; } = "var(--flare-color-surface)";
    /// <summary>Width of the separator drawn between pie, donut and rose slices, in viewBox units.</summary>
    [CssVar(Chart.SliceStrokeWidth)] public required string SliceStrokeWidth { get; init; }

    /// <summary>Base color of the sequential ramp used where a value is encoded by intensity.</summary>
    [CssVar(Chart.RampColor)] public string RampColor { get; init; } = "var(--flare-color-primary)";
    /// <summary>Ramp opacity at the minimum value; the floor that keeps an empty cell visible.</summary>
    [CssVar(Chart.RampMinOpacity)] public required string RampMinOpacity { get; init; }
    /// <summary>Ramp opacity at the maximum value.</summary>
    [CssVar(Chart.RampMaxOpacity)] public required string RampMaxOpacity { get; init; }
    /// <summary>Corner radius of a heat-map cell, in viewBox units.</summary>
    [CssVar(Chart.CellRadius)] public required string CellRadius { get; init; }
    /// <summary>Gap between heat-map cells, in viewBox units.</summary>
    [CssVar(Chart.CellGap)] public required string CellGap { get; init; }
    /// <summary>Opacity of a hovered heat-map cell.</summary>
    [CssVar(Chart.CellHoverOpacity)] public required string CellHoverOpacity { get; init; }

    /// <summary>Color of the grid lines behind a cartesian or radial plot.</summary>
    [CssVar(Chart.GridColor)] public string GridColor { get; init; } = "var(--flare-color-outline-variant)";
    /// <summary>Width of the grid lines, in viewBox units.</summary>
    [CssVar(Chart.GridWidth)] public required string GridWidth { get; init; }
    /// <summary>Dash pattern of the grid lines; <c>none</c> draws them solid.</summary>
    [CssVar(Chart.GridDash)] public required string GridDash { get; init; }
    /// <summary>Color of the minor grid lines drawn between two major ones.</summary>
    [CssVar(Chart.GridMinorColor)] public string GridMinorColor { get; init; } = "color-mix(in srgb, var(--flare-color-outline-variant) 45%, transparent)";
    /// <summary>Width of the minor grid lines, in viewBox units.</summary>
    [CssVar(Chart.GridMinorWidth)] public required string GridMinorWidth { get; init; }
    /// <summary>Color of the axis tick labels.</summary>
    [CssVar(Chart.LabelColor)] public string LabelColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Font size of the axis tick labels, in viewBox units.</summary>
    [CssVar(Chart.LabelSize)] public string LabelSize { get; init; } = "var(--flare-typescale-label-small-size)";
    /// <summary>Color of a data value drawn beside its mark, against the plot background.</summary>
    [CssVar(Chart.ValueColor)] public string ValueColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Color of a data value drawn on top of its mark, against the series fill.</summary>
    [CssVar(Chart.ValueOnFillColor)] public string ValueOnFillColor { get; init; } = "var(--flare-color-surface)";
    /// <summary>Font size of the data values, in viewBox units.</summary>
    [CssVar(Chart.ValueSize)] public required string ValueSize { get; init; }
    /// <summary>Color of the axis titles.</summary>
    [CssVar(Chart.AxisTitleColor)] public string AxisTitleColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Font size of the axis titles, in viewBox units.</summary>
    [CssVar(Chart.AxisTitleSize)] public required string AxisTitleSize { get; init; }

    /// <summary>Background of the chart container.</summary>
    [CssVar(Chart.Surface)] public string Surface { get; init; } = "var(--flare-color-surface)";
    /// <summary>Corner radius of the chart container.</summary>
    [CssVar(Chart.Radius)] public string Radius { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Border width of the chart container; <c>0</c> removes the frame.</summary>
    [CssVar(Chart.BorderWidth)] public required string BorderWidth { get; init; }
    /// <summary>Border color of the chart container.</summary>
    [CssVar(Chart.BorderColor)] public string BorderColor { get; init; } = "var(--flare-color-outline-variant)";
    /// <summary>Padding inside the chart container.</summary>
    [CssVar(Chart.Padding)] public string Padding { get; init; } = "var(--flare-spacing-8)";
    /// <summary>Gap between the title, the plot and the legend.</summary>
    [CssVar(Chart.Gap)] public string Gap { get; init; } = "var(--flare-spacing-4)";

    /// <summary>Gap between legend entries.</summary>
    [CssVar(Chart.LegendGap)] public string LegendGap { get; init; } = "var(--flare-spacing-6)";
    /// <summary>Gap between a legend swatch and its label.</summary>
    [CssVar(Chart.LegendItemGap)] public string LegendItemGap { get; init; } = "var(--flare-spacing-3)";
    /// <summary>Size of the legend swatch.</summary>
    [CssVar(Chart.LegendDotSize)] public required string LegendDotSize { get; init; }
    /// <summary>Corner radius of the legend swatch; a square-swatch theme sets this to <c>0</c>.</summary>
    [CssVar(Chart.LegendDotRadius)] public string LegendDotRadius { get; init; } = "var(--flare-shape-full)";
    /// <summary>Font size of the legend labels.</summary>
    [CssVar(Chart.LegendSize)] public string LegendSize { get; init; } = "var(--flare-typescale-label-small-size)";
    /// <summary>Color of the legend labels.</summary>
    [CssVar(Chart.LegendColor)] public string LegendColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Opacity of a legend entry whose series has been toggled off.</summary>
    [CssVar(Chart.LegendOffOpacity)] public required string LegendOffOpacity { get; init; }

    /// <summary>Width of an overlaid trend line, in viewBox units.</summary>
    [CssVar(Chart.TrendWidth)] public required string TrendWidth { get; init; }
    /// <summary>Dash pattern of an overlaid trend line.</summary>
    [CssVar(Chart.TrendDash)] public required string TrendDash { get; init; }
    /// <summary>Opacity of an overlaid trend line, which sits behind the data it summarizes.</summary>
    [CssVar(Chart.TrendOpacity)] public required string TrendOpacity { get; init; }
    /// <summary>Default color of threshold and band annotations, overridable per annotation.</summary>
    [CssVar(Chart.AnnotationColor)] public string AnnotationColor { get; init; } = "var(--flare-color-error)";
    /// <summary>Width of an annotation line, in viewBox units.</summary>
    [CssVar(Chart.AnnotationWidth)] public required string AnnotationWidth { get; init; }
    /// <summary>Dash pattern of an annotation line.</summary>
    [CssVar(Chart.AnnotationDash)] public required string AnnotationDash { get; init; }
    /// <summary>Fill opacity of an annotation band.</summary>
    [CssVar(Chart.AnnotationBandOpacity)] public required string AnnotationBandOpacity { get; init; }

    /// <summary>Dash array behind <c>ChartLineStyle.Dashed</c>.</summary>
    [CssVar(Chart.LineDashDashed)] public required string LineDashDashed { get; init; }
    /// <summary>Dash array behind <c>ChartLineStyle.Dotted</c>.</summary>
    [CssVar(Chart.LineDashDotted)] public required string LineDashDotted { get; init; }
    /// <summary>Dash array behind <c>ChartLineStyle.DashDot</c>.</summary>
    [CssVar(Chart.LineDashDashDot)] public required string LineDashDashDot { get; init; }

    /// <summary>Arrowhead length of a directional annotation, in viewBox units.</summary>
    [CssVar(Chart.AnnotationArrowSize)] public required string AnnotationArrowSize { get; init; }
    /// <summary>Marker radius of a point annotation, in viewBox units.</summary>
    [CssVar(Chart.AnnotationPointRadius)] public required string AnnotationPointRadius { get; init; }

    /// <summary>Fill of the drag-to-zoom selection rectangle.</summary>
    [CssVar(Chart.ZoomSelectionFill)] public string ZoomSelectionFill { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)";
    /// <summary>Stroke of the drag-to-zoom selection rectangle.</summary>
    [CssVar(Chart.ZoomSelectionStroke)] public string ZoomSelectionStroke { get; init; } = "var(--flare-color-primary)";
}
