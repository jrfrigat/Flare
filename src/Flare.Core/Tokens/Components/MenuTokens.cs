namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareMenu</c> / <c>FlareMenuItem</c> (dropdown panel + items).</summary>
public sealed record MenuTokens
{
    // --- 1. ПАНЕЛЬ ---
    /// <summary>Panel min width token (<c>10rem</c>).</summary>
    public string PanelMinWidth { get; init; } = "10rem";
    /// <summary>Name of the panel entrance @keyframes animation (MD3 = scale+fade; Fluent = fade/slide without scale).</summary>
    public string EnterAnimation { get; init; } = "flare-menu-in";
    /// <summary>Panel radius token (<c>var(--flare-popover-radius, var(--flare-shape-small))</c>).</summary>
    public string PanelRadius { get; init; } = "var(--flare-popover-radius, var(--flare-shape-small))";
    /// <summary>Panel bg token (<c>var(--flare-color-surface-container)</c>).</summary>
    public string PanelBg { get; init; } = "var(--flare-color-surface-container)";
    /// <summary>Panel shadow token (<c>var(--flare-elevation-2)</c>).</summary>
    public string PanelShadow { get; init; } = "var(--flare-elevation-2)";
    /// <summary>Panel padding block token (<c>0.25rem</c>).</summary>
    public string PanelPaddingBlock { get; init; } = "0.25rem";
    /// <summary>Panel padding inline token (<c>0.25rem</c>).</summary>
    public string PanelPaddingInline { get; init; } = "0.25rem";

    // --- 2. ПУНКТ (геометрия) ---
    /// <summary>Item height token (<c>0</c>).</summary>
    public string ItemHeight { get; init; } = "0";           // min-height пункта (0 = авто)
    /// <summary>Item padding block token (<c>0.625rem</c>).</summary>
    public string ItemPaddingBlock { get; init; } = "0.625rem";
    /// <summary>Item padding inline token (<c>1rem</c>).</summary>
    public string ItemPaddingInline { get; init; } = "1rem";
    /// <summary>Item gap token (<c>0.75rem</c>).</summary>
    public string ItemGap { get; init; } = "0.75rem";        // зазор иконка<->текст
    /// <summary>Item gap between token (<c>0rem</c>).</summary>
    public string ItemGapBetween { get; init; } = "0rem";    // зазор МЕЖДУ пунктами
    /// <summary>Item icon size token (<c>1.25rem</c>).</summary>
    public string ItemIconSize { get; init; } = "1.25rem";
    /// <summary>Item radius token (<c>0</c>).</summary>
    public string ItemRadius { get; init; } = "0";           // скругление пункта
    /// <summary>Item radius end token (<c>0</c>).</summary>
    public string ItemRadiusEnd { get; init; } = "0";        // внешние углы первого/последнего

    // --- 2b. ГРУППА (секция) ---
    /// <summary>Group radius token (<c>0</c>).</summary>
    public string GroupRadius { get; init; } = "0";          // скругление группы
    /// <summary>Group padding token (<c>0</c>).</summary>
    public string GroupPadding { get; init; } = "0";         // внутренний отступ группы

    // --- 3. ПУНКТ (типографика метки) ---
    /// <summary>Item label font token (<c>var(--flare-typescale-body-large-font)</c>).</summary>
    public string ItemLabelFont { get; init; } = "var(--flare-typescale-body-large-font)";
    /// <summary>Item label weight token (<c>var(--flare-typescale-body-large-weight)</c>).</summary>
    public string ItemLabelWeight { get; init; } = "var(--flare-typescale-body-large-weight)";
    /// <summary>Item label size token (<c>var(--flare-typescale-body-large-size)</c>).</summary>
    public string ItemLabelSize { get; init; } = "var(--flare-typescale-body-large-size)";
    /// <summary>Item label height token (<c>var(--flare-typescale-body-large-height)</c>).</summary>
    public string ItemLabelHeight { get; init; } = "var(--flare-typescale-body-large-height)";
    /// <summary>Item label spacing token (<c>var(--flare-typescale-body-large-spacing)</c>).</summary>
    public string ItemLabelSpacing { get; init; } = "var(--flare-typescale-body-large-spacing)";

    // --- 4. ФОКУС-КОЛЬЦО ---
    /// <summary>Item focus ring color token (<c>var(--flare-color-secondary)</c>).</summary>
    public string ItemFocusRingColor { get; init; } = "var(--flare-color-secondary)";
    /// <summary>Item focus ring thickness token (<c>3px</c>).</summary>
    public string ItemFocusRingThickness { get; init; } = "3px";
    /// <summary>Item focus ring offset token (<c>-3px</c>).</summary>
    public string ItemFocusRingOffset { get; init; } = "-3px";
}
