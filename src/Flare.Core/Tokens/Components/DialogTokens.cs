namespace Flare.Core.Tokens.Components;

/// <summary>
/// Design tokens for Dialog, ConfirmDialog, and MessageBox components.
/// These control the geometry, spacing, and appearance of modal dialogs.
/// </summary>
public sealed record DialogTokens
{
    /// <summary>Background color of the dialog surface.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-surface-container-high)";

    /// <summary>Border radius of the dialog.</summary>
    public string Radius { get; init; } = "var(--flare-shape-extra-large)";

    /// <summary>Maximum width of the dialog.</summary>
    public string MaxWidth { get; init; } = "560px";

    /// <summary>Minimum width of the dialog.</summary>
    public string MinWidth { get; init; } = "280px";

    /// <summary>Padding inside the dialog.</summary>
    public string Padding { get; init; } = "24px";

    /// <summary>Padding between header and content.</summary>
    public string HeaderPadding { get; init; } = "24px 24px 0 24px";

    /// <summary>Padding between content and actions.</summary>
    public string ActionsPadding { get; init; } = "0 24px 24px 24px";

    /// <summary>Gap between action buttons.</summary>
    public string ActionsGap { get; init; } = "8px";

    /// <summary>Background color of the scrim (overlay).</summary>
    public string ScrimColor { get; init; } = "var(--flare-color-scrim)";

    /// <summary>Opacity of the scrim.</summary>
    public string ScrimOpacity { get; init; } = "0.32";

    /// <summary>Elevation (box-shadow) of the dialog.</summary>
    public string Elevation { get; init; } = "var(--flare-elevation-3)";

    /// <summary>Color of the dialog title text.</summary>
    public string TitleColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the dialog title.</summary>
    public string TitleFontFamily { get; init; } = "var(--flare-typescale-headline-small-font)";

    /// <summary>Font size of the dialog title.</summary>
    public string TitleFontSize { get; init; } = "var(--flare-typescale-headline-small-size)";

    /// <summary>Font weight of the dialog title.</summary>
    public string TitleFontWeight { get; init; } = "var(--flare-typescale-headline-small-weight)";

    /// <summary>Color of the dialog content text.</summary>
    public string ContentColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Font family of the dialog content.</summary>
    public string ContentFontFamily { get; init; } = "var(--flare-typescale-body-medium-font)";

    /// <summary>Font size of the dialog content.</summary>
    public string ContentFontSize { get; init; } = "var(--flare-typescale-body-medium-size)";

    /// <summary>Transition duration for dialog open/close.</summary>
    public string TransitionDuration { get; init; } = "var(--flare-motion-duration-medium2)";

    /// <summary>Transition easing for dialog open/close.</summary>
    public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Width for the xs size variant.</summary>
    public string SizeXsWidth { get; init; } = "320px";

    /// <summary>Width for the sm size variant.</summary>
    public string SizeSmWidth { get; init; } = "400px";

    /// <summary>Width for the md size variant (default).</summary>
    public string SizeMdWidth { get; init; } = "560px";

    /// <summary>Width for the lg size variant.</summary>
    public string SizeLgWidth { get; init; } = "720px";

    /// <summary>Width for the xl size variant.</summary>
    public string SizeXlWidth { get; init; } = "880px";

    /// <summary>Width for the full size variant.</summary>
    public string SizeFullWidth { get; init; } = "100%";
}
