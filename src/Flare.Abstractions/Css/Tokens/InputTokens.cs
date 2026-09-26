namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for input field.</summary>
public static class InputField
{
    // Font, text/caret color, error border/color and label/helper styling are gone - the field frame and the
    // shared typescale/color scales own them. Only the control-well geometry and per-variant/-state borders
    // remain (plus the CSS-computed keyboard focus ring).
    /// <summary>CSS custom-property name for the filled-variant background.</summary>
    public const string FilledBg = "--flare-input-bg";
    /// <summary>CSS custom-property name for the control-well border COLOUR (all four sides). The border
    /// width is reserved structurally in the CSS on every variant, so the theme sets only the colour;
    /// <c>transparent</c> makes a side invisible (the filled variant's top/left/right).</summary>
    public const string BorderColor = "--flare-input-border-color";
    /// <summary>CSS custom-property name for the control-well radius (rounded-top for filled).</summary>
    public const string OutlinedRadius = "--flare-input-radius";
    /// <summary>CSS custom-property name for the resting bottom-border COLOUR (the filled variant's active
    /// indicator).</summary>
    public const string BorderBottomColor = "--flare-input-border-bottom-color";
    /// <summary>CSS custom-property name for the hover bottom-border COLOUR.</summary>
    public const string HoverBorderBottomColor = "--flare-input-hover-border-bottom-color";
    /// <summary>CSS custom-property name for the bottom-border COLOUR while the field has focus.</summary>
    public const string FocusBorderBottomColor = "--flare-input-focus-border-bottom-color";
    /// <summary>CSS custom-property name for the filled-variant hover state-layer.</summary>
    public const string HoverStateLayer = "--flare-input-hover-state-layer";
    // Inset around the control's content. The block half no longer sets the field height - the Height
    // ramp below does - so these five now only decide how the content sits inside that height. Every step
    // carries its size in the name, medium included: an unsuffixed "the padding" with four suffixed
    // siblings reads as a base the others modify, which is not what it is. They are per-size tokens rather
    // than lengths in the stylesheet because a ramp half-owned by core cannot stay ordered around a middle
    // step the theme sets.
    /// <summary>CSS custom-property name for the control padding at the extra-small size.</summary>
    public const string PaddingXs = "--flare-input-padding-xs";
    /// <summary>CSS custom-property name for the control padding at the small size.</summary>
    public const string PaddingSm = "--flare-input-padding-sm";
    /// <summary>CSS custom-property name for the control padding at the default (medium) size.</summary>
    public const string PaddingMd = "--flare-input-padding-md";
    /// <summary>CSS custom-property name for the control padding at the large size.</summary>
    public const string PaddingLg = "--flare-input-padding-lg";
    /// <summary>CSS custom-property name for the control padding at the extra-large size.</summary>
    public const string PaddingXl = "--flare-input-padding-xl";
    // The well's own height, and the reason the family lines up: a control's height is this token and
    // not whatever the well happens to hold. Block padding centres the content inside it instead of
    // defining it, so a trailing chevron, a clear button, a picker toggle or a larger type step cannot
    // move one control out of line with the text field beside it. Single-line wells take it as an exact
    // height; the wells that legitimately grow (TextArea, TagField) take it as a floor.
    /// <summary>CSS custom-property name for the field-well height at the extra-small size.</summary>
    public const string HeightXs = "--flare-input-height-xs";
    /// <summary>CSS custom-property name for the field-well height at the small size.</summary>
    public const string HeightSm = "--flare-input-height-sm";
    /// <summary>CSS custom-property name for the field-well height at the default (medium) size.</summary>
    public const string HeightMd = "--flare-input-height-md";
    /// <summary>CSS custom-property name for the field-well height at the large size.</summary>
    public const string HeightLg = "--flare-input-height-lg";
    /// <summary>CSS custom-property name for the field-well height at the extra-large size.</summary>
    public const string HeightXl = "--flare-input-height-xl";
    /// <summary>CSS custom-property name for the leading/trailing icon size.</summary>
    public const string IconSize = "--flare-input-icon-size";
    /// <summary>CSS custom-property name for the placeholder color.</summary>
    public const string PlaceholderColor = "--flare-input-placeholder-color";
    /// <summary>CSS custom-property name for the disabled background.</summary>
    public const string DisabledBg = "--flare-input-disabled-bg";
    /// <summary>CSS custom-property name for the disabled border/indicator color.</summary>
    public const string DisabledIndicator = "--flare-input-disabled-indicator";
    /// <summary>CSS custom-property name for the errored-field hover bottom-border color.</summary>
    public const string ErrorHoverIndicator = "--flare-input-error-hover-indicator";
    /// <summary>CSS custom-property name for the focus indicator box-shadow (a ring or an inset bar).</summary>
    public const string FocusRing = "--flare-input-focus-ring";
    /// <summary>CSS custom-property name for the focus indicator outline (a real CSS <c>outline</c>).</summary>
    public const string FocusOutline = "--flare-input-focus-outline";
    /// <summary>CSS custom-property name for the focus outline offset.</summary>
    public const string FocusOutlineOffset = "--flare-input-focus-outline-offset";
    /// <summary>CSS custom-property name for the disabled-field content opacity token.</summary>
    public const string DisabledOpacity = "--flare-input-disabled-opacity";
    /// <summary>CSS custom-property name for the border COLOUR (all four sides) while the field has focus.</summary>
    public const string FocusBorderColor = "--flare-input-focus-border-color";
    /// <summary>CSS custom-property name for the border COLOUR (all four sides) of an errored field.</summary>
    public const string ErrorBorderColor = "--flare-input-error-border-color";
    /// <summary>CSS custom-property name for the bottom-border COLOUR of an errored field.</summary>
    public const string ErrorBorderBottomColor = "--flare-input-error-border-bottom-color";
    /// <summary>CSS custom-property name for the focus indicator box-shadow of an errored field.</summary>
    public const string ErrorFocusRing = "--flare-input-error-focus-ring";

