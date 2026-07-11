namespace Flare.Abstractions;

/// <summary>
/// Presentation options for a component dialog opened via
/// <see cref="IDialogService.ShowAsync{TComponent}"/> / <see cref="IDialogService.Show{TComponent}"/>.
/// </summary>
public sealed class DialogOptions
{
    /// <summary>The maximum-width preset. Defaults to <see cref="DialogSize.Md"/>.</summary>
    public DialogSize Size { get; set; } = DialogSize.Md;

    /// <summary>
    /// Where the dialog is anchored. Defaults to <see cref="DialogPosition.Center"/>.
    /// Use <see cref="DialogPosition.Bottom"/> (or the <see cref="IDialogService.ShowSheetAsync{TComponent}"/>
    /// helper) to present the dialog as a slide-up bottom sheet.
    /// </summary>
    public DialogPosition Position { get; set; } = DialogPosition.Center;

    /// <summary>
    /// Shows a drag-grabber handle at the top of the panel. Meaningful for
    /// <see cref="DialogPosition.Bottom"/> sheets; ignored for centered dialogs. Default
    /// <see langword="false"/> (the <see cref="IDialogService.ShowSheetAsync{TComponent}"/> helper turns it on).
    /// </summary>
    public bool ShowGrabber { get; set; }

    /// <summary>
    /// Extra CSS class(es) applied to the dialog panel, so an app can skin a specific dialog
    /// (e.g. a glass surface or a bespoke sheet) without hand-writing global CSS. Null for none.
    /// </summary>
    public string? PanelClass { get; set; }

    /// <summary>
    /// Extra CSS class(es) applied to the dialog scrim/backdrop. Null for none.
    /// </summary>
    public string? ScrimClass { get; set; }

    /// <summary>
    /// Accessible name applied (as <c>aria-label</c>) when the dialog has no title. Set this for
    /// header-less component dialogs so the modal is not left without an accessible name.
    /// </summary>
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Dismisses the dialog (resolving as <see cref="DialogResult.Cancel"/>) when the scrim backdrop
    /// is clicked. Default <see langword="true"/>.
    /// </summary>
    public bool CloseOnScrimClick { get; set; } = true;

    /// <summary>
    /// Dismisses the dialog (resolving as <see cref="DialogResult.Cancel"/>) when the Escape key is
    /// pressed. Default <see langword="true"/>.
    /// </summary>
    public bool CloseOnEsc { get; set; } = true;

    /// <summary>Shows a divider between the title header and the body. Default <see langword="false"/>.</summary>
    public bool Divider { get; set; }

    /// <summary>Shows a built-in close (X) button in the top corner of the panel. Default <see langword="false"/>.</summary>
    public bool ShowCloseButton { get; set; }

    /// <summary>Dismisses the dialog automatically when the app navigates to a new route. Default <see langword="true"/>.</summary>
    public bool CloseOnNavigation { get; set; } = true;

    /// <summary>Lets the user move the dialog by dragging its header. Default <see langword="false"/>.</summary>
    public bool Draggable { get; set; }

    /// <summary>Lets the user resize the dialog from a bottom-right gripper. Default <see langword="false"/>.</summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// Optional guard consulted before a scrim / Escape / close-button dismissal. Return
    /// <see langword="true"/> to allow the close or <see langword="false"/> to veto it (e.g. to keep the
    /// dialog open while there are unsaved changes). A programmatic close via the dialog handle bypasses it.
    /// </summary>
    public Func<DialogCloseReason, ValueTask<bool>>? BeforeClose { get; set; }

    /// <summary>Shared, immutable-by-convention default options used when a caller supplies none.</summary>
    public static DialogOptions Default { get; } = new();
}
