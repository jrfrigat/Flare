using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareMenu</c> / <c>FlareMenuItem</c> (dropdown panel + items).</summary>
public sealed record MenuTokens
{
    // --- 1. PANEL ---
    /// <summary>Panel min width token (<c>10rem</c>).</summary>
    [CssVar(MenuPanel.MinWidth)] public required string PanelMinWidth { get; init; }
    /// <summary>Name of the panel entrance @keyframes animation (e.g. scale+fade, or fade/slide without scale).</summary>
    [CssVar(MenuPanel.EnterAnimation)] public required string EnterAnimation { get; init; }
    /// <summary>Panel radius token (<c>var(--flare-popover-radius, var(--flare-shape-small))</c>).</summary>
    [CssVar(MenuPanel.Radius)] public required string PanelRadius { get; init; }
    /// <summary>Panel bg token (<c>var(--flare-color-surface-container)</c>).</summary>
    [CssVar(MenuPanel.Bg)] public required string PanelBg { get; init; }
    /// <summary>Panel shadow token (<c>var(--flare-elevation-2)</c>).</summary>
    [CssVar(MenuPanel.Shadow)] public required string PanelShadow { get; init; }
    /// <summary>Panel padding block token (<c>0.25rem</c>).</summary>
    [CssVar(MenuPanel.PaddingBlock)] public required string PanelPaddingBlock { get; init; }
    /// <summary>Panel padding inline token (<c>0.25rem</c>).</summary>
    [CssVar(MenuPanel.PaddingInline)] public required string PanelPaddingInline { get; init; }

    // --- 2. ITEM (geometry) ---
    /// <summary>Item height token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemHeight)] public required string ItemHeight { get; init; }
    /// <summary>Item padding block token (<c>0.625rem</c>).</summary>
    [CssVar(MenuPanel.ItemPaddingBlock)] public required string ItemPaddingBlock { get; init; }
    /// <summary>Item padding inline token (<c>1rem</c>).</summary>
    [CssVar(MenuPanel.ItemPaddingInline)] public required string ItemPaddingInline { get; init; }
    /// <summary>Item gap token (<c>0.75rem</c>).</summary>
    [CssVar(MenuPanel.ItemGap)] public required string ItemGap { get; init; }
    /// <summary>Item gap between token (<c>0rem</c>).</summary>
    [CssVar(MenuPanel.ItemGapBetween)] public required string ItemGapBetween { get; init; }
    /// <summary>Item icon size token (<c>1.25rem</c>).</summary>
    [CssVar(MenuPanel.ItemIconSize)] public required string ItemIconSize { get; init; }
    /// <summary>Item radius token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemRadius)] public required string ItemRadius { get; init; }
    /// <summary>Item radius end token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemRadiusEnd)] public required string ItemRadiusEnd { get; init; }

    // --- 2b. GROUP (section) ---
    /// <summary>Group radius token (<c>0</c>).</summary>
    [CssVar(MenuPanel.GroupRadius)] public required string GroupRadius { get; init; }
    /// <summary>Group padding token (<c>0</c>).</summary>
    [CssVar(MenuPanel.GroupPadding)] public required string GroupPadding { get; init; }
    /// <summary>Group "island" background. Default transparent (no island treatment).</summary>
    [CssVar(MenuPanel.GroupBg)] public required string GroupBg { get; init; }
    /// <summary>Gap between group "islands". Default 0.5rem.</summary>
    [CssVar(MenuPanel.GroupGap)] public required string GroupGap { get; init; }
    /// <summary>Group "island" shadow. Default none.</summary>
    [CssVar(MenuPanel.GroupShadow)] public required string GroupShadow { get; init; }
    /// <summary>Grouped-panel backing-slab background. Default = the panel background.</summary>
    [CssVar(MenuPanel.GroupedPanelBg)] public required string GroupedPanelBg { get; init; }
    /// <summary>Grouped-panel backing-slab shadow. Default = the panel shadow.</summary>
    [CssVar(MenuPanel.GroupedPanelShadow)] public required string GroupedPanelShadow { get; init; }

    // --- 3. ITEM (label typography) ---
    /// <summary>Item label font token (<c>var(--flare-typescale-body-large-font)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelFont)] public required string ItemLabelFont { get; init; }
    /// <summary>Item label weight token (<c>var(--flare-typescale-body-large-weight)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelWeight)] public required string ItemLabelWeight { get; init; }
    /// <summary>Item label size token (<c>var(--flare-typescale-body-large-size)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelSize)] public required string ItemLabelSize { get; init; }
    /// <summary>Item label height token (<c>var(--flare-typescale-body-large-height)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelHeight)] public required string ItemLabelHeight { get; init; }
    /// <summary>Item label spacing token (<c>var(--flare-typescale-body-large-spacing)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelSpacing)] public required string ItemLabelSpacing { get; init; }

    // --- 4. FOCUS RING ---
    /// <summary>Item focus ring color token (<c>var(--flare-color-secondary)</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingColor)] public required string ItemFocusRingColor { get; init; }
    /// <summary>Item focus ring thickness token (<c>3px</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingThickness)] public required string ItemFocusRingThickness { get; init; }
    /// <summary>Item focus ring offset token (<c>-3px</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingOffset)] public required string ItemFocusRingOffset { get; init; }
}
