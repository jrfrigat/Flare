namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for the type scale - one nested class per step, five names each.
/// <para>
/// Spelled out rather than built from a slug. A name assembled at runtime
/// (<c>$"--flare-typescale-{scale}-font"</c>) is a name <c>Flare.CssAudit</c> cannot read, so it can
/// neither confirm that every step a component asks for exists nor notice one that does not. That is
/// the whole point of the registry, and a helper method quietly opted out of it.
/// </para>
/// </summary>
public static class Typography
{
    /// <summary>Type-scale tokens for the display large step.</summary>
    public static class DisplayLarge
    {
        /// <summary>CSS custom-property name for the font-family of the display large step.</summary>
        public const string Font = "--flare-typescale-display-large-font";
        /// <summary>CSS custom-property name for the font-weight of the display large step.</summary>
        public const string Weight = "--flare-typescale-display-large-weight";
        /// <summary>CSS custom-property name for the font-size of the display large step.</summary>
        public const string Size = "--flare-typescale-display-large-size";
        /// <summary>CSS custom-property name for the line-height of the display large step.</summary>
        public const string Height = "--flare-typescale-display-large-height";
        /// <summary>CSS custom-property name for the letter-spacing of the display large step.</summary>
        public const string Spacing = "--flare-typescale-display-large-spacing";
    }

    /// <summary>Type-scale tokens for the display medium step.</summary>
    public static class DisplayMedium
    {
        /// <summary>CSS custom-property name for the font-family of the display medium step.</summary>
        public const string Font = "--flare-typescale-display-medium-font";
        /// <summary>CSS custom-property name for the font-weight of the display medium step.</summary>
        public const string Weight = "--flare-typescale-display-medium-weight";
        /// <summary>CSS custom-property name for the font-size of the display medium step.</summary>
        public const string Size = "--flare-typescale-display-medium-size";
        /// <summary>CSS custom-property name for the line-height of the display medium step.</summary>
        public const string Height = "--flare-typescale-display-medium-height";
        /// <summary>CSS custom-property name for the letter-spacing of the display medium step.</summary>
        public const string Spacing = "--flare-typescale-display-medium-spacing";
    }

    /// <summary>Type-scale tokens for the display small step.</summary>
    public static class DisplaySmall
    {
        /// <summary>CSS custom-property name for the font-family of the display small step.</summary>
        public const string Font = "--flare-typescale-display-small-font";
        /// <summary>CSS custom-property name for the font-weight of the display small step.</summary>
        public const string Weight = "--flare-typescale-display-small-weight";
        /// <summary>CSS custom-property name for the font-size of the display small step.</summary>
        public const string Size = "--flare-typescale-display-small-size";
        /// <summary>CSS custom-property name for the line-height of the display small step.</summary>
        public const string Height = "--flare-typescale-display-small-height";
        /// <summary>CSS custom-property name for the letter-spacing of the display small step.</summary>
        public const string Spacing = "--flare-typescale-display-small-spacing";
    }

    /// <summary>Type-scale tokens for the headline large step.</summary>
    public static class HeadlineLarge
    {
        /// <summary>CSS custom-property name for the font-family of the headline large step.</summary>
        public const string Font = "--flare-typescale-headline-large-font";
        /// <summary>CSS custom-property name for the font-weight of the headline large step.</summary>
        public const string Weight = "--flare-typescale-headline-large-weight";
        /// <summary>CSS custom-property name for the font-size of the headline large step.</summary>
        public const string Size = "--flare-typescale-headline-large-size";
        /// <summary>CSS custom-property name for the line-height of the headline large step.</summary>
        public const string Height = "--flare-typescale-headline-large-height";
        /// <summary>CSS custom-property name for the letter-spacing of the headline large step.</summary>
        public const string Spacing = "--flare-typescale-headline-large-spacing";
    }

    /// <summary>Type-scale tokens for the headline medium step.</summary>
    public static class HeadlineMedium
    {
        /// <summary>CSS custom-property name for the font-family of the headline medium step.</summary>
        public const string Font = "--flare-typescale-headline-medium-font";
        /// <summary>CSS custom-property name for the font-weight of the headline medium step.</summary>
        public const string Weight = "--flare-typescale-headline-medium-weight";
        /// <summary>CSS custom-property name for the font-size of the headline medium step.</summary>
        public const string Size = "--flare-typescale-headline-medium-size";
        /// <summary>CSS custom-property name for the line-height of the headline medium step.</summary>
        public const string Height = "--flare-typescale-headline-medium-height";
        /// <summary>CSS custom-property name for the letter-spacing of the headline medium step.</summary>
        public const string Spacing = "--flare-typescale-headline-medium-spacing";
    }

