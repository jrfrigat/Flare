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
    [CssVar(Button.Gap.Xs)] public required string GapXs { get; init; }
    /// <summary>Gap sm token (<c>0.375rem</c>).</summary>
    [CssVar(Button.Gap.Sm)] public required string GapSm { get; init; }
    /// <summary>Gap md token (<c>0.5rem</c>).</summary>
    [CssVar(Button.Gap.Md)] public required string GapMd { get; init; }
    /// <summary>Gap lg token (<c>0.5rem</c>).</summary>
    [CssVar(Button.Gap.Lg)] public required string GapLg { get; init; }
    /// <summary>Gap xl token (<c>0.75rem</c>).</summary>
    [CssVar(Button.Gap.Xl)] public required string GapXl { get; init; }

    // --- 2. ПОУГЛОВЫЕ РАДИУСЫ ПОД КАЖДЫЙ ИЗ 5 РАЗМЕРОВ ---
    // (compound: each expands to 4 per-corner --flare-btn-radius-* vars in CssVarMap.FlattenDesign)
    // All 5 sizes are fully-rounded pills (radius == half of the fixed container height for every
    // size), so they reference the Shape.Full scale token - like every other component that follows
    // the shape scale - instead of a hardcoded half-height rem literal. This preserves the pill visual
    // (border-radius: 9999px renders identically to an explicit half-height on these fixed heights)
    // while giving theme authors the standard Shape.Full override hook for button rounding.
    /// <summary>Radius xs token.</summary>
    public required CornerRadiusTokens RadiusXs { get; init; }
    /// <summary>Radius sm token.</summary>
    public required CornerRadiusTokens RadiusSm { get; init; }
    /// <summary>Radius md token.</summary>
    public required CornerRadiusTokens RadiusMd { get; init; }
    /// <summary>Radius lg token.</summary>
    public required CornerRadiusTokens RadiusLg { get; init; }
    /// <summary>Radius xl token.</summary>
    public required CornerRadiusTokens RadiusXl { get; init; }

    // --- 3. ВЫСОТА КОНТЕЙНЕРОВ (Container Heights) ---
    /// <summary>Height xs token (<c>2rem</c>).</summary>
    [CssVar(Button.Height.Xs)] public required string HeightXs { get; init; }
    /// <summary>Height sm token (<c>2.5rem</c>).</summary>
    [CssVar(Button.Height.Sm)] public required string HeightSm { get; init; }
    /// <summary>Height md token (<c>3rem</c>).</summary>
    [CssVar(Button.Height.Md)] public required string HeightMd { get; init; }
    /// <summary>Height lg token (<c>3.5rem</c>).</summary>
    [CssVar(Button.Height.Lg)] public required string HeightLg { get; init; }
    /// <summary>Height xl token (<c>4rem</c>).</summary>
    [CssVar(Button.Height.Xl)] public required string HeightXl { get; init; }

    // --- 4. БОКОВЫЕ ОТСТУПЫ (Padding Inline) ---
    /// <summary>Padding inline xs token (<c>0.75rem</c>).</summary>
    [CssVar(Button.PaddingInline.Xs)] public required string PaddingInlineXs { get; init; }
    /// <summary>Padding inline sm token (<c>1rem</c>).</summary>
    [CssVar(Button.PaddingInline.Sm)] public required string PaddingInlineSm { get; init; }
    /// <summary>Padding inline md token (<c>1.5rem</c>).</summary>
    [CssVar(Button.PaddingInline.Md)] public required string PaddingInlineMd { get; init; }
    /// <summary>Padding inline lg token (<c>2rem</c>).</summary>
    [CssVar(Button.PaddingInline.Lg)] public required string PaddingInlineLg { get; init; }
    /// <summary>Padding inline xl token (<c>2.5rem</c>).</summary>
    [CssVar(Button.PaddingInline.Xl)] public required string PaddingInlineXl { get; init; }

    // --- 5. ФОКУСЫ И ПОВЕДЕНИЕ ---
    /// <summary>Focus outline token (<c>2px solid transparent</c>).</summary>
    [CssVar(Button.FocusOutline)] public required string FocusOutline { get; init; }
    /// <summary>Focus outline offset token (<c>0px</c>).</summary>
    [CssVar(Button.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }
    /// <summary>Focus shadow token (<c>none</c>).</summary>
    [CssVar(Button.FocusShadow)] public required string FocusShadow { get; init; }
    /// <summary>Filled hover shadow token (<c>none</c>).</summary>
    [CssVar(Button.FilledHoverShadow)] public required string FilledHoverShadow { get; init; }

    // --- 6. РАЗМЕР ИКОНКИ (Icon size) под 5 размеров ---
    /// <summary>Icon size xs token (<c>1.25rem</c>).</summary>
    [CssVar(Button.IconSize.Xs)] public required string IconSizeXs { get; init; }
    /// <summary>Icon size sm token (<c>1.25rem</c>).</summary>
    [CssVar(Button.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Icon size md token (<c>1.5rem</c>).</summary>
    [CssVar(Button.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Icon size lg token (<c>2rem</c>).</summary>
    [CssVar(Button.IconSize.Lg)] public required string IconSizeLg { get; init; }
    /// <summary>Icon size xl token (<c>2.5rem</c>).</summary>
    [CssVar(Button.IconSize.Xl)] public required string IconSizeXl { get; init; }

    // --- 7. ТИПОГРАФИКА МЕТКИ (Label) под 5 размеров ---
    // Каждая тема задаёт свою шкалу (напр. MD3: label-large -> title-medium -> headline-*).
    // (compound: each TypeStyle expands to several --flare-btn-label-* vars in CssVarMap.FlattenDesign)
    /// <summary>Label xs token.</summary>
    public required TypeStyle LabelXs { get; init; }
    /// <summary>Label sm token.</summary>
    public required TypeStyle LabelSm { get; init; }
    /// <summary>Label md token.</summary>
    public required TypeStyle LabelMd { get; init; }
    /// <summary>Label lg token.</summary>
    public required TypeStyle LabelLg { get; init; }
    /// <summary>Label xl token.</summary>
    public required TypeStyle LabelXl { get; init; }
}
