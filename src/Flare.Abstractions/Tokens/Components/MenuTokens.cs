using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareMenu</c> / <c>FlareMenuItem</c> (dropdown panel + items).</summary>
public sealed record MenuTokens
{
    // --- 1. ПАНЕЛЬ ---
    /// <summary>Panel min width token (<c>10rem</c>).</summary>
    [CssVar(MenuPanel.MinWidth)] public string PanelMinWidth { get; init; } = "10rem";
    /// <summary>Name of the panel entrance @keyframes animation (MD3 = scale+fade; Fluent = fade/slide without scale).</summary>
    [CssVar(MenuPanel.EnterAnimation)] public string EnterAnimation { get; init; } = "flare-menu-in";
    /// <summary>Panel radius token (<c>var(--flare-popover-radius, var(--flare-shape-small))</c>).</summary>
    [CssVar(MenuPanel.Radius)] public string PanelRadius { get; init; } = "var(--flare-popover-radius, var(--flare-shape-small))";
    /// <summary>Panel bg token (<c>var(--flare-color-surface-container)</c>).</summary>
    [CssVar(MenuPanel.Bg)] public string PanelBg { get; init; } = Vars.Var(Color.SurfaceContainer);
    /// <summary>Panel shadow token (<c>var(--flare-elevation-2)</c>).</summary>
    [CssVar(MenuPanel.Shadow)] public string PanelShadow { get; init; } = Vars.Var(Elevation.Level2);
    /// <summary>Panel padding block token (<c>0.25rem</c>).</summary>
    [CssVar(MenuPanel.PaddingBlock)] public string PanelPaddingBlock { get; init; } = "0.25rem";
    /// <summary>Panel padding inline token (<c>0.25rem</c>).</summary>
    [CssVar(MenuPanel.PaddingInline)] public string PanelPaddingInline { get; init; } = "0.25rem";

    // --- 2. ПУНКТ (геометрия) ---
    /// <summary>Item height token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemHeight)] public string ItemHeight { get; init; } = "0";           // min-height пункта (0 = авто)
    /// <summary>Item padding block token (<c>0.625rem</c>).</summary>
    [CssVar(MenuPanel.ItemPaddingBlock)] public string ItemPaddingBlock { get; init; } = "0.625rem";
    /// <summary>Item padding inline token (<c>1rem</c>).</summary>
    [CssVar(MenuPanel.ItemPaddingInline)] public string ItemPaddingInline { get; init; } = "1rem";
    /// <summary>Item gap token (<c>0.75rem</c>).</summary>
    [CssVar(MenuPanel.ItemGap)] public string ItemGap { get; init; } = "0.75rem";        // зазор иконка<->текст
    /// <summary>Item gap between token (<c>0rem</c>).</summary>
    [CssVar(MenuPanel.ItemGapBetween)] public string ItemGapBetween { get; init; } = "0rem";    // зазор МЕЖДУ пунктами
    /// <summary>Item icon size token (<c>1.25rem</c>).</summary>
    [CssVar(MenuPanel.ItemIconSize)] public string ItemIconSize { get; init; } = "1.25rem";
    /// <summary>Item radius token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemRadius)] public string ItemRadius { get; init; } = "0";           // скругление пункта
    /// <summary>Item radius end token (<c>0</c>).</summary>
    [CssVar(MenuPanel.ItemRadiusEnd)] public string ItemRadiusEnd { get; init; } = "0";        // внешние углы первого/последнего

    // --- 2b. ГРУППА (секция) ---
    /// <summary>Group radius token (<c>0</c>).</summary>
    [CssVar(MenuPanel.GroupRadius)] public string GroupRadius { get; init; } = "0";          // скругление группы
    /// <summary>Group padding token (<c>0</c>).</summary>
    [CssVar(MenuPanel.GroupPadding)] public string GroupPadding { get; init; } = "0";         // внутренний отступ группы

    // --- 3. ПУНКТ (типографика метки) ---
    /// <summary>Item label font token (<c>var(--flare-typescale-body-large-font)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelFont)] public string ItemLabelFont { get; init; } = Vars.Var(Typography.Font("body-large"));
    /// <summary>Item label weight token (<c>var(--flare-typescale-body-large-weight)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelWeight)] public string ItemLabelWeight { get; init; } = Vars.Var(Typography.Weight("body-large"));
    /// <summary>Item label size token (<c>var(--flare-typescale-body-large-size)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelSize)] public string ItemLabelSize { get; init; } = Vars.Var(Typography.Size("body-large"));
    /// <summary>Item label height token (<c>var(--flare-typescale-body-large-height)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelHeight)] public string ItemLabelHeight { get; init; } = Vars.Var(Typography.Height("body-large"));
    /// <summary>Item label spacing token (<c>var(--flare-typescale-body-large-spacing)</c>).</summary>
    [CssVar(MenuPanel.ItemLabelSpacing)] public string ItemLabelSpacing { get; init; } = Vars.Var(Typography.Spacing("body-large"));

    // --- 4. ФОКУС-КОЛЬЦО ---
    /// <summary>Item focus ring color token (<c>var(--flare-color-secondary)</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingColor)] public string ItemFocusRingColor { get; init; } = Vars.Var(Color.Secondary);
    /// <summary>Item focus ring thickness token (<c>3px</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingThickness)] public string ItemFocusRingThickness { get; init; } = "3px";
    /// <summary>Item focus ring offset token (<c>-3px</c>).</summary>
    [CssVar(MenuPanel.ItemFocusRingOffset)] public string ItemFocusRingOffset { get; init; } = "-3px";
}
