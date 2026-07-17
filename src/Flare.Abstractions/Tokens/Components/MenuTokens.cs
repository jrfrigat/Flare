using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareMenu</c> / <c>FlareMenuItem</c> (dropdown panel + items).</summary>
public sealed record MenuTokens
{
    /// <summary>Rule drawn between groups of items, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(MenuPanel.GroupDivider)] public required string GroupDivider { get; init; }
    // --- 1. PANEL ---
    /// <summary>Floor on the panel width, so a menu of short labels does not collapse to a sliver.</summary>
    [CssVar(MenuPanel.MinWidth)] public required string PanelMinWidth { get; init; }
    /// <summary>Name of the panel entrance @keyframes animation (e.g. scale+fade, or fade/slide without scale).</summary>
    [CssVar(MenuPanel.EnterAnimation)] public required string EnterAnimation { get; init; }
    /// <summary>Corner radius of the dropdown panel.</summary>
    [CssVar(MenuPanel.Radius)] public required string PanelRadius { get; init; }
    /// <summary>Background of the dropdown panel.</summary>
    [CssVar(MenuPanel.Bg)] public required string PanelBg { get; init; }
    /// <summary>Shadow that lifts the panel off the content behind it.</summary>
    [CssVar(MenuPanel.Shadow)] public required string PanelShadow { get; init; }
    /// <summary>Space between the panel's top/bottom edge and its first/last item.</summary>
    [CssVar(MenuPanel.PaddingBlock)] public required string PanelPaddingBlock { get; init; }
    /// <summary>Space between the panel's side edges and its items.</summary>
    [CssVar(MenuPanel.PaddingInline)] public required string PanelPaddingInline { get; init; }

    // --- 2. ITEM (geometry) ---
    /// <summary>Floor on an item's height. A theme that wants the height to follow the padding and label
    /// alone parks this at <c>0</c>.</summary>
    [CssVar(MenuPanel.ItemHeight)] public required string ItemHeight { get; init; }
    /// <summary>Space above and below an item's content.</summary>
    [CssVar(MenuPanel.ItemPaddingBlock)] public required string ItemPaddingBlock { get; init; }
    /// <summary>Space between an item's side edges and its content.</summary>
    [CssVar(MenuPanel.ItemPaddingInline)] public required string ItemPaddingInline { get; init; }
    /// <summary>Space above and below an item's content when the menu is marked dense.</summary>
    [CssVar(MenuPanel.ItemPaddingBlockDense)] public required string ItemPaddingBlockDense { get; init; }
    /// <summary>Space between an item's icon and its label.</summary>
    [CssVar(MenuPanel.ItemGap)] public required string ItemGap { get; init; }
    /// <summary>Space between an item's icon and its label when the menu is marked dense.</summary>
    [CssVar(MenuPanel.ItemGapDense)] public required string ItemGapDense { get; init; }
    /// <summary>Space between adjacent items. A theme with a continuous list parks this at <c>0</c>;
    /// a theme with separated item pills raises it.</summary>
    [CssVar(MenuPanel.ItemGapBetween)] public required string ItemGapBetween { get; init; }
    /// <summary>Leading icon glyph size on an item.</summary>
    [CssVar(MenuPanel.ItemIconSize)] public required string ItemIconSize { get; init; }
    /// <summary>Corner radius of an item. A theme whose items are flush rows parks this at <c>0</c>.</summary>
    [CssVar(MenuPanel.ItemRadius)] public required string ItemRadius { get; init; }
    /// <summary>Corner radius of the outer corners of the first and last item, so the ends of the list can
    /// mirror the panel's own shape instead of squaring off inside it.</summary>
    [CssVar(MenuPanel.ItemRadiusEnd)] public required string ItemRadiusEnd { get; init; }

    // --- 2b. GROUP (section) ---
    /// <summary>Corner radius of a group container, when a theme renders groups as rounded islands.</summary>
    [CssVar(MenuPanel.GroupRadius)] public required string GroupRadius { get; init; }
    /// <summary>Space between a group's edges and its items, when a theme insets group islands.</summary>
    [CssVar(MenuPanel.GroupPadding)] public required string GroupPadding { get; init; }
    /// <summary>Background of a group "island". A theme with no island treatment parks this at
    /// <c>transparent</c>.</summary>
    [CssVar(MenuPanel.GroupBg)] public required string GroupBg { get; init; }
    /// <summary>Space between adjacent group "islands".</summary>
    [CssVar(MenuPanel.GroupGap)] public required string GroupGap { get; init; }
    /// <summary>Shadow on a group "island". A theme with flat groups parks this at <c>none</c>.</summary>
    [CssVar(MenuPanel.GroupShadow)] public required string GroupShadow { get; init; }
    /// <summary>Background of the slab behind a grouped panel, which a theme may want to differ from the
    /// plain panel background once the groups carry their own surfaces.</summary>
    [CssVar(MenuPanel.GroupedPanelBg)] public required string GroupedPanelBg { get; init; }
    /// <summary>Shadow on the slab behind a grouped panel.</summary>
    [CssVar(MenuPanel.GroupedPanelShadow)] public required string GroupedPanelShadow { get; init; }

    // --- 3. ITEM (label typography) ---
    /// <summary>Font family of an item's label. Each theme decides which step of its own type scale a
    /// menu item maps to.</summary>
    [CssVar(MenuPanel.ItemLabelFont)] public required string ItemLabelFont { get; init; }
    /// <summary>Font weight of an item's label.</summary>
    [CssVar(MenuPanel.ItemLabelWeight)] public required string ItemLabelWeight { get; init; }
    /// <summary>Font size of an item's label.</summary>
    [CssVar(MenuPanel.ItemLabelSize)] public required string ItemLabelSize { get; init; }
    /// <summary>Line height of an item's label.</summary>
    [CssVar(MenuPanel.ItemLabelHeight)] public required string ItemLabelHeight { get; init; }
    /// <summary>Letter spacing of an item's label.</summary>
    [CssVar(MenuPanel.ItemLabelSpacing)] public required string ItemLabelSpacing { get; init; }

    // --- 4. FOCUS RING ---
    /// <summary>Colour of the ring drawn around an item on keyboard focus.</summary>
    [CssVar(MenuPanel.ItemFocusRingColor)] public required string ItemFocusRingColor { get; init; }
    /// <summary>Thickness of an item's keyboard-focus ring.</summary>
    [CssVar(MenuPanel.ItemFocusRingThickness)] public required string ItemFocusRingThickness { get; init; }
    /// <summary>Offset of an item's keyboard-focus ring. A negative value pulls the ring inside the item,
    /// so it is not clipped by the panel edge.</summary>
    [CssVar(MenuPanel.ItemFocusRingOffset)] public required string ItemFocusRingOffset { get; init; }
}
