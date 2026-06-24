using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsLineDemo
{
    private static readonly ChartData _lineData = new(
        [new ChartSeries("Revenue", [12, 19, 15, 28, 34, 45, 38, 52, 61, 58, 70, 84])],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]);
}