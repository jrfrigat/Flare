using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsPieDemo
{
    private static readonly ChartData _pieData = new(
    [
        new ChartSeries("Chrome", [58]),
        new ChartSeries("Firefox", [16]),
        new ChartSeries("Safari", [14]),
        new ChartSeries("Edge", [12]),
    ]);
}