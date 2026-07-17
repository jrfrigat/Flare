namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for badge.</summary>
public static class Badge
{
    /// <summary>Border radius of the badge pill indicator.</summary>
    public const string Radius = "--flare-badge-radius";

    /// <summary>Offset of the indicator from the anchor corner (count/text).</summary>
    public const string Offset = "--flare-badge-offset";

    /// <summary>Offset of the indicator from the anchor corner (dot).</summary>
    public const string DotOffset = "--flare-badge-dot-offset";

    /// <summary>Per-size minimum width of the count / text indicator.</summary>
    public static class MinWidth
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-badge-min-width-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-badge-min-width-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-badge-min-width-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-badge-min-width-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-badge-min-width-xl";
    }

    /// <summary>Per-size height of the count / text indicator.</summary>
    public static class Height
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-badge-height-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-badge-height-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-badge-height-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-badge-height-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-badge-height-xl";
    }

    /// <summary>Per-size diameter of the dot variant.</summary>
    public static class DotSize
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-badge-dot-size-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-badge-dot-size-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-badge-dot-size-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-badge-dot-size-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-badge-dot-size-xl";
    }

    /// <summary>Per-size horizontal padding inside the indicator.</summary>
    public static class PaddingX
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-badge-padding-x-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-badge-padding-x-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-badge-padding-x-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-badge-padding-x-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-badge-padding-x-xl";
    }

    /// <summary>Per-size font size of the count / text inside the indicator.</summary>
    public static class LabelSize
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-badge-label-size-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-badge-label-size-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-badge-label-size-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-badge-label-size-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-badge-label-size-xl";
    }
}
