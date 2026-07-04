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
    [CssVar(DialogPanel.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Border radius of the dialog.</summary>
    [CssVar(DialogPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Maximum width of the dialog.</summary>
    [CssVar(DialogPanel.MaxWidth)] public required string MaxWidth { get; init; }

    /// <summary>Minimum width of the dialog.</summary>
    [CssVar(DialogPanel.MinWidth)] public required string MinWidth { get; init; }

    /// <summary>Padding inside the dialog.</summary>
    [CssVar(DialogPanel.Padding)] public required string Padding { get; init; }

    /// <summary>Padding between header and content.</summary>
    [CssVar(DialogPanel.HeaderPadding)] public required string HeaderPadding { get; init; }

    /// <summary>Padding between content and actions.</summary>
    [CssVar(DialogPanel.ActionsPadding)] public required string ActionsPadding { get; init; }

    /// <summary>Gap between action buttons.</summary>
    [CssVar(DialogPanel.ActionsGap)] public required string ActionsGap { get; init; }

    /// <summary>Background color of the scrim (overlay).</summary>
    [CssVar(DialogPanel.ScrimColor)] public required string ScrimColor { get; init; }

    /// <summary>Opacity of the scrim.</summary>
    [CssVar(DialogPanel.ScrimOpacity)] public required string ScrimOpacity { get; init; }

    /// <summary>Elevation (box-shadow) of the dialog.</summary>
    [CssVar(DialogPanel.Elevation)] public required string Elevation { get; init; }

    /// <summary>Color of the dialog title text.</summary>
    [CssVar(DialogPanel.TitleColor)] public required string TitleColor { get; init; }

    /// <summary>Font family of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontFamily)] public required string TitleFontFamily { get; init; }

    /// <summary>Font size of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontSize)] public required string TitleFontSize { get; init; }

    /// <summary>Font weight of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFontWeight)] public required string TitleFontWeight { get; init; }

    /// <summary>Color of the dialog content text.</summary>
    [CssVar(DialogPanel.ContentColor)] public required string ContentColor { get; init; }

    /// <summary>Font family of the dialog content.</summary>
    [CssVar(DialogPanel.ContentFontFamily)] public required string ContentFontFamily { get; init; }

    /// <summary>Font size of the dialog content.</summary>
    [CssVar(DialogPanel.ContentFontSize)] public required string ContentFontSize { get; init; }

    /// <summary>Transition duration for dialog open/close.</summary>
    [CssVar(DialogPanel.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for dialog open/close.</summary>
    [CssVar(DialogPanel.TransitionEasing)] public required string TransitionEasing { get; init; }

    /// <summary>Width for the xs size variant.</summary>
    [CssVar(DialogPanel.SizeXsWidth)] public required string SizeXsWidth { get; init; }

    /// <summary>Width for the sm size variant.</summary>
    [CssVar(DialogPanel.SizeSmWidth)] public required string SizeSmWidth { get; init; }

    /// <summary>Width for the md size variant (default).</summary>
    [CssVar(DialogPanel.SizeMdWidth)] public required string SizeMdWidth { get; init; }

    /// <summary>Width for the lg size variant.</summary>
    [CssVar(DialogPanel.SizeLgWidth)] public required string SizeLgWidth { get; init; }

    /// <summary>Width for the xl size variant.</summary>
    [CssVar(DialogPanel.SizeXlWidth)] public required string SizeXlWidth { get; init; }

    /// <summary>Width for the full size variant.</summary>
    [CssVar(DialogPanel.SizeFullWidth)] public required string SizeFullWidth { get; init; }
}
