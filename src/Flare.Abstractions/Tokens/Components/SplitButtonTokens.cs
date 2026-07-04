using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme geometry and seam tokens for <c>FlareSplitButton</c>.</summary>
public sealed record SplitButtonTokens
{
    // --- 1. ШОВ / ЗАЗОР МЕЖДУ КНОПКАМИ ---
    /// <summary>Gap token (<c>0.125rem</c>).</summary>
    [CssVar(SplitButton.Gap)] public required string Gap { get; init; }

    // Ширина кнопки-триггера. По умолчанию (MD3) = высоте (квадрат); Fluent задаёт фикс. 24dp.
    /// <summary>Trigger width token (<c>var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))</c>).</summary>
    [CssVar(SplitButton.TriggerWidth)] public required string TriggerWidth { get; init; }

    // --- 2. БОКОВЫЕ ОТСТУПЫ ТРИГГЕРА (Стрелочки выпадающего меню) ---
    /// <summary>Trigger padding xs token (<c>0.375rem</c>).</summary>
    [CssVar(SplitButton.TriggerPaddingInline.Xs)] public required string TriggerPaddingXs { get; init; }
    /// <summary>Trigger padding sm token (<c>0.5rem</c>).</summary>
    [CssVar(SplitButton.TriggerPaddingInline.Sm)] public required string TriggerPaddingSm { get; init; }
    /// <summary>Trigger padding md token (<c>1rem</c>).</summary>
    [CssVar(SplitButton.TriggerPaddingInline.Md)] public required string TriggerPaddingMd { get; init; }
    /// <summary>Trigger padding lg token (<c>1.25rem</c>).</summary>
    [CssVar(SplitButton.TriggerPaddingInline.Lg)] public required string TriggerPaddingLg { get; init; }
    /// <summary>Trigger padding xl token (<c>1.5rem</c>).</summary>
    [CssVar(SplitButton.TriggerPaddingInline.Xl)] public required string TriggerPaddingXl { get; init; }

    // --- 3. РАЗМЕРЫ ИКОНКИ СТРЕЛКИ (Caret Sizes) ---
    /// <summary>Caret size xs token (<c>0.875rem</c>).</summary>
    [CssVar(SplitButton.CaretSize.Xs)] public required string CaretSizeXs { get; init; }
    /// <summary>Caret size sm token (<c>1rem</c>).</summary>
    [CssVar(SplitButton.CaretSize.Sm)] public required string CaretSizeSm { get; init; }
    /// <summary>Caret size md token (<c>1.25rem</c>).</summary>
    [CssVar(SplitButton.CaretSize.Md)] public required string CaretSizeMd { get; init; }
    /// <summary>Caret size lg token (<c>1.5rem</c>).</summary>
    [CssVar(SplitButton.CaretSize.Lg)] public required string CaretSizeLg { get; init; }
    /// <summary>Caret size xl token (<c>1.75rem</c>).</summary>
    [CssVar(SplitButton.CaretSize.Xl)] public required string CaretSizeXl { get; init; }

    // --- 4. ПОУГЛОВЫЕ РАДИУСЫ ДЛЯ ГЛАВНОЙ КНОПКИ (Main Button) ---
    /// <summary>Main radius xs token (<c>1rem</c>).</summary>
    public required CornerRadiusTokens MainRadiusXs { get; init; }
    /// <summary>Main radius sm token (<c>1.25rem</c>).</summary>
    public required CornerRadiusTokens MainRadiusSm { get; init; }
    /// <summary>Main radius md token (<c>1.5rem</c>).</summary>
    public required CornerRadiusTokens MainRadiusMd { get; init; }
    /// <summary>Main radius lg token (<c>1.75rem</c>).</summary>
    public required CornerRadiusTokens MainRadiusLg { get; init; }
    /// <summary>Main radius xl token (<c>2rem</c>).</summary>
    public required CornerRadiusTokens MainRadiusXl { get; init; }

    // --- 5. ПОУГЛОВЫЕ РАДИУСЫ ДЛЯ КНОПКИ-ТРИГГЕРА (Trigger Button) ---
    /// <summary>Trigger radius xs token (<c>8px</c>).</summary>
    public required CornerRadiusTokens TriggerRadiusXs { get; init; }
    /// <summary>Trigger radius sm token (<c>8px</c>).</summary>
    public required CornerRadiusTokens TriggerRadiusSm { get; init; }
    /// <summary>Trigger radius md token (<c>8px</c>).</summary>
    public required CornerRadiusTokens TriggerRadiusMd { get; init; }
    /// <summary>Trigger radius lg token (<c>8px</c>).</summary>
    public required CornerRadiusTokens TriggerRadiusLg { get; init; }
    /// <summary>Trigger radius xl token (<c>8px</c>).</summary>
    public required CornerRadiusTokens TriggerRadiusXl { get; init; }
}
