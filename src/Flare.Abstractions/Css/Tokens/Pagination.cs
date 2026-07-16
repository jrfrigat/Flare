namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlarePagination</c>.</summary>
public static class Pagination
{
    /// <summary>CSS custom-property name for the page-button square size token.</summary>
    /// <summary>Per-size control-size tokens (button min-width/height). The theme emits all five and the
    /// component's size class reads the matching one, so the ramp lives in the theme rather than in the
    /// component CSS.</summary>
    public static class Size
    {
        /// <summary>CSS custom-property name for the xs control size token.</summary>
        public const string Xs = "--flare-pagination-size-xs";
        /// <summary>CSS custom-property name for the sm control size token.</summary>
        public const string Sm = "--flare-pagination-size-sm";
        /// <summary>CSS custom-property name for the md control size token.</summary>
        public const string Md = "--flare-pagination-size-md";
        /// <summary>CSS custom-property name for the lg control size token.</summary>
        public const string Lg = "--flare-pagination-size-lg";
        /// <summary>CSS custom-property name for the xl control size token.</summary>
        public const string Xl = "--flare-pagination-size-xl";
    }
    /// <summary>CSS custom-property name for the page-button corner radius token.</summary>
    public const string Radius = "--flare-pagination-radius";
    /// <summary>CSS custom-property name for the page-button border color token.</summary>
    public const string BorderColor = "--flare-pagination-border-color";
    /// <summary>CSS custom-property name for the default active-page color token.</summary>
    public const string ActiveColor = "--flare-pagination-active-color";
}