    /// <summary>Type-scale tokens for the headline small step.</summary>
    public static class HeadlineSmall
    {
        /// <summary>CSS custom-property name for the font-family of the headline small step.</summary>
        public const string Font = "--flare-typescale-headline-small-font";
        /// <summary>CSS custom-property name for the font-weight of the headline small step.</summary>
        public const string Weight = "--flare-typescale-headline-small-weight";
        /// <summary>CSS custom-property name for the font-size of the headline small step.</summary>
        public const string Size = "--flare-typescale-headline-small-size";
        /// <summary>CSS custom-property name for the line-height of the headline small step.</summary>
        public const string Height = "--flare-typescale-headline-small-height";
        /// <summary>CSS custom-property name for the letter-spacing of the headline small step.</summary>
        public const string Spacing = "--flare-typescale-headline-small-spacing";
    }

    /// <summary>Type-scale tokens for the title large step.</summary>
    public static class TitleLarge
    {
        /// <summary>CSS custom-property name for the font-family of the title large step.</summary>
        public const string Font = "--flare-typescale-title-large-font";
        /// <summary>CSS custom-property name for the font-weight of the title large step.</summary>
        public const string Weight = "--flare-typescale-title-large-weight";
        /// <summary>CSS custom-property name for the font-size of the title large step.</summary>
        public const string Size = "--flare-typescale-title-large-size";
        /// <summary>CSS custom-property name for the line-height of the title large step.</summary>
        public const string Height = "--flare-typescale-title-large-height";
        /// <summary>CSS custom-property name for the letter-spacing of the title large step.</summary>
        public const string Spacing = "--flare-typescale-title-large-spacing";
    }

    /// <summary>Type-scale tokens for the title medium step.</summary>
    public static class TitleMedium
    {
        /// <summary>CSS custom-property name for the font-family of the title medium step.</summary>
        public const string Font = "--flare-typescale-title-medium-font";
        /// <summary>CSS custom-property name for the font-weight of the title medium step.</summary>
        public const string Weight = "--flare-typescale-title-medium-weight";
        /// <summary>CSS custom-property name for the font-size of the title medium step.</summary>
        public const string Size = "--flare-typescale-title-medium-size";
        /// <summary>CSS custom-property name for the line-height of the title medium step.</summary>
        public const string Height = "--flare-typescale-title-medium-height";
        /// <summary>CSS custom-property name for the letter-spacing of the title medium step.</summary>
        public const string Spacing = "--flare-typescale-title-medium-spacing";
    }

    /// <summary>Type-scale tokens for the title small step.</summary>
    public static class TitleSmall
    {
        /// <summary>CSS custom-property name for the font-family of the title small step.</summary>
        public const string Font = "--flare-typescale-title-small-font";
        /// <summary>CSS custom-property name for the font-weight of the title small step.</summary>
        public const string Weight = "--flare-typescale-title-small-weight";
        /// <summary>CSS custom-property name for the font-size of the title small step.</summary>
        public const string Size = "--flare-typescale-title-small-size";
        /// <summary>CSS custom-property name for the line-height of the title small step.</summary>
        public const string Height = "--flare-typescale-title-small-height";
        /// <summary>CSS custom-property name for the letter-spacing of the title small step.</summary>
        public const string Spacing = "--flare-typescale-title-small-spacing";
    }

    /// <summary>Type-scale tokens for the body large step.</summary>
    public static class BodyLarge
    {
        /// <summary>CSS custom-property name for the font-family of the body large step.</summary>
        public const string Font = "--flare-typescale-body-large-font";
        /// <summary>CSS custom-property name for the font-weight of the body large step.</summary>
        public const string Weight = "--flare-typescale-body-large-weight";
        /// <summary>CSS custom-property name for the font-size of the body large step.</summary>
        public const string Size = "--flare-typescale-body-large-size";
        /// <summary>CSS custom-property name for the line-height of the body large step.</summary>
        public const string Height = "--flare-typescale-body-large-height";
        /// <summary>CSS custom-property name for the letter-spacing of the body large step.</summary>
        public const string Spacing = "--flare-typescale-body-large-spacing";
    }

