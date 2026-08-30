using Flare.Components;

namespace Flare.Gallery.Pages.Components.Gauge.Examples;

public partial class GaugeDashboardDemo
{
    private static readonly ChartData _trend = new(
        [new ChartSeries("Load", [38, 44, 41, 52, 61, 58, 66, 72, 69, 74, 81, 72], Css.Tokens.Chart.SeriesVar(0))]);
}
