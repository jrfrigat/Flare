namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme geometry, typography and state tokens for <c>FlareButton</c>.</summary>
public sealed record ButtonTokens
{
    // --- 1. СТРОГО ТИПИЗИРОВАННЫЕ ЗАЗОРЫ (Gaps между иконкой и текстом) ---
    /// <summary>Gap xs token (<c>0.25rem</c>).</summary>
    public string GapXs { get; init; } = "0.25rem";        // 4px для микро-кнопок
    /// <summary>Gap sm token (<c>0.375rem</c>).</summary>
    public string GapSm { get; init; } = "0.375rem";       // 6px
    /// <summary>Gap md token (<c>0.5rem</c>).</summary>
    public string GapMd { get; init; } = "0.5rem";         // 8px (базовый зазор MD3)
    /// <summary>Gap lg token (<c>0.5rem</c>).</summary>
    public string GapLg { get; init; } = "0.5rem";         // 8px
    /// <summary>Gap xl token (<c>0.75rem</c>).</summary>
    public string GapXl { get; init; } = "0.75rem";        // 12px для гигантских кнопок

    // --- 2. ПОУГЛОВЫЕ РАДИУСЫ ПОД КАЖДЫЙ ИЗ 5 РАЗМЕРОВ ---
    /// <summary>Radius xs token.</summary>
    public CornerRadiusTokens RadiusXs { get; init; } = CornerRadiusTokens.All("1rem");     // 32px / 2 = 16px
    /// <summary>Radius sm token.</summary>
    public CornerRadiusTokens RadiusSm { get; init; } = CornerRadiusTokens.All("1.25rem");  // 40px / 2 = 20px
    /// <summary>Radius md token.</summary>
    public CornerRadiusTokens RadiusMd { get; init; } = CornerRadiusTokens.All("1.5rem");   // 48px / 2 = 24px
    /// <summary>Radius lg token.</summary>
    public CornerRadiusTokens RadiusLg { get; init; } = CornerRadiusTokens.All("1.75rem");  // 56px / 2 = 28px
    /// <summary>Radius xl token.</summary>
    public CornerRadiusTokens RadiusXl { get; init; } = CornerRadiusTokens.All("2rem");     // 64px / 2 = 32px

    // --- 3. ВЫСОТА КОНТЕЙНЕРОВ (Container Heights) ---
    /// <summary>Height xs token (<c>2rem</c>).</summary>
    public string HeightXs { get; init; } = "2rem";     // 32px
    /// <summary>Height sm token (<c>2.5rem</c>).</summary>
    public string HeightSm { get; init; } = "2.5rem";    // 40px
    /// <summary>Height md token (<c>3rem</c>).</summary>
    public string HeightMd { get; init; } = "3rem";     // 48px
    /// <summary>Height lg token (<c>3.5rem</c>).</summary>
    public string HeightLg { get; init; } = "3.5rem";    // 56px
    /// <summary>Height xl token (<c>4rem</c>).</summary>
    public string HeightXl { get; init; } = "4rem";     // 64px

    // --- 4. БОКОВЫЕ ОТСТУПЫ (Padding Inline) ---
    /// <summary>Padding inline xs token (<c>0.75rem</c>).</summary>
    public string PaddingInlineXs { get; init; } = "0.75rem";
    /// <summary>Padding inline sm token (<c>1rem</c>).</summary>
    public string PaddingInlineSm { get; init; } = "1rem";
    /// <summary>Padding inline md token (<c>1.5rem</c>).</summary>
    public string PaddingInlineMd { get; init; } = "1.5rem";
    /// <summary>Padding inline lg token (<c>2rem</c>).</summary>
    public string PaddingInlineLg { get; init; } = "2rem";
    /// <summary>Padding inline xl token (<c>2.5rem</c>).</summary>
    public string PaddingInlineXl { get; init; } = "2.5rem";

    // --- 5. ФОКУСЫ И ПОВЕДЕНИЕ ---
    /// <summary>Focus outline token (<c>2px solid transparent</c>).</summary>
    public string FocusOutline { get; init; } = "2px solid transparent";
    /// <summary>Focus outline offset token (<c>0px</c>).</summary>
    public string FocusOutlineOffset { get; init; } = "0px";
    /// <summary>Focus shadow token (<c>none</c>).</summary>
    public string FocusShadow { get; init; } = "none";
    /// <summary>Filled hover shadow token (<c>none</c>).</summary>
    public string FilledHoverShadow { get; init; } = "none";

    // --- 6. РАЗМЕР ИКОНКИ (Icon size) под 5 размеров ---
    /// <summary>Icon size xs token (<c>1.25rem</c>).</summary>
    public string IconSizeXs { get; init; } = "1.25rem";
    /// <summary>Icon size sm token (<c>1.25rem</c>).</summary>
    public string IconSizeSm { get; init; } = "1.25rem";
    /// <summary>Icon size md token (<c>1.5rem</c>).</summary>
    public string IconSizeMd { get; init; } = "1.5rem";
    /// <summary>Icon size lg token (<c>2rem</c>).</summary>
    public string IconSizeLg { get; init; } = "2rem";
    /// <summary>Icon size xl token (<c>2.5rem</c>).</summary>
    public string IconSizeXl { get; init; } = "2.5rem";

    // --- 7. ТИПОГРАФИКА МЕТКИ (Label) под 5 размеров ---
    // Каждая тема задаёт свою шкалу (напр. MD3: label-large -> title-medium -> headline-*).
    private static readonly TypeStyle _defaultLabel = new()
    {
        FontFamily = "Roboto",
        FontWeight = "500",
        FontSize = "0.875rem",
        LineHeight = "1.25rem",
        LetterSpacing = "0.00625em",
    };
    /// <summary>Label xs token.</summary>
    public TypeStyle LabelXs { get; init; } = _defaultLabel;
    /// <summary>Label sm token.</summary>
    public TypeStyle LabelSm { get; init; } = _defaultLabel;
    /// <summary>Label md token.</summary>
    public TypeStyle LabelMd { get; init; } = _defaultLabel;
    /// <summary>Label lg token.</summary>
    public TypeStyle LabelLg { get; init; } = _defaultLabel;
    /// <summary>Label xl token.</summary>
    public TypeStyle LabelXl { get; init; } = _defaultLabel;
}
