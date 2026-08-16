namespace Flare.Css.Tokens;



/// <summary>CSS variable tokens for button.</summary>
public static class Button
{
    /// <summary>CSS custom-property name for the loading opacity token.</summary>
    public const string LoadingOpacity = "--flare-btn-loading-opacity";
    /// <summary>Fallback container corner radius for buttons that do not set a per-size radius.</summary>
    public const string ContainerRadius = "--flare-btn-radius";
    /// <summary>CSS custom-property name for the text padding inline token.</summary>
    public const string TextPaddingInline = "--flare-btn-text-padding-inline";
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

    /// <summary>Width of the container's border, per size. Reserved on every button whatever its variant
    /// so switching variant never shifts layout, and painted only by the ones that draw an outline.</summary>
    public static class OutlineWidth
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-outline-width-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-outline-width-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-outline-width-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-outline-width-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-outline-width-xl";
    }

    /// <summary>Corner radius a SELECTED button takes, per size. Selection is a shape change as much as a
    /// colour change, and the two directions are not the same value, which is why this is a family of its
    /// own rather than a reuse of the rest radii.</summary>
    public static class SelectedRadius
    {
        /// <summary>CSS custom-property name for the xs token.</summary>
        public const string Xs = "--flare-btn-selected-radius-xs";
        /// <summary>CSS custom-property name for the sm token.</summary>
        public const string Sm = "--flare-btn-selected-radius-sm";
        /// <summary>CSS custom-property name for the md token.</summary>
        public const string Md = "--flare-btn-selected-radius-md";
        /// <summary>CSS custom-property name for the lg token.</summary>
        public const string Lg = "--flare-btn-selected-radius-lg";
        /// <summary>CSS custom-property name for the xl token.</summary>
        public const string Xl = "--flare-btn-selected-radius-xl";
    }

    /// <summary>CSS custom-property name for the corner radius a selected button takes when its rest
    /// shape is the explicit square. One value rather than a ramp: a language that opens a square out
    /// on selection opens it to a capsule, which is arithmetic on the segment's own height.</summary>
    public const string SelectedRadiusSquare = "--flare-btn-selected-radius-square";
    /// <summary>CSS custom-property name for the selected container background token, used by any variant
    /// that does not name its own.</summary>
    public const string SelectedBg = "--flare-btn-selected-bg";
    /// <summary>CSS custom-property name for the selected foreground token.</summary>
    public const string SelectedColor = "--flare-btn-selected-color";

    /// <summary>Per-variant paint for a toggle button, which Material states as a table of its own: "the
    /// default and toggle buttons use different colors". A selected button of each variant lands somewhere
    /// the variant does not otherwise go, and one variant - filled - also differs from its own default
    /// while UNselected, which is why that pair exists here too.</summary>
    public static class Toggle
    {
        /// <summary>CSS custom-property name for the selected elevated container.</summary>
        public const string ElevatedSelectedBg = "--flare-btn-elevated-selected-bg";
        /// <summary>CSS custom-property name for the selected elevated icon and label.</summary>
        public const string ElevatedSelectedColor = "--flare-btn-elevated-selected-color";
        /// <summary>CSS custom-property name for the selected filled container.</summary>
        public const string FilledSelectedBg = "--flare-btn-filled-selected-bg";
        /// <summary>CSS custom-property name for the selected filled icon and label.</summary>
        public const string FilledSelectedColor = "--flare-btn-filled-selected-color";
        /// <summary>CSS custom-property name for the selected tonal container.</summary>
        public const string TonalSelectedBg = "--flare-btn-tonal-selected-bg";
        /// <summary>CSS custom-property name for the selected tonal icon and label.</summary>
        public const string TonalSelectedColor = "--flare-btn-tonal-selected-color";
        /// <summary>CSS custom-property name for the selected outlined container.</summary>
        public const string OutlinedSelectedBg = "--flare-btn-outlined-selected-bg";
        /// <summary>CSS custom-property name for the selected outlined icon and label.</summary>
        public const string OutlinedSelectedColor = "--flare-btn-outlined-selected-color";
        /// <summary>CSS custom-property name for the UNselected filled toggle container - the one place a
        /// toggle differs from its variant's default before anything is selected.</summary>
        public const string FilledUnselectedBg = "--flare-btn-filled-unselected-bg";
        /// <summary>CSS custom-property name for the unselected filled toggle icon and label.</summary>
        public const string FilledUnselectedColor = "--flare-btn-filled-unselected-color";
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
    /// <summary>CSS custom-property name for the disabled opacity token.</summary>
    public const string DisabledOpacity = "--flare-btn-disabled-opacity";
    /// <summary>CSS custom-property name for the disabled state layer token.</summary>
    public const string DisabledLayer = "--flare-btn-disabled-layer";
}
