using System.Collections;

namespace Flare.Components;

// Virtualize accepts ICollection<T> and reaches the visible window via Skip/Take. An IList
// view lets it index directly into a read-only source without taking a snapshot of every item.
internal sealed class VirtualListItems<T>(IReadOnlyList<T> source) : IList<T>
{
    public static ICollection<T> Adapt(IReadOnlyList<T>? items) =>
        items as ICollection<T> ?? (items is null ? Array.Empty<T>() : new VirtualListItems<T>(items));

    public int Count => source.Count;
    public bool IsReadOnly => true;

    public T this[int index]
    {
        get => source[index];
        set => throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < Count; i++)
            if (comparer.Equals(source[i], item))
                return i;
        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Insufficient destination space.", nameof(array));

        for (var i = 0; i < Count; i++)
            array[arrayIndex + i] = source[i];
    }

    public void Add(T item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public void Insert(int index, T item) => throw new NotSupportedException();
    public bool Remove(T item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
}
