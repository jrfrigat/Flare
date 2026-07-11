namespace Flare.Abstractions;

/// <summary>Why a dialog is being dismissed - passed to a <c>BeforeClose</c> guard so it can decide
/// whether to allow or veto the close (e.g. block a scrim click while there are unsaved changes).</summary>
public enum DialogCloseReason
{
    /// <summary>The user clicked the scrim/backdrop behind the dialog.</summary>
    ScrimClick,
    /// <summary>The user pressed the Escape key.</summary>
    EscapeKey,
    /// <summary>The user clicked the built-in header close button.</summary>
    CloseButton,
}
