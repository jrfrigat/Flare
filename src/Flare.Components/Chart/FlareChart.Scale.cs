namespace Flare.Components;

public partial class FlareChart
{
    // Applies the YMin/YMax overrides to an auto-computed (min,max) pair.
    /// <summary>A resolved value axis: the window the data is projected into and how many grid lines span it.</summary>
    /// <param name="Min">Lower bound of the axis, after any rounding and any <see cref="YMin"/> override.</param>
    /// <param name="Max">Upper bound of the axis, after any rounding and any <see cref="YMax"/> override.</param>
    /// <param name="Lines">Number of major grid lines to draw, counting both ends.</param>
    private readonly record struct AxisScale(double Min, double Max, int Lines);

    // One grid line per _tickSlot units of plot, the same budget rule the X axis uses for its labels: the
    // count is then a consequence of the chart's size rather than a constant, so a 120px chart draws three
    // lines where a 600px one draws nine. A default 220px chart still lands on five - what it drew before.
    private const double _tickSlot = 44;

    private int TickLines(double extent) => YAxisTickCount is { } n
        ? Math.Clamp(n, 2, 24)
        : Math.Clamp((int)Math.Round(extent / _tickSlot) + 1, 3, 9);

    // The 1/2/2.5/5/10 progression every spreadsheet axis is built from, snapped to the NEAREST member
    // rather than up. Always rounding up coarsens the axis badly at the boundaries - a 0..470 range asking
    // for five lines lands on a step of 200 and an axis running to 600, when 100 and 0..500 is both rounder
    // and tighter. Rounding down costs at most one extra line, which the caller is told to expect.
    private static double NiceStep(double rough)
    {
        if (rough <= 0 || double.IsNaN(rough) || double.IsInfinity(rough)) return 1;
        double pow = Math.Pow(10, Math.Floor(Math.Log10(rough)));
        double f = rough / pow;
        return (f < 1.5 ? 1 : f < 2.25 ? 2 : f < 3.75 ? 2.5 : f < 7.5 ? 5 : 10) * pow;
    }

    // The next member of the progression above a step already on it.
    private static double NextNiceStep(double step)
    {
        double pow = Math.Pow(10, Math.Floor(Math.Log10(step)));
        double f = step / pow;
        return (f < 1.5 ? 2 : f < 2.25 ? 2.5 : f < 3.75 ? 5 : 10) * pow;
    }

    // Decimals the axis actually carries - enough to write its origin and its step without lying about
    // either, capped so a floating-point artefact cannot ask for twenty digits.
    private static int AxisDecimals(double origin, double step)
    {
        int d = 0;
        while (d < 6 && !(Exact(origin, d) && Exact(step, d))) d++;
        return d;

        static bool Exact(double v, int digits) =>
            Math.Abs(v - Math.Round(v, digits)) <= 1e-9 * Math.Max(1, Math.Abs(v));
    }

    // Bounds, rounding and line count are one calculation, not three: the nice step depends on how many
    // lines were asked for, and the count that is finally drawn depends on where the rounding landed. Split
    // them across the projection and the grid writer and the two drift out of alignment - which is the
    // defect that made a tick-count parameter useless on its own.
    // The widest value range seen while StickyDomain has been on. It outlives the render, which is the
    // whole point: a chart handed a new dataset every three seconds re-derives its domain from that
    // dataset alone, so one new point among twenty moves the other nineteen.
    private double _stickyMin = double.NaN;
    private double _stickyMax = double.NaN;

    /// <summary>Forgets the range <see cref="StickyDomain"/> is holding, so the next update sizes the
    /// value axis from the data alone. For when the caller knows the scale has changed for good.</summary>
    public void ResetDomain()
    {
        _stickyMin = double.NaN;
        _stickyMax = double.NaN;
        StateHasChanged();
    }

