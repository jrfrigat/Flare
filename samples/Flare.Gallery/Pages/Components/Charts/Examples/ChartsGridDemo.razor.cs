using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsGridDemo
{
    // Deliberately unround values: they are what makes the difference between a rounded and an unrounded
    // axis visible at a glance.
    private static readonly ChartData _revenue = new(
        [new ChartSeries("Revenue", [94, 213, 187, 342, 296, 470, 415, 388])],
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"]);

    private bool _auto = true;
    private bool _nice = true;
    private bool _vertical;
    private double _ticks = 5;
    private double _minor;
}