    /// <summary>CSS variable names of the explicit filled variant (<c>Variant="Filled"</c>). The variant
    /// class points the field's own variables at these.</summary>
    public static class Filled
    {
        /// <summary>Well background.</summary>
        public const string Bg = "--flare-input-filled-bg";
        /// <summary>Border colour on all four sides.</summary>
        public const string BorderColor = "--flare-input-filled-border-color";
        /// <summary>Bottom-border colour (the active indicator).</summary>
        public const string BorderBottomColor = "--flare-input-filled-border-bottom-color";
        /// <summary>Well corner radius.</summary>
        public const string Radius = "--flare-input-filled-radius";
        /// <summary>Focus indicator box-shadow.</summary>
        public const string FocusRing = "--flare-input-filled-focus-ring";
        /// <summary>Border colour while focused.</summary>
        public const string FocusBorderColor = "--flare-input-filled-focus-border-color";
        /// <summary>Bottom-border colour while focused.</summary>
        public const string FocusBorderBottomColor = "--flare-input-filled-focus-border-bottom-color";
        /// <summary>Border colour of an errored field.</summary>
        public const string ErrorBorderColor = "--flare-input-filled-error-border-color";
        /// <summary>Bottom-border colour of an errored field.</summary>
        public const string ErrorBorderBottomColor = "--flare-input-filled-error-border-bottom-color";
        /// <summary>Focus indicator box-shadow of an errored field.</summary>
        public const string ErrorFocusRing = "--flare-input-filled-error-focus-ring";
    }

    /// <summary>CSS variable names of the explicit outlined variant (<c>Variant="Outlined"</c>). The variant
    /// class points the field's own variables at these.</summary>
    public static class Outlined
    {
        /// <summary>Well background.</summary>
        public const string Bg = "--flare-input-outlined-bg";
        /// <summary>Border colour on all four sides.</summary>
        public const string BorderColor = "--flare-input-outlined-border-color";
        /// <summary>Bottom-border colour.</summary>
        public const string BorderBottomColor = "--flare-input-outlined-border-bottom-color";
        /// <summary>Well corner radius.</summary>
        public const string Radius = "--flare-input-outlined-radius";
        /// <summary>Focus indicator box-shadow.</summary>
        public const string FocusRing = "--flare-input-outlined-focus-ring";
        /// <summary>Border colour while focused.</summary>
        public const string FocusBorderColor = "--flare-input-outlined-focus-border-color";
        /// <summary>Bottom-border colour while focused.</summary>
        public const string FocusBorderBottomColor = "--flare-input-outlined-focus-border-bottom-color";
        /// <summary>Border colour of an errored field.</summary>
        public const string ErrorBorderColor = "--flare-input-outlined-error-border-color";
        /// <summary>Bottom-border colour of an errored field.</summary>
        public const string ErrorBorderBottomColor = "--flare-input-outlined-error-border-bottom-color";
        /// <summary>Focus indicator box-shadow of an errored field.</summary>
        public const string ErrorFocusRing = "--flare-input-outlined-error-focus-ring";
    }
}

