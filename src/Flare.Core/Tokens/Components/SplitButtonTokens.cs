namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme geometry and seam tokens for <c>FlareSplitButton</c>.</summary>
public sealed record SplitButtonTokens
{
    // --- 1. ШОВ / ЗАЗОР МЕЖДУ КНОПКАМИ ---
    /// <summary>Gap token (<c>0.125rem</c>).</summary>
    public string Gap { get; init; } = "0.125rem"; // 2dp

    // Ширина кнопки-триггера. По умолчанию (MD3) = высоте (квадрат); Fluent задаёт фикс. 24dp.
    /// <summary>Trigger width token (<c>var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))</c>).</summary>
    public string TriggerWidth { get; init; } = "var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))";

    // --- 2. БОКОВЫЕ ОТСТУПЫ ТРИГГЕРА (Стрелочки выпадающего меню) ---
    /// <summary>Trigger padding xs token (<c>0.375rem</c>).</summary>
    public string TriggerPaddingXs { get; init; } = "0.375rem";
    /// <summary>Trigger padding sm token (<c>0.5rem</c>).</summary>
    public string TriggerPaddingSm { get; init; } = "0.5rem";
    /// <summary>Trigger padding md token (<c>1rem</c>).</summary>
    public string TriggerPaddingMd { get; init; } = "1rem";
    /// <summary>Trigger padding lg token (<c>1.25rem</c>).</summary>
    public string TriggerPaddingLg { get; init; } = "1.25rem";
    /// <summary>Trigger padding xl token (<c>1.5rem</c>).</summary>
    public string TriggerPaddingXl { get; init; } = "1.5rem";

    // --- 3. РАЗМЕРЫ ИКОНКИ СТРЕЛКИ (Caret Sizes) ---
    /// <summary>Caret size xs token (<c>0.875rem</c>).</summary>
    public string CaretSizeXs { get; init; } = "0.875rem";
    /// <summary>Caret size sm token (<c>1rem</c>).</summary>
    public string CaretSizeSm { get; init; } = "1rem";
    /// <summary>Caret size md token (<c>1.25rem</c>).</summary>
    public string CaretSizeMd { get; init; } = "1.25rem";
    /// <summary>Caret size lg token (<c>1.5rem</c>).</summary>
    public string CaretSizeLg { get; init; } = "1.5rem";
    /// <summary>Caret size xl token (<c>1.75rem</c>).</summary>
    public string CaretSizeXl { get; init; } = "1.75rem";

    // --- 4. ПОУГЛОВЫЕ РАДИУСЫ ДЛЯ ГЛАВНОЙ КНОПКИ (Main Button) ---
    /// <summary>Main radius xs token (<c>1rem</c>).</summary>
    public CornerRadiusTokens MainRadiusXs { get; init; } = new() { TopLeft = "1rem", BottomLeft = "1rem", TopRight = "8px", BottomRight = "8px" };
    /// <summary>Main radius sm token (<c>1.25rem</c>).</summary>
    public CornerRadiusTokens MainRadiusSm { get; init; } = new() { TopLeft = "1.25rem", BottomLeft = "1.25rem", TopRight = "8px", BottomRight = "8px" };
    /// <summary>Main radius md token (<c>1.5rem</c>).</summary>
    public CornerRadiusTokens MainRadiusMd { get; init; } = new() { TopLeft = "1.5rem", BottomLeft = "1.5rem", TopRight = "8px", BottomRight = "8px" };
    /// <summary>Main radius lg token (<c>1.75rem</c>).</summary>
    public CornerRadiusTokens MainRadiusLg { get; init; } = new() { TopLeft = "1.75rem", BottomLeft = "1.75rem", TopRight = "8px", BottomRight = "8px" };
    /// <summary>Main radius xl token (<c>2rem</c>).</summary>
    public CornerRadiusTokens MainRadiusXl { get; init; } = new() { TopLeft = "2rem", BottomLeft = "2rem", TopRight = "8px", BottomRight = "8px" };

    // --- 5. ПОУГЛОВЫЕ РАДИУСЫ ДЛЯ КНОПКИ-ТРИГГЕРА (Trigger Button) ---
    /// <summary>Trigger radius xs token (<c>8px</c>).</summary>
    public CornerRadiusTokens TriggerRadiusXs { get; init; } = new() { TopLeft = "8px", BottomLeft = "8px", TopRight = "1rem", BottomRight = "1rem" };
    /// <summary>Trigger radius sm token (<c>8px</c>).</summary>
    public CornerRadiusTokens TriggerRadiusSm { get; init; } = new() { TopLeft = "8px", BottomLeft = "8px", TopRight = "1.25rem", BottomRight = "1.25rem" };
    /// <summary>Trigger radius md token (<c>8px</c>).</summary>
    public CornerRadiusTokens TriggerRadiusMd { get; init; } = new() { TopLeft = "8px", BottomLeft = "8px", TopRight = "1.5rem", BottomRight = "1.5rem" };
    /// <summary>Trigger radius lg token (<c>8px</c>).</summary>
    public CornerRadiusTokens TriggerRadiusLg { get; init; } = new() { TopLeft = "8px", BottomLeft = "8px", TopRight = "1.75rem", BottomRight = "1.75rem" };
    /// <summary>Trigger radius xl token (<c>8px</c>).</summary>
    public CornerRadiusTokens TriggerRadiusXl { get; init; } = new() { TopLeft = "8px", BottomLeft = "8px", TopRight = "2rem", BottomRight = "2rem" };
}
