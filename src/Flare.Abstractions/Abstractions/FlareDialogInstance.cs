using System.Diagnostics.CodeAnalysis;
namespace Flare.Abstractions;

/// <summary>
/// The runtime handle for one open component dialog. The dialog provider cascades it to the dialog
/// body as a <c>[CascadingParameter]</c>, so the body can close itself and return a result:
/// <code>
/// [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
/// // ...
/// await Dialog.CloseAsync(editedModel); // confirm with a payload
/// await Dialog.CancelAsync();           // dismiss
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentType,
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
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type ContentType { get; }

    /// <summary>The dialog title shown in the header, or null for a header-less dialog.</summary>
    public string? Title { get; }

    /// <summary>The parameters bound to the body component.</summary>
    public DialogParameters Parameters { get; }

    /// <summary>The presentation options applied to this dialog.</summary>
    public DialogOptions Options { get; }

    /// <summary>The close guards shared by this handle and its displayed dialog.</summary>
    public DialogCloseContext CloseContext { get; } = new();

    /// <summary>Whether the dialog has already been closed (its result is settled).</summary>
    public bool IsClosed => _closed != 0;

    // The awaitable surfaced through DialogReference.Result.
    internal Task<DialogResult> ResultTask => _completion.Task;

    // Cached parameter snapshot for DynamicComponent; built once since parameters are fixed per show.
    internal Dictionary<string, object> ComponentParameters =>
        _componentParameters ??= Parameters.ToComponentParameters();

    /// <summary>Checks the close guards and, if allowed, resolves the caller with <paramref name="result"/>.
    /// Returns false when vetoed, already closed, or another close request is pending.</summary>
    /// <param name="result">The outcome to return to the caller.</param>
    /// <returns>True if this request closed the dialog.</returns>
    public Task<bool> CloseAsync(DialogResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return IsClosed ? Task.FromResult(false) : CloseContext.TryCloseAsync(
            DialogCloseReason.Programmatic, Options.BeforeClose,
            () => ValueTask.FromResult(CompleteClose(result)));
    }

    internal bool CompleteClose(DialogResult result)
    {
        // First accepted close wins; a queued event cannot replace its result.
        if (Interlocked.Exchange(ref _closed, 1) != 0)
            return false;
        _completion.TrySetResult(result);
        _onClose(this);
        return true;
    }

    /// <summary>Checks the guards and closes with a confirmed result without a payload.
    /// Returns false if closing is vetoed, pending, or already completed.</summary>
    public Task<bool> CloseAsync() => CloseAsync(DialogResult.Ok());

    /// <summary>Checks the guards and closes with a strongly-typed payload.
    /// Returns false if closing is vetoed, pending, or already completed.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="returnValue">The value to return to the caller.</param>
    public Task<bool> CloseAsync<T>(T returnValue) => CloseAsync(DialogResult.Ok(returnValue));

    /// <summary>Checks the guards and resolves the caller with a cancelled result.
    /// Returns false if closing is vetoed, pending, or already completed.</summary>
    public Task<bool> CancelAsync() => CloseAsync(DialogResult.Cancel());
}
