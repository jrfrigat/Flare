using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsLineStylesDemo
{
    private static readonly string[] _months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun"];

    private static readonly ChartData _mixed = new(
        [
            new ChartSeries("Actual", [24, 38, 31, 45, 52, 40], Smooth: true, Area: true),
            new ChartSeries("Plan", [28, 34, 36, 40, 46, 48], LineStyle: ChartLineStyle.Dashed),
            new ChartSeries("Last year", [20, 26, 30, 33, 38, 36], Smooth: true,
                LineStyle: ChartLineStyle.Dotted, ShowMarkers: true),
            new ChartSeries("Forecast", [26, 32, 34, 42, 50, 55], LineStyle: ChartLineStyle.DashDot),
        ],
        _months);

    private static readonly ChartData _styles = new(
        [
            new ChartSeries("Solid", [10, 14, 12, 18, 16, 20]),
            new ChartSeries("Dashed", [16, 20, 18, 24, 22, 26], LineStyle: ChartLineStyle.Dashed),
            new ChartSeries("Dotted", [22, 26, 24, 30, 28, 32], LineStyle: ChartLineStyle.Dotted),
            new ChartSeries("DashDot", [28, 32, 30, 36, 34, 38], LineStyle: ChartLineStyle.DashDot),
        ],
        _months);
}
