namespace Flare.Components.Combobox;

/// <summary>How many items a combobox may hold selected at once.</summary>
public enum SelectionMode
{
    /// <summary>At most one selected value (FlareSelect, FlareCombobox).</summary>
    Single,

    /// <summary>Any number of selected values (FlareMultiSelect, FlareTagField).</summary>
    Multiple,
}

/// <summary>What user gesture opens the dropdown.</summary>
public enum MenuTrigger
{
    /// <summary>Opens on a pointer click / Enter / Space / Arrow on the trigger (select-only pattern).</summary>
    Click,

    /// <summary>Opens as the user types in an editable input (combobox pattern).</summary>
    Input,

    /// <summary>Opens as soon as the trigger gains focus.</summary>
    Focus,

    /// <summary>Never opens on its own; the host drives <see cref="ComboboxState{TItem}.Open"/> explicitly.</summary>
    Manual,
}

/// <summary>Which option to highlight when the dropdown opens.</summary>
public enum FocusStrategy
{
    /// <summary>Highlight the first enabled option.</summary>
    First,

    /// <summary>Highlight the last enabled option.</summary>
    Last,

    /// <summary>Highlight the (first) selected option if any, otherwise the first enabled option.</summary>
    KeepSelected,
}

/// <summary>Tri-state of a "select all" affordance over the currently visible, enabled options.</summary>
public enum TriState
{
    /// <summary>None of the visible enabled options are selected.</summary>
    None,

    /// <summary>Some but not all are selected (renders as the mixed / indeterminate state).</summary>
    Some,

    /// <summary>All visible enabled options are selected.</summary>
    All,
}

/// <summary>The outcome of a commit intent, so the shell can forward the right consumer callback.</summary>
public enum CommitKind
{
    /// <summary>Nothing happened (no highlighted item, empty commit).</summary>
    NoOp,

    /// <summary>The selection set changed (added or removed); the shell should raise its value callback.</summary>
    Changed,

    /// <summary>Rejected: the maximum selection count was already reached.</summary>
    RejectedMax,

    /// <summary>Rejected: the target option is disabled.</summary>
    RejectedDisabled,

    /// <summary>Rejected: removing would empty a selection that must keep at least one item.</summary>
    RejectedEmpty,
}

/// <summary>The result of a commit intent on <see cref="ComboboxState{TItem}"/>.</summary>
/// <param name="Kind">What the engine did (or why it declined).</param>
/// <param name="MenuClosed">True when the commit also closed the dropdown (single-select close-on-select).</param>
public readonly record struct CommitResult(CommitKind Kind, bool MenuClosed)
{
    /// <summary>True when the selection set actually changed, so the shell must raise its value callback.</summary>
    public bool ChangedSelection => Kind == CommitKind.Changed;

    /// <summary>True when the commit was declined because the maximum selection count was reached.</summary>
    public bool RejectedForMax => Kind == CommitKind.RejectedMax;
}
