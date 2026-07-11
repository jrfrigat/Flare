using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsAdvancedDemo
{
    private static readonly ChartData _bars = new(
        [new ChartSeries("Sales", [24, 38, 31, 45, 52, 40, 58, 62])],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"]);

    private static readonly ChartData _heat = new(
        [
            new ChartSeries("Week 1", [2, 5, 8, 6, 9, 3, 1]),
            new ChartSeries("Week 2", [4, 7, 6, 9, 8, 5, 2]),
            new ChartSeries("Week 3", [1, 3, 9, 7, 6, 8, 4]),
            new ChartSeries("Week 4", [6, 8, 5, 4, 7, 9, 3]),
        ],
        ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);
}
