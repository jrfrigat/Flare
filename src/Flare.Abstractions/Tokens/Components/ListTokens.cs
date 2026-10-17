using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareList</c> / <c>FlareListItem</c> (the container and its rows).</summary>
public sealed record ListTokens
{
    /// <summary>Background of the list container.</summary>
    [CssVar(ListField.Bg)] public string Bg { get; init; } = "var(--flare-color-surface)";
    /// <summary>Corner radius of the list container, its own rather than the card's, so a theme can
    /// reshape one without reshaping the other.</summary>
    [CssVar(ListField.Radius)] public string Radius { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Rule drawn between adjacent items, as a CSS <c>border</c> shorthand. A theme whose rows
    /// are separated by space rather than a line parks this at <c>none</c>.</summary>
    [CssVar(ListField.Divider)] public string Divider { get; init; } = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline-variant)";

    /// <summary>Floor on the height of a one-line item.</summary>
    [CssVar(ListField.ItemHeight)] public required string ItemHeight { get; init; }
    /// <summary>Floor on the height of an item carrying supporting text.</summary>
    [CssVar(ListField.ItemHeightTwoLine)] public required string ItemHeightTwoLine { get; init; }
    /// <summary>Floor on the height of a one-line item when the list is marked dense.</summary>
    [CssVar(ListField.ItemHeightDense)] public required string ItemHeightDense { get; init; }
    /// <summary>Floor on the height of a two-line item when the list is marked dense.</summary>
    [CssVar(ListField.ItemHeightTwoLineDense)] public required string ItemHeightTwoLineDense { get; init; }
    /// <summary>Space above and below an item's content.</summary>
    [CssVar(ListField.ItemPaddingBlock)] public string ItemPaddingBlock { get; init; } = "var(--flare-spacing-6)";
    /// <summary>Space above and below an item's content when the list is marked dense.</summary>
    [CssVar(ListField.ItemPaddingBlockDense)] public string ItemPaddingBlockDense { get; init; } = "var(--flare-spacing-3)";
    /// <summary>Space between an item's side edges and its content.</summary>
    [CssVar(ListField.ItemPaddingInline)] public string ItemPaddingInline { get; init; } = "var(--flare-spacing-8)";
    /// <summary>Space between an item's leading slot, text and trailing slot.</summary>
    [CssVar(ListField.ItemGap)] public string ItemGap { get; init; } = "var(--flare-spacing-8)";
    /// <summary>Space between an item's primary line and its supporting text.</summary>
    [CssVar(ListField.ItemContentGap)] public string ItemContentGap { get; init; } = "var(--flare-spacing-1)";
    /// <summary>Corner radius of an item. A theme with flush rows parks this at <c>0</c>; a theme that
    /// renders rows as separated pills raises it.</summary>
    [CssVar(ListField.ItemRadius)] public required string ItemRadius { get; init; }

    /// <summary>Font family of an item's label. Each theme decides which step of its own type scale a
    /// list row maps to.</summary>
    [CssVar(ListField.ItemLabelFont)] public string ItemLabelFont { get; init; } = "var(--flare-typescale-body-large-font)";
    /// <summary>Font size of an item's label.</summary>
    [CssVar(ListField.ItemLabelSize)] public string ItemLabelSize { get; init; } = "var(--flare-typescale-body-large-size)";
    /// <summary>Foreground of an item.</summary>
    [CssVar(ListField.ItemColor)] public string ItemColor { get; init; } = "var(--flare-color-on-surface)";
    /// <summary>Foreground of an item's trailing slot, which most languages mute relative to the label.</summary>
    [CssVar(ListField.ItemTrailingColor)] public string ItemTrailingColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Background of a selected item.</summary>
    [CssVar(ListField.ItemSelectedBg)] public string ItemSelectedBg { get; init; } = "var(--flare-color-secondary-container)";
    /// <summary>Foreground of a selected item.</summary>
    [CssVar(ListField.ItemSelectedColor)] public string ItemSelectedColor { get; init; } = "var(--flare-color-on-secondary-container)";
    /// <summary>How far a disabled item fades. A language that repaints disabled rows in a flat palette
    /// leaves this opaque and carries the change in its own stylesheet, since a foreground colour has no
    /// value meaning "leave this as painted".</summary>
    [CssVar(ListField.ItemDisabledOpacity)] public string ItemDisabledOpacity { get; init; } = "var(--flare-state-disabled-opacity)";

    /// <summary>Size of an icon placed in an item's leading or trailing slot. A larger element there,
    /// such as an avatar or an image, keeps its own size.</summary>
    [CssVar(ListField.ItemIconSize)] public required string ItemIconSize { get; init; }
}
