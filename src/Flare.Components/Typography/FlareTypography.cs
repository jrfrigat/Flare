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
        var s = Slug(scale);
        return $"font-family:var(--flare-typescale-{s}-font);"
             + $"font-weight:var(--flare-typescale-{s}-weight);"
             + $"font-size:var(--flare-typescale-{s}-size);"
             + $"line-height:var(--flare-typescale-{s}-height)";
    }

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
