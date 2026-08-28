using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsZoomDemo
{
    private ChartZoom? _window;

    private static readonly ChartData _daily = Build();

    private static ChartData Build()
    {
        // A deterministic walk, so the demo looks the same on every load and in every screenshot.
        var rng = new Random(20260828);
        var values = new double[240];
        var labels = new string[240];
        double level = 100;
        var start = new DateOnly(2025, 1, 1);
        for (var i = 0; i < values.Length; i++)
        {
            level += rng.NextDouble() * 6 - 2.6;
            values[i] = Math.Round(level, 1);
            labels[i] = start.AddDays(i * 3).ToString("MMM d");
        }
        return new ChartData([new ChartSeries("Readings", values)], labels);
    }
}
