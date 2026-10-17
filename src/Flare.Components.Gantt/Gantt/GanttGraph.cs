namespace Flare.Components.Gantt;

/// <summary>Dependency graph rules: cycle checks, pushing successors and the critical path.</summary>
public static class GanttGraph
{
    private const int MaxPasses = 10_000;

    /// <summary>Checks whether <paramref name="candidate"/> may be added to <paramref name="links"/>.</summary>
    public static GanttValidation Validate(IEnumerable<GanttLink> links, GanttLink candidate)
    {
        if (candidate.FromKey == candidate.ToKey) return new GanttValidation(GanttValidationCode.SelfLink, [candidate.FromKey]);
        var list = links.ToList();
        if (list.Any(l => l.FromKey == candidate.FromKey && l.ToKey == candidate.ToKey && l.Type == candidate.Type))
            return new GanttValidation(GanttValidationCode.Duplicate, [candidate.FromKey, candidate.ToKey]);

        // The new edge From -> To closes a cycle exactly when To already reaches From.
        var next = list.ToLookup(l => l.FromKey, l => l.ToKey);
        var parent = new Dictionary<string, string> { [candidate.ToKey] = candidate.ToKey };
        var queue = new Queue<string>([candidate.ToKey]);
        while (queue.Count > 0)
        {
            var key = queue.Dequeue();
            if (key == candidate.FromKey)
            {
                var path = new List<string> { key };
                while (path[^1] != candidate.ToKey) path.Add(parent[path[^1]]);
                path.Reverse();
                path.Insert(0, candidate.FromKey);
                return new GanttValidation(GanttValidationCode.Cycle, path);
            }
            foreach (var to in next[key])
                if (parent.TryAdd(to, key)) queue.Enqueue(to);
        }
        return GanttValidation.Ok;
    }

    /// <summary>
    /// Moves <paramref name="moved"/> to <paramref name="newSpan"/> and, under <see cref="GanttScheduleMode.Push"/>,
    /// pushes every successor whose constraint breaks later by the smallest amount, keeping its duration,
    /// transitively. Items are never pulled earlier. Returns only the items whose span changed, including the moved one.
    /// </summary>
    public static IReadOnlyDictionary<string, GanttSpan> Cascade(IReadOnlyDictionary<string, GanttSpan> spans, IEnumerable<GanttLink> links,
        string moved, GanttSpan newSpan, GanttScheduleMode mode = GanttScheduleMode.Push)
    {
        var current = new Dictionary<string, GanttSpan>(spans) { [moved] = newSpan };
        var changed = new HashSet<string> { moved };
        if (mode == GanttScheduleMode.Push)
        {
            var outgoing = links.Where(l => current.ContainsKey(l.FromKey) && current.ContainsKey(l.ToKey)).ToLookup(l => l.FromKey);
            var queue = new Queue<string>([moved]);
            for (var pass = 0; queue.Count > 0 && pass < MaxPasses; pass++)
            {
                var key = queue.Dequeue();
                foreach (var link in outgoing[key])
                {
                    var successor = current[link.ToKey];
                    var shift = Required(current[key], successor, link);
                    if (shift <= TimeSpan.Zero) continue;
                    current[link.ToKey] = successor.Shift(shift);
                    changed.Add(link.ToKey);
                    queue.Enqueue(link.ToKey);
                }
            }
        }
        return changed.Where(k => !spans.TryGetValue(k, out var before) || before != current[k]).ToDictionary(k => k, k => current[k]);
    }

    /// <summary>How much later the successor must move to satisfy the link; zero or negative when it already does.</summary>
    public static TimeSpan Required(GanttSpan predecessor, GanttSpan successor, GanttLink link) => link.Type switch
    {
        GanttLinkType.FinishToStart => predecessor.End + link.Lag - successor.Start,
        GanttLinkType.StartToStart => predecessor.Start + link.Lag - successor.Start,
        GanttLinkType.FinishToFinish => predecessor.End + link.Lag - successor.End,
        _ => predecessor.Start + link.Lag - successor.End
    };

    /// <summary>
    /// Items with no slack in the current schedule: a backward pass from the end of the plan gives every item the
    /// latest finish its successors allow, and an item whose latest finish is not after its own finish is critical.
    /// Returns an empty set when the links form a cycle.
    /// </summary>
    public static IReadOnlySet<string> CriticalPath(IReadOnlyDictionary<string, GanttSpan> spans, IEnumerable<GanttLink> links)
    {
        if (spans.Count == 0) return new HashSet<string>();
        var edges = links.Where(l => spans.ContainsKey(l.FromKey) && spans.ContainsKey(l.ToKey) && l.FromKey != l.ToKey).ToList();
        var order = TopologicalOrder(spans.Keys, edges);
        if (order is null) return new HashSet<string>();

        var projectEnd = spans.Values.Max(s => s.End);
        var outgoing = edges.ToLookup(l => l.FromKey);
        var lateFinish = new Dictionary<string, DateTime>();
        for (var i = order.Count - 1; i >= 0; i--)
        {
            var key = order[i];
            var duration = spans[key].Duration;
            var finish = projectEnd;
            foreach (var link in outgoing[key])
            {
                var succFinish = lateFinish[link.ToKey];
                var succStart = succFinish - spans[link.ToKey].Duration;
                var bound = link.Type switch
                {
                    GanttLinkType.FinishToStart => succStart - link.Lag,
                    GanttLinkType.StartToStart => succStart - link.Lag + duration,
                    GanttLinkType.FinishToFinish => succFinish - link.Lag,
                    _ => succFinish - link.Lag + duration
                };
                if (bound < finish) finish = bound;
            }
            lateFinish[key] = finish;
        }
        return spans.Keys.Where(k => lateFinish[k] <= spans[k].End).ToHashSet();
    }

    private static List<string>? TopologicalOrder(IEnumerable<string> keys, List<GanttLink> edges)
    {
        var all = keys.ToList();
        var incoming = all.ToDictionary(k => k, _ => 0);
        foreach (var edge in edges) incoming[edge.ToKey]++;
        var outgoing = edges.ToLookup(l => l.FromKey, l => l.ToKey);
        var ready = new Queue<string>(all.Where(k => incoming[k] == 0));
        var order = new List<string>();
        while (ready.Count > 0)
        {
            var key = ready.Dequeue();
            order.Add(key);
            foreach (var to in outgoing[key])
                if (--incoming[to] == 0) ready.Enqueue(to);
        }
        return order.Count == all.Count ? order : null;
    }
}
