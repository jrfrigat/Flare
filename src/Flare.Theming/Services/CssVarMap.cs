using Flare.Abstractions.Tokens;

namespace Flare.Theming;

/// <summary>Flattens theme token records into the CSS-variable name/value map injected at runtime.</summary>
public static class CssVarMap
{
    /// <summary>
    /// Merges the two theme axes into the full CSS-variable map consumed by the var-injection
    /// delivery path: mode-agnostic <see cref="DesignTokens"/> plus a mode-specific
    /// <see cref="ColorScheme"/>. Colors win on key collisions (there are none today).
    /// </summary>
    public static Dictionary<string, string> Flatten(this DesignTokens design, ColorScheme colors)
    {
        var v = design.FlattenDesign();
        foreach (var (k, val) in colors.FlattenColors())
            v[k] = val;
        return v;
    }

    /// <summary>The mode-agnostic design CSS variables (theme half): focus ring, typography,
    /// shape, spacing, elevation geometry, motion, state layers, every component token and the
    /// <see cref="DesignTokens.Extended"/> overrides. Colors live in <see cref="FlattenColors"/>.</summary>
    public static Dictionary<string, string> FlattenDesign(this DesignTokens t)
    {
        var v = new Dictionary<string, string>();

        // Focus ring
        v[Css.Tokens.Vars.FocusRing] = t.FocusRing;

        // Typography
        FlattenType(v, "display-large", t.Typography.DisplayLarge);
        FlattenType(v, "display-medium", t.Typography.DisplayMedium);
        FlattenType(v, "display-small", t.Typography.DisplaySmall);
        FlattenType(v, "headline-large", t.Typography.HeadlineLarge);
        FlattenType(v, "headline-medium", t.Typography.HeadlineMedium);
        FlattenType(v, "headline-small", t.Typography.HeadlineSmall);
        FlattenType(v, "title-large", t.Typography.TitleLarge);
        FlattenType(v, "title-medium", t.Typography.TitleMedium);
        FlattenType(v, "title-small", t.Typography.TitleSmall);
        FlattenType(v, "body-large", t.Typography.BodyLarge);
        FlattenType(v, "body-medium", t.Typography.BodyMedium);
        FlattenType(v, "body-small", t.Typography.BodySmall);
        FlattenType(v, "label-large", t.Typography.LabelLarge);
        FlattenType(v, "label-medium", t.Typography.LabelMedium);
        FlattenType(v, "label-small", t.Typography.LabelSmall);

        // Shape
        v[Css.Tokens.Shape.None] = t.Shape.None;
        v[Css.Tokens.Shape.ExtraSmall] = t.Shape.ExtraSmall;
        v[Css.Tokens.Shape.Small] = t.Shape.Small;
        v[Css.Tokens.Shape.Medium] = t.Shape.Medium;
        v[Css.Tokens.Shape.Large] = t.Shape.Large;
        v[Css.Tokens.Shape.ExtraLarge] = t.Shape.ExtraLarge;
        v[Css.Tokens.Shape.Full] = t.Shape.Full;

        // Spacing scale (padding/margin/gap)
        v[Css.Tokens.Spacing.S0] = t.Spacing.S0;
        v[Css.Tokens.Spacing.S1] = t.Spacing.S1;
        v[Css.Tokens.Spacing.S2] = t.Spacing.S2;
        v[Css.Tokens.Spacing.S3] = t.Spacing.S3;
        v[Css.Tokens.Spacing.S4] = t.Spacing.S4;
        v[Css.Tokens.Spacing.S5] = t.Spacing.S5;
        v[Css.Tokens.Spacing.S6] = t.Spacing.S6;
        v[Css.Tokens.Spacing.S8] = t.Spacing.S8;
        v[Css.Tokens.Spacing.S10] = t.Spacing.S10;
        v[Css.Tokens.Spacing.S12] = t.Spacing.S12;
        v[Css.Tokens.Spacing.S16] = t.Spacing.S16;
        v[Css.Tokens.Spacing.S24] = t.Spacing.S24;
        v[Css.Tokens.Spacing.S32] = t.Spacing.S32;

        // Elevation
        v[Css.Tokens.Elevation.Level0] = t.Elevation.Level0;
        v[Css.Tokens.Elevation.Level1] = t.Elevation.Level1;
        v[Css.Tokens.Elevation.Level2] = t.Elevation.Level2;
        v[Css.Tokens.Elevation.Level3] = t.Elevation.Level3;
        v[Css.Tokens.Elevation.Level4] = t.Elevation.Level4;
        v[Css.Tokens.Elevation.Level5] = t.Elevation.Level5;

        // Motion
        v[Css.Tokens.Motion.DurationShort1] = t.Motion.DurationShort1;
        v[Css.Tokens.Motion.DurationShort2] = t.Motion.DurationShort2;
        v[Css.Tokens.Motion.DurationShort3] = t.Motion.DurationShort3;
        v[Css.Tokens.Motion.DurationShort4] = t.Motion.DurationShort4;
        v[Css.Tokens.Motion.DurationMedium1] = t.Motion.DurationMedium1;
        v[Css.Tokens.Motion.DurationMedium2] = t.Motion.DurationMedium2;
        v[Css.Tokens.Motion.DurationLong1] = t.Motion.DurationLong1;
        v[Css.Tokens.Motion.DurationLong2] = t.Motion.DurationLong2;
        v[Css.Tokens.Motion.EasingStandard] = t.Motion.EasingStandard;
        v[Css.Tokens.Motion.EasingDecelerate] = t.Motion.EasingDecelerate;
        v[Css.Tokens.Motion.EasingAccelerate] = t.Motion.EasingAccelerate;
        v[Css.Tokens.Motion.EasingEmphasized] = t.Motion.EasingEmphasized;

        // State layers
        v[Css.Tokens.State.HoverOpacity] = t.State.HoverOpacity;
        v[Css.Tokens.State.SelectedOpacity] = t.State.SelectedOpacity;
        v[Css.Tokens.State.FocusOpacity] = t.State.FocusOpacity;
        v[Css.Tokens.State.PressedOpacity] = t.State.PressedOpacity;
        v[Css.Tokens.State.DraggedOpacity] = t.State.DraggedOpacity;
        v[Css.Tokens.State.DisabledOpacity] = t.State.DisabledOpacity;
        v[Css.Tokens.State.DisabledContainerOpacity] = t.State.DisabledContainerOpacity;
        v[Css.Tokens.State.HoverLayer] = t.State.HoverLayer;
        v[Css.Tokens.State.FocusLayer] = t.State.FocusLayer;
        v[Css.Tokens.State.PressedLayer] = t.State.PressedLayer;
        v[Css.Tokens.State.DraggedLayer] = t.State.DraggedLayer;

        #region BADGE
        v[Css.Tokens.Badge.Radius] = t.Badge.Radius;
        v[Css.Tokens.Badge.MinWidth.Xs] = t.Badge.MinWidthXs;
        v[Css.Tokens.Badge.MinWidth.Sm] = t.Badge.MinWidthSm;
        v[Css.Tokens.Badge.MinWidth.Md] = t.Badge.MinWidthMd;
        v[Css.Tokens.Badge.MinWidth.Lg] = t.Badge.MinWidthLg;
        v[Css.Tokens.Badge.MinWidth.Xl] = t.Badge.MinWidthXl;
        v[Css.Tokens.Badge.Height.Xs] = t.Badge.HeightXs;
        v[Css.Tokens.Badge.Height.Sm] = t.Badge.HeightSm;
        v[Css.Tokens.Badge.Height.Md] = t.Badge.HeightMd;
        v[Css.Tokens.Badge.Height.Lg] = t.Badge.HeightLg;
        v[Css.Tokens.Badge.Height.Xl] = t.Badge.HeightXl;
        v[Css.Tokens.Badge.DotSize.Xs] = t.Badge.DotSizeXs;
        v[Css.Tokens.Badge.DotSize.Sm] = t.Badge.DotSizeSm;
        v[Css.Tokens.Badge.DotSize.Md] = t.Badge.DotSizeMd;
        v[Css.Tokens.Badge.DotSize.Lg] = t.Badge.DotSizeLg;
        v[Css.Tokens.Badge.DotSize.Xl] = t.Badge.DotSizeXl;
        v[Css.Tokens.Badge.PaddingX.Xs] = t.Badge.PaddingXXs;
        v[Css.Tokens.Badge.PaddingX.Sm] = t.Badge.PaddingXSm;
        v[Css.Tokens.Badge.PaddingX.Md] = t.Badge.PaddingXMd;
        v[Css.Tokens.Badge.PaddingX.Lg] = t.Badge.PaddingXLg;
        v[Css.Tokens.Badge.PaddingX.Xl] = t.Badge.PaddingXXl;
        v[Css.Tokens.Badge.LabelSize.Xs] = t.Badge.LabelSizeXs;
        v[Css.Tokens.Badge.LabelSize.Sm] = t.Badge.LabelSizeSm;
        v[Css.Tokens.Badge.LabelSize.Md] = t.Badge.LabelSizeMd;
        v[Css.Tokens.Badge.LabelSize.Lg] = t.Badge.LabelSizeLg;
        v[Css.Tokens.Badge.LabelSize.Xl] = t.Badge.LabelSizeXl;
        v[Css.Tokens.Badge.Offset] = t.Badge.Offset;
        v[Css.Tokens.Badge.DotOffset] = t.Badge.DotOffset;
        #endregion

        #region ALERT
        v[Css.Tokens.Alert.Radius] = t.Alert.Radius;
        v[Css.Tokens.Alert.BodyOpacity] = t.Alert.BodyOpacity;
        v[Css.Tokens.Alert.CloseOpacity] = t.Alert.CloseOpacity;
        v[Css.Tokens.Alert.BorderWidth] = t.Alert.BorderWidth;
        v[Css.Tokens.Alert.Padding] = t.Alert.Padding;
        v[Css.Tokens.Alert.Gap] = t.Alert.Gap;
        #endregion

        #region BUTTON
        // Gaps
        v[Css.Tokens.Button.Gap.Xs] = t.Button.GapXs;
        v[Css.Tokens.Button.LoadingOpacity] = t.Button.LoadingOpacity;
        v[Css.Tokens.Button.ContainerRadius] = t.Button.ContainerRadius;
        v[Css.Tokens.Button.TextPaddingInline] = t.Button.TextPaddingInline;
        v[Css.Tokens.Button.Gap.Sm] = t.Button.GapSm;
        v[Css.Tokens.Button.Gap.Md] = t.Button.GapMd;
        v[Css.Tokens.Button.Gap.Lg] = t.Button.GapLg;
        v[Css.Tokens.Button.Gap.Xl] = t.Button.GapXl;

        // Fixed container heights (Heights)
        v[Css.Tokens.Button.Height.Xs] = t.Button.HeightXs;
        v[Css.Tokens.Button.Height.Sm] = t.Button.HeightSm;
        v[Css.Tokens.Button.Height.Md] = t.Button.HeightMd;
        v[Css.Tokens.Button.Height.Lg] = t.Button.HeightLg;
        v[Css.Tokens.Button.Height.Xl] = t.Button.HeightXl;

        // Inline padding (Padding Inline)
        v[Css.Tokens.Button.PaddingInline.Xs] = t.Button.PaddingInlineXs;
        v[Css.Tokens.Button.PaddingInline.Sm] = t.Button.PaddingInlineSm;
        v[Css.Tokens.Button.PaddingInline.Md] = t.Button.PaddingInlineMd;
        v[Css.Tokens.Button.PaddingInline.Lg] = t.Button.PaddingInlineLg;
        v[Css.Tokens.Button.PaddingInline.Xl] = t.Button.PaddingInlineXl;

        // Per-corner radii (Corner Radii) for all 5 sizes
        // Extra Small (XS)
        v[Css.Tokens.Button.Radius.XsTopLeft] = t.Button.RadiusXs.TopLeft;
        v[Css.Tokens.Button.Radius.XsTopRight] = t.Button.RadiusXs.TopRight;
        v[Css.Tokens.Button.Radius.XsBottomRight] = t.Button.RadiusXs.BottomRight;
        v[Css.Tokens.Button.Radius.XsBottomLeft] = t.Button.RadiusXs.BottomLeft;

        // Small (SM)
        v[Css.Tokens.Button.Radius.SmTopLeft] = t.Button.RadiusSm.TopLeft;
        v[Css.Tokens.Button.Radius.SmTopRight] = t.Button.RadiusSm.TopRight;
        v[Css.Tokens.Button.Radius.SmBottomRight] = t.Button.RadiusSm.BottomRight;
        v[Css.Tokens.Button.Radius.SmBottomLeft] = t.Button.RadiusSm.BottomLeft;

        // Medium (MD)
        v[Css.Tokens.Button.Radius.MdTopLeft] = t.Button.RadiusMd.TopLeft;
        v[Css.Tokens.Button.Radius.MdTopRight] = t.Button.RadiusMd.TopRight;
        v[Css.Tokens.Button.Radius.MdBottomRight] = t.Button.RadiusMd.BottomRight;
        v[Css.Tokens.Button.Radius.MdBottomLeft] = t.Button.RadiusMd.BottomLeft;

        // Large (LG)
        v[Css.Tokens.Button.Radius.LgTopLeft] = t.Button.RadiusLg.TopLeft;
        v[Css.Tokens.Button.Radius.LgTopRight] = t.Button.RadiusLg.TopRight;
        v[Css.Tokens.Button.Radius.LgBottomRight] = t.Button.RadiusLg.BottomRight;
        v[Css.Tokens.Button.Radius.LgBottomLeft] = t.Button.RadiusLg.BottomLeft;

        // Extra Large (XL)
        v[Css.Tokens.Button.Radius.XlTopLeft] = t.Button.RadiusXl.TopLeft;
        v[Css.Tokens.Button.Radius.XlTopRight] = t.Button.RadiusXl.TopRight;
        v[Css.Tokens.Button.Radius.XlBottomRight] = t.Button.RadiusXl.BottomRight;
        v[Css.Tokens.Button.Radius.XlBottomLeft] = t.Button.RadiusXl.BottomLeft;

        // Focus and shadow behavior
        v[Css.Tokens.Button.FocusOutline] = t.Button.FocusOutline;
        v[Css.Tokens.Button.FocusOutlineOffset] = t.Button.FocusOutlineOffset;
        v[Css.Tokens.Button.FocusShadow] = t.Button.FocusShadow;
        v[Css.Tokens.Button.FilledHoverShadow] = t.Button.FilledHoverShadow;

        // Icon size
        v[Css.Tokens.Button.IconSize.Xs] = t.Button.IconSizeXs;
        v[Css.Tokens.Button.IconSize.Sm] = t.Button.IconSizeSm;
        v[Css.Tokens.Button.IconSize.Md] = t.Button.IconSizeMd;
        v[Css.Tokens.Button.IconSize.Lg] = t.Button.IconSizeLg;
        v[Css.Tokens.Button.IconSize.Xl] = t.Button.IconSizeXl;

        // Label typography (across 5 sizes)
        FlattenBtnLabel(v, "xs", t.Button.LabelXs);
        FlattenBtnLabel(v, "sm", t.Button.LabelSm);
        FlattenBtnLabel(v, "md", t.Button.LabelMd);
        FlattenBtnLabel(v, "lg", t.Button.LabelLg);
        FlattenBtnLabel(v, "xl", t.Button.LabelXl);
        #endregion

        #region BUTTON GROUP
        v[Css.Tokens.ButtonGroup.Gap] = t.ButtonGroup.Gap;
        v[Css.Tokens.ButtonGroup.Overlap] = t.ButtonGroup.Overlap;
        v[Css.Tokens.ButtonGroup.OuterRadius] = t.ButtonGroup.OuterRadius;
        v[Css.Tokens.ButtonGroup.InnerRadius] = t.ButtonGroup.InnerRadius;
        v[Css.Tokens.ButtonGroup.ZActive] = t.ButtonGroup.ZActive;
        #endregion

        #region SPLIT BUTTON
        // Base seam gap
        v[Css.Tokens.SplitButton.Gap] = t.SplitButton.Gap;
        v[Css.Tokens.SplitButton.TriggerWidth] = t.SplitButton.TriggerWidth;

        // Chevron caret sizes for all 5 sizes
        v[Css.Tokens.SplitButton.CaretSize.Xs] = t.SplitButton.CaretSizeXs;
        v[Css.Tokens.SplitButton.CaretSize.Sm] = t.SplitButton.CaretSizeSm;
        v[Css.Tokens.SplitButton.CaretSize.Md] = t.SplitButton.CaretSizeMd;
        v[Css.Tokens.SplitButton.CaretSize.Lg] = t.SplitButton.CaretSizeLg;
        v[Css.Tokens.SplitButton.CaretSize.Xl] = t.SplitButton.CaretSizeXl;

        // Per-corner radii of the Main button (Main Radius) for all 5 sizes
        // XS
        v[Css.Tokens.SplitButton.MainRadius.XsTopLeft] = t.SplitButton.MainRadiusXs.TopLeft;
        v[Css.Tokens.SplitButton.MainRadius.XsTopRight] = t.SplitButton.MainRadiusXs.TopRight;
        v[Css.Tokens.SplitButton.MainRadius.XsBottomRight] = t.SplitButton.MainRadiusXs.BottomRight;
        v[Css.Tokens.SplitButton.MainRadius.XsBottomLeft] = t.SplitButton.MainRadiusXs.BottomLeft;
        // SM
        v[Css.Tokens.SplitButton.MainRadius.SmTopLeft] = t.SplitButton.MainRadiusSm.TopLeft;
        v[Css.Tokens.SplitButton.MainRadius.SmTopRight] = t.SplitButton.MainRadiusSm.TopRight;
        v[Css.Tokens.SplitButton.MainRadius.SmBottomRight] = t.SplitButton.MainRadiusSm.BottomRight;
        v[Css.Tokens.SplitButton.MainRadius.SmBottomLeft] = t.SplitButton.MainRadiusSm.BottomLeft;
        // MD
        v[Css.Tokens.SplitButton.MainRadius.MdTopLeft] = t.SplitButton.MainRadiusMd.TopLeft;
        v[Css.Tokens.SplitButton.MainRadius.MdTopRight] = t.SplitButton.MainRadiusMd.TopRight;
        v[Css.Tokens.SplitButton.MainRadius.MdBottomRight] = t.SplitButton.MainRadiusMd.BottomRight;
        v[Css.Tokens.SplitButton.MainRadius.MdBottomLeft] = t.SplitButton.MainRadiusMd.BottomLeft;
        // LG
        v[Css.Tokens.SplitButton.MainRadius.LgTopLeft] = t.SplitButton.MainRadiusLg.TopLeft;
        v[Css.Tokens.SplitButton.MainRadius.LgTopRight] = t.SplitButton.MainRadiusLg.TopRight;
        v[Css.Tokens.SplitButton.MainRadius.LgBottomRight] = t.SplitButton.MainRadiusLg.BottomRight;
        v[Css.Tokens.SplitButton.MainRadius.LgBottomLeft] = t.SplitButton.MainRadiusLg.BottomLeft;
        // XL
        v[Css.Tokens.SplitButton.MainRadius.XlTopLeft] = t.SplitButton.MainRadiusXl.TopLeft;
        v[Css.Tokens.SplitButton.MainRadius.XlTopRight] = t.SplitButton.MainRadiusXl.TopRight;
        v[Css.Tokens.SplitButton.MainRadius.XlBottomRight] = t.SplitButton.MainRadiusXl.BottomRight;
        v[Css.Tokens.SplitButton.MainRadius.XlBottomLeft] = t.SplitButton.MainRadiusXl.BottomLeft;

        // Per-corner radii of the Trigger button (Trigger Radius) for all 5 sizes
        // XS
        v[Css.Tokens.SplitButton.TriggerRadius.XsTopLeft] = t.SplitButton.TriggerRadiusXs.TopLeft;
        v[Css.Tokens.SplitButton.TriggerRadius.XsTopRight] = t.SplitButton.TriggerRadiusXs.TopRight;
        v[Css.Tokens.SplitButton.TriggerRadius.XsBottomRight] = t.SplitButton.TriggerRadiusXs.BottomRight;
        v[Css.Tokens.SplitButton.TriggerRadius.XsBottomLeft] = t.SplitButton.TriggerRadiusXs.BottomLeft;
        // SM
        v[Css.Tokens.SplitButton.TriggerRadius.SmTopLeft] = t.SplitButton.TriggerRadiusSm.TopLeft;
        v[Css.Tokens.SplitButton.TriggerRadius.SmTopRight] = t.SplitButton.TriggerRadiusSm.TopRight;
        v[Css.Tokens.SplitButton.TriggerRadius.SmBottomRight] = t.SplitButton.TriggerRadiusSm.BottomRight;
        v[Css.Tokens.SplitButton.TriggerRadius.SmBottomLeft] = t.SplitButton.TriggerRadiusSm.BottomLeft;
        // MD
        v[Css.Tokens.SplitButton.TriggerRadius.MdTopLeft] = t.SplitButton.TriggerRadiusMd.TopLeft;
        v[Css.Tokens.SplitButton.TriggerRadius.MdTopRight] = t.SplitButton.TriggerRadiusMd.TopRight;
        v[Css.Tokens.SplitButton.TriggerRadius.MdBottomRight] = t.SplitButton.TriggerRadiusMd.BottomRight;
        v[Css.Tokens.SplitButton.TriggerRadius.MdBottomLeft] = t.SplitButton.TriggerRadiusMd.BottomLeft;
        // LG
        v[Css.Tokens.SplitButton.TriggerRadius.LgTopLeft] = t.SplitButton.TriggerRadiusLg.TopLeft;
        v[Css.Tokens.SplitButton.TriggerRadius.LgTopRight] = t.SplitButton.TriggerRadiusLg.TopRight;
        v[Css.Tokens.SplitButton.TriggerRadius.LgBottomRight] = t.SplitButton.TriggerRadiusLg.BottomRight;
        v[Css.Tokens.SplitButton.TriggerRadius.LgBottomLeft] = t.SplitButton.TriggerRadiusLg.BottomLeft;
        // XL
        v[Css.Tokens.SplitButton.TriggerRadius.XlTopLeft] = t.SplitButton.TriggerRadiusXl.TopLeft;
        v[Css.Tokens.SplitButton.TriggerRadius.XlTopRight] = t.SplitButton.TriggerRadiusXl.TopRight;
        v[Css.Tokens.SplitButton.TriggerRadius.XlBottomRight] = t.SplitButton.TriggerRadiusXl.BottomRight;
        v[Css.Tokens.SplitButton.TriggerRadius.XlBottomLeft] = t.SplitButton.TriggerRadiusXl.BottomLeft;
        #endregion

        #region TOGGLE BUTTON
        v[Css.Tokens.ToggleButton.Height.Sm] = t.ToggleButton.HeightSm;
        v[Css.Tokens.ToggleButton.HeightXs] = t.ToggleButton.HeightXs;
        v[Css.Tokens.ToggleButton.HeightXl] = t.ToggleButton.HeightXl;
        v[Css.Tokens.ToggleButton.PaddingXs] = t.ToggleButton.PaddingXs;
        v[Css.Tokens.ToggleButton.PaddingXl] = t.ToggleButton.PaddingXl;
        v[Css.Tokens.ToggleButton.Height.Md] = t.ToggleButton.HeightMd;
        v[Css.Tokens.ToggleButton.Height.Lg] = t.ToggleButton.HeightLg;

        v[Css.Tokens.ToggleButton.Padding.Sm] = t.ToggleButton.PaddingSm;
        v[Css.Tokens.ToggleButton.Padding.Md] = t.ToggleButton.PaddingMd;
        v[Css.Tokens.ToggleButton.Padding.Lg] = t.ToggleButton.PaddingLg;

        v[Css.Tokens.ToggleButton.Gap] = t.ToggleButton.Gap;
        v[Css.Tokens.ToggleButton.Radius] = t.ToggleButton.Radius;

        v[Css.Tokens.ToggleButton.RadiusSelected.Sm] = t.ToggleButton.RadiusSelectedSm;
        v[Css.Tokens.ToggleButton.RadiusSelected.Md] = t.ToggleButton.RadiusSelectedMd;
        v[Css.Tokens.ToggleButton.RadiusSelected.Lg] = t.ToggleButton.RadiusSelectedLg;

        v[Css.Tokens.ToggleButton.RestBg] = t.ToggleButton.RestBg;
        v[Css.Tokens.ToggleButton.RestColor] = t.ToggleButton.RestColor;
        v[Css.Tokens.ToggleButton.SelectedBg] = t.ToggleButton.SelectedBg;
        v[Css.Tokens.ToggleButton.SelectedColor] = t.ToggleButton.SelectedColor;

        v[Css.Tokens.ToggleButton.GroupBorder] = t.ToggleButton.GroupBorder;
        v[Css.Tokens.ToggleButton.GroupRadius] = t.ToggleButton.GroupRadius;
        v[Css.Tokens.ToggleButton.GroupRadiusVertical] = t.ToggleButton.GroupRadiusVertical;
        v[Css.Tokens.ToggleButton.GroupDivider] = t.ToggleButton.GroupDivider;
        #endregion

        #region FAB
        v[Css.Tokens.Fab.Padding.Sm] = t.Fab.PaddingSm;
        v[Css.Tokens.Fab.Padding.Md] = t.Fab.PaddingMd;
        v[Css.Tokens.Fab.Padding.Lg] = t.Fab.PaddingLg;

        v[Css.Tokens.Fab.Radius.Sm] = t.Fab.RadiusSm;
        v[Css.Tokens.Fab.Radius.Md] = t.Fab.RadiusMd;
        v[Css.Tokens.Fab.Radius.Lg] = t.Fab.RadiusLg;

        v[Css.Tokens.Fab.Gap] = t.Fab.Gap;
        v[Css.Tokens.Fab.Shadow] = t.Fab.Shadow;
        v[Css.Tokens.Fab.HoverShadow] = t.Fab.HoverShadow;
        v[Css.Tokens.Fab.AnchorOffset] = t.Fab.AnchorOffset;
        #endregion

        #region MENU
        v[Css.Tokens.Checkbox.BorderWidth] = t.Checkbox.BorderWidth;
        v[Css.Tokens.Checkbox.Size] = t.Checkbox.Size;
        v[Css.Tokens.Checkbox.Radius] = t.Checkbox.Radius;
        v[Css.Tokens.Checkbox.StateLayerHover] = t.Checkbox.StateLayerHover;
        v[Css.Tokens.Checkbox.StateLayerHoverChecked] = t.Checkbox.StateLayerHoverChecked;
        v[Css.Tokens.Checkbox.FocusOutline] = t.Checkbox.FocusOutline;
        v[Css.Tokens.Checkbox.FocusOutlineOffset] = t.Checkbox.FocusOutlineOffset;
        v[Css.Tokens.Checkbox.FocusShadow] = t.Checkbox.FocusShadow;
        v[Css.Tokens.Radio.StateLayerHover] = t.Radio.StateLayerHover;
        v[Css.Tokens.Radio.Size] = t.Radio.Size;
        v[Css.Tokens.Radio.StateLayerHoverChecked] = t.Radio.StateLayerHoverChecked;
        v[Css.Tokens.Chip.Radius] = t.Chip.Radius;
        v[Css.Tokens.Chip.Height] = t.Chip.Height;
        v[Css.Tokens.Tabs.IndicatorThickness] = t.Tabs.IndicatorThickness;
        v[Css.Tokens.Tabs.ActiveWeight] = t.Tabs.ActiveWeight;
        v[Css.Tokens.Tabs.CloseOpacity] = t.Tabs.CloseOpacity;
        v[Css.Tokens.Tabs.LabelFont] = t.Tabs.LabelFont;
        v[Css.Tokens.Tabs.LabelSize] = t.Tabs.LabelSize;
        v[Css.Tokens.Tabs.LabelWeight] = t.Tabs.LabelWeight;
        v[Css.Tokens.Tabs.ScrollShadowOpacity] = t.Tabs.ScrollShadowOpacity;
        v[Css.Tokens.Tabs.ActiveColor] = t.Tabs.ActiveColor;
        v[Css.Tokens.Tabs.InactiveColor] = t.Tabs.InactiveColor;
        v[Css.Tokens.Tabs.DividerColor] = t.Tabs.DividerColor;
        v[Css.Tokens.Tabs.SelectedBg] = t.Tabs.SelectedBg;
        v[Css.Tokens.Tabs.SelectedFg] = t.Tabs.SelectedFg;
        v[Css.Tokens.Tabs.FilledBg] = t.Tabs.FilledBg;
        v[Css.Tokens.Tabs.FilledFg] = t.Tabs.FilledFg;
        v[Css.Tokens.Tabs.TrackBg] = t.Tabs.TrackBg;
        v[Css.Tokens.Tabs.PillRadius] = t.Tabs.PillRadius;
        v[Css.Tokens.TableOfContents.ActiveColor] = t.TableOfContents.ActiveColor;
        v[Css.Tokens.TableOfContents.ActiveWeight] = t.TableOfContents.ActiveWeight;
        v[Css.Tokens.TableOfContents.HoverBgOpacity] = t.TableOfContents.HoverBgOpacity;
        v[Css.Tokens.TableOfContents.LineHeight] = t.TableOfContents.LineHeight;
        v[Css.Tokens.TableOfContents.TitleTracking] = t.TableOfContents.TitleTracking;
        v[Css.Tokens.TableOfContents.TitleWeight] = t.TableOfContents.TitleWeight;
        v[Css.Tokens.TableOfContents.InactiveColor] = t.TableOfContents.InactiveColor;
        v[Css.Tokens.TableOfContents.TitleColor] = t.TableOfContents.TitleColor;
        v[Css.Tokens.TableOfContents.RailColor] = t.TableOfContents.RailColor;
        v[Css.Tokens.TableOfContents.RailWidth] = t.TableOfContents.RailWidth;
        v[Css.Tokens.TableOfContents.ActiveBg] = t.TableOfContents.ActiveBg;
        v[Css.Tokens.TableOfContents.ActiveRadius] = t.TableOfContents.ActiveRadius;
        v[Css.Tokens.TableOfContents.MarkerWidth] = t.TableOfContents.MarkerWidth;
        v[Css.Tokens.TableOfContents.LinkPadX] = t.TableOfContents.LinkPadX;
        v[Css.Tokens.TableOfContents.Indent] = t.TableOfContents.Indent;

        // Size-dependent geometry is emitted as one token per size; the component's size class reads the
        // matching one. The theme owns every value, so the component CSS needs no defaults of its own.
        v[Css.Tokens.Slider.TrackHeight.Xs] = t.Slider.TrackHeightXs;
        v[Css.Tokens.Slider.TrackHeight.Sm] = t.Slider.TrackHeightSm;
        v[Css.Tokens.Slider.TrackHeight.Md] = t.Slider.TrackHeightMd;
        v[Css.Tokens.Slider.TrackHeight.Lg] = t.Slider.TrackHeightLg;
        v[Css.Tokens.Slider.TrackHeight.Xl] = t.Slider.TrackHeightXl;
        v[Css.Tokens.Slider.TrackRadius.Xs] = t.Slider.TrackRadiusXs;
        v[Css.Tokens.Slider.TrackRadius.Sm] = t.Slider.TrackRadiusSm;
        v[Css.Tokens.Slider.TrackRadius.Md] = t.Slider.TrackRadiusMd;
        v[Css.Tokens.Slider.TrackRadius.Lg] = t.Slider.TrackRadiusLg;
        v[Css.Tokens.Slider.TrackRadius.Xl] = t.Slider.TrackRadiusXl;
        v[Css.Tokens.Slider.HandleHeight.Xs] = t.Slider.HandleHeightXs;
        v[Css.Tokens.Slider.HandleHeight.Sm] = t.Slider.HandleHeightSm;
        v[Css.Tokens.Slider.HandleHeight.Md] = t.Slider.HandleHeightMd;
        v[Css.Tokens.Slider.HandleHeight.Lg] = t.Slider.HandleHeightLg;
        v[Css.Tokens.Slider.HandleHeight.Xl] = t.Slider.HandleHeightXl;
        v[Css.Tokens.Slider.IconSize.Xs] = t.Slider.IconSizeXs;
        v[Css.Tokens.Slider.IconSize.Sm] = t.Slider.IconSizeSm;
        v[Css.Tokens.Slider.IconSize.Md] = t.Slider.IconSizeMd;
        v[Css.Tokens.Slider.IconSize.Lg] = t.Slider.IconSizeLg;
        v[Css.Tokens.Slider.IconSize.Xl] = t.Slider.IconSizeXl;
        v[Css.Tokens.Slider.Length] = t.Slider.Length;
        v[Css.Tokens.Slider.GapRadius] = t.Slider.GapRadius;
        v[Css.Tokens.Slider.Gap] = t.Slider.Gap;
        v[Css.Tokens.Slider.HandleWidth] = t.Slider.HandleWidth;
        v[Css.Tokens.Slider.HandlePressedWidth] = t.Slider.HandlePressedWidth;
        v[Css.Tokens.Slider.HandleRadius] = t.Slider.HandleRadius;
        v[Css.Tokens.Slider.HandleClipPath] = t.Slider.HandleClipPath;
        v[Css.Tokens.Slider.HandleBorderWidth] = t.Slider.HandleBorderWidth;
        v[Css.Tokens.Slider.HandleFill] = t.Slider.HandleFill;
        v[Css.Tokens.Slider.ActiveColor] = t.Slider.ActiveColor;
        v[Css.Tokens.Slider.InactiveColor] = t.Slider.InactiveColor;
        v[Css.Tokens.Slider.StateLayerSize] = t.Slider.StateLayerSize;
        v[Css.Tokens.Slider.StateHoverOpacity] = t.Slider.StateHoverOpacity;
        v[Css.Tokens.Slider.StatePressedOpacity] = t.Slider.StatePressedOpacity;
        v[Css.Tokens.Slider.StopColor] = t.Slider.StopColor;
        v[Css.Tokens.Slider.StopColorSelected] = t.Slider.StopColorSelected;
        v[Css.Tokens.Slider.StopSize] = t.Slider.StopSize;
        v[Css.Tokens.Slider.ValueBg] = t.Slider.ValueBg;
        v[Css.Tokens.Slider.ValueColor] = t.Slider.ValueColor;

        v[Css.Tokens.Rating.Size.Xs] = t.Rating.SizeXs;
        v[Css.Tokens.Rating.Size.Sm] = t.Rating.SizeSm;
        v[Css.Tokens.Rating.Size.Md] = t.Rating.SizeMd;
        v[Css.Tokens.Rating.Size.Lg] = t.Rating.SizeLg;
        v[Css.Tokens.Rating.Size.Xl] = t.Rating.SizeXl;
        v[Css.Tokens.Rating.EmptyColor] = t.Rating.EmptyColor;
        v[Css.Tokens.Rating.FilledColor] = t.Rating.FilledColor;
        v[Css.Tokens.Rating.HoverScale] = t.Rating.HoverScale;

        v[Css.Tokens.Pagination.Size.Xs] = t.Pagination.SizeXs;
        v[Css.Tokens.Pagination.Size.Sm] = t.Pagination.SizeSm;
        v[Css.Tokens.Pagination.Size.Md] = t.Pagination.SizeMd;
        v[Css.Tokens.Pagination.Size.Lg] = t.Pagination.SizeLg;
        v[Css.Tokens.Pagination.Size.Xl] = t.Pagination.SizeXl;
        v[Css.Tokens.Pagination.Radius] = t.Pagination.Radius;
        v[Css.Tokens.Pagination.BorderColor] = t.Pagination.BorderColor;
        v[Css.Tokens.Pagination.ActiveColor] = t.Pagination.ActiveColor;

        v[Css.Tokens.Timeline.DotSize] = t.Timeline.DotSize;
        v[Css.Tokens.Timeline.DotBg] = t.Timeline.DotBg;
        v[Css.Tokens.Timeline.DotBorderWidth] = t.Timeline.DotBorderWidth;
        v[Css.Tokens.Timeline.DotIconSize] = t.Timeline.DotIconSize;
        v[Css.Tokens.Timeline.LineWidth] = t.Timeline.LineWidth;
        v[Css.Tokens.Timeline.LineColor] = t.Timeline.LineColor;
        v[Css.Tokens.Timeline.ConnectorWidth] = t.Timeline.ConnectorWidth;

        v[Css.Tokens.Stepper.CircleSize] = t.Stepper.CircleSize;
        v[Css.Tokens.Stepper.FocusRingThickness] = t.Stepper.FocusRingThickness;
        v[Css.Tokens.Stepper.FocusRingColor] = t.Stepper.FocusRingColor;
        v[Css.Tokens.Stepper.CircleBorderWidth] = t.Stepper.CircleBorderWidth;
        v[Css.Tokens.Stepper.CircleIconSize] = t.Stepper.CircleIconSize;
        v[Css.Tokens.Stepper.ConnectorThickness] = t.Stepper.ConnectorThickness;
        v[Css.Tokens.Stepper.ConnectorMinLength] = t.Stepper.ConnectorMinLength;
        v[Css.Tokens.Stepper.ConnectorColor] = t.Stepper.ConnectorColor;
        v[Css.Tokens.Stepper.ConnectorActiveColor] = t.Stepper.ConnectorActiveColor;
        v[Css.Tokens.Stepper.StepMinWidth] = t.Stepper.StepMinWidth;

        v[Css.Tokens.Tree.Indent] = t.Tree.Indent;
        v[Css.Tokens.Tree.ToggleHoverBg] = t.Tree.ToggleHoverBg;
        v[Css.Tokens.Tree.DropInsideBg] = t.Tree.DropInsideBg;
        v[Css.Tokens.Tree.ToggleSize] = t.Tree.ToggleSize;
        v[Css.Tokens.Tree.IconSize] = t.Tree.IconSize;
        v[Css.Tokens.Tree.SelectedBg] = t.Tree.SelectedBg;
        v[Css.Tokens.Tree.SelectedColor] = t.Tree.SelectedColor;
        v[Css.Tokens.Tree.DropIndicatorColor] = t.Tree.DropIndicatorColor;

        v[Css.Tokens.Calendar.MaxWidth] = t.Calendar.MaxWidth;
        v[Css.Tokens.Calendar.EventPadY] = t.Calendar.EventPadY;
        v[Css.Tokens.Calendar.MonthMinWidth] = t.Calendar.MonthMinWidth;
        v[Css.Tokens.Calendar.NavBtnSize] = t.Calendar.NavBtnSize;
        v[Css.Tokens.Calendar.CellMinHeight] = t.Calendar.CellMinHeight;
        v[Css.Tokens.Calendar.DayNumSize] = t.Calendar.DayNumSize;
        v[Css.Tokens.Calendar.TodayBg] = t.Calendar.TodayBg;
        v[Css.Tokens.Calendar.TodayColor] = t.Calendar.TodayColor;
        v[Css.Tokens.Calendar.SelectedBg] = t.Calendar.SelectedBg;
        v[Css.Tokens.Calendar.OtherMonthOpacity] = t.Calendar.OtherMonthOpacity;

        v[Css.Tokens.MenuPanel.MinWidth] = t.Menu.PanelMinWidth;
        v[Css.Tokens.MenuPanel.GroupDivider] = t.Menu.GroupDivider;
        v[Css.Tokens.MenuPanel.EnterAnimation] = t.Menu.EnterAnimation;
        v[Css.Tokens.MenuPanel.Radius] = t.Menu.PanelRadius;
        v[Css.Tokens.MenuPanel.Bg] = t.Menu.PanelBg;
        v[Css.Tokens.MenuPanel.Shadow] = t.Menu.PanelShadow;
        v[Css.Tokens.MenuPanel.PaddingBlock] = t.Menu.PanelPaddingBlock;
        v[Css.Tokens.MenuPanel.PaddingInline] = t.Menu.PanelPaddingInline;
        v[Css.Tokens.MenuPanel.ItemHeight] = t.Menu.ItemHeight;
        v[Css.Tokens.MenuPanel.ItemPaddingBlock] = t.Menu.ItemPaddingBlock;
        v[Css.Tokens.MenuPanel.ItemPaddingInline] = t.Menu.ItemPaddingInline;
        v[Css.Tokens.MenuPanel.ItemPaddingBlockDense] = t.Menu.ItemPaddingBlockDense;
        v[Css.Tokens.MenuPanel.ItemGap] = t.Menu.ItemGap;
        v[Css.Tokens.MenuPanel.ItemGapDense] = t.Menu.ItemGapDense;
        v[Css.Tokens.MenuPanel.ItemGapBetween] = t.Menu.ItemGapBetween;
        v[Css.Tokens.MenuPanel.ItemIconSize] = t.Menu.ItemIconSize;
        v[Css.Tokens.MenuPanel.ItemRadius] = t.Menu.ItemRadius;
        v[Css.Tokens.MenuPanel.ItemRadiusEnd] = t.Menu.ItemRadiusEnd;
        v[Css.Tokens.MenuPanel.GroupRadius] = t.Menu.GroupRadius;
        v[Css.Tokens.MenuPanel.GroupPadding] = t.Menu.GroupPadding;
        v[Css.Tokens.MenuPanel.GroupBg] = t.Menu.GroupBg;
        v[Css.Tokens.MenuPanel.GroupGap] = t.Menu.GroupGap;
        v[Css.Tokens.MenuPanel.GroupShadow] = t.Menu.GroupShadow;
        v[Css.Tokens.MenuPanel.GroupedPanelBg] = t.Menu.GroupedPanelBg;
        v[Css.Tokens.MenuPanel.GroupedPanelShadow] = t.Menu.GroupedPanelShadow;
        v[Css.Tokens.MenuPanel.ItemLabelFont] = t.Menu.ItemLabelFont;
        v[Css.Tokens.MenuPanel.ItemLabelWeight] = t.Menu.ItemLabelWeight;
        v[Css.Tokens.MenuPanel.ItemLabelSize] = t.Menu.ItemLabelSize;
        v[Css.Tokens.MenuPanel.ItemLabelHeight] = t.Menu.ItemLabelHeight;
        v[Css.Tokens.MenuPanel.ItemLabelSpacing] = t.Menu.ItemLabelSpacing;
        v[Css.Tokens.MenuPanel.ItemFocusRingColor] = t.Menu.ItemFocusRingColor;
        v[Css.Tokens.MenuPanel.ItemFocusRingThickness] = t.Menu.ItemFocusRingThickness;
        v[Css.Tokens.MenuPanel.ItemFocusRingOffset] = t.Menu.ItemFocusRingOffset;
        #endregion

        #region INPUT
        v[Css.Tokens.InputField.FilledBg] = t.Input.FilledBg;
        v[Css.Tokens.InputField.OutlinedBorder] = t.Input.OutlinedBorder;
        v[Css.Tokens.InputField.OutlinedRadius] = t.Input.OutlinedRadius;
        v[Css.Tokens.InputField.FilledBorderBottom] = t.Input.FilledBorderBottom;
        v[Css.Tokens.InputField.FocusRing] = t.Input.FocusRing;
        v[Css.Tokens.InputField.FocusOutline] = t.Input.FocusOutline;
        v[Css.Tokens.InputField.FocusOutlineOffset] = t.Input.FocusOutlineOffset;
        v[Css.Tokens.InputField.HoverBorderBottom] = t.Input.HoverBorderBottom;
        v[Css.Tokens.InputField.HoverStateLayer] = t.Input.HoverStateLayer;
        v[Css.Tokens.InputField.Padding] = t.Input.Padding;
        v[Css.Tokens.InputField.IconSize] = t.Input.IconSize;
        v[Css.Tokens.InputField.PlaceholderColor] = t.Input.PlaceholderColor;
        v[Css.Tokens.InputField.DisabledBg] = t.Input.DisabledBg;
        v[Css.Tokens.InputField.DisabledIndicator] = t.Input.DisabledIndicator;
        v[Css.Tokens.InputField.ErrorHoverIndicator] = t.Input.ErrorHoverIndicator;
        #endregion

        #region DIALOG
        v[Css.Tokens.DialogPanel.Radius] = t.Dialog.Radius;
        v[Css.Tokens.DialogPanel.IconSize] = t.Dialog.IconSize;
        #endregion

        #region DRAWER
        v[Css.Tokens.DrawerPanel.Width] = t.Drawer.Width;
        v[Css.Tokens.DrawerPanel.MiniWidth] = t.Drawer.MiniWidth;
        #endregion

        #region SNACKBAR
        v[Css.Tokens.SnackbarPanel.Radius] = t.Snackbar.Radius;
        v[Css.Tokens.SnackbarPanel.MinHeight] = t.Snackbar.MinHeight;
        v[Css.Tokens.SnackbarPanel.PaddingBlock] = t.Snackbar.PaddingBlock;
        v[Css.Tokens.SnackbarPanel.ProviderInset] = t.Snackbar.ProviderInset;
        v[Css.Tokens.SnackbarPanel.CloseOpacity] = t.Snackbar.CloseOpacity;
        #endregion

        #region TOOLTIP
        v[Css.Tokens.TooltipPopup.MaxWidth] = t.Tooltip.MaxWidth;
        v[Css.Tokens.TooltipPopup.Offset] = t.Tooltip.Offset;
        #endregion

        #region POPOVER
        v[Css.Tokens.PopoverPopup.Radius] = t.Popover.Radius;
        #endregion

        #region DATAGRID
        v[Css.Tokens.DataGridField.SortIconSize] = t.DataGrid.SortIconSize;
        v[Css.Tokens.DataGridField.SortPrioritySize] = t.DataGrid.SortPrioritySize;
        v[Css.Tokens.DataGridField.FilterIconSize] = t.DataGrid.FilterIconSize;
        v[Css.Tokens.DataGridField.BoolIconSize] = t.DataGrid.BoolIconSize;
        v[Css.Tokens.DataGridField.BtnIconSize] = t.DataGrid.BtnIconSize;
        v[Css.Tokens.DataGridField.CloseIconSize] = t.DataGrid.CloseIconSize;
        v[Css.Tokens.DataGridField.ChevronSize] = t.DataGrid.ChevronSize;
        v[Css.Tokens.DataGridField.DetailIconSize] = t.DataGrid.DetailIconSize;
        v[Css.Tokens.DataGridField.TreeToggleSize] = t.DataGrid.TreeToggleSize;
        v[Css.Tokens.DataGridField.CompositeLabelSize] = t.DataGrid.CompositeLabelSize;
        v[Css.Tokens.DataGridField.ResizeHandleWidth] = t.DataGrid.ResizeHandleWidth;
        v[Css.Tokens.DataGridField.RecordDividerWidth] = t.DataGrid.RecordDividerWidth;
        v[Css.Tokens.DataGridField.AggregateDividerWidth] = t.DataGrid.AggregateDividerWidth;
        v[Css.Tokens.DataGridField.FilterGroupRail] = t.DataGrid.FilterGroupRail;
        v[Css.Tokens.DataGridField.ActiveCellOutline] = t.DataGrid.ActiveCellOutline;
        v[Css.Tokens.DataGridField.ColumnPickerMinWidth] = t.DataGrid.ColumnPickerMinWidth;
        v[Css.Tokens.DataGridField.RowSelectedHoverPct] = t.DataGrid.RowSelectedHoverPct;
        v[Css.Tokens.DataGridField.RowEditingPct] = t.DataGrid.RowEditingPct;
        v[Css.Tokens.DataGridField.LoadingVeilPct] = t.DataGrid.LoadingVeilPct;
        v[Css.Tokens.DataGridField.LoadingDim] = t.DataGrid.LoadingDim;
        #endregion

        #region CARD
        v[Css.Tokens.CardField.ElevatedBg] = t.Card.ElevatedBg;
        v[Css.Tokens.CardField.FilledBg] = t.Card.FilledBg;
        v[Css.Tokens.CardField.FilledBorder] = t.Card.FilledBorder;
        v[Css.Tokens.CardField.OutlinedBg] = t.Card.OutlinedBg;
        v[Css.Tokens.CardField.OutlinedBorder] = t.Card.OutlinedBorder;
        v[Css.Tokens.CardField.TonalBg] = t.Card.TonalBg;
        v[Css.Tokens.CardField.TonalColor] = t.Card.TonalColor;
        v[Css.Tokens.CardField.TextColor] = t.Card.TextColor;
        v[Css.Tokens.CardField.Radius] = t.Card.Radius;
        v[Css.Tokens.CardField.Elevation] = t.Card.Elevation;
        v[Css.Tokens.CardField.ElevationHover] = t.Card.ElevationHover;
        v[Css.Tokens.CardField.SelectedBorder] = t.Card.SelectedBorder;
        v[Css.Tokens.CardField.SelectedBg] = t.Card.SelectedBg;
        v[Css.Tokens.CardField.StateLayer] = t.Card.StateLayer;
        v[Css.Tokens.CardField.PaddingTop] = t.Card.PaddingTop;
        v[Css.Tokens.CardField.PaddingRight] = t.Card.PaddingRight;
        v[Css.Tokens.CardField.PaddingBottom] = t.Card.PaddingBottom;
        v[Css.Tokens.CardField.PaddingLeft] = t.Card.PaddingLeft;
        v[Css.Tokens.CardField.ContentPadding] = t.Card.ContentPadding;
        v[Css.Tokens.CardField.HeaderPadding] = t.Card.HeaderPadding;
        v[Css.Tokens.CardField.FooterPadding] = t.Card.FooterPadding;
        v[Css.Tokens.CardField.ActionsPadding] = t.Card.ActionsPadding;
        v[Css.Tokens.CardField.ActionsGap] = t.Card.ActionsGap;
        v[Css.Tokens.CardField.MediaRadius] = t.Card.MediaRadius;
        v[Css.Tokens.CardField.TitleColor] = t.Card.TitleColor;
        v[Css.Tokens.CardField.TitleFontFamily] = t.Card.TitleFontFamily;
        v[Css.Tokens.CardField.TitleFontSize] = t.Card.TitleFontSize;
        v[Css.Tokens.CardField.SubtitleColor] = t.Card.SubtitleColor;
        v[Css.Tokens.CardField.SubtitleFontFamily] = t.Card.SubtitleFontFamily;
        v[Css.Tokens.CardField.SubtitleFontSize] = t.Card.SubtitleFontSize;
        v[Css.Tokens.CardField.TransitionDuration] = t.Card.TransitionDuration;
        v[Css.Tokens.CardField.TransitionEasing] = t.Card.TransitionEasing;
        #endregion

        #region AVATAR
        v[Css.Tokens.AvatarField.GroupSpacing] = t.Avatar.GroupSpacing;
        v[Css.Tokens.AvatarField.GroupBorderWidth] = t.Avatar.GroupBorderWidth;
        v[Css.Tokens.AvatarField.GroupBorderColor] = t.Avatar.GroupBorderColor;
        v[Css.Tokens.AvatarField.OverflowBg] = t.Avatar.OverflowBg;
        v[Css.Tokens.AvatarField.OverflowColor] = t.Avatar.OverflowColor;
        #endregion

        #region PROGRESS
        v[Css.Tokens.ProgressField.LinearHeight.Xs] = t.Progress.LinearHeightXs;
        v[Css.Tokens.ProgressField.LinearHeight.Sm] = t.Progress.LinearHeightSm;
        v[Css.Tokens.ProgressField.LinearHeight.Md] = t.Progress.LinearHeightMd;
        v[Css.Tokens.ProgressField.LinearHeight.Lg] = t.Progress.LinearHeightLg;
        v[Css.Tokens.ProgressField.LinearHeight.Xl] = t.Progress.LinearHeightXl;
        v[Css.Tokens.ProgressField.TrackRadius] = t.Progress.TrackRadius;
        v[Css.Tokens.ProgressField.Gap] = t.Progress.Gap;
        v[Css.Tokens.ProgressField.StopSize] = t.Progress.StopSize;
        v[Css.Tokens.ProgressField.StopInset] = t.Progress.StopInset;
        v[Css.Tokens.ProgressField.StopColor] = t.Progress.StopColor;
        v[Css.Tokens.ProgressField.BufferOpacity] = t.Progress.BufferOpacity;
        v[Css.Tokens.ProgressField.CircularSize.Xs] = t.Progress.CircularSizeXs;
        v[Css.Tokens.ProgressField.CircularSize.Sm] = t.Progress.CircularSizeSm;
        v[Css.Tokens.ProgressField.CircularSize.Md] = t.Progress.CircularSizeMd;
        v[Css.Tokens.ProgressField.CircularSize.Lg] = t.Progress.CircularSizeLg;
        v[Css.Tokens.ProgressField.CircularSize.Xl] = t.Progress.CircularSizeXl;
        v[Css.Tokens.ProgressField.CircularWidth.Xs] = t.Progress.CircularWidthXs;
        v[Css.Tokens.ProgressField.CircularWidth.Sm] = t.Progress.CircularWidthSm;
        v[Css.Tokens.ProgressField.CircularWidth.Md] = t.Progress.CircularWidthMd;
        v[Css.Tokens.ProgressField.CircularWidth.Lg] = t.Progress.CircularWidthLg;
        v[Css.Tokens.ProgressField.CircularWidth.Xl] = t.Progress.CircularWidthXl;
        v[Css.Tokens.ProgressField.CircularCap] = t.Progress.CircularCap;
        v[Css.Tokens.ProgressField.CircularGap] = t.Progress.CircularGap;
        v[Css.Tokens.ProgressField.WavyEnabled] = t.Progress.WavyEnabled;
        v[Css.Tokens.ProgressField.WavyHeight] = t.Progress.WavyHeight;
        v[Css.Tokens.ProgressField.WaveLength] = t.Progress.WaveLength;
        v[Css.Tokens.ProgressField.WaveAmplitude] = t.Progress.WaveAmplitude;
        v[Css.Tokens.ProgressField.WaveSpeed] = t.Progress.WaveSpeed;
        v[Css.Tokens.ProgressField.RingWaves] = t.Progress.RingWaves;
        v[Css.Tokens.ProgressField.RingWaveAmplitude] = t.Progress.RingWaveAmplitude;
        #endregion

        #region NAV
        v[Css.Tokens.NavField.ItemRadius] = t.Nav.ItemRadius;
        v[Css.Tokens.NavField.ActiveWeight] = t.Nav.ActiveWeight;
        v[Css.Tokens.NavField.BadgeWeight] = t.Nav.BadgeWeight;
        v[Css.Tokens.NavField.RailLabelLineHeight] = t.Nav.RailLabelLineHeight;
        v[Css.Tokens.NavField.IndicatorRadius] = t.Nav.IndicatorRadius;
        v[Css.Tokens.NavField.ActiveIndicator] = t.Nav.ActiveIndicator;
        v[Css.Tokens.NavField.ActiveLeftBar] = t.Nav.ActiveLeftBar;
        #endregion

        #region BOTTOM NAV
        v[Css.Tokens.BottomNavField.BarHeight] = t.BottomNav.BarHeight;
        v[Css.Tokens.BottomNavField.BarBg] = t.BottomNav.BarBg;
        v[Css.Tokens.BottomNavField.BorderColor] = t.BottomNav.BorderColor;
        v[Css.Tokens.BottomNavField.SafeAreaPadding] = t.BottomNav.SafeAreaPadding;
        v[Css.Tokens.BottomNavField.InactiveColor] = t.BottomNav.InactiveColor;
        v[Css.Tokens.BottomNavField.ActiveColor] = t.BottomNav.ActiveColor;
        v[Css.Tokens.BottomNavField.IconSize] = t.BottomNav.IconSize;
        v[Css.Tokens.BottomNavField.LabelFontSize] = t.BottomNav.LabelFontSize;
        v[Css.Tokens.BottomNavField.LabelFontWeight] = t.BottomNav.LabelFontWeight;
        v[Css.Tokens.BottomNavField.LabelFontWeightActive] = t.BottomNav.LabelFontWeightActive;
        v[Css.Tokens.BottomNavField.IndicatorBg] = t.BottomNav.IndicatorBg;
        v[Css.Tokens.BottomNavField.IndicatorRadius] = t.BottomNav.IndicatorRadius;
        v[Css.Tokens.BottomNavField.IndicatorSize] = t.BottomNav.IndicatorSize;
        #endregion

        #region SWITCH
        v[Css.Tokens.SwitchField.TrackWidth] = t.Switch.TrackWidth;
        v[Css.Tokens.SwitchField.TrackHeight] = t.Switch.TrackHeight;
        v[Css.Tokens.SwitchField.TrackOffBg] = t.Switch.TrackOffBg;
        v[Css.Tokens.SwitchField.TrackOnBg] = t.Switch.TrackOnBg;
        v[Css.Tokens.SwitchField.TrackBorder] = t.Switch.TrackBorder;
        v[Css.Tokens.SwitchField.TrackHoverBorderColor] = t.Switch.TrackHoverBorderColor;
        v[Css.Tokens.SwitchField.ThumbOffSize] = t.Switch.ThumbOffSize;
        v[Css.Tokens.SwitchField.ThumbOnSize] = t.Switch.ThumbOnSize;
        v[Css.Tokens.SwitchField.ThumbPressedOffSize] = t.Switch.ThumbPressedOffSize;
        v[Css.Tokens.SwitchField.ThumbPressedOnSize] = t.Switch.ThumbPressedOnSize;
        v[Css.Tokens.SwitchField.ThumbOffLeft] = t.Switch.ThumbOffLeft;
        v[Css.Tokens.SwitchField.ThumbOnLeft] = t.Switch.ThumbOnLeft;
        v[Css.Tokens.SwitchField.ThumbOffColor] = t.Switch.ThumbOffColor;
        v[Css.Tokens.SwitchField.ThumbOnColor] = t.Switch.ThumbOnColor;
        v[Css.Tokens.SwitchField.ThumbStateOffColor] = t.Switch.ThumbStateOffColor;
        v[Css.Tokens.SwitchField.ThumbStateOnColor] = t.Switch.ThumbStateOnColor;
        v[Css.Tokens.SwitchField.IconSize] = t.Switch.IconSize;
        v[Css.Tokens.SwitchField.IconOffColor] = t.Switch.IconOffColor;
        v[Css.Tokens.SwitchField.IconOnColor] = t.Switch.IconOnColor;
        v[Css.Tokens.SwitchField.FocusOutline] = t.Switch.FocusOutline;
        v[Css.Tokens.SwitchField.FocusOutlineOffset] = t.Switch.FocusOutlineOffset;
        v[Css.Tokens.SwitchField.FocusShadow] = t.Switch.FocusShadow;
        v[Css.Tokens.SwitchField.TrackHoverOffBg] = t.Switch.TrackHoverOffBg;
        v[Css.Tokens.SwitchField.TrackHoverOnBg] = t.Switch.TrackHoverOnBg;
        v[Css.Tokens.SwitchField.HoverShadowOff] = t.Switch.HoverShadowOff;
        v[Css.Tokens.SwitchField.HoverShadowOn] = t.Switch.HoverShadowOn;
        v[Css.Tokens.SwitchField.DisabledTrackBg] = t.Switch.DisabledTrackBg;
        v[Css.Tokens.SwitchField.DisabledTrackBorder] = t.Switch.DisabledTrackBorder;
        v[Css.Tokens.SwitchField.DisabledHandleBg] = t.Switch.DisabledHandleBg;
        #endregion

        #region COLOR PICKER
        v[Css.Tokens.ColorPickerField.CheckerColor] = t.ColorPicker.CheckerColor;
        v[Css.Tokens.ColorPickerField.ThumbBg] = t.ColorPicker.ThumbBg;
        v[Css.Tokens.ColorPickerField.ThumbBorderColor] = t.ColorPicker.ThumbBorderColor;
        #endregion

        #region APPBAR
        v[Css.Tokens.AppBarField.Gap] = t.AppBar.Gap;
        v[Css.Tokens.AppBarField.Height] = t.AppBar.Height;
        v[Css.Tokens.AppBarField.HeightDense] = t.AppBar.HeightDense;
        v[Css.Tokens.AppBarField.PaddingX] = t.AppBar.PaddingX;
        v[Css.Tokens.AppBarField.TitlePaddingX] = t.AppBar.TitlePaddingX;
        #endregion

        #region BREADCRUMB
        v[Css.Tokens.BreadcrumbField.LinkHoverOpacity] = t.Breadcrumb.LinkHoverOpacity;
        v[Css.Tokens.BreadcrumbField.SeparatorOpacity] = t.Breadcrumb.SeparatorOpacity;
        #endregion

        #region DATETIMEPICKER
        v[Css.Tokens.DateTimePickerField.PanelGap] = t.DateTimePicker.PanelGap;
        #endregion

        #region DROPZONE
        v[Css.Tokens.FileUploadField.BorderWidth] = t.FileUpload.BorderWidth;
        v[Css.Tokens.FileUploadField.HoverBg] = t.FileUpload.HoverBg;
        v[Css.Tokens.FileUploadField.DraggingBg] = t.FileUpload.DraggingBg;
        v[Css.Tokens.FileUploadField.ZoneMinHeight] = t.FileUpload.ZoneMinHeight;
        v[Css.Tokens.FileUploadField.ZoneRadius] = t.FileUpload.ZoneRadius;
        v[Css.Tokens.FileUploadField.FileIconSize] = t.FileUpload.FileIconSize;
        v[Css.Tokens.FileUploadField.DraggingRingWidth] = t.FileUpload.DraggingRingWidth;
        v[Css.Tokens.FileUploadField.IconSize] = t.FileUpload.IconSize;
        #endregion

        #region FORM
        v[Css.Tokens.FormField.HorizontalColumns] = t.Form.HorizontalColumns;
        #endregion

        #region LAYOUT
        v[Css.Tokens.LayoutField.AppBarHeight] = t.Layout.AppBarHeight;
        v[Css.Tokens.LayoutField.AppBarHeightDense] = t.Layout.AppBarHeightDense;
        v[Css.Tokens.LayoutField.AppBarBg] = t.Layout.AppBarBg;
        v[Css.Tokens.LayoutField.ContentPadding] = t.Layout.ContentPadding;
        v[Css.Tokens.LayoutField.ContentPaddingMobile] = t.Layout.ContentPaddingMobile;
        v[Css.Tokens.LayoutField.DrawerRailWidth] = t.Layout.DrawerRailWidth;
        v[Css.Tokens.LayoutField.DrawerWidth] = t.Layout.DrawerWidth;
        #endregion

        #region LINK
        v[Css.Tokens.LinkField.FocusRingWidth] = t.Link.FocusRingWidth;
        v[Css.Tokens.LinkField.HoverOpacity] = t.Link.HoverOpacity;
        #endregion

        #region OTP
        v[Css.Tokens.OtpField.BorderWidth] = t.Otp.BorderWidth;
        v[Css.Tokens.OtpField.CellHeight] = t.Otp.CellHeight;
        v[Css.Tokens.OtpField.CellWidth] = t.Otp.CellWidth;
        v[Css.Tokens.OtpField.FocusRingWidth] = t.Otp.FocusRingWidth;
        v[Css.Tokens.OtpField.FontSize] = t.Otp.FontSize;
        v[Css.Tokens.OtpField.FontWeight] = t.Otp.FontWeight;
        #endregion

        #region PICKER
        v[Css.Tokens.PickerField.OutsideOpacity] = t.Picker.OutsideOpacity;
        v[Css.Tokens.PickerField.DisabledOpacity] = t.Picker.DisabledOpacity;
        v[Css.Tokens.PickerField.WeekNumberOpacity] = t.Picker.WeekNumberOpacity;
        #endregion

        #region SCRIM
        v[Css.Tokens.ScrimField.Opacity] = t.Scrim.Opacity;
        #endregion

        #region SCROLLTOP
        v[Css.Tokens.ScrollTopField.TopInset] = t.ScrollTop.TopInset;
        v[Css.Tokens.ScrollTopField.TopSize] = t.ScrollTop.TopSize;
        #endregion

        #region SKELETON
        v[Css.Tokens.SkeletonField.PulseMinOpacity] = t.Skeleton.PulseMinOpacity;
        v[Css.Tokens.SkeletonField.WaveOpacity] = t.Skeleton.WaveOpacity;
        #endregion

        #region TABLE
        v[Css.Tokens.TableField.CellPaddingH] = t.Table.CellPaddingH;
        v[Css.Tokens.TableField.CellPaddingV] = t.Table.CellPaddingV;
        v[Css.Tokens.TableField.StripeOpacity] = t.Table.StripeOpacity;
        #endregion

        #region TIMEPICKER
        v[Css.Tokens.TimePickerField.ColumnsSepSize] = t.TimePicker.ColumnsSepSize;
        v[Css.Tokens.TimePickerField.DisplaySize] = t.TimePicker.DisplaySize;
        v[Css.Tokens.TimePickerField.HeadlineTracking] = t.TimePicker.HeadlineTracking;
        v[Css.Tokens.TimePickerField.PanelRadius] = t.TimePicker.PanelRadius;
        v[Css.Tokens.TimePickerField.TimeSepSize] = t.TimePicker.TimeSepSize;
        #endregion

        // Extended (deeply custom overrides)
        foreach (var (k, val) in t.Extended)
            v[k] = val;

        return v;
    }

