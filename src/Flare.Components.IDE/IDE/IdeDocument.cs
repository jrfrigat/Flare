namespace Flare.Components.IDE;

/// <summary>A document placed in a region of an IDE layout.</summary>
/// <param name="Key">Stable key, unique within the layout.</param>
/// <param name="Title">Document title displayed in the tab strip.</param>
/// <param name="Region">Current region.</param>
/// <param name="Closeable">Whether the layout offers a close button for the document's dock tab.</param>
/// <param name="Modified">Whether the document has unsaved changes; the dock tab marks it and announces it.</param>
public sealed record IdeDocument(
    string Key,
    string Title,
    IdeDocumentRegion Region = IdeDocumentRegion.Center,
    bool Closeable = true,
    bool Modified = false);

/// <summary>Region in which an IDE document can be displayed.</summary>
public enum IdeDocumentRegion
{
    /// <summary>The main document area.</summary>
    Center,
    /// <summary>The left tool area.</summary>
    Left,
    /// <summary>The right tool area.</summary>
    Right
}

/// <summary>A request to move a document to another region.</summary>
/// <param name="Document">The document in its current region.</param>
/// <param name="Destination">The requested region.</param>
public sealed record IdeDocumentMove(IdeDocument Document, IdeDocumentRegion Destination);

/// <summary>Selection in a document region.</summary>
/// <param name="Region">Region in which the selection changed.</param>
/// <param name="Key">Selected document key, or null for the region's tool content.</param>
public sealed record IdeDocumentSelection(IdeDocumentRegion Region, string? Key);
