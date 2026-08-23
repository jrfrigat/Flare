namespace Flare.Components;

/// <summary>
/// Host for the confirmation dialogs raised through <c>IConfirmDialogService</c>. It cascades itself, so
/// anything beneath it can take the service as a cascading parameter and await a yes/no answer instead
/// of wiring up a dialog and its state.
/// </summary>
public partial class FlareConfirmDialogProvider;