/// <summary>CSS variable tokens for the dialog panel.</summary>
public static class DialogPanel
{
    // Dialog scrim/padding/title/content font+color/motion and the per-size widths reuse the shared
    // color/spacing/typescale tokens (or hardcoded size classes) directly in dialog.css. The panel surface,
    // text and shadow are the dialog's own, next to its geometry knobs.
    /// <summary>CSS custom-property name for the dialog corner radius.</summary>
    public const string Radius = "--flare-dialog-radius";
    /// <summary>CSS custom-property name for the dialog header/close icon size.</summary>
    public const string IconSize = "--flare-dialog-icon-size";
    /// <summary>CSS custom-property name for the dialog panel background.</summary>
    public const string Bg = "--flare-dialog-bg";
    /// <summary>CSS custom-property name for the dialog panel text color.</summary>
    public const string Color = "--flare-dialog-color";
    /// <summary>CSS custom-property name for the dialog panel shadow.</summary>
    public const string Shadow = "--flare-dialog-shadow";
}

/// <summary>CSS variable tokens for the navigation drawer.</summary>
public static class DrawerPanel
{
    // Drawer surface/elevation/radius/scrim/motion/padding/title reuse the shared color/elevation/shape/
    // motion/spacing/typescale tokens directly (read in drawer.css). The responsive breakpoint widths were
    // never wired to CSS/JS. Only the two drawer-specific widths remain.
    /// <summary>CSS custom-property name for the open drawer width.</summary>
    public const string Width = "--flare-drawer-width";
    /// <summary>CSS custom-property name for the mini (rail) drawer width.</summary>
    public const string MiniWidth = "--flare-drawer-mini-width";
    /// <summary>CSS custom-property name for the edge an always-visible drawer (permanent or mini) draws
    /// against the content, as a <c>border</c> shorthand.</summary>
    public const string Border = "--flare-drawer-border";
    /// <summary>CSS custom-property name for the divider between a side panel's fixed regions and its
    /// scrolling body, as a <c>border</c> shorthand. Shared with the nav menu's header and footer: the
    /// line under a panel header is one decision wherever the panel comes from.</summary>
    public const string SectionBorder = "--flare-drawer-section-border";
}

/// <summary>CSS variable tokens for the snackbar.</summary>
public static class SnackbarPanel
{
    // Snackbar fonts, gaps, severity surfaces and motion reuse the shared color/typescale/spacing/motion
    // tokens directly in snackbar.css. The default surface, text, action color, shadow and geometry are
    // the snackbar's own.
    /// <summary>CSS custom-property name for the snackbar corner radius.</summary>
    public const string Radius = "--flare-snackbar-radius";
    /// <summary>CSS custom-property name for the single-line minimum height.</summary>
    public const string MinHeight = "--flare-snackbar-min-height";
    /// <summary>CSS custom-property name for the vertical (block) padding.</summary>
    public const string PaddingBlock = "--flare-snackbar-padding-block";
    /// <summary>CSS custom-property name for the provider inset from the viewport edge.</summary>
    public const string ProviderInset = "--flare-snackbar-provider-inset";
    /// <summary>CSS custom-property name for the dismiss-button opacity.</summary>
    public const string CloseOpacity = "--flare-snackbar-close-opacity";
    /// <summary>CSS custom-property name for the snackbar minimum width token.</summary>
    public const string MinWidth = "--flare-snackbar-min-width";
    /// <summary>CSS custom-property name for the snackbar maximum width token.</summary>
    public const string MaxWidth = "--flare-snackbar-max-width";
    /// <summary>CSS custom-property name for the horizontal (inline) padding.</summary>
    public const string PaddingInline = "--flare-snackbar-padding-inline";
    /// <summary>CSS custom-property name for the default snackbar background.</summary>
    public const string Bg = "--flare-snackbar-bg";
    /// <summary>CSS custom-property name for the default snackbar text color.</summary>
    public const string Color = "--flare-snackbar-color";
    /// <summary>CSS custom-property name for the default snackbar action label color.</summary>
    public const string ActionColor = "--flare-snackbar-action-color";
    /// <summary>CSS custom-property name for the snackbar shadow.</summary>
    public const string Shadow = "--flare-snackbar-shadow";
}
