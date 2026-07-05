namespace Flare.Components.Combobox;

/// <summary>
/// The single behaviour engine for the whole select family. One instance is <em>owned as a field</em> by
/// each shell (<c>FlareSelect</c>, <c>FlareMultiSelect</c>, <c>FlareCombobox</c>, <c>FlareTagField</c>) -
/// never inherited - and only the <see cref="ComboboxPolicy{TItem}"/> differs between them. It owns the
/// open, input (query) and highlight axes; delegates the item view to <see cref="ListCollection{TItem}"/>
/// and the selection axis to <see cref="SelectionManager{TItem}"/>. Highlight and selection are kept
/// strictly separate: hover/arrow/type-ahead move the highlight, and committing is the only bridge to the
/// selection. It raises <see cref="Changed"/> so the shell re-renders and <see cref="ScrollRequested"/> so
/// the shell scrolls the active row into view - but it never touches <c>ComponentBase</c>, a
/// <c>RenderFragment</c>, an <c>EventCallback</c> or JS, which keeps it fully unit-testable and is enforced
/// by a reflection guard test.
/// </summary>
/// <typeparam name="TItem">The option value type.</typeparam>
public sealed class ComboboxState<TItem>
{
    private readonly ComboboxPolicy<TItem> _policy;
    private string _typeBuffer = string.Empty;

    /// <summary>Creates the engine for the given policy.</summary>
    /// <param name="policy">The behaviour configuration (from a <c>ComboboxPolicy.For*</c> factory).</param>
    public ComboboxState(ComboboxPolicy<TItem> policy)
    {
        _policy = policy;
        Collection = new ListCollection<TItem>(policy);
        Selection = new SelectionManager<TItem>(policy);
    }

    /// <summary>The behaviour configuration in force.</summary>
    public ComboboxPolicy<TItem> Policy => _policy;

    /// <summary>The ordered / filtered item view.</summary>
    public ListCollection<TItem> Collection { get; }

    /// <summary>The selection axis (selected items + membership + select-all).</summary>
    public SelectionManager<TItem> Selection { get; }

    /// <summary>Whether the dropdown is open.</summary>
    public bool IsOpen { get; private set; }

    /// <summary>The current input / query text (editable variants).</summary>
    public string Input { get; private set; } = string.Empty;

    /// <summary>The highlighted option index into <see cref="ListCollection{TItem}.Filtered"/>; -1 = none.</summary>
    public int HighlightedIndex { get; private set; } = -1;

    /// <summary>The highlighted option value, or default when nothing is highlighted.</summary>
    public TItem? HighlightedItem =>
        HighlightedIndex >= 0 && HighlightedIndex < Collection.Count ? Collection[HighlightedIndex] : default;

    /// <summary>Raised after any intent mutates state; the shell subscribes to re-render.</summary>
    public event Action? Changed;

    /// <summary>Raised with the highlighted option index when the shell should scroll it into view (keyboard nav).</summary>
    public event Action<int>? ScrollRequested;

    // ---- Data feed (called by the shell in OnParametersSet; does not raise Changed) -----------------

    /// <summary>Replaces the source items (merged Items + declarative options). Re-applies the query and
    /// clamps the highlight.</summary>
    /// <param name="items">The new source items.</param>
    public void SetSource(IReadOnlyList<TItem>? items)
    {
        Collection.SetSource(items);
        if (_policy.Searchable) Collection.SetQuery(Input);
        if (HighlightedIndex >= Collection.Count) HighlightedIndex = Collection.Count - 1;
    }

    /// <summary>Replaces the selection (the shell pushes a controlled value in).</summary>
    /// <param name="items">The selected items, in order.</param>
    public void SetSelection(IEnumerable<TItem>? items) => Selection.Set(items);

    // ---- Open / close -------------------------------------------------------------------------------

    /// <summary>Opens the dropdown and highlights an option per <paramref name="strategy"/>.</summary>
    /// <param name="strategy">Which option to highlight on open.</param>
    public void Open(FocusStrategy strategy = FocusStrategy.KeepSelected)
    {
        if (IsOpen) return;
        IsOpen = true;
        HighlightedIndex = InitialHighlight(strategy);
        Raise();
    }

    /// <summary>Closes the dropdown, clears the highlight and the type-ahead buffer.</summary>
    public void Close()
    {
        if (!IsOpen && HighlightedIndex < 0) return;
        IsOpen = false;
        HighlightedIndex = -1;
        _typeBuffer = string.Empty;
        Raise();
    }

    /// <summary>Toggles the dropdown open/closed.</summary>
    public void ToggleOpen()
    {
        if (IsOpen) Close();
        else Open();
    }

    private int InitialHighlight(FocusStrategy strategy)
    {
        if (Collection.Count == 0) return -1;
        return strategy switch
        {
            FocusStrategy.First => Collection.FirstEnabled(),
            FocusStrategy.Last => Collection.LastEnabled(),
            _ => FirstSelectedIndex() is var s && s >= 0 ? s : Collection.FirstEnabled(),
        };
    }

    private int FirstSelectedIndex()
    {
        for (var i = 0; i < Collection.Count; i++)
            if (Selection.IsSelected(Collection[i])) return i;
        return -1;
    }

    // ---- Highlight ----------------------------------------------------------------------------------

    /// <summary>Moves the highlight one step (<paramref name="dir"/> = +1 down / -1 up); opens first if closed.</summary>
    /// <param name="dir">+1 to move down, -1 to move up.</param>
    public void MoveHighlight(int dir)
    {
        if (!IsOpen)
        {
            Open(dir > 0 ? FocusStrategy.First : FocusStrategy.Last);
            return;
        }
        var next = Collection.NextEnabled(HighlightedIndex, dir, _policy.WrapNavigation);
        if (next >= 0) SetHighlight(next, scroll: true);
    }

