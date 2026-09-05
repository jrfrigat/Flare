namespace Flare.Components;

/// <summary>Helpers for resolving a <see cref="TypographyScale"/> to CSS.</summary>
internal static class FlareTypography
{
    /// <summary>
    /// Inline font declaration (family/weight/size/line-height) for a type scale, sourced from the
    /// active theme's <c>--flare-typescale-*</c> variables. Used by inputs where a typography utility
    /// class would be out-specified by the size-grid selectors, so the scale must be applied inline.
    /// </summary>
    public static string FontStyle(TypographyScale scale)
    {
        var t = Tokens(scale);
        return $"font-family:var({t.Font});"
             + $"font-weight:var({t.Weight});"
             + $"font-size:var({t.Size});"
             + $"line-height:var({t.Height})";
    }

    /// <summary>
    /// The four type-scale token NAMES for a scale. Named rather than built from a slug: a name
    /// assembled at runtime is one <c>Flare.CssAudit</c> cannot match against the stylesheet, so it can
    /// neither confirm the step exists nor catch a component asking for one that does not.
    /// </summary>
    public static (string Font, string Weight, string Size, string Height) Tokens(TypographyScale scale) =>
        scale switch
        {
            TypographyScale.DisplayLarge => (Css.Tokens.Typography.DisplayLarge.Font, Css.Tokens.Typography.DisplayLarge.Weight, Css.Tokens.Typography.DisplayLarge.Size, Css.Tokens.Typography.DisplayLarge.Height),
            TypographyScale.DisplayMedium => (Css.Tokens.Typography.DisplayMedium.Font, Css.Tokens.Typography.DisplayMedium.Weight, Css.Tokens.Typography.DisplayMedium.Size, Css.Tokens.Typography.DisplayMedium.Height),
            TypographyScale.DisplaySmall => (Css.Tokens.Typography.DisplaySmall.Font, Css.Tokens.Typography.DisplaySmall.Weight, Css.Tokens.Typography.DisplaySmall.Size, Css.Tokens.Typography.DisplaySmall.Height),
            TypographyScale.HeadlineLarge => (Css.Tokens.Typography.HeadlineLarge.Font, Css.Tokens.Typography.HeadlineLarge.Weight, Css.Tokens.Typography.HeadlineLarge.Size, Css.Tokens.Typography.HeadlineLarge.Height),
            TypographyScale.HeadlineMedium => (Css.Tokens.Typography.HeadlineMedium.Font, Css.Tokens.Typography.HeadlineMedium.Weight, Css.Tokens.Typography.HeadlineMedium.Size, Css.Tokens.Typography.HeadlineMedium.Height),
            TypographyScale.HeadlineSmall => (Css.Tokens.Typography.HeadlineSmall.Font, Css.Tokens.Typography.HeadlineSmall.Weight, Css.Tokens.Typography.HeadlineSmall.Size, Css.Tokens.Typography.HeadlineSmall.Height),
            TypographyScale.TitleLarge => (Css.Tokens.Typography.TitleLarge.Font, Css.Tokens.Typography.TitleLarge.Weight, Css.Tokens.Typography.TitleLarge.Size, Css.Tokens.Typography.TitleLarge.Height),
            TypographyScale.TitleMedium => (Css.Tokens.Typography.TitleMedium.Font, Css.Tokens.Typography.TitleMedium.Weight, Css.Tokens.Typography.TitleMedium.Size, Css.Tokens.Typography.TitleMedium.Height),
            TypographyScale.TitleSmall => (Css.Tokens.Typography.TitleSmall.Font, Css.Tokens.Typography.TitleSmall.Weight, Css.Tokens.Typography.TitleSmall.Size, Css.Tokens.Typography.TitleSmall.Height),
            TypographyScale.BodyLarge => (Css.Tokens.Typography.BodyLarge.Font, Css.Tokens.Typography.BodyLarge.Weight, Css.Tokens.Typography.BodyLarge.Size, Css.Tokens.Typography.BodyLarge.Height),
            TypographyScale.BodyMedium => (Css.Tokens.Typography.BodyMedium.Font, Css.Tokens.Typography.BodyMedium.Weight, Css.Tokens.Typography.BodyMedium.Size, Css.Tokens.Typography.BodyMedium.Height),
            TypographyScale.BodySmall => (Css.Tokens.Typography.BodySmall.Font, Css.Tokens.Typography.BodySmall.Weight, Css.Tokens.Typography.BodySmall.Size, Css.Tokens.Typography.BodySmall.Height),
            TypographyScale.LabelLarge => (Css.Tokens.Typography.LabelLarge.Font, Css.Tokens.Typography.LabelLarge.Weight, Css.Tokens.Typography.LabelLarge.Size, Css.Tokens.Typography.LabelLarge.Height),
            TypographyScale.LabelMedium => (Css.Tokens.Typography.LabelMedium.Font, Css.Tokens.Typography.LabelMedium.Weight, Css.Tokens.Typography.LabelMedium.Size, Css.Tokens.Typography.LabelMedium.Height),
            TypographyScale.LabelSmall => (Css.Tokens.Typography.LabelSmall.Font, Css.Tokens.Typography.LabelSmall.Weight, Css.Tokens.Typography.LabelSmall.Size, Css.Tokens.Typography.LabelSmall.Height),
            _ => (Css.Tokens.Typography.BodyMedium.Font, Css.Tokens.Typography.BodyMedium.Weight, Css.Tokens.Typography.BodyMedium.Size, Css.Tokens.Typography.BodyMedium.Height),
        };

