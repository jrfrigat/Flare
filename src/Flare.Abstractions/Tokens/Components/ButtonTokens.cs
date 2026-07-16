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
    /// <summary>Opacity of the whole button while it is loading, dimming it behind the spinner.</summary>
    [CssVar(Button.LoadingOpacity)] public required string LoadingOpacity { get; init; }
    /// <summary>Fallback container corner radius for buttons that do not set a per-size radius.</summary>
    [CssVar(Button.ContainerRadius)] public required string ContainerRadius { get; init; }
    /// <summary>Side padding for the Text variant, replacing the per-size inline padding at every size.
    /// A text button has no container to sit inside, so it usually hugs its label more tightly.</summary>
    [CssVar(Button.TextPaddingInline)] public required string TextPaddingInline { get; init; }
    // --- 1. STRONGLY-TYPED GAPS (gaps between icon and text) ---
    /// <summary>Space between the icon and the label at the xs size.</summary>
    [CssVar(Button.Gap.Xs)] public required string GapXs { get; init; }
    /// <summary>Space between the icon and the label at the sm size.</summary>
    [CssVar(Button.Gap.Sm)] public required string GapSm { get; init; }
    /// <summary>Space between the icon and the label at the md size.</summary>
    [CssVar(Button.Gap.Md)] public required string GapMd { get; init; }
    /// <summary>Space between the icon and the label at the lg size.</summary>
    [CssVar(Button.Gap.Lg)] public required string GapLg { get; init; }
    /// <summary>Space between the icon and the label at the xl size.</summary>
    [CssVar(Button.Gap.Xl)] public required string GapXl { get; init; }

    // --- 2. PER-CORNER RADII FOR EACH OF THE 5 SIZES ---
    // (compound: each expands to 4 per-corner --flare-btn-radius-* vars in CssVarMap.FlattenDesign)
    // A theme is free to give every size the same radius (e.g. by referencing the Shape.Full scale token
    // for a pill at every size) or to ramp the rounding per size; the core takes no position either way.
    /// <summary>Container corner radii at the xs size, one value per corner.</summary>
    public required CornerRadiusTokens RadiusXs { get; init; }
    /// <summary>Container corner radii at the sm size, one value per corner.</summary>
    public required CornerRadiusTokens RadiusSm { get; init; }
    /// <summary>Container corner radii at the md size, one value per corner.</summary>
    public required CornerRadiusTokens RadiusMd { get; init; }
    /// <summary>Container corner radii at the lg size, one value per corner.</summary>
    public required CornerRadiusTokens RadiusLg { get; init; }
    /// <summary>Container corner radii at the xl size, one value per corner.</summary>
    public required CornerRadiusTokens RadiusXl { get; init; }

    // --- 3. CONTAINER HEIGHTS ---
    /// <summary>Container height at the xs size. The button is a fixed-height control, so this defines
    /// the size step rather than merely constraining it.</summary>
    [CssVar(Button.Height.Xs)] public required string HeightXs { get; init; }
    /// <summary>Container height at the sm size.</summary>
    [CssVar(Button.Height.Sm)] public required string HeightSm { get; init; }
    /// <summary>Container height at the md size.</summary>
    [CssVar(Button.Height.Md)] public required string HeightMd { get; init; }
    /// <summary>Container height at the lg size.</summary>
    [CssVar(Button.Height.Lg)] public required string HeightLg { get; init; }
    /// <summary>Container height at the xl size.</summary>
    [CssVar(Button.Height.Xl)] public required string HeightXl { get; init; }

    // --- 4. INLINE PADDING (side padding) ---
    /// <summary>Space between the container edge and the content at the xs size, on both sides.</summary>
    [CssVar(Button.PaddingInline.Xs)] public required string PaddingInlineXs { get; init; }
    /// <summary>Space between the container edge and the content at the sm size, on both sides.</summary>
    [CssVar(Button.PaddingInline.Sm)] public required string PaddingInlineSm { get; init; }
    /// <summary>Space between the container edge and the content at the md size, on both sides.</summary>
    [CssVar(Button.PaddingInline.Md)] public required string PaddingInlineMd { get; init; }
    /// <summary>Space between the container edge and the content at the lg size, on both sides.</summary>
    [CssVar(Button.PaddingInline.Lg)] public required string PaddingInlineLg { get; init; }
    /// <summary>Space between the container edge and the content at the xl size, on both sides.</summary>
    [CssVar(Button.PaddingInline.Xl)] public required string PaddingInlineXl { get; init; }

    // --- 5. FOCUS AND BEHAVIOR ---
    /// <summary>Shorthand <c>outline</c> drawn around the button on keyboard focus
    /// (<c>:focus-visible</c>), not on a pointer press.</summary>
    [CssVar(Button.FocusOutline)] public required string FocusOutline { get; init; }
    /// <summary>Distance the focus outline sits away from the container edge.</summary>
    [CssVar(Button.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }
    /// <summary>Shadow drawn on keyboard focus, in addition to the focus outline. A theme that signals
    /// focus with the outline alone parks this at <c>none</c>.</summary>
    [CssVar(Button.FocusShadow)] public required string FocusShadow { get; init; }
    /// <summary>Shadow the Filled variant lifts to on hover. A theme with flat filled buttons parks this
    /// at <c>none</c>.</summary>
    [CssVar(Button.FilledHoverShadow)] public required string FilledHoverShadow { get; init; }

    // --- 6. ICON SIZE for the 5 sizes ---
    /// <summary>Leading/trailing icon glyph size at the xs size.</summary>
    [CssVar(Button.IconSize.Xs)] public required string IconSizeXs { get; init; }
    /// <summary>Leading/trailing icon glyph size at the sm size.</summary>
    [CssVar(Button.IconSize.Sm)] public required string IconSizeSm { get; init; }
    /// <summary>Leading/trailing icon glyph size at the md size.</summary>
    [CssVar(Button.IconSize.Md)] public required string IconSizeMd { get; init; }
    /// <summary>Leading/trailing icon glyph size at the lg size.</summary>
    [CssVar(Button.IconSize.Lg)] public required string IconSizeLg { get; init; }
    /// <summary>Leading/trailing icon glyph size at the xl size.</summary>
    [CssVar(Button.IconSize.Xl)] public required string IconSizeXl { get; init; }

    // --- 7. LABEL TYPOGRAPHY for the 5 sizes ---
    // (compound: each TypeStyle expands to several --flare-btn-label-* vars in CssVarMap.FlattenDesign)
    /// <summary>Label typography at the xs size. Each theme decides which step of its own type scale a
    /// button size maps to.</summary>
    public required TypeStyle LabelXs { get; init; }
    /// <summary>Label typography at the sm size.</summary>
    public required TypeStyle LabelSm { get; init; }
    /// <summary>Label typography at the md size.</summary>
    public required TypeStyle LabelMd { get; init; }
    /// <summary>Label typography at the lg size.</summary>
    public required TypeStyle LabelLg { get; init; }
    /// <summary>Label typography at the xl size.</summary>
    public required TypeStyle LabelXl { get; init; }
}
