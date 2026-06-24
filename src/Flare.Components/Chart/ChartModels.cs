namespace Flare.Components;

/// <summary>The kind of chart <see cref="FlareChart"/> renders from its data.</summary>
public enum ChartType
{
    /// <summary>Line chart connecting data points.</summary>
    Line,
    /// <summary>Vertical bar chart.</summary>
    Bar,
    /// <summary>Pie chart (full circle of proportional slices).</summary>
    Pie,
    /// <summary>Donut chart (pie with a hollow center).</summary>
    Donut,
}

/// <summary>A single named data series plotted on a chart.</summary>
/// <param name="Label">Series name shown in the legend and tooltips.</param>
/// <param name="Values">The numeric values, one per category.</param>
/// <param name="Color">Optional explicit color (any CSS color); when null a themed palette color is assigned.</param>
public sealed record ChartSeries(string Label, IReadOnlyList<double> Values, string? Color = null);

/// <summary>The data plotted by <see cref="FlareChart"/>: one or more series and optional category labels.</summary>
/// <param name="Series">The series to plot.</param>
/// <param name="Labels">Optional category (x-axis) labels shared by all series.</param>
public sealed record ChartData(
    IReadOnlyList<ChartSeries> Series,
    IReadOnlyList<string>? Labels = null);