    /// <summary>Type-scale tokens for the body medium step.</summary>
    public static class BodyMedium
    {
        /// <summary>CSS custom-property name for the font-family of the body medium step.</summary>
        public const string Font = "--flare-typescale-body-medium-font";
        /// <summary>CSS custom-property name for the font-weight of the body medium step.</summary>
        public const string Weight = "--flare-typescale-body-medium-weight";
        /// <summary>CSS custom-property name for the font-size of the body medium step.</summary>
        public const string Size = "--flare-typescale-body-medium-size";
        /// <summary>CSS custom-property name for the line-height of the body medium step.</summary>
        public const string Height = "--flare-typescale-body-medium-height";
        /// <summary>CSS custom-property name for the letter-spacing of the body medium step.</summary>
        public const string Spacing = "--flare-typescale-body-medium-spacing";
    }

    /// <summary>Type-scale tokens for the body small step.</summary>
    public static class BodySmall
    {
        /// <summary>CSS custom-property name for the font-family of the body small step.</summary>
        public const string Font = "--flare-typescale-body-small-font";
        /// <summary>CSS custom-property name for the font-weight of the body small step.</summary>
        public const string Weight = "--flare-typescale-body-small-weight";
        /// <summary>CSS custom-property name for the font-size of the body small step.</summary>
        public const string Size = "--flare-typescale-body-small-size";
        /// <summary>CSS custom-property name for the line-height of the body small step.</summary>
        public const string Height = "--flare-typescale-body-small-height";
        /// <summary>CSS custom-property name for the letter-spacing of the body small step.</summary>
        public const string Spacing = "--flare-typescale-body-small-spacing";
    }

    /// <summary>Type-scale tokens for the label large step.</summary>
    public static class LabelLarge
    {
        /// <summary>CSS custom-property name for the font-family of the label large step.</summary>
        public const string Font = "--flare-typescale-label-large-font";
        /// <summary>CSS custom-property name for the font-weight of the label large step.</summary>
        public const string Weight = "--flare-typescale-label-large-weight";
        /// <summary>CSS custom-property name for the font-size of the label large step.</summary>
        public const string Size = "--flare-typescale-label-large-size";
        /// <summary>CSS custom-property name for the line-height of the label large step.</summary>
        public const string Height = "--flare-typescale-label-large-height";
        /// <summary>CSS custom-property name for the letter-spacing of the label large step.</summary>
        public const string Spacing = "--flare-typescale-label-large-spacing";
    }

    /// <summary>Type-scale tokens for the label medium step.</summary>
    public static class LabelMedium
    {
        /// <summary>CSS custom-property name for the font-family of the label medium step.</summary>
        public const string Font = "--flare-typescale-label-medium-font";
        /// <summary>CSS custom-property name for the font-weight of the label medium step.</summary>
        public const string Weight = "--flare-typescale-label-medium-weight";
        /// <summary>CSS custom-property name for the font-size of the label medium step.</summary>
        public const string Size = "--flare-typescale-label-medium-size";
        /// <summary>CSS custom-property name for the line-height of the label medium step.</summary>
        public const string Height = "--flare-typescale-label-medium-height";
        /// <summary>CSS custom-property name for the letter-spacing of the label medium step.</summary>
        public const string Spacing = "--flare-typescale-label-medium-spacing";
    }

    /// <summary>Type-scale tokens for the label small step.</summary>
    public static class LabelSmall
    {
        /// <summary>CSS custom-property name for the font-family of the label small step.</summary>
        public const string Font = "--flare-typescale-label-small-font";
        /// <summary>CSS custom-property name for the font-weight of the label small step.</summary>
        public const string Weight = "--flare-typescale-label-small-weight";
        /// <summary>CSS custom-property name for the font-size of the label small step.</summary>
        public const string Size = "--flare-typescale-label-small-size";
        /// <summary>CSS custom-property name for the line-height of the label small step.</summary>
        public const string Height = "--flare-typescale-label-small-height";
        /// <summary>CSS custom-property name for the letter-spacing of the label small step.</summary>
        public const string Spacing = "--flare-typescale-label-small-spacing";
    }
}
