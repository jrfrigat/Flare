namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for fab.</summary>
public static class Fab
{
    private const string FlareFab = $"{Vars.Flare}-fab";

    /// <summary>Padding around the glyph, per size.</summary>
    public static class Padding
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareFab}-padding-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareFab}-padding-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareFab}-padding-lg";
    }

    /// <summary>Corner radius, per size (names match the legacy variables).</summary>
    public static class Radius
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareFab}-radius-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareFab}-radius";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareFab}-radius-lg";
    }

    /// <summary>CSS custom-property name for the gap token.</summary>
    public const string Gap = $"{FlareFab}-gap";
    /// <summary>CSS custom-property name for the shadow token.</summary>
    public const string Shadow = $"{FlareFab}-shadow";
    /// <summary>CSS custom-property name for the hover shadow token.</summary>
    public const string HoverShadow = $"{FlareFab}-hover-shadow";
    /// <summary>CSS custom-property name for the anchor offset token.</summary>
    public const string AnchorOffset = $"{FlareFab}-anchor-offset";
}
