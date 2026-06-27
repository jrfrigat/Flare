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
        v[Css.Tokens.State.FocusOpacity] = t.State.FocusOpacity;
        v[Css.Tokens.State.PressedOpacity] = t.State.PressedOpacity;
        v[Css.Tokens.State.DraggedOpacity] = t.State.DraggedOpacity;
        v[Css.Tokens.State.DisabledOpacity] = t.State.DisabledOpacity;
        v[Css.Tokens.State.DisabledContainerOpacity] = t.State.DisabledContainerOpacity;

        #region BADGE
        v[Css.Tokens.Badge.Radius] = t.Badge.Radius;
        v[Css.Tokens.Badge.MinWidth] = t.Badge.MinWidth;
        v[Css.Tokens.Badge.Height] = t.Badge.Height;
        v[Css.Tokens.Badge.DotSize] = t.Badge.DotSize;
        v[Css.Tokens.Badge.PaddingX] = t.Badge.PaddingX;
        v[Css.Tokens.Badge.Offset] = t.Badge.Offset;
        v[Css.Tokens.Badge.DotOffset] = t.Badge.DotOffset;
        #endregion

        #region ALERT
        v[Css.Tokens.Alert.Radius] = t.Alert.Radius;
        v[Css.Tokens.Alert.BorderWidth] = t.Alert.BorderWidth;
        v[Css.Tokens.Alert.Padding] = t.Alert.Padding;
        v[Css.Tokens.Alert.Gap] = t.Alert.Gap;
        #endregion

        #region BUTTON
        // Зазоры (Gaps)
        v[Css.Tokens.Button.Gap.Xs] = t.Button.GapXs;
        v[Css.Tokens.Button.Gap.Sm] = t.Button.GapSm;
        v[Css.Tokens.Button.Gap.Md] = t.Button.GapMd;
        v[Css.Tokens.Button.Gap.Lg] = t.Button.GapLg;
        v[Css.Tokens.Button.Gap.Xl] = t.Button.GapXl;

        // Фиксированные высоты контейнеров (Heights)
        v[Css.Tokens.Button.Height.Xs] = t.Button.HeightXs;
        v[Css.Tokens.Button.Height.Sm] = t.Button.HeightSm;
        v[Css.Tokens.Button.Height.Md] = t.Button.HeightMd;
        v[Css.Tokens.Button.Height.Lg] = t.Button.HeightLg;
        v[Css.Tokens.Button.Height.Xl] = t.Button.HeightXl;

        // Боковые отступы (Padding Inline)
        v[Css.Tokens.Button.PaddingInline.Xs] = t.Button.PaddingInlineXs;
        v[Css.Tokens.Button.PaddingInline.Sm] = t.Button.PaddingInlineSm;
        v[Css.Tokens.Button.PaddingInline.Md] = t.Button.PaddingInlineMd;
        v[Css.Tokens.Button.PaddingInline.Lg] = t.Button.PaddingInlineLg;
        v[Css.Tokens.Button.PaddingInline.Xl] = t.Button.PaddingInlineXl;

        // Поугловые радиусы скругления (Corner Radii) под все 5 размеров
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

        // Поведение фокуса и теней
        v[Css.Tokens.Button.FocusOutline] = t.Button.FocusOutline;
        v[Css.Tokens.Button.FocusOutlineOffset] = t.Button.FocusOutlineOffset;
        v[Css.Tokens.Button.FocusShadow] = t.Button.FocusShadow;
        v[Css.Tokens.Button.FilledHoverShadow] = t.Button.FilledHoverShadow;

        // Размер иконки
        v[Css.Tokens.Button.IconSize.Xs] = t.Button.IconSizeXs;
        v[Css.Tokens.Button.IconSize.Sm] = t.Button.IconSizeSm;
        v[Css.Tokens.Button.IconSize.Md] = t.Button.IconSizeMd;
        v[Css.Tokens.Button.IconSize.Lg] = t.Button.IconSizeLg;
        v[Css.Tokens.Button.IconSize.Xl] = t.Button.IconSizeXl;

        // Типографика метки (по 5 размеров)
        FlattenBtnLabel(v, "xs", t.Button.LabelXs);
        FlattenBtnLabel(v, "sm", t.Button.LabelSm);
        FlattenBtnLabel(v, "md", t.Button.LabelMd);
        FlattenBtnLabel(v, "lg", t.Button.LabelLg);
        FlattenBtnLabel(v, "xl", t.Button.LabelXl);
        #endregion

        #region SPLIT BUTTON
        // Базовый зазор шва
        v[Css.Tokens.SplitButton.Gap] = t.SplitButton.Gap;
        v[Css.Tokens.SplitButton.TriggerWidth] = t.SplitButton.TriggerWidth;

        // Боковые отступы триггера под все 5 размеров
        v[Css.Tokens.SplitButton.TriggerPaddingInline.Xs] = t.SplitButton.TriggerPaddingXs;
        v[Css.Tokens.SplitButton.TriggerPaddingInline.Sm] = t.SplitButton.TriggerPaddingSm;
        v[Css.Tokens.SplitButton.TriggerPaddingInline.Md] = t.SplitButton.TriggerPaddingMd;
        v[Css.Tokens.SplitButton.TriggerPaddingInline.Lg] = t.SplitButton.TriggerPaddingLg;
        v[Css.Tokens.SplitButton.TriggerPaddingInline.Xl] = t.SplitButton.TriggerPaddingXl;

        // Размеры стрелки шеврона под все 5 размеров
        v[Css.Tokens.SplitButton.CaretSize.Xs] = t.SplitButton.CaretSizeXs;
        v[Css.Tokens.SplitButton.CaretSize.Sm] = t.SplitButton.CaretSizeSm;
        v[Css.Tokens.SplitButton.CaretSize.Md] = t.SplitButton.CaretSizeMd;
        v[Css.Tokens.SplitButton.CaretSize.Lg] = t.SplitButton.CaretSizeLg;
        v[Css.Tokens.SplitButton.CaretSize.Xl] = t.SplitButton.CaretSizeXl;

        // Поугловые радиусы Главной кнопки (Main Radius) под все 5 размеров
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

        // Поугловые радиусы Кнопки-триггера (Trigger Radius) под все 5 размеров
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
        v[Css.Tokens.Checkbox.Radius] = t.Checkbox.Radius;
        v[Css.Tokens.Checkbox.StateLayerHover] = t.Checkbox.StateLayerHover;
        v[Css.Tokens.Checkbox.StateLayerHoverChecked] = t.Checkbox.StateLayerHoverChecked;
        v[Css.Tokens.Checkbox.FocusOutline] = t.Checkbox.FocusOutline;
        v[Css.Tokens.Checkbox.FocusOutlineOffset] = t.Checkbox.FocusOutlineOffset;
        v[Css.Tokens.Checkbox.FocusShadow] = t.Checkbox.FocusShadow;
        v[Css.Tokens.Radio.StateLayerHover] = t.Radio.StateLayerHover;
        v[Css.Tokens.Radio.StateLayerHoverChecked] = t.Radio.StateLayerHoverChecked;
        v[Css.Tokens.Chip.Radius] = t.Chip.Radius;
        v[Css.Tokens.Chip.Height] = t.Chip.Height;
        v[Css.Tokens.Tabs.IndicatorThickness] = t.Tabs.IndicatorThickness;
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
        v[Css.Tokens.TableOfContents.InactiveColor] = t.TableOfContents.InactiveColor;
        v[Css.Tokens.TableOfContents.TitleColor] = t.TableOfContents.TitleColor;
        v[Css.Tokens.TableOfContents.RailColor] = t.TableOfContents.RailColor;
        v[Css.Tokens.TableOfContents.RailWidth] = t.TableOfContents.RailWidth;
        v[Css.Tokens.TableOfContents.ActiveBg] = t.TableOfContents.ActiveBg;
        v[Css.Tokens.TableOfContents.ActiveRadius] = t.TableOfContents.ActiveRadius;
        v[Css.Tokens.TableOfContents.MarkerWidth] = t.TableOfContents.MarkerWidth;
        v[Css.Tokens.TableOfContents.LinkPadX] = t.TableOfContents.LinkPadX;
        v[Css.Tokens.TableOfContents.Indent] = t.TableOfContents.Indent;

        // Geometry tokens are always emitted (MD3 = "initial" -> component per-size fallback),
        // so a theme switch deterministically overwrites the previous theme's values.
        v[Css.Tokens.Slider.TrackHeight] = t.Slider.TrackHeight;
        v[Css.Tokens.Slider.TrackRadius] = t.Slider.TrackRadius;
        v[Css.Tokens.Slider.GapRadius] = t.Slider.GapRadius;
        v[Css.Tokens.Slider.Gap] = t.Slider.Gap;
        v[Css.Tokens.Slider.HandleHeight] = t.Slider.HandleHeight;
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

        v[Css.Tokens.MenuPanel.MinWidth] = t.Menu.PanelMinWidth;
        v[Css.Tokens.MenuPanel.EnterAnimation] = t.Menu.EnterAnimation;
        v[Css.Tokens.MenuPanel.Radius] = t.Menu.PanelRadius;
        v[Css.Tokens.MenuPanel.Bg] = t.Menu.PanelBg;
        v[Css.Tokens.MenuPanel.Shadow] = t.Menu.PanelShadow;
        v[Css.Tokens.MenuPanel.PaddingBlock] = t.Menu.PanelPaddingBlock;
        v[Css.Tokens.MenuPanel.PaddingInline] = t.Menu.PanelPaddingInline;
        v[Css.Tokens.MenuPanel.ItemHeight] = t.Menu.ItemHeight;
        v[Css.Tokens.MenuPanel.ItemPaddingBlock] = t.Menu.ItemPaddingBlock;
        v[Css.Tokens.MenuPanel.ItemPaddingInline] = t.Menu.ItemPaddingInline;
        v[Css.Tokens.MenuPanel.ItemGap] = t.Menu.ItemGap;
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
        v[Css.Tokens.InputField.FilledRadius] = t.Input.FilledRadius;
        v[Css.Tokens.InputField.FocusBorder] = t.Input.FocusBorder;
        v[Css.Tokens.InputField.FocusBorderBottom] = t.Input.FocusBorderBottom;
        v[Css.Tokens.InputField.HoverBorderBottom] = t.Input.HoverBorderBottom;
        v[Css.Tokens.InputField.HoverStateLayer] = t.Input.HoverStateLayer;
        v[Css.Tokens.InputField.Padding] = t.Input.Padding;
        v[Css.Tokens.InputField.FontFamily] = t.Input.FontFamily;
        v[Css.Tokens.InputField.FontSize] = t.Input.FontSize;
        v[Css.Tokens.InputField.TextColor] = t.Input.TextColor;
        v[Css.Tokens.InputField.PlaceholderColor] = t.Input.PlaceholderColor;
        v[Css.Tokens.InputField.CaretColor] = t.Input.CaretColor;
        v[Css.Tokens.InputField.ErrorBorder] = t.Input.ErrorBorder;
        v[Css.Tokens.InputField.ErrorColor] = t.Input.ErrorColor;
        v[Css.Tokens.InputField.DisabledBg] = t.Input.DisabledBg;
        v[Css.Tokens.InputField.DisabledIndicator] = t.Input.DisabledIndicator;
        v[Css.Tokens.InputField.HelperFontSize] = t.Input.HelperFontSize;
        v[Css.Tokens.InputField.HelperColor] = t.Input.HelperColor;
        v[Css.Tokens.InputField.LabelFontFamily] = t.Input.LabelFontFamily;
        v[Css.Tokens.InputField.LabelFontSize] = t.Input.LabelFontSize;
        v[Css.Tokens.InputField.LabelFontWeight] = t.Input.LabelFontWeight;
        v[Css.Tokens.InputField.LabelColor] = t.Input.LabelColor;
        #endregion

        #region DIALOG
        v[Css.Tokens.DialogPanel.SurfaceColor] = t.Dialog.SurfaceColor;
        v[Css.Tokens.DialogPanel.Radius] = t.Dialog.Radius;
        v[Css.Tokens.DialogPanel.MaxWidth] = t.Dialog.MaxWidth;
        v[Css.Tokens.DialogPanel.MinWidth] = t.Dialog.MinWidth;
        v[Css.Tokens.DialogPanel.Padding] = t.Dialog.Padding;
        v[Css.Tokens.DialogPanel.HeaderPadding] = t.Dialog.HeaderPadding;
        v[Css.Tokens.DialogPanel.ActionsPadding] = t.Dialog.ActionsPadding;
        v[Css.Tokens.DialogPanel.ActionsGap] = t.Dialog.ActionsGap;
        v[Css.Tokens.DialogPanel.ScrimColor] = t.Dialog.ScrimColor;
        v[Css.Tokens.DialogPanel.ScrimOpacity] = t.Dialog.ScrimOpacity;
        v[Css.Tokens.DialogPanel.Elevation] = t.Dialog.Elevation;
        v[Css.Tokens.DialogPanel.TitleColor] = t.Dialog.TitleColor;
        v[Css.Tokens.DialogPanel.TitleFontFamily] = t.Dialog.TitleFontFamily;
        v[Css.Tokens.DialogPanel.TitleFontSize] = t.Dialog.TitleFontSize;
        v[Css.Tokens.DialogPanel.TitleFontWeight] = t.Dialog.TitleFontWeight;
        v[Css.Tokens.DialogPanel.ContentColor] = t.Dialog.ContentColor;
        v[Css.Tokens.DialogPanel.ContentFontFamily] = t.Dialog.ContentFontFamily;
        v[Css.Tokens.DialogPanel.ContentFontSize] = t.Dialog.ContentFontSize;
        v[Css.Tokens.DialogPanel.TransitionDuration] = t.Dialog.TransitionDuration;
        v[Css.Tokens.DialogPanel.TransitionEasing] = t.Dialog.TransitionEasing;
        v[Css.Tokens.DialogPanel.SizeXsWidth] = t.Dialog.SizeXsWidth;
        v[Css.Tokens.DialogPanel.SizeSmWidth] = t.Dialog.SizeSmWidth;
        v[Css.Tokens.DialogPanel.SizeMdWidth] = t.Dialog.SizeMdWidth;
        v[Css.Tokens.DialogPanel.SizeLgWidth] = t.Dialog.SizeLgWidth;
        v[Css.Tokens.DialogPanel.SizeXlWidth] = t.Dialog.SizeXlWidth;
        v[Css.Tokens.DialogPanel.SizeFullWidth] = t.Dialog.SizeFullWidth;
        #endregion

        #region DRAWER
        v[Css.Tokens.DrawerPanel.SurfaceColor] = t.Drawer.SurfaceColor;
        v[Css.Tokens.DrawerPanel.Width] = t.Drawer.Width;
        v[Css.Tokens.DrawerPanel.MiniWidth] = t.Drawer.MiniWidth;
        v[Css.Tokens.DrawerPanel.BreakpointSmWidth] = t.Drawer.BreakpointSmWidth;
        v[Css.Tokens.DrawerPanel.BreakpointMdWidth] = t.Drawer.BreakpointMdWidth;
        v[Css.Tokens.DrawerPanel.BreakpointLgWidth] = t.Drawer.BreakpointLgWidth;
        v[Css.Tokens.DrawerPanel.BreakpointXlWidth] = t.Drawer.BreakpointXlWidth;
        v[Css.Tokens.DrawerPanel.Elevation] = t.Drawer.Elevation;
        v[Css.Tokens.DrawerPanel.Radius] = t.Drawer.Radius;
        v[Css.Tokens.DrawerPanel.ScrimColor] = t.Drawer.ScrimColor;
        v[Css.Tokens.DrawerPanel.ScrimOpacity] = t.Drawer.ScrimOpacity;
        v[Css.Tokens.DrawerPanel.TransitionDuration] = t.Drawer.TransitionDuration;
        v[Css.Tokens.DrawerPanel.TransitionEasing] = t.Drawer.TransitionEasing;
        v[Css.Tokens.DrawerPanel.HeaderPadding] = t.Drawer.HeaderPadding;
        v[Css.Tokens.DrawerPanel.ContentPadding] = t.Drawer.ContentPadding;
        v[Css.Tokens.DrawerPanel.TitleColor] = t.Drawer.TitleColor;
        v[Css.Tokens.DrawerPanel.TitleFontFamily] = t.Drawer.TitleFontFamily;
        v[Css.Tokens.DrawerPanel.TitleFontSize] = t.Drawer.TitleFontSize;
        #endregion

        #region SNACKBAR
        v[Css.Tokens.SnackbarPanel.SurfaceColor] = t.Snackbar.SurfaceColor;
        v[Css.Tokens.SnackbarPanel.TextColor] = t.Snackbar.TextColor;
        v[Css.Tokens.SnackbarPanel.ActionColor] = t.Snackbar.ActionColor;
        v[Css.Tokens.SnackbarPanel.Radius] = t.Snackbar.Radius;
        v[Css.Tokens.SnackbarPanel.MinWidth] = t.Snackbar.MinWidth;
        v[Css.Tokens.SnackbarPanel.MaxWidth] = t.Snackbar.MaxWidth;
        v[Css.Tokens.SnackbarPanel.Height] = t.Snackbar.Height;
        v[Css.Tokens.SnackbarPanel.HeightMultiLine] = t.Snackbar.HeightMultiLine;
        v[Css.Tokens.SnackbarPanel.Padding] = t.Snackbar.Padding;
        v[Css.Tokens.SnackbarPanel.Gap] = t.Snackbar.Gap;
        v[Css.Tokens.SnackbarPanel.Elevation] = t.Snackbar.Elevation;
        v[Css.Tokens.SnackbarPanel.FontFamily] = t.Snackbar.FontFamily;
        v[Css.Tokens.SnackbarPanel.FontSize] = t.Snackbar.FontSize;
        v[Css.Tokens.SnackbarPanel.ActionFontWeight] = t.Snackbar.ActionFontWeight;
        v[Css.Tokens.SnackbarPanel.ActionFontSize] = t.Snackbar.ActionFontSize;
        v[Css.Tokens.SnackbarPanel.TransitionDuration] = t.Snackbar.TransitionDuration;
        v[Css.Tokens.SnackbarPanel.TransitionEasing] = t.Snackbar.TransitionEasing;
        v[Css.Tokens.SnackbarPanel.AutoHideDelay] = t.Snackbar.AutoHideDelay.ToString();
        v[Css.Tokens.SnackbarPanel.BottomOffset] = t.Snackbar.BottomOffset;
        v[Css.Tokens.SnackbarPanel.LeftOffset] = t.Snackbar.LeftOffset;
        v[Css.Tokens.SnackbarPanel.RightOffset] = t.Snackbar.RightOffset;
        v[Css.Tokens.SnackbarPanel.StackGap] = t.Snackbar.StackGap;
        #endregion

        #region TOOLTIP
        v[Css.Tokens.TooltipPopup.SurfaceColor] = t.Tooltip.SurfaceColor;
        v[Css.Tokens.TooltipPopup.TextColor] = t.Tooltip.TextColor;
        v[Css.Tokens.TooltipPopup.Radius] = t.Tooltip.Radius;
        v[Css.Tokens.TooltipPopup.Padding] = t.Tooltip.Padding;
        v[Css.Tokens.TooltipPopup.MaxWidth] = t.Tooltip.MaxWidth;
        v[Css.Tokens.TooltipPopup.FontFamily] = t.Tooltip.FontFamily;
        v[Css.Tokens.TooltipPopup.FontSize] = t.Tooltip.FontSize;
        v[Css.Tokens.TooltipPopup.FontWeight] = t.Tooltip.FontWeight;
        v[Css.Tokens.TooltipPopup.LineHeight] = t.Tooltip.LineHeight;
        v[Css.Tokens.TooltipPopup.Offset] = t.Tooltip.Offset;
        v[Css.Tokens.TooltipPopup.ArrowSize] = t.Tooltip.ArrowSize;
        v[Css.Tokens.TooltipPopup.TransitionDuration] = t.Tooltip.TransitionDuration;
        v[Css.Tokens.TooltipPopup.TransitionEasing] = t.Tooltip.TransitionEasing;
        v[Css.Tokens.TooltipPopup.ShowDelay] = t.Tooltip.ShowDelay.ToString();
        v[Css.Tokens.TooltipPopup.HideDelay] = t.Tooltip.HideDelay.ToString();
        #endregion

        #region POPOVER
        v[Css.Tokens.PopoverPopup.SurfaceColor] = t.Popover.SurfaceColor;
        v[Css.Tokens.PopoverPopup.Radius] = t.Popover.Radius;
        v[Css.Tokens.PopoverPopup.Elevation] = t.Popover.Elevation;
        v[Css.Tokens.PopoverPopup.Padding] = t.Popover.Padding;
        v[Css.Tokens.PopoverPopup.MinWidth] = t.Popover.MinWidth;
        v[Css.Tokens.PopoverPopup.MaxWidth] = t.Popover.MaxWidth;
        v[Css.Tokens.PopoverPopup.MaxHeight] = t.Popover.MaxHeight;
        v[Css.Tokens.PopoverPopup.Offset] = t.Popover.Offset;
        v[Css.Tokens.PopoverPopup.ArrowSize] = t.Popover.ArrowSize;
        v[Css.Tokens.PopoverPopup.ScrimColor] = t.Popover.ScrimColor;
        v[Css.Tokens.PopoverPopup.TransitionDuration] = t.Popover.TransitionDuration;
        v[Css.Tokens.PopoverPopup.TransitionEasing] = t.Popover.TransitionEasing;
        #endregion

        #region DATAGRID
        v[Css.Tokens.DataGridField.SurfaceColor] = t.DataGrid.SurfaceColor;
        v[Css.Tokens.DataGridField.HeaderBg] = t.DataGrid.HeaderBg;
        v[Css.Tokens.DataGridField.HeaderColor] = t.DataGrid.HeaderColor;
        v[Css.Tokens.DataGridField.HeaderFontFamily] = t.DataGrid.HeaderFontFamily;
        v[Css.Tokens.DataGridField.HeaderFontSize] = t.DataGrid.HeaderFontSize;
        v[Css.Tokens.DataGridField.HeaderFontWeight] = t.DataGrid.HeaderFontWeight;
        v[Css.Tokens.DataGridField.HeaderHeight] = t.DataGrid.HeaderHeight;
        v[Css.Tokens.DataGridField.HeaderPadding] = t.DataGrid.HeaderPadding;
        v[Css.Tokens.DataGridField.RowHeight] = t.DataGrid.RowHeight;
        v[Css.Tokens.DataGridField.RowHeightDense] = t.DataGrid.RowHeightDense;
        v[Css.Tokens.DataGridField.CellPadding] = t.DataGrid.CellPadding;
        v[Css.Tokens.DataGridField.CellColor] = t.DataGrid.CellColor;
        v[Css.Tokens.DataGridField.CellFontFamily] = t.DataGrid.CellFontFamily;
        v[Css.Tokens.DataGridField.CellFontSize] = t.DataGrid.CellFontSize;
        v[Css.Tokens.DataGridField.SelectedRowBg] = t.DataGrid.SelectedRowBg;
        v[Css.Tokens.DataGridField.SelectedRowColor] = t.DataGrid.SelectedRowColor;
        v[Css.Tokens.DataGridField.HoverRowBg] = t.DataGrid.HoverRowBg;
        v[Css.Tokens.DataGridField.SortIconColor] = t.DataGrid.SortIconColor;
        v[Css.Tokens.DataGridField.SortIconActiveColor] = t.DataGrid.SortIconActiveColor;
        v[Css.Tokens.DataGridField.BorderColor] = t.DataGrid.BorderColor;
        v[Css.Tokens.DataGridField.BorderWidth] = t.DataGrid.BorderWidth;
        v[Css.Tokens.DataGridField.FilterRowBg] = t.DataGrid.FilterRowBg;
        v[Css.Tokens.DataGridField.GroupHeaderBg] = t.DataGrid.GroupHeaderBg;
        v[Css.Tokens.DataGridField.GroupHeaderColor] = t.DataGrid.GroupHeaderColor;
        v[Css.Tokens.DataGridField.ToolbarBg] = t.DataGrid.ToolbarBg;
        v[Css.Tokens.DataGridField.ToolbarHeight] = t.DataGrid.ToolbarHeight;
        v[Css.Tokens.DataGridField.ToolbarPadding] = t.DataGrid.ToolbarPadding;
        v[Css.Tokens.DataGridField.EmptyStateBg] = t.DataGrid.EmptyStateBg;
        v[Css.Tokens.DataGridField.EmptyStateColor] = t.DataGrid.EmptyStateColor;
        v[Css.Tokens.DataGridField.ResizeHandleWidth] = t.DataGrid.ResizeHandleWidth;
        v[Css.Tokens.DataGridField.ResizeHandleColor] = t.DataGrid.ResizeHandleColor;
        v[Css.Tokens.DataGridField.ColumnPickerBg] = t.DataGrid.ColumnPickerBg;
        v[Css.Tokens.DataGridField.ColumnPickerElevation] = t.DataGrid.ColumnPickerElevation;
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
        v[Css.Tokens.AvatarField.SurfaceColor] = t.Avatar.SurfaceColor;
        v[Css.Tokens.AvatarField.TextColor] = t.Avatar.TextColor;
        v[Css.Tokens.AvatarField.IconColor] = t.Avatar.IconColor;
        v[Css.Tokens.AvatarField.RoundedRadius] = t.Avatar.RoundedRadius;
        v[Css.Tokens.AvatarField.SquareRadius] = t.Avatar.SquareRadius;
        v[Css.Tokens.AvatarField.SizeXs] = t.Avatar.SizeXs;
        v[Css.Tokens.AvatarField.SizeSm] = t.Avatar.SizeSm;
        v[Css.Tokens.AvatarField.SizeMd] = t.Avatar.SizeMd;
        v[Css.Tokens.AvatarField.SizeLg] = t.Avatar.SizeLg;
        v[Css.Tokens.AvatarField.SizeXl] = t.Avatar.SizeXl;
        v[Css.Tokens.AvatarField.FontFamily] = t.Avatar.FontFamily;
        v[Css.Tokens.AvatarField.FontSize] = t.Avatar.FontSize;
        v[Css.Tokens.AvatarField.FontWeight] = t.Avatar.FontWeight;
        v[Css.Tokens.AvatarField.GroupBorderWidth] = t.Avatar.GroupBorderWidth;
        v[Css.Tokens.AvatarField.GroupBorderColor] = t.Avatar.GroupBorderColor;
        v[Css.Tokens.AvatarField.GroupOverflowBg] = t.Avatar.GroupOverflowBg;
        v[Css.Tokens.AvatarField.GroupOverflowColor] = t.Avatar.GroupOverflowColor;
        #endregion

        #region PROGRESS
        v[Css.Tokens.ProgressField.TrackColor] = t.Progress.TrackColor;
        v[Css.Tokens.ProgressField.IndicatorColor] = t.Progress.IndicatorColor;
        v[Css.Tokens.ProgressField.CircularColor] = t.Progress.CircularColor;
        v[Css.Tokens.ProgressField.CircularTrackColor] = t.Progress.CircularTrackColor;
        v[Css.Tokens.ProgressField.LinearHeight] = t.Progress.LinearHeight;
        v[Css.Tokens.ProgressField.LinearHeightSm] = t.Progress.LinearHeightSm;
        v[Css.Tokens.ProgressField.LinearHeightLg] = t.Progress.LinearHeightLg;
        v[Css.Tokens.ProgressField.LinearRadius] = t.Progress.LinearRadius;
        v[Css.Tokens.ProgressField.CircularSize] = t.Progress.CircularSize;
        v[Css.Tokens.ProgressField.CircularSizeSm] = t.Progress.CircularSizeSm;
        v[Css.Tokens.ProgressField.CircularSizeLg] = t.Progress.CircularSizeLg;
        v[Css.Tokens.ProgressField.CircularStrokeWidth] = t.Progress.CircularStrokeWidth;
        v[Css.Tokens.ProgressField.CircularStrokeWidthSm] = t.Progress.CircularStrokeWidthSm;
        v[Css.Tokens.ProgressField.CircularStrokeWidthLg] = t.Progress.CircularStrokeWidthLg;
        v[Css.Tokens.ProgressField.IndeterminateDuration] = t.Progress.IndeterminateDuration;
        v[Css.Tokens.ProgressField.IndeterminateEasing] = t.Progress.IndeterminateEasing;
        v[Css.Tokens.ProgressField.BufferColor] = t.Progress.BufferColor;
        v[Css.Tokens.ProgressField.WavyDuration] = t.Progress.WavyDuration;
        v[Css.Tokens.ProgressField.TrackRadius] = t.Progress.TrackRadius;
        v[Css.Tokens.ProgressField.Gap] = t.Progress.Gap;
        v[Css.Tokens.ProgressField.StopSize] = t.Progress.StopSize;
        v[Css.Tokens.ProgressField.StopInset] = t.Progress.StopInset;
        v[Css.Tokens.ProgressField.CircularWidth] = t.Progress.CircularWidth;
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
        v[Css.Tokens.NavField.IndicatorRadius] = t.Nav.IndicatorRadius;
        v[Css.Tokens.NavField.ActiveIndicator] = t.Nav.ActiveIndicator;
        v[Css.Tokens.NavField.ActiveLeftBar] = t.Nav.ActiveLeftBar;
        #endregion

        #region SWITCH
        v[Css.Tokens.SwitchField.TrackWidth] = t.Switch.TrackWidth;
        v[Css.Tokens.SwitchField.TrackHeight] = t.Switch.TrackHeight;
        v[Css.Tokens.SwitchField.TrackWidthSm] = t.Switch.TrackWidthSm;
        v[Css.Tokens.SwitchField.TrackHeightSm] = t.Switch.TrackHeightSm;
        v[Css.Tokens.SwitchField.TrackWidthLg] = t.Switch.TrackWidthLg;
        v[Css.Tokens.SwitchField.TrackHeightLg] = t.Switch.TrackHeightLg;
        v[Css.Tokens.SwitchField.TrackRadius] = t.Switch.TrackRadius;
        v[Css.Tokens.SwitchField.TrackColor] = t.Switch.TrackColor;
        v[Css.Tokens.SwitchField.TrackBorderColor] = t.Switch.TrackBorderColor;
        v[Css.Tokens.SwitchField.TrackBorderWidth] = t.Switch.TrackBorderWidth;
        v[Css.Tokens.SwitchField.TrackColorSelected] = t.Switch.TrackColorSelected;
        v[Css.Tokens.SwitchField.TrackBorderColorSelected] = t.Switch.TrackBorderColorSelected;
        v[Css.Tokens.SwitchField.ThumbSize] = t.Switch.ThumbSize;
        v[Css.Tokens.SwitchField.ThumbSizeSm] = t.Switch.ThumbSizeSm;
        v[Css.Tokens.SwitchField.ThumbSizeLg] = t.Switch.ThumbSizeLg;
        v[Css.Tokens.SwitchField.ThumbColor] = t.Switch.ThumbColor;
        v[Css.Tokens.SwitchField.ThumbColorSelected] = t.Switch.ThumbColorSelected;
        v[Css.Tokens.SwitchField.ThumbIconColor] = t.Switch.ThumbIconColor;
        v[Css.Tokens.SwitchField.ThumbIconColorSelected] = t.Switch.ThumbIconColorSelected;
        v[Css.Tokens.SwitchField.ThumbShadow] = t.Switch.ThumbShadow;
        v[Css.Tokens.SwitchField.FocusOutlineWidth] = t.Switch.FocusOutlineWidth;
        v[Css.Tokens.SwitchField.FocusOutlineColor] = t.Switch.FocusOutlineColor;
        v[Css.Tokens.SwitchField.FocusOutlineOffset] = t.Switch.FocusOutlineOffset;
        v[Css.Tokens.SwitchField.TransitionDuration] = t.Switch.TransitionDuration;
        v[Css.Tokens.SwitchField.TransitionEasing] = t.Switch.TransitionEasing;
        v[Css.Tokens.SwitchField.PressedLayerColor] = t.Switch.PressedLayerColor;
        v[Css.Tokens.SwitchField.PressedLayerOpacity] = t.Switch.PressedLayerOpacity;
        v[Css.Tokens.SwitchField.DisabledOpacity] = t.Switch.DisabledOpacity;
        #endregion

        // Extended (Глубоко кастомные оверрайды)
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
