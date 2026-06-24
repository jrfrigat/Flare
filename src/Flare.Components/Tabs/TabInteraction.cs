namespace Flare.Components;

/// <summary>The kind of tab interaction reported to <c>FlareTabs.OnPreviewInteraction</c>.</summary>
public enum TabInteractionType
{
    /// <summary>The user is about to activate (switch to) a tab.</summary>
    Activate,
    /// <summary>The user is about to close a closeable tab.</summary>
    Close,
}

/// <summary>
/// Mutable event payload passed to <c>FlareTabs.OnPreviewInteraction</c> before a tab is activated
/// or closed. Set <see cref="Cancel"/> to <c>true</c> in the handler to veto the interaction (for
/// example to keep the current tab open while there are unsaved changes).
/// </summary>
public sealed class TabInteractionEventArgs
{
    /// <summary>Zero-based index of the tab targeted by the interaction.</summary>
    public int PanelIndex { get; init; }

    /// <summary>The label of the tab targeted by the interaction.</summary>
    public string Label { get; init; } = "";

    /// <summary>What the user is attempting to do (activate or close).</summary>
    public TabInteractionType InteractionType { get; init; }

    /// <summary>Set to <c>true</c> to cancel the interaction; the active tab is left unchanged.</summary>
    public bool Cancel { get; set; }
}
