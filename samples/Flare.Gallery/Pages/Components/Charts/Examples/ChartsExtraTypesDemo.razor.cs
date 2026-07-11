using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsExtraTypesDemo
{
    private static readonly ChartData _combo = new(
        [
            new ChartSeries("Volume", [22, 34, 28, 41, 37, 45], Kind: ChartSeriesKind.Bar),
            new ChartSeries("Avg", [26, 30, 32, 35, 38, 40], Kind: ChartSeriesKind.Line),
        ],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun"]);

    private static readonly ChartAnnotation[] _targets =
    [
        new(ChartAnnotationKind.HorizontalLine, 42, Label: "Target"),
    ];

    private static readonly ChartData _radial = new(
        [
            new ChartSeries("Speed", [8]),
            new ChartSeries("Power", [6]),
            new ChartSeries("Range", [9]),
            new ChartSeries("Comfort", [5]),
            new ChartSeries("Economy", [7]),
        ]);

    private static readonly ChartData _bubble = new(
        [
            new ChartSeries("Deals", System.Array.Empty<double>(), Points:
            [
                new ChartPoint(2, 4, 3), new ChartPoint(4, 7, 9), new ChartPoint(6, 5, 5),
                new ChartPoint(7, 9, 12), new ChartPoint(9, 6, 6), new ChartPoint(5, 3, 2),
            ]),
        ]);

    private static readonly ChartData _trend = new(
        [new ChartSeries("Signups", [8, 14, 11, 19, 16, 24, 21, 29, 26, 34])],
        ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10"]);
}
