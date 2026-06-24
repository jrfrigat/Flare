using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsMultiLineDemo
{
    private static readonly ChartData _multiData = new(
        [
            new ChartSeries("Users", [320, 450, 410, 530, 620, 710, 680, 800, 850, 920, 980, 1100]),
            new ChartSeries("Sessions", [510, 720, 640, 830, 960, 1100, 1050, 1200, 1280, 1350, 1420, 1600])
        ],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]);
}