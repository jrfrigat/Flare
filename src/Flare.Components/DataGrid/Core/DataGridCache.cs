namespace Flare.Components;

/// <summary>
/// Memoized cache for DataGrid pipeline results.
/// Avoids re-computation when inputs haven't changed.
/// </summary>
public sealed class DataGridCache<TItem>
{
    private IReadOnlyList<TItem>? _lastSource;
    private IReadOnlyList<DataGridSort>? _lastSorts;
    private IReadOnlyList<DataGridFilter>? _lastFilters;
    private DataGridFilterGroup? _lastAdvancedFilter;
    private Func<TItem, bool>? _lastQuickFilter;
    private int _lastPage;
    private int _lastPageSize;

    private DataGridResult<TItem>? _cachedResult;

    /// <summary>
    /// Returns cached result if inputs match, otherwise executes pipeline and caches.
    /// </summary>
    public DataGridResult<TItem> GetOrExecute(
        IReadOnlyList<TItem> source,
        IReadOnlyList<DataGridSort> sorts,
        IReadOnlyList<DataGridFilter> filters,
        DataGridFilterGroup? advancedFilter,
        Func<TItem, bool>? quickFilter,
        int page,
        int pageSize)
    {
        if (_cachedResult is not null &&
            ReferenceEquals(_lastSource, source) &&
            ReferenceEquals(_lastSorts, sorts) &&
            ReferenceEquals(_lastFilters, filters) &&
            ReferenceEquals(_lastAdvancedFilter, advancedFilter) &&
            ReferenceEquals(_lastQuickFilter, quickFilter) &&
            _lastPage == page &&
            _lastPageSize == pageSize)
        {
            return _cachedResult;
        }

        _lastSource = source;
        _lastSorts = sorts;
        _lastFilters = filters;
        _lastAdvancedFilter = advancedFilter;
        _lastQuickFilter = quickFilter;
        _lastPage = page;
        _lastPageSize = pageSize;

        _cachedResult = DataGridPipeline<TItem>.Execute(
            source, sorts, filters, advancedFilter, quickFilter,
            null, null, page, pageSize);

        return _cachedResult;
    }

    /// <summary>Invalidates the cache, forcing re-computation on next access.</summary>
    public void Invalidate()
    {
        _cachedResult = null;
        _lastSource = null;
        _lastSorts = null;
        _lastFilters = null;
        _lastAdvancedFilter = null;
        _lastQuickFilter = null;
    }

    /// <summary>Whether the cache is valid for the given inputs.</summary>
    public bool IsValid(
        IReadOnlyList<TItem> source,
        IReadOnlyList<DataGridSort> sorts,
        IReadOnlyList<DataGridFilter> filters,
        DataGridFilterGroup? advancedFilter,
        Func<TItem, bool>? quickFilter,
        int page,
        int pageSize) =>
        _cachedResult is not null &&
        ReferenceEquals(_lastSource, source) &&
        ReferenceEquals(_lastSorts, sorts) &&
        ReferenceEquals(_lastFilters, filters) &&
        ReferenceEquals(_lastAdvancedFilter, advancedFilter) &&
        ReferenceEquals(_lastQuickFilter, quickFilter) &&
        _lastPage == page &&
        _lastPageSize == pageSize;
}
