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
        public const string Md = "--flare-fab-radius-md";
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

    /// <summary>Glyph size of the FAB's icon, one per size step. The icon grows with the container: a
    /// language that sizes the hero FAB up sizes its glyph up too.</summary>
    public static class IconSize
    {
        /// <summary>CSS custom-property name for the sm FAB icon size.</summary>
        public const string Sm = "--flare-fab-icon-size-sm";
        /// <summary>CSS custom-property name for the md FAB icon size.</summary>
        public const string Md = "--flare-fab-icon-size-md";
        /// <summary>CSS custom-property name for the lg FAB icon size.</summary>
        public const string Lg = "--flare-fab-icon-size-lg";
    }
}
