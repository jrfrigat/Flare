namespace Flare.Components.Combobox;

/// <summary>
/// The selection axis for one <see cref="ComboboxState{TItem}"/>: which items are selected, in insertion
/// order (so chips and the comma list read naturally), with O(1) membership. Deliberately separate from the
/// highlight axis (which lives on <see cref="ComboboxState{TItem}"/> as an index) - highlight moving on
/// hover/arrow must never change selection, and committing is the only bridge between the two. Pure logic:
/// no DOM, no components, unit-testable with plain xUnit.
/// </summary>
/// <typeparam name="TItem">The option value type.</typeparam>
public sealed class SelectionManager<TItem>
{
    private readonly ComboboxPolicy<TItem> _policy;
    private readonly List<TItem> _order;      // selection order, drives chip / comma-list display
    private HashSet<TItem> _set;               // O(1) membership for per-option render

    /// <summary>Creates an empty selection for the given policy (mode, comparer, max, disallow-empty).</summary>
    /// <param name="policy">The owning combobox policy.</param>
    public SelectionManager(ComboboxPolicy<TItem> policy)
    {
        _policy = policy;
        _order = new List<TItem>();
        _set = new HashSet<TItem>(policy.Comparer);
    }

    /// <summary>Single- vs multi-value selection (from the policy).</summary>
    public SelectionMode Mode => _policy.SelectionMode;

    /// <summary>The selected items in selection order.</summary>
    public IReadOnlyList<TItem> Selected => _order;

    /// <summary>Number of selected items.</summary>
    public int Count => _order.Count;

    /// <summary>Whether <paramref name="item"/> is currently selected (O(1)).</summary>
    /// <param name="item">The item to test.</param>
    public bool IsSelected(TItem item) => _set.Contains(item);

    /// <summary>Replaces the whole selection (used by the shell to push a controlled value in).</summary>
    /// <param name="items">The new selected items, in order (null clears).</param>
    public void Set(IEnumerable<TItem>? items)
    {
        _order.Clear();
        _set = new HashSet<TItem>(_policy.Comparer);
        if (items is null) return;
        foreach (var i in items)
            if (_set.Add(i)) _order.Add(i);
    }

    /// <summary>
    /// Commits <paramref name="item"/>: replaces the selection in single mode, toggles membership in multi
    /// mode. Enforces max-selections and disallow-empty. Returns what happened so the shell can forward the
    /// right callback.
    /// </summary>
    /// <param name="item">The item to commit.</param>
    public CommitKind Toggle(TItem item)
    {
        if (_policy.SelectionMode == SelectionMode.Single)
        {
            _order.Clear();
            _set.Clear();
            _set.Add(item);
            _order.Add(item);
            return CommitKind.Changed;
        }

        if (_set.Contains(item))
        {
            if (_policy.DisallowEmpty && _order.Count == 1) return CommitKind.RejectedEmpty;
            Remove(item);
            return CommitKind.Changed;
        }

        if (_policy.MaxSelections > 0 && _order.Count >= _policy.MaxSelections) return CommitKind.RejectedMax;
        _set.Add(item);
        _order.Add(item);
        return CommitKind.Changed;
    }

    /// <summary>Removes the most recently added item (Backspace on an empty query). Returns false when empty.</summary>
    public bool RemoveLast()
    {
        if (_order.Count == 0) return false;
        if (_policy.DisallowEmpty && _order.Count == 1) return false;
        Remove(_order[^1]);
        return true;
    }

    private void Remove(TItem item)
    {
        _set.Remove(item);
        for (var i = 0; i < _order.Count; i++)
            if (_policy.Comparer.Equals(_order[i], item)) { _order.RemoveAt(i); break; }
    }

    /// <summary>
    /// The tri-state of a "select all" control over <paramref name="visibleEnabled"/>: All when every one is
    /// selected, None when none are, Some otherwise (renders as the mixed / indeterminate checkbox state).
    /// </summary>
    /// <param name="visibleEnabled">The currently visible, enabled options.</param>
    public TriState SelectAllState(IReadOnlyList<TItem> visibleEnabled)
    {
        if (visibleEnabled.Count == 0) return TriState.None;
        var selected = 0;
        foreach (var i in visibleEnabled)
            if (_set.Contains(i)) selected++;
        if (selected == 0) return TriState.None;
        return selected == visibleEnabled.Count ? TriState.All : TriState.Some;
    }

    /// <summary>
    /// Toggles "select all" over <paramref name="visibleEnabled"/>: deselects them all when all are selected,
    /// otherwise selects as many as the max-selections cap allows. Disabled items are excluded by the caller.
    /// </summary>
    /// <param name="visibleEnabled">The currently visible, enabled options.</param>
    public CommitKind ToggleSelectAll(IReadOnlyList<TItem> visibleEnabled)
    {
        var all = true;
        foreach (var i in visibleEnabled)
            if (!_set.Contains(i)) { all = false; break; }

        if (all)
        {
            foreach (var i in visibleEnabled) Remove(i);
        }
        else
        {
            foreach (var i in visibleEnabled)
            {
                if (_policy.MaxSelections > 0 && _order.Count >= _policy.MaxSelections) break;
                if (_set.Add(i)) _order.Add(i);
            }
        }
        return CommitKind.Changed;
    }
}
