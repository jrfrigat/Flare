namespace Flare.Components;

/// <summary>
/// Host for the message boxes raised through <c>IMessageBoxService</c>. Placed once in the layout, it
/// renders whichever alert, confirmation or prompt is currently open, so calling code awaits an answer
/// instead of managing a dialog.
/// </summary>
public partial class FlareMessageBoxProvider;
