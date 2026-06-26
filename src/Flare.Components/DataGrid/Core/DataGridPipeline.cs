using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Stateless, single-enumeration data pipeline for FlareDataGrid.
/// Sort -> Filter -> Group -> Page in one pass. No intermediate ToList() calls.
/// </summary>
public static class DataGridPipeline<TItem>
{
    /// <summary>
    /// Executes the full pipeline: sort -> filter -> group -> page.
    /// Returns paged items + total count (before paging).
    /// </summary>
    public static DataGridResult<TItem> Execute(
        IReadOnlyList<TItem> source,
        IReadOnlyList<DataGridSort> sorts,
        IReadOnlyList<DataGridFilter> filters,
        DataGridFilterGroup? advancedFilter,
        Func<TItem, bool>? quickFilter,
        IReadOnlyList<string>? groupKeys,
        Func<TItem, object?>? groupSelector,
        int page,
        int pageSize,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        if (source.Count == 0)
            return new DataGridResult<TItem>([], 0);

        // Materialize once — the source is already a list from the caller
        IEnumerable<TItem> query = source;

        // 1. Quick filter (cheapest, applied first)
        if (quickFilter is not null)
            query = query.Where(quickFilter);

        // 2. Text/typed filters (AND logic)
        if (filters.Count > 0)
            query = ApplyFilters(query, filters, strategies);

        // 3. Advanced filter tree (AND/OR)
        if (advancedFilter is not null && !advancedFilter.IsEmpty)
            query = ApplyFilterTree(query, advancedFilter, strategies);

        // 4. Sort
        if (sorts.Count > 0)
            query = ApplySorts(query, sorts, strategies);

        // 5. Count before paging
        var totalCount = 0;
        List<TItem>? materialized = null;

        if (groupKeys is { Count: > 0 } && groupSelector is not null)
        {
            // Grouping: materialize to build group headers
            materialized = query.ToList();
            totalCount = materialized.Count;
        }
        else
        {
            // No grouping: count efficiently
            // This IList fast-path is only reached when no operator ran (query is still the source
            // list); once a sort/filter is applied, query is a deferred iterator, so the OrderBy in
            // ApplySorts is materialized by the ToList() below -- it is never skipped.
            if (query is IList<TItem> list)
            {
                totalCount = list.Count; //-V3220
            }
            else
            {
                // Last resort: materialize to count
                materialized = query.ToList();
                totalCount = materialized.Count;
            }
        }

        // 6. Page
        List<TItem> paged;
        if (materialized is not null)
        {
            paged = materialized.Skip(page * pageSize).Take(pageSize).ToList();
        }
        else if (query is IList<TItem> iList)
        {
            var start = page * pageSize;
            var count = Math.Min(pageSize, Math.Max(0, iList.Count - start));
            paged = new List<TItem>(count);
            for (var i = start; i < start + count && i < iList.Count; i++)
                paged.Add(iList[i]);
        }
        else
        {
            paged = query.Skip(page * pageSize).Take(pageSize).ToList();
        }

        return new DataGridResult<TItem>(paged, totalCount);
    }

    private static IEnumerable<TItem> ApplyFilters(IEnumerable<TItem> source, IReadOnlyList<DataGridFilter> filters,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        var result = source;
        foreach (var filter in filters)
        {
            if (string.IsNullOrEmpty(filter.Key)) continue;
            result = result.Where(item => MatchesFilter(item, filter, strategies));
        }
        return result;
    }

