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

    /// <summary>Shared, immutable-by-convention default options used when a caller supplies none.</summary>
    public static DialogOptions Default { get; } = new();
}
