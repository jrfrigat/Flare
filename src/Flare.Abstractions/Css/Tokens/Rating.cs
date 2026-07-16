namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareRating</c>.</summary>
public static class Rating
{
    /// <summary>CSS custom-property name for the star glyph size token.</summary>
    /// <summary>Per-size star-glyph size tokens. The theme emits all five and the component's size class
    /// reads the matching one, so the ramp lives in the theme rather than in the component CSS.</summary>
    public static class Size
    {
        /// <summary>CSS custom-property name for the xs star size token.</summary>
        public const string Xs = "--flare-rating-size-xs";
        /// <summary>CSS custom-property name for the sm star size token.</summary>
        public const string Sm = "--flare-rating-size-sm";
        /// <summary>CSS custom-property name for the md star size token.</summary>
        public const string Md = "--flare-rating-size-md";
        /// <summary>CSS custom-property name for the lg star size token.</summary>
        public const string Lg = "--flare-rating-size-lg";
        /// <summary>CSS custom-property name for the xl star size token.</summary>
        public const string Xl = "--flare-rating-size-xl";
    }
    /// <summary>CSS custom-property name for the unfilled (empty) star color token.</summary>
    public const string EmptyColor = "--flare-rating-empty-color";
    /// <summary>CSS custom-property name for the default filled-star color token.</summary>
    public const string FilledColor = "--flare-rating-filled-color";
    /// <summary>CSS custom-property name for the hover scale token.</summary>
    public const string HoverScale = "--flare-rating-hover-scale";
}
