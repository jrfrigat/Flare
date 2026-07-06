namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for input field.</summary>
public static class InputField
{
    /// <summary>CSS custom-property name for the filled bg token.</summary>
    public const string FilledBg = "--flare-input-bg";
    /// <summary>CSS custom-property name for the outlined border token.</summary>
    public const string OutlinedBorder = "--flare-input-border";
    /// <summary>CSS custom-property name for the outlined radius token.</summary>
    public const string OutlinedRadius = "--flare-input-radius";
    /// <summary>CSS custom-property name for the filled border bottom token.</summary>
    public const string FilledBorderBottom = "--flare-input-border-bottom";
    /// <summary>CSS custom-property name for the filled radius token.</summary>
    public const string FilledRadius = "--flare-input-filled-radius";
    /// <summary>CSS custom-property name for the focus border token.</summary>
    public const string FocusBorder = "--flare-input-focus-border";
    /// <summary>CSS custom-property name for the focus border bottom token.</summary>
    public const string FocusBorderBottom = "--flare-input-focus-border-bottom";
    /// <summary>CSS custom-property name for the filled hover bottom-border token.</summary>
    public const string HoverBorderBottom = "--flare-input-hover-border-bottom";
    /// <summary>CSS custom-property name for the filled hover state-layer token.</summary>
    public const string HoverStateLayer = "--flare-input-hover-state-layer";
    /// <summary>CSS custom-property name for the padding token.</summary>
    public const string Padding = "--flare-input-padding";
    /// <summary>CSS custom-property name for the font family token.</summary>
    public const string FontFamily = "--flare-input-font-family";
    /// <summary>CSS custom-property name for the font size token.</summary>
    public const string FontSize = "--flare-input-font-size";
    /// <summary>CSS custom-property name for the text color token.</summary>
    public const string TextColor = "--flare-input-text-color";
    /// <summary>CSS custom-property name for the placeholder color token.</summary>
    public const string PlaceholderColor = "--flare-input-placeholder-color";
    /// <summary>CSS custom-property name for the caret color token.</summary>
    public const string CaretColor = "--flare-input-caret-color";
    /// <summary>CSS custom-property name for the error border token.</summary>
    public const string ErrorBorder = "--flare-input-error-border";
    /// <summary>CSS custom-property name for the error color token.</summary>
    public const string ErrorColor = "--flare-input-error-color";
    /// <summary>CSS custom-property name for the disabled bg token.</summary>
    public const string DisabledBg = "--flare-input-disabled-bg";
    /// <summary>CSS custom-property name for the disabled indicator token.</summary>
    public const string DisabledIndicator = "--flare-input-disabled-indicator";
    /// <summary>CSS custom-property name for the helper font size token.</summary>
    public const string HelperFontSize = "--flare-input-helper-font-size";
    /// <summary>CSS custom-property name for the helper color token.</summary>
    public const string HelperColor = "--flare-input-helper-color";
    /// <summary>CSS custom-property name for the label font family token.</summary>
    public const string LabelFontFamily = "--flare-input-label-font-family";
    /// <summary>CSS custom-property name for the label font size token.</summary>
    public const string LabelFontSize = "--flare-input-label-font-size";
    /// <summary>CSS custom-property name for the label font weight token.</summary>
    public const string LabelFontWeight = "--flare-input-label-font-weight";
    /// <summary>CSS custom-property name for the label color token.</summary>
    public const string LabelColor = "--flare-input-label-color";
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
