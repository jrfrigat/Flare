namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for fab.</summary>
public static class Fab
{
    /// <summary>Padding around the glyph, per size.</summary>
    public static class Padding
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-fab-padding-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-fab-padding-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-fab-padding-lg";
    }

    /// <summary>Corner radius, per size (names match the legacy variables).</summary>
    public static class Radius
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-fab-radius-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-fab-radius";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-fab-radius-lg";
    }

    /// <summary>CSS custom-property name for the gap token.</summary>
    public const string Gap = "--flare-fab-gap";
    /// <summary>CSS custom-property name for the shadow token.</summary>
    public const string Shadow = "--flare-fab-shadow";
    /// <summary>CSS custom-property name for the hover shadow token.</summary>
    public const string HoverShadow = "--flare-fab-hover-shadow";
    /// <summary>CSS custom-property name for the anchor offset token.</summary>
    public const string AnchorOffset = "--flare-fab-anchor-offset";
}
