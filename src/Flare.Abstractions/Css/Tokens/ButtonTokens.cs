namespace Flare.Css.Tokens;



/// <summary>CSS variable tokens for button.</summary>
public static class Button
{
    /// <summary>Per-corner button radii, addressable by size and side.</summary>
    public static class Radius
    {
        /// <summary>CSS custom-property name for the xs top left token.</summary>
        public const string XsTopLeft = "--flare-btn-radius-xs-top-left";
        /// <summary>CSS custom-property name for the xs top right token.</summary>
        public const string XsTopRight = "--flare-btn-radius-xs-top-right";
        /// <summary>CSS custom-property name for the xs bottom right token.</summary>
        public const string XsBottomRight = "--flare-btn-radius-xs-bottom-right";
        /// <summary>CSS custom-property name for the xs bottom left token.</summary>
        public const string XsBottomLeft = "--flare-btn-radius-xs-bottom-left";

        /// <summary>CSS custom-property name for the sm top left token.</summary>
        public const string SmTopLeft = "--flare-btn-radius-sm-top-left";
        /// <summary>CSS custom-property name for the sm top right token.</summary>
        public const string SmTopRight = "--flare-btn-radius-sm-top-right";
        /// <summary>CSS custom-property name for the sm bottom right token.</summary>
        public const string SmBottomRight = "--flare-btn-radius-sm-bottom-right";
        /// <summary>CSS custom-property name for the sm bottom left token.</summary>
        public const string SmBottomLeft = "--flare-btn-radius-sm-bottom-left";

        /// <summary>CSS custom-property name for the md top left token.</summary>
        public const string MdTopLeft = "--flare-btn-radius-md-top-left";
        /// <summary>CSS custom-property name for the md top right token.</summary>
        public const string MdTopRight = "--flare-btn-radius-md-top-right";
        /// <summary>CSS custom-property name for the md bottom right token.</summary>
        public const string MdBottomRight = "--flare-btn-radius-md-bottom-right";
        /// <summary>CSS custom-property name for the md bottom left token.</summary>
        public const string MdBottomLeft = "--flare-btn-radius-md-bottom-left";

        /// <summary>CSS custom-property name for the lg top left token.</summary>
        public const string LgTopLeft = "--flare-btn-radius-lg-top-left";
        /// <summary>CSS custom-property name for the lg top right token.</summary>
        public const string LgTopRight = "--flare-btn-radius-lg-top-right";
        /// <summary>CSS custom-property name for the lg bottom right token.</summary>
        public const string LgBottomRight = "--flare-btn-radius-lg-bottom-right";
        /// <summary>CSS custom-property name for the lg bottom left token.</summary>
        public const string LgBottomLeft = "--flare-btn-radius-lg-bottom-left";

        /// <summary>CSS custom-property name for the xl top left token.</summary>
        public const string XlTopLeft = "--flare-btn-radius-xl-top-left";
        /// <summary>CSS custom-property name for the xl top right token.</summary>
        public const string XlTopRight = "--flare-btn-radius-xl-top-right";
        /// <summary>CSS custom-property name for the xl bottom right token.</summary>
        public const string XlBottomRight = "--flare-btn-radius-xl-bottom-right";
        /// <summary>CSS custom-property name for the xl bottom left token.</summary>
        public const string XlBottomLeft = "--flare-btn-radius-xl-bottom-left";
    }

    /// <summary>Gap between the icon and label inside a button, per size.</summary>
    public static class Gap
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-gap-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-gap-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-gap-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-gap-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-gap-xl";
    }

    /// <summary>Button container heights, per size.</summary>
    public static class Height
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-height-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-height-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-height-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-height-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-height-xl";
    }

    /// <summary>CSS variable tokens for padding inline.</summary>
    public static class PaddingInline
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-padding-inline-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-padding-inline-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-padding-inline-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-padding-inline-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-padding-inline-xl";
    }

    /// <summary>Button icon size, per size.</summary>
    public static class IconSize
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-icon-size-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-icon-size-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-icon-size-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-icon-size-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-icon-size-xl";
    }

    /// <summary>Prefix for the label typography CSS variables: --flare-btn-label-{size}-{font|weight|size|height|spacing}.</summary>
    public const string LabelPrefix = "--flare-btn-label";

    // Focus ring and shadow behavior
    /// <summary>CSS custom-property name for the focus outline token.</summary>
    public const string FocusOutline = "--flare-btn-focus-outline";
    /// <summary>CSS custom-property name for the focus outline offset token.</summary>
    public const string FocusOutlineOffset = "--flare-btn-focus-outline-offset";
    /// <summary>CSS custom-property name for the focus shadow token.</summary>
    public const string FocusShadow = "--flare-btn-focus-shadow";
    /// <summary>CSS custom-property name for the filled hover shadow token.</summary>
    public const string FilledHoverShadow = "--flare-btn-filled-hover-shadow";
}
