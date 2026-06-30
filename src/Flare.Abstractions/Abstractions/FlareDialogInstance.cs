namespace Flare.Abstractions;

/// <summary>
/// The runtime handle for one open component dialog. The dialog provider cascades it to the dialog
/// body as a <c>[CascadingParameter]</c>, so the body can close itself and return a result:
/// <code>
/// [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
/// // ...
/// Dialog.Close(editedModel); // confirm with a payload
/// Dialog.Cancel();           // dismiss
/// </code>
/// </summary>
public sealed class FlareDialogInstance
{
    private readonly TaskCompletionSource<DialogResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Action<FlareDialogInstance> _onClose;
    private Dictionary<string, object>? _componentParameters;
    private int _closed;

    internal FlareDialogInstance(
        Guid id,
        Type contentType,
        string? title,
        DialogParameters parameters,
        DialogOptions options,
        Action<FlareDialogInstance> onClose)
    {
        Id = id;
        ContentType = contentType;
        Title = title;
        Parameters = parameters;
        Options = options;
        _onClose = onClose;
    }

    /// <summary>The unique id of this open dialog.</summary>
    public Guid Id { get; }

    /// <summary>The component type rendered as the dialog body.</summary>
    public Type ContentType { get; }

    /// <summary>The dialog title shown in the header, or null for a header-less dialog.</summary>
    public string? Title { get; }

    /// <summary>The parameters bound to the body component.</summary>
    public DialogParameters Parameters { get; }

    /// <summary>The presentation options applied to this dialog.</summary>
    public DialogOptions Options { get; }

    /// <summary>Whether the dialog has already been closed (its result is settled).</summary>
    public bool IsClosed => _closed != 0;

    // The awaitable surfaced through DialogReference.Result.
    internal Task<DialogResult> ResultTask => _completion.Task;

    // Cached parameter snapshot for DynamicComponent; built once since parameters are fixed per show.
    internal Dictionary<string, object> ComponentParameters =>
        _componentParameters ??= Parameters.ToComponentParameters();

    /// <summary>Closes the dialog, resolving the awaiting caller with <paramref name="result"/>.</summary>
    /// <param name="result">The outcome to return to the caller.</param>
    public void Close(DialogResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        // First close wins; later Close/Cancel calls (e.g. a queued scrim click) are ignored.
        if (Interlocked.Exchange(ref _closed, 1) != 0)
            return;
        _completion.TrySetResult(result);
        _onClose(this);
    }

    /// <summary>Closes the dialog with a confirmed result that carries no payload.</summary>
    public void Close() => Close(DialogResult.Ok());

    /// <summary>Closes the dialog with a confirmed, strongly-typed payload.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="returnValue">The value to return to the caller.</param>
    public void Close<T>(T returnValue) => Close(DialogResult.Ok(returnValue));

    /// <summary>Cancels the dialog, resolving the awaiting caller with a cancelled result.</summary>
    public void Cancel() => Close(DialogResult.Cancel());
}
