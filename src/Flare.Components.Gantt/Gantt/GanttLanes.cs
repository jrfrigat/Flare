namespace Flare.Components.Gantt;

/// <summary>Placement of overlapping intervals on sub-lanes of one row.</summary>
/// <typeparam name="TKey">Key of an interval.</typeparam>
/// <param name="Lanes">Lane index of every key, starting at zero.</param>
/// <param name="Count">Number of lanes used; at least one.</param>
public sealed record GanttLaneLayout<TKey>(IReadOnlyDictionary<TKey, int> Lanes, int Count) where TKey : notnull;

/// <summary>Greedy interval partitioning: every interval takes the first lane where it overlaps nothing.</summary>
public static class GanttLanes
{
    /// <summary>
    /// Assigns lanes. Intervals are taken by start, longer first, then in input order, so the result is stable.
    /// Touching intervals share a lane; a zero-length interval still occupies its point.
    /// </summary>
    public static GanttLaneLayout<TKey> Assign<TKey>(IEnumerable<(TKey Key, GanttSpan Span)> intervals) where TKey : notnull
    {
        var ordered = intervals.Select((x, i) => (x.Key, x.Span, Index: i))
            .OrderBy(x => x.Span.Start).ThenByDescending(x => x.Span.Duration).ThenBy(x => x.Index);
        var laneEnds = new List<DateTime>();
        var lanes = new Dictionary<TKey, int>();
        foreach (var (key, span, _) in ordered)
        {
            // A point is given the smallest length so that two points at the same moment do not share a lane.
            var end = span.End > span.Start ? span.End : span.Start.AddTicks(1);
            var lane = laneEnds.FindIndex(laneEnd => laneEnd <= span.Start);
            if (lane < 0) { lane = laneEnds.Count; laneEnds.Add(end); }
            else laneEnds[lane] = end;
            lanes[key] = lane;
        }
        return new GanttLaneLayout<TKey>(lanes, Math.Max(1, laneEnds.Count));
    }
}

/// <summary>Summary of child items for a parent bar.</summary>
public static class GanttRollup
{
    /// <summary>From the earliest start to the latest end, or null when there are no children.</summary>
    public static GanttSpan? Span(IEnumerable<GanttSpan> children)
    {
        DateTime? start = null, end = null;
        foreach (var child in children)
        {
            if (start is null || child.Start < start) start = child.Start;
            if (end is null || child.End > end) end = child.End;
        }
        return start is { } s && end is { } e ? new GanttSpan(s, e) : null;
    }

    /// <summary>Progress weighted by duration; a plain average when every child has zero length; zero for none.</summary>
    public static double Progress(IEnumerable<(GanttSpan Span, double Progress)> children)
    {
        var list = children.ToList();
        if (list.Count == 0) return 0;
        var total = list.Sum(x => x.Span.Duration.TotalHours);
        return total > 0
            ? list.Sum(x => x.Span.Duration.TotalHours * x.Progress) / total
            : list.Average(x => x.Progress);
    }
}
