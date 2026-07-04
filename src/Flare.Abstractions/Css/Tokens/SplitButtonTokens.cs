namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for split button.</summary>
public static class SplitButton
{
    private const string FlareSplit = $"{Vars.Flare}-split-btn";

    /// <summary>CSS custom-property name for the gap token.</summary>
    public const string Gap = $"{FlareSplit}-gap";
    /// <summary>CSS custom-property name for the trigger width token.</summary>
    public const string TriggerWidth = $"{FlareSplit}-trigger-width";

    /// <summary>Per-corner radii for the main (left) split button across all 5 sizes.</summary>
    public static class MainRadius
    {
        private const string Prefix = $"{FlareSplit}-main-radius";

        /// <summary>CSS custom-property name for the xs top left token.</summary>
        public const string XsTopLeft = $"{Prefix}-{Size.Xs}-{Side.TL}";
        /// <summary>CSS custom-property name for the xs top right token.</summary>
        public const string XsTopRight = $"{Prefix}-{Size.Xs}-{Side.TR}";
        /// <summary>CSS custom-property name for the xs bottom right token.</summary>
        public const string XsBottomRight = $"{Prefix}-{Size.Xs}-{Side.BR}";
        /// <summary>CSS custom-property name for the xs bottom left token.</summary>
        public const string XsBottomLeft = $"{Prefix}-{Size.Xs}-{Side.BL}";

        /// <summary>CSS custom-property name for the sm top left token.</summary>
        public const string SmTopLeft = $"{Prefix}-{Size.Sm}-{Side.TL}";
        /// <summary>CSS custom-property name for the sm top right token.</summary>
        public const string SmTopRight = $"{Prefix}-{Size.Sm}-{Side.TR}";
        /// <summary>CSS custom-property name for the sm bottom right token.</summary>
        public const string SmBottomRight = $"{Prefix}-{Size.Sm}-{Side.BR}";
        /// <summary>CSS custom-property name for the sm bottom left token.</summary>
        public const string SmBottomLeft = $"{Prefix}-{Size.Sm}-{Side.BL}";

        /// <summary>CSS custom-property name for the md top left token.</summary>
        public const string MdTopLeft = $"{Prefix}-{Size.Md}-{Side.TL}";
        /// <summary>CSS custom-property name for the md top right token.</summary>
        public const string MdTopRight = $"{Prefix}-{Size.Md}-{Side.TR}";
        /// <summary>CSS custom-property name for the md bottom right token.</summary>
        public const string MdBottomRight = $"{Prefix}-{Size.Md}-{Side.BR}";
        /// <summary>CSS custom-property name for the md bottom left token.</summary>
        public const string MdBottomLeft = $"{Prefix}-{Size.Md}-{Side.BL}";

        /// <summary>CSS custom-property name for the lg top left token.</summary>
        public const string LgTopLeft = $"{Prefix}-{Size.Lg}-{Side.TL}";
        /// <summary>CSS custom-property name for the lg top right token.</summary>
        public const string LgTopRight = $"{Prefix}-{Size.Lg}-{Side.TR}";
        /// <summary>CSS custom-property name for the lg bottom right token.</summary>
        public const string LgBottomRight = $"{Prefix}-{Size.Lg}-{Side.BR}";
        /// <summary>CSS custom-property name for the lg bottom left token.</summary>
        public const string LgBottomLeft = $"{Prefix}-{Size.Lg}-{Side.BL}";

        /// <summary>CSS custom-property name for the xl top left token.</summary>
        public const string XlTopLeft = $"{Prefix}-{Size.Xl}-{Side.TL}";
        /// <summary>CSS custom-property name for the xl top right token.</summary>
        public const string XlTopRight = $"{Prefix}-{Size.Xl}-{Side.TR}";
        /// <summary>CSS custom-property name for the xl bottom right token.</summary>
        public const string XlBottomRight = $"{Prefix}-{Size.Xl}-{Side.BR}";
        /// <summary>CSS custom-property name for the xl bottom left token.</summary>
        public const string XlBottomLeft = $"{Prefix}-{Size.Xl}-{Side.BL}";
    }

