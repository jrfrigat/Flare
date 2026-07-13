namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for input field.</summary>
public static class InputField
{
    // Font, text/caret color, error border/color and label/helper styling are gone - the field frame and the
    // shared typescale/color scales own them. Only the control-well geometry and per-variant/-state borders
    // remain (plus the CSS-computed keyboard focus ring).
    /// <summary>CSS custom-property name for the filled-variant background.</summary>
    public const string FilledBg = "--flare-input-bg";
    /// <summary>CSS custom-property name for the outlined-variant border.</summary>
    public const string OutlinedBorder = "--flare-input-border";
    /// <summary>CSS custom-property name for the control-well radius (rounded-top for filled).</summary>
    public const string OutlinedRadius = "--flare-input-radius";
    /// <summary>CSS custom-property name for the filled-variant resting bottom border.</summary>
    public const string FilledBorderBottom = "--flare-input-border-bottom";
    /// <summary>CSS custom-property name for the outlined-variant focus border.</summary>
    public const string FocusBorder = "--flare-input-focus-border";
    /// <summary>CSS custom-property name for the filled-variant focus bottom border.</summary>
    public const string FocusBorderBottom = "--flare-input-focus-border-bottom";
    /// <summary>CSS custom-property name for the filled-variant hover bottom border.</summary>
    public const string HoverBorderBottom = "--flare-input-hover-border-bottom";
    /// <summary>CSS custom-property name for the filled-variant hover state-layer.</summary>
    public const string HoverStateLayer = "--flare-input-hover-state-layer";
    /// <summary>CSS custom-property name for the control padding.</summary>
    public const string Padding = "--flare-input-padding";
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
}

/// <summary>CSS variable tokens for the dialog panel.</summary>
public static class DialogPanel
{
    // Dialog surface/elevation/scrim/padding/title/content font+color/motion and the per-size widths reuse
    // the shared color/elevation/spacing/typescale tokens (or hardcoded size classes) directly in dialog.css.
    // Only the two dialog-specific geometry knobs the CSS reads remain.
    /// <summary>CSS custom-property name for the dialog corner radius.</summary>
    public const string Radius = "--flare-dialog-radius";
    /// <summary>CSS custom-property name for the dialog header/close icon size.</summary>
    public const string IconSize = "--flare-dialog-icon-size";
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
}

/// <summary>CSS variable tokens for the snackbar.</summary>
public static class SnackbarPanel
{
    // Snackbar surface/text/action colors, fonts, elevation, widths, offsets, gaps and motion reuse the
    // shared color/typescale/elevation/spacing/motion tokens directly in snackbar.css. Only the snackbar-
    // specific geometry the CSS reads remains.
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
}
