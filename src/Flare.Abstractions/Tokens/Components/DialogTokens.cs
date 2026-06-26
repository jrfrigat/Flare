using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Dialog, ConfirmDialog, and MessageBox components.
/// These control the geometry, spacing, and appearance of modal dialogs.
/// </summary>
public sealed record DialogTokens
{
    /// <summary>Background color of the dialog surface.</summary>
    [CssVar(DialogPanel.SurfaceColor)] public string SurfaceColor { get; init; } = "var(--flare-color-surface-container-high)";

    /// <summary>Border radius of the dialog.</summary>
    [CssVar(DialogPanel.Radius)] public string Radius { get; init; } = "var(--flare-shape-extra-large)";

    /// <summary>Maximum width of the dialog.</summary>
    [CssVar(DialogPanel.MaxWidth)] public string MaxWidth { get; init; } = "560px";

    /// <summary>Minimum width of the dialog.</summary>
    [CssVar(DialogPanel.MinWidth)] public string MinWidth { get; init; } = "280px";

    /// <summary>Padding inside the dialog.</summary>
    [CssVar(DialogPanel.Padding)] public string Padding { get; init; } = "24px";

    /// <summary>Padding between header and content.</summary>
    [CssVar(DialogPanel.HeaderPadding)] public string HeaderPadding { get; init; } = "24px 24px 0 24px";

    /// <summary>Padding between content and actions.</summary>
    [CssVar(DialogPanel.ActionsPadding)] public string ActionsPadding { get; init; } = "0 24px 24px 24px";

    /// <summary>Gap between action buttons.</summary>
    [CssVar(DialogPanel.ActionsGap)] public string ActionsGap { get; init; } = "8px";

    /// <summary>Background color of the scrim (overlay).</summary>
    [CssVar(DialogPanel.ScrimColor)] public string ScrimColor { get; init; } = "var(--flare-color-scrim)";

    /// <summary>Opacity of the scrim.</summary>
    [CssVar(DialogPanel.ScrimOpacity)] public string ScrimOpacity { get; init; } = "0.32";

    /// <summary>Elevation (box-shadow) of the dialog.</summary>
    [CssVar(DialogPanel.Elevation)] public string Elevation { get; init; } = "var(--flare-elevation-3)";

    /// <summary>Color of the dialog title text.</summary>
    [CssVar(DialogPanel.TitleColor)] public string TitleColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontFamily)] public string TitleFontFamily { get; init; } = "var(--flare-typescale-headline-small-font)";

    /// <summary>Font size of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontSize)] public string TitleFontSize { get; init; } = "var(--flare-typescale-headline-small-size)";

    /// <summary>Font weight of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontWeight)] public string TitleFontWeight { get; init; } = "var(--flare-typescale-headline-small-weight)";

    /// <summary>Color of the dialog content text.</summary>
    [CssVar(DialogPanel.ContentColor)] public string ContentColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Font family of the dialog content.</summary>
    [CssVar(DialogPanel.ContentFontFamily)] public string ContentFontFamily { get; init; } = "var(--flare-typescale-body-medium-font)";

    /// <summary>Font size of the dialog content.</summary>
    [CssVar(DialogPanel.ContentFontSize)] public string ContentFontSize { get; init; } = "var(--flare-typescale-body-medium-size)";

    /// <summary>Transition duration for dialog open/close.</summary>
    [CssVar(DialogPanel.TransitionDuration)] public string TransitionDuration { get; init; } = "var(--flare-motion-duration-medium2)";

    /// <summary>Transition easing for dialog open/close.</summary>
    [CssVar(DialogPanel.TransitionEasing)] public string TransitionEasing { get; init; } = "var(--flare-motion-easing-standard)";

    /// <summary>Width for the xs size variant.</summary>
    [CssVar(DialogPanel.SizeXsWidth)] public string SizeXsWidth { get; init; } = "320px";

    /// <summary>Width for the sm size variant.</summary>
    [CssVar(DialogPanel.SizeSmWidth)] public string SizeSmWidth { get; init; } = "400px";

    /// <summary>Width for the md size variant (default).</summary>
    [CssVar(DialogPanel.SizeMdWidth)] public string SizeMdWidth { get; init; } = "560px";

    /// <summary>Width for the lg size variant.</summary>
    [CssVar(DialogPanel.SizeLgWidth)] public string SizeLgWidth { get; init; } = "720px";

    /// <summary>Width for the xl size variant.</summary>
    [CssVar(DialogPanel.SizeXlWidth)] public string SizeXlWidth { get; init; } = "880px";

    /// <summary>Width for the full size variant.</summary>
    [CssVar(DialogPanel.SizeFullWidth)] public string SizeFullWidth { get; init; } = "100%";
}
