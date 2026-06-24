namespace Flare.Components;

/// <summary>
/// Optional per-column strategy overrides threaded into <see cref="DataGridPipeline{TItem}"/>.
/// Each map is keyed by the column key. This is how a column owns its custom sort/filter behaviour:
/// the grid collects the overrides declared on <see cref="FlareColumn{TItem}"/> and hands them to
/// the otherwise column-agnostic pipeline.
/// </summary>
/// <typeparam name="TItem">The grid row type.</typeparam>
public sealed record DataGridColumnStrategies<TItem>
{
    /// <summary>Value selectors used for sorting (e.g. composite fields whose key is not a property name).</summary>
    public IReadOnlyDictionary<string, Func<TItem, object?>>? SortSelectors { get; init; }

    /// <summary>Value selectors used for filtering (same purpose as <see cref="SortSelectors"/>).</summary>
    public IReadOnlyDictionary<string, Func<TItem, object?>>? FilterSelectors { get; init; }

    /// <summary>Custom row comparisons per column. When a sorted column has an entry here it is used
    /// instead of selector-based ordering (ascending uses the result as-is, descending negates it).</summary>
    public IReadOnlyDictionary<string, Comparison<TItem>>? SortComparers { get; init; }

    /// <summary>Custom filter predicates per column: <c>(row, filterText) =&gt; keep</c>. When a filtered
    /// column has an entry here it replaces the built-in text/number/date match.</summary>
    public IReadOnlyDictionary<string, Func<TItem, string, bool>>? ColumnFilters { get; init; }

    /// <summary>Resolved data type per column key. Lets the pipeline compare numbers, dates and times
    /// by value (not lexically) for equality/range operators.</summary>
    public IReadOnlyDictionary<string, ColumnDataType>? ColumnTypes { get; init; }

    /// <summary>True when no override is set (lets the pipeline skip strategy lookups entirely).</summary>
    public bool IsEmpty =>
        SortSelectors is null && FilterSelectors is null && SortComparers is null && ColumnFilters is null;
}
