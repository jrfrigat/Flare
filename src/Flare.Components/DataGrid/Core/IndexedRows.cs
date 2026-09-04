using System.Collections;

namespace Flare.Components;

// A row's absolute index is part of what a row IS: aria-rowindex, the cell-selection focus and the
// range anchor are all indices. The plain path counts as it enumerates; Virtualize hands its child
// content the item alone, so the index has to travel WITH the item or the virtual path needs a second,
// poorer row renderer - which is exactly the split this type exists to remove.
//
// A view rather than a projection: pairing is done in the indexer, so nothing is allocated per row and
// a five-million-row set costs one wrapper. IList is implemented for the same reason - Enumerable.Skip
// takes an O(1) path over an IList and an O(skip) one over a bare IEnumerable, and Virtualize skips to
// its window on every scroll.
internal readonly record struct IndexedRow<T>(int Index, T Item);

// One page of a row list, as a VIEW. `Skip(n).Take(m)` walks n elements every time it is enumerated,
// and the page is read several times per render - the aggregate footer, the select-all tri-state, the
// group builder and the row loop all ask for it. At a page size of ten that is invisible; at the page
// sizes an unpaged or a virtualized grid works with it is the whole cost of the render.
internal sealed class ListWindow<T> : IList<T>, IReadOnlyList<T>
{
    private readonly IList<T> _source;
    private readonly int _offset;

    public ListWindow(IList<T> source, int offset, int count)
    {
        _source = source;
        _offset = Math.Clamp(offset, 0, source.Count);
        Count = Math.Clamp(count, 0, source.Count - _offset);
    }

    public T this[int index]
    {
        get => _source[_offset + index];
        set => throw new NotSupportedException();
    }

    public int Count { get; }
    public bool IsReadOnly => true;

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return _source[_offset + i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        for (var i = 0; i < Count; i++)
            if (EqualityComparer<T>.Default.Equals(_source[_offset + i], item))
                return i;
        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < Count; i++)
            array[arrayIndex + i] = _source[_offset + i];
    }

    public void Add(T item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public void Insert(int index, T item) => throw new NotSupportedException();
    public bool Remove(T item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
}

internal sealed class IndexedRows<T> : IList<IndexedRow<T>>, IReadOnlyList<IndexedRow<T>>
{
    private readonly IList<T> _source;

    public IndexedRows(IList<T> source) => _source = source;

    public IndexedRow<T> this[int index]
    {
        get => new(index, _source[index]);
        set => throw new NotSupportedException();
    }

    public int Count => _source.Count;
    public bool IsReadOnly => true;

    public IEnumerator<IndexedRow<T>> GetEnumerator()
    {
        for (var i = 0; i < _source.Count; i++)
            yield return new IndexedRow<T>(i, _source[i]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(IndexedRow<T> item) =>
        item.Index >= 0 && item.Index < _source.Count ? item.Index : -1;

    public bool Contains(IndexedRow<T> item) => IndexOf(item) >= 0;

    public void CopyTo(IndexedRow<T>[] array, int arrayIndex)
    {
        for (var i = 0; i < _source.Count; i++)
            array[arrayIndex + i] = new IndexedRow<T>(i, _source[i]);
    }

    public void Add(IndexedRow<T> item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public void Insert(int index, IndexedRow<T> item) => throw new NotSupportedException();
    public bool Remove(IndexedRow<T> item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
}
