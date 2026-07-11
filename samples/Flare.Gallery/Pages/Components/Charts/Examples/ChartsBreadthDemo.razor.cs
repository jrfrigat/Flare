using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsBreadthDemo
{
    private static readonly ChartData _stacked = new(
        [
            new ChartSeries("Direct", [12, 19, 15, 22]),
            new ChartSeries("Referral", [8, 11, 14, 9]),
            new ChartSeries("Organic", [15, 13, 18, 24]),
        ],
        ["Q1", "Q2", "Q3", "Q4"]);

    private static readonly ChartData _scatter = new(
        [
            new ChartSeries("Group A", System.Array.Empty<double>(), Points:
            [
                new ChartPoint(1, 4), new ChartPoint(2, 6), new ChartPoint(3, 5),
                new ChartPoint(4, 9), new ChartPoint(5, 7), new ChartPoint(6, 11),
            ]),
            new ChartSeries("Group B", System.Array.Empty<double>(), Points:
            [
                new ChartPoint(1, 8), new ChartPoint(2, 5), new ChartPoint(3, 9),
                new ChartPoint(4, 4), new ChartPoint(5, 10), new ChartPoint(6, 6),
            ]),
        ]);

    private static readonly ChartData _radar = new(
        [
            new ChartSeries("Model X", [8, 6, 9, 5, 7]),
            new ChartSeries("Model Y", [6, 9, 5, 8, 6]),
        ],
        ["Speed", "Power", "Range", "Comfort", "Economy"]);
}
