using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// One column of a typed <see cref="FlareTable{TItem}"/> when the table is described by
/// <see cref="FlareTable{TItem}.Columns"/> instead of raw table markup. A column renders its header
/// once and its <see cref="Field"/> or <see cref="Cell"/> for every item.
/// </summary>
/// <typeparam name="TItem">Row type the table shows.</typeparam>
public sealed class FlareTableColumn<TItem>
{
    /// <summary>Text shown in the column's header cell.</summary>
    public string Header { get; init; } = string.Empty;

    /// <summary>Value rendered as plain text in each cell, formatted with the current culture.
    /// Ignored when <see cref="Cell"/> is set.</summary>
    public Func<TItem, object?>? Field { get; init; }

    /// <summary>Custom cell content - a badge, a link, a stack of several pieces. When set it
    /// replaces the plain <see cref="Field"/> text.</summary>
    public Func<TItem, RenderFragment>? Cell { get; init; }
}
