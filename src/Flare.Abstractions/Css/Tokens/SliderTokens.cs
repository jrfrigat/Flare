namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for slider.</summary>
public static class Slider
{
    /// <summary>Per-size track thickness tokens. Size-dependent geometry gets one token per size (the
    /// theme emits all five on <c>:root</c> and the component's size class reads the matching one), so the
    /// ramp lives in the theme instead of being hardcoded in the component CSS.</summary>
    public static class TrackHeight
    {
        /// <summary>CSS custom-property name for the xs track height token.</summary>
        public const string Xs = "--flare-slider-track-height-xs";
        /// <summary>CSS custom-property name for the sm track height token.</summary>
        public const string Sm = "--flare-slider-track-height-sm";
        /// <summary>CSS custom-property name for the md track height token.</summary>
        public const string Md = "--flare-slider-track-height-md";
        /// <summary>CSS custom-property name for the lg track height token.</summary>
        public const string Lg = "--flare-slider-track-height-lg";
        /// <summary>CSS custom-property name for the xl track height token.</summary>
        public const string Xl = "--flare-slider-track-height-xl";
    }

    /// <summary>Per-size track corner-radius tokens.</summary>
    public static class TrackRadius
    {
        /// <summary>CSS custom-property name for the xs track radius token.</summary>
        public const string Xs = "--flare-slider-track-radius-xs";
        /// <summary>CSS custom-property name for the sm track radius token.</summary>
        public const string Sm = "--flare-slider-track-radius-sm";
        /// <summary>CSS custom-property name for the md track radius token.</summary>
        public const string Md = "--flare-slider-track-radius-md";
        /// <summary>CSS custom-property name for the lg track radius token.</summary>
        public const string Lg = "--flare-slider-track-radius-lg";
        /// <summary>CSS custom-property name for the xl track radius token.</summary>
        public const string Xl = "--flare-slider-track-radius-xl";
    }

    /// <summary>Per-size handle height tokens.</summary>
    public static class HandleHeight
    {
        /// <summary>CSS custom-property name for the xs handle height token.</summary>
        public const string Xs = "--flare-slider-handle-height-xs";
        /// <summary>CSS custom-property name for the sm handle height token.</summary>
        public const string Sm = "--flare-slider-handle-height-sm";
        /// <summary>CSS custom-property name for the md handle height token.</summary>
        public const string Md = "--flare-slider-handle-height-md";
        /// <summary>CSS custom-property name for the lg handle height token.</summary>
        public const string Lg = "--flare-slider-handle-height-lg";
        /// <summary>CSS custom-property name for the xl handle height token.</summary>
        public const string Xl = "--flare-slider-handle-height-xl";
    }

    /// <summary>Per-size size tokens for the icons flanking the track (<c>StartIcon</c>/<c>EndIcon</c>).</summary>
    public static class IconSize
    {
        /// <summary>CSS custom-property name for the xs flanking-icon size token.</summary>
        public const string Xs = "--flare-slider-icon-size-xs";
        /// <summary>CSS custom-property name for the sm flanking-icon size token.</summary>
        public const string Sm = "--flare-slider-icon-size-sm";
        /// <summary>CSS custom-property name for the md flanking-icon size token.</summary>
        public const string Md = "--flare-slider-icon-size-md";
        /// <summary>CSS custom-property name for the lg flanking-icon size token.</summary>
        public const string Lg = "--flare-slider-icon-size-lg";
        /// <summary>CSS custom-property name for the xl flanking-icon size token.</summary>
        public const string Xl = "--flare-slider-icon-size-xl";
    }

    /// <summary>CSS custom-property name for the vertical-slider length token. A consumer can override it
    /// per instance (it is read on the component root, so an inline style wins over the theme's value).</summary>
    public const string Length = "--flare-slider-length";

    /// <summary>CSS custom-property name for the gap radius token.</summary>
    public const string GapRadius = "--flare-slider-gap-radius";
    /// <summary>CSS custom-property name for the gap token.</summary>
    public const string Gap = "--flare-slider-gap";
    /// <summary>CSS custom-property name for the handle width token.</summary>
    public const string HandleWidth = "--flare-slider-handle-width";
    /// <summary>CSS custom-property name for the handle pressed width token.</summary>
    public const string HandlePressedWidth = "--flare-slider-handle-pressed-width";
    /// <summary>CSS custom-property name for the handle radius token.</summary>
    public const string HandleRadius = "--flare-slider-handle-radius";
    /// <summary>CSS custom-property name for the handle clip path token.</summary>
    public const string HandleClipPath = "--flare-slider-handle-clip";
    /// <summary>CSS custom-property name for the handle border width token.</summary>
    public const string HandleBorderWidth = "--flare-slider-handle-border-width";
    /// <summary>CSS custom-property name for the handle fill token.</summary>
    public const string HandleFill = "--flare-slider-handle-fill";
    /// <summary>CSS custom-property name for the active color token.</summary>
    public const string ActiveColor = "--flare-slider-active-color";
    /// <summary>CSS custom-property name for the inactive color token.</summary>
    public const string InactiveColor = "--flare-slider-inactive-color";
    /// <summary>CSS custom-property name for the state layer size token.</summary>
    public const string StateLayerSize = "--flare-slider-state-layer-size";
    /// <summary>CSS custom-property name for the state hover opacity token.</summary>
    public const string StateHoverOpacity = "--flare-slider-state-hover-opacity";
    /// <summary>CSS custom-property name for the state pressed opacity token.</summary>
    public const string StatePressedOpacity = "--flare-slider-state-pressed-opacity";
    /// <summary>CSS custom-property name for the stop color token.</summary>
    public const string StopColor = "--flare-slider-stop-color";
    /// <summary>CSS custom-property name for the stop color selected token.</summary>
    public const string StopColorSelected = "--flare-slider-stop-color-selected";
    /// <summary>CSS custom-property name for the stop size token.</summary>
    public const string StopSize = "--flare-slider-stop-size";
    /// <summary>CSS custom-property name for the value bg token.</summary>
    public const string ValueBg = "--flare-slider-value-bg";
    /// <summary>CSS custom-property name for the value color token.</summary>
    public const string ValueColor = "--flare-slider-value-color";
}
