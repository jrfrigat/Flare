namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for toggle button.</summary>
public static class ToggleButton
{
    private const string FlareToggleBtn = $"{Vars.Flare}-toggle-btn";

    /// <summary>Container height per size (with label).</summary>
    public static class Height
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareToggleBtn}-height-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareToggleBtn}-height-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareToggleBtn}-height-lg";
    }

    /// <summary>Horizontal padding per size.</summary>
    public static class Padding
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareToggleBtn}-padding-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareToggleBtn}-padding-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareToggleBtn}-padding-lg";
    }

    /// <summary>CSS custom-property name for the gap token.</summary>
    public const string Gap = $"{FlareToggleBtn}-gap";

    /// <summary>CSS custom-property name for the radius token.</summary>
    public const string Radius = $"{FlareToggleBtn}-radius";

    /// <summary>Selected-state (morph) radius per size.</summary>
    public static class RadiusSelected
    {
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = $"{FlareToggleBtn}-radius-selected-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = $"{FlareToggleBtn}-radius-selected-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = $"{FlareToggleBtn}-radius-selected-lg";
    }

    /// <summary>CSS custom-property name for the rest bg token.</summary>
    public const string RestBg = $"{FlareToggleBtn}-rest-bg";
    /// <summary>CSS custom-property name for the rest color token.</summary>
    public const string RestColor = $"{FlareToggleBtn}-rest-color";
    /// <summary>CSS custom-property name for the selected bg token.</summary>
    public const string SelectedBg = $"{FlareToggleBtn}-selected-bg";
    /// <summary>CSS custom-property name for the selected color token.</summary>
    public const string SelectedColor = $"{FlareToggleBtn}-selected-color";

    // Group (Segmented)
    private const string FlareToggleGroup = $"{Vars.Flare}-togglegroup";
    /// <summary>CSS custom-property name for the group border token.</summary>
    public const string GroupBorder = $"{FlareToggleGroup}-border";
    /// <summary>CSS custom-property name for the group radius token.</summary>
    public const string GroupRadius = $"{FlareToggleGroup}-radius";
    /// <summary>CSS custom-property name for the group radius vertical token.</summary>
    public const string GroupRadiusVertical = $"{FlareToggleGroup}-radius-vertical";
    /// <summary>CSS custom-property name for the group divider token.</summary>
    public const string GroupDivider = $"{FlareToggleGroup}-divider";
}
