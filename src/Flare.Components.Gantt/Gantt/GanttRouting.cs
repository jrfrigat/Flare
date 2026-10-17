using System.Globalization;

namespace Flare.Components.Gantt;

/// <summary>Orthogonal connector paths between bars for an SVG overlay.</summary>
public static class GanttRouting
{
    /// <summary>
    /// SVG path data from <paramref name="from"/> to <paramref name="to"/>. The connector leaves the predecessor on the
    /// side the link type names (finish on the right, start on the left), steps out by <paramref name="gap"/>, and
    /// enters the successor on its named side. When the straight route would run backwards it detours along the
    /// horizontal line between the two bars.
    /// </summary>
    public static string Path(GanttBox from, GanttBox to, GanttLinkType type, double gap)
    {
        var fromFinish = type is GanttLinkType.FinishToStart or GanttLinkType.FinishToFinish;
        var toStart = type is GanttLinkType.FinishToStart or GanttLinkType.StartToStart;

        var x0 = fromFinish ? from.Right : from.X;
        var y0 = from.MiddleY;
        var x3 = toStart ? to.X : to.Right;
        var y3 = to.MiddleY;
        var x1 = fromFinish ? x0 + gap : x0 - gap;
        var x2 = toStart ? x3 - gap : x3 + gap;

        // Same direction out and in, with room between: one vertical run at the exit column.
        var direct = (fromFinish, toStart) switch
        {
            (true, true) => x1 <= x2,
            (false, false) => x1 >= x2,
            (true, false) => true,
            _ => true
        };
        if (direct)
        {
            var column = (fromFinish, toStart) switch
            {
                (true, false) => Math.Max(x1, x2),
                (false, true) => Math.Min(x1, x2),
                _ => x1
            };
            return $"M {F(x0)} {F(y0)} H {F(column)} V {F(y3)} H {F(x3)}";
        }

        // Detour: out, along the line between the two bars, then into the successor.
        var middle = y3 >= y0 ? (from.Y + from.Height + to.Y) / 2 : (to.Y + to.Height + from.Y) / 2;
        return $"M {F(x0)} {F(y0)} H {F(x1)} V {F(middle)} H {F(x2)} V {F(y3)} H {F(x3)}";
    }

    private static string F(double value) => Math.Round(value, 1).ToString("0.#", CultureInfo.InvariantCulture);
}
