using Flare.Css;
using Flare.Css.Tokens;

namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme geometry, typography and state tokens for <c>FlareButton</c>. Scalar properties carry a
/// <see cref="CssVarAttribute"/> naming the <c>--flare-*</c> variable they populate (see
/// <c>Flare.Css.Tokens.Button</c>); the compound radius/label tokens expand to several variables and
/// are mapped in <c>CssVarMap.FlattenDesign</c>.
/// </summary>
public sealed record ButtonTokens
{
    // --- 1. СТРОГО ТИПИЗИРОВАННЫЕ ЗАЗОРЫ (Gaps между иконкой и текстом) ---
    /// <summary>Gap xs token (<c>0.25rem</c>).</summary>
    [CssVar(Button.Gap.Xs)] public string GapXs { get; init; } = "0.25rem";        // 4px для микро-кнопок
    /// <summary>Gap sm token (<c>0.375rem</c>).</summary>
    [CssVar(Button.Gap.Sm)] public string GapSm { get; init; } = "0.375rem";       // 6px
    /// <summary>Gap md token (<c>0.5rem</c>).</summary>
    [CssVar(Button.Gap.Md)] public string GapMd { get; init; } = "0.5rem";         // 8px (базовый зазор MD3)
    /// <summary>Gap lg token (<c>0.5rem</c>).</summary>
    [CssVar(Button.Gap.Lg)] public string GapLg { get; init; } = "0.5rem";         // 8px
    /// <summary>Gap xl token (<c>0.75rem</c>).</summary>
    [CssVar(Button.Gap.Xl)] public string GapXl { get; init; } = "0.75rem";        // 12px для гигантских кнопок

    // --- 2. ПОУГЛОВЫЕ РАДИУСЫ ПОД КАЖДЫЙ ИЗ 5 РАЗМЕРОВ ---
    // (compound: each expands to 4 per-corner --flare-btn-radius-* vars in CssVarMap.FlattenDesign)
    // All 5 sizes are fully-rounded pills (radius == half of the fixed container height for every
    // size), so they reference the Shape.Full scale token - like every other component that follows
    // the shape scale - instead of a hardcoded half-height rem literal. This preserves the pill visual
    // (border-radius: 9999px renders identically to an explicit half-height on these fixed heights)
    // while giving theme authors the standard Shape.Full override hook for button rounding.
    /// <summary>Radius xs token.</summary>
    public CornerRadiusTokens RadiusXs { get; init; } = CornerRadiusTokens.All(Vars.Var(Shape.Full));
    /// <summary>Radius sm token.</summary>
    public CornerRadiusTokens RadiusSm { get; init; } = CornerRadiusTokens.All(Vars.Var(Shape.Full));
    /// <summary>Radius md token.</summary>
    public CornerRadiusTokens RadiusMd { get; init; } = CornerRadiusTokens.All(Vars.Var(Shape.Full));
    /// <summary>Radius lg token.</summary>
    public CornerRadiusTokens RadiusLg { get; init; } = CornerRadiusTokens.All(Vars.Var(Shape.Full));
    /// <summary>Radius xl token.</summary>
    public CornerRadiusTokens RadiusXl { get; init; } = CornerRadiusTokens.All(Vars.Var(Shape.Full));

    // --- 3. ВЫСОТА КОНТЕЙНЕРОВ (Container Heights) ---
    /// <summary>Height xs token (<c>2rem</c>).</summary>
    [CssVar(Button.Height.Xs)] public string HeightXs { get; init; } = "2rem";     // 32px
    /// <summary>Height sm token (<c>2.5rem</c>).</summary>
    [CssVar(Button.Height.Sm)] public string HeightSm { get; init; } = "2.5rem";    // 40px
    /// <summary>Height md token (<c>3rem</c>).</summary>
    [CssVar(Button.Height.Md)] public string HeightMd { get; init; } = "3rem";     // 48px
    /// <summary>Height lg token (<c>3.5rem</c>).</summary>
    [CssVar(Button.Height.Lg)] public string HeightLg { get; init; } = "3.5rem";    // 56px
    /// <summary>Height xl token (<c>4rem</c>).</summary>
    [CssVar(Button.Height.Xl)] public string HeightXl { get; init; } = "4rem";     // 64px

    // --- 4. БОКОВЫЕ ОТСТУПЫ (Padding Inline) ---
    /// <summary>Padding inline xs token (<c>0.75rem</c>).</summary>
    [CssVar(Button.PaddingInline.Xs)] public string PaddingInlineXs { get; init; } = "0.75rem";
    /// <summary>Padding inline sm token (<c>1rem</c>).</summary>
    [CssVar(Button.PaddingInline.Sm)] public string PaddingInlineSm { get; init; } = "1rem";
    /// <summary>Padding inline md token (<c>1.5rem</c>).</summary>
    [CssVar(Button.PaddingInline.Md)] public string PaddingInlineMd { get; init; } = "1.5rem";
    /// <summary>Padding inline lg token (<c>2rem</c>).</summary>
    [CssVar(Button.PaddingInline.Lg)] public string PaddingInlineLg { get; init; } = "2rem";
    /// <summary>Padding inline xl token (<c>2.5rem</c>).</summary>
    [CssVar(Button.PaddingInline.Xl)] public string PaddingInlineXl { get; init; } = "2.5rem";

    // --- 5. ФОКУСЫ И ПОВЕДЕНИЕ ---
    /// <summary>Focus outline token (<c>2px solid transparent</c>).</summary>
    [CssVar(Button.FocusOutline)] public string FocusOutline { get; init; } = "2px solid transparent";
    /// <summary>Focus outline offset token (<c>0px</c>).</summary>
    [CssVar(Button.FocusOutlineOffset)] public string FocusOutlineOffset { get; init; } = "0px";
    /// <summary>Focus shadow token (<c>none</c>).</summary>
    [CssVar(Button.FocusShadow)] public string FocusShadow { get; init; } = "none";
    /// <summary>Filled hover shadow token (<c>none</c>).</summary>
    [CssVar(Button.FilledHoverShadow)] public string FilledHoverShadow { get; init; } = "none";

    // --- 6. РАЗМЕР ИКОНКИ (Icon size) под 5 размеров ---
    /// <summary>Icon size xs token (<c>1.25rem</c>).</summary>
    [CssVar(Button.IconSize.Xs)] public string IconSizeXs { get; init; } = "1.25rem";
    /// <summary>Icon size sm token (<c>1.25rem</c>).</summary>
    [CssVar(Button.IconSize.Sm)] public string IconSizeSm { get; init; } = "1.25rem";
    /// <summary>Icon size md token (<c>1.5rem</c>).</summary>
    [CssVar(Button.IconSize.Md)] public string IconSizeMd { get; init; } = "1.5rem";
    /// <summary>Icon size lg token (<c>2rem</c>).</summary>
    [CssVar(Button.IconSize.Lg)] public string IconSizeLg { get; init; } = "2rem";
    /// <summary>Icon size xl token (<c>2.5rem</c>).</summary>
    [CssVar(Button.IconSize.Xl)] public string IconSizeXl { get; init; } = "2.5rem";

    // --- 7. ТИПОГРАФИКА МЕТКИ (Label) под 5 размеров ---
    // Каждая тема задаёт свою шкалу (напр. MD3: label-large -> title-medium -> headline-*).
    // (compound: each TypeStyle expands to several --flare-btn-label-* vars in CssVarMap.FlattenDesign)
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