    /// <summary>The ~47 color-role CSS variables for one mode (palette half).</summary>
    public static Dictionary<string, string> FlattenColors(this ColorScheme c)
    {
        var v = new Dictionary<string, string>
        {
            [Css.Tokens.Color.Primary] = c.Primary,
            [Css.Tokens.Color.OnPrimary] = c.OnPrimary,
            [Css.Tokens.Color.PrimaryContainer] = c.PrimaryContainer,
            [Css.Tokens.Color.OnPrimaryContainer] = c.OnPrimaryContainer,
            [Css.Tokens.Color.Secondary] = c.Secondary,
            [Css.Tokens.Color.OnSecondary] = c.OnSecondary,
            [Css.Tokens.Color.SecondaryContainer] = c.SecondaryContainer,
            [Css.Tokens.Color.OnSecondaryContainer] = c.OnSecondaryContainer,
            [Css.Tokens.Color.Tertiary] = c.Tertiary,
            [Css.Tokens.Color.OnTertiary] = c.OnTertiary,
            [Css.Tokens.Color.TertiaryContainer] = c.TertiaryContainer,
            [Css.Tokens.Color.OnTertiaryContainer] = c.OnTertiaryContainer,
            [Css.Tokens.Color.Error] = c.Error,
            [Css.Tokens.Color.OnError] = c.OnError,
            [Css.Tokens.Color.ErrorContainer] = c.ErrorContainer,
            [Css.Tokens.Color.OnErrorContainer] = c.OnErrorContainer,
            [Css.Tokens.Color.Success] = c.Success,
            [Css.Tokens.Color.OnSuccess] = c.OnSuccess,
            [Css.Tokens.Color.SuccessContainer] = c.SuccessContainer,
            [Css.Tokens.Color.OnSuccessContainer] = c.OnSuccessContainer,
            [Css.Tokens.Color.Warning] = c.Warning,
            [Css.Tokens.Color.OnWarning] = c.OnWarning,
            [Css.Tokens.Color.WarningContainer] = c.WarningContainer,
            [Css.Tokens.Color.OnWarningContainer] = c.OnWarningContainer,
            [Css.Tokens.Color.Info] = c.Info,
            [Css.Tokens.Color.OnInfo] = c.OnInfo,
            [Css.Tokens.Color.InfoContainer] = c.InfoContainer,
            [Css.Tokens.Color.OnInfoContainer] = c.OnInfoContainer,
            [Css.Tokens.Color.Surface] = c.Surface,
            [Css.Tokens.Color.OnSurface] = c.OnSurface,
            [Css.Tokens.Color.SurfaceVariant] = c.SurfaceVariant,
            [Css.Tokens.Color.OnSurfaceVariant] = c.OnSurfaceVariant,
            [Css.Tokens.Color.OnSurfaceVariant2] = c.OnSurfaceVariant2,
            [Css.Tokens.Color.SurfaceContainer] = c.SurfaceContainer,
            [Css.Tokens.Color.SurfaceContainerLow] = c.SurfaceContainerLow,
            [Css.Tokens.Color.SurfaceContainerHigh] = c.SurfaceContainerHigh,
            [Css.Tokens.Color.SurfaceContainerHighest] = c.SurfaceContainerHighest,
            [Css.Tokens.Color.Background] = c.Background,
            [Css.Tokens.Color.OnBackground] = c.OnBackground,
            [Css.Tokens.Color.Outline] = c.Outline,
            [Css.Tokens.Color.OutlineVariant] = c.OutlineVariant,
            [Css.Tokens.Color.InverseSurface] = c.InverseSurface,
            [Css.Tokens.Color.InverseOnSurface] = c.InverseOnSurface,
            [Css.Tokens.Color.InversePrimary] = c.InversePrimary,
            [Css.Tokens.Color.Scrim] = c.Scrim,
            [Css.Tokens.Color.Shadow] = c.Shadow,
            [Css.Tokens.Color.ShadowUmbra] = c.ShadowUmbra,
            [Css.Tokens.Color.ShadowPenumbra] = c.ShadowPenumbra,
        };
        return v;
    }

    private static void FlattenType(Dictionary<string, string> v, string scale, TypeStyle s)
    {
        v[Css.Tokens.Typography.Font(scale)] = s.FontFamily;
        v[Css.Tokens.Typography.Weight(scale)] = s.FontWeight;
        v[Css.Tokens.Typography.Size(scale)] = s.FontSize;
        v[Css.Tokens.Typography.Height(scale)] = s.LineHeight;
        v[Css.Tokens.Typography.Spacing(scale)] = s.LetterSpacing;
    }

    private static void FlattenBtnLabel(Dictionary<string, string> v, string size, TypeStyle s)
    {
        v[$"{Css.Tokens.Button.LabelPrefix}-{size}-font"] = s.FontFamily;
        v[$"{Css.Tokens.Button.LabelPrefix}-{size}-weight"] = s.FontWeight;
        v[$"{Css.Tokens.Button.LabelPrefix}-{size}-size"] = s.FontSize;
        v[$"{Css.Tokens.Button.LabelPrefix}-{size}-height"] = s.LineHeight;
        v[$"{Css.Tokens.Button.LabelPrefix}-{size}-spacing"] = s.LetterSpacing;
    }
}
