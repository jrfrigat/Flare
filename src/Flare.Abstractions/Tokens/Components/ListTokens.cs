using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareList</c> / <c>FlareListItem</c> (the container and its rows).</summary>
public sealed record ListTokens
{
    // --- 1. CONTAINER ---
    /// <summary>Background of the list container.</summary>
    [CssVar(ListField.Bg)] public required string Bg { get; init; }
    /// <summary>Corner radius of the list container. This used to read the CARD's radius, which meant a
    /// theme could not reshape one without reshaping the other.</summary>
    [CssVar(ListField.Radius)] public required string Radius { get; init; }
    /// <summary>Rule drawn between adjacent items, as a CSS <c>border</c> shorthand. A theme whose rows
    /// are separated by space rather than a line parks this at <c>none</c>.</summary>
    [CssVar(ListField.Divider)] public required string Divider { get; init; }

    // --- 2. ITEM (geometry) ---
    /// <summary>Floor on the height of a one-line item.</summary>
    [CssVar(ListField.ItemHeight)] public required string ItemHeight { get; init; }
    /// <summary>Floor on the height of an item carrying supporting text.</summary>
    [CssVar(ListField.ItemHeightTwoLine)] public required string ItemHeightTwoLine { get; init; }
    /// <summary>Floor on the height of a one-line item when the list is marked dense.</summary>
    [CssVar(ListField.ItemHeightDense)] public required string ItemHeightDense { get; init; }
    /// <summary>Floor on the height of a two-line item when the list is marked dense.</summary>
    [CssVar(ListField.ItemHeightTwoLineDense)] public required string ItemHeightTwoLineDense { get; init; }
    /// <summary>Space above and below an item's content.</summary>
    [CssVar(ListField.ItemPaddingBlock)] public required string ItemPaddingBlock { get; init; }
    /// <summary>Space above and below an item's content when the list is marked dense.</summary>
    [CssVar(ListField.ItemPaddingBlockDense)] public required string ItemPaddingBlockDense { get; init; }
    /// <summary>Space between an item's side edges and its content.</summary>
    [CssVar(ListField.ItemPaddingInline)] public required string ItemPaddingInline { get; init; }
    /// <summary>Space between an item's leading slot, text and trailing slot.</summary>
    [CssVar(ListField.ItemGap)] public required string ItemGap { get; init; }
    /// <summary>Space between an item's primary line and its supporting text.</summary>
    [CssVar(ListField.ItemContentGap)] public required string ItemContentGap { get; init; }
    /// <summary>Corner radius of an item. A theme with flush rows parks this at <c>0</c>; a theme that
    /// renders rows as separated pills raises it.</summary>
    [CssVar(ListField.ItemRadius)] public required string ItemRadius { get; init; }

    // --- 3. ITEM (paint) ---
    /// <summary>Font family of an item's label. Each theme decides which step of its own type scale a
    /// list row maps to.</summary>
    [CssVar(ListField.ItemLabelFont)] public required string ItemLabelFont { get; init; }
    /// <summary>Font size of an item's label.</summary>
    [CssVar(ListField.ItemLabelSize)] public required string ItemLabelSize { get; init; }
    /// <summary>Foreground of an item.</summary>
    [CssVar(ListField.ItemColor)] public required string ItemColor { get; init; }
    /// <summary>Foreground of an item's trailing slot, which most languages mute relative to the label.</summary>
    [CssVar(ListField.ItemTrailingColor)] public required string ItemTrailingColor { get; init; }
    /// <summary>Background of a selected item.</summary>
    [CssVar(ListField.ItemSelectedBg)] public required string ItemSelectedBg { get; init; }
    /// <summary>Foreground of a selected item.</summary>
    [CssVar(ListField.ItemSelectedColor)] public required string ItemSelectedColor { get; init; }
    /// <summary>How far a disabled item fades. A language that repaints disabled rows in a flat palette
    /// leaves this opaque and carries the change in its own stylesheet, since a foreground colour has no
    /// value meaning "leave this as painted".</summary>
    [CssVar(ListField.ItemDisabledOpacity)] public required string ItemDisabledOpacity { get; init; }
}
