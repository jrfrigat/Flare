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
}

/// <summary>A single X/Y point for a scatter series.</summary>
/// <param name="X">The horizontal value.</param>
/// <param name="Y">The vertical value.</param>
public readonly record struct ChartPoint(double X, double Y);

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

/// <summary>A single named data series plotted on a chart.</summary>
/// <param name="Label">Series name shown in the legend and tooltips.</param>
/// <param name="Values">The numeric values, one per category (line/bar/pie/radar).</param>
/// <param name="Color">Optional explicit color (any CSS color); when null a themed palette color is assigned.</param>
/// <param name="Points">X/Y points for a <see cref="ChartType.Scatter"/> series (used instead of <paramref name="Values"/>).</param>
public sealed record ChartSeries(
    string Label,
    IReadOnlyList<double> Values,
    string? Color = null,
    IReadOnlyList<ChartPoint>? Points = null);

/// <summary>The data plotted by <see cref="FlareChart"/>: one or more series and optional category labels.</summary>
/// <param name="Series">The series to plot.</param>
/// <param name="Labels">Optional category (x-axis) labels shared by all series.</param>
public sealed record ChartData(
    IReadOnlyList<ChartSeries> Series,
    IReadOnlyList<string>? Labels = null);
