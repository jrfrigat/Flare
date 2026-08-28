using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// Single-value dropdown over a list of items: a select-only combobox by default, or a filtering one with
/// <c>Searchable</c>. Rows render as plain labels, as an <c>ItemTemplate</c>, or - with <c>NullOption</c> -
/// as a pinned "no value" row, so a filter select expresses "all" without a sentinel value.
/// </summary>
/// <typeparam name="TValue">Type of the bound value and of the items.</typeparam>
public partial class FlareSelect<TValue>
{
    /// <summary>
    /// Text of a selectable "no value" row prepended to the list - the "All ingredients" / "Any" case.
    /// Choosing it sets <see cref="Value"/> to null, and the closed field then shows this text rather than
    /// the placeholder, so a filter select needs no sentinel value of its own.
    /// <para>
    /// The row is pinned: it stays at the top and survives typing, because a row meaning "no filter" is
    /// useless if searching hides it. <see cref="Clearable"/> composes with it - clearing selects it.
    /// </para>
    /// <para>
    /// Requires a nullable <typeparamref name="TValue"/>. For a struct such as an int or an enum the
    /// default is a real value and cannot mean "none", so binding the nullable form is the only way to
    /// express it; setting this on a non-nullable value type throws rather than selecting zero.
    /// </para>
    /// </summary>
    [Parameter] public string? NullOption { get; set; }

    /// <summary>Rich content for the <see cref="NullOption"/> row, used both in the list and in the closed
    /// field. Falls back to the <see cref="NullOption"/> text.</summary>
    [Parameter] public RenderFragment? NullOptionTemplate { get; set; }

    private bool HasNullOption => NullOption is not null;

    // The null option and the placeholder answer different questions - "no value is chosen, and that is a
    // choice" versus "nothing has been chosen yet" - so only one of them ever shows.
    private bool ShowNullOption => Current is null && !_editing && HasNullOption;

    private static bool IsNullOption(TValue item) => item is null;

    // Fails loudly rather than silently selecting zero: on a non-nullable struct default(TValue) is a real
    // value, so a "no value" row cannot exist and the caller has to bind the nullable form.
    private void GuardNullOption()
    {
        if (!HasNullOption || default(TValue) is null) return;
        throw new InvalidOperationException(
            "FlareSelect: NullOption requires a nullable TValue. TValue is " + typeof(TValue).Name
            + ", whose default is a real value and cannot mean 'no value'. Bind the nullable form instead.");
    }

    // The null row is a real source item, so selection, keyboard navigation and the highlight index all
    // work on it without a second code path in the engine.
    private IReadOnlyList<TValue> WithNullOption(IReadOnlyList<TValue> items)
    {
        if (!HasNullOption) return items;
        var withNull = new List<TValue>(items.Count + 1);
        withNull.Add(default!);
        foreach (var item in items)
            if (item is not null) withNull.Add(item);
        return withNull;
    }

    // The list renders items through ItemTemplate; the null row needs its own content, so the template
    // handed to the option list dispatches on the row instead of the option list learning about it.
    private RenderFragment<TValue>? RowTemplate => ItemTemplate is null && NullOptionTemplate is null
        ? null
        : RenderRow;

    private RenderFragment RenderRow(TValue item) => builder => BuildRow(builder, item);

    private void BuildRow(RenderTreeBuilder builder, TValue item)
    {
        if (item is null)
        {
            if (NullOptionTemplate is not null) builder.AddContent(0, NullOptionTemplate);
            else builder.AddContent(1, NullOption);
        }
        else if (ItemTemplate is not null)
        {
            builder.AddContent(2, ItemTemplate(item));
        }
        else
        {
            builder.AddContent(3, ResolveLabel(item));
        }
    }
}
