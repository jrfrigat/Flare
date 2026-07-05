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

    /// <inheritdoc />
    protected override IReadOnlyList<TValue> SelectedValues =>
        Value is null ? System.Array.Empty<TValue>() : new[] { Value };

    /// <inheritdoc />
    protected override bool IsSelected(TValue item) =>
        EqualityComparer<TValue>.Default.Equals(item, Value);

    /// <inheritdoc />
    protected override async Task CommitAsync(TValue item)
    {
        if (EqualityComparer<TValue>.Default.Equals(item, Value)) return;
        await ValueChanged.InvokeAsync(item);
        NotifyFieldChanged();
    }

    /// <inheritdoc />
    protected override void UpdateBoundField() => UpdateFieldIdentifier(For);
}
