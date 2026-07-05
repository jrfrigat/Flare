using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Single-value dropdown select: a custom combobox trigger (not a native <c>&lt;select&gt;</c>) that
/// renders the selected value as rich <see cref="FlareSelectBase{TValue}.ItemTemplate"/> content, with an
/// optionally searchable, groupable and virtualized dropdown. Shares its whole implementation with
/// <see cref="FlareMultiSelect{TValue}"/> via <see cref="FlareSelectBase{TValue}"/>; this facade only adds
/// the single-value binding and validation.
/// </summary>
/// <typeparam name="TValue">The selectable value type.</typeparam>
public class FlareSelect<TValue> : FlareSelectBase<TValue>, IFlareField<TValue>
{
    /// <summary>Currently selected item value (supports <c>@bind-Value</c>).</summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>Callback invoked when the selection changes, enabling <c>@bind-Value</c>.</summary>
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>Expression used to bind and validate the field against an <c>EditContext</c>.</summary>
    [Parameter] public Expression<Func<TValue>>? For { get; set; }

    /// <inheritdoc />
    protected override bool Multiple => false;
    /// <inheritdoc />
    protected override bool CloseOnSelect => true;

    /// <inheritdoc />
    protected override string RootCssClass => Css.Classes.Select.Root;
    /// <inheritdoc />
    protected override string OpenModifierClass => Css.Classes.Select.Open;
    /// <inheritdoc />
    protected override string ControlCssClass => Css.Classes.Select.Control;
    /// <inheritdoc />
    protected override string ValueCssClass => Css.Classes.Select.Value;
    /// <inheritdoc />
    protected override string OptionCssClass => Css.Classes.Select.Option;
    /// <inheritdoc />
    protected override string DropdownCssClass => Css.Classes.Select.Dropdown;

    // Controlled vs uncontrolled: when the consumer two-way-binds (ValueChanged has a delegate) they own
    // the value and Value is the source of truth. Otherwise the component keeps its own selection (seeded
    // from any one-way Value), so a bare `<FlareSelect Items="..." />` works like a native <select>.
    private TValue? _internal;
    private TValue? _prevValue;
    private bool _seeded;

    private bool Controlled => ValueChanged.HasDelegate;
    private TValue? Current => Controlled ? Value : _internal;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Controlled)
        {
            _internal = Value;
        }
        else if (!_seeded || !EqualityComparer<TValue?>.Default.Equals(Value, _prevValue))
        {
            // Seed once, and re-seed only when the consumer explicitly changes the one-way Value (reset).
            _internal = Value;
            _seeded = true;
        }
        _prevValue = Value;
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override IReadOnlyList<TValue> SelectedValues =>
        Current is null ? System.Array.Empty<TValue>() : new[] { Current };

    /// <inheritdoc />
    protected override bool IsSelected(TValue item) =>
        EqualityComparer<TValue>.Default.Equals(item, Current);

    /// <inheritdoc />
    protected override async Task CommitAsync(TValue item)
    {
        if (EqualityComparer<TValue>.Default.Equals(item, Current)) return;
        _internal = item;   // drives the display in uncontrolled mode
        await ValueChanged.InvokeAsync(item);
        NotifyFieldChanged();
    }

    /// <inheritdoc />
    protected override void UpdateBoundField() => UpdateFieldIdentifier(For);
}
