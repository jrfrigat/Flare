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

    // --- 4a. OUTLINE WIDTH ---
    // Per size rather than one value, because a stroke that reads as a hairline beside a small label is
    // a thread beside a large one - Material thickens it as the button grows. It is reserved on every
    // variant, not just the outlined one, so that changing variant never moves anything.
    /// <summary>Container border width at the xs size.</summary>
    [CssVar(Button.OutlineWidth.Xs)] public required string OutlineWidthXs { get; init; }
    /// <summary>Container border width at the sm size.</summary>
    [CssVar(Button.OutlineWidth.Sm)] public required string OutlineWidthSm { get; init; }
    /// <summary>Container border width at the md size.</summary>
    [CssVar(Button.OutlineWidth.Md)] public required string OutlineWidthMd { get; init; }
    /// <summary>Container border width at the lg size.</summary>
    [CssVar(Button.OutlineWidth.Lg)] public required string OutlineWidthLg { get; init; }
    /// <summary>Container border width at the xl size.</summary>
    [CssVar(Button.OutlineWidth.Xl)] public required string OutlineWidthXl { get; init; }

    // --- 4b. SELECTED (a button whose toggle is on) ---
    // Selection is a shape change as much as a colour one, and Material states the shape half as a swap:
    // a round button becomes square when selected and a square one becomes round. The value below is the
    // first half of that swap - what a button with the theme's own (round) shape takes when selected -
    // while the explicitly square shape travels to a capsule, which is geometry the core computes from
    // the height rather than a value a theme could usefully name. Per size because the two shapes are a
    // per-size pair in the spec, not one radius applied five times.
    /// <summary>Corner radius of a selected button at the xs size.</summary>
    [CssVar(Button.SelectedRadius.Xs)] public required string SelectedRadiusXs { get; init; }
    /// <summary>Corner radius of a selected button at the sm size.</summary>
    [CssVar(Button.SelectedRadius.Sm)] public required string SelectedRadiusSm { get; init; }
    /// <summary>Corner radius of a selected button at the md size.</summary>
    [CssVar(Button.SelectedRadius.Md)] public required string SelectedRadiusMd { get; init; }
    /// <summary>Corner radius of a selected button at the lg size.</summary>
    [CssVar(Button.SelectedRadius.Lg)] public required string SelectedRadiusLg { get; init; }
    /// <summary>Corner radius of a selected button at the xl size.</summary>
    [CssVar(Button.SelectedRadius.Xl)] public required string SelectedRadiusXl { get; init; }
    /// <summary>Corner radius a selected button takes when its rest shape is the explicit square - the
    /// other half of the swap, and the direction that travels outward. A language that does not reshape
    /// on selection points this back at its own square radius, which is the difference between the two
    /// kinds of theme rather than something the core is entitled to decide.</summary>
    [CssVar(Button.SelectedRadiusSquare)] public required string SelectedRadiusSquare { get; init; }
    /// <summary>Container background of a selected button, shared by every variant. A theme that wants a
    /// variant to answer selection differently says so in its own stylesheet; this is the one answer the
    /// core paints from.</summary>
    [CssVar(Button.SelectedBg)] public required string SelectedBg { get; init; }
    /// <summary>Icon and label colour of a selected button, which must stay legible on
    /// <see cref="SelectedBg"/>.</summary>
    [CssVar(Button.SelectedColor)] public required string SelectedColor { get; init; }

    // Per-variant toggle paint. Material keeps a separate colour table for toggle buttons - "the default
    // and toggle buttons use different colors" - and every variant lands somewhere its default never
    // goes when selected. Filled is the one that also differs while UNselected, which is why it is the
    // only variant with an unselected pair: a filled toggle at rest is not a filled button.
    /// <summary>Container of a selected Elevated button.</summary>
    [CssVar(Button.Toggle.ElevatedSelectedBg)] public required string ElevatedSelectedBg { get; init; }
    /// <summary>Icon and label of a selected Elevated button.</summary>
    [CssVar(Button.Toggle.ElevatedSelectedColor)] public required string ElevatedSelectedColor { get; init; }
    /// <summary>Container of a selected Filled button.</summary>
    [CssVar(Button.Toggle.FilledSelectedBg)] public required string FilledSelectedBg { get; init; }
    /// <summary>Icon and label of a selected Filled button.</summary>
    [CssVar(Button.Toggle.FilledSelectedColor)] public required string FilledSelectedColor { get; init; }
    /// <summary>Container of a selected Tonal button.</summary>
    [CssVar(Button.Toggle.TonalSelectedBg)] public required string TonalSelectedBg { get; init; }
    /// <summary>Icon and label of a selected Tonal button.</summary>
    [CssVar(Button.Toggle.TonalSelectedColor)] public required string TonalSelectedColor { get; init; }
    /// <summary>Container of a selected Outlined button.</summary>
    [CssVar(Button.Toggle.OutlinedSelectedBg)] public required string OutlinedSelectedBg { get; init; }
    /// <summary>Icon and label of a selected Outlined button.</summary>
    [CssVar(Button.Toggle.OutlinedSelectedColor)] public required string OutlinedSelectedColor { get; init; }
    /// <summary>Container of a Filled button that is a toggle and currently unselected - the one state
    /// where being a toggle changes a button before anything has been selected. A theme that draws no
    /// such distinction points this at its own filled container.</summary>
    [CssVar(Button.Toggle.FilledUnselectedBg)] public required string FilledUnselectedBg { get; init; }
    /// <summary>Icon and label of an unselected Filled toggle.</summary>
    [CssVar(Button.Toggle.FilledUnselectedColor)] public required string FilledUnselectedColor { get; init; }

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

    /// <summary>How far a disabled button fades. A language that signals disabled by dimming the whole
    /// control sets a fraction here; one that repaints it in a flat palette leaves it fully opaque and
    /// carries the change in <see cref="DisabledLayer"/> instead. Per component rather than shared,
    /// because a theme may well dim its other controls while repainting this one.</summary>
    [CssVar(Button.DisabledOpacity)] public required string DisabledOpacity { get; init; }
    /// <summary>Paint laid over a disabled button's container. A language that dims parks this at a
    /// transparent value - the only genuinely neutral answer, since no CSS value means "leave the
    /// colour alone" - while one that repaints puts its flat disabled fill here.</summary>
    [CssVar(Button.DisabledLayer)] public required string DisabledLayer { get; init; }

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