    /// <summary>
    /// The shared <c>flare-text--*</c> utility class for a type scale, or <c>null</c> when the scale is
    /// unset. Every component that lets a caller override its label typography reads this, so the mapping
    /// lives once: it was copied out three times before (FlareText, FlareTypography's own callers and
    /// FlareButton), and a fourth copy in FlareFileUploadButton is what prompted pulling it together.
    /// </summary>
    public static string? CssClass(TypographyScale? scale) => scale switch
    {
        TypographyScale.DisplayLarge => Css.Classes.Text.DisplayLarge,
        TypographyScale.DisplayMedium => Css.Classes.Text.DisplayMedium,
        TypographyScale.DisplaySmall => Css.Classes.Text.DisplaySmall,
        TypographyScale.HeadlineLarge => Css.Classes.Text.HeadlineLarge,
        TypographyScale.HeadlineMedium => Css.Classes.Text.HeadlineMedium,
        TypographyScale.HeadlineSmall => Css.Classes.Text.HeadlineSmall,
        TypographyScale.TitleLarge => Css.Classes.Text.TitleLarge,
        TypographyScale.TitleMedium => Css.Classes.Text.TitleMedium,
        TypographyScale.TitleSmall => Css.Classes.Text.TitleSmall,
        TypographyScale.BodyLarge => Css.Classes.Text.BodyLarge,
        TypographyScale.BodyMedium => Css.Classes.Text.BodyMedium,
        TypographyScale.BodySmall => Css.Classes.Text.BodySmall,
        TypographyScale.LabelLarge => Css.Classes.Text.LabelLarge,
        TypographyScale.LabelMedium => Css.Classes.Text.LabelMedium,
        TypographyScale.LabelSmall => Css.Classes.Text.LabelSmall,
        _ => null,
    };

    /// <summary>Maps a type scale to its kebab-case token slug (e.g. <c>BodySmall</c> -> <c>body-small</c>).</summary>
    public static string Slug(TypographyScale scale) => scale switch
    {
        TypographyScale.DisplayLarge => "display-large",
        TypographyScale.DisplayMedium => "display-medium",
        TypographyScale.DisplaySmall => "display-small",
        TypographyScale.HeadlineLarge => "headline-large",
        TypographyScale.HeadlineMedium => "headline-medium",
        TypographyScale.HeadlineSmall => "headline-small",
        TypographyScale.TitleLarge => "title-large",
        TypographyScale.TitleMedium => "title-medium",
        TypographyScale.TitleSmall => "title-small",
        TypographyScale.BodyLarge => "body-large",
        TypographyScale.BodyMedium => "body-medium",
        TypographyScale.BodySmall => "body-small",
        TypographyScale.LabelLarge => "label-large",
        TypographyScale.LabelMedium => "label-medium",
        TypographyScale.LabelSmall => "label-small",
        _ => "body-medium",
    };
}