    private (double Min, double Max) ApplySticky(double min, double max)
    {
        if (!StickyDomain)
        {
            _stickyMin = _stickyMax = double.NaN;
            return (min, max);
        }

        if (double.IsNaN(_stickyMin))
        {
            (_stickyMin, _stickyMax) = (min, max);
            return (min, max);
        }

        // Half, and not "any decrease": a domain that never shrinks is ruined for good by one spike,
        // and one that follows every dip back down is the jitter this exists to remove. Half is the
        // point at which the reader would rather have the resolution back than the stillness.
        double held = _stickyMax - _stickyMin;
        if (held > 0 && max - min < held / 2)
        {
            (_stickyMin, _stickyMax) = (min, max);
            return (min, max);
        }

        _stickyMin = Math.Min(_stickyMin, min);
        _stickyMax = Math.Max(_stickyMax, max);
        return (_stickyMin, _stickyMax);
    }

    private AxisScale ResolveAxis(double min, double max, double extent, bool honorMin = true)
    {
        // Before the pins and the rounding: the held range is a range of DATA, and comparing it with a
        // rounded one would let the rounding creep outward a step at a time.
        bool pinLo = honorMin && YMin.HasValue && double.IsFinite(YMin.Value);
        bool pinHi = YMax.HasValue && double.IsFinite(YMax.Value);
        bool invalidPair = YMin.HasValue && YMax.HasValue && double.IsFinite(YMin.Value)
            && double.IsFinite(YMax.Value) && YMin.Value >= YMax.Value;
        if (invalidPair) pinLo = pinHi = false;
        // Positive stacks always start at zero, so a nonpositive upper pin cannot describe their axis.
        if (!honorMin && pinHi && YMax!.Value <= 0) pinHi = false;
        if (!(pinLo && pinHi)) (min, max) = ApplySticky(min, max);

        if (pinLo) min = YMin!.Value;
        if (pinHi) max = YMax!.Value;
        if (max <= min)
        {
            if (pinHi && !pinLo)
            {
                min = max - 1;
                if (min >= max) min = Math.BitDecrement(max);
            }
            else
            {
                max = min + 1;
                if (max <= min) max = Math.BitIncrement(min);
            }
        }
        if (!double.IsFinite(min) || !double.IsFinite(max) || max <= min)
        {
            (min, max) = (0, 1);
            pinLo = pinHi = false;
        }
        int lines = TickLines(extent);
        // An axis pinned at both ends is the caller's window verbatim; rounding it would move a bound
        // they set on purpose.
        if (!NiceScale || (pinLo && pinHi)) return new AxisScale(min, max, lines);

        // Two effects push the count up: snapping the step DOWN to a rounder value, and then extending
        // both bounds outward to whole multiples of it. Together they can overshoot badly - a 94..470 range
        // asking for seven lines lands on a step of 50 and draws ten. So the step climbs the progression
        // until the overshoot is at most the one extra line the parameter documents.
        double step = NiceStep((max - min) / Math.Max(1, lines - 1));
        double lo, hi;
        int drawn;
        for (int guard = 0; ; guard++)
        {
            lo = pinLo ? min : Math.Floor(min / step) * step;
            hi = pinHi ? max : Math.Ceiling(max / step) * step;
            if (hi <= lo) hi = lo + step;
            drawn = (int)Math.Round((hi - lo) / step) + 1;
            if (drawn <= lines + 1 || guard >= 3) break;
            step = NextNiceStep(step);
        }
        return new AxisScale(lo, hi, Math.Clamp(drawn, 2, 40));
    }

    // The plot dimension the VALUE axis runs along - width on a horizontal bar chart, height everywhere
    // else - so the derived line count is budgeted against the axis it actually belongs to.
    private double _valueExtent => Horizontal && Type is ChartType.Bar ? _plotW : _plotH;

    private (double Min, double Max) ApplyBounds(double min, double max)
    {
        var axis = ResolveAxis(min, max, _valueExtent);
        return (axis.Min, axis.Max);
    }

}
