namespace Flare.Abstractions;

/// <summary>
/// A handle to an open component dialog returned by <see cref="IDialogService.Show{TComponent}"/>.
/// Await <see cref="Result"/> for its outcome, or close it programmatically from the caller side.
/// </summary>
public sealed class DialogReference
{
    private readonly FlareDialogInstance _instance;

    internal DialogReference(FlareDialogInstance instance) => _instance = instance;

    /// <summary>The dialog's unique id.</summary>
    public Guid Id => _instance.Id;

    /// <summary>The live instance handle (also cascaded to the dialog body).</summary>
    public FlareDialogInstance Instance => _instance;

    /// <summary>Completes when the dialog closes, with the user's <see cref="DialogResult"/>.</summary>
    public Task<DialogResult> Result => _instance.ResultTask;

    /// <summary>Programmatically closes the dialog with the given result.</summary>
    /// <param name="result">The outcome to resolve the dialog with.</param>
    public void Close(DialogResult result) => _instance.Close(result);

    /// <summary>Programmatically cancels (dismisses) the dialog.</summary>
    public void Cancel() => _instance.Cancel();
}
