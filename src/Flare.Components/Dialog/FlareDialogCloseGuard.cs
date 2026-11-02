using Flare.Abstractions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components;

/// <summary>Confirms closing a dialog with unsaved changes. Place inside a dialog, and inside its
/// EditForm to use EditContext.IsModified automatically. After saving, clear HasUnsavedChanges or
/// call EditContext.MarkAsUnmodified before closing. Requires a FlareDialogProvider for the default prompt.</summary>
public sealed class FlareDialogCloseGuard : ComponentBase, IDisposable
{
    /// <summary>Explicit dirty state. Null reads the surrounding EditContext; false disables this guard.</summary>
    [Parameter] public bool? HasUnsavedChanges { get; set; }
    /// <summary>Confirmation title. Null uses the localized default.</summary>
    [Parameter] public string? Title { get; set; }
    /// <summary>Confirmation message. Null uses the localized default.</summary>
    [Parameter] public string? Message { get; set; }
    /// <summary>Label for discarding changes. Null uses the localized default.</summary>
    [Parameter] public string? ConfirmLabel { get; set; }
    /// <summary>Label for continuing to edit. Null uses the localized default.</summary>
    [Parameter] public string? CancelLabel { get; set; }
    /// <summary>Replaces the default prompt for a dirty form. Return true to permit closing or
    /// false to keep editing. Clean forms do not call this handler.</summary>
    [Parameter] public Func<DialogCloseReason, ValueTask<bool>>? ConfirmClose { get; set; }
    /// <summary>The surrounding dialog's shared close context.</summary>
    [CascadingParameter] public DialogCloseContext? DialogContext { get; set; }
    /// <summary>The surrounding form whose modification state is used when HasUnsavedChanges is null.</summary>
    [CascadingParameter] public EditContext? EditContext { get; set; }

    [Inject] private IDialogService Dialogs { get; set; } = default!;
    private readonly CancellationTokenSource _lifetime = new();
    private DialogCloseContext? _registeredContext;
    private IDisposable? _registration;
    private DialogReference? _confirmation;
    private bool _disposed;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (DialogContext is null)
            throw new InvalidOperationException("FlareDialogCloseGuard must be placed inside a FlareDialog.");
        if (ReferenceEquals(DialogContext, _registeredContext)) return;
        _registration?.Dispose();
        _registeredContext = DialogContext;
        _registration = DialogContext.RegisterGuard(CheckAsync);
    }

    private async ValueTask<bool> CheckAsync(DialogCloseReason reason)
    {
        var allowed = false;
        await InvokeAsync(async () =>
        {
            if (_disposed) return;
            if (!(HasUnsavedChanges ?? EditContext?.IsModified() ?? false))
            {
                allowed = true;
                return;
            }
            try
            {
                if (ConfirmClose is not null)
                    allowed = await ConfirmClose(reason).AsTask().WaitAsync(_lifetime.Token);
                else
                {
                    var parameters = new DialogParameters()
                        .Add(nameof(DialogCloseConfirmation.Message), Message ?? FlareStrings.DialogGuard_Message)
                        .Add(nameof(DialogCloseConfirmation.ConfirmLabel), ConfirmLabel ?? FlareStrings.DialogGuard_Discard)
                        .Add(nameof(DialogCloseConfirmation.CancelLabel), CancelLabel ?? FlareStrings.DialogGuard_KeepEditing);
                    _confirmation = Dialogs.Show<DialogCloseConfirmation>(Title ?? FlareStrings.DialogGuard_Title,
                        parameters, new DialogOptions { Size = DialogSize.Sm, CloseOnNavigation = false });
                    var result = await _confirmation.Result.WaitAsync(_lifetime.Token);
                    allowed = !result.Cancelled;
                }
            }
            catch (OperationCanceledException) when (_disposed) { }
            finally { _confirmation = null; }
        });
        return !_disposed && allowed;
    }

    /// <summary>Unregisters the guard and dismisses only the confirmation owned by this guard.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _registration?.Dispose();
        var confirmation = _confirmation;
        _lifetime.Cancel();
        confirmation?.Instance.CompleteClose(DialogResult.Cancel());
        _confirmation = null;
        _lifetime.Dispose();
    }
}
