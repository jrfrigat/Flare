namespace Flare.Components.IDE;

/// <summary>How <see cref="FlareIdeLayout"/> mounts the content of docked documents.</summary>
public enum IdeDocumentMounting
{
    /// <summary>Every document in <see cref="FlareIdeLayout.Documents"/> invokes its content and stays
    /// mounted while it exists, hidden when inactive. State is preserved at one component instance per
    /// document.</summary>
    All,

    /// <summary>Only the active document of each region - plus documents retained by
    /// <see cref="FlareIdeLayout.RetainDocumentContent"/> - invokes its content. An inactive document's
    /// content is disposed, so its transient state is lost until it becomes active again. Keeps the cost
    /// of expensive editors proportional to the visible documents.</summary>
    ActiveOnly
}
