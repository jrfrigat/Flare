using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsConfigDemo
{
    private static readonly ChartData _bars = new(
        [new ChartSeries("Users", [12400, 9800, 15200, 7300])],
        ["North", "South", "East", "West"]);

    private static readonly ChartData _pie = new(
        [
            new ChartSeries("Desktop", [62]),
            new ChartSeries("Mobile", [30]),
            new ChartSeries("Tablet", [8]),
        ]);

    private static readonly ChartData _line = new(
        [new ChartSeries("Signups", [8, 14, 11, 19, 16, 24, 21])],
        ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);

    private int? _last;
    private string _lineTitle => _last is { } i ? $"Clicked point #{i + 1}" : "Interactive line (click a point)";

    private void OnPoint(int index) => _last = index;
}
