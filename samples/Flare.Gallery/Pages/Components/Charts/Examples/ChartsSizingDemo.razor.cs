using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsSizingDemo
{
    private static readonly ChartData _data = new(
        [
            new ChartSeries("Revenue", [24, 38, 31, 45, 52, 40, 58, 62, 55, 66, 71, 68]),
            new ChartSeries("Cost", [18, 26, 24, 30, 33, 29, 36, 39, 35, 41, 44, 42]),
        ],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]);
}
