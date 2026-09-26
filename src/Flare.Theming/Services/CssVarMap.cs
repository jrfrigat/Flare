using Flare.Abstractions.Tokens;

namespace Flare.Theming;

/// <summary>Flattens theme token records into the CSS-variable name/value map injected at runtime.</summary>
public static partial class CssVarMap
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

        // Every [CssVar] token property, one assignment each - generated from the attributes (CssVarMapGenerator).
        // What follows by hand are only the compound tokens that expand to several variables.
        FlattenAttributed(v, t);

        // Typography
        FlattenType(v, Css.Tokens.Typography.DisplayLarge.Font, Css.Tokens.Typography.DisplayLarge.Weight, Css.Tokens.Typography.DisplayLarge.Size, Css.Tokens.Typography.DisplayLarge.Height, Css.Tokens.Typography.DisplayLarge.Spacing, t.Typography.DisplayLarge);
        FlattenType(v, Css.Tokens.Typography.DisplayMedium.Font, Css.Tokens.Typography.DisplayMedium.Weight, Css.Tokens.Typography.DisplayMedium.Size, Css.Tokens.Typography.DisplayMedium.Height, Css.Tokens.Typography.DisplayMedium.Spacing, t.Typography.DisplayMedium);
        FlattenType(v, Css.Tokens.Typography.DisplaySmall.Font, Css.Tokens.Typography.DisplaySmall.Weight, Css.Tokens.Typography.DisplaySmall.Size, Css.Tokens.Typography.DisplaySmall.Height, Css.Tokens.Typography.DisplaySmall.Spacing, t.Typography.DisplaySmall);
        FlattenType(v, Css.Tokens.Typography.HeadlineLarge.Font, Css.Tokens.Typography.HeadlineLarge.Weight, Css.Tokens.Typography.HeadlineLarge.Size, Css.Tokens.Typography.HeadlineLarge.Height, Css.Tokens.Typography.HeadlineLarge.Spacing, t.Typography.HeadlineLarge);
        FlattenType(v, Css.Tokens.Typography.HeadlineMedium.Font, Css.Tokens.Typography.HeadlineMedium.Weight, Css.Tokens.Typography.HeadlineMedium.Size, Css.Tokens.Typography.HeadlineMedium.Height, Css.Tokens.Typography.HeadlineMedium.Spacing, t.Typography.HeadlineMedium);
        FlattenType(v, Css.Tokens.Typography.HeadlineSmall.Font, Css.Tokens.Typography.HeadlineSmall.Weight, Css.Tokens.Typography.HeadlineSmall.Size, Css.Tokens.Typography.HeadlineSmall.Height, Css.Tokens.Typography.HeadlineSmall.Spacing, t.Typography.HeadlineSmall);
        FlattenType(v, Css.Tokens.Typography.TitleLarge.Font, Css.Tokens.Typography.TitleLarge.Weight, Css.Tokens.Typography.TitleLarge.Size, Css.Tokens.Typography.TitleLarge.Height, Css.Tokens.Typography.TitleLarge.Spacing, t.Typography.TitleLarge);
        FlattenType(v, Css.Tokens.Typography.TitleMedium.Font, Css.Tokens.Typography.TitleMedium.Weight, Css.Tokens.Typography.TitleMedium.Size, Css.Tokens.Typography.TitleMedium.Height, Css.Tokens.Typography.TitleMedium.Spacing, t.Typography.TitleMedium);
        FlattenType(v, Css.Tokens.Typography.TitleSmall.Font, Css.Tokens.Typography.TitleSmall.Weight, Css.Tokens.Typography.TitleSmall.Size, Css.Tokens.Typography.TitleSmall.Height, Css.Tokens.Typography.TitleSmall.Spacing, t.Typography.TitleSmall);
        FlattenType(v, Css.Tokens.Typography.BodyLarge.Font, Css.Tokens.Typography.BodyLarge.Weight, Css.Tokens.Typography.BodyLarge.Size, Css.Tokens.Typography.BodyLarge.Height, Css.Tokens.Typography.BodyLarge.Spacing, t.Typography.BodyLarge);
        FlattenType(v, Css.Tokens.Typography.BodyMedium.Font, Css.Tokens.Typography.BodyMedium.Weight, Css.Tokens.Typography.BodyMedium.Size, Css.Tokens.Typography.BodyMedium.Height, Css.Tokens.Typography.BodyMedium.Spacing, t.Typography.BodyMedium);
        FlattenType(v, Css.Tokens.Typography.BodySmall.Font, Css.Tokens.Typography.BodySmall.Weight, Css.Tokens.Typography.BodySmall.Size, Css.Tokens.Typography.BodySmall.Height, Css.Tokens.Typography.BodySmall.Spacing, t.Typography.BodySmall);
        FlattenType(v, Css.Tokens.Typography.LabelLarge.Font, Css.Tokens.Typography.LabelLarge.Weight, Css.Tokens.Typography.LabelLarge.Size, Css.Tokens.Typography.LabelLarge.Height, Css.Tokens.Typography.LabelLarge.Spacing, t.Typography.LabelLarge);
        FlattenType(v, Css.Tokens.Typography.LabelMedium.Font, Css.Tokens.Typography.LabelMedium.Weight, Css.Tokens.Typography.LabelMedium.Size, Css.Tokens.Typography.LabelMedium.Height, Css.Tokens.Typography.LabelMedium.Spacing, t.Typography.LabelMedium);
        FlattenType(v, Css.Tokens.Typography.LabelSmall.Font, Css.Tokens.Typography.LabelSmall.Weight, Css.Tokens.Typography.LabelSmall.Size, Css.Tokens.Typography.LabelSmall.Height, Css.Tokens.Typography.LabelSmall.Spacing, t.Typography.LabelSmall);

        // Motion
        FlattenMotion(v, t.Motion);

        #region BUTTON

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

        // Label typography (across 5 sizes)
        FlattenBtnLabel(v, Css.Tokens.Button.Label.Xs.Font, Css.Tokens.Button.Label.Xs.Weight, Css.Tokens.Button.Label.Xs.Size, Css.Tokens.Button.Label.Xs.Height, Css.Tokens.Button.Label.Xs.Spacing, t.Button.LabelXs);
        FlattenBtnLabel(v, Css.Tokens.Button.Label.Sm.Font, Css.Tokens.Button.Label.Sm.Weight, Css.Tokens.Button.Label.Sm.Size, Css.Tokens.Button.Label.Sm.Height, Css.Tokens.Button.Label.Sm.Spacing, t.Button.LabelSm);
        FlattenBtnLabel(v, Css.Tokens.Button.Label.Md.Font, Css.Tokens.Button.Label.Md.Weight, Css.Tokens.Button.Label.Md.Size, Css.Tokens.Button.Label.Md.Height, Css.Tokens.Button.Label.Md.Spacing, t.Button.LabelMd);
        FlattenBtnLabel(v, Css.Tokens.Button.Label.Lg.Font, Css.Tokens.Button.Label.Lg.Weight, Css.Tokens.Button.Label.Lg.Size, Css.Tokens.Button.Label.Lg.Height, Css.Tokens.Button.Label.Lg.Spacing, t.Button.LabelLg);
        FlattenBtnLabel(v, Css.Tokens.Button.Label.Xl.Font, Css.Tokens.Button.Label.Xl.Weight, Css.Tokens.Button.Label.Xl.Size, Css.Tokens.Button.Label.Xl.Height, Css.Tokens.Button.Label.Xl.Spacing, t.Button.LabelXl);
        #endregion

        #region SPLIT BUTTON

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

        foreach (var (k, val) in t.Extended)
            v[k] = val;

        return v;
    }

    /// <summary>The ~61 color-role CSS variables for one mode (palette half).</summary>
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
            [Css.Tokens.Color.SurfaceContainerLowest] = c.SurfaceContainerLowest,
            [Css.Tokens.Color.SurfaceContainerLow] = c.SurfaceContainerLow,
            [Css.Tokens.Color.SurfaceContainerHigh] = c.SurfaceContainerHigh,
            [Css.Tokens.Color.SurfaceContainerHighest] = c.SurfaceContainerHighest,
            [Css.Tokens.Color.SurfaceBright] = c.SurfaceBright,
            [Css.Tokens.Color.SurfaceDim] = c.SurfaceDim,
            [Css.Tokens.Color.PrimaryFixed] = c.PrimaryFixed,
            [Css.Tokens.Color.PrimaryFixedDim] = c.PrimaryFixedDim,
            [Css.Tokens.Color.OnPrimaryFixed] = c.OnPrimaryFixed,
            [Css.Tokens.Color.OnPrimaryFixedVariant] = c.OnPrimaryFixedVariant,
            [Css.Tokens.Color.SecondaryFixed] = c.SecondaryFixed,
            [Css.Tokens.Color.SecondaryFixedDim] = c.SecondaryFixedDim,
            [Css.Tokens.Color.OnSecondaryFixed] = c.OnSecondaryFixed,
            [Css.Tokens.Color.OnSecondaryFixedVariant] = c.OnSecondaryFixedVariant,
            [Css.Tokens.Color.TertiaryFixed] = c.TertiaryFixed,
            [Css.Tokens.Color.TertiaryFixedDim] = c.TertiaryFixedDim,
            [Css.Tokens.Color.OnTertiaryFixed] = c.OnTertiaryFixed,
            [Css.Tokens.Color.OnTertiaryFixedVariant] = c.OnTertiaryFixedVariant,
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

    // The five names are passed in rather than derived from a slug: a name built at runtime is one
    // Flare.CssAudit cannot match against the stylesheet, and the registry exists to be matched.
    private static void FlattenType(
        Dictionary<string, string> v, string font, string weight, string size, string height, string spacing,
        TypeStyle s)
    {
        v[font] = s.FontFamily;
        v[weight] = s.FontWeight;
        v[size] = s.FontSize;
        v[height] = s.LineHeight;
        v[spacing] = s.LetterSpacing;
    }

    private static void FlattenBtnLabel(
        Dictionary<string, string> v, string font, string weight, string size, string height, string spacing,
        TypeStyle s)
    {
        v[font] = s.FontFamily;
        v[weight] = s.FontWeight;
        v[size] = s.FontSize;
        v[height] = s.LineHeight;
        v[spacing] = s.LetterSpacing;
    }
}
