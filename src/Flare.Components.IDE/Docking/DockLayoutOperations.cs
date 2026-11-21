using System.Collections.Immutable;

namespace Flare.Components.IDE;

internal sealed class DockLayoutIndex
{
    internal Dictionary<string, DockGroup> Groups { get; } = new(StringComparer.Ordinal);
    internal Dictionary<string, DockSplit> Splits { get; } = new(StringComparer.Ordinal);
    internal Dictionary<string, DockGroup> Owners { get; } = new(StringComparer.Ordinal);

    internal static DockLayoutIndex? Validate(DockNode? root, IReadOnlyDictionary<string, DockItem> catalog)
    {
        if (root is null) return null;
        var index = new DockLayoutIndex();
        var nodeKeys = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Stack<(DockNode Node, int Depth)>();
        pending.Push((root, 0));

        while (pending.TryPop(out var entry))
        {
            var (node, depth) = entry;
            // Bound recursive rendering and transitions on unsupported input.
            if (node is null || depth > 256 || string.IsNullOrWhiteSpace(node.Key) || !nodeKeys.Add(node.Key))
                return null;

            switch (node)
            {
                case DockSplit split when Enum.IsDefined(split.Axis) && split.Fraction > 0 && split.Fraction < 1:
                    index.Splits.Add(split.Key, split);
                    pending.Push((split.Second, depth + 1));
                    pending.Push((split.First, depth + 1));
                    break;
                case DockGroup group when !group.Items.IsDefault && (depth == 0 || group.Items.Length > 0):
                    if (group.ActiveKey is not null && !group.Items.Contains(group.ActiveKey))
                        return null;
                    index.Groups.Add(group.Key, group);
                    foreach (var key in group.Items)
                        if (string.IsNullOrWhiteSpace(key) || !catalog.ContainsKey(key) || !index.Owners.TryAdd(key, group))
                            return null;
                    break;
                default:
                    return null;
            }
        }

        return index;
    }
}

internal static class DockLayoutOperations
{
    internal static DockNode Activate(DockNode root, DockGroup owner, string item) =>
        owner.ActiveKey == item ? root : Rewrite(root, owner.Key, owner with { ActiveKey = item });

    internal static DockNode Close(DockNode root, DockGroup owner, string item) =>
        Prune(Rewrite(root, owner.Key, Remove(owner, item))) ?? new DockGroup(owner.Key, []);

    internal static DockNode? Move(DockNode root, DockGroup source, DockGroup target, string item, int index)
    {
        var reduced = Remove(source, item);
        var destination = source.Key == target.Key ? reduced : target;
        if (index < 0 || index > destination.Items.Length) return null;
        var items = destination.Items.Insert(index, item);
        if (source.Key == target.Key)
            return items.SequenceEqual(source.Items) ? root : Rewrite(root, source.Key, source with { Items = items });

        var withoutSource = Rewrite(root, source.Key, reduced);
        return Prune(Rewrite(withoutSource, target.Key, target with { Items = items, ActiveKey = item }));
    }

    internal static DockNode? Split(DockNode root, DockGroup source, DockGroup target, string item,
        DockAxis axis, bool first, string splitKey, string groupKey)
    {
        var reduced = Remove(source, item);
        var other = source.Key == target.Key ? reduced : target;
        if (other.Items.IsEmpty) return null;

        var added = new DockGroup(groupKey, [item], item);
        var split = new DockSplit(splitKey, axis, first ? added : other, first ? other : added);
        var withoutSource = Rewrite(root, source.Key, reduced);
        return Prune(Rewrite(withoutSource, target.Key, split));
    }

    internal static DockNode Resize(DockNode root, DockSplit split, double fraction) =>
        Rewrite(root, split.Key, split with { Fraction = fraction });

    private static DockGroup Remove(DockGroup group, string item)
    {
        var position = group.Items.IndexOf(item);
        var items = group.Items.RemoveAt(position);
        var selected = group.ActiveKey != item ? group.ActiveKey :
            items.IsEmpty ? null : items[Math.Min(position, items.Length - 1)];
        return group with { Items = items, ActiveKey = selected };
    }

    private static DockNode Rewrite(DockNode node, string key, DockNode replacement)
    {
        if (node.Key == key) return replacement;
        if (node is not DockSplit split) return node;
        var first = Rewrite(split.First, key, replacement);
        var second = Rewrite(split.Second, key, replacement);
        return ReferenceEquals(first, split.First) && ReferenceEquals(second, split.Second)
            ? split : split with { First = first, Second = second };
    }

    private static DockNode? Prune(DockNode node)
    {
        if (node is DockGroup group) return group.Items.IsEmpty ? null : group;
        var split = (DockSplit)node;
        var first = Prune(split.First);
        var second = Prune(split.Second);
        if (first is null) return second;
        if (second is null) return first;
        return ReferenceEquals(first, split.First) && ReferenceEquals(second, split.Second)
            ? split : split with { First = first, Second = second };
    }
}
