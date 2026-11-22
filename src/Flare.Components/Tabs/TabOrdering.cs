namespace Flare.Components;

/// <summary>Applies and moves the key order of <c>FlareTab</c> items.</summary>
internal static class TabOrdering
{
    internal static IReadOnlyList<T> Apply<T>(IReadOnlyList<T> items, IReadOnlyList<string>? order, Func<T, string> key)
    {
        if (order is null || order.Count == 0) return items.ToArray();
        var rank = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var id in order) rank.TryAdd(id, rank.Count);
        return items.OrderBy(item => rank.GetValueOrDefault(key(item), int.MaxValue)).ToArray();
    }

    internal static string[]? Move<T>(IReadOnlyList<T> items, T item, int destination, Func<T, string> key)
    {
        if ((uint)destination >= (uint)items.Count) return null;
        var keys = items.Select(key).ToList();
        var id = key(item);
        var index = keys.IndexOf(id);
        if (index < 0 || index == destination) return null;
        keys.RemoveAt(index);
        keys.Insert(destination, id);
        return keys.ToArray();
    }
}