    /// <summary>Per-corner radii for the trigger (right) split button across all 5 sizes.</summary>
    public static class TriggerRadius
    {
        private const string Prefix = $"{FlareSplit}-trigger-radius";

        /// <summary>CSS custom-property name for the xs top left token.</summary>
        public const string XsTopLeft = $"{Prefix}-{Size.Xs}-{Side.TL}";
        /// <summary>CSS custom-property name for the xs top right token.</summary>
        public const string XsTopRight = $"{Prefix}-{Size.Xs}-{Side.TR}";
        /// <summary>CSS custom-property name for the xs bottom right token.</summary>
        public const string XsBottomRight = $"{Prefix}-{Size.Xs}-{Side.BR}";
        /// <summary>CSS custom-property name for the xs bottom left token.</summary>
        public const string XsBottomLeft = $"{Prefix}-{Size.Xs}-{Side.BL}";

        /// <summary>CSS custom-property name for the sm top left token.</summary>
        public const string SmTopLeft = $"{Prefix}-{Size.Sm}-{Side.TL}";
        /// <summary>CSS custom-property name for the sm top right token.</summary>
        public const string SmTopRight = $"{Prefix}-{Size.Sm}-{Side.TR}";
        /// <summary>CSS custom-property name for the sm bottom right token.</summary>
        public const string SmBottomRight = $"{Prefix}-{Size.Sm}-{Side.BR}";
        /// <summary>CSS custom-property name for the sm bottom left token.</summary>
        public const string SmBottomLeft = $"{Prefix}-{Size.Sm}-{Side.BL}";

        /// <summary>CSS custom-property name for the md top left token.</summary>
        public const string MdTopLeft = $"{Prefix}-{Size.Md}-{Side.TL}";
        /// <summary>CSS custom-property name for the md top right token.</summary>
        public const string MdTopRight = $"{Prefix}-{Size.Md}-{Side.TR}";
        /// <summary>CSS custom-property name for the md bottom right token.</summary>
        public const string MdBottomRight = $"{Prefix}-{Size.Md}-{Side.BR}";
        /// <summary>CSS custom-property name for the md bottom left token.</summary>
        public const string MdBottomLeft = $"{Prefix}-{Size.Md}-{Side.BL}";

        /// <summary>CSS custom-property name for the lg top left token.</summary>
        public const string LgTopLeft = $"{Prefix}-{Size.Lg}-{Side.TL}";
        /// <summary>CSS custom-property name for the lg top right token.</summary>
        public const string LgTopRight = $"{Prefix}-{Size.Lg}-{Side.TR}";
        /// <summary>CSS custom-property name for the lg bottom right token.</summary>
        public const string LgBottomRight = $"{Prefix}-{Size.Lg}-{Side.BR}";
        /// <summary>CSS custom-property name for the lg bottom left token.</summary>
        public const string LgBottomLeft = $"{Prefix}-{Size.Lg}-{Side.BL}";

        /// <summary>CSS custom-property name for the xl top left token.</summary>
        public const string XlTopLeft = $"{Prefix}-{Size.Xl}-{Side.TL}";
        /// <summary>CSS custom-property name for the xl top right token.</summary>
        public const string XlTopRight = $"{Prefix}-{Size.Xl}-{Side.TR}";
        /// <summary>CSS custom-property name for the xl bottom right token.</summary>
        public const string XlBottomRight = $"{Prefix}-{Size.Xl}-{Side.BR}";
        /// <summary>CSS custom-property name for the xl bottom left token.</summary>
        public const string XlBottomLeft = $"{Prefix}-{Size.Xl}-{Side.BL}";
    }

    /// <summary>Arrow icon size across the 5 sizes.</summary>
    public static class CaretSize
    {
        private const string Prefix = $"{FlareSplit}-caret-size";
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = $"{Prefix}-{Size.Xs}";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{Prefix}-{Size.Sm}";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{Prefix}-{Size.Md}";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{Prefix}-{Size.Lg}";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = $"{Prefix}-{Size.Xl}";
    }
}
