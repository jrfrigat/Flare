using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsBarDemo
{
    private static readonly ChartData _barData = new(
        [
            new ChartSeries("2024", [45, 62, 58, 71]),
            new ChartSeries("2025", [52, 70, 65, 84])
        ],
        ["Q1", "Q2", "Q3", "Q4"]);
}