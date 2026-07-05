namespace Flare.Components.Combobox;

/// <summary>
/// One visible row in the option surface: either a group header or an option. Options carry their index
/// into <see cref="ListCollection{TItem}.Filtered"/> so the listbox can render a stable option id
/// (for <c>aria-activedescendant</c>) and grouped virtualization from a single flat list.
/// </summary>
/// <typeparam name="TItem">The option value type.</typeparam>
/// <param name="IsHeader">True for a group header row (not selectable / not navigable).</param>
/// <param name="Group">The group label, for a header row.</param>
/// <param name="Item">The option value, for an option row.</param>
/// <param name="OptionIndex">The option's index into <see cref="ListCollection{TItem}.Filtered"/>; -1 for a header.</param>
public readonly record struct ComboboxRow<TItem>(bool IsHeader, string? Group, TItem Item, int OptionIndex);

/// <summary>
/// The ordered, grouped and filtered view over the source items for one <see cref="ComboboxState{TItem}"/>.
/// Owns item order (grouped-contiguous when a group selector is set), the filtered projection (recomputed
/// only when the source or the query changes, not per render), enabled-aware keyboard navigation and
/// type-ahead matching. Pure logic: no DOM, no components, unit-testable with plain xUnit.
/// </summary>
/// <typeparam name="TItem">The option value type.</typeparam>
public sealed class ListCollection<TItem>
{
    private readonly ComboboxPolicy<TItem> _policy;
    private IReadOnlyList<TItem> _source = Array.Empty<TItem>();
    private List<TItem> _ordered = new();
    private List<TItem> _filtered = new();
    private string _query = string.Empty;
    private IReadOnlyList<ComboboxRow<TItem>>? _rows;

    /// <summary>Creates the view for the given policy (which supplies label/group/disabled/filter delegates).</summary>
    /// <param name="policy">The owning combobox policy.</param>
    public ListCollection(ComboboxPolicy<TItem> policy) => _policy = policy;

    /// <summary>All items in display order (grouped-contiguous when grouping), unfiltered.</summary>
    public IReadOnlyList<TItem> Ordered => _ordered;

    /// <summary>The items currently visible after the active query (in display order).</summary>
    public IReadOnlyList<TItem> Filtered => _filtered;

    /// <summary>Number of currently visible options.</summary>
    public int Count => _filtered.Count;

    /// <summary>The visible option at <paramref name="index"/> (into <see cref="Filtered"/>).</summary>
    /// <param name="index">Zero-based index into the filtered list.</param>
    public TItem this[int index] => _filtered[index];

    /// <summary>Replaces the source items and recomputes the ordered + filtered projections.</summary>
    /// <param name="items">The new source items (null is treated as empty).</param>
    public void SetSource(IReadOnlyList<TItem>? items)
    {
        _source = items ?? Array.Empty<TItem>();
        _ordered = _policy.Group is null
            ? new List<TItem>(_source)
            : _source.GroupBy(_policy.Group).SelectMany(static g => g).ToList();
        Refilter();
    }

    /// <summary>Sets the filter query and recomputes the filtered projection (no-op when unchanged).</summary>
    /// <param name="query">The query text; empty shows all options.</param>
    public void SetQuery(string? query)
    {
        query ??= string.Empty;
        if (string.Equals(query, _query, StringComparison.Ordinal)) return;
        _query = query;
        Refilter();
    }

    private void Refilter()
    {
        IEnumerable<TItem> result;
        if (string.IsNullOrWhiteSpace(_query))
            result = _ordered;
        else if (_policy.RankScorer is not null)
            result = FlareSearch.Rank(_source, i => _policy.RankScorer(i, _query));
        else if (_policy.FilterPredicate is not null)
            result = _source.Where(i => _policy.FilterPredicate(i, _query));
        else if (_policy.Fuzzy)
            result = FlareSearch.Rank(_source, i => FlareSearch.Score(_policy.Label(i), _query));
        else
            result = _source.Where(i => _policy.Label(i).Contains(_query, StringComparison.OrdinalIgnoreCase));

        // Re-group after filtering/ranking so headers render at correct boundaries and the active index
        // stays aligned with the rendered order (ranking would otherwise scatter a group's members).
        if (_policy.Group is not null)
            result = result.GroupBy(_policy.Group).SelectMany(static g => g);

        _filtered = result as List<TItem> ?? result.ToList();
        _rows = null; // invalidate the cached row projection; rebuilt on next access
    }

