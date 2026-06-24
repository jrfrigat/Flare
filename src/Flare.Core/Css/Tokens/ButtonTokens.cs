namespace Flare.Css.Tokens;



/// <summary>CSS variable tokens for button.</summary>
public static class Button
{
    private const string FlareBtn = $"{Vars.Flare}-btn";

    /// <summary>Per-corner button radii, addressable by size and side.</summary>
    public static class Radius
    {
        private const string FlareBtnRadius = $"{FlareBtn}-radius";

        /// <summary>CSS custom-property name for the xs top left token.</summary>
        public const string XsTopLeft = $"{FlareBtnRadius}-{Size.Xs}-{Side.TL}";
        /// <summary>CSS custom-property name for the xs top right token.</summary>
        public const string XsTopRight = $"{FlareBtnRadius}-{Size.Xs}-{Side.TR}";
        /// <summary>CSS custom-property name for the xs bottom right token.</summary>
        public const string XsBottomRight = $"{FlareBtnRadius}-{Size.Xs}-{Side.BR}";
        /// <summary>CSS custom-property name for the xs bottom left token.</summary>
        public const string XsBottomLeft = $"{FlareBtnRadius}-{Size.Xs}-{Side.BL}";

        /// <summary>CSS custom-property name for the sm top left token.</summary>
        public const string SmTopLeft = $"{FlareBtnRadius}-{Size.Sm}-{Side.TL}";
        /// <summary>CSS custom-property name for the sm top right token.</summary>
        public const string SmTopRight = $"{FlareBtnRadius}-{Size.Sm}-{Side.TR}";
        /// <summary>CSS custom-property name for the sm bottom right token.</summary>
        public const string SmBottomRight = $"{FlareBtnRadius}-{Size.Sm}-{Side.BR}";
        /// <summary>CSS custom-property name for the sm bottom left token.</summary>
        public const string SmBottomLeft = $"{FlareBtnRadius}-{Size.Sm}-{Side.BL}";

        /// <summary>CSS custom-property name for the md top left token.</summary>
        public const string MdTopLeft = $"{FlareBtnRadius}-{Size.Md}-{Side.TL}";
        /// <summary>CSS custom-property name for the md top right token.</summary>
        public const string MdTopRight = $"{FlareBtnRadius}-{Size.Md}-{Side.TR}";
        /// <summary>CSS custom-property name for the md bottom right token.</summary>
        public const string MdBottomRight = $"{FlareBtnRadius}-{Size.Md}-{Side.BR}";
        /// <summary>CSS custom-property name for the md bottom left token.</summary>
        public const string MdBottomLeft = $"{FlareBtnRadius}-{Size.Md}-{Side.BL}";

        /// <summary>CSS custom-property name for the lg top left token.</summary>
        public const string LgTopLeft = $"{FlareBtnRadius}-{Size.Lg}-{Side.TL}";
        /// <summary>CSS custom-property name for the lg top right token.</summary>
        public const string LgTopRight = $"{FlareBtnRadius}-{Size.Lg}-{Side.TR}";
        /// <summary>CSS custom-property name for the lg bottom right token.</summary>
        public const string LgBottomRight = $"{FlareBtnRadius}-{Size.Lg}-{Side.BR}";
        /// <summary>CSS custom-property name for the lg bottom left token.</summary>
        public const string LgBottomLeft = $"{FlareBtnRadius}-{Size.Lg}-{Side.BL}";

        /// <summary>CSS custom-property name for the xl top left token.</summary>
        public const string XlTopLeft = $"{FlareBtnRadius}-{Size.Xl}-{Side.TL}";
        /// <summary>CSS custom-property name for the xl top right token.</summary>
        public const string XlTopRight = $"{FlareBtnRadius}-{Size.Xl}-{Side.TR}";
        /// <summary>CSS custom-property name for the xl bottom right token.</summary>
        public const string XlBottomRight = $"{FlareBtnRadius}-{Size.Xl}-{Side.BR}";
        /// <summary>CSS custom-property name for the xl bottom left token.</summary>
        public const string XlBottomLeft = $"{FlareBtnRadius}-{Size.Xl}-{Side.BL}";
    }

    /// <summary>Gap between the icon and label inside a button, per size.</summary>
    public static class Gap
    {
        private const string FlareBtnGap = $"{FlareBtn}-gap";

        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = $"{FlareBtnGap}-{Size.Xs}";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareBtnGap}-{Size.Sm}";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareBtnGap}-{Size.Md}";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareBtnGap}-{Size.Lg}";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = $"{FlareBtnGap}-{Size.Xl}";
    }

    /// <summary>Button container heights, per size.</summary>
    public static class Height
    {
        private const string FlareBtnHeight = $"{FlareBtn}-height";

        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = $"{FlareBtnHeight}-{Size.Xs}";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareBtnHeight}-{Size.Sm}";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareBtnHeight}-{Size.Md}";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareBtnHeight}-{Size.Lg}";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = $"{FlareBtnHeight}-{Size.Xl}";
    }

    /// <summary>CSS variable tokens for padding inline.</summary>
    public static class PaddingInline
    {
        private const string FlareBtnPadding = $"{FlareBtn}-padding-inline";

        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = $"{FlareBtnPadding}-{Size.Xs}";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareBtnPadding}-{Size.Sm}";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareBtnPadding}-{Size.Md}";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareBtnPadding}-{Size.Lg}";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = $"{FlareBtnPadding}-{Size.Xl}";
    }

    /// <summary>Button icon size, per size.</summary>
    public static class IconSize
    {
        private const string Prefix = $"{FlareBtn}-icon-size";
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

    /// <summary>Prefix for the label typography CSS variables: --flare-btn-label-{size}-{font|weight|size|height|spacing}.</summary>
    public const string LabelPrefix = $"{FlareBtn}-label";

    // Фокусное кольцо и поведение теней
    /// <summary>CSS custom-property name for the focus outline token.</summary>
    public const string FocusOutline = $"{FlareBtn}-focus-outline";
    /// <summary>CSS custom-property name for the focus outline offset token.</summary>
    public const string FocusOutlineOffset = $"{FlareBtn}-focus-outline-offset";
    /// <summary>CSS custom-property name for the focus shadow token.</summary>
    public const string FocusShadow = $"{FlareBtn}-focus-shadow";
    /// <summary>CSS custom-property name for the filled hover shadow token.</summary>
    public const string FilledHoverShadow = $"{FlareBtn}-filled-hover-shadow";
}
