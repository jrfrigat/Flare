using Flare.Components;

namespace Flare.Gallery.Pages.Components.Charts.Examples;

public partial class ChartsGapsDemo
{
    private static readonly string[] _months =
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    private static readonly double[] _allYear = [42, 38, 45, 51, 55, 62, 68, 64, 57, 50, 46, 44];

    // Ice cream sells from May to September and not at all outside it - which is an absence of data,
    // not a month of zero sales.
    private static readonly double?[] _seasonal =
        [null, null, null, null, 18, 44, 71, 66, 25, null, null, null];

    // Two separate runs rather than one: the set menu was on offer in spring and again in autumn, so
    // the line is drawn twice and the winter and summer holes stay empty.
    private static readonly double?[] _twoRuns =
        [null, 31, 36, 33, null, null, null, null, 29, 34, 30, null];

    private static readonly ChartData _withGaps = new(
        [
            new ChartSeries("Coffee", _allYear),
            new ChartSeries("Ice cream", ChartSeries.Gaps(_seasonal)),
            new ChartSeries("Set menu", ChartSeries.Gaps(_twoRuns)),
        ],
        _months);

    private static readonly ChartData _zeroPadded = new(
        [
            new ChartSeries("Coffee", _allYear),
            new ChartSeries("Ice cream", [.. _seasonal.Select(v => v ?? 0)]),
            new ChartSeries("Set menu", [.. _twoRuns.Select(v => v ?? 0)]),
        ],
        _months);

    private bool _padWithZeros;
}