    /// <summary>Returns an item's group label, or null when grouping is off.</summary>
    /// <param name="item">The item to classify.</param>
    public string? GroupOf(TItem item) => _policy.Group?.Invoke(item);

    /// <summary>Returns whether an item is disabled (never committable; still highlightable).</summary>
    /// <param name="item">The item to test.</param>
    public bool IsDisabled(TItem item) => _policy.ItemDisabled?.Invoke(item) ?? false;

    /// <summary>Finds the visible index of <paramref name="item"/> using the policy comparer, or -1.</summary>
    /// <param name="item">The item to locate in the filtered list.</param>
    public int IndexOf(TItem item)
    {
        for (var i = 0; i < _filtered.Count; i++)
            if (_policy.Comparer.Equals(_filtered[i], item)) return i;
        return -1;
    }

    /// <summary>
    /// Returns the next enabled option index from <paramref name="from"/> in direction
    /// <paramref name="dir"/> (+1 down, -1 up), skipping disabled options, wrapping when
    /// <paramref name="wrap"/> is set. Returns -1 when no enabled option exists.
    /// </summary>
    /// <param name="from">The starting index (-1 to start before the first option).</param>
    /// <param name="dir">+1 to move down, -1 to move up.</param>
    /// <param name="wrap">Whether to wrap around the ends.</param>
    public int NextEnabled(int from, int dir, bool wrap)
    {
        var n = _filtered.Count;
        if (n == 0) return -1;
        var i = from;
        for (var step = 0; step < n; step++)
        {
            i += dir;
            if (i < 0 || i >= n)
            {
                if (!wrap) return -1;
                i = (i % n + n) % n;
            }
            if (!IsDisabled(_filtered[i])) return i;
        }
        return -1;
    }

    /// <summary>The first enabled option index, or -1.</summary>
    public int FirstEnabled() => NextEnabled(-1, 1, false);

    /// <summary>The last enabled option index, or -1.</summary>
    public int LastEnabled() => NextEnabled(_filtered.Count, -1, false);

    /// <summary>
    /// Finds the next enabled option whose label starts with <paramref name="buffer"/>, searching forward
    /// (wrapping) from just after <paramref name="fromIndex"/> so repeated presses cycle. Returns -1 for no match.
    /// </summary>
    /// <param name="buffer">The accumulated type-ahead prefix.</param>
    /// <param name="fromIndex">The current highlight; the search starts at the next index.</param>
    public int TypeaheadMatch(string buffer, int fromIndex)
    {
        var n = _filtered.Count;
        if (n == 0 || string.IsNullOrEmpty(buffer)) return -1;
        for (var step = 1; step <= n; step++)
        {
            var i = ((fromIndex + step) % n + n) % n;
            if (IsDisabled(_filtered[i])) continue;
            if (_policy.Label(_filtered[i]).StartsWith(buffer, StringComparison.OrdinalIgnoreCase)) return i;
        }
        return -1;
    }

    /// <summary>
    /// The header+option row projection, cached and rebuilt only when the filtered set changes (so moving
    /// the highlight or the selection never re-materializes off-screen rows). This is the single point that
    /// eliminates the old per-render option allocation.
    /// </summary>
    public IReadOnlyList<ComboboxRow<TItem>> Rows => _rows ??= BuildRows();

    /// <summary>
    /// Projects the filtered options into a flat header+option row list for grouped rendering and grouped
    /// virtualization from one windowed list. Without grouping this is one option row per item.
    /// </summary>
    public IReadOnlyList<ComboboxRow<TItem>> BuildRows()
    {
        var rows = new List<ComboboxRow<TItem>>(_filtered.Count + 4);
        string? lastGroup = null;
        for (var i = 0; i < _filtered.Count; i++)
        {
            var item = _filtered[i];
            if (_policy.Group is not null)
            {
                var g = _policy.Group(item);
                if (!string.Equals(g, lastGroup, StringComparison.Ordinal))
                {
                    lastGroup = g;
                    rows.Add(new ComboboxRow<TItem>(true, g, default!, -1));
                }
            }
            rows.Add(new ComboboxRow<TItem>(false, null, item, i));
        }
        return rows;
    }
}