    private static bool MatchesFilter(TItem item, DataGridFilter filter,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        // A column-owned predicate (FlareColumn.FilterFunc) wins over the built-in match.
        if (strategies?.ColumnFilters is { } funcs && funcs.TryGetValue(filter.Key, out var func))
            return func(item, filter.Value ?? "");

        // Prefer an explicit selector (e.g. composite fields whose key is not a property name);
        // otherwise read the property named by the filter key via reflection.
        var raw = strategies?.FilterSelectors is { } sels && sels.TryGetValue(filter.Key, out var sel)
            ? sel(item)
            : GetPropertyValue(item, filter.Key);
        var itemValue = raw?.ToString() ?? "";
        var filterValue = filter.Value ?? "";

        // Resolved column type (Number/Date/DateTime/Time/Boolean) enables value comparison instead of
        // lexical string comparison for equality and range operators.
        var type = strategies?.ColumnTypes is { } ts && ts.TryGetValue(filter.Key, out var t) ? t : ColumnDataType.Text;

        return filter.Operator switch
        {
            FilterOperator.Contains => itemValue.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.StartsWith => itemValue.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.EndsWith => itemValue.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.Equals => TypedCompare(raw, filterValue, type) is { } ce
                ? ce == 0 : string.Equals(itemValue, filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotEquals => TypedCompare(raw, filterValue, type) is { } cn
                ? cn != 0 : !string.Equals(itemValue, filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.GreaterThan => Compare(raw, itemValue, filterValue, type) > 0,
            FilterOperator.GreaterThanOrEqual => Compare(raw, itemValue, filterValue, type) >= 0,
            FilterOperator.LessThan => Compare(raw, itemValue, filterValue, type) < 0,
            FilterOperator.LessThanOrEqual => Compare(raw, itemValue, filterValue, type) <= 0,
            FilterOperator.Between => Compare(raw, itemValue, filterValue, type) >= 0
                && Compare(raw, itemValue, filter.Value2 ?? "", type) <= 0,
            FilterOperator.NotBetween => !(Compare(raw, itemValue, filterValue, type) >= 0
                && Compare(raw, itemValue, filter.Value2 ?? "", type) <= 0),
            FilterOperator.IsNull => string.IsNullOrEmpty(itemValue),
            FilterOperator.IsNotNull => !string.IsNullOrEmpty(itemValue),
            FilterOperator.In => filter.Values?.Any(v => string.Equals(itemValue, v, StringComparison.OrdinalIgnoreCase)) ?? false,
            FilterOperator.NotIn => !(filter.Values?.Any(v => string.Equals(itemValue, v, StringComparison.OrdinalIgnoreCase)) ?? false),
            _ => true,
        };
    }

    private static IEnumerable<TItem> ApplyFilterTree(IEnumerable<TItem> source, DataGridFilterGroup group,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        if (group.Or)
            return source.Where(item => group.Groups.Any(g => MatchesGroup(item, g, strategies)) ||
                                        group.Conditions.Any(c => MatchesFilter(item, c, strategies)));
        else
            return source.Where(item => group.Groups.All(g => MatchesGroup(item, g, strategies)) &&
                                        group.Conditions.All(c => MatchesFilter(item, c, strategies)));
    }

    private static bool MatchesGroup(TItem item, DataGridFilterGroup group,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        if (group.Or)
            return group.Groups.Any(g => MatchesGroup(item, g, strategies)) ||
                   group.Conditions.Any(c => MatchesFilter(item, c, strategies));
        else
            return group.Groups.All(g => MatchesGroup(item, g, strategies)) &&
                   group.Conditions.All(c => MatchesFilter(item, c, strategies));
    }

    private static IEnumerable<TItem> ApplySorts(IEnumerable<TItem> source, IReadOnlyList<DataGridSort> sorts,
        DataGridColumnStrategies<TItem>? strategies = null)
    {
        IOrderedEnumerable<TItem>? ordered = null;
        foreach (var sort in sorts)
        {
            if (string.IsNullOrEmpty(sort.Key)) continue;
            var asc = sort.Direction == SortDirection.Ascending;

            // A column-owned comparison (FlareColumn.SortComparison) wins: order by identity with a
            // comparer that negates for descending.
            if (strategies?.SortComparers is { } comparers && comparers.TryGetValue(sort.Key, out var cmp))
            {
                var comparer = Comparer<TItem>.Create((a, b) => asc ? cmp(a, b) : cmp(b, a));
                ordered = ordered is null ? source.OrderBy(x => x, comparer) : ordered.ThenBy(x => x, comparer);
                continue;
            }

            // Prefer an explicit selector (e.g. composite fields whose key is not a property name);
            // otherwise sort by the property named by the key (so formatted columns sort by raw value).
            Func<TItem, object?> selector = strategies?.SortSelectors is { } sels && sels.TryGetValue(sort.Key, out var sel)
                ? sel
                : item => GetPropertyValue(item, sort.Key);

            if (ordered is null)
                ordered = asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
            else
                ordered = asc ? ordered.ThenBy(selector) : ordered.ThenByDescending(selector);
        }
        return ordered ?? source;
    }

    private static object? GetPropertyValue(TItem item, string key)
    {
        if (item is null) return null;
        var prop = typeof(TItem).GetProperty(key);
        return prop?.GetValue(item);
    }

    // Ordered comparison for range operators: compare the raw value to the filter text by the column's
    // type when possible, otherwise fall back to numeric-or-lexical string comparison.
    private static int Compare(object? raw, string itemValue, string filterValue, ColumnDataType type)
        => TypedCompare(raw, filterValue, type) ?? CompareValues(itemValue, filterValue);

    // Compares a raw cell value to a filter string by the column's data type. Returns the sign of
    // (value - filter), or null when the type is not value-comparable or either side fails to parse
    // (the caller then falls back to string comparison).
    private static int? TypedCompare(object? raw, string filterValue, ColumnDataType type)
    {
        if (raw is null || filterValue.Length == 0) return null;
        switch (type)
        {
            case ColumnDataType.Number:
                if (TryToDouble(raw, out var rn) && TryParseDouble(filterValue, out var fn))
                    return rn.CompareTo(fn);
                break;
            case ColumnDataType.Date:
            case ColumnDataType.DateTime:
                if (TryToDateTime(raw, out var rd) && TryParseDateTime(filterValue, out var fd))
                    return (type == ColumnDataType.Date ? rd.Date : rd).CompareTo(type == ColumnDataType.Date ? fd.Date : fd);
                break;
            case ColumnDataType.Time:
                if (TryToTime(raw, out var rt) && TryParseTime(filterValue, out var ft))
                    return rt.CompareTo(ft);
                break;
            case ColumnDataType.Boolean:
                if (raw is bool rb && TryParseBool(filterValue, out var fb))
                    return rb == fb ? 0 : (rb ? 1 : -1);
                break;
        }
        return null;
    }

    private static bool TryToDouble(object value, out double result)
    {
        if (value is IConvertible) { try { result = Convert.ToDouble(value, CultureInfo.InvariantCulture); return true; } catch { } }
        return double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseDouble(string s, out double result)
        => double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
        || double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out result);

    private static bool TryToDateTime(object value, out DateTime result)
    {
        switch (value)
        {
            case DateTime dt: result = dt; return true;
            case DateTimeOffset dto: result = dto.DateTime; return true;
            case DateOnly d: result = d.ToDateTime(TimeOnly.MinValue); return true;
            default: return DateTime.TryParse(value.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
        }
    }

    private static bool TryParseDateTime(string s, out DateTime result)
        => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
        || DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out result);

    private static bool TryToTime(object value, out TimeSpan result)
    {
        switch (value)
        {
            case TimeOnly to: result = to.ToTimeSpan(); return true;
            case TimeSpan ts: result = ts; return true;
            case DateTime dt: result = dt.TimeOfDay; return true;
            default: result = default; return false;
        }
    }

    private static bool TryParseTime(string s, out TimeSpan result)
    {
        if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var to)) { result = to.ToTimeSpan(); return true; }
        if (TimeOnly.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out to)) { result = to.ToTimeSpan(); return true; }
        return TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseBool(string s, out bool result)
    {
        if (bool.TryParse(s, out result)) return true;
        switch (s.Trim().ToLowerInvariant())
        {
            case "1": case "yes": case "y": case "on": result = true; return true;
            case "0": case "no": case "n": case "off": result = false; return true;
            default: return false;
        }
    }

    private static int CompareValues(string a, string b)
    {
        if (double.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out var da)
            && double.TryParse(b, NumberStyles.Any, CultureInfo.InvariantCulture, out var db))
            return da.CompareTo(db);
        return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
