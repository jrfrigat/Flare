using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Multi-value dropdown select: a custom combobox trigger that renders the selection as a comma list or
/// removable chips, with an optionally searchable, groupable, virtualized dropdown of leading-checkbox
/// rows and an optional select-all. Shares its whole implementation with <see cref="FlareSelect{TValue}"/>
/// via <see cref="FlareSelectBase{TValue}"/>; this facade only adds the collection binding, validation and
/// the chips / select-all opt-ins.
/// </summary>
/// <typeparam name="TValue">The element type of the selectable values.</typeparam>
public class FlareMultiSelect<TValue> : FlareSelectBase<TValue>, IFlareMultiField<TValue>
{
    /// <summary>The bound collection of selected values (supports <c>@bind-Values</c>).</summary>
    [Parameter] public IReadOnlyList<TValue> Values { get; set; } = System.Array.Empty<TValue>();

    /// <summary>Callback invoked when the selection changes, enabling <c>@bind-Values</c>.</summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }

    /// <summary>Expression used to bind and validate the field against an <c>EditContext</c>.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<TValue>>>? For { get; set; }

    /// <summary>Renders the selected values as removable chips in the control (instead of a comma list).</summary>
    [Parameter] public bool Chips { get; set; }

    /// <summary>Shows a select-all checkbox row at the top of the dropdown.</summary>
    [Parameter] public bool ShowSelectAll { get; set; }

    /// <inheritdoc />
    protected override bool Multiple => true;
    /// <inheritdoc />
    protected override bool CloseOnSelect => false;
    /// <inheritdoc />
    protected override bool ChipsEnabled => Chips;
    /// <inheritdoc />
    protected override bool ShowSelectAllEnabled => ShowSelectAll;

    /// <inheritdoc />
    protected override string RootCssClass => Css.Classes.Multiselect.Root;
    /// <inheritdoc />
    protected override string OpenModifierClass => Css.Classes.Multiselect.Open;
    /// <inheritdoc />
    protected override string ControlCssClass => Css.Classes.Multiselect.Control;
    /// <inheritdoc />
    protected override string ValueCssClass => Css.Classes.Multiselect.Value;
    /// <inheritdoc />
    protected override string OptionCssClass => Css.Classes.Multiselect.Option;
    /// <inheritdoc />
    protected override string DropdownCssClass => Css.Classes.Multiselect.Dropdown;

    /// <inheritdoc />
    protected override IReadOnlyList<TValue> SelectedValues => Current;

    // Controlled vs uncontrolled: when the consumer two-way-binds (ValuesChanged has a delegate) they own
    // the collection and Values is the source of truth. Otherwise the component keeps its own selection
    // (seeded from any one-way Values), so a bare `<FlareMultiSelect Items="..." />` works standalone.
    private IReadOnlyList<TValue> _internal = System.Array.Empty<TValue>();
    private IReadOnlyList<TValue>? _prevValues;
    private bool _seeded;

    private bool Controlled => ValuesChanged.HasDelegate;
    private IReadOnlyList<TValue> Current => Controlled ? Values : _internal;

    // O(1) membership for the option rows + chips, rebuilt only when the effective collection changes.
    private HashSet<TValue> _selected = new();

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Controlled)
        {
            _internal = Values;
        }
        else if (!_seeded || !ReferenceEquals(Values, _prevValues))
        {
            // Seed once, and re-seed only when the consumer explicitly swaps the one-way Values (reset).
            _internal = Values;
            _seeded = true;
        }
        _prevValues = Values;
        _selected = new HashSet<TValue>(Current);
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override bool IsSelected(TValue item) => _selected.Contains(item);

    /// <inheritdoc />
    protected override async Task CommitAsync(TValue item)
    {
        var set = new HashSet<TValue>(Current);
        if (!set.Remove(item)) set.Add(item);
        Apply(set);
        await ValuesChanged.InvokeAsync(_internal);
        NotifyFieldChanged();
    }

    /// <inheritdoc />
    protected override async Task ToggleSelectAllAsync(IReadOnlyList<TValue> visible)
    {
        var set = new HashSet<TValue>(Current);
        var allSelected = visible.Count > 0 && visible.All(set.Contains);
        if (allSelected)
            foreach (var item in visible) set.Remove(item);
        else
            foreach (var item in visible) set.Add(item);
        Apply(set);
        await ValuesChanged.InvokeAsync(_internal);
        NotifyFieldChanged();
    }

    // Commit a new selection set: update the internal value + the O(1) lookup so it displays immediately
    // (uncontrolled), before raising ValuesChanged (which the parent may echo back when controlled).
    private void Apply(HashSet<TValue> set)
    {
        _internal = set.ToArray();
        _selected = set;
    }

    /// <inheritdoc />
    protected override void UpdateBoundField() => UpdateFieldIdentifier(For);
}
