using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsSparklineDemo
{
    private static readonly ChartData _lineData = new(
        [new ChartSeries("Revenue", [12, 19, 15, 28, 34, 45, 38, 52, 61, 58, 70, 84])],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]);

    private static readonly ChartData _spark1 = new(
        [new ChartSeries("Throughput", [8, 10, 9, 13, 12, 16, 14, 19, 22, 18, 24, 27, 25, 30], "var(--flare-color-primary)")]);

    private static readonly ChartData _spark2 = new(
        [new ChartSeries("Latency", [52, 48, 55, 44, 47, 41, 45, 39, 42, 46, 40, 43, 38, 42], "var(--flare-color-tertiary)")]);
}
