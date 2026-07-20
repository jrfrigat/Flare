using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Querio.Linq;

/// <summary>
/// Collapses a group of rows into one value. Kept apart from the expression building because an
/// aggregate is a decision about a set of rows rather than about the meaning of a single value.
/// </summary>
internal static class QueryAggregates
{
    /// <summary>
    /// Computes one aggregate over a group. An aggregate of nothing is nothing rather than zero,
    /// which is what a store returns and what keeps an empty group from reading as a real result.
    /// </summary>
    /// <param name="item">The selected item describing the aggregate.</param>
    /// <param name="read">Reads the value being aggregated. Null when rows themselves are counted.</param>
    /// <param name="rows">The rows in the group.</param>
    internal static object? Compute(
        QuerySelect item, Func<object?[], object?>? read, IReadOnlyList<object?[]> rows)
    {
        if (item.Aggregate == QueryAggregate.Count && read is null) return (long)rows.Count;

        var values = new List<object?>(rows.Count);
        foreach (var row in rows)
        {
            var value = read!(row);
            if (value is not null) values.Add(value);
        }
        if (item.Distinct) values = Unique(values);

        switch (item.Aggregate)
        {
            case QueryAggregate.Count:
                return (long)values.Count;

            case QueryAggregate.Min:
                return Extreme(values, -1);
            case QueryAggregate.Max:
                return Extreme(values, 1);

            case QueryAggregate.Sum:
                return values.Count == 0 ? null : (double?)values.Sum(Number);
            case QueryAggregate.Avg:
                return values.Count == 0 ? null : (double?)values.Average(Number);
            case QueryAggregate.Percentile:
                return Percentile(values, item.Percentile ?? 0.5);

            default:
                return null;
        }
    }

    private static List<object?> Unique(List<object?> values)
    {
        var seen = new HashSet<object>();
        var kept = new List<object?>(values.Count);
        foreach (var value in values)
        {
            if (value is not null && seen.Add(value)) kept.Add(value);
        }
        return kept;
    }

    private static object? Extreme(List<object?> values, int sign)
    {
        object? best = null;
        foreach (var value in values)
        {
            if (best is null || Math.Sign(QueryClrValue.Compare(value, best)) == sign) best = value;
        }
        return best;
    }

    /// <summary>
    /// The value the given fraction of the group falls below, interpolating between neighbours when
    /// the rank lands between two rows. That is the continuous reading of a percentile, so the
    /// answer matches what a database computing it would return rather than merely being close.
    /// </summary>
    private static double? Percentile(List<object?> values, double rank)
    {
        if (values.Count == 0) return null;

        var sorted = values.Select(Number).OrderBy(value => value).ToList();
        if (sorted.Count == 1) return sorted[0];

        var position = Math.Max(0d, Math.Min(1d, rank)) * (sorted.Count - 1);
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);
        if (lower == upper) return sorted[lower];
        return sorted[lower] + ((sorted[upper] - sorted[lower]) * (position - lower));
    }

    private static double Number(object? value)
        => value is null ? 0d : Convert.ToDouble(value, CultureInfo.InvariantCulture);
}
