namespace Flare.Components.Combobox;

/// <summary>
/// The immutable behaviour configuration for a <see cref="ComboboxState{TItem}"/>. A shell builds one
/// from a named factory (<see cref="ForSingleSelect"/> etc.) and refines it with <c>with { ... }</c> to
/// attach the label/group/disabled/filter delegates. Because the four valid shapes are enumerated by the
/// factories, nonsensical combinations (for example chips on a single select) are never assembled, and
/// <see cref="SelectionMode"/> lives here as a construction knob rather than as a render-time branch -
/// there is no <c>Multiple ? ...</c> ternary anywhere in the family markup.
/// </summary>
/// <typeparam name="TItem">The option value type.</typeparam>
public sealed record ComboboxPolicy<TItem>
{
    /// <summary>Single- vs multi-value selection.</summary>
    public SelectionMode SelectionMode { get; init; }

    /// <summary>What gesture opens the dropdown.</summary>
    public MenuTrigger MenuTrigger { get; init; }

    /// <summary>Closes the dropdown after a commit (single-select); multi keeps it open.</summary>
    public bool CloseOnSelect { get; init; }

    /// <summary>Clears the filter query after a commit (multi/tag, so the next pick starts fresh).</summary>
    public bool ClearFilterOnSelect { get; init; }

    /// <summary>Allows committing a typed value that is not in the item list (free typing).</summary>
    public bool AllowsCustomValue { get; init; }

    /// <summary>The dropdown filters its options against the typed query (editable combobox).</summary>
    public bool Searchable { get; init; }

    /// <summary>Maximum number of selected items; 0 means unlimited.</summary>
    public int MaxSelections { get; init; }

    /// <summary>Prevents deselecting the last remaining item (radio-like guarantee).</summary>
    public bool DisallowEmpty { get; init; }

    /// <summary>Arrow navigation wraps around the ends instead of stopping.</summary>
    public bool WrapNavigation { get; init; }

    /// <summary>Milliseconds of inactivity after which the type-ahead buffer resets. The shell owns the timer.</summary>
    public int TypeaheadResetMs { get; init; } = 500;

    /// <summary>Resolves an item's display text (used for labels, filtering and type-ahead).</summary>
    public Func<TItem, string> Label { get; init; } = static i => i?.ToString() ?? string.Empty;

    /// <summary>Returns an item's group label; when non-null, options are grouped contiguously under headers.</summary>
    public Func<TItem, string>? Group { get; init; }

    /// <summary>Returns whether an item is disabled (perceivable, highlightable, never committable).</summary>
    public Func<TItem, bool>? ItemDisabled { get; init; }

    /// <summary>Equality used for selection membership and item lookup. Defaults to the type's default comparer.</summary>
    public IEqualityComparer<TItem> Comparer { get; init; } = EqualityComparer<TItem>.Default;

    /// <summary>Custom filter predicate <c>(item, query) =&gt; keep</c>. When null, a case-insensitive
    /// contains-on-label filter is used. Ignored when <see cref="RankScorer"/> is set.</summary>
    public Func<TItem, string, bool>? FilterPredicate { get; init; }

    /// <summary>Custom relevance scorer <c>(item, query) =&gt; score</c>; only positive scores are kept,
    /// ordered best-first. Overrides <see cref="FilterPredicate"/> and <see cref="Fuzzy"/>.</summary>
    public Func<TItem, string, double>? RankScorer { get; init; }

    /// <summary>Ranks matches by fuzzy relevance (via <see cref="FlareSearch"/>) instead of insertion order.</summary>
    public bool Fuzzy { get; init; }

    /// <summary>Configuration for a single-value select (click to open, close on select).</summary>
    public static ComboboxPolicy<TItem> ForSingleSelect() => new()
    {
        SelectionMode = SelectionMode.Single,
        MenuTrigger = MenuTrigger.Click,
        CloseOnSelect = true,
    };

    /// <summary>Configuration for a multi-value select (click to open, stay open, clear filter on each pick).</summary>
    public static ComboboxPolicy<TItem> ForMultiSelect() => new()
    {
        SelectionMode = SelectionMode.Multiple,
        MenuTrigger = MenuTrigger.Click,
        CloseOnSelect = false,
        ClearFilterOnSelect = true,
    };

    /// <summary>Configuration for a single-value editable combobox (open on input, filter, close on select).</summary>
    public static ComboboxPolicy<TItem> ForCombobox() => new()
    {
        SelectionMode = SelectionMode.Single,
        MenuTrigger = MenuTrigger.Input,
        Searchable = true,
        CloseOnSelect = true,
    };

    /// <summary>Configuration for a free-typing multi-value tag field (open on input, custom values, stay open).</summary>
    public static ComboboxPolicy<TItem> ForTagField() => new()
    {
        SelectionMode = SelectionMode.Multiple,
        MenuTrigger = MenuTrigger.Input,
        Searchable = true,
        AllowsCustomValue = true,
        CloseOnSelect = false,
        ClearFilterOnSelect = true,
    };
}
