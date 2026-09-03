using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsAnnotationsDemo
{
    private static readonly ChartData _data = new(
        [new ChartSeries("Latency", [120, 138, 131, 205, 252, 190, 158, 142])],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"]);

    private static readonly ChartAnnotation[] _annotations =
    [
        ChartAnnotation.Threshold(200, "SLA", FlareColor.Error),
        ChartAnnotation.Band(120, 160, "Target range", FlareColor.Success),
        ChartAnnotation.Arrow(4, 252, 7, 142, "Recovering", FlareColor.Primary),
        ChartAnnotation.At(4, 252, "Incident", FlareColor.Error),
    ];

    private static readonly ChartAnnotation[] _bandOnly =
    [
        ChartAnnotation.VerticalBand(3, 5, "Migration", FlareColor.Warning),
    ];

    // The same band twice, differing only in the layer it draws on.
    private static readonly ChartAnnotation[] _bandOver =
    [
        ChartAnnotation.VerticalBand(2, 5, "Plan", FlareColor.Primary),
    ];

    private static readonly ChartAnnotation[] _bandUnder =
    [
        ChartAnnotation.VerticalBand(2, 5, "Plan", FlareColor.Primary, ChartAnnotationLayer.Under),
    ];
}