    /// <summary>Jumps the highlight to the first enabled option (Home).</summary>
    public void HighlightFirst() { if (IsOpen) SetHighlight(Collection.FirstEnabled(), scroll: true); }

    /// <summary>Jumps the highlight to the last enabled option (End).</summary>
    public void HighlightLast() { if (IsOpen) SetHighlight(Collection.LastEnabled(), scroll: true); }

    /// <summary>Sets the highlight to a specific option index (hover); does not scroll or open.</summary>
    /// <param name="index">The filtered index to highlight, or -1 to clear.</param>
    public void HighlightAt(int index) => SetHighlight(index, scroll: false);

    private void SetHighlight(int index, bool scroll)
    {
        HighlightedIndex = index;
        if (scroll && index >= 0) ScrollRequested?.Invoke(index);
        Raise();
    }

    // ---- Input / filter -----------------------------------------------------------------------------

    /// <summary>
    /// Sets the input/query text. For searchable variants this re-filters and auto-highlights the first
    /// match; it opens the dropdown when the policy opens on input. Never changes the selection.
    /// </summary>
    /// <param name="text">The new input text.</param>
    public void SetInput(string? text)
    {
        Input = text ?? string.Empty;
        if (_policy.Searchable)
        {
            Collection.SetQuery(Input);
            HighlightedIndex = Collection.FirstEnabled();
        }
        if (!IsOpen && _policy.MenuTrigger == MenuTrigger.Input) IsOpen = true;
        Raise();
    }

    // ---- Type-ahead (select-only first-letter navigation) -------------------------------------------

    /// <summary>
    /// Appends a printable character to the type-ahead buffer and moves the highlight to the next matching
    /// option (repeating the same character cycles). Opens the dropdown if closed. Returns whether it was
    /// handled. The shell must call <see cref="ResetTypeahead"/> after <see cref="ComboboxPolicy{TItem}.TypeaheadResetMs"/>.
    /// </summary>
    /// <param name="c">The typed character.</param>
    public bool TypeAhead(char c)
    {
        if (char.IsControl(c)) return false;
        if (!IsOpen) Open();

        _typeBuffer += char.ToLowerInvariant(c);

        string needle;
        int from;
        if (_typeBuffer.Length > 1 && AllSameChar(_typeBuffer))
        {
            needle = _typeBuffer[0].ToString();
            from = HighlightedIndex;            // cycle to the next match
        }
        else
        {
            needle = _typeBuffer;
            from = HighlightedIndex - 1;         // re-check current so a growing prefix can stay put
        }

        var match = Collection.TypeaheadMatch(needle, from);
        if (match >= 0) SetHighlight(match, scroll: true);
        else Raise();
        return true;
    }

    /// <summary>Clears the type-ahead buffer (called by the shell's inactivity timer).</summary>
    public void ResetTypeahead() => _typeBuffer = string.Empty;

    private static bool AllSameChar(string s)
    {
        for (var i = 1; i < s.Length; i++)
            if (s[i] != s[0]) return false;
        return true;
    }

    // ---- Commit -------------------------------------------------------------------------------------

    /// <summary>Commits a specific option (a click or a resolved custom value).</summary>
    /// <param name="item">The item to commit.</param>
    public CommitResult Commit(TItem item)
    {
        if (Collection.IsDisabled(item)) return new CommitResult(CommitKind.RejectedDisabled, false);

        var kind = Selection.Toggle(item);
        if (kind != CommitKind.Changed) return new CommitResult(kind, false);

        var closed = false;
        if (_policy.CloseOnSelect)
        {
            IsOpen = false;
            HighlightedIndex = -1;
            closed = true;
        }
        if (_policy.ClearFilterOnSelect)
        {
            Input = string.Empty;
            Collection.SetQuery(string.Empty);
            HighlightedIndex = IsOpen ? Collection.FirstEnabled() : -1;
        }
        _typeBuffer = string.Empty;
        Raise();
        return new CommitResult(kind, closed);
    }

    /// <summary>Commits the currently highlighted option (Enter / Space), or NoOp when none is highlighted.</summary>
    public CommitResult CommitHighlighted()
    {
        if (HighlightedIndex < 0 || HighlightedIndex >= Collection.Count)
            return new CommitResult(CommitKind.NoOp, false);
        return Commit(Collection[HighlightedIndex]);
    }

    /// <summary>Toggles "select all" over the currently visible enabled options (multi only).</summary>
    public CommitResult ToggleSelectAll()
    {
        var visible = VisibleEnabled();
        var kind = Selection.ToggleSelectAll(visible);
        Raise();
        return new CommitResult(kind, false);
    }

    /// <summary>The tri-state of "select all" over the currently visible enabled options.</summary>
    public TriState SelectAllState() => Selection.SelectAllState(VisibleEnabled());

    /// <summary>Removes the most recently selected item (Backspace on an empty query); returns whether it removed.</summary>
    public bool RemoveLast()
    {
        if (!Selection.RemoveLast()) return false;
        Raise();
        return true;
    }

    private List<TItem> VisibleEnabled()
    {
        var list = new List<TItem>(Collection.Count);
        for (var i = 0; i < Collection.Count; i++)
        {
            var item = Collection[i];
            if (!Collection.IsDisabled(item)) list.Add(item);
        }
        return list;
    }

    private void Raise() => Changed?.Invoke();
}
